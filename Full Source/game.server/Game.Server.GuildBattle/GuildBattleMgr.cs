using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GuildBattle.Action;
using Game.Server.Managers;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GuildBattle;

[Serializable]
public class GuildBattleMgr
{
	[NonSerialized]
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ArrayList m_actions;

	private long m_passTick = 0L;

	public int CurrentActionCount = 0;

	private long m_waitTimer = 0L;

	public GuildBattleState State;

	public DateTime TimeStart;

	public DateTime TimeStop;

	public readonly DateTime GuildBattleStartTime = DateTime.Parse(GameProperties.GuildBattleStartTime);

	public readonly List<Point> GuildPointDefault = new List<Point>
	{
		new Point(649, 954),
		new Point(988, 584),
		new Point(1643, 611),
		new Point(2359, 600),
		new Point(2503, 987),
		new Point(2086, 1512),
		new Point(1599, 1445),
		new Point(868, 1362)
	};

	private Dictionary<int, UserGuildBattleInfo> m_listPlayer = new Dictionary<int, UserGuildBattleInfo>();

	private Dictionary<int, GuildBattleConsortiaInfo> m_listConsortia = new Dictionary<int, GuildBattleConsortiaInfo>();

	private object m_object = new object();

	public bool IsOpen { get; set; }

	public bool IsSendMail { get; set; }

	public bool IsSendAward { get; set; }

	public DateTime LastDateReloadTop { get; set; }

	public GuildBattleMgr()
	{
		m_actions = new ArrayList();
		State = GuildBattleState.CLOSE;
		m_listPlayer = new Dictionary<int, UserGuildBattleInfo>();
		m_listConsortia = new Dictionary<int, GuildBattleConsortiaInfo>();
		IsSendMail = false;
		IsSendAward = false;
		TimeStart = DateTime.Now;
		TimeStop = DateTime.Now;
	}

	public void SaveCurrentRankToDatabase(bool clear)
	{
		using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
		if (clear)
		{
			consortiaBussiness.RemoveAllConsortiaBattleRank();
			consortiaBussiness.RemoveAllConsortiaBattlePlayerRank();
		}
		GuildBattleConsortiaInfo[] array = (from a in GetAllConsortia()
			orderby a.Score descending
			select a).ToArray();
		UserGuildBattleInfo[] array2 = (from a in GetAllUser()
			orderby a.Score descending
			select a).ToArray();
		int num = 1;
		GuildBattleConsortiaInfo[] array3 = array;
		foreach (GuildBattleConsortiaInfo guildBattleConsortiaInfo in array3)
		{
			ConsortiaWarRankInfo consortiaWarRankInfo = new ConsortiaWarRankInfo();
			consortiaWarRankInfo.ConsortiaID = guildBattleConsortiaInfo.ConsortiaID;
			consortiaWarRankInfo.ConsortiaName = guildBattleConsortiaInfo.ConsortiaName;
			consortiaWarRankInfo.Rank = num;
			consortiaWarRankInfo.Score = guildBattleConsortiaInfo.Score;
			consortiaWarRankInfo.TimeCreate = DateTime.Now;
			consortiaBussiness.AddConsortiaBattleRank(consortiaWarRankInfo);
			num++;
		}
		UserGuildBattleInfo[] array4 = array2;
		foreach (UserGuildBattleInfo userGuildBattleInfo in array4)
		{
			ConsortiaWarPlayerRankInfo consortiaWarPlayerRankInfo = new ConsortiaWarPlayerRankInfo();
			consortiaWarPlayerRankInfo.UserID = userGuildBattleInfo.UserID;
			consortiaWarPlayerRankInfo.NickName = userGuildBattleInfo.NickName;
			consortiaWarPlayerRankInfo.ZoneID = GameServer.Instance.Configuration.AreaId;
			consortiaWarPlayerRankInfo.ZoneName = GameServer.Instance.Configuration.AreaName;
			consortiaWarPlayerRankInfo.ConsortiaID = userGuildBattleInfo.ConsortiaID;
			consortiaWarPlayerRankInfo.Score = userGuildBattleInfo.Score;
			consortiaWarPlayerRankInfo.TimeCreate = DateTime.Now;
			consortiaBussiness.AddConsortiaBattlePlayerRank(consortiaWarPlayerRankInfo);
		}
		if (clear)
		{
			m_listConsortia = new Dictionary<int, GuildBattleConsortiaInfo>();
			m_listPlayer = new Dictionary<int, UserGuildBattleInfo>();
		}
	}

	public void ChangeOpenClose(bool _isOpen)
	{
		ChangeOpenClose(_isOpen, DateTime.Now);
	}

	public void ChangeOpenClose(bool _isOpen, DateTime timeEnd)
	{
		if (_isOpen)
		{
			State = GuildBattleState.OPEN;
			IsSendAward = false;
			m_listPlayer = new Dictionary<int, UserGuildBattleInfo>();
			if (LastDateReloadTop.Date != DateTime.Now.Date)
			{
				InstallConsortiaList(GetTOPConsortiaDayOnline());
			}
		}
		else
		{
			State = GuildBattleState.CLOSE;
			IsSendMail = false;
		}
		IsOpen = _isOpen;
		TimeStart = DateTime.Now;
		TimeStop = timeEnd;
		SendAllOpenClose();
	}

	public void RemoveAllPlayerInRoom()
	{
		lock (m_listPlayer)
		{
			foreach (UserGuildBattleInfo value in m_listPlayer.Values)
			{
				if (value != null && value.IsActive && value.Player != null && value.Player.CurrentRoom != null)
				{
					value.Player.CurrentRoom.RemovePlayerUnsafe(value.Player);
				}
				value.IsActive = false;
				value.Player = null;
			}
		}
	}

	public ConsortiaInfo[] GetTOPConsortiaWeekRiches()
	{
		ConsortiaInfo[] result = null;
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			result = consortiaBussiness.GetRankWeekRichesConsortias();
		}
		return result;
	}

	public ConsortiaInfo[] GetTOPConsortiaDayRiches()
	{
		ConsortiaInfo[] result = null;
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			result = consortiaBussiness.GetRankDayRichesConsortias();
		}
		return result;
	}

	public ConsortiaInfo[] GetTOPConsortiaDayOnline()
	{
		ConsortiaInfo[] result = null;
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			result = consortiaBussiness.GetRankDayOnlineConsortias();
		}
		return result;
	}

	public string InstallConsortiaList(ConsortiaInfo[] list)
	{
		List<string> list2 = new List<string>();
		lock (m_listConsortia)
		{
			m_listConsortia.Clear();
			int num = 0;
			foreach (ConsortiaInfo consortiaInfo in list)
			{
				if (!m_listConsortia.ContainsKey(consortiaInfo.ConsortiaID))
				{
					GuildBattleConsortiaInfo guildBattleConsortiaInfo = new GuildBattleConsortiaInfo();
					guildBattleConsortiaInfo.ConsortiaID = consortiaInfo.ConsortiaID;
					guildBattleConsortiaInfo.ConsortiaName = consortiaInfo.ConsortiaName;
					guildBattleConsortiaInfo.Rank = 0;
					guildBattleConsortiaInfo.Score = 0;
					guildBattleConsortiaInfo.DefaultPoint = GuildPointDefault[num];
					m_listConsortia.Add(guildBattleConsortiaInfo.ConsortiaID, guildBattleConsortiaInfo);
					list2.Add(consortiaInfo.ConsortiaName);
					num++;
				}
			}
		}
		LastDateReloadTop = DateTime.Now;
		return string.Join(",", list2.ToArray());
	}

	public bool AddPlayer(GamePlayer p)
	{
		bool flag = false;
		GuildBattleConsortiaInfo guildBattleConsortiaInfo = FindConsortia(p.PlayerCharacter.ConsortiaID);
		if (p.PlayerCharacter.ConsortiaID != 0 && guildBattleConsortiaInfo != null && State == GuildBattleState.OPEN)
		{
			UserGuildBattleInfo userGuildBattleInfo = FindUser(p.PlayerId);
			if (userGuildBattleInfo != null)
			{
				userGuildBattleInfo.NickName = p.PlayerCharacter.NickName;
				userGuildBattleInfo.IsActive = true;
				userGuildBattleInfo.Player = p;
				flag = true;
			}
			else
			{
				userGuildBattleInfo = new UserGuildBattleInfo(p);
				flag = AddUser(userGuildBattleInfo);
			}
			userGuildBattleInfo.Postion = guildBattleConsortiaInfo.DefaultPoint;
			if (p.PlayerCharacter.ReduceStartBlood <= 0)
			{
				p.PlayerCharacter.ReduceStartBlood = p.PlayerCharacter.hp;
			}
			if (flag)
			{
				SendInitSeftInfo(userGuildBattleInfo);
			}
		}
		return flag;
	}

	public void RemovePlayer(GamePlayer p)
	{
		UserGuildBattleInfo userGuildBattleInfo = FindUser(p.PlayerId);
		if (userGuildBattleInfo != null)
		{
			RemovePlayer(userGuildBattleInfo);
		}
	}

	public void RemovePlayer(UserGuildBattleInfo p)
	{
		SendRemovePlayer(p);
		p.IsActive = false;
		p.WinStreak = 0;
		p.FailBuffCount = 0;
		p.LostCount = 0;
		p.Status = UserGuildBattleStatus.NORMAL;
	}

	public void ChallengeGame(GamePlayer p1, GamePlayer p2)
	{
		lock (m_object)
		{
			if (p1.CurrentRoom != null && p2.CurrentRoom != null && p1.CurrentRoom.RoomType == eRoomType.ConsortiaBattle && p2.CurrentRoom.RoomType == eRoomType.ConsortiaBattle && !p1.CurrentRoom.IsPlaying && !p2.CurrentRoom.IsPlaying)
			{
				UserGuildBattleInfo userGuildBattleInfo = FindUser(p1.PlayerId);
				UserGuildBattleInfo userGuildBattleInfo2 = FindUser(p2.PlayerId);
				if (userGuildBattleInfo != null && userGuildBattleInfo2 != null && userGuildBattleInfo.Status == UserGuildBattleStatus.NORMAL && userGuildBattleInfo2.Status == UserGuildBattleStatus.NORMAL && !userGuildBattleInfo.IsDead && !userGuildBattleInfo2.IsDead)
				{
					userGuildBattleInfo.Status = UserGuildBattleStatus.FIGHTING;
					userGuildBattleInfo2.Status = UserGuildBattleStatus.FIGHTING;
					if (userGuildBattleInfo.FailBuffCount >= 1)
					{
						userGuildBattleInfo.Player.PlayerCharacter.AgiPlusGuildBattle = 30;
						userGuildBattleInfo.Player.PlayerCharacter.AttPlusGuildBattle = 30;
					}
					else
					{
						userGuildBattleInfo.Player.PlayerCharacter.AgiPlusGuildBattle = 0;
						userGuildBattleInfo.Player.PlayerCharacter.AttPlusGuildBattle = 0;
					}
					if (userGuildBattleInfo2.FailBuffCount >= 1)
					{
						userGuildBattleInfo2.Player.PlayerCharacter.AgiPlusGuildBattle = 30;
						userGuildBattleInfo2.Player.PlayerCharacter.AttPlusGuildBattle = 30;
					}
					else
					{
						userGuildBattleInfo2.Player.PlayerCharacter.AgiPlusGuildBattle = 0;
						userGuildBattleInfo2.Player.PlayerCharacter.AttPlusGuildBattle = 0;
					}
					SendUpdatePlayerStatus(userGuildBattleInfo);
					SendUpdatePlayerStatus(userGuildBattleInfo2);
					RoomMgr.StartConsortiaBattle(p1.CurrentRoom, p2.CurrentRoom);
				}
				else
				{
					p1.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.RoomStillWaiting"));
				}
			}
			else
			{
				p1.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.RoomStillWaiting"));
			}
		}
	}

	public void UpdateScoreMatch(int winId, int lostId)
	{
		UserGuildBattleInfo userGuildBattleInfo = FindUser(winId);
		UserGuildBattleInfo userGuildBattleInfo2 = FindUser(lostId);
		if (userGuildBattleInfo != null && userGuildBattleInfo2 != null && (!userGuildBattleInfo.IsActive || userGuildBattleInfo.Status == UserGuildBattleStatus.FIGHTING) && (!userGuildBattleInfo2.IsActive || userGuildBattleInfo2.Status == UserGuildBattleStatus.FIGHTING))
		{
			userGuildBattleInfo.Status = UserGuildBattleStatus.NORMAL;
			userGuildBattleInfo2.Status = UserGuildBattleStatus.NORMAL;
			int winStreak = userGuildBattleInfo2.WinStreak;
			if (winStreak >= 3)
			{
				SendBroadcastMsg(2, winStreak, userGuildBattleInfo.NickName, userGuildBattleInfo2.NickName);
			}
			userGuildBattleInfo.WinStreak++;
			userGuildBattleInfo.VictoryCount++;
			userGuildBattleInfo.FailBuffCount = 0;
			userGuildBattleInfo.LostCount = 0;
			userGuildBattleInfo2.WinStreak = 0;
			userGuildBattleInfo2.LostCount++;
			if (userGuildBattleInfo2.LostCount >= 3)
			{
				userGuildBattleInfo2.FailBuffCount = 1;
			}
			if (userGuildBattleInfo.WinStreak == 3 || userGuildBattleInfo.WinStreak == 6 || userGuildBattleInfo.WinStreak >= 10)
			{
				SendBroadcastMsg(1, winStreak, userGuildBattleInfo.NickName, userGuildBattleInfo2.NickName);
			}
			AddScore(userGuildBattleInfo, winStreak);
		}
	}

	public void AddScore(UserGuildBattleInfo p, int endStack)
	{
		int num = 0;
		num = ((p.WinStreak >= 3 && p.WinStreak < 6) ? (num + 50) : ((p.WinStreak >= 6 && p.WinStreak < 10) ? (num + 70) : ((p.WinStreak < 10) ? (num + 30) : (num + 110))));
		if (endStack >= 6 && endStack < 10)
		{
			num += 50;
		}
		else if (endStack == 10)
		{
			num += 70;
		}
		else if (endStack > 10)
		{
			num += 90;
		}
		if (p.DupeScoreConsortiaBattle)
		{
			num *= 2;
		}
		p.Score += num;
		FindConsortia(p.ConsortiaID).Score += num;
	}

	private void SendBroadcastMsg(byte type, int winStreak, string winName, string lostName)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(19);
		gSPacketIn.WriteByte(type);
		if (type == 1)
		{
			gSPacketIn.WriteInt(winStreak);
			gSPacketIn.WriteString(winName);
		}
		else
		{
			gSPacketIn.WriteInt(winStreak);
			gSPacketIn.WriteString(lostName);
			gSPacketIn.WriteString(winName);
		}
		SendToAll(gSPacketIn);
	}

	public void SendAllPlayerList(UserGuildBattleInfo ex, bool toAll)
	{
		UserGuildBattleInfo[] allActiveUser = GetAllActiveUser();
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(allActiveUser.Length);
		UserGuildBattleInfo[] array = allActiveUser;
		foreach (UserGuildBattleInfo userGuildBattleInfo in array)
		{
			gSPacketIn.WriteInt(userGuildBattleInfo.UserID);
			gSPacketIn.WriteDateTime(userGuildBattleInfo.TombStoneEndTime);
			gSPacketIn.WriteByte((byte)userGuildBattleInfo.Status);
			gSPacketIn.WriteInt(userGuildBattleInfo.Postion.X);
			gSPacketIn.WriteInt(userGuildBattleInfo.Postion.Y);
			gSPacketIn.WriteBoolean(userGuildBattleInfo.Player.PlayerCharacter.Sex);
			gSPacketIn.WriteInt(userGuildBattleInfo.ConsortiaID);
			gSPacketIn.WriteString(userGuildBattleInfo.ConsortiaName);
			gSPacketIn.WriteInt(userGuildBattleInfo.WinStreak);
			gSPacketIn.WriteInt(userGuildBattleInfo.FailBuffCount);
			gSPacketIn.WriteInt(userGuildBattleInfo.Player.PlayerCharacter.MountsType);
			gSPacketIn.WriteInt(userGuildBattleInfo.Player.PlayerCharacter.PetsID);
			gSPacketIn.WriteBoolean(userGuildBattleInfo.Player.PlayerCharacter.Sex);
			gSPacketIn.WriteString(userGuildBattleInfo.Player.PlayerCharacter.Style);
			gSPacketIn.WriteString(userGuildBattleInfo.Player.PlayerCharacter.Colors);
		}
		if (toAll)
		{
			SendToAll(gSPacketIn, ex);
		}
		else
		{
			ex.Player.SendTCP(gSPacketIn);
		}
	}

	public void SendAddPlayer(UserGuildBattleInfo user)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(user.UserID);
		gSPacketIn.WriteDateTime(user.TombStoneEndTime);
		gSPacketIn.WriteByte((byte)user.Status);
		gSPacketIn.WriteInt(user.Postion.X);
		gSPacketIn.WriteInt(user.Postion.Y);
		gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.Sex);
		gSPacketIn.WriteInt(user.ConsortiaID);
		gSPacketIn.WriteString(user.ConsortiaName);
		gSPacketIn.WriteInt(user.WinStreak);
		gSPacketIn.WriteInt(user.FailBuffCount);
		gSPacketIn.WriteInt(user.Player.PlayerCharacter.MountsType);
		gSPacketIn.WriteInt(user.Player.PlayerCharacter.PetsID);
		gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(user.Player.PlayerCharacter.Style);
		gSPacketIn.WriteString(user.Player.PlayerCharacter.Colors);
		SendToAll(gSPacketIn);
	}

	public void SendInitSeftInfo(UserGuildBattleInfo user)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteDateTime(user.TombStoneEndTime);
		gSPacketIn.WriteInt(user.Postion.X);
		gSPacketIn.WriteInt(user.Postion.Y);
		gSPacketIn.WriteInt(user.Player.PlayerCharacter.ReduceStartBlood);
		gSPacketIn.WriteInt(user.VictoryCount);
		gSPacketIn.WriteInt(user.WinStreak);
		gSPacketIn.WriteInt(user.Score);
		gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.ActivePowFirstGame);
		gSPacketIn.WriteBoolean(user.DupeScoreConsortiaBattle);
		user.Player.SendTCP(gSPacketIn);
	}

	public void SendPlayerMove(UserGuildBattleInfo user, string moveStr)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(user.UserID);
		gSPacketIn.WriteInt(user.Postion.X);
		gSPacketIn.WriteInt(user.Postion.Y);
		gSPacketIn.WriteString(moveStr);
		SendToAll(gSPacketIn, user);
	}

	public void SendRemovePlayer(UserGuildBattleInfo user)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(user.UserID);
		SendToAll(gSPacketIn);
	}

	public void SendUpdateScore(UserGuildBattleInfo user, byte type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(16);
		gSPacketIn.WriteByte(type);
		if (type == 1)
		{
			GuildBattleConsortiaInfo[] array = (from a in GetAllConsortia()
				orderby a.Score descending
				select a).ToArray();
			gSPacketIn.WriteInt(array.Length);
			int num = 1;
			GuildBattleConsortiaInfo[] array2 = array;
			foreach (GuildBattleConsortiaInfo guildBattleConsortiaInfo in array2)
			{
				gSPacketIn.WriteString(guildBattleConsortiaInfo.ConsortiaName);
				gSPacketIn.WriteInt(num);
				gSPacketIn.WriteInt(guildBattleConsortiaInfo.Score);
				num++;
			}
		}
		else
		{
			UserGuildBattleInfo[] array3 = (from a in GetUserByConsortia(user.ConsortiaID)
				orderby a.Score descending
				select a).ToArray();
			gSPacketIn.WriteInt(array3.Length);
			int num2 = 1;
			UserGuildBattleInfo[] array4 = array3;
			foreach (UserGuildBattleInfo userGuildBattleInfo in array4)
			{
				gSPacketIn.WriteString(userGuildBattleInfo.NickName);
				gSPacketIn.WriteInt(num2);
				gSPacketIn.WriteInt(userGuildBattleInfo.Score);
				num2++;
			}
		}
		user.Player.SendTCP(gSPacketIn);
	}

	public void SendUpdateSceneInfo(UserGuildBattleInfo user)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(user.Player.PlayerCharacter.ReduceStartBlood);
		gSPacketIn.WriteInt(user.VictoryCount);
		gSPacketIn.WriteInt(user.WinStreak);
		gSPacketIn.WriteInt(user.Score);
		gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.ActivePowFirstGame);
		gSPacketIn.WriteBoolean(user.DupeScoreConsortiaBattle);
		gSPacketIn.WriteDateTime(user.TombStoneEndTime);
		user.Player.SendTCP(gSPacketIn);
	}

	public void SendUpdatePlayerStatus(UserGuildBattleInfo user)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(user.UserID);
		gSPacketIn.WriteDateTime(user.TombStoneEndTime);
		gSPacketIn.WriteByte((byte)user.Status);
		gSPacketIn.WriteInt(user.Postion.X);
		gSPacketIn.WriteInt(user.Postion.Y);
		gSPacketIn.WriteInt(user.WinStreak);
		gSPacketIn.WriteInt(user.FailBuffCount);
		gSPacketIn.WriteInt(user.Player.PlayerCharacter.MountsType);
		gSPacketIn.WriteInt(user.Player.PlayerCharacter.PetsID);
		gSPacketIn.WriteBoolean(user.Player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(user.Player.PlayerCharacter.Style);
		gSPacketIn.WriteString(user.Player.PlayerCharacter.Colors);
		SendToAll(gSPacketIn);
	}

	public void SendAllOpenClose()
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GuildBattleConsortiaInfo[] allConsortia = GetAllConsortia();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer p in array)
		{
			GSPacketIn pkg = SendOpenClosePkg(allConsortia.SingleOrDefault((GuildBattleConsortiaInfo a) => a.ConsortiaID == p.PlayerCharacter.ConsortiaID) != null);
			p.SendTCP(pkg);
		}
	}

	public void SendSingleOpenClose(GamePlayer p)
	{
		GSPacketIn pkg = SendOpenClosePkg(FindConsortia(p.PlayerCharacter.ConsortiaID) != null);
		p.SendTCP(pkg);
	}

	private GSPacketIn SendOpenClosePkg(bool canEnter)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteBoolean(IsOpen);
		gSPacketIn.WriteDateTime(TimeStart);
		gSPacketIn.WriteDateTime(TimeStop);
		gSPacketIn.WriteBoolean(canEnter);
		return gSPacketIn;
	}

	public void SendToTeam(GSPacketIn pkg, int consortiaId)
	{
		UserGuildBattleInfo[] allActiveUser = GetAllActiveUser(consortiaId);
		UserGuildBattleInfo[] array = allActiveUser;
		foreach (UserGuildBattleInfo userGuildBattleInfo in array)
		{
			if (userGuildBattleInfo != null && userGuildBattleInfo.Player != null)
			{
				userGuildBattleInfo.Player.SendTCP(pkg);
			}
		}
	}

	public void SendToTeam(GSPacketIn pkg, UserGuildBattleInfo except)
	{
		UserGuildBattleInfo[] allActiveUser = GetAllActiveUser(except.ConsortiaID);
		UserGuildBattleInfo[] array = allActiveUser;
		foreach (UserGuildBattleInfo userGuildBattleInfo in array)
		{
			if (userGuildBattleInfo != null && userGuildBattleInfo.Player != null && userGuildBattleInfo != except)
			{
				userGuildBattleInfo.Player.SendTCP(pkg);
			}
		}
	}

	public void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public void SendToAll(GSPacketIn pkg, UserGuildBattleInfo except)
	{
		UserGuildBattleInfo[] allActiveUser = GetAllActiveUser();
		UserGuildBattleInfo[] array = allActiveUser;
		foreach (UserGuildBattleInfo userGuildBattleInfo in array)
		{
			if (userGuildBattleInfo != null && userGuildBattleInfo != except)
			{
				userGuildBattleInfo.Player.SendTCP(pkg);
			}
		}
	}

	public bool AddUser(UserGuildBattleInfo user)
	{
		lock (m_listPlayer)
		{
			if (!m_listPlayer.ContainsKey(user.UserID))
			{
				m_listPlayer[user.UserID] = user;
				return true;
			}
			return false;
		}
	}

	public UserGuildBattleInfo FindUser(int userid)
	{
		lock (m_listPlayer)
		{
			if (m_listPlayer.ContainsKey(userid))
			{
				return m_listPlayer[userid];
			}
			return null;
		}
	}

	public UserGuildBattleInfo[] GetAllUser()
	{
		return m_listPlayer.Values.ToArray();
	}

	public UserGuildBattleInfo[] GetAllActiveUser()
	{
		return m_listPlayer.Values.Where((UserGuildBattleInfo a) => a.IsActive).ToArray();
	}

	public UserGuildBattleInfo[] GetAllActiveUser(int consortiaId)
	{
		return m_listPlayer.Values.Where((UserGuildBattleInfo a) => a.IsActive && a.ConsortiaID == consortiaId).ToArray();
	}

	public UserGuildBattleInfo[] GetUserByConsortia(int consortiaId)
	{
		return m_listPlayer.Values.Where((UserGuildBattleInfo a) => a.ConsortiaID == consortiaId).ToArray();
	}

	public GuildBattleConsortiaInfo FindConsortia(int consortiaId)
	{
		lock (m_listConsortia)
		{
			if (m_listConsortia.ContainsKey(consortiaId))
			{
				return m_listConsortia[consortiaId];
			}
			return null;
		}
	}

	public GuildBattleConsortiaInfo[] GetAllConsortia()
	{
		return m_listConsortia.Values.ToArray();
	}

	public bool CanEnterGame(int consortiaId)
	{
		if (FindConsortia(consortiaId) != null && State == GuildBattleState.OPEN)
		{
			return true;
		}
		return false;
	}

	public bool CanStartGame(DateTime now)
	{
		TimeSpan timeOfDay = GuildBattleStartTime.TimeOfDay;
		if (timeOfDay <= now.TimeOfDay && now.TimeOfDay.Hours < timeOfDay.Hours + 1)
		{
			return true;
		}
		return false;
	}

	public void AddCountDownRevive(UserGuildBattleInfo user, int timeout)
	{
		user.IsDead = true;
		user.TombStoneEndTime = DateTime.Now.AddSeconds(timeout);
		if (!CheckAction(typeof(PlayerReviveCountDownAction)))
		{
			AddAction(new PlayerReviveCountDownAction(user, timeout * 1000, stayRespawn: false));
		}
	}

	public void QuickRevive(UserGuildBattleInfo user, bool stay)
	{
		AddAction(new PlayerReviveCountDownAction(user, 0, stay));
	}

	public bool CheckAction(IGuildBattleAction action)
	{
		lock (m_actions)
		{
			if (m_actions.Contains(action))
			{
				return true;
			}
			return false;
		}
	}

	public bool CheckAction(Type type)
	{
		lock (m_actions)
		{
			if (m_actions.ToArray().SingleOrDefault((object a) => a.GetType().Equals(type)) != null)
			{
				return true;
			}
			return false;
		}
	}

	public void AddAction(IGuildBattleAction action)
	{
		lock (m_actions)
		{
			m_actions.Add(action);
		}
	}

	public void AddAction(ArrayList actions)
	{
		lock (m_actions)
		{
			m_actions.AddRange(actions);
		}
	}

	public void Update(long tick)
	{
		if (m_passTick >= tick)
		{
			return;
		}
		ArrayList arrayList;
		lock (m_actions)
		{
			arrayList = (ArrayList)m_actions.Clone();
			m_actions.Clear();
		}
		CurrentActionCount = arrayList.Count;
		if (arrayList.Count > 0)
		{
			ArrayList arrayList2 = new ArrayList();
			foreach (IGuildBattleAction item in arrayList)
			{
				try
				{
					item.Execute(this, tick);
					if (!item.IsFinished(tick))
					{
						arrayList2.Add(item);
					}
				}
				catch (Exception exception)
				{
					log.Error("Map update error:", exception);
				}
			}
			AddAction(arrayList2);
		}
		else if (m_waitTimer < tick)
		{
			CheckState(0);
		}
	}

	public void CheckState(int delay)
	{
		AddAction(new CheckGuildBattleStateAction(delay));
	}

	public void WaitTime(int delay)
	{
		m_waitTimer = Math.Max(m_waitTimer, TickHelper.GetTickCount() + delay);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.CollectionTask;
using Game.Server.DragonBoat;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.NewHall;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public sealed class WorldMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	private static Dictionary<int, GamePlayer> m_players = new Dictionary<int, GamePlayer>();

	public static Scene _marryScene;

	private static Dictionary<int, AreaConfigInfo> m_areaConfigs = new Dictionary<int, AreaConfigInfo>();

	private static Dictionary<int, EdictumInfo> m_edictums = new Dictionary<int, EdictumInfo>();

	private static RSACryptoServiceProvider m_rsa;

	private static NewHallRoom _newHallRoom;

	private static CollectionTaskRoom _collectionTaskRoom;

	private static DateTime _EndTrailEliteDate;

	public static readonly string[] TrailEliteAward = GameProperties.TrailEliteAwardLevel.Split(',');

	private static object m_lock = new object();

	public static Scene MarryScene => _marryScene;

	public static RSACryptoServiceProvider RsaCryptor => m_rsa;

	public static NewHallRoom NewHallRooms => _newHallRoom;

	public static CollectionTaskRoom CollectionTaskRooms => _collectionTaskRoom;

	public static DateTime EndTrailEliteDate => _EndTrailEliteDate;

	public static bool Init()
	{
		bool result = false;
		try
		{
			m_rsa = new RSACryptoServiceProvider();
			m_rsa.FromXmlString(GameServer.Instance.Configuration.PrivateKey);
			m_players.Clear();
			_marryScene = new Scene();
			_newHallRoom = new NewHallRoom();
			_collectionTaskRoom = new CollectionTaskRoom();
			Dictionary<int, EdictumInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_edictums, dictionary);
			}
			_EndTrailEliteDate = DateTime.Now.GetStartNextMonth(1);
			result = true;
		}
		catch (Exception exception)
		{
			log.Error("WordMgr Init", exception);
		}
		return result;
	}

	private static Dictionary<int, EdictumInfo> LoadFromDatabase()
	{
		Dictionary<int, EdictumInfo> dictionary = new Dictionary<int, EdictumInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			EdictumInfo[] allEdictum = produceBussiness.GetAllEdictum();
			EdictumInfo[] array = allEdictum;
			foreach (EdictumInfo edictumInfo in array)
			{
				if (!dictionary.ContainsKey(edictumInfo.ID))
				{
					dictionary.Add(edictumInfo.ID, edictumInfo);
				}
			}
		}
		return dictionary;
	}

	public static GSPacketIn SendSysNotice(eMessageType type, string msg, int ItemID, int TemplateID, string key)
	{
		int val = msg.IndexOf(TemplateID.ToString(), StringComparison.Ordinal);
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt((int)type);
		gSPacketIn.WriteString(msg.Replace(TemplateID.ToString(), ""));
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(TemplateID);
		gSPacketIn.WriteInt(ItemID);
		if (!string.IsNullOrEmpty(key))
		{
			gSPacketIn.WriteString(key);
		}
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static void AddAreaConfig(AreaConfigInfo[] Areas)
	{
		foreach (AreaConfigInfo areaConfigInfo in Areas)
		{
			if (!m_areaConfigs.ContainsKey(areaConfigInfo.AreaID))
			{
				m_areaConfigs.Add(areaConfigInfo.AreaID, areaConfigInfo);
			}
		}
		if (Areas.Length != 0)
		{
			DragonBoatMgr.Init();
		}
	}

	public static AreaConfigInfo FindAreaConfig(int zoneId)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_areaConfigs.ContainsKey(zoneId))
			{
				return m_areaConfigs[zoneId];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static AreaConfigInfo[] GetAllAreaConfig()
	{
		List<AreaConfigInfo> list = new List<AreaConfigInfo>();
		foreach (AreaConfigInfo value in m_areaConfigs.Values)
		{
			list.Add(value);
		}
		return list.ToArray();
	}

	public static EdictumInfo[] GetAllEdictumVersion()
	{
		List<EdictumInfo> list = new List<EdictumInfo>();
		foreach (EdictumInfo value in m_edictums.Values)
		{
			if (value.EndDate.Date > DateTime.Now.Date)
			{
				list.Add(value);
			}
		}
		return list.ToArray();
	}

	public static void SendToAll(GSPacketIn pkg)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.SendTCP(pkg);
		}
	}

	public static void SendToNoGame(GSPacketIn pkg, GamePlayer ext)
	{
		GamePlayer[] allPlayersNoRoom = GetAllPlayersNoRoom();
		GamePlayer[] array = allPlayersNoRoom;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != ext)
			{
				gamePlayer.SendTCP(pkg);
			}
		}
	}

	public static void SendToConsortia(GSPacketIn pkg, int consortiaId)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID != 0 && gamePlayer.PlayerCharacter.ConsortiaID == consortiaId)
			{
				gamePlayer.SendTCP(pkg);
			}
		}
	}

	public static GSPacketIn SendSysNotice(eMessageType type, string msg, int itemId, int templateId, int zoneId, string key)
	{
		int val = msg.IndexOf(templateId.ToString(), StringComparison.Ordinal);
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt((int)type);
		gSPacketIn.WriteString(msg.Replace(templateId.ToString(), ""));
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(templateId);
		gSPacketIn.WriteInt(itemId);
		if (!string.IsNullOrEmpty(key))
		{
			gSPacketIn.WriteString(key);
		}
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static GSPacketIn SendSysNotice(eMessageType chatNormal, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteString(msg);
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static GSPacketIn SendSysNotice2(eMessageType chatNormal, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(12);
		gSPacketIn.WriteString(msg);
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static GSPacketIn SendSysRedPackageNotice(int consortiaId, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(111);
		gSPacketIn.WriteString(msg);
		SendToConsortia(gSPacketIn, consortiaId);
		return gSPacketIn;
	}

	public static GSPacketIn SendGMNotice(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString(msg);
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static GSPacketIn SendCrossSysNotice(string msg, int zoneId, string zoneName, int itemID, int templateID, int caddy, string nick, string key)
	{
		int val = msg.IndexOf(templateID.ToString(), StringComparison.Ordinal);
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(12);
		gSPacketIn.WriteString(msg.Replace(templateID.ToString(), ""));
		gSPacketIn.WriteInt(zoneId);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(templateID);
		gSPacketIn.WriteInt(itemID);
		if (!string.IsNullOrEmpty(key))
		{
			gSPacketIn.WriteString(key);
		}
		gSPacketIn.WriteInt(caddy);
		if (caddy > 0)
		{
			gSPacketIn.WriteString(nick);
			gSPacketIn.WriteInt(templateID);
			gSPacketIn.WriteString(zoneName);
		}
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static void ChatEntertamentModeOpenOrClose()
	{
		GSPacketIn packet = new GSPacketIn(191);
		GameServer.Instance.LoginServer.SendPacket(packet);
	}

	public static void ChatEntertamentModeGetPoint(PlayerInfo p, int point)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(189);
		gSPacketIn.WriteInt(p.ID);
		gSPacketIn.WriteInt(point);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public static void ChatEntertamentModeUpdatePoint(PlayerInfo p, int point)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(190);
		gSPacketIn.WriteInt(p.ID);
		gSPacketIn.WriteInt(point);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public static List<ItemInfo> CreateRandomPropItem()
	{
		lock (m_lock)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			List<int> list2 = new List<int>
			{
				10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10010, 10020,
				10021, 10023, 10028
			};
			Random random = new Random();
			for (int i = 0; i < 3; i++)
			{
				int index = random.Next(0, list2.Count - 1);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list2[index]), 1, 105);
				if (itemInfo != null)
				{
					list.Add(itemInfo);
				}
				list2.RemoveAt(index);
			}
			return list;
		}
	}

	public static ItemInfo GetRandomPropItem(List<ItemInfo> itemsNotSame)
	{
		lock (m_lock)
		{
			Random random = new Random();
			List<int> list = new List<int>
			{
				10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008, 10010, 10020,
				10021, 10023, 10028
			};
			foreach (ItemInfo item in itemsNotSame)
			{
				if (list.Contains(item.TemplateID))
				{
					list.Remove(item.TemplateID);
				}
			}
			int index = random.Next(0, list.Count - 1);
			return ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list[index]), 1, 105);
		}
	}

	public static bool AddPlayer(int playerId, GamePlayer player)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_players.ContainsKey(playerId))
			{
				return false;
			}
			m_players.Add(playerId, player);
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return true;
	}

	internal static GSPacketIn SendSysNotice(string logMessage)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteString(logMessage);
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static bool RemovePlayer(int playerId)
	{
		m_clientLocker.AcquireWriterLock(-1);
		GamePlayer gamePlayer = null;
		try
		{
			if (m_players.ContainsKey(playerId))
			{
				gamePlayer = m_players[playerId];
				m_players.Remove(playerId);
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		if (gamePlayer == null)
		{
			return false;
		}
		GameServer.Instance.LoginServer.SendUserOffline(playerId, gamePlayer.PlayerCharacter.ConsortiaID);
		return true;
	}

	public static GamePlayer GetPlayerById(int playerId)
	{
		GamePlayer result = null;
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			if (m_players.ContainsKey(playerId))
			{
				result = m_players[playerId];
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return result;
	}

	public static GamePlayer GetClientByPlayerNickName(string nickName)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == nickName)
			{
				return gamePlayer;
			}
		}
		return null;
	}

	public static GamePlayer[] GetAllPlayers()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			foreach (GamePlayer value in m_players.Values)
			{
				if (value != null && value.PlayerCharacter != null)
				{
					list.Add(value);
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}

	public static GamePlayer[] GetAllPlayersNoGame()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				if (gamePlayer.CurrentRoom?.Game == null)
				{
					list.Add(gamePlayer);
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}

	public static GamePlayer[] GetAllPlayersNoRoom()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				if (gamePlayer.CurrentRoom == null)
				{
					list.Add(gamePlayer);
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}

	public static GamePlayer GetSinglePlayerNoGame()
	{
		GamePlayer result = null;
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				if (gamePlayer.CurrentRoom == null)
				{
					result = gamePlayer;
					break;
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return result;
	}

	public static string GetPlayerStringByPlayerNickName(string nickName)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == nickName)
			{
				return gamePlayer.ToString();
			}
		}
		return nickName + " is not online!";
	}

	public static string DisconnectPlayerByName(string nickName)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == nickName)
			{
				gamePlayer.Disconnect();
				return "OK";
			}
		}
		return nickName + " is not online!";
	}

	public static void OnPlayerOffline(int playerid, int consortiaID)
	{
		ChangePlayerState(playerid, 0, consortiaID);
	}

	public static void OnPlayerOnline(int playerid, int consortiaID)
	{
		ChangePlayerState(playerid, 1, consortiaID);
	}

	public static void ChangePlayerState(int playerID, int state, int consortiaID)
	{
		GSPacketIn gSPacketIn = null;
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if ((gamePlayer.Friends != null && gamePlayer.Friends.ContainsKey(playerID) && gamePlayer.Friends[playerID] == 0) || (gamePlayer.PlayerCharacter.ConsortiaID != 0 && gamePlayer.PlayerCharacter.ConsortiaID == consortiaID))
			{
				if (gSPacketIn == null)
				{
					gSPacketIn = gamePlayer.Out.SendFriendState(playerID, state, gamePlayer.PlayerCharacter.typeVIP, gamePlayer.PlayerCharacter.VIPLevel);
				}
				else
				{
					gamePlayer.SendTCP(gSPacketIn);
				}
			}
		}
	}

	public static bool CheckOpenCityBattle()
	{
		TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
		if (timeOfDay >= DateTime.Parse(GameProperties.CityBattleBeginTime).TimeOfDay && timeOfDay < DateTime.Parse(GameProperties.CityBattleEndTime).TimeOfDay)
		{
			return true;
		}
		return false;
	}

	public static void UpdateTrailEliteTimer()
	{
		if (!(DateTime.Now.Date >= _EndTrailEliteDate))
		{
			return;
		}
		_EndTrailEliteDate = DateTime.Now.GetStartNextMonth(1);
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer == null || !gamePlayer.IsActive)
			{
				continue;
			}
			gamePlayer.TrailElite.ResetTrailElite();
			string[] trailEliteAward = TrailEliteAward;
			foreach (string text in trailEliteAward)
			{
				string[] array2 = text.Split('|');
				foreach (string s in array2)
				{
					gamePlayer.RemoveTemplate(int.Parse(s), 1, canRenew: true, canTransfer: true);
				}
			}
			gamePlayer.TrailElite.SendTrailEliteInfo();
		}
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			playerBussiness.RemoveAllTrailEliteSession();
		}
		log.Info("// TrailElite clear and starting new session.");
	}

	public static GSPacketIn SendStaffHelp(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteString(msg);
		return gSPacketIn;
	}

	public static GamePlayer[] GetAllPlayersByTeamId(int teamId)
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(10000);
		try
		{
			foreach (GamePlayer value in m_players.Values)
			{
				if (value != null && value.PlayerCharacter != null && value != null && value.PlayerCharacter.Team != null && value != null && value.PlayerCharacter.Team.teamID == teamId)
				{
					list.Add(value);
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}

	public static string GetUserNickNameByUserId(int userId)
	{
		bool flag = false;
		GamePlayer[] allPlayers = GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			if (gamePlayer.PlayerId == userId)
			{
				return gamePlayer.PlayerCharacter.NickName;
			}
			flag = true;
		}
		if (flag)
		{
			PlayerData playerData = new PlayerData();
			return playerData.Database.SqlQuery<string>("Select NickName from Sys_Users_Detail Where UserID = {0}", new object[1] { userId }).ToList()[0];
		}
		return "";
	}

	public static GamePlayer[] GetAllConsortiaPlayers(int consortiaId)
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			list.AddRange(m_players.Values.Where((GamePlayer value) => value?.PlayerCharacter != null && value.PlayerCharacter.ConsortiaID == consortiaId));
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}
}

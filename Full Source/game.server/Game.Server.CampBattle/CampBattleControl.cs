using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.CampBattle.Action;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Rooms;
using log4net;

namespace Game.Server.CampBattle;

public class CampBattleControl
{
	public enum CampBattleState
	{
		CLOSE,
		OPEN
	}

	[NonSerialized]
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ArrayList m_actions;

	private long m_passTick = 0L;

	public int CurrentActionCount = 0;

	private long m_waitTimer = 0L;

	public CampBattleState State;

	private Dictionary<int, CampBattlePlayerInfo> m_players;

	private Dictionary<int, CampBattleMonsterInfo> m_monstersList;

	private Dictionary<int, CampBattleTeamInfo> m_team;

	private CampBattlePlayerInfo m_capturePlayer;

	private DateTime m_lastTimeCapture;

	private int m_MonsterId;

	private readonly Point[] pointMonsterSpawn = new Point[15]
	{
		new Point(610, 274),
		new Point(1012, 256),
		new Point(860, 264),
		new Point(1122, 264),
		new Point(1254, 266),
		new Point(560, 2204),
		new Point(604, 324),
		new Point(782, 210),
		new Point(796, 334),
		new Point(938, 198),
		new Point(1102, 194),
		new Point(1102, 304),
		new Point(1202, 194),
		new Point(1140, 314),
		new Point(914, 278)
	};

	private readonly Point[] pointMapPVPSpawn = new Point[4]
	{
		new Point(518, 175),
		new Point(2798, 417),
		new Point(2950, 1406),
		new Point(874, 1798)
	};

	private readonly Point m_pointStart = new Point(184, 281);

	private object m_object = new object();

	public DateTime StartTime { get; set; }

	public DateTime EndTime { get; set; }

	public bool IsSendMail { get; set; }

	public bool IsOpenCampBattle { get; set; }

	public CampBattleControl()
	{
		m_actions = new ArrayList();
		m_players = new Dictionary<int, CampBattlePlayerInfo>();
		m_monstersList = new Dictionary<int, CampBattleMonsterInfo>();
		m_team = new Dictionary<int, CampBattleTeamInfo>();
		IsOpenCampBattle = false;
		m_lastTimeCapture = DateTime.Now;
		StartTime = DateTime.Now;
		EndTime = DateTime.Now;
		ClearAll();
	}

	public Point GetStartPoint(int MapId, int team)
	{
		if (MapId == 0)
		{
			return m_pointStart;
		}
		return pointMapPVPSpawn[team - 1];
	}

	public CampBattleTeamInfo GetTeam(int id)
	{
		lock (m_team)
		{
			if (m_team.ContainsKey(id))
			{
				return m_team[id];
			}
			return null;
		}
	}

	public List<CampBattleTeamInfo> GetAllTeam()
	{
		lock (m_team)
		{
			return m_team.Values.ToList();
		}
	}

	private List<CampBattlePlayerInfo> GetAllPlayers()
	{
		lock (m_players)
		{
			return m_players.Values.ToList();
		}
	}

	private List<CampBattlePlayerInfo> GetAllActivePlayers()
	{
		lock (m_players)
		{
			return m_players.Values.Where((CampBattlePlayerInfo a) => a.IsActive && a.Player != null).ToList();
		}
	}

	private List<CampBattlePlayerInfo> GetTopRank()
	{
		lock (m_players)
		{
			List<CampBattlePlayerInfo> list = (from a in m_players.Values
				where a.Score > 0
				orderby a.Score descending
				select a).ToList();
			int num = 1;
			foreach (CampBattlePlayerInfo item in list)
			{
				item.Rank = num;
				num++;
			}
			return list;
		}
	}

	public void ClearAll()
	{
		m_players.Clear();
		m_monstersList.Clear();
		m_team.Clear();
		m_MonsterId = 0;
		InstallTeam();
	}

	private void InstallTeam()
	{
		for (int i = 1; i <= 4; i++)
		{
			CampBattleTeamInfo campBattleTeamInfo = new CampBattleTeamInfo();
			campBattleTeamInfo.TeamID = i;
			campBattleTeamInfo.Point = 0;
			campBattleTeamInfo.OpenGate = false;
			campBattleTeamInfo.MosterCount = 1;
			m_team.Add(i, campBattleTeamInfo);
		}
	}

	public void ChallengePVPGame(GamePlayer p1, GamePlayer p2)
	{
		lock (m_object)
		{
			if (p1.CurrentRoom != null && p2.CurrentRoom != null && p1.CurrentRoom.RoomType == eRoomType.CampBattleBattle && p2.CurrentRoom.RoomType == eRoomType.CampBattleBattle && !p1.CurrentRoom.IsPlaying && !p2.CurrentRoom.IsPlaying)
			{
				CampBattlePlayerInfo player = GetPlayer(p1.ZoneId, p1.PlayerId);
				CampBattlePlayerInfo player2 = GetPlayer(p2.ZoneId, p2.PlayerId);
				if (player != null && player2 != null && player.StateType == CampBattlePlayerState.NORMAL && player2.StateType == CampBattlePlayerState.NORMAL && player.Team != player2.Team)
				{
					player.StateType = CampBattlePlayerState.FIGHTING;
					player2.StateType = CampBattlePlayerState.FIGHTING;
					RoomMgr.StartCampBattlePVP(p1.CurrentRoom, p2.CurrentRoom);
				}
			}
		}
	}

	public void ChallengePVEGame(GamePlayer p, int monsterId)
	{
		lock (m_object)
		{
			if (p.CurrentRoom == null || p.CurrentRoom.RoomType != eRoomType.CampBattleBattle || p.CurrentRoom.IsPlaying)
			{
				return;
			}
			CampBattlePlayerInfo player = GetPlayer(p.ZoneId, p.PlayerId);
			CampBattleMonsterInfo singleMonster = GetSingleMonster(monsterId);
			if (singleMonster != null && singleMonster.StateType != CampBattlePlayerState.DEAD && player != null && player.StateType == CampBattlePlayerState.NORMAL)
			{
				if (singleMonster.StateType == CampBattlePlayerState.FIGHTING && singleMonster.CurrentGame != null && singleMonster.CurrentGame.GameState == eGameState.Playing)
				{
					if (singleMonster.CurrentGame.CanAddPlayer())
					{
						player.StateType = CampBattlePlayerState.FIGHTING;
						Player p2 = singleMonster.CurrentGame.AddPlayer(p);
						singleMonster.CurrentGame.MissionSingleStart(p2);
					}
					else
					{
						p.SendMessage(LanguageMgr.GetTranslation("GameServer.CampBattle.PVE.EnterRoom.Error"));
					}
				}
				else if (singleMonster.StateType == CampBattlePlayerState.NORMAL)
				{
					List<GamePlayer> players = p.CurrentRoom.GetPlayers();
					List<IGamePlayer> list = new List<IGamePlayer>();
					foreach (GamePlayer item in players)
					{
						if (item != null)
						{
							list.Add(item);
						}
					}
					BaseGame baseGame = GameMgr.StartCampPVEGame(p.CurrentRoom.RoomId, list, singleMonster.MissionId, 4, singleMonster.ID);
					if (baseGame != null)
					{
						singleMonster.CurrentGame = baseGame;
						singleMonster.StateType = CampBattlePlayerState.FIGHTING;
						player.StateType = CampBattlePlayerState.FIGHTING;
						p.CurrentRoom.IsPlaying = true;
						p.CurrentRoom.StartGame(baseGame);
					}
				}
				else
				{
					p.SendMessage(LanguageMgr.GetTranslation("GameServer.CampBattle.PVEEnding.Notice"));
				}
			}
			else
			{
				p.SendMessage(LanguageMgr.GetTranslation("GameServer.CampBattle.PVEStop.Notice"));
			}
		}
	}

	public int GetTeamLowPlayer()
	{
		int[] array = new int[4]
		{
			m_players.Where((KeyValuePair<int, CampBattlePlayerInfo> a) => a.Value.Team == 1 && a.Value.IsActive).Count(),
			m_players.Where((KeyValuePair<int, CampBattlePlayerInfo> a) => a.Value.Team == 2 && a.Value.IsActive).Count(),
			m_players.Where((KeyValuePair<int, CampBattlePlayerInfo> a) => a.Value.Team == 3 && a.Value.IsActive).Count(),
			m_players.Where((KeyValuePair<int, CampBattlePlayerInfo> a) => a.Value.Team == 4 && a.Value.IsActive).Count()
		};
		return Array.IndexOf(array, array.Min()) + 1;
	}

	public void UpdatePointAllTeam()
	{
		List<CampBattlePlayerInfo> allPlayers = GetAllPlayers();
		List<CampBattleTeamInfo> allTeam = GetAllTeam();
		foreach (CampBattleTeamInfo t in allTeam)
		{
			t.Point = allPlayers.Where((CampBattlePlayerInfo a) => a.Team == t.TeamID).Sum((CampBattlePlayerInfo a) => a.Score);
		}
	}

	public bool CaptureStatue(CampBattlePlayerInfo player, bool isCapture)
	{
		if (m_capturePlayer == null && isCapture && player.MapId == 1)
		{
			m_capturePlayer = player;
			SendCaptureStatue(player, isCapture);
			SendMessenge(LanguageMgr.GetTranslation("CampBattle.Msg.CaptureStatue.Success", player.NickName), player.MapId, player.Team);
		}
		else if (m_capturePlayer != null && !isCapture && m_capturePlayer == player)
		{
			m_capturePlayer = null;
			SendCaptureStatue(player, isCapture);
		}
		return false;
	}

	public void RemoveAllPlayerInRoom()
	{
		lock (m_players)
		{
			foreach (CampBattlePlayerInfo value in m_players.Values)
			{
				if (value.IsActive && value.Player != null && value.Player.CurrentRoom != null)
				{
					value.Player.CurrentRoom.RemovePlayerUnsafe(value.Player);
				}
				value.IsActive = false;
				value.Player = null;
			}
		}
	}

	public CampBattlePlayerInfo GetPlayer(int zoneId, int userId)
	{
		lock (m_players)
		{
			return m_players.Values.SingleOrDefault((CampBattlePlayerInfo a) => a.ZoneId == zoneId && a.UserID == userId);
		}
	}

	public List<CampBattlePlayerInfo> GetListPlayer(int team)
	{
		lock (m_players)
		{
			return m_players.Values.Where((CampBattlePlayerInfo a) => a.Team == team).ToList();
		}
	}

	public List<CampBattlePlayerInfo> GetListPlayersActive(int team)
	{
		lock (m_players)
		{
			return m_players.Values.Where((CampBattlePlayerInfo a) => a.Team == team && a.IsActive).ToList();
		}
	}

	public List<CampBattlePlayerInfo> GetListPlayer(int mapId, int team)
	{
		lock (m_players)
		{
			if (mapId == 0)
			{
				return m_players.Values.Where((CampBattlePlayerInfo a) => a.MapId == mapId && a.Team == team).ToList();
			}
			return m_players.Values.Where((CampBattlePlayerInfo a) => a.MapId == mapId).ToList();
		}
	}

	public List<CampBattlePlayerInfo> GetListPlayerActives(int mapId, int team)
	{
		lock (m_players)
		{
			if (mapId == 0)
			{
				return m_players.Values.Where((CampBattlePlayerInfo a) => a.MapId == mapId && a.Team == team && a.IsActive).ToList();
			}
			return m_players.Values.Where((CampBattlePlayerInfo a) => a.MapId == mapId && a.IsActive).ToList();
		}
	}

	private void AddPlayer(CampBattlePlayerInfo p)
	{
		lock (m_players)
		{
			if (m_players.ContainsKey(p.UserID))
			{
				m_players[p.UserID] = p;
			}
			else
			{
				m_players.Add(p.UserID, p);
			}
		}
	}

	public bool AddPlayer(GamePlayer p)
	{
		bool flag = false;
		if (IsOpenCampBattle)
		{
			CampBattlePlayerInfo campBattlePlayerInfo = GetPlayer(p.ZoneId, p.PlayerId);
			if (campBattlePlayerInfo != null)
			{
				if (campBattlePlayerInfo.LastTimeLogout.AddMinutes(5.0) > DateTime.Now)
				{
					TimeSpan timeSpan = campBattlePlayerInfo.LastTimeLogout.AddMinutes(5.0) - DateTime.Now;
					p.SendMessage(LanguageMgr.GetTranslation("CampBattle.Msg.Limit.Logout", (int)timeSpan.TotalSeconds));
				}
				else
				{
					campBattlePlayerInfo.Team = 1;
					campBattlePlayerInfo.IsActive = true;
					campBattlePlayerInfo.StateType = CampBattlePlayerState.NORMAL;
					campBattlePlayerInfo.Player = p;
					flag = true;
				}
			}
			else
			{
				campBattlePlayerInfo = new CampBattlePlayerInfo();
				campBattlePlayerInfo.UserID = p.PlayerId;
				campBattlePlayerInfo.ZoneId = p.ZoneId;
				campBattlePlayerInfo.ZoneName = p.ZoneName;
				campBattlePlayerInfo.NickName = p.PlayerCharacter.NickName;
				campBattlePlayerInfo.Player = p;
				campBattlePlayerInfo.PosX = m_pointStart.X;
				campBattlePlayerInfo.PosY = m_pointStart.Y;
				campBattlePlayerInfo.Team = 1;
				campBattlePlayerInfo.TimerCount = 0;
				campBattlePlayerInfo.IsActive = true;
				campBattlePlayerInfo.LastTimeLogout = DateTime.Now;
				AddPlayer(campBattlePlayerInfo);
				flag = true;
			}
			if (flag)
			{
				SendCampInitSecen(campBattlePlayerInfo);
				SendAddPlayer(campBattlePlayerInfo);
				SendCampSocerRank();
			}
		}
		return flag;
	}

	public void RemovePlayer(GamePlayer player)
	{
		CampBattlePlayerInfo player2 = GetPlayer(player.ZoneId, player.PlayerId);
		if (player2 != null)
		{
			RemovePlayer(player2);
		}
	}

	public void RemovePlayer(CampBattlePlayerInfo p)
	{
		SendRemovePlayer(p);
		p.IsActive = false;
		p.LastTimeLogout = DateTime.Now;
		CaptureStatue(p, isCapture: false);
	}

	public int NextMonsterId()
	{
		return Interlocked.Increment(ref m_MonsterId);
	}

	public List<CampBattleMonsterInfo> GetLivingMonster(int team)
	{
		lock (m_monstersList)
		{
			return m_monstersList.Values.Where((CampBattleMonsterInfo a) => a.Team == team && a.StateType != CampBattlePlayerState.DEAD).ToList();
		}
	}

	public List<CampBattleMonsterInfo> GetMonster(int team)
	{
		lock (m_monstersList)
		{
			return m_monstersList.Values.Where((CampBattleMonsterInfo a) => a.Team == team).ToList();
		}
	}

	public CampBattleMonsterInfo GetSingleMonster(int monsterId)
	{
		lock (m_monstersList)
		{
			return m_monstersList.Values.SingleOrDefault((CampBattleMonsterInfo a) => a.ID == monsterId);
		}
	}

	private void AddMonster(int team, int missionId, string action, Point point)
	{
		lock (m_monstersList)
		{
			CampBattleMonsterInfo campBattleMonsterInfo = new CampBattleMonsterInfo();
			campBattleMonsterInfo.ID = NextMonsterId();
			campBattleMonsterInfo.ActionMovieName = action;
			campBattleMonsterInfo.MissionId = missionId;
			campBattleMonsterInfo.Team = team;
			campBattleMonsterInfo.PosX = point.X;
			campBattleMonsterInfo.PosY = point.Y;
			m_monstersList.Add(campBattleMonsterInfo.ID, campBattleMonsterInfo);
		}
	}

	public void InstallMonster(CampBattleTeamInfo team)
	{
		switch (team.MosterCount)
		{
		case 1:
		{
			for (int j = 0; j < 14; j++)
			{
				AddMonster(team.TeamID, 60001, "Living220", pointMonsterSpawn[j]);
			}
			AddMonster(team.TeamID, 60002, "Living222", pointMonsterSpawn[14]);
			break;
		}
		case 2:
		{
			for (int num2 = 0; num2 < 14; num2++)
			{
				AddMonster(team.TeamID, 60003, "Living060", pointMonsterSpawn[num2]);
			}
			AddMonster(team.TeamID, 60004, "Living065", pointMonsterSpawn[14]);
			break;
		}
		case 3:
		{
			for (int k = 0; k < 14; k++)
			{
				AddMonster(team.TeamID, 60005, "Living058", pointMonsterSpawn[k]);
			}
			AddMonster(team.TeamID, 60006, "Living064", pointMonsterSpawn[14]);
			break;
		}
		case 4:
		{
			for (int n = 0; n < 14; n++)
			{
				AddMonster(team.TeamID, 60007, "Living047", pointMonsterSpawn[n]);
			}
			AddMonster(team.TeamID, 60008, "Living074", pointMonsterSpawn[14]);
			break;
		}
		case 5:
		{
			for (int num3 = 0; num3 < 14; num3++)
			{
				AddMonster(team.TeamID, 60009, "Living061", pointMonsterSpawn[num3]);
			}
			AddMonster(team.TeamID, 60010, "Living226", pointMonsterSpawn[14]);
			break;
		}
		case 6:
		{
			for (int m = 0; m < 14; m++)
			{
				AddMonster(team.TeamID, 60011, "Living097", pointMonsterSpawn[m]);
			}
			AddMonster(team.TeamID, 60012, "Living224", pointMonsterSpawn[14]);
			break;
		}
		case 7:
		{
			for (int num4 = 0; num4 < 14; num4++)
			{
				AddMonster(team.TeamID, 60013, "Living042", pointMonsterSpawn[num4]);
			}
			AddMonster(team.TeamID, 60014, "Living075", pointMonsterSpawn[14]);
			break;
		}
		case 8:
		{
			for (int num = 0; num < 14; num++)
			{
				AddMonster(team.TeamID, 60015, "Living221", pointMonsterSpawn[num]);
			}
			AddMonster(team.TeamID, 60016, "Living055", pointMonsterSpawn[14]);
			break;
		}
		case 9:
		{
			for (int l = 0; l < 14; l++)
			{
				AddMonster(team.TeamID, 60017, "Living228", pointMonsterSpawn[l]);
			}
			AddMonster(team.TeamID, 60018, "Living225", pointMonsterSpawn[14]);
			break;
		}
		case 10:
		{
			for (int i = 0; i < 14; i++)
			{
				AddMonster(team.TeamID, 60019, "Living045", pointMonsterSpawn[i]);
			}
			AddMonster(team.TeamID, 60020, "Living057", pointMonsterSpawn[14]);
			break;
		}
		}
	}

	public bool CheckAction(ICampBattleAction action)
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

	public void AddAction(ICampBattleAction action)
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
			foreach (ICampBattleAction item in arrayList)
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
		AddAction(new CheckCampBattleStateAction(delay));
	}

	public void WaitTime(int delay)
	{
		m_waitTimer = Math.Max(m_waitTimer, TickHelper.GetTickCount() + delay);
	}

	public void SendPerScoreRank(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(21);
		List<CampBattlePlayerInfo> topRank = GetTopRank();
		gSPacketIn.WriteInt(topRank.Count);
		foreach (CampBattlePlayerInfo item in topRank)
		{
			gSPacketIn.WriteInt(item.ZoneId);
			gSPacketIn.WriteInt(item.UserID);
			gSPacketIn.WriteString(item.ZoneName);
			gSPacketIn.WriteString(item.NickName);
			gSPacketIn.WriteInt(item.Score);
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendCampSocerRank()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(20);
		List<CampBattleTeamInfo> allTeam = GetAllTeam();
		gSPacketIn.WriteInt(allTeam.Count);
		foreach (CampBattleTeamInfo item in allTeam)
		{
			gSPacketIn.WriteInt(item.TeamID);
			gSPacketIn.WriteInt(item.Point);
			gSPacketIn.WriteInt(item.TotalPlayers);
		}
		SendToALL(gSPacketIn);
	}

	public void SendCampInitSecen(CampBattlePlayerInfo player)
	{
		CampBattleTeamInfo team = GetTeam(player.Team);
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(player.MapId);
		gSPacketIn.WriteInt(team.MosterCount);
		if (m_capturePlayer == null)
		{
			gSPacketIn.WriteBoolean(val: false);
		}
		else
		{
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(m_capturePlayer.Player.ZoneId);
			gSPacketIn.WriteInt(m_capturePlayer.Player.PlayerId);
		}
		List<CampBattlePlayerInfo> listPlayer = GetListPlayer(player.MapId, player.Team);
		gSPacketIn.WriteInt(listPlayer.Count);
		foreach (CampBattlePlayerInfo item in listPlayer)
		{
			gSPacketIn.WriteInt(item.Player.PlayerCharacter.ID);
			gSPacketIn.WriteInt(item.Player.ZoneId);
			gSPacketIn.WriteBoolean(item.Player.PlayerCharacter.Sex);
			gSPacketIn.WriteString(item.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteInt(player.Team);
			gSPacketIn.WriteInt(item.PosX);
			gSPacketIn.WriteInt(item.PosY);
			gSPacketIn.WriteInt((int)item.StateType);
			gSPacketIn.WriteInt(item.TimerCount);
			gSPacketIn.WriteBoolean((item.Player.PlayerCharacter.typeVIP != 0) ? true : false);
			gSPacketIn.WriteInt(item.Player.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteInt(item.Player.PlayerCharacter.MountsType);
		}
		if (player.MapId == 0)
		{
			List<CampBattleMonsterInfo> livingMonster = GetLivingMonster(player.Team);
			gSPacketIn.WriteInt(livingMonster.Count);
			foreach (CampBattleMonsterInfo item2 in livingMonster)
			{
				gSPacketIn.WriteInt(item2.ID);
				gSPacketIn.WriteString(item2.ActionMovieName);
				gSPacketIn.WriteString("");
				gSPacketIn.WriteInt(item2.PosX);
				gSPacketIn.WriteInt(item2.PosY);
				gSPacketIn.WriteInt((int)item2.StateType);
				gSPacketIn.WriteInt(1);
			}
		}
		else
		{
			gSPacketIn.WriteInt(0);
		}
		gSPacketIn.WriteInt(player.Score);
		gSPacketIn.WriteBoolean(team.OpenGate);
		player.Player.SendTCP(gSPacketIn);
	}

	public void SendRemovePlayer(CampBattlePlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(player.ZoneId);
		gSPacketIn.WriteInt(player.UserID);
		player.Player.SendTCP(gSPacketIn);
		SendToTeam(gSPacketIn, player.MapId, player.Team);
	}

	public void SendUpdateMonsterStatus(CampBattleMonsterInfo monster)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(monster.ID);
		gSPacketIn.WriteInt((int)monster.StateType);
		SendToTeam(gSPacketIn, monster.Team);
	}

	public void SendUpdatePlayerStatus(CampBattlePlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(8);
		gSPacketIn.WriteInt(player.ZoneId);
		gSPacketIn.WriteInt(player.UserID);
		gSPacketIn.WriteInt((int)player.StateType);
		gSPacketIn.WriteInt((player.StateType == CampBattlePlayerState.DEAD) ? 1 : 0);
		SendToTeam(gSPacketIn, player.MapId, player.Team);
	}

	public void SendUpdateScore(CampBattlePlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteInt(player.Score);
		player.Player.SendTCP(gSPacketIn);
	}

	public void SendUpdateWinCount(CampBattlePlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(19);
		gSPacketIn.WriteInt(player.WinCount);
		player.Player.SendTCP(gSPacketIn);
	}

	public void SendAddPlayer(CampBattlePlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(player.UserID);
		gSPacketIn.WriteInt(player.ZoneId);
		gSPacketIn.WriteBoolean(player.Player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(player.NickName);
		gSPacketIn.WriteInt(player.Team);
		gSPacketIn.WriteInt(player.PosX);
		gSPacketIn.WriteInt(player.PosY);
		gSPacketIn.WriteInt((int)player.StateType);
		gSPacketIn.WriteInt(player.TimerCount);
		gSPacketIn.WriteBoolean((player.Player.PlayerCharacter.typeVIP != 0) ? true : false);
		gSPacketIn.WriteInt(player.Player.PlayerCharacter.VIPLevel);
		gSPacketIn.WriteInt(player.Player.PlayerCharacter.MountsType);
		SendToTeam(gSPacketIn, player);
	}

	public void SendPlayerMove(CampBattlePlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(player.PosX);
		gSPacketIn.WriteInt(player.PosY);
		gSPacketIn.WriteInt(player.ZoneId);
		gSPacketIn.WriteInt(player.UserID);
		SendToTeam(gSPacketIn, player);
	}

	public void SendCaptureStatue(CampBattlePlayerInfo player, bool isCapture)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(17);
		gSPacketIn.WriteBoolean(isCapture);
		if (isCapture)
		{
			gSPacketIn.WriteInt(player.ZoneId);
			gSPacketIn.WriteInt(player.UserID);
			gSPacketIn.WriteString(player.NickName);
			gSPacketIn.WriteInt(player.Team);
		}
		else
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString("");
			gSPacketIn.WriteInt(0);
		}
		SendToTeam(gSPacketIn, player.MapId, player.Team);
	}

	public void SendAllOpenClose()
	{
		GSPacketIn pkg = CampBattleOpenClosePkg();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			if (gamePlayer != null && gamePlayer.IsActive && gamePlayer.CanActive("campbattle"))
			{
				gamePlayer.SendTCP(pkg);
			}
		}
	}

	public GSPacketIn CampBattleOpenClosePkg()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(146);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteBoolean(IsOpenCampBattle);
		gSPacketIn.WriteDateTime(StartTime);
		gSPacketIn.WriteDateTime(EndTime);
		return gSPacketIn;
	}

	public void SendMessenge(string msg, int team)
	{
		SendMessenge(msg, 0, team);
	}

	public void SendMessenge(string msg, int map, int team)
	{
		List<CampBattlePlayerInfo> listPlayer = GetListPlayer(map, team);
		foreach (CampBattlePlayerInfo item in listPlayer)
		{
			if (item.IsActive && item.Player != null && item.Player.IsActive)
			{
				item.Player.SendMessage(msg);
			}
		}
	}

	public void SendToALL(GSPacketIn packet)
	{
		SendToALL(packet, null);
	}

	public void SendToALL(GSPacketIn packet, GamePlayer except)
	{
		lock (m_players)
		{
			foreach (CampBattlePlayerInfo value in m_players.Values)
			{
				if (value != null && value.IsActive && value.Player != null && value.Player.IsActive && value.Player != except)
				{
					value.Player.Out.SendTCP(packet);
				}
			}
		}
	}

	public void SendToTeam(GSPacketIn packet, int team)
	{
		SendToTeam(packet, 0, team);
	}

	public void SendToTeam(GSPacketIn packet, int map, int team)
	{
		List<CampBattlePlayerInfo> listPlayer = GetListPlayer(map, team);
		foreach (CampBattlePlayerInfo item in listPlayer)
		{
			if (item != null && item.IsActive && item.Player != null && item.Player.IsActive)
			{
				item.Player.SendTCP(packet);
			}
		}
	}

	public void SendToTeam(GSPacketIn packet, CampBattlePlayerInfo except)
	{
		List<CampBattlePlayerInfo> listPlayer = GetListPlayer(except.MapId, except.Team);
		foreach (CampBattlePlayerInfo item in listPlayer)
		{
			if (item != null && item.IsActive && item.Player != null && item.Player.IsActive && item != except)
			{
				item.Player.Out.SendTCP(packet);
			}
		}
	}
}

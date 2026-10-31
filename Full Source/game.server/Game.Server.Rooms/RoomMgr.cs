using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Rooms;

public class RoomMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static bool m_running;

	private static Queue<IAction> m_actionQueue;

	private static Thread m_thread;

	private static BaseRoom[] m_rooms;

	private static BaseWaitingRoom m_waitingRoom;

	private static BaseWorldBossRoom m_worldbossRoom;

	private static BaseSevenDoubleRoom[] m_sevenDoubleRoom;

	private static BaseChristmasRoom m_christmasRoom;

	private static BaseCatchInsectRoom m_catchInsectRoom;

	public static int MaxRaceLength = 40000;

	public static int TimehorseRaceCanRaceTimeDown = 10;

	private static BaseHorseRaceRoom[] m_horseRaceRoom;

	public static readonly int THREAD_INTERVAL = 40;

	public static readonly int CLEAR_ROOM_INTERVAL = 400;

	private static long m_clearTick = 0L;

	public static BaseSevenDoubleRoom[] SevenDoubleRoom => m_sevenDoubleRoom;

	public static BaseChristmasRoom ChristmasRoom => m_christmasRoom;

	public static BaseRoom[] Rooms => m_rooms;

	public static BaseWaitingRoom WaitingRoom => m_waitingRoom;

	public static BaseWorldBossRoom WorldBossRoom => m_worldbossRoom;

	public static BaseCatchInsectRoom CatchInsectRoom => m_catchInsectRoom;

	public static BaseHorseRaceRoom[] HorseRaceRoom => m_horseRaceRoom;

	public static object CampBattleRoom { get; internal set; }

	public static bool Setup(int maxRoom)
	{
		maxRoom = ((maxRoom < 1) ? 1 : maxRoom);
		m_thread = new Thread(RoomThread);
		m_actionQueue = new Queue<IAction>();
		m_rooms = new BaseRoom[maxRoom];
		for (int i = 0; i < maxRoom; i++)
		{
			m_rooms[i] = new BaseRoom(i + 1);
		}
		m_sevenDoubleRoom = new BaseSevenDoubleRoom[maxRoom];
		for (int j = 0; j < maxRoom; j++)
		{
			m_sevenDoubleRoom[j] = new BaseSevenDoubleRoom(j + 1);
		}
		m_horseRaceRoom = new BaseHorseRaceRoom[maxRoom];
		for (int k = 0; k < maxRoom; k++)
		{
			m_horseRaceRoom[k] = new BaseHorseRaceRoom(k + 1);
		}
		m_waitingRoom = new BaseWaitingRoom();
		m_worldbossRoom = new BaseWorldBossRoom();
		m_christmasRoom = new BaseChristmasRoom();
		m_catchInsectRoom = new BaseCatchInsectRoom();
		return true;
	}

	public static void Start()
	{
		if (!m_running)
		{
			m_running = true;
			m_thread.Start();
		}
	}

	public static void Stop()
	{
		if (m_running)
		{
			m_running = false;
			m_thread.Join();
		}
	}

	private static void RoomThread()
	{
		Thread.CurrentThread.Priority = ThreadPriority.Highest;
		long num = 0L;
		m_clearTick = TickHelper.GetTickCount();
		while (m_running)
		{
			long tickCount = TickHelper.GetTickCount();
			int num2 = 0;
			try
			{
				num2 = ExecuteActions();
				if (m_clearTick <= tickCount)
				{
					m_clearTick += CLEAR_ROOM_INTERVAL;
					ClearRooms(tickCount);
				}
			}
			catch (Exception exception)
			{
				log.Error("Room Mgr Thread Error:", exception);
			}
			long tickCount2 = TickHelper.GetTickCount();
			num += THREAD_INTERVAL - (tickCount2 - tickCount);
			if (tickCount2 - tickCount > THREAD_INTERVAL)
			{
				log.WarnFormat("Room Mgr is spent too much times: {0} ms,count:{1}", tickCount2 - tickCount, num2);
			}
			if (num > 0)
			{
				Thread.Sleep((int)num);
				num = 0L;
			}
			else if (num < -1000)
			{
				num += 1000;
			}
		}
	}

	private static int ExecuteActions()
	{
		IAction[] array = null;
		lock (m_actionQueue)
		{
			if (m_actionQueue.Count > 0)
			{
				array = new IAction[m_actionQueue.Count];
				m_actionQueue.CopyTo(array, 0);
				m_actionQueue.Clear();
			}
		}
		if (array != null)
		{
			IAction[] array2 = array;
			foreach (IAction action in array2)
			{
				try
				{
					long tickCount = TickHelper.GetTickCount();
					action.Execute();
					long tickCount2 = TickHelper.GetTickCount();
					if (tickCount2 - tickCount > THREAD_INTERVAL)
					{
						log.WarnFormat("RoomMgr action spent too much times:{0},{1}ms!", action.GetType(), tickCount2 - tickCount);
					}
				}
				catch (Exception exception)
				{
					log.Error("RoomMgr execute action error:", exception);
				}
			}
			return array.Length;
		}
		return 0;
	}

	public static void ClearRooms(long tick)
	{
		BaseRoom[] rooms = m_rooms;
		foreach (BaseRoom baseRoom in rooms)
		{
			if (baseRoom.IsUsing && baseRoom.PlayerCount == 0)
			{
				baseRoom.Stop();
			}
		}
	}

	public static void ClearPveRooms()
	{
		lock (m_rooms)
		{
			BaseRoom[] rooms = m_rooms;
			foreach (BaseRoom baseRoom in rooms)
			{
				if (baseRoom.IsUsing && (baseRoom.RoomType == eRoomType.Dungeon || baseRoom.RoomType == eRoomType.Element_PVE))
				{
					baseRoom.Stop();
				}
			}
		}
	}

	public static void AddAction(IAction action)
	{
		lock (m_actionQueue)
		{
			m_actionQueue.Enqueue(action);
		}
	}

	public static void CreateRoom(GamePlayer player, string name, string password, eRoomType roomType, byte timeType)
	{
		AddAction(new CreateRoomAction(player, name, password, roomType, timeType));
	}

	public static void CreateCryptBossRoom(GamePlayer player, int id, int level)
	{
		AddAction(new CreateCryptBossAction(player, id, level));
	}

	public static void CreateCatchBeastRoom(GamePlayer player)
	{
		AddAction(new CreateCatchBeastAction(player));
	}

	public static void CreateConsortiaBattleRoom(GamePlayer player)
	{
		AddAction(new CreateConsortiaBattleRoomAction(player));
	}

	public static void CreateConsortiaDomainRoom(GamePlayer player)
	{
		AddAction(new CreateConsortiaDomainRoomAction(player));
	}

	public static void CreateCampBattleRoom(GamePlayer player)
	{
		AddAction(new CreateCampBattleRoomAction(player));
	}

	public static void CreateTrialBattleRoom(GamePlayer player)
	{
		AddAction(new CreateTrialBattleRoomAction(player));
	}

	public static void CreateRingStationRoom(GamePlayer player, UserRingStationInfo challengePlayer)
	{
		AddAction(new CreateRingStationRoomAction(player, challengePlayer));
	}

	public static void CreateBattleRoom(GamePlayer player, eRoomType roomType)
	{
		AddAction(new CreateBattleRoomAction(player, roomType));
	}

	public static void CreateFightFootballTimeRoom(GamePlayer player, eRoomType roomType)
	{
		AddAction(new CreateFightFootballTimeRoomAction(player, roomType));
	}

	public static void CreateEncounterRoom(GamePlayer player, eRoomType roomType)
	{
		AddAction(new CreateEncounterRoomAction(player, roomType));
	}

	public static void CreateGroupBattleRoom(GamePlayer player, int groupType)
	{
		AddAction(new CreateGroupBattleRoomAction(player, groupType));
	}

	public static void CreateBombKingRoom(GamePlayer player)
	{
		AddAction(new CreateBombKingRoomAction(player));
	}

	public static void CreateSurvivalRoom(GamePlayer player, int groupType)
	{
		AddAction(new CreateSurvivalRoomAction(player, groupType));
	}

	public static void CreateConsortiaBossRoom(GamePlayer player, eRoomType roomType, int bossLevel)
	{
		AddAction(new CreateConsortiaBossAction(player, roomType, bossLevel));
	}

	public static void EnterRoom(GamePlayer player, int roomId, string pwd, int type, bool isInvite)
	{
		AddAction(new EnterRoomAction(player, roomId, pwd, type, isInvite));
	}

	public static void ExitRoom(BaseRoom room, GamePlayer player)
	{
		AddAction(new ExitRoomAction(room, player));
	}

	public static void StartGame(BaseRoom room)
	{
		AddAction(new StartGameAction(room));
	}

	public static void StartGameMission(BaseRoom room)
	{
		AddAction(new StartGameMissionAction(room));
	}

	public static void StartConsortiaBattle(BaseRoom red, BaseRoom blue)
	{
		AddAction(new StartConsortiaBattleRoomAction(red, blue));
	}

	public static void StartCampBattlePVP(BaseRoom red, BaseRoom blue)
	{
		AddAction(new StartCampBattleRoomAction(red, blue));
	}

	public static void UpdatePlayerState(GamePlayer player, byte state)
	{
		AddAction(new UpdatePlayerStateAction(player, player.CurrentRoom, state));
	}

	public static void UpdateRoomPos(BaseRoom room, int pos, bool isOpened, int place, int placeView)
	{
		AddAction(new UpdateRoomPosAction(room, pos, isOpened, place, placeView));
	}

	public static void KickPlayer(BaseRoom baseRoom, byte index)
	{
		AddAction(new KickPlayerAction(baseRoom, index));
	}

	public static void EnterWaitingRoom(GamePlayer player)
	{
		AddAction(new EnterWaitingRoomAction(player));
	}

	public static void ExitWaitingRoom(GamePlayer player)
	{
		AddAction(new ExitWaitRoomAction(player));
	}

	public static void CancelPickup(BattleServer server, BaseRoom room)
	{
		AddAction(new CancelPickupAction(server, room));
	}

	public static void UpdateRoomGameType(BaseRoom room, eRoomType roomType, byte timeMode, eHardLevel hardLevel, int levelLimits, int mapId, string password, string roomname, bool isCrosszone, bool isOpenBoss, string Pic, int currentFloor)
	{
		AddAction(new RoomSetupChangeAction(room, roomType, timeMode, hardLevel, levelLimits, mapId, password, roomname, isCrosszone, isOpenBoss, Pic, currentFloor));
	}

	internal static void SwitchTeam(GamePlayer gamePlayer)
	{
		AddAction(new SwitchTeamAction(gamePlayer));
	}

	public static void CreateHorseRaceRoom(GamePlayer player)
	{
		AddAction(new CreateHorseRaceRoomAction(player));
	}

	public static void ExitHorseRaceRoom(BaseHorseRaceRoom room, GamePlayer player)
	{
	}

	public static List<BaseRoom> GetAllUsingRoom()
	{
		List<BaseRoom> list = new List<BaseRoom>();
		lock (m_rooms)
		{
			BaseRoom[] rooms = m_rooms;
			foreach (BaseRoom baseRoom in rooms)
			{
				if (baseRoom.IsUsing)
				{
					list.Add(baseRoom);
				}
			}
		}
		return list;
	}

	public static List<BaseRoom> GetAllRooms()
	{
		List<BaseRoom> list = new List<BaseRoom>();
		lock (m_rooms)
		{
			BaseRoom[] rooms = m_rooms;
			foreach (BaseRoom baseRoom in rooms)
			{
				if (!baseRoom.IsEmpty)
				{
					list.Add(baseRoom);
				}
			}
		}
		return list;
	}

	public static List<BaseRoom> GetAllRooms(BaseRoom seffRoom)
	{
		List<BaseRoom> list = new List<BaseRoom>();
		if (seffRoom != null)
		{
			list.Add(seffRoom);
			if (seffRoom.Host != null && seffRoom.Host.PlayerState == ePlayerState.Away)
			{
				list.AddRange(GetAllPveRooms());
			}
			else
			{
				list.AddRange(GetAllMatchRooms());
			}
		}
		return list;
	}

	public static List<BaseRoom> GetAllPveRooms()
	{
		List<BaseRoom> list = new List<BaseRoom>();
		lock (m_rooms)
		{
			BaseRoom[] rooms = m_rooms;
			foreach (BaseRoom baseRoom in rooms)
			{
				if (baseRoom.IsUsing && (baseRoom.RoomType == eRoomType.Dungeon || baseRoom.RoomType == eRoomType.AcademyDungeon || baseRoom.RoomType == eRoomType.ActivityDungeon || baseRoom.RoomType == eRoomType.SpecialActivityDungeon || baseRoom.RoomType == eRoomType.Element_PVE))
				{
					list.Add(baseRoom);
				}
			}
		}
		return list;
	}

	public static List<BaseRoom> GetAllMatchRooms()
	{
		List<BaseRoom> list = new List<BaseRoom>();
		lock (m_rooms)
		{
			BaseRoom[] rooms = m_rooms;
			foreach (BaseRoom baseRoom in rooms)
			{
				if (baseRoom.IsUsing && (baseRoom.RoomType == eRoomType.Match || baseRoom.RoomType == eRoomType.Freedom))
				{
					list.Add(baseRoom);
				}
			}
		}
		return list;
	}

	public static void StartProxyGame(BaseRoom room, ProxyGame game)
	{
		AddAction(new StartProxyGameAction(room, game));
	}

	public static void StopProxyGame(BaseRoom room)
	{
		AddAction(new StopProxyGameAction(room));
	}

	public static void FakeRoom(string roomName, int playerCount, int maxPlayerCount, int roomType)
	{
		AddAction(new FakeRoomAction(roomName, playerCount, maxPlayerCount, roomType));
	}
}

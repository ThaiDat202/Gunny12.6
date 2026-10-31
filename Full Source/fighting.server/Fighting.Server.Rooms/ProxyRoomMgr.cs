using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Fighting.Server.Games;
using Game.Logic;
using log4net;

namespace Fighting.Server.Rooms;

public class ProxyRoomMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly int THREAD_INTERVAL = 20;

	public static readonly int PICK_UP_INTERVAL = 5000;

	public static readonly int CLEAR_ROOM_INTERVAL = 250;

	private static bool m_running = false;

	private static int m_serverId = 1;

	private static Queue<IAction> m_actionQueue = new Queue<IAction>();

	private static Thread m_thread;

	private static Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

	private static int RoomIndex = 0;

	private static ThreadSafeRandom rand = new ThreadSafeRandom();

	private static long m_nextPickTick = 0L;

	private static long m_nextClearTick = 0L;

	public static bool Setup()
	{
		m_thread = new Thread(RoomThread);
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

	public static void AddAction(IAction action)
	{
		lock (m_actionQueue)
		{
			m_actionQueue.Enqueue(action);
		}
	}

	private static void RoomThread()
	{
		long num = 0L;
		m_nextClearTick = TickHelper.GetTickCount();
		m_nextPickTick = TickHelper.GetTickCount();
		while (m_running)
		{
			long tickCount = TickHelper.GetTickCount();
			try
			{
				ExecuteActions();
				if (m_nextPickTick <= tickCount)
				{
					m_nextPickTick += PICK_UP_INTERVAL;
					PickUpRooms(tickCount);
				}
				if (m_nextClearTick <= tickCount)
				{
					m_nextClearTick += CLEAR_ROOM_INTERVAL;
					ClearRooms(tickCount);
					ClearAutoBotRooms();
				}
			}
			catch (Exception exception)
			{
				log.Error("Room Mgr Thread Error:", exception);
			}
			long tickCount2 = TickHelper.GetTickCount();
			num += THREAD_INTERVAL - (tickCount2 - tickCount);
			if (num > 0)
			{
				Thread.Sleep((int)num);
				num = 0L;
			}
			else if (num < -1000)
			{
				log.WarnFormat("Room Mgr is delay {0} ms!", num);
				num += 1000;
			}
		}
	}

	private static void ExecuteActions()
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
		if (array == null)
		{
			return;
		}
		IAction[] array2 = array;
		foreach (IAction action in array2)
		{
			try
			{
				action.Execute();
			}
			catch (Exception exception)
			{
				log.Error("RoomMgr execute action error:", exception);
			}
		}
	}

	private static void PickUpRooms(long tick)
	{
		List<ProxyRoom> waitMatchRoomUnsafe = GetWaitMatchRoomUnsafe();
		foreach (ProxyRoom item in waitMatchRoomUnsafe)
		{
			ProxyRoom proxyRoom = null;
			if (item.IsPlaying)
			{
				break;
			}
			if (item.IsCrossZone)
			{
				int num = 5;
				int num2 = 0;
				int num3 = 2;
				switch (item.RoomType)
				{
				case eRoomType.TrialBattle:
					foreach (ProxyRoom item2 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item2))
						{
							proxyRoom = item2;
						}
					}
					break;
				case eRoomType.TEAM_ROOM:
					foreach (ProxyRoom item3 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item3))
						{
							proxyRoom = item3;
						}
					}
					break;
				case eRoomType.EntertainmentPK:
					foreach (ProxyRoom item4 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item4))
						{
							proxyRoom = item4;
						}
					}
					break;
				case eRoomType.Entertainment:
					foreach (ProxyRoom item5 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item5))
						{
							proxyRoom = item5;
						}
					}
					break;
				case eRoomType.FightFootballTime:
				{
					ProxyRoom proxyRoom7 = null;
					ProxyRoom proxyRoom8 = null;
					ProxyRoom proxyRoom9 = null;
					ProxyRoom redC = null;
					ProxyRoom blueC = null;
					ProxyRoom redD2 = null;
					ProxyRoom blueD2 = null;
					foreach (ProxyRoom item6 in waitMatchRoomUnsafe)
					{
						if (proxyRoom7 == null && CanPickCrosszone(item, item6))
						{
							proxyRoom7 = item6;
						}
						if (proxyRoom8 == null && CanPickCrosszone(item, item6) && CanPickCrosszone(proxyRoom7, item6))
						{
							proxyRoom8 = item6;
						}
						if (proxyRoom9 == null && CanPickCrosszone(item, item6) && CanPickCrosszone(proxyRoom7, item6) && CanPickCrosszone(proxyRoom8, item6))
						{
							proxyRoom9 = item6;
						}
						if (proxyRoom8 != null && proxyRoom7 != null && proxyRoom9 != null)
						{
							StartMatchGame(item, proxyRoom7, proxyRoom8, proxyRoom9, redC, blueC, redD2, blueD2, 30, item.IsCrossZone);
							break;
						}
					}
					break;
				}
				case eRoomType.FightGround:
					foreach (ProxyRoom item7 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item7))
						{
							proxyRoom = item7;
						}
					}
					break;
				case eRoomType.ConsortiaBattle:
					foreach (ProxyRoom item8 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item8))
						{
							proxyRoom = item8;
						}
					}
					break;
				case eRoomType.PolarRegion:
					foreach (ProxyRoom item9 in waitMatchRoomUnsafe)
					{
						if (CanPickCrosszone(item, item9))
						{
							proxyRoom = item9;
						}
					}
					break;
				case eRoomType.SurvivalMode:
				{
					ProxyRoom proxyRoom2 = null;
					ProxyRoom proxyRoom3 = null;
					ProxyRoom proxyRoom4 = null;
					ProxyRoom proxyRoom5 = null;
					ProxyRoom proxyRoom6 = null;
					ProxyRoom redD = null;
					ProxyRoom blueD = null;
					foreach (ProxyRoom item10 in waitMatchRoomUnsafe)
					{
						if (proxyRoom2 == null && CanPickCrosszone(item, item10))
						{
							proxyRoom2 = item10;
						}
						if (proxyRoom3 == null && CanPickCrosszone(item, item10) && CanPickCrosszone(proxyRoom2, item10))
						{
							proxyRoom3 = item10;
						}
						if (proxyRoom4 == null && CanPickCrosszone(item, item10) && CanPickCrosszone(proxyRoom2, item10) && CanPickCrosszone(proxyRoom3, item10))
						{
							proxyRoom4 = item10;
						}
						if (proxyRoom5 == null && CanPickCrosszone(item, item10) && CanPickCrosszone(proxyRoom2, item10) && CanPickCrosszone(proxyRoom3, item10) && CanPickCrosszone(proxyRoom4, item10))
						{
							proxyRoom5 = item10;
						}
						if (proxyRoom6 == null && CanPickCrosszone(item, item10) && CanPickCrosszone(proxyRoom2, item10) && CanPickCrosszone(proxyRoom3, item10) && CanPickCrosszone(proxyRoom4, item10) && CanPickCrosszone(proxyRoom5, item10))
						{
							proxyRoom6 = item10;
						}
						if (proxyRoom3 != null && proxyRoom5 != null && proxyRoom2 != null && proxyRoom4 != null && proxyRoom6 != null && item.PickUpCount <= num3)
						{
							StartMatchGame(item, proxyRoom2, proxyRoom3, proxyRoom4, proxyRoom5, proxyRoom6, redD, blueD, 121, item.IsCrossZone);
							break;
						}
						if (proxyRoom3 != null && proxyRoom2 != null && proxyRoom4 != null && item.PickUpCount > num3)
						{
							StartMatchGame(item, proxyRoom2, proxyRoom3, proxyRoom4, proxyRoom5, proxyRoom6, redD, blueD, 121, item.IsCrossZone);
							break;
						}
					}
					break;
				}
				case eRoomType.BombKing:
					foreach (ProxyRoom item11 in waitMatchRoomUnsafe)
					{
						if (item11 == item || item11.RoomType != eRoomType.BombKing || item11.IsPlaying || item11.PlayerCount != item.PlayerCount || item.PickUpCount < item.GetPlayers()[0].BombKingDelay)
						{
							continue;
						}
						if (item.GetPlayers()[0].battleId > 0)
						{
							if (item.GetPlayers()[0].battleId == item11.GetPlayers()[0].PlayerCharacter.ID)
							{
								proxyRoom = item11;
							}
						}
						else
						{
							proxyRoom = item11;
						}
					}
					break;
				case eRoomType.Match:
					if (item.GameType == eGameType.Guild)
					{
						foreach (ProxyRoom item12 in waitMatchRoomUnsafe)
						{
							if ((item12.GuildId == 0 || item12.GuildId != item.GuildId) && item12 != item && item12.PlayerCount == item.PlayerCount && !item12.IsPlaying && item12.IsCrossZone && item12.GameType == eGameType.Guild)
							{
								num2 = CalculateScore(item, item12);
								if (num2 < num || item.PickUpCount > num3)
								{
									proxyRoom = item12;
								}
							}
						}
						break;
					}
					foreach (ProxyRoom item13 in waitMatchRoomUnsafe)
					{
						if (item13 != item && item13.PlayerCount == item.PlayerCount && !item13.IsPlaying && item13.IsCrossZone && (item13.GameType == eGameType.ALL || item13.GameType == eGameType.Free))
						{
							num2 = CalculateScore(item, item13);
							if (num2 < num || item.PickUpCount > num3)
							{
								proxyRoom = item13;
							}
						}
					}
					break;
				}
				if (proxyRoom != null)
				{
					StartMatchGame(item, proxyRoom, 1, isCrosszone: true);
					continue;
				}
				item.PickUpCount++;
				if (num < 10)
				{
					num += 2;
				}
				continue;
			}
			int num4 = 2;
			int num5 = 0;
			int num6 = 2;
			switch (item.RoomType)
			{
			case eRoomType.Encounter:
			{
				ProxyRoom proxyRoom10 = null;
				ProxyRoom proxyRoom11 = null;
				ProxyRoom proxyRoom12 = null;
				ProxyRoom redC2 = null;
				ProxyRoom blueC2 = null;
				ProxyRoom redD3 = null;
				ProxyRoom blueD3 = null;
				foreach (ProxyRoom item14 in waitMatchRoomUnsafe)
				{
					if (proxyRoom10 == null && CanPickLocal(item, item14))
					{
						proxyRoom10 = item14;
					}
					if (proxyRoom11 == null && CanPickLocal(item, item14) && CanPickLocal(proxyRoom10, item14))
					{
						proxyRoom11 = item14;
					}
					if (proxyRoom12 == null && CanPickLocal(item, item14) && CanPickLocal(proxyRoom10, item14) && CanPickLocal(proxyRoom11, item14))
					{
						proxyRoom12 = item14;
					}
					if (proxyRoom11 != null && proxyRoom10 != null && proxyRoom12 != null)
					{
						StartMatchGame(item, proxyRoom10, proxyRoom11, proxyRoom12, redC2, blueC2, redD3, blueD3, 16, item.IsCrossZone);
						break;
					}
				}
				break;
			}
			case eRoomType.ConsortiaBattle:
				foreach (ProxyRoom item15 in waitMatchRoomUnsafe)
				{
					if (CanPickLocal(item, item15))
					{
						proxyRoom = item15;
					}
				}
				break;
			case eRoomType.TEAM_ROOM:
				foreach (ProxyRoom item16 in waitMatchRoomUnsafe)
				{
					if (CanPickLocal(item, item16))
					{
						proxyRoom = item16;
					}
				}
				break;
			case eRoomType.SingleBattle:
			{
				ProxyRoom proxyRoom13 = null;
				ProxyRoom proxyRoom14 = null;
				ProxyRoom proxyRoom15 = null;
				ProxyRoom proxyRoom16 = null;
				ProxyRoom proxyRoom17 = null;
				ProxyRoom proxyRoom18 = null;
				ProxyRoom proxyRoom19 = null;
				foreach (ProxyRoom item17 in waitMatchRoomUnsafe)
				{
					if (proxyRoom13 == null && CanPickLocal(item, item17))
					{
						proxyRoom13 = item17;
					}
					if (item.groupType == 7 && proxyRoom13 != null)
					{
						StartMatchGame(item, proxyRoom13, proxyRoom14, proxyRoom15, proxyRoom16, proxyRoom17, proxyRoom18, proxyRoom19, item.groupType, item.IsCrossZone);
						break;
					}
					if (proxyRoom14 == null && CanPickLocal(item, item17) && CanPickLocal(proxyRoom13, item17))
					{
						proxyRoom14 = item17;
					}
					if (proxyRoom15 == null && CanPickLocal(item, item17) && CanPickLocal(proxyRoom13, item17) && CanPickLocal(proxyRoom14, item17))
					{
						proxyRoom15 = item17;
					}
					if (item.groupType == 8 && proxyRoom14 != null && proxyRoom13 != null && proxyRoom15 != null)
					{
						StartMatchGame(item, proxyRoom13, proxyRoom14, proxyRoom15, proxyRoom16, proxyRoom17, proxyRoom18, proxyRoom19, item.groupType, item.IsCrossZone);
						break;
					}
					if (proxyRoom16 == null && CanPickLocal(item, item17) && CanPickLocal(proxyRoom13, item17) && CanPickLocal(proxyRoom14, item17) && CanPickLocal(proxyRoom15, item17))
					{
						proxyRoom16 = item17;
					}
					if (proxyRoom17 == null && CanPickLocal(item, item17) && CanPickLocal(proxyRoom13, item17) && CanPickLocal(proxyRoom14, item17) && CanPickLocal(proxyRoom15, item17) && CanPickLocal(proxyRoom16, item17))
					{
						proxyRoom17 = item17;
					}
					if (item.groupType == 9 && proxyRoom14 != null && proxyRoom16 != null && proxyRoom13 != null && proxyRoom15 != null && proxyRoom17 != null)
					{
						StartMatchGame(item, proxyRoom13, proxyRoom14, proxyRoom15, proxyRoom16, proxyRoom17, proxyRoom18, proxyRoom19, item.groupType, item.IsCrossZone);
						break;
					}
					if (proxyRoom18 == null && CanPickLocal(item, item17) && CanPickLocal(proxyRoom13, item17) && CanPickLocal(proxyRoom14, item17) && CanPickLocal(proxyRoom15, item17) && CanPickLocal(proxyRoom16, item17) && CanPickLocal(proxyRoom17, item17))
					{
						proxyRoom18 = item17;
					}
					if (proxyRoom19 == null && CanPickLocal(item, item17) && CanPickLocal(proxyRoom13, item17) && CanPickLocal(proxyRoom14, item17) && CanPickLocal(proxyRoom15, item17) && CanPickLocal(proxyRoom16, item17) && CanPickLocal(proxyRoom17, item17) && CanPickLocal(proxyRoom18, item17))
					{
						proxyRoom19 = item17;
					}
					if (item.groupType == 10 && proxyRoom14 != null && proxyRoom16 != null && proxyRoom18 != null && proxyRoom13 != null && proxyRoom15 != null && proxyRoom17 != null && proxyRoom19 != null)
					{
						StartMatchGame(item, proxyRoom13, proxyRoom14, proxyRoom15, proxyRoom16, proxyRoom17, proxyRoom18, proxyRoom19, item.groupType, item.IsCrossZone);
						break;
					}
				}
				break;
			}
			case eRoomType.PolarRegion:
				foreach (ProxyRoom item18 in waitMatchRoomUnsafe)
				{
					if (CanPickLocal(item, item18))
					{
						proxyRoom = item18;
					}
				}
				break;
			case eRoomType.Match:
				if (item.GameType == eGameType.Guild)
				{
					foreach (ProxyRoom item19 in waitMatchRoomUnsafe)
					{
						if ((item19.GuildId == 0 || item19.GuildId != item.GuildId) && item19 != item && item19.PlayerCount == item.PlayerCount && !item19.IsPlaying && !item19.IsCrossZone && item19.ZoneId == item.ZoneId && item19.GameType == eGameType.Guild)
						{
							num5 = CalculateScore(item, item19);
							if (num5 < num4 || item.PickUpCount > num6)
							{
								proxyRoom = item19;
							}
						}
					}
				}
				else
				{
					if (item.RoomType == eRoomType.RingStation)
					{
						break;
					}
					foreach (ProxyRoom item20 in waitMatchRoomUnsafe)
					{
						if (item20 != item && !item.isAutoBot && item20.PlayerCount == item.PlayerCount && !item20.IsPlaying && !item20.IsCrossZone && item20.ZoneId == item.ZoneId && (item20.GameType == eGameType.ALL || item20.GameType == eGameType.Free))
						{
							num5 = CalculateScore(item, item20);
							if (num5 < num4 && item.PickUpCount < num6 && !item20.isAutoBot)
							{
								proxyRoom = item20;
							}
						}
					}
				}
				break;
			}
			if (proxyRoom != null)
			{
				StartMatchGame(item, proxyRoom, 1);
			}
			else
			{
				if (item.RoomType != 0)
				{
					continue;
				}
				if (item.PickUpNPC && !item.isAutoBot)
				{
					foreach (ProxyRoom item21 in waitMatchRoomUnsafe)
					{
						if (item21 != item && !item21.IsPlaying && item21.isAutoBot && item21.IsFreedom && item.NpcId == item21.NpcId)
						{
							Console.WriteLine("Start fight with AutoBot No.{0}", item.NpcId);
							StartMatchGame(item, item21, 1);
						}
					}
				}
				if (item.isAutoBot && !item.IsPlaying)
				{
					item.PickUpCount--;
					continue;
				}
				item.PickUpCount++;
				if (num4 < 10)
				{
					num4 += 3;
				}
			}
		}
	}

	public static bool CanPickLocal(ProxyRoom red, ProxyRoom blue)
	{
		if (blue != red && blue.RoomType == red.RoomType && !blue.IsPlaying && !blue.IsCrossZone && blue.ZoneId == red.ZoneId && blue.PlayerCount == red.PlayerCount)
		{
			return true;
		}
		return false;
	}

	public static bool CanPickCrosszone(ProxyRoom red, ProxyRoom blue)
	{
		if (blue != red && blue.RoomType == red.RoomType && !blue.IsPlaying && blue.IsCrossZone && blue.PlayerCount == red.PlayerCount)
		{
			return true;
		}
		return false;
	}

	private static int CalculateScore(ProxyRoom red, ProxyRoom blue)
	{
		if (GameProperties.servidor == "nexus")
		{
			if ((DateTime.Now - red.createDate).TotalSeconds >= 30.0 || (DateTime.Now - blue.createDate).TotalSeconds >= 30.0)
			{
				return 1;
			}
		}
		else if ((DateTime.Now - red.createDate).TotalSeconds >= 120.0 && (DateTime.Now - blue.createDate).TotalSeconds >= 120.0)
		{
			return 1;
		}
		if (red.PickUpCount < 3)
		{
			int num = Math.Abs(red.FightPower - blue.FightPower);
			if (red.FightPower.ToString().Length < 5 && blue.FightPower.ToString().Length < 5 && num < 10000)
			{
				return 1;
			}
			if (red.FightPower.ToString().Length > 5 && blue.FightPower.ToString().Length > 5 && num < 100000)
			{
				return 1;
			}
			if (red.FightPower.ToString().Length > 6 && blue.FightPower.ToString().Length > 6 && num < 600000)
			{
				return 1;
			}
		}
		return Math.Abs(red.AvgLevel - blue.AvgLevel);
	}

	private static void ClearRooms(long tick)
	{
		List<ProxyRoom> list = new List<ProxyRoom>();
		foreach (ProxyRoom value in m_rooms.Values)
		{
			if (!value.IsPlaying && value.Game != null)
			{
				list.Add(value);
			}
		}
		foreach (ProxyRoom item in list)
		{
			m_rooms.Remove(item.RoomId);
			try
			{
				item.Dispose();
			}
			catch (Exception exception)
			{
				log.Error("Room dispose error:", exception);
			}
		}
	}

	private static void ClearAutoBotRooms()
	{
		List<ProxyRoom> list = new List<ProxyRoom>();
		foreach (ProxyRoom value in m_rooms.Values)
		{
			if (!value.IsPlaying && value.PickUpCount < -1)
			{
				list.Add(value);
			}
		}
		foreach (ProxyRoom item in list)
		{
			m_rooms.Remove(item.RoomId);
			try
			{
				item.Dispose();
			}
			catch (Exception exception)
			{
				log.Error("Room dispose error:", exception);
			}
		}
	}

	public static void StartMatchGame(ProxyRoom red1, ProxyRoom blue1, int group)
	{
		StartMatchGame(red1, blue1, null, null, null, null, null, null, group, isCrosszone: false);
	}

	public static void StartMatchGame(ProxyRoom red1, ProxyRoom blue1, int group, bool isCrosszone)
	{
		StartMatchGame(red1, blue1, null, null, null, null, null, null, group, isCrosszone);
	}

	public static void StartMatchGame(ProxyRoom redA, ProxyRoom blueA, ProxyRoom redB, ProxyRoom blueB, ProxyRoom redC, ProxyRoom blueC, ProxyRoom redD, ProxyRoom blueD, int group, bool isCrosszone)
	{
		int mapIndex = MapMgr.GetMapIndex(0, 0, m_serverId);
		eGameType gameType = eGameType.Free;
		eRoomType roomType = eRoomType.Match;
		if (redA.GameType == blueA.GameType)
		{
			gameType = redA.GameType;
			roomType = redA.RoomType;
		}
		if (redA.RoomType == eRoomType.FightFootballTime)
		{
			mapIndex = 1313;
		}
		if (redA.RoomType == eRoomType.PolarRegion)
		{
			int[] array = new int[4] { 1455, 1456, 1457, 1458 };
			int num = rand.Next(array.Length);
			mapIndex = array[num];
		}
		switch (group)
		{
		case 8:
		case 16:
		case 30:
		{
			List<IGamePlayer> players9 = redA.GetPlayers();
			List<IGamePlayer> players10 = blueA.GetPlayers();
			List<IGamePlayer> players11 = redB.GetPlayers();
			List<IGamePlayer> players12 = blueB.GetPlayers();
			BaseGame baseGame2 = GameMgr.StartSingleBattle2vs2(players9, redA, players10, blueA, players11, redB, players12, blueB, mapIndex, roomType, gameType, 2, isCrosszone);
			if (baseGame2 != null)
			{
				blueA.StartGame(baseGame2);
				redA.StartGame(baseGame2);
				blueB.StartGame(baseGame2);
				redB.StartGame(baseGame2);
			}
			return;
		}
		case 9:
		{
			List<IGamePlayer> players19 = redA.GetPlayers();
			List<IGamePlayer> players20 = blueA.GetPlayers();
			List<IGamePlayer> players21 = redB.GetPlayers();
			List<IGamePlayer> players22 = blueB.GetPlayers();
			List<IGamePlayer> players23 = redC.GetPlayers();
			List<IGamePlayer> players24 = blueC.GetPlayers();
			BaseGame baseGame5 = GameMgr.StartSingleBattle3vs3(players19, redA, players20, blueA, players21, redB, players22, blueB, players23, redC, players24, blueC, mapIndex, roomType, gameType, 2, isCrosszone);
			if (baseGame5 != null)
			{
				blueA.StartGame(baseGame5);
				redA.StartGame(baseGame5);
				blueB.StartGame(baseGame5);
				redB.StartGame(baseGame5);
				blueC.StartGame(baseGame5);
				redC.StartGame(baseGame5);
			}
			return;
		}
		case 10:
		{
			List<IGamePlayer> players = redA.GetPlayers();
			List<IGamePlayer> players2 = blueA.GetPlayers();
			List<IGamePlayer> players3 = redB.GetPlayers();
			List<IGamePlayer> players4 = blueB.GetPlayers();
			List<IGamePlayer> players5 = redC.GetPlayers();
			List<IGamePlayer> players6 = blueC.GetPlayers();
			List<IGamePlayer> players7 = redC.GetPlayers();
			List<IGamePlayer> players8 = blueC.GetPlayers();
			BaseGame baseGame = GameMgr.StartSingleBattle4vs4(players, redA, players2, blueA, players3, redB, players4, blueB, players5, redC, players6, blueC, players7, redD, players8, blueD, mapIndex, roomType, gameType, 2, isCrosszone);
			if (baseGame != null)
			{
				blueA.StartGame(baseGame);
				redA.StartGame(baseGame);
				blueB.StartGame(baseGame);
				redB.StartGame(baseGame);
				blueC.StartGame(baseGame);
				redC.StartGame(baseGame);
				blueD.StartGame(baseGame);
				redD.StartGame(baseGame);
			}
			return;
		}
		case 121:
		{
			List<IGamePlayer> players13 = redA.GetPlayers();
			List<IGamePlayer> players14 = blueA.GetPlayers();
			List<IGamePlayer> players15 = redB.GetPlayers();
			List<IGamePlayer> players16 = blueB.GetPlayers();
			if (redC == null && blueC == null)
			{
				BaseGame baseGame3 = GameMgr.StartSingleBattle2vs2(players13, redA, players14, blueA, players15, redB, players16, blueB, mapIndex, roomType, gameType, 2, isCrosszone);
				if (baseGame3 != null)
				{
					blueA.StartGame(baseGame3);
					redA.StartGame(baseGame3);
					blueB.StartGame(baseGame3);
					redB.StartGame(baseGame3);
				}
				return;
			}
			List<IGamePlayer> players17 = redC.GetPlayers();
			List<IGamePlayer> players18 = blueC.GetPlayers();
			BaseGame baseGame4 = GameMgr.StartSingleBattle3vs3(players13, redA, players14, blueA, players15, redB, players16, blueB, players17, redC, players18, blueC, mapIndex, roomType, gameType, 2, isCrosszone);
			if (baseGame4 != null)
			{
				blueA.StartGame(baseGame4);
				redA.StartGame(baseGame4);
				blueB.StartGame(baseGame4);
				redB.StartGame(baseGame4);
				blueC.StartGame(baseGame4);
				redC.StartGame(baseGame4);
			}
			return;
		}
		}
		List<IGamePlayer> players25 = redA.GetPlayers();
		List<IGamePlayer> players26 = blueA.GetPlayers();
		BaseGame baseGame6 = GameMgr.StartBattleGame(players25, redA, players26, blueA, mapIndex, roomType, gameType, 2, isCrosszone);
		if (baseGame6 != null)
		{
			blueA.StartGame(baseGame6);
			redA.StartGame(baseGame6);
			if (baseGame6.GameType == eGameType.Guild)
			{
				redA.Client.SendConsortiaAlly(players25[0].PlayerCharacter.ConsortiaID, players26[0].PlayerCharacter.ConsortiaID, baseGame6.Id);
			}
		}
	}

	public static void StartWithNpcUnsafe(ProxyRoom room)
	{
		List<ProxyRoom> waitMatchRoomUnsafe = GetWaitMatchRoomUnsafe();
		foreach (ProxyRoom item in waitMatchRoomUnsafe)
		{
			if (item.isAutoBot && !item.IsPlaying && item.Game == null && item.NpcId == room.NpcId)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("Start fight with AutoBot or VPlayer No.{0}", room.NpcId);
				Console.ResetColor();
				StartMatchGame(room, item, 1);
			}
		}
	}

	public static bool AddRoomUnsafe(ProxyRoom room)
	{
		if (!m_rooms.ContainsKey(room.RoomId))
		{
			m_rooms.Add(room.RoomId, room);
			return true;
		}
		return false;
	}

	public static bool RemoveRoomUnsafe(int roomId)
	{
		if (m_rooms.ContainsKey(roomId))
		{
			m_rooms.Remove(roomId);
			return true;
		}
		return false;
	}

	public static ProxyRoom GetRoomUnsafe(int roomId)
	{
		if (m_rooms.ContainsKey(roomId))
		{
			return m_rooms[roomId];
		}
		return null;
	}

	public static ProxyRoom[] GetAllRoom()
	{
		lock (m_rooms)
		{
			return GetAllRoomUnsafe();
		}
	}

	public static ProxyRoom[] GetAllRoomUnsafe()
	{
		ProxyRoom[] array = new ProxyRoom[m_rooms.Values.Count];
		m_rooms.Values.CopyTo(array, 0);
		return array;
	}

	public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
	{
		List<ProxyRoom> list = new List<ProxyRoom>();
		foreach (ProxyRoom value in m_rooms.Values)
		{
			if (!value.IsPlaying && value.Game == null)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static int NextRoomId()
	{
		return Interlocked.Increment(ref RoomIndex);
	}

	public static void AddRoom(ProxyRoom room)
	{
		AddAction(new AddRoomAction(room));
	}

	public static void RemoveRoom(ProxyRoom room)
	{
		AddAction(new RemoveRoomAction(room));
	}
}

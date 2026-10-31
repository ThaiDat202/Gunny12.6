using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Fighting.Server.GameObjects;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Fighting.Server.Games;

public class GameMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly long THREAD_INTERVAL = 40L;

	private static Dictionary<int, BaseGame> m_games;

	private static Thread m_thread;

	private static bool m_running;

	private static int m_serverId;

	private static int m_boxBroadcastLevel;

	private static int m_gameId;

	private static readonly int CLEAR_GAME_INTERVAL = 60000;

	private static long m_clearGamesTimer;

	public static int BoxBroadcastLevel => m_boxBroadcastLevel;

	public static bool Setup(int serverId, int boxBroadcastLevel)
	{
		m_thread = new Thread(GameThread);
		m_games = new Dictionary<int, BaseGame>();
		m_serverId = serverId;
		m_boxBroadcastLevel = boxBroadcastLevel;
		m_gameId = 0;
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

	private static void GameThread()
	{
		long num = 0L;
		m_clearGamesTimer = TickHelper.GetTickCount();
		while (m_running)
		{
			long tickCount = TickHelper.GetTickCount();
			try
			{
				UpdateGames(tickCount);
				ClearStoppedGames(tickCount);
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

	private static void UpdateGames(long tick)
	{
		IList games = GetGames();
		if (games == null)
		{
			return;
		}
		foreach (BaseGame item in games)
		{
			try
			{
				item.Update(tick);
			}
			catch (Exception exception)
			{
				log.Error("Game  updated error:", exception);
			}
		}
	}

	private static void ClearStoppedGames(long tick)
	{
		if (m_clearGamesTimer > tick)
		{
			return;
		}
		m_clearGamesTimer += CLEAR_GAME_INTERVAL;
		ArrayList arrayList = new ArrayList();
		lock (m_games)
		{
			foreach (BaseGame value in m_games.Values)
			{
				if (value.GameState == eGameState.Stopped)
				{
					arrayList.Add(value);
				}
			}
			foreach (BaseGame item in arrayList)
			{
				m_games.Remove(item.Id);
				try
				{
					item.Dispose();
				}
				catch (Exception exception)
				{
					log.Error("game dispose error:", exception);
				}
			}
		}
	}

	public static List<BaseGame> GetGames()
	{
		List<BaseGame> list = new List<BaseGame>();
		lock (m_games)
		{
			list.AddRange(m_games.Values);
		}
		return list;
	}

	public static BaseGame FindGame(int id)
	{
		lock (m_games)
		{
			if (m_games.ContainsKey(id))
			{
				return m_games[id];
			}
		}
		return null;
	}

	public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType, bool isCrosszone)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				BattleGame battleGame = new BattleGame(m_gameId++, red, roomRed, blue, roomBlue, null, null, null, null, null, null, null, null, null, null, null, null, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(battleGame.Id, battleGame);
				}
				battleGame.Prepare();
				SendStartMessage(battleGame, isCrosszone);
				return battleGame;
			}
			log.ErrorFormat("Can not find mapid: {0}", mapIndex2);
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create battle game error:", exception);
			return null;
		}
	}

	public static BattleGame StartSingleBattle2vs2(List<IGamePlayer> red1, ProxyRoom roomRed1, List<IGamePlayer> blue1, ProxyRoom roomBlue1, List<IGamePlayer> red2, ProxyRoom roomRed2, List<IGamePlayer> blue2, ProxyRoom roomBlue2, int mapIndex, eRoomType roomType, eGameType gameType, int timeType, bool isCrosszone)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				BattleGame battleGame = new BattleGame(m_gameId++, red1, roomRed1, blue1, roomBlue1, red2, roomRed2, blue2, roomBlue2, null, null, null, null, null, null, null, null, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(battleGame.Id, battleGame);
				}
				battleGame.Prepare();
				SendStartMessage(battleGame, isCrosszone);
				return battleGame;
			}
			log.ErrorFormat("Can not find mapid: {0}", mapIndex2);
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create battle game error:", exception);
			return null;
		}
	}

	public static BattleGame StartSingleBattle3vs3(List<IGamePlayer> red1, ProxyRoom roomRed1, List<IGamePlayer> blue1, ProxyRoom roomBlue1, List<IGamePlayer> red2, ProxyRoom roomRed2, List<IGamePlayer> blue2, ProxyRoom roomBlue2, List<IGamePlayer> red3, ProxyRoom roomRed3, List<IGamePlayer> blue3, ProxyRoom roomBlue3, int mapIndex, eRoomType roomType, eGameType gameType, int timeType, bool isCrosszone)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				BattleGame battleGame = new BattleGame(m_gameId++, red1, roomRed1, blue1, roomBlue1, red2, roomRed2, blue2, roomBlue2, red3, roomRed3, blue3, roomBlue3, null, null, null, null, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(battleGame.Id, battleGame);
				}
				battleGame.Prepare();
				SendStartMessage(battleGame, isCrosszone);
				return battleGame;
			}
			log.ErrorFormat("找不到地图: {0}", mapIndex2);
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create battle game error:", exception);
			return null;
		}
	}

	public static BattleGame StartSingleBattle4vs4(List<IGamePlayer> red1, ProxyRoom roomRed1, List<IGamePlayer> blue1, ProxyRoom roomBlue1, List<IGamePlayer> red2, ProxyRoom roomRed2, List<IGamePlayer> blue2, ProxyRoom roomBlue2, List<IGamePlayer> red3, ProxyRoom roomRed3, List<IGamePlayer> blue3, ProxyRoom roomBlue3, List<IGamePlayer> red4, ProxyRoom roomRed4, List<IGamePlayer> blue4, ProxyRoom roomBlue4, int mapIndex, eRoomType roomType, eGameType gameType, int timeType, bool isCrosszone)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				BattleGame battleGame = new BattleGame(m_gameId++, red1, roomRed1, blue1, roomBlue1, red2, roomRed2, blue2, roomBlue2, red3, roomRed3, blue3, roomBlue3, red4, roomRed4, blue4, roomBlue4, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(battleGame.Id, battleGame);
				}
				battleGame.Prepare();
				SendStartMessage(battleGame, isCrosszone);
				return battleGame;
			}
			log.ErrorFormat("Can not find mapid: {0}", mapIndex2);
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create battle game error:", exception);
			return null;
		}
	}

	public static void SendStartMessage(BattleGame game, bool isCrosszone)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(2);
		string str = "";
		if (CanUseBuff(game.GameType))
		{
			foreach (Player allFightPlayer in game.GetAllFightPlayers())
			{
				(allFightPlayer.PlayerDetail as ProxyPlayer).m_antiAddictionRate = 1.0;
				GSPacketIn pkg = SendBufferList(allFightPlayer, (allFightPlayer.PlayerDetail as ProxyPlayer).Buffers);
				game.SendToAll(pkg);
			}
			str = LanguageMgr.GetTranslation("StartMessage.free");
		}
		if (game.RoomType == eRoomType.FightFootballTime)
		{
			str = LanguageMgr.GetTranslation("FightFootballTime.Msg");
		}
		if (game.RoomType == eRoomType.SurvivalMode)
		{
			str = LanguageMgr.GetTranslation("SurvivalMode.Msg");
		}
		if (game.RoomType == eRoomType.RingStation)
		{
			str = LanguageMgr.GetTranslation("RingStation.Msg");
		}
		gSPacketIn.WriteString(str);
		game.SendToAll(gSPacketIn);
		string text = "Local";
		if (isCrosszone)
		{
			text = "Crosszone";
			Console.ForegroundColor = ConsoleColor.Magenta;
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Green;
		}
		Console.WriteLine($"Create {game.RoomType} with {game.PlayerCount} player, createID {game.Id} - {text}");
		Console.ResetColor();
	}

	public static bool CanUseBuff(eGameType gameType)
	{
		switch (gameType)
		{
		case eGameType.Free:
		case eGameType.Guild:
		case eGameType.ALL:
		case eGameType.GuildLeageRank:
		case eGameType.Encounter:
		case eGameType.ConsortiaBattle:
		case eGameType.BattleGame:
		case eGameType.CampBattleModelPve:
		case eGameType.RingStation:
		case eGameType.EntertainmentMode:
		case eGameType.EntertainmentModePK:
		case eGameType.Survival:
			return true;
		default:
			return false;
		}
	}

	public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(186, player.Id);
		gSPacketIn.WriteInt(infos.Count);
		foreach (BufferInfo info in infos)
		{
			gSPacketIn.WriteInt(info.Type);
			gSPacketIn.WriteBoolean(info.IsExist);
			gSPacketIn.WriteDateTime(info.BeginDate);
			gSPacketIn.WriteInt(info.ValidDate);
			gSPacketIn.WriteInt(info.Value);
		}
		return gSPacketIn;
	}
}

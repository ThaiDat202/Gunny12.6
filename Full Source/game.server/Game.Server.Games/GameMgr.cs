using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Server.CampBattle;
using Game.Server.ConsortiaDomain;
using Game.Server.GuildBattle;
using Game.Server.RingStation;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Games;

public class GameMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly long THREAD_INTERVAL = 40L;

	private static List<BaseGame> m_games;

	private static GuildBattleMgr m_guildBattle;

	private static CampBattleControl m_campBattle;

	private static ConsortiaDomainMgr m_consortiaDomain;

	private static Thread m_thread;

	private static bool m_running;

	private static int m_serverId;

	private static int m_boxBroadcastLevel;

	private static int m_gameId;

	private static DateTime m_synDate;

	private static readonly int CLEAR_GAME_INTERVAL = 5000;

	private static readonly int UPDATE_GAME_INTERVAL = 60000;

	private static long m_clearGamesTimer;

	private static long m_updateGamesTimer;

	public static GuildBattleMgr GuildBattle => m_guildBattle;

	public static CampBattleControl CampBattle => m_campBattle;

	public static ConsortiaDomainMgr ConsortiaDomain => m_consortiaDomain;

	public static int SynDate => DateTime.Compare(m_synDate.AddSeconds(THREAD_INTERVAL), DateTime.Now);

	public static bool CityBattleOpen { get; set; }

	public static int BoxBroadcastLevel => m_boxBroadcastLevel;

	public static bool Setup(int serverId, int boxBroadcastLevel)
	{
		string path = AppDomain.CurrentDomain.BaseDirectory + "datas";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		m_thread = new Thread(GameThread);
		m_games = new List<BaseGame>();
		m_guildBattle = new GuildBattleMgr();
		m_campBattle = new CampBattleControl();
		m_consortiaDomain = new ConsortiaDomainMgr();
		m_serverId = serverId;
		m_boxBroadcastLevel = boxBroadcastLevel;
		m_gameId = 0;
		m_synDate = DateTime.Now;
		CityBattleOpen = false;
		return true;
	}

	public static bool Start()
	{
		if (!m_running)
		{
			m_running = true;
			m_thread.Start();
			return true;
		}
		return false;
	}

	public static void Stop()
	{
		if (m_running)
		{
			m_running = false;
			if (!m_thread.Join(1000))
			{
				m_thread.Abort();
			}
		}
	}

	private static void GameThread()
	{
		Thread.CurrentThread.Priority = ThreadPriority.Highest;
		long num = 0L;
		m_clearGamesTimer = TickHelper.GetTickCount();
		m_updateGamesTimer = TickHelper.GetTickCount();
		while (m_running)
		{
			long tickCount = TickHelper.GetTickCount();
			try
			{
				UpdateGames(tickCount);
				UpdateBots(tickCount);
				ClearStoppedGames(tickCount);
				UpdateGuildBattle(tickCount);
				UpdateCampBattle(tickCount);
			}
			catch (Exception exception)
			{
				log.Error("Game Mgr Thread Error:", exception);
			}
			long tickCount2 = TickHelper.GetTickCount();
			num += THREAD_INTERVAL - (tickCount2 - tickCount);
			if (num > 0)
			{
				if ((int)num > UPDATE_GAME_INTERVAL)
				{
					Thread.Sleep(UPDATE_GAME_INTERVAL);
					if (log.IsInfoEnabled)
					{
						log.WarnFormat("Game Mgr current balance {0} ms!", num);
					}
				}
				else
				{
					Thread.Sleep((int)num);
				}
				num = 0L;
			}
			else if (num < -1000)
			{
				if (log.IsInfoEnabled)
				{
					log.WarnFormat("Game Mgr is delay {0} ms!", num);
				}
				num += 1000;
			}
			m_synDate = DateTime.Now;
		}
	}

	private static int UpdateBots(long tick)
	{
		IList allPlayer = RingStationMgr.GetAllPlayer();
		if (allPlayer == null)
		{
			return 0;
		}
		foreach (VirtualGamePlayer item in allPlayer)
		{
			try
			{
				item.Update(tick);
			}
			catch (Exception exception)
			{
				log.Error("Auto Bot updated error:", exception);
			}
		}
		return allPlayer.Count;
	}

	public static void ClearAllGames()
	{
		ArrayList arrayList = new ArrayList();
		lock (m_games)
		{
			foreach (BaseGame game in m_games)
			{
				arrayList.Add(game);
			}
			foreach (BaseGame item in arrayList)
			{
				m_games.Remove(item);
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
			foreach (BaseGame game in m_games)
			{
				if (game.GameState == eGameState.Stopped)
				{
					arrayList.Add(game);
				}
			}
			foreach (BaseGame item in arrayList)
			{
				m_games.Remove(item);
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

	private static void UpdateGuildBattle(long tick)
	{
		try
		{
			m_guildBattle.Update(tick);
		}
		catch (Exception exception)
		{
			log.Error("Game GuildBattle updated error:", exception);
		}
	}

	private static void UpdateCampBattle(long tick)
	{
		try
		{
			m_campBattle.Update(tick);
		}
		catch (Exception exception)
		{
			log.Error("Game CampBattle updated error:", exception);
		}
	}

	private static int UpdateGames(long tick)
	{
		IList allGame = GetAllGame();
		if (allGame != null)
		{
			foreach (BaseGame item in allGame)
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
			if (m_updateGamesTimer <= tick)
			{
				m_updateGamesTimer += UPDATE_GAME_INTERVAL;
			}
			return allGame.Count;
		}
		return 0;
	}

	public static List<BaseGame> GetAllGame()
	{
		List<BaseGame> list = new List<BaseGame>();
		lock (m_games)
		{
			list.AddRange(m_games);
		}
		return list;
	}

	public static BaseGame StartChallengePVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, BaseRoom redRoom, BaseRoom blueRoom, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				BattleGame battleGame = new BattleGame(m_gameId++, red, redRoom, blue, blueRoom, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(battleGame);
				}
				battleGame.Prepare();
				Console.WriteLine("StartChallengePVPGame create {0} with {1} player, createID {2}", battleGame.RoomType, battleGame.PlayerCount, battleGame.Id);
				return battleGame;
			}
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create game error:", exception);
			return null;
		}
	}

	public static BaseGame StartPVPGame(int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				PVPGame pVPGame = new PVPGame(m_gameId++, roomId, red, blue, null, null, null, null, null, null, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(pVPGame);
				}
				pVPGame.Prepare();
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("StartPVP create {0} with {1} player, createID {2}", pVPGame.RoomType, pVPGame.PlayerCount, pVPGame.Id);
				Console.ResetColor();
				return pVPGame;
			}
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create game error:", exception);
			return null;
		}
	}

	public static BaseGame StartCampPVEGame(int roomId, List<IGamePlayer> players, int copyId, int timeType, int bossId)
	{
		try
		{
			PveInfo pveInfoById = PveInfoMgr.GetPveInfoById(copyId);
			if (pveInfoById != null)
			{
				CampBattlePVEGame campBattlePVEGame = new CampBattlePVEGame(m_gameId++, roomId, pveInfoById, null, eRoomType.CampBattleBattle, eGameType.CampBattleModelPve, timeType, eHardLevel.Easy, 0);
				campBattlePVEGame.BossCampId = bossId;
				lock (m_games)
				{
					m_games.Add(campBattlePVEGame);
				}
				campBattlePVEGame.Prepare();
				foreach (IGamePlayer player in players)
				{
					if (campBattlePVEGame.AddPlayer(player) == null)
					{
						log.Error("////ADD PLAYER CREATE CAMPBATTLE ERROR: " + player.PlayerCharacter.NickName);
					}
				}
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("StartCampPVE type {0} with {1} player, <{2}>game count:: {5}, <{3}>{4}", campBattlePVEGame.RoomType, campBattlePVEGame.PlayerCount, campBattlePVEGame.Id, copyId, pveInfoById.Name, m_games.Count);
				Console.ResetColor();
				return campBattlePVEGame;
			}
			Console.WriteLine("copyId {0} is null", copyId);
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create game error:", exception);
			return null;
		}
	}

	public static BaseGame StartPVEGame(int roomId, List<IGamePlayer> players, int copyId, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int levelLimits, int currentFloor)
	{
		try
		{
			PveInfo pveInfo = null;
			pveInfo = ((copyId != 0 && copyId != 100000) ? PveInfoMgr.GetPveInfoById(copyId) : PveInfoMgr.GetPveInfoByType(roomType, levelLimits));
			if (pveInfo != null)
			{
				PVEGame pVEGame = new PVEGame(m_gameId++, roomId, pveInfo, players, null, roomType, gameType, timeType, hardLevel, currentFloor);
				lock (m_games)
				{
					m_games.Add(pVEGame);
				}
				pVEGame.Prepare();
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("StartPVE type {0} with {1} player, <{2}>game count:: {5}, <{3}>{4}", pVEGame.RoomType, pVEGame.PlayerCount, pVEGame.Id, copyId, pveInfo.Name, m_games.Count);
				Console.ResetColor();
				return pVEGame;
			}
			Console.WriteLine("copyId {0} is null", copyId);
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create game error:", exception);
			return null;
		}
	}
}

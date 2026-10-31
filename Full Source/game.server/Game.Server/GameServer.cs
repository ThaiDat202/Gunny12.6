using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Academy;
using Game.Server.Battle;
using Game.Server.BombKing;
using Game.Server.CardNewSystem;
using Game.Server.ConsortiaTask;
using Game.Server.DevilTurn;
using Game.Server.DragonBoat;
using Game.Server.Elf;
using Game.Server.Emblem;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.GypsyShop;
using Game.Server.HomeTemple;
using Game.Server.HotSpringRooms;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.RingStation;
using Game.Server.Rooms;
using Game.Server.Statics;
using Game.Server.Team;
using log4net;
using log4net.Config;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server;

public class GameServer : BaseServer
{
	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly string Edition = "5498628";

	public static bool KeepRunning = false;

	private static GameServer _instance;

	private bool _isRunning;

	private bool _isGui;

	private GameServerConfig m_config;

	private LoginServerConnector _loginServer;

	private const int BUF_SIZE = 16384;

	private Queue _packetBufPool;

	private bool m_debugMenory = false;

	private static int m_tryCount = 4;

	private static bool _compiled;

	private HubConnection _hubConnection;

	private Timer _shutdownTimer;

	private int _shutdownCount;

	protected Timer m_saveDbTimer;

	protected Timer m_pingCheckTimer;

	protected Timer m_saveRecordTimer;

	protected Timer m_buffScanTimer;

	protected Timer m_qqTipScanTimer;

	protected Timer m_bagMailScanTimer;

	protected Timer m_validateItemTimer;

	protected Timer m_trailEliteTimer;

	protected Timer m_reduceMemoryTimer;

	protected Timer m_avisoAnuncio;

	public static GameServer Instance => _instance;

	public bool IsRunning => _isRunning;

	public bool IsGUI
	{
		get
		{
			return _isGui;
		}
		set
		{
			_isGui = value;
		}
	}

	public GameServerConfig Configuration => m_config;

	public LoginServerConnector LoginServer => _loginServer;

	public int PacketPoolSize => _packetBufPool.Count;

	public int ShutdownDelay => _shutdownCount;

	[DllImport("psapi.dll")]
	private static extern int EmptyWorkingSet(IntPtr hwProc);

	public static void CreateInstance(GameServerConfig config)
	{
		if (_instance == null)
		{
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(GameServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			_instance = new GameServer(config);
		}
	}

	protected GameServer(GameServerConfig config)
	{
		m_config = config;
		if (log.IsDebugEnabled)
		{
			log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
			log.Debug("Gameserver root directory is: " + Configuration.RootDirectory);
			log.Debug("Changing directory to root directory");
		}
		Directory.SetCurrentDirectory(Configuration.RootDirectory);
	}

	private bool AllocatePacketBuffers()
	{
		int num = Configuration.MaxClientCount * 3;
		_packetBufPool = new Queue(num);
		for (int i = 0; i < num; i++)
		{
			_packetBufPool.Enqueue(new byte[16384]);
		}
		if (log.IsDebugEnabled)
		{
			log.DebugFormat("allocated packet buffers: {0}", num);
		}
		return true;
	}

	public byte[] AcquirePacketBuffer()
	{
		lock (_packetBufPool.SyncRoot)
		{
			if (_packetBufPool.Count > 0)
			{
				return (byte[])_packetBufPool.Dequeue();
			}
		}
		log.Warn("packet buffer pool is empty!");
		return new byte[16384];
	}

	public void ReleasePacketBuffer(byte[] buf)
	{
		if (buf == null || GC.GetGeneration(buf) < GC.MaxGeneration)
		{
			return;
		}
		lock (_packetBufPool.SyncRoot)
		{
			_packetBufPool.Enqueue(buf);
		}
	}

	protected override BaseClient GetNewClient()
	{
		return new GameClient(this, AcquirePacketBuffer(), AcquirePacketBuffer());
	}

	public new GameClient[] GetAllClients()
	{
		GameClient[] array = null;
		lock (_clients.SyncRoot)
		{
			array = new GameClient[_clients.Count];
			_clients.Keys.CopyTo(array, 0);
		}
		return array;
	}

	public int GetAllPlayers()
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		return (allPlayers != null) ? allPlayers.Length : 0;
	}

	public bool clearDB()
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			playerBussiness.ClearDatabase();
		}
		return true;
	}

	public override bool Start()
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		if (_isRunning)
		{
			return false;
		}
		try
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Thread.CurrentThread.Priority = ThreadPriority.Normal;
			GameProperties.Refresh();
			if (!InitComponent(clearDB(), "CleanDB"))
			{
				return false;
			}
			if (!InitComponent(RecompileScripts(), "Recompile Scripts"))
			{
				return false;
			}
			if (!InitComponent(StartScriptComponents(), "Script components"))
			{
				return false;
			}
			if (!InitComponent(GameProperties.EDITION == Edition, "Edition: " + Edition))
			{
				return false;
			}
			if (!InitComponent(InitSocket(IPAddress.Parse(Configuration.Ip), Configuration.Port), "InitSocket Port: " + Configuration.Port))
			{
				return false;
			}
			if (!InitComponent(AllocatePacketBuffers(), "AllocatePacketBuffers()"))
			{
				return false;
			}
			if (!InitComponent(LogMgr.Setup(Configuration.GAME_TYPE, Configuration.ServerID, Configuration.AreaId), "LogMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(WorldMgr.Init(), "WorldMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MapMgr.Init(), "MapMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ItemMgr.Init(isUpdate: true), "ItemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ItemBoxMgr.Init(), "ItemBoxMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AccumulActiveLoginMgr.Init(), "AccumulActiveLoginMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(BallMgr.Init(), "BallMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ExerciseMgr.Init(), "ExerciseMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(LevelMgr.Init(), "LevelMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MaxLevelTemplateMgr.Init(), "MaxLevelTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(BallConfigMgr.Init(), "BallConfigMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(FusionMgr.Init(), "FusionMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AwardMgr.Init(), "AwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ActivityQuestMgr.Init(), "ActivityQuestMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ActivityHalloweenItemsMgr.Init(), "ActivityHalloweenItemsMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ChargeSpendRewardTemplateMgr.Init(), "ChargeSpendRewardTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AchievementMgr.Init(), "AchievementMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(LotteryShowTemplateMgr.Init(), "LotteryShowTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(TreeTemplateMgr.Init(), "TreeTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(NewTitleMgr.Init(), "NewTitleMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(HomeFishMgr.Init(), "HomeFishMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(PetMoePropertyMgr.Init(), "PetMoePropertyMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RankTemplateMgr.Init(), "RankTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(DailyLeagueAwardMgr.Init(), "DailyLeagueAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(SearchGoodsMgr.Init(), "SearchGoodsMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(SuitMgr.Init(), "SuitMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(DiceGameAwardMgr.Init(), "DiceGameAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(EveryDayActiveMgr.Init(), "EveryDayActiveMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(FightLabDropItemMgr.Init(), "FightLabDropItemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(HelpGameRewardMgr.Init(), "HelpGameRewardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MagicFusionDataMgr.Init(), "MagicFusionDataMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MagicItemTemplateMgr.Init(), "MagicItemTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AllQuestionsMgr.Init(), "AllQuestionsMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ChargeActiveTemplateMgr.Init(), "ChargeActiveTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MissionInfoMgr.Init(), "MissionInfoMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MissionEnergyMgr.Init(), "MissionEnergyMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(PveInfoMgr.Init(), "PveInfoMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(DropMgr.Init(), "DropMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AvatarColectionMgr.Init(), "AvatarColectionMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(FightRateMgr.Init(), "FightRateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RefineryMgr.Init(), "RefineryMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(StrengthenMgr.Init(), "StrengthenMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(PropItemMgr.Init(), "PropItemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ShopMgr.Init(), "ShopMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(QuestMgr.Init(), "QuestMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ActiveMgr.Init(), "ActiveMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(GypsyShopMgr.Init(), "GypsyShopMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ConsortiaMgr.Init(), "ConsortiaMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ConsortiaExtraMgr.Init(), "ConsortiaExtraMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RateMgr.Init(Configuration), "RateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(WindMgr.Init(), "WindMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(CardMgr.Init(), "CardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(PetMgr.Init(), "PetMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(GoldEquipMgr.Init(), "GoldEquipMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RuneMgr.Init(), "RuneMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(TotemMgr.Init(), "TotemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(TotemHonorMgr.Init(), "TotemHonorMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(TreasureAwardMgr.Init(), "TreasureAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(FairBattleRewardMgr.Init(), "FairBattleRewardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MagicStoneMgr.Init(), "MagicStoneMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(VipStoreMgr.Init(), "VipStoreMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(LoveLevelMgr.Init(), "LoveLevelMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(FightSpiritTemplateMgr.Init(), "FightSpiritTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(SetsBuildTempMgr.Init(), "SetsBuildTempMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(PetBreakMgr.Init(), "PetBreakMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MacroDropMgr.Init(), "MacroDropMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RankMgr.Init(), "RankMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(CommunalActiveMgr.Init(), "CommunalActiveMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(QQTipsMgr.Init(), "QQTipsMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(LightriddleQuestMgr.Init(), "LightriddleQuestMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(WorldEventMgr.Init(), "WorldEventMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(SubActiveMgr.Init(), "SubActiveMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(DiceLevelAwardMgr.Init(), "DiceLevelAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(EventAwardMgr.Init(), "EventAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ActiveSystermAwardMgr.Init(), "ActiveSystermAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MountMgr.Init(), "MountMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(GuardCoreMgr.Init(), "GuardCoreMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(HomeTempPracticeMgr.Init(), "HomeTempPracticeMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(CloudBuyLotteryMgr.Init(), "CloudBuyLotteryMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ActiveSystemMgr.Init(), "ActiveSystemMgr.Init()"))
			{
				return false;
			}
			if (!InitComponent(CardAchievementMgr.Init(), "CardAchievementMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RuneAdvanceTemplateMgr.Init(), "RuneAdvanceTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(LanguageMgr.Setup(""), "LanguageMgr.Setup"))
			{
				return false;
			}
			if (!InitComponent(MarryRoomMgr.Init(), "MarryRoomMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(HotSpringRoomsMgr.Init(), "HotSpringRoomsMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(BattleBonusMgr.Init(), "BattleBonusMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(RoomMgr.Setup(Configuration.MaxRoomCount), "RoomMgr.Setup"))
			{
				return false;
			}
			if (!InitComponent(GameMgr.Setup(Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION), "GameMgr.Setup"))
			{
				return false;
			}
			if (!InitComponent(BattleMgr.Setup(), "BattleMgr.Setup"))
			{
				return false;
			}
			if (!InitComponent(RingStationMgr.Init(), "RingStationMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(BombKingMgr.Init(), "BombKingMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(BeautyVoteMgr.Init(), "BeautyVoteMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(SubWeaponEvolutionTemplateMgr.Init(), "SubWeaponEvolutionTemplateMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AmuletGradeItemMgr.Init(), "AmuletGradeItemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AmuletItemMgr.Init(), "AmuletItemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AmuletPhaseItemMgr.Init(), "AmuletPhaseItemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(MountTalismansMgr.Init(), "MountTalismansMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(QiYuanAwardMgr.Init(), "QiYuanAwardMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(BankMgr.Init(), "BankMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(SpiritInfoMgr.Init(), "SpiritInfoMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(EngraveMgr.Init(), "EngraveMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(JampsManualMgr.Init(), "JampsManualMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(VipIntegrationMgr.Init(), "VipIntegrationMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(CardNewSystemMgr.Init(), "CardNewSystemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(NecklaceStrengthMgr.Init(), "NecklaceStrengthMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(EmblemMgr.Init(), "EmblemMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ElfMgr.Init(), "ElfMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(DevilTurnMgr.Init(), "DevilTurnMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(TeamMgr.Init(), "TeamMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(PetSpecialSkillMgr.Init(), "PetSpecialSkillMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(GmActivityMgr.Init(), "GmActivityMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(ConsortiaTaskMgr.Init(), "ConsortiaTaskMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(AcademyMgr.Init(), "AcademyMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(HomeTempleMgr.Init(), "HomeTempleMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(InitGlobalTimer(), "InitGlobalTimer"))
			{
				return false;
			}
			if (!InitComponent(ComandosMgr.Init(), "ComandosMgr.Init"))
			{
				return false;
			}
			if (!InitComponent(LogMgr.Setup(1, Configuration.ServerID, Configuration.AreaId), "LogMgr.Setup"))
			{
				return false;
			}
			GameEventMgr.Notify(ScriptEvent.Loaded);
			if (!InitComponent(InitLoginServer(), "Init Global Timers"))
			{
				return false;
			}
			RoomMgr.Start();
			GameMgr.Start();
			BattleMgr.Start();
			MacroDropMgr.Start();
			GlobalConstants.Start();
			if (!InitComponent(base.Start(), "base.Start()"))
			{
				return false;
			}
			GameEventMgr.Notify(GameServerEvent.Started, this);
			GC.Collect(GC.MaxGeneration);
			MinimizeFootprint();
			if (log.IsInfoEnabled)
			{
				log.Info("GameServer is now open for connections!");
			}
			_isRunning = true;
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Failed to start the server", exception);
			}
			return false;
		}
	}

	private bool InitLoginServer()
	{
		_loginServer = new LoginServerConnector(m_config.LoginServerIp, m_config.LoginServerPort, m_config.ServerID, m_config.AreaId, m_config.ServerName, AcquirePacketBuffer(), AcquirePacketBuffer());
		_loginServer.Disconnected += loginServer_Disconnected;
		return _loginServer.Connect();
	}

	private void loginServer_Disconnected(BaseClient client)
	{
		bool isRunning = _isRunning;
		Stop();
		if (isRunning && m_tryCount > 0)
		{
			m_tryCount--;
			log.Error("Center Server Disconnect! Stopping Server");
			log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", m_tryCount);
			Thread.Sleep(1000);
			if (Start())
			{
				log.Error("Restart the game server success!");
			}
		}
		else
		{
			if (m_tryCount == 0)
			{
				log.ErrorFormat("Restart the game server failed after {0} times.", 4);
				log.Error("Server Stopped!");
			}
			LogManager.Shutdown();
		}
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			log.Error("Unhandled exception!\n" + e.ExceptionObject.ToString());
			if (e.IsTerminating)
			{
				log.Error("Stop by CurrentDomain_UnhandledException");
				Stop();
			}
		}
		catch
		{
			try
			{
				using FileStream stream = new FileStream("c:\\testme.log", FileMode.Append, FileAccess.Write);
				using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
				streamWriter.WriteLine(e.ExceptionObject);
			}
			catch
			{
			}
		}
	}

	public bool RecompileScripts()
	{
		if (!_compiled)
		{
			string path = Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] asm_names = Configuration.ScriptAssemblies.Split(',');
			_compiled = ScriptMgr.CompileScripts(compileVB: false, path, Configuration.ScriptCompilationTarget, asm_names);
		}
		return _compiled;
	}

	protected bool StartScriptComponents()
	{
		try
		{
			if (log.IsInfoEnabled)
			{
				log.Info("Server rules: true");
			}
			ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
			ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
			ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
			ArrayList arrayList = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly item in arrayList)
			{
				GameEventMgr.RegisterGlobalEvents(item, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
				GameEventMgr.RegisterGlobalEvents(item, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
				GameEventMgr.RegisterGlobalEvents(item, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
				GameEventMgr.RegisterGlobalEvents(item, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
			}
			if (log.IsInfoEnabled)
			{
				log.Info("Registering global event handlers: true");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("StartScriptComponents", exception);
			}
			return false;
		}
		return true;
	}

	public void EnviarAltoFalante(string msg, int userid, string nickname)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(72);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(userid);
		gSPacketIn.WriteString(nickname);
		gSPacketIn.WriteString(msg);
		Instance.LoginServer.SendPacket(gSPacketIn);
	}

	private void ReduceMemory(object sender)
	{
		if (log.IsInfoEnabled)
		{
			log.Info("Cleaning Memory");
		}
		int tickCount = Environment.TickCount;
		MinimizeFootprint();
		if (log.IsInfoEnabled)
		{
			log.Info("Memory Cleaned in " + (Environment.TickCount - tickCount) + "ms");
		}
	}

	public static void MinimizeFootprint()
	{
		GC.Collect(GC.MaxGeneration);
		EmptyWorkingSet(Process.GetCurrentProcess().Handle);
	}

	protected bool InitComponent(bool componentInitState, string text)
	{
		if (m_debugMenory)
		{
			log.Debug("Start Memory " + text + ": " + GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024);
		}
		if (log.IsInfoEnabled && !_isGui)
		{
			log.Info(text + ": " + componentInitState);
		}
		else
		{
			Console.WriteLine(text + ": " + componentInitState);
		}
		if (!componentInitState)
		{
			log.Error("Stop by " + text + " (InitComponent)");
			Stop();
		}
		if (m_debugMenory)
		{
			log.Debug("Finish Memory " + text + ": " + GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024);
		}
		return componentInitState;
	}

	public override void Stop()
	{
		if (_isRunning)
		{
			_isRunning = false;
			if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
			{
				Console.WriteLine("Update BreakTime failed");
			}
			RoomMgr.Stop();
			GameMgr.Stop();
			ActiveSystemMgr.StopAllTimer();
			RingStationMgr.StopAllTimer();
			DragonBoatMgr.StopAllTimer();
			CloudBuyLotteryMgr.StopAllTimer();
			GypsyShopMgr.StopAllTimer();
			HotSpringRoomsMgr.StopAllTimer();
			MinesMgr.StopTimer();
			StopValidateItemsScan();
			GlobalConstants.StopGlobalTimer();
			if (_loginServer != null)
			{
				_loginServer.Disconnected -= loginServer_Disconnected;
				_loginServer.Disconnect();
			}
			BattleMgr.Disconnet(1);
			RingStationMgr.RingStationBattle.Stop();
			CloudBuyLotteryMgr.SaveCloudBuyLottery();
			BeautyVoteMgr.StopTimer();
			ConsortiaMgr.Save();
			if (m_pingCheckTimer != null)
			{
				m_pingCheckTimer.Change(-1, -1);
				m_pingCheckTimer.Dispose();
				m_pingCheckTimer = null;
			}
			if (m_saveDbTimer != null)
			{
				m_saveDbTimer.Change(-1, -1);
				m_saveDbTimer.Dispose();
				m_saveDbTimer = null;
			}
			if (m_saveRecordTimer != null)
			{
				m_saveRecordTimer.Change(-1, -1);
				m_saveRecordTimer.Dispose();
				m_saveRecordTimer = null;
			}
			if (m_buffScanTimer != null)
			{
				m_buffScanTimer.Change(-1, -1);
				m_buffScanTimer.Dispose();
				m_buffScanTimer = null;
			}
			if (m_qqTipScanTimer != null)
			{
				m_qqTipScanTimer.Change(-1, -1);
				m_qqTipScanTimer.Dispose();
				m_qqTipScanTimer = null;
			}
			if (m_bagMailScanTimer != null)
			{
				m_bagMailScanTimer.Change(-1, -1);
				m_bagMailScanTimer.Dispose();
				m_bagMailScanTimer = null;
			}
			if (m_trailEliteTimer != null)
			{
				m_trailEliteTimer.Change(-1, -1);
				m_trailEliteTimer.Dispose();
				m_trailEliteTimer = null;
			}
			if (m_reduceMemoryTimer != null)
			{
				m_reduceMemoryTimer.Change(-1, -1);
				m_reduceMemoryTimer.Dispose();
				m_reduceMemoryTimer = null;
			}
			base.Stop();
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			log.Info("Server Stopped!");
			LogManager.Shutdown();
			Console.WriteLine("Server Stopped!");
			ClearInstance();
		}
	}

	public void ClearInstance()
	{
		_instance = null;
	}

	public void ConnectToHub()
	{
		string urlHub = ConfigurationManager.AppSettings["RemoteHost"];
		_hubConnection = new HubConnection(urlHub);
		_hubConnection.CreateHubProxy("RemoteHub");
		_hubConnection.Start().ContinueWith(delegate(Task task)
		{
			if (task.IsFaulted)
			{
				if (task.Exception != null)
				{
					Console.WriteLine("Can not connect to: {0}, error: {1}", urlHub, task.Exception.Message);
				}
			}
			else
			{
				Console.WriteLine("Success! Connected with client connection id {0}", _hubConnection.ConnectionId);
			}
		});
		_hubConnection.Closed += delegate
		{
			Console.WriteLine("Remote connection closed!");
		};
		_hubConnection.Received += delegate(string data)
		{
			HubInfo hubInfo = JsonConvert.DeserializeObject<HubInfo>(data);
			if (hubInfo != null && hubInfo.A != null && hubInfo.A.Length == 2)
			{
				Console.WriteLine("Remote Hub call server id {0}<{1}>", hubInfo.A[0], hubInfo.A[1]);
				if (hubInfo.A[0] == Configuration.ServerID.ToString())
				{
					string text = hubInfo.A[1];
					string text2 = text;
					if (!(text2 == "start"))
					{
						if (text2 == "shutdown")
						{
							StopGameServer();
						}
					}
					else
					{
						StartGameServer();
					}
				}
			}
		};
	}

	private void StopGameServer()
	{
		if (Instance != null)
		{
			if (!Instance.IsRunning)
			{
				Console.WriteLine("Server start do not complete...");
				SendToRemoteHub("GameServer <" + Configuration.ServerID + "::" + Configuration.ServerName + "> start do not complete!");
			}
			else if (ShutdownDelay > 0)
			{
				Console.WriteLine("Server start do not complete...");
				SendToRemoteHub("GameServer <" + Configuration.ServerID + "::" + Configuration.ServerName + "> still running, server will shutdown after " + ShutdownDelay + " minute.");
			}
			else
			{
				int delay = int.Parse(ConfigurationManager.AppSettings["ShutdownInterval"]);
				Shutdown(delay);
			}
		}
	}

	private void StartGameServer()
	{
		if (Instance == null)
		{
			CreateInstance(new GameServerConfig());
			Instance?.Start();
			return;
		}
		Console.WriteLine("GameServer still running please wait a moment!");
		SendToRemoteHub("GameServer <" + Configuration.ServerID + "::" + Configuration.ServerName + "> still running please wait a moment!");
	}

	private void SendToRemoteHub(string massage)
	{
		if (_hubConnection != null)
		{
			string connectionId = _hubConnection.ConnectionId;
			string text = ConfigurationManager.AppSettings["LoginKey"];
			IHubProxy hubProxy = _hubConnection.CreateHubProxy("RemoteHub");
			hubProxy.Invoke("SendRemote", true, connectionId, massage);
		}
	}

	public void ReloadGameDatas(eReloadType type)
	{
		_loginServer?.Reload(type, isSchema: false);
	}

	public void Shutdown(int delay)
	{
		_shutdownCount = delay;
		Instance.LoginServer.SendShutdown(isStoping: true);
		_shutdownTimer = new Timer(ShutDownCallBack, null, 0, 60000);
	}

	private void ShutDownCallBack(object state)
	{
		try
		{
			_shutdownCount--;
			Console.WriteLine("Server will shutdown after {0} mins!", _shutdownCount);
			GameClient[] allClients = Instance.GetAllClients();
			GameClient[] array = allClients;
			foreach (GameClient gameClient in array)
			{
				if (gameClient.Out != null)
				{
					gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
				}
			}
			if (_shutdownCount <= 0)
			{
				Instance.LoginServer.SendShutdown(isStoping: false);
				_shutdownTimer.Dispose();
				_shutdownTimer = null;
				Instance.Stop();
				log.Error("Stop by ShutDownCallBack");
				Console.WriteLine("Server has stopped!");
			}
		}
		catch (Exception message)
		{
			log.Error(message);
		}
	}

	protected void ValidateTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Validate Item Scaning ...");
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			GameClient[] allClients = GetAllClients();
			if (allClients != null)
			{
				GameClient[] array = allClients;
				foreach (GameClient gameClient in array)
				{
					if (gameClient != null && gameClient.IsConnected && gameClient.Player != null && ((gameClient.Player.CurrentRoom != null && !gameClient.Player.CurrentRoom.IsPlaying) || gameClient.Player.CurrentRoom == null))
					{
						gameClient.Player.ValidateItems();
					}
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Validate Item Scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ValidateTimerProc", exception);
			}
		}
	}

	public void StopValidateItemsScan()
	{
		if (m_validateItemTimer != null)
		{
			m_validateItemTimer.Dispose();
			m_validateItemTimer = null;
		}
	}

	public bool InitGlobalTimer()
	{
		int num = Configuration.DBSaveInterval * 60 * 1000;
		if (m_saveDbTimer == null)
		{
			m_saveDbTimer = new Timer(SaveTimerProc, null, num, num);
		}
		else
		{
			m_saveDbTimer.Change(num, num);
		}
		num = Configuration.PingCheckInterval * 60 * 1000;
		if (m_pingCheckTimer == null)
		{
			m_pingCheckTimer = new Timer(PingCheck, null, num, num);
		}
		else
		{
			m_pingCheckTimer.Change(num, num);
		}
		num = Configuration.SaveRecordInterval * 60 * 1000;
		if (m_saveRecordTimer == null)
		{
			m_saveRecordTimer = new Timer(SaveRecordProc, null, num, num);
		}
		else
		{
			m_saveRecordTimer.Change(num, num);
		}
		num = 60000;
		if (m_buffScanTimer == null)
		{
			m_buffScanTimer = new Timer(BuffScanTimerProc, null, num, num);
		}
		else
		{
			m_buffScanTimer.Change(num, num);
		}
		num = 300000;
		if (m_qqTipScanTimer == null)
		{
			m_qqTipScanTimer = new Timer(QQTipScanTimerProc, null, num, num);
		}
		else
		{
			m_qqTipScanTimer.Change(num, num);
		}
		num = 900000;
		if (m_bagMailScanTimer == null)
		{
			m_bagMailScanTimer = new Timer(BagMailScanTimerProc, null, num, num);
		}
		else
		{
			m_bagMailScanTimer.Change(num, num);
		}
		num = 1800000;
		if (m_validateItemTimer == null)
		{
			m_validateItemTimer = new Timer(ValidateTimerProc, null, num, num);
		}
		else
		{
			m_validateItemTimer.Change(num, num);
		}
		num = 60000;
		if (m_trailEliteTimer == null)
		{
			m_trailEliteTimer = new Timer(TrailEliteTimerProc, null, num, num);
		}
		else
		{
			m_trailEliteTimer.Change(num, num);
		}
		num = 600000;
		if (m_reduceMemoryTimer == null)
		{
			m_reduceMemoryTimer = new Timer(ReduceMemory, null, num, num);
		}
		else
		{
			m_reduceMemoryTimer.Change(num, num);
		}
		return true;
	}

	protected void PingCheck(object sender)
	{
		try
		{
			log.Info("Begin ping check....");
			long num = (long)Configuration.PingCheckInterval * 60L * 1000 * 1000 * 10;
			GameClient[] allClients = GetAllClients();
			if (allClients != null)
			{
				GameClient[] array = allClients;
				foreach (GameClient gameClient in array)
				{
					if (gameClient == null)
					{
						continue;
					}
					if (gameClient.IsConnected)
					{
						if (gameClient.Player != null)
						{
							gameClient.Out.SendPingTime(gameClient.Player);
							if (AntiAddictionMgr.ISASSon && AntiAddictionMgr.count == 0)
							{
								AntiAddictionMgr.count++;
							}
						}
						else if (gameClient.PingTime + num < DateTime.Now.Ticks)
						{
							gameClient.Disconnect();
						}
					}
					else
					{
						gameClient.Disconnect();
					}
				}
			}
			log.Info("End ping check....");
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PingCheck callback", exception);
			}
		}
		try
		{
			log.Info("Begin ping center check....");
			Instance.LoginServer.SendPingCenter();
			log.Info("End ping center check....");
		}
		catch (Exception exception2)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PingCheck center callback", exception2);
			}
		}
	}

	protected void SaveTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving database...");
				log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			int num = 0;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.SaveIntoDatabase();
				num++;
			}
			AcademyMgr.RemoveOldRequest();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				if (IsGUI)
				{
					Console.WriteLine("Saved all databases and " + num + " players in " + tickCount + "ms");
				}
				else
				{
					log.Info("Saving database complete!");
					log.Info("Saved all databases and " + num + " players in " + tickCount + "ms");
				}
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Saved all databases and {0} players in {1} ms", num, tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SaveTimerProc", exception);
			}
		}
		finally
		{
			GameEventMgr.Notify(GameServerEvent.WorldSave);
		}
	}

	protected void SaveRecordProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving Record...");
				log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			LogMgr.Save();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving Record complete!");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Saved all Record  in {0} ms", tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SaveRecordProc", exception);
			}
		}
	}

	protected void BuffScanTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Buff Scaning ...");
				log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			int num = 0;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				if (gamePlayer.BufferList != null)
				{
					gamePlayer.BufferList.Update();
					num++;
				}
				gamePlayer.Reset(saveToDb: false);
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				if (IsGUI)
				{
					Console.WriteLine("Buff all " + num + " players in " + tickCount + "ms");
				}
				else
				{
					log.Info("Buff Scan complete!");
					log.Info("Buff all " + num + " players in " + tickCount + "ms");
				}
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Scan all Buff and {0} players in {1} ms", num, tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("BuffScanTimerProc", exception);
			}
		}
		finally
		{
			if (log.IsInfoEnabled)
			{
				log.Info("GameMgr Scaning ...");
			}
			if (GameMgr.SynDate < 0)
			{
				GameMgr.ClearAllGames();
				GameMgr.Stop();
				GameMgr.Setup(Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION);
				GameMgr.Start();
				if (log.IsInfoEnabled)
				{
					log.Warn("Game PVE Restart Success!");
				}
			}
			else if (log.IsInfoEnabled)
			{
				log.InfoFormat("GameMgr.SynDate: {0}", GameMgr.SynDate);
			}
			if (log.IsInfoEnabled)
			{
				log.Info("GameMgr Scan complete!");
			}
		}
	}

	protected void QQTipScanTimerProc(object sender)
	{
		try
		{
			if (log.IsInfoEnabled)
			{
				log.Info("QQTips Scaning ...");
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			QQtipsMessagesInfo qQtipsMessages = QQTipsMgr.GetQQtipsMessages(Instance.Configuration.AreaId);
			GamePlayer[] allPlayersNoRoom = WorldMgr.GetAllPlayersNoRoom();
			GamePlayer[] array = allPlayersNoRoom;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.Out.SendQQtips(qQtipsMessages);
			}
			Thread.CurrentThread.Priority = priority;
			if (log.IsInfoEnabled)
			{
				log.Info("QQTips Scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("QQTipScanTimerProc", exception);
			}
		}
	}

	protected void BagMailScanTimerProc(object sender)
	{
		try
		{
			if (log.IsInfoEnabled)
			{
				log.Info("BagMail Scaning ...");
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			Thread.CurrentThread.Priority = priority;
			if (log.IsInfoEnabled)
			{
				log.Info("BagMail Scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("BagMailScanTimerProc", exception);
			}
		}
	}

	protected void TrailEliteTimerProc(object sender)
	{
		try
		{
			if (log.IsInfoEnabled)
			{
				log.Info("TrailElite Scaning ...");
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			WorldMgr.UpdateTrailEliteTimer();
			Thread.CurrentThread.Priority = priority;
			if (log.IsInfoEnabled)
			{
				log.Info("TrailElite Scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TrailEliteTimerProc", exception);
			}
		}
	}
}

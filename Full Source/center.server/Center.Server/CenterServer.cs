using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Center.Server.Managers;
using Center.Server.Statics;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Server.Managers;
using log4net;
using log4net.Config;
using SqlDataProvider.Data;

namespace Center.Server
{
	public class CenterServer : BaseServer
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private CenterServerConfig _config;

		private string Edition = "5498628";

		private Random rand = new Random();

		private bool _aSSState;

		private bool _dailyAwardState;

		private Timer m_loginLapseTimer;

		private Timer m_saveDBTimer;

		private Timer m_saveRecordTimer;

		private Timer m_scanAuction;

		private Timer m_scanMail;

		private Timer m_scanConsortia;

		private Timer m_worldEvent;

		private Timer m_consortiaboss;

		private Timer m_sytemNotice;

		private Timer m_cityBattleTimer;

		private Timer m_reduceMemoryTimer;

		private static CenterServer _instance;

		public bool ASSState
		{
			get
			{
				return _aSSState;
			}
			set
			{
				_aSSState = value;
			}
		}

		public bool DailyAwardState
		{
			get
			{
				return _dailyAwardState;
			}
			set
			{
				_dailyAwardState = value;
			}
		}

		public static CenterServer Instance => _instance;

		[DllImport("psapi.dll")]
		private static extern int EmptyWorkingSet(IntPtr hwProc);

		protected override BaseClient GetNewClient()
		{
			return new ServerClient(this);
		}

		public override bool Start()
		{
			try
			{
				Thread.CurrentThread.Priority = ThreadPriority.Normal;
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				GameProperties.Refresh();
				if (!InitComponent(StartScriptComponents(), "Script components") || !InitComponent(GameProperties.EDITION == Edition, "Check Server Edition:" + Edition) || !InitComponent(InitSocket(IPAddress.Parse(_config.Ip), _config.Port), "InitSocket Port:" + _config.Port) || !InitComponent(CenterService.Start(), "Center Service") || !InitComponent(ServerMgr.Start(), "Load serverlist") || !InitComponent(ServerDataMgr.Init(), "Load ServerData") || !InitComponent(ConsortiaExtraMgr.Init(), "ConsortiaLevelMgr Init ") || !InitComponent(MacroDropMgr.Init(), "MacroDropMgr Init ") || !InitComponent(WorldEventMgr.Init(), "WorldEventMgr Init") || !InitComponent(NewTitleMgr.Init(), "NewTitleMgr Init") || !InitComponent(BombKingMgr.Init(), "BombKingMgr Init") || !InitComponent(QiYuanAwardMgr.Init(), "QiYuanAwardMgr Init") || !InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init") || !InitComponent(SystemNoticeMgr.Start(), "SystemNoticeMgr Init") || !InitComponent(WorldMgr.Setup(), "WorldMgr Init") || !InitComponent(InitGlobalTimers(), "Init Global Timers"))
				{
					return false;
				}
				GameEventMgr.Notify(ScriptEvent.Loaded);
				MacroDropMgr.Start();
				if (!InitComponent(base.Start(), "base.Start()"))
				{
					return false;
				}
				GameEventMgr.Notify(GameServerEvent.Started, this);
				ReduceMemory(null);
				log.Info("GameServer is now open for connections!");
				GameProperties.Save();
				return true;
			}
			catch (Exception exception)
			{
				log.Error("Failed to start the server", exception);
				return false;
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
			if (e.IsTerminating)
			{
				LogManager.Shutdown();
			}
		}

		protected bool InitComponent(bool componentInitState, string text)
		{
			log.Info(text + ": " + componentInitState);
			if (!componentInitState)
			{
				Stop();
			}
			return componentInitState;
		}

		public bool RecompileScripts()
		{
			string path = _config.RootDirectory + Path.DirectorySeparatorChar + "scripts";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] asm_names = _config.ScriptAssemblies.Split(',');
			return ScriptMgr.CompileScripts(compileVB: false, path, _config.ScriptCompilationTarget, asm_names);
		}

		protected bool StartScriptComponents()
		{
			try
			{
				ScriptMgr.InsertAssembly(typeof(CenterServer).Assembly);
				ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
				Assembly[] scripts = ScriptMgr.Scripts;
				foreach (Assembly asm in scripts)
				{
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
					GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
				}
				log.Info("Registering global event handlers: true");
				return true;
			}
			catch (Exception exception)
			{
				log.Error("StartScriptComponents", exception);
				return false;
			}
		}

		public bool InitGlobalTimers()
		{
			int num = _config.SaveIntervalInterval * 60 * 1000;
			if (m_saveDBTimer == null)
			{
				m_saveDBTimer = new Timer(SaveTimerProc, null, num, num);
			}
			else
			{
				m_saveDBTimer.Change(num, num);
			}
			int num2 = _config.LoginLapseInterval * 60 * 1000;
			if (m_loginLapseTimer == null)
			{
				m_loginLapseTimer = new Timer(LoginLapseTimerProc, null, num2, num2);
			}
			else
			{
				m_loginLapseTimer.Change(num2, num2);
			}
			int num3 = _config.SaveRecordInterval * 60 * 1000;
			if (m_saveRecordTimer == null)
			{
				m_saveRecordTimer = new Timer(SaveRecordProc, null, num3, num3);
			}
			else
			{
				m_saveRecordTimer.Change(num3, num3);
			}
			int num4 = _config.ScanAuctionInterval * 60 * 1000;
			if (m_scanAuction == null)
			{
				m_scanAuction = new Timer(ScanAuctionProc, null, num4, num4);
			}
			else
			{
				m_scanAuction.Change(num4, num4);
			}
			int num5 = _config.ScanMailInterval * 60 * 1000;
			if (m_scanMail == null)
			{
				m_scanMail = new Timer(ScanMailProc, null, num5, num5);
			}
			else
			{
				m_scanMail.Change(num5, num5);
			}
			int num6 = _config.ScanConsortiaInterval * 60 * 1000;
			if (m_scanConsortia == null)
			{
				m_scanConsortia = new Timer(ScanConsortiaProc, null, num6, num6);
			}
			else
			{
				m_scanConsortia.Change(num6, num6);
			}
			int num7 = 60000;
			if (m_worldEvent == null)
			{
				m_worldEvent = new Timer(ScanWorldEventProc, null, num7, num7);
			}
			else
			{
				m_worldEvent.Change(num7, num7);
			}
			if (m_consortiaboss == null)
			{
				m_consortiaboss = new Timer(ScanConsortiabossProc, null, num7, num7);
			}
			else
			{
				m_consortiaboss.Change(num7, num7);
			}
			int num8 = 300000;
			if (m_sytemNotice == null)
			{
				m_sytemNotice = new Timer(SystemNoticeTimerProc, null, num8, num8);
			}
			else
			{
				m_sytemNotice.Change(num8, num8);
			}
			int num9 = 60000;
			if (m_cityBattleTimer == null)
			{
				m_cityBattleTimer = new Timer(CityBattleTimerProc, null, num9, num9);
			}
			else
			{
				m_cityBattleTimer.Change(num9, num9);
			}
			int num10 = 600000;
			if (m_reduceMemoryTimer == null)
			{
				m_reduceMemoryTimer = new Timer(ReduceMemory, null, num10, num10);
			}
			else
			{
				m_reduceMemoryTimer.Change(num10, num10);
			}
			return true;
		}

		public void DisposeGlobalTimers()
		{
			if (m_saveDBTimer != null)
			{
				m_saveDBTimer.Dispose();
			}
			if (m_loginLapseTimer != null)
			{
				m_loginLapseTimer.Dispose();
			}
			if (m_saveRecordTimer != null)
			{
				m_saveRecordTimer.Dispose();
			}
			if (m_scanAuction != null)
			{
				m_scanAuction.Dispose();
			}
			if (m_scanMail != null)
			{
				m_scanMail.Dispose();
			}
			if (m_scanConsortia != null)
			{
				m_scanConsortia.Dispose();
			}
			if (m_worldEvent != null)
			{
				m_worldEvent.Dispose();
			}
			if (m_consortiaboss != null)
			{
				m_consortiaboss.Dispose();
			}
			if (m_sytemNotice != null)
			{
				m_sytemNotice.Dispose();
			}
			if (m_cityBattleTimer != null)
			{
				m_cityBattleTimer.Dispose();
			}
			if (m_reduceMemoryTimer != null)
			{
				m_reduceMemoryTimer.Dispose();
			}
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

		protected void CityBattleTimerProc(object state)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("CityBattle ...");
					log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				if (WorldMgr.CityBattle != null)
				{
					WorldMgr.CityBattle.UpdateCityBattleTimer();
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("CityBattle complete!");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("CityBattleTimerProc", exception);
				}
			}
		}

		protected void SystemNoticeTimerProc(object state)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("System Notice ...");
					log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				List<string> notceList = SystemNoticeMgr.NotceList;
				if (notceList.Count > 0)
				{
					int index = rand.Next(notceList.Count);
					Instance.SendSystemNotice(notceList[index], 4);
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("System Notice complete!");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SystemNoticeTimerProc", exception);
				}
			}
		}

		protected void SaveTimerProc(object state)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving database...");
					log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				ServerMgr.SaveToDatabase();
				WorldMgr.CityBattle.Save();
				ServerDataMgr.SaveServerData();
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving database complete!");
					log.Info("Saved all databases " + num + "ms");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("SaveTimerProc", exception);
				}
			}
		}

		protected void LoginLapseTimerProc(object sender)
		{
			try
			{
				long ticks = DateTime.Now.Ticks;
				long num = (long)_config.LoginLapseInterval * 10L * 1000;
				ZoneLoginMgr[] allZoneLogin = ServerMgr.GetAllZoneLogin();
				foreach (ZoneLoginMgr zoneLoginMgr in allZoneLogin)
				{
					Player[] allPlayer = zoneLoginMgr.GetAllPlayer();
					foreach (Player player in allPlayer)
					{
						if (player.State == ePlayerState.NotLogin)
						{
							if (player.LastTime + num < ticks)
							{
								zoneLoginMgr.RemovePlayer(player.Id);
							}
						}
						else
						{
							player.LastTime = ticks;
						}
					}
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("LoginLapseTimer callback", exception);
				}
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
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Saving Record complete!");
				}
				if (num > 120000)
				{
					log.WarnFormat("Saved all Record  in {0} ms!", num);
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

		protected void ScanAuctionProc(object sender)
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
				string noticeUserID = "";
				AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
				foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo))
					{
						playerBussiness.ScanAuction(ref noticeUserID);
					}
					string text = noticeUserID;
					char[] separator = new char[1] { ',' };
					string[] array = text.Split(separator);
					foreach (string text2 in array)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							GSPacketIn gSPacketIn = new GSPacketIn(117);
							gSPacketIn.WriteInt(int.Parse(text2));
							gSPacketIn.WriteInt(1);
							SendToALL(gSPacketIn, areaConfigInfo.AreaID);
						}
					}
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan Auction complete!");
				}
				if (num > 120000)
				{
					log.WarnFormat("Scan all Auction  in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ScanAuctionProc", exception);
				}
			}
		}

		protected void ScanMailProc(object sender)
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
				string noticeUserID = "";
				AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
				foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo))
					{
						playerBussiness.ScanMail(ref noticeUserID);
					}
					string text = noticeUserID;
					char[] separator = new char[1] { ',' };
					string[] array = text.Split(separator);
					foreach (string text2 in array)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							GSPacketIn gSPacketIn = new GSPacketIn(117);
							gSPacketIn.WriteInt(int.Parse(text2));
							gSPacketIn.WriteInt(1);
							SendToALL(gSPacketIn, areaConfigInfo.AreaID);
						}
					}
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan Mail complete!");
				}
				if (num > 120000)
				{
					log.WarnFormat("Scan all Mail in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ScanMailProc", exception);
				}
			}
		}

		protected void ScanConsortiabossProc(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan Consortiaboss...");
					log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				ConsortiaBossMgr.UpdateTime();
				ConsortiaBossMgr.TimeCheckingAward++;
				if (ConsortiaBossMgr.TimeCheckingAward > 5)
				{
					List<int> allConsortiaGetAward = ConsortiaBossMgr.GetAllConsortiaGetAward();
					GSPacketIn gSPacketIn = new GSPacketIn(185);
					gSPacketIn.WriteInt(allConsortiaGetAward.Count);
					foreach (int item in allConsortiaGetAward)
					{
						gSPacketIn.WriteInt(item);
					}
					SendToALL(gSPacketIn);
					ConsortiaBossMgr.TimeCheckingAward = 0;
					log.Info("Scan Consortiaboss award complete!");
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan Consortiaboss complete!");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ScanConsortiabossProc", exception);
				}
			}
		}

		protected void ScanWorldEventProc(object sender)
		{
			try
			{
				int tickCount = Environment.TickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan  WorldEvent ...");
					log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
				}
				ThreadPriority priority = Thread.CurrentThread.Priority;
				Thread.CurrentThread.Priority = ThreadPriority.Lowest;
				ServerClient[] allClients = GetAllClients();
				if (allClients != null)
				{
					ServerClient[] array = allClients;
					foreach (ServerClient serverClient in array)
					{
						serverClient.SendUpdateWorldEvent();
					}
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan WorldEvent complete!");
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Scan WorldEvent Proc", exception);
				}
			}
		}

		protected void ScanConsortiaProc(object sender)
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
				AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
				foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
				{
					string noticeID = "";
					using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness(areaConfigInfo))
					{
						consortiaBussiness.ScanConsortia(ref noticeID);
						consortiaBussiness.RenamesBatch(areaConfigInfo.Catalog);
					}
					string text = noticeID;
					char[] separator = new char[1] { ',' };
					string[] array = text.Split(separator);
					foreach (string text2 in array)
					{
						if (!string.IsNullOrEmpty(text2))
						{
							GSPacketIn gSPacketIn = new GSPacketIn(128);
							gSPacketIn.WriteByte(2);
							gSPacketIn.WriteInt(int.Parse(text2));
							SendToALL(gSPacketIn, areaConfigInfo.AreaID);
						}
					}
				}
				Thread.CurrentThread.Priority = priority;
				int num = Environment.TickCount - tickCount;
				if (log.IsInfoEnabled)
				{
					log.Info("Scan Consortia complete!");
				}
				if (num > 120000)
				{
					log.WarnFormat("Scan all Consortia in {0} ms", num);
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("ScanConsortiaProc", exception);
				}
			}
		}

		public override void Stop()
		{
			DisposeGlobalTimers();
			SaveTimerProc(null);
			SaveRecordProc(null);
			CenterService.Stop();
			base.Stop();
		}

		public new ServerClient[] GetAllClients()
		{
			ServerClient[] array = null;
			lock (_clients.SyncRoot)
			{
				array = new ServerClient[_clients.Count];
				_clients.Keys.CopyTo(array, 0);
			}
			return array;
		}

		public void SendToALL(GSPacketIn pkg)
		{
			SendToAllZones(pkg, null);
		}

		public void SendToAllZones(GSPacketIn pkg, ServerClient except)
		{
			ServerClient[] allClients = GetAllClients();
			if (allClients == null)
			{
				return;
			}
			ServerClient[] array = allClients;
			foreach (ServerClient serverClient in array)
			{
				if (serverClient != except)
				{
					serverClient.SendTCP(pkg);
				}
			}
		}

		public void SendToALL(GSPacketIn pkg, int exceptZone)
		{
			SendToALL(pkg, null, exceptZone);
		}

		public void SendToALL(GSPacketIn pkg, ServerClient except, int exceptZone)
		{
			ServerClient[] allClients = GetAllClients();
			if (allClients == null)
			{
				return;
			}
			ServerClient[] array = allClients;
			foreach (ServerClient serverClient in array)
			{
				if (serverClient != except && serverClient.ZoneID == exceptZone)
				{
					serverClient.SendTCP(pkg);
				}
			}
		}

		public void SendConsortiaDelete(int consortiaId)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(128);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteInt(consortiaId);
			SendToALL(gSPacketIn);
		}

		public void SendSystemNotice(string msg)
		{
			SendSystemNotice(msg, 3);
		}

		public void SendGMNotice(string msg)
		{
			SendSystemNotice(msg, 0);
		}

		public void SendSystemNotice(string msg, int type)
		{
			ChatMrg.AddChat(new ChatInfo
			{
				Type = 0,
				Msg = msg,
				ChatTime = DateTime.Now
			});
			GSPacketIn gSPacketIn = new GSPacketIn(10);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteString(msg);
			SendToALL(gSPacketIn);
		}

		public bool SendAAS(bool state)
		{
			if (!StaticFunction.UpdateConfig("Center.Service.exe.config", "AAS", state.ToString()))
			{
				return false;
			}
			ASSState = state;
			GSPacketIn gSPacketIn = new GSPacketIn(7);
			gSPacketIn.WriteBoolean(state);
			SendToALL(gSPacketIn);
			return true;
		}

		public bool SendConfigState(int type, bool state)
		{
			string empty = string.Empty;
			string name;
			switch (type)
			{
			case 1:
				name = "AAS";
				break;
			case 2:
				name = "DailyAwardState";
				break;
			default:
				return false;
			}
			if (!StaticFunction.UpdateConfig("Center.Service.exe.config", name, state.ToString()))
			{
				return false;
			}
			switch (type)
			{
			case 1:
				ASSState = state;
				break;
			case 2:
				DailyAwardState = state;
				break;
			}
			SendConfigState();
			return true;
		}

		public void SendConfigState()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(8);
			gSPacketIn.WriteBoolean(ASSState);
			gSPacketIn.WriteBoolean(DailyAwardState);
			SendToALL(gSPacketIn);
		}

		public int RateUpdate(int serverId)
		{
			ServerClient[] allClients = GetAllClients();
			if (allClients != null)
			{
				ServerClient[] array = allClients;
				foreach (ServerClient serverClient in array)
				{
					if (serverClient.Info.ID == serverId)
					{
						GSPacketIn gSPacketIn = new GSPacketIn(177);
						gSPacketIn.WriteInt(serverId);
						serverClient.SendTCP(gSPacketIn);
						return 0;
					}
				}
			}
			return 1;
		}

		public int NoticeServerUpdate(int serverId, int type)
		{
			ServerClient[] allClients = GetAllClients();
			if (allClients != null)
			{
				ServerClient[] array = allClients;
				foreach (ServerClient serverClient in array)
				{
					if (serverClient.Info.ID == serverId)
					{
						GSPacketIn gSPacketIn = new GSPacketIn(11);
						gSPacketIn.WriteInt(type);
						serverClient.SendTCP(gSPacketIn);
						return 0;
					}
				}
			}
			return 1;
		}

		public void SendReloadHalloweenRank()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(90);
			gSPacketIn.WriteByte(5);
			SendToALL(gSPacketIn);
		}

		public bool SendReload(eReloadType type)
		{
			try
			{
				switch (type)
				{
				case eReloadType.center:
					_config.Refresh();
					InitGlobalTimers();
					LoadConfig();
					ServerMgr.ReLoadServerList();
					WorldMgr.Setup();
					SendConfigState();
					break;
				case eReloadType.notice:
					SystemNoticeMgr.LoadNotice("");
					break;
				case eReloadType.clearchat:
					ChatMrg.ClearAllChat();
					break;
				case (eReloadType)9999:
					ServerMgr.ReLoadServerList();
					break;
				default:
					Console.WriteLine(SendeReload(type));
					break;
				}
				GSPacketIn gSPacketIn = new GSPacketIn(11);
				gSPacketIn.WriteInt((int)type);
				SendToALL(gSPacketIn);
				return true;
			}
			catch (Exception exception)
			{
				log.Error("Order is not Exist!", exception);
			}
			return false;
		}

		public string SendeReload(eReloadType type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(193);
			gSPacketIn.WriteInt((int)type);
			SendToALL(gSPacketIn);
			return $"Send reload '{type}' to all road complete!";
		}

		public void SendShutdown()
		{
			SendToALL(new GSPacketIn(15));
		}

		public bool SendShutdown(int zoneId)
		{
			SendToALL(new GSPacketIn(15), zoneId);
			Console.WriteLine("Send shutdown to road have zoneId: {0} complete!", zoneId);
			return true;
		}

		public void SendSynBombKingAll()
		{
			ServerClient[] allClients = GetAllClients();
			if (allClients != null)
			{
				ServerClient[] array = allClients;
				foreach (ServerClient serverClient in array)
				{
					serverClient.SendSynBombKing();
				}
			}
		}

		public void SendBombKingOpenClose(bool value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(301);
			gSPacketIn.WriteBoolean(value);
			SendToALL(gSPacketIn);
		}

		public void SendBombKingUpdateRequest()
		{
			SendToALL(new GSPacketIn(302));
		}

		public void SendBombKingPlayerUpdateStep1()
		{
			SendToALL(new GSPacketIn(303));
		}

		public void SendBombKingPlayerUpdateStep2()
		{
			SendToALL(new GSPacketIn(304));
		}

		public void SendBombKingUpdatePlayer(BombKingPlayerInfo info)
		{
			SendBombKingUpdatePlayer(new List<BombKingPlayerInfo> { info });
		}

		public void SendBombKingUpdatePlayer(List<BombKingPlayerInfo> lists)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(306);
			gSPacketIn.WriteInt(lists.Count);
			foreach (BombKingPlayerInfo list in lists)
			{
				gSPacketIn.WriteInt(list.UserID);
				gSPacketIn.WriteInt(list.AreaID);
				gSPacketIn.WriteInt(list.Place);
				gSPacketIn.WriteString(list.NickName);
				gSPacketIn.WriteString(list.AreaName);
				gSPacketIn.WriteInt(list.vipType);
				gSPacketIn.WriteInt(list.vipLevel);
				gSPacketIn.WriteInt(list.myRank);
				gSPacketIn.WriteInt(list.myScore);
				gSPacketIn.WriteInt(list.Score);
				gSPacketIn.WriteInt(list.RankType);
				gSPacketIn.WriteInt(list.status);
				gSPacketIn.WriteString(list.style);
				gSPacketIn.WriteString(list.color);
				gSPacketIn.WriteBoolean(list.sex);
				gSPacketIn.WriteString(list.smg);
			}
			gSPacketIn.Compress();
			SendToALL(gSPacketIn);
		}

		public void SendBombKingClearPlayer()
		{
			SendToALL(new GSPacketIn(307));
		}

		public void SendMailToGame()
		{
			int zoneId = 1001;
			int num = 1;
			int playerId = 18;
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(zoneId);
			if (areaConfigInfo == null)
			{
				return;
			}
			using PlayerBussiness pb = new PlayerBussiness(areaConfigInfo);
			List<LuckyStartToptenAwardInfo> luckyStartAwardByRank = WorldEventMgr.GetLuckyStartAwardByRank(num);
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (LuckyStartToptenAwardInfo item in luckyStartAwardByRank)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), 1, 105);
				itemInfo.IsBinds = item.IsBinds;
				itemInfo.ValidDate = item.Validate;
				itemInfo.Count = item.Count;
				list.Add(itemInfo);
			}
			WorldEventMgr.SendItemsToMailCrosszone(pb, list, playerId, "HimeHinata™", zoneId, LanguageMgr.GetTranslation("LuckyStarTopTenAward.GetAward", num), 83, null, null);
			SendMailResponse(zoneId, playerId);
		}

		public void ShowCityBattleMember()
		{
			List<UserCityBattleInfo> allPlayers = WorldMgr.CityBattle.GetAllPlayers();
			Console.WriteLine("-TotalPlayers : " + allPlayers.Count);
			foreach (UserCityBattleInfo item in allPlayers)
			{
				Console.WriteLine("+UID:" + item.UserID + "|ZoneID:" + item.ZoneID + "|Score:" + item.Score + "|Rank:" + item.Rank + "|Team:" + item.Team);
			}
			Console.WriteLine("-BlueScore : " + WorldMgr.CityBattle.TotalBlueScore);
			Console.WriteLine("-RedScore : " + WorldMgr.CityBattle.TotalRedScore);
		}

		public void SendMailResponse(int zoneId, int playerId)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
			if (zoneLoginByAreaID != null)
			{
				ServerClient serverClient = zoneLoginByAreaID.GetServerClient(playerId);
				if (serverClient != null)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(117);
					gSPacketIn.WriteInt(playerId);
					gSPacketIn.WriteInt(1);
					serverClient.SendTCP(gSPacketIn);
				}
			}
		}

		public CenterServer(CenterServerConfig config)
		{
			_config = config;
			LoadConfig();
		}

		public void LoadConfig()
		{
			_aSSState = bool.Parse(ConfigurationManager.AppSettings["AAS"]);
			_dailyAwardState = bool.Parse(ConfigurationManager.AppSettings["DailyAwardState"]);
		}

		public static void CreateInstance(CenterServerConfig config)
		{
			if (Instance == null)
			{
				FileInfo fileInfo = new FileInfo(config.LogConfigFile);
				if (!fileInfo.Exists)
				{
					ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(CenterServer)));
				}
				XmlConfigurator.ConfigureAndWatch(fileInfo);
				_instance = new CenterServer(config);
			}
		}
	}
}

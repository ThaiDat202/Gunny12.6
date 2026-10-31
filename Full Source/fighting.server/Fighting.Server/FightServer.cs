using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Managers;
using log4net;
using log4net.Config;

namespace Fighting.Server;

public class FightServer : BaseServer
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private FightServerConfig m_config;

	private Timer m_reduceMemoryTimer;

	private bool m_running;

	private static bool m_compiled = false;

	private static FightServer m_instance;

	public FightServerConfig Configuration => m_config;

	public static FightServer Instance => m_instance;

	[DllImport("psapi.dll")]
	private static extern int EmptyWorkingSet(IntPtr hwProc);

	protected override BaseClient GetNewClient()
	{
		return new ServerClient(this);
	}

	public override bool Start()
	{
		if (m_running)
		{
			return false;
		}
		try
		{
			m_running = true;
			Thread.CurrentThread.Priority = ThreadPriority.Normal;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			GameProperties.Refresh();
			if (!InitComponent(InitSocket(IPAddress.Parse(m_config.Ip), m_config.Port), "InitSocket Port:" + m_config.Port))
			{
				return false;
			}
			if (!InitComponent(StartScriptComponents(), "Script components"))
			{
				return false;
			}
			if (!InitComponent(ProxyRoomMgr.Setup(), "RoomMgr.Setup"))
			{
				return false;
			}
			if (!InitComponent(GameMgr.Setup(0, 4), "GameMgr.Setup"))
			{
				return false;
			}
			if (!InitComponent(MapMgr.Init(), "MapMgr Init"))
			{
				return false;
			}
			if (!InitComponent(RuneMgr.Init(), "RuneMgr Init"))
			{
				return false;
			}
			if (!InitComponent(ItemMgr.Init(isUpdate: false), "ItemMgr Init"))
			{
				return false;
			}
			if (!InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
			{
				return false;
			}
			if (!InitComponent(PetMgr.Init(), "PetMgr Init"))
			{
				return false;
			}
			if (!InitComponent(MountMgr.Init(), "MountMgr Init"))
			{
				return false;
			}
			if (!InitComponent(BallMgr.Init(), "BallMgr Init"))
			{
				return false;
			}
			if (!InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
			{
				return false;
			}
			if (!InitComponent(DropMgr.Init(), "DropMgr Init"))
			{
				return false;
			}
			if (!InitComponent(BattleBonusMgr.Init(), "BattleBonusMgr Init"))
			{
				return false;
			}
			if (!InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
			{
				return false;
			}
			if (!InitComponent(WindMgr.Init(), "WindMgr Init"))
			{
				return false;
			}
			if (!InitComponent(GoldEquipMgr.Init(), "GoldEquipMgr Init"))
			{
				return false;
			}
			if (!InitComponent(GuardCoreMgr.Init(), "GuardCoreMgr Init"))
			{
				return false;
			}
			if (!InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
			{
				return false;
			}
			if (!InitComponent(PetSpecialSkillMgr.Init(), "PetSpecialSkillMgr Init"))
			{
				return false;
			}
			if (!InitComponent(EngraveMgr.Init(), "EngraveMgr Init"))
			{
				return false;
			}
			if (!InitComponent(InitGlobalTimers(), "InitGlobalTimers Init"))
			{
				return false;
			}
			GameEventMgr.Notify(ScriptEvent.Loaded);
			if (!InitComponent(base.Start(), "base.Start()"))
			{
				return false;
			}
			ProxyRoomMgr.Start();
			GameMgr.Start();
			GameEventMgr.Notify(GameServerEvent.Started, this);
			GC.Collect(GC.MaxGeneration);
			log.Info("GameServer is now open for connections!");
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
		if (!m_compiled)
		{
			string path = Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] asm_names = Configuration.ScriptAssemblies.Split(',');
			m_compiled = ScriptMgr.CompileScripts(compileVB: false, path, Configuration.ScriptCompilationTarget, asm_names);
		}
		return m_compiled;
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

	public bool InitGlobalTimers()
	{
		int num = 600000;
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

	private void DisposeGlobalTimers()
	{
		if (m_reduceMemoryTimer != null)
		{
			m_reduceMemoryTimer.Dispose();
		}
	}

	public static void MinimizeFootprint()
	{
		GC.Collect(GC.MaxGeneration);
		EmptyWorkingSet(Process.GetCurrentProcess().Handle);
	}

	protected bool StartScriptComponents()
	{
		try
		{
			ScriptMgr.InsertAssembly(typeof(FightServer).Assembly);
			ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
			Assembly[] scripts = ScriptMgr.Scripts;
			Assembly[] array = scripts;
			foreach (Assembly asm in array)
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

	public override void Stop()
	{
		if (!m_running)
		{
			return;
		}
		try
		{
			DisposeGlobalTimers();
			m_running = false;
			GameMgr.Stop();
			ProxyRoomMgr.Stop();
		}
		catch (Exception exception)
		{
			log.Error("Server stopp error:", exception);
		}
		finally
		{
			base.Stop();
		}
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
		SendToALL(pkg, null);
	}

	public void SendToALL(GSPacketIn pkg, ServerClient except)
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

	private FightServer(FightServerConfig config)
	{
		m_config = config;
	}

	public static void CreateInstance(FightServerConfig config)
	{
		if (m_instance == null)
		{
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(FightServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			m_instance = new FightServer(config);
		}
	}
}

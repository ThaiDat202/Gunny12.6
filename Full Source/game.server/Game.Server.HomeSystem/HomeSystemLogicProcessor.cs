using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HomeSystem.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HomeSystem;

[HomeSystemProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class HomeSystemLogicProcessor : AbstractHomeSystemProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HomeSystemHandleMgr _commandMgr;

	public HomeSystemLogicProcessor()
	{
		_commandMgr = new HomeSystemHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HomePackageType homePackageType = (HomePackageType)packet.ReadByte();
		try
		{
			IHomeSystemCommandHadler homeSystemCommandHadler = _commandMgr.LoadCommandHandler((int)homePackageType);
			if (homeSystemCommandHadler != null)
			{
				homeSystemCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HomeSystemPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), homePackageType));
		}
	}
}

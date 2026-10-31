using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.ActiveSystem.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.ActiveSystem;

[ActiveSystemProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class ActiveSystemLogicProcessor : AbstractActiveSystemProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private ActiveSystemHandleMgr _commandMgr;

	public ActiveSystemLogicProcessor()
	{
		_commandMgr = new ActiveSystemHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		ActiveSystemPackageType activeSystemPackageType = (ActiveSystemPackageType)packet.ReadByte();
		try
		{
			IActiveSystemCommandHadler activeSystemCommandHadler = _commandMgr.LoadCommandHandler((int)activeSystemPackageType);
			if (activeSystemCommandHadler != null)
			{
				activeSystemCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("ActiveSystemPackageType: {0} not found!", activeSystemPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception arg)
		{
			log.Error(string.Format("ActiveSystemPackageType: {1}, OnGameData is Error: {0}", arg, activeSystemPackageType));
		}
	}
}

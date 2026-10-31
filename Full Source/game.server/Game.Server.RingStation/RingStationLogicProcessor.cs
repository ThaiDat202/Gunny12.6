using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.RingStation.Handle;
using log4net;

namespace Game.Server.RingStation;

[RingStationProcessorAtribute(40, "礼堂逻辑")]
public class RingStationLogicProcessor : AbstractRingStationProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private RingStationHandleMgr _commandMgr;

	public RingStationLogicProcessor()
	{
		_commandMgr = new RingStationHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		RingStationPackageType ringStationPackageType = (RingStationPackageType)packet.ReadByte();
		try
		{
			IRingStationCommandHadler ringStationCommandHadler = _commandMgr.LoadCommandHandler((int)ringStationPackageType);
			if (ringStationCommandHadler != null)
			{
				ringStationCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch
		{
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("RingStationLogicProcessor PackageType {0} not found!", ringStationPackageType);
			Console.WriteLine("_______________END_______________");
		}
	}
}

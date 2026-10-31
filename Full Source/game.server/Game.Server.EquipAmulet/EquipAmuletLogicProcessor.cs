using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.EquipAmulet.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.EquipAmulet;

[EquipAmuletProcessorAtribute(40, "礼堂逻辑")]
public class EquipAmuletLogicProcessor : AbstractEquipAmuletProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private EquipAmuletHandleMgr _commandMgr;

	public EquipAmuletLogicProcessor()
	{
		_commandMgr = new EquipAmuletHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		EquipAmuletPackageType equipAmuletPackageType = (EquipAmuletPackageType)packet.ReadByte();
		try
		{
			IEquipAmuletCommandHadler equipAmuletCommandHadler = _commandMgr.LoadCommandHandler((int)equipAmuletPackageType);
			if (equipAmuletCommandHadler != null)
			{
				equipAmuletCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("EquipAmuletLogicProcessor EquipAmuletPackageType.{0} not found!", equipAmuletPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("EquipAmuletLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), equipAmuletPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.FlowerGiving.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.FlowerGiving;

[FlowerGivingProcessorAtribute(40, "礼堂逻辑")]
public class FlowerGivingLogicProcessor : AbstractFlowerGivingProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private FlowerGivingHandleMgr _commandMgr;

	public FlowerGivingLogicProcessor()
	{
		_commandMgr = new FlowerGivingHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		FlowerGivingPackageType code = (FlowerGivingPackageType)packet.ReadByte();
		try
		{
			IFlowerGivingCommandHadler flowerGivingCommandHadler = _commandMgr.LoadCommandHandler((int)code);
			if (flowerGivingCommandHadler != null)
			{
				flowerGivingCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
		}
	}
}

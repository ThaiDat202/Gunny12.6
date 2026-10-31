using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Horse.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Horse;

[HorseProcessorAtribute(40, "礼堂逻辑")]
public class HorseLogicProcessor : AbstractHorseProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HorseHandleMgr _commandMgr;

	public HorseLogicProcessor()
	{
		_commandMgr = new HorseHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HorsePackageType horsePackageType = (HorsePackageType)packet.ReadByte();
		try
		{
			IHorseCommandHadler horseCommandHadler = _commandMgr.LoadCommandHandler((int)horsePackageType);
			if (horseCommandHadler != null)
			{
				horseCommandHadler.CommandHandler(player, packet);
				if (player.DebugMode)
				{
					Console.WriteLine("HorseLogicProcessor PackageType {0}", horsePackageType);
				}
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("HorseLogicProcessor PackageType {0} not found!", horsePackageType);
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception arg)
		{
			log.Error(string.Format("type:{1}, OnGameData is Error: {0}", arg, horsePackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

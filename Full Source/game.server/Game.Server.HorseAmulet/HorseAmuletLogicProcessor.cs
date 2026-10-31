using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HorseAmulet.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HorseAmulet;

[HorseAmuletProcessorAtribute(40, "礼堂逻辑")]
public class HorseAmuletLogicProcessor : AbstractHorseAmuletProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HorseAmuletHandleMgr _commandMgr;

	public HorseAmuletLogicProcessor()
	{
		_commandMgr = new HorseAmuletHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HorseAmuletPackageType horseAmuletPackageType = (HorseAmuletPackageType)packet.ReadByte();
		try
		{
			IHorseAmuletCommandHadler horseAmuletCommandHadler = _commandMgr.LoadCommandHandler((int)horseAmuletPackageType);
			if (horseAmuletCommandHadler != null)
			{
				if (player.Client.Server.Configuration.DebugMode)
				{
					Console.WriteLine(horseAmuletCommandHadler);
				}
				horseAmuletCommandHadler.CommandHandler(player, packet);
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("HorseAmuletLogicProcessor HorseAmuletPackageType.{0} not found!", horseAmuletPackageType);
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HorseAmuletLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), horseAmuletPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

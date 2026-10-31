using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HomeTemple.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HomeTemple;

[HomeTempleProcessorAtribute(40, "礼堂逻辑")]
public class HomeTempleLogicProcessor : AbstractHomeTempleProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HomeTempleHandleMgr _commandMgr;

	public HomeTempleLogicProcessor()
	{
		_commandMgr = new HomeTempleHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HomeTemplePackageType homeTemplePackageType = (HomeTemplePackageType)packet.ReadByte();
		try
		{
			IHomeTempleCommandHadler homeTempleCommandHadler = _commandMgr.LoadCommandHandler((int)homeTemplePackageType);
			if (homeTempleCommandHadler != null)
			{
				homeTempleCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("HomeTempleLogicProcessor HomeTemplePackageType.{0} not found!", homeTemplePackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HomeTempleLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), homeTemplePackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

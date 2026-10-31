using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GodCardRaise.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.GodCardRaise;

[GodCardRaiseProcessorAtribute(40, "礼堂逻辑")]
public class GodCardRaiseLogicProcessor : AbstractGodCardRaiseProcessor
{
	private static readonly ILog log;

	private GodCardRaiseHandleMgr godCardRaiseHandleMgr_0;

	static GodCardRaiseLogicProcessor()
	{
		log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}

	public GodCardRaiseLogicProcessor()
	{
		godCardRaiseHandleMgr_0 = new GodCardRaiseHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		GodCardRaisePackageType godCardRaisePackageType = (GodCardRaisePackageType)packet.ReadByte();
		try
		{
			IGodCardRaiseCommandHadler godCardRaiseCommandHadler = godCardRaiseHandleMgr_0.LoadCommandHandler((int)godCardRaisePackageType);
			if (godCardRaiseCommandHadler == null)
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("GodCardRaiseLogicProcessor GodCardRaisePackageType.{0} not found!", godCardRaisePackageType);
				Console.WriteLine("_______________END_______________");
				return;
			}
			godCardRaiseCommandHadler.CommandHandler(player, packet);
			if (player.Client.Server.Configuration.DebugMode)
			{
				Console.WriteLine(godCardRaiseCommandHadler);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			log.Error(string.Format("GodCardRaiseLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex2.ToString(), godCardRaisePackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

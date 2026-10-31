using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GuardCore.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.GuardCore;

[GuardCoreProcessorAtribute(40, "礼堂逻辑")]
public class GuardCoreLogicProcessor : AbstractGuardCoreProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private GuardCoreHandleMgr _commandMgr;

	public GuardCoreLogicProcessor()
	{
		_commandMgr = new GuardCoreHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		GuardCorePackageType guardCorePackageType = (GuardCorePackageType)packet.ReadByte();
		try
		{
			IGuardCoreCommandHadler guardCoreCommandHadler = _commandMgr.LoadCommandHandler((int)guardCorePackageType);
			if (guardCoreCommandHadler != null)
			{
				guardCoreCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("GuardCoreLogicProcessor GuardCorePackageType.{0} not found!", guardCorePackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("GuardCoreLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), guardCorePackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

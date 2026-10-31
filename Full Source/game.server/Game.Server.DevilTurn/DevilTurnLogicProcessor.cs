using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.DevilTurn.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.DevilTurn;

public class DevilTurnLogicProcessor : AbstractDevilTurnProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private DevilTurnHandleMgr _commandMgr;

	public DevilTurnLogicProcessor()
	{
		_commandMgr = new DevilTurnHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		DevilTurnPackageType devilTurnPackageType = (DevilTurnPackageType)packet.ReadByte();
		try
		{
			IDevilTurnCommandHandler devilTurnCommandHandler = _commandMgr.LoadCommandHandler((int)devilTurnPackageType);
			if (devilTurnCommandHandler != null)
			{
				devilTurnCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("DevilTurnLogicProcessor DevilTurnPackageType.{0} not found!", devilTurnPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("DevilTurnLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), devilTurnPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

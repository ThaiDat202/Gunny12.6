using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Labyrinth.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Labyrinth;

public class LabyrinthLogicProcessor : AbstractLabyrinthProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private LabyrinthHandleMgr _commandMgr;

	public LabyrinthLogicProcessor()
	{
		_commandMgr = new LabyrinthHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		LabyrinthPackageType labyrinthPackageType = (LabyrinthPackageType)packet.ReadInt();
		try
		{
			ILabyrinthCommandHandler labyrinthCommandHandler = _commandMgr.LoadCommandHandler((int)labyrinthPackageType);
			if (labyrinthCommandHandler != null)
			{
				labyrinthCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LabyrinthLogicProcessor LabyrinthPackageType.{0} not found!", labyrinthPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("LabyrinthLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), labyrinthPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.ConsortiaTask.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.ConsortiaTask;

public class ConsortiaTaskLogicProcessor : AbstractConsortiaTaskProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private ConsortiaTaskHandleMgr _commandMgr;

	public ConsortiaTaskLogicProcessor()
	{
		_commandMgr = new ConsortiaTaskHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		ConsortiaTaskPackageType consortiaTaskPackageType = (ConsortiaTaskPackageType)packet.ReadInt();
		try
		{
			IConsortiaTaskCommandHandler consortiaTaskCommandHandler = _commandMgr.LoadCommandHandler((int)consortiaTaskPackageType);
			if (consortiaTaskCommandHandler != null)
			{
				consortiaTaskCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("ConsortiaTaskLogicProcessor ConsortiaTaskPackageType.{0} not found!", consortiaTaskPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("ConsortiaTaskLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), consortiaTaskPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

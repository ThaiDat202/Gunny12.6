using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.Emblem.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Emblem;

public class EmblemLogicProcessor : AbstractEmblemProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private EmblemHandleMgr _commandMgr;

	public EmblemLogicProcessor()
	{
		_commandMgr = new EmblemHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		EmblemEpackgeType emblemEpackgeType = (EmblemEpackgeType)packet.ReadByte();
		try
		{
			IEmblemCommandHandler emblemCommandHandler = _commandMgr.LoadCommandHandler((int)emblemEpackgeType);
			if (emblemCommandHandler != null)
			{
				emblemCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("EmblemLogicProcessor EmblemEpackgeType.{0} not found!", emblemEpackgeType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("EmblemLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), emblemEpackgeType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

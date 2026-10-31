using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.Academy.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Academy;

public class AcademyLogicProcessor : AbstractAcademyProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private AcademyHandleMgr _commandMgr;

	public AcademyLogicProcessor()
	{
		_commandMgr = new AcademyHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		AcademyPackageType academyPackageType = (AcademyPackageType)packet.ReadByte();
		try
		{
			IAcademyCommandHandler academyCommandHandler = _commandMgr.LoadCommandHandler((int)academyPackageType);
			if (academyCommandHandler != null)
			{
				academyCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("AcademyLogicProcessor AcademyPackageType.{0} not found!", academyPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("AcademyLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), academyPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

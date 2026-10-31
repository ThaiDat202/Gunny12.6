using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.Elf.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Elf;

public class ElfLogicProcessor : AbstractElfProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private ElfHandleMgr _commandMgr;

	public ElfLogicProcessor()
	{
		_commandMgr = new ElfHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		ElfPackageType elfPackageType = (ElfPackageType)packet.ReadByte();
		try
		{
			IElfCommandHandler elfCommandHandler = _commandMgr.LoadCommandHandler((int)elfPackageType);
			if (elfCommandHandler != null)
			{
				elfCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("ElfLogicProcessor ElfPackageType.{0} not found!", elfPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("ElfLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), elfPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.MemoryGame.Handle;
using log4net;

namespace Game.Server.MemoryGame;

[MemoryGameProcessorAtribute(40, "礼堂逻辑")]
public class MemoryGameLogicProcessor : AbstractMemoryGameProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private MemoryGameHandleMgr _commandMgr;

	public MemoryGameLogicProcessor()
	{
		_commandMgr = new MemoryGameHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		MemoryGamePackageType memoryGamePackageType = (MemoryGamePackageType)packet.ReadByte();
		try
		{
			IMemoryGameCommandHadler memoryGameCommandHadler = _commandMgr.LoadCommandHandler((int)memoryGamePackageType);
			if (memoryGameCommandHadler != null)
			{
				memoryGameCommandHadler.CommandHandler(player, packet);
				if (player.Client.Server.Configuration.DebugMode)
				{
					Console.WriteLine(memoryGameCommandHadler);
				}
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("MemoryGameLogicProcessor MemoryGamePackageType.{0} not found!", memoryGamePackageType);
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception ex)
		{
			log.Error(string.Format("MemoryGameLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), memoryGamePackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}

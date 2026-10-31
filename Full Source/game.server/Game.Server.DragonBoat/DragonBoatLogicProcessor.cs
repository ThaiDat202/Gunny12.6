using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.DragonBoat.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.DragonBoat;

[DragonBoatProcessorAtribute(40, "礼堂逻辑")]
public class DragonBoatLogicProcessor : AbstractDragonBoatProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private DragonBoatHandleMgr _commandMgr;

	public DragonBoatLogicProcessor()
	{
		_commandMgr = new DragonBoatHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		DragonBoatPackageType code = (DragonBoatPackageType)packet.ReadByte();
		try
		{
			IDragonBoatCommandHadler dragonBoatCommandHadler = _commandMgr.LoadCommandHandler((int)code);
			if (dragonBoatCommandHadler != null)
			{
				dragonBoatCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
		}
	}
}

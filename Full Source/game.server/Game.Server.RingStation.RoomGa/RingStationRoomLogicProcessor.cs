using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.RingStation.RoomGamePkg.TankHandle;
using log4net;

namespace Game.Server.RingStation.RoomGamePkg;

[GameProcessor(99, "礼堂逻辑")]
public class RingStationRoomLogicProcessor : AbstractGameProcessor
{
	private GameCommandMgr _commandMgr = new GameCommandMgr();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public readonly int TIMEOUT = 60000;

	public override void OnGameData(RoomGame room, VirtualGamePlayer player, GSPacketIn packet)
	{
		GameCmdType code = (GameCmdType)packet.Code;
		try
		{
			IGameCommandHandler gameCommandHandler = _commandMgr.LoadCommandHandler((int)code);
			if (gameCommandHandler != null)
			{
				gameCommandHandler.HandleCommand(this, player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error($" RingStationLogic OnGameData is Error: {ex.ToString()}, type: {code}");
		}
	}

	public override void OnTick(RoomGame room)
	{
	}
}

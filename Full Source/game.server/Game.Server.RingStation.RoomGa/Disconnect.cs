using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle;

[GameCommandAttbute(83)]
public class Disconnect : IGameCommandHandler
{
	public bool HandleCommand(RingStationRoomLogicProcessor process, VirtualGamePlayer player, GSPacketIn packet)
	{
		return true;
	}
}

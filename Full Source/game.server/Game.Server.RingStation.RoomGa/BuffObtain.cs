using Game.Base.Packets;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle;

[GameCommandAttbute(186)]
public class BuffObtain : IGameCommandHandler
{
	public bool HandleCommand(RingStationRoomLogicProcessor process, VirtualGamePlayer player, GSPacketIn packet)
	{
		return true;
	}
}

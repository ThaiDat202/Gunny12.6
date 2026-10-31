using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms;

public interface IHotSpringRoomsProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}

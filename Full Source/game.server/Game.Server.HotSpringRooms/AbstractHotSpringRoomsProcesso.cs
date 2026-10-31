using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms;

public abstract class AbstractHotSpringRoomsProcessor : IHotSpringRoomsProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}

using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms.Handle;

public interface IHotSpringRoomsCommandHadler
{
	bool CommandHandler(GamePlayer Player, GSPacketIn packet);
}

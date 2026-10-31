using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms.Handle;

[HotSpringRoomsHandleAttbute(13)]
public class ClickFlowerEgg : IHotSpringRoomsCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.CurrentHotSpringRoom != null)
		{
			int id = packet.ReadInt();
			int type = packet.ReadInt();
			Player.CurrentHotSpringRoom.HotSpringFlower_Egg(id, type);
			return true;
		}
		return false;
	}
}

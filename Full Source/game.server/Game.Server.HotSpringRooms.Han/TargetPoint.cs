using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HotSpringRooms.Handle;

[HotSpringRoomsHandleAttbute(1)]
public class TargetPoint : IHotSpringRoomsCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.CurrentHotSpringRoom != null)
		{
			string pathStr = packet.ReadString();
			int id = packet.ReadInt();
			int x = packet.ReadInt();
			int y = packet.ReadInt();
			int num = packet.ReadInt();
			int direction = packet.ReadInt();
			Player.CurrentHotSpringRoom.PlayerMove(id, x, y, pathStr, direction);
			return true;
		}
		return false;
	}
}

using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.NewHall.Handle;

[NewHallHandleAttbute(2)]
public class PlayerExit : INewHallCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		WorldMgr.NewHallRooms.RemovePlayer(id);
		return true;
	}
}

using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.NewHall.Handle;

[NewHallHandleAttbute(0)]
public class PlayerInfo : INewHallCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		WorldMgr.NewHallRooms.PlayerInfo(Player);
		return true;
	}
}

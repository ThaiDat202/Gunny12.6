using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(5)]
public class RemovePlayer : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.CurrentRoom != null)
		{
			if (Player.CurrentRoom.RoomType == eRoomType.ActivityDungeon)
			{
				Player.Actives.SendQXAward();
			}
			if (Player.CurrentRoom.RoomType == eRoomType.BombKing)
			{
				Player.battleStage = 0;
			}
			RoomMgr.ExitRoom(Player.CurrentRoom, Player);
		}
		return true;
	}
}

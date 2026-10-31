using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(1)]
public class Login : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int type = packet.ReadInt();
		int num = packet.ReadInt();
		int roomId = -1;
		string pwd = "";
		if (num == -1)
		{
			roomId = packet.ReadInt();
			pwd = packet.ReadString();
		}
		RoomMgr.EnterRoom(Player, roomId, pwd, type, isInvite: false);
		return true;
	}
}

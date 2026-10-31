using Bussiness;
using Game.Base.Packets;
using Game.Server.HotSpringRooms;

namespace Game.Server.Packets.Client;

[PacketHandler(212, "礼堂数据")]
public class HotSpringEnterConfirmHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int roomId = packet.ReadInt();
		HotSpringRoom singleRoom = HotSpringRoomsMgr.GetSingleRoom(roomId);
		if (singleRoom == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("HotSpringEnterConfirmHandler.RoomNotExit"));
		}
		else
		{
			GSPacketIn gSPacketIn = new GSPacketIn(212);
			gSPacketIn.WriteInt(singleRoom.RoomId);
			client.Player.SendTCP(gSPacketIn);
		}
		return 0;
	}
}

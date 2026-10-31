using Bussiness;
using Game.Base.Packets;
using Game.Server.HotSpringRooms;

namespace Game.Server.Packets.Client;

[PacketHandler(190, "礼堂数据")]
public class HotSpringRoomQuickEnterHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		HotSpringRoom randomRoom = HotSpringRoomsMgr.GetRandomRoom();
		if (randomRoom == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("HotSpringEnterConfirmHandler.RoomFullAll"));
		}
		else
		{
			GSPacketIn gSPacketIn = new GSPacketIn(212);
			gSPacketIn.WriteInt(randomRoom.RoomId);
			client.Player.SendTCP(gSPacketIn);
		}
		return 0;
	}
}

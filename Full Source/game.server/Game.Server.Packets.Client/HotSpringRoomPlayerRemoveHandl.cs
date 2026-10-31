using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(169, "礼堂数据")]
public class HotSpringRoomPlayerRemoveHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = -1;
		if (client.Player.CurrentHotSpringRoom != null)
		{
			num = client.Player.CurrentHotSpringRoom.RoomId;
			client.Player.CurrentHotSpringRoom.RemovePlayer(client.Player);
			GSPacketIn gSPacketIn = new GSPacketIn(169);
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("HotSpringRoomPlayerRemoveHandler.Msg1", num));
			client.Player.SendTCP(gSPacketIn);
		}
		return 0;
	}
}

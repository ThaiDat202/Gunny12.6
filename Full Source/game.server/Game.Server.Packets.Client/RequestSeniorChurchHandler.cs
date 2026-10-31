using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(338, "打开物品")]
public class RequestSeniorChurchHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(338);
		gSPacketIn.WriteBoolean(val: false);
		client.SendTCP(gSPacketIn);
		return 1;
	}
}

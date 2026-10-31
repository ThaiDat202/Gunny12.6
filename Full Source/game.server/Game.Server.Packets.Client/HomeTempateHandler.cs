using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(297, "神庙系统")]
public class HomeTempateHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.HomeTemple.ProcessData(client.Player, packet);
		return 1;
	}
}

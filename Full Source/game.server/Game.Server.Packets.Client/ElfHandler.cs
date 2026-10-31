using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(664, "圣灵系统")]
public class ElfHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.ElfHandler.ProcessData(client.Player, packet);
		return 1;
	}
}

using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(673, "考验")]
public class RapportHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		client.Player.SendMessage("Chức năng này đang được phát triển...");
		return 1;
	}
}

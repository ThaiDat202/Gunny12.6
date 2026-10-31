using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(655, "纹章系统")]
public class EmblemHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.EmblemHandler.ProcessData(client.Player, packet);
		return 1;
	}
}

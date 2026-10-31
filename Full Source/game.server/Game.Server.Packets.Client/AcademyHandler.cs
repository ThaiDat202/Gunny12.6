using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(141, "师徒系统")]
public class AcademyHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.AcademyHandler.ProcessData(client.Player, packet);
		return 1;
	}
}

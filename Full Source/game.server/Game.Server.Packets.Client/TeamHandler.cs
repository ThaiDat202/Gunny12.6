using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(390, "战队系统")]
public class TeamHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.TeamHandler.ProcessData(client.Player, packet);
		return 1;
	}
}

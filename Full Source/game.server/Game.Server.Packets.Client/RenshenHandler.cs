using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(298, "场景用户离开")]
public class RenshenHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.RenshenHandler != null)
		{
			client.Player.RenshenHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}

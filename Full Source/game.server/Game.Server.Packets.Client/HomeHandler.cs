using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(285, "场景用户离开")]
public class HomeHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.HomeSystemHandler != null)
		{
			client.Player.HomeSystemHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}

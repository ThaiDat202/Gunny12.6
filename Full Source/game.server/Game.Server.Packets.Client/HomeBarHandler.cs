using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(294, "场景用户离开")]
public class HomeBarHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.HomeBarHandler != null)
		{
			client.Player.HomeBarHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}

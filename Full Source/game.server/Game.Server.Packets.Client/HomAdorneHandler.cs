using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(292, "场景用户离开")]
public class HomAdorneHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.HomeAdornHandler != null)
		{
			client.Player.HomeAdornHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}

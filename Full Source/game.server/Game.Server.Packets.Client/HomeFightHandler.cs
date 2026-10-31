using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(291, "场景用户离开")]
public class HomeFightHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.HomeFightHandler != null)
		{
			client.Player.HomeFightHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}

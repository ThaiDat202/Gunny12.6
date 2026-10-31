using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(296, "场景用户离开")]
public class MoneyTreeHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.MoneyTreeHandler != null)
		{
			client.Player.MoneyTreeHandler.ProcessData(client.Player, packet);
		}
		return 0;
	}
}

using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(336, "打开物品")]
public class GuardCoreHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.GuardCore == null)
		{
			return 0;
		}
		client.Player.GuardCore.ProcessData(client.Player, packet);
		return 1;
	}
}

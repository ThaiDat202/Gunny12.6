using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(375, "打开物品")]
public class HorseAmuletHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.HorseAmulet == null)
		{
			return 0;
		}
		client.Player.HorseAmulet.ProcessData(client.Player, packet);
		return 1;
	}
}

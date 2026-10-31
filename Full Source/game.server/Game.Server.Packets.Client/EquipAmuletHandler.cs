using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(386, "打开物品")]
public class EquipAmuletHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.EquipAmulet == null)
		{
			return 0;
		}
		client.Player.EquipAmulet.ProcessData(client.Player, packet);
		return 1;
	}
}

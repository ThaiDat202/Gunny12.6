using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(313, "客户端日记")]
public class ItemCellIsClockedHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType bageType = (eBageType)packet.ReadInt();
		int slot = packet.ReadInt();
		bool cellLocked = packet.ReadBoolean();
		PlayerInventory inventory = client.Player.GetInventory(bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		if (itemAt != null)
		{
			itemAt.cellLocked = cellLocked;
			inventory.UpdateItem(itemAt);
		}
		return 0;
	}
}

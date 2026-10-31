using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(127, "物品比较")]
public class ItemReclaimHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType bageType = (eBageType)packet.ReadByte();
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory(bageType);
		ItemInfo itemAt = inventory.GetItemAt(num);
		if (itemAt != null)
		{
			if (inventory.GetItemAt(num).Count <= num2)
			{
				num2 = inventory.GetItemAt(num).Count;
			}
			int num3 = num2 * itemAt.Template.ReclaimValue;
			if (itemAt.Template.ReclaimType == 2)
			{
				client.Player.AddGiftToken(num3);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success1", num3));
			}
			if (itemAt.Template.ReclaimType == 1)
			{
				client.Player.AddGold(num3);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success2", num3));
			}
			inventory.RemoveItemAt(num);
			return 0;
		}
		client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.NoSuccess"));
		return 1;
	}
}

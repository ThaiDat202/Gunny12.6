using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(55, "出售道具")]
public class PropSellHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		int iD = packet.ReadInt();
		ItemInfo itemAt = client.Player.FightBag.GetItemAt(slot);
		if (itemAt != null)
		{
			client.Player.FightBag.RemoveItem(itemAt);
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			ShopMgr.SetItemType(shopItemInfoById, 1, ref specialValue);
			client.Player.AddGold(specialValue.Gold);
		}
		return 0;
	}
}

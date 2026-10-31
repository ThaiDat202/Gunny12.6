using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(120, "物品倾向转移")]
public class ItemTrendHandle : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType bagType = (eBageType)packet.ReadInt();
		int place = packet.ReadInt();
		eBageType bagType2 = (eBageType)packet.ReadInt();
		ItemInfo itemInfo = null;
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		ItemInfo itemInfo2 = null;
		int num = packet.ReadInt();
		int operation = packet.ReadInt();
		if (num == -1)
		{
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			int num4 = 0;
			int num5 = 0;
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(34101);
			itemInfo2 = ItemInfo.CreateFromTemplate(goods, 1, 102);
			list = ShopMgr.FindShopbyTemplatID(34101);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
				{
					num5 = list[i].AValue1;
					itemInfo2.ValidDate = list[i].AUnit;
				}
			}
			if (itemInfo2 != null)
			{
				if (num4 <= client.Player.PlayerCharacter.Gold && num5 <= client.Player.PlayerCharacter.Money)
				{
					client.Player.RemoveMoney(num5);
					client.Player.RemoveGold(num4);
				}
				else
				{
					itemInfo2 = null;
				}
			}
		}
		else
		{
			itemInfo2 = client.Player.GetItemAt(bagType2, num);
		}
		itemInfo = client.Player.GetItemAt(bagType, place);
		StringBuilder stringBuilder = new StringBuilder();
		if (itemInfo2 == null || itemInfo == null)
		{
			return 1;
		}
		bool result = false;
		ItemTemplateInfo itemTemplateInfo = RefineryMgr.RefineryTrend(operation, itemInfo, ref result);
		if (result && itemTemplateInfo != null)
		{
			ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 115);
			AbstractInventory itemInventory = client.Player.GetItemInventory(itemTemplateInfo);
			if (itemInventory.AddItem(itemInfo3, itemInventory.BeginSlot))
			{
				client.Player.UpdateItem(itemInfo3);
				client.Player.RemoveItem(itemInfo);
				itemInfo2.Count--;
				client.Player.UpdateItem(itemInfo2);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Success"));
			}
			else
			{
				stringBuilder.Append("NoPlace");
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(itemInfo3.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
			}
			return 1;
		}
		client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemTrendHandle.Fail"));
		return 1;
	}
}

using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(126, "场景用户离开")]
public class QuickBuyGoldBoxHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(1123301);
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
		int value = num * shopItemInfoById.AValue1;
		if (client.Player.MoneyDirect(value))
		{
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			List<ItemInfo> itemInfos = new List<ItemInfo>();
			ItemBoxMgr.CreateItemBox(itemTemplateInfo.TemplateID, itemInfos, ref specialValue);
			int num2 = num * specialValue.Gold;
			client.Player.AddGold(num2);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("QuickBuyGoldBoxHandler.Msg", num2));
		}
		return 0;
	}
}

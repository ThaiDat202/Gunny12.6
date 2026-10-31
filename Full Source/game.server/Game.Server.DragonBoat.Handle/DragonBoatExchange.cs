using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DragonBoat.Handle;

[DragonBoatHandleAttbute(4)]
public class DragonBoatExchange : IDragonBoatCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(100);
		ActiveSystemInfo info = Player.Actives.Info;
		int iD = packet.ReadInt();
		int num = packet.ReadInt();
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		eMessageType type = eMessageType.Normal;
		string translation = LanguageMgr.GetTranslation("UserBuyItemHandler.Success");
		ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
		if (shopItemInfoById == null)
		{
			return false;
		}
		bool flag = false;
		if (shopItemInfoById != null && ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 95)
		{
			flag = true;
		}
		if (!flag)
		{
			Player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
			return false;
		}
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		for (int i = 0; i < num; i++)
		{
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
			if (itemInfo != null || shopItemInfoById != null)
			{
				if (shopItemInfoById.BuyType == 0)
				{
					itemInfo.ValidDate = shopItemInfoById.AUnit;
				}
				else
				{
					itemInfo.Count = shopItemInfoById.AUnit;
				}
				itemInfo.IsBinds = true;
				if (!dictionary.Keys.Contains(itemInfo.TemplateID))
				{
					dictionary.Add(itemInfo.TemplateID, itemInfo);
				}
				else
				{
					dictionary[itemInfo.TemplateID].Count += itemInfo.Count;
				}
				ShopMgr.SetItemType(shopItemInfoById, 1, ref specialValue);
			}
		}
		if (dictionary.Count == 0)
		{
			return false;
		}
		bool flag2 = false;
		if (info.useableScore < specialValue.UseableScore)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("DragonBoatHandler.Msg1"));
			return false;
		}
		info.useableScore -= specialValue.UseableScore;
		if (true)
		{
			string text = "";
			foreach (ItemInfo value in dictionary.Values)
			{
				text += ((text == "") ? value.TemplateID.ToString() : ("," + value.TemplateID));
				if (value.Template.MaxCount == 1)
				{
					for (int j = 0; j < value.Count; j++)
					{
						ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(value.Template, value);
						itemInfo2.Count = 1;
						Player.AddTemplate(itemInfo2);
					}
					continue;
				}
				int num2 = 0;
				for (int k = 0; k < value.Count; k++)
				{
					if (num2 == value.Template.MaxCount)
					{
						ItemInfo itemInfo3 = ItemInfo.CloneFromTemplate(value.Template, value);
						itemInfo3.Count = num2;
						Player.AddTemplate(itemInfo3);
						num2 = 0;
					}
					num2++;
				}
				if (num2 > 0)
				{
					ItemInfo itemInfo4 = ItemInfo.CloneFromTemplate(value.Template, value);
					itemInfo4.Count = num2;
					Player.AddTemplate(itemInfo4);
					num2 = 0;
				}
			}
		}
		else
		{
			type = eMessageType.ERROR;
			translation = LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission");
		}
		Player.SendMessage(type, LanguageMgr.GetTranslation(translation));
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(info.useableScore);
		gSPacketIn.WriteInt(info.totalScore);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}

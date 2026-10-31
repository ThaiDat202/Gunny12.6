using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(8)]
public class BuyItems : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int iD = packet.ReadInt();
		int num = packet.ReadInt();
		if (num <= 0 || num > 999)
		{
			return 1;
		}
		MinigameShopTemplateInfo minigameShopTemplateInfo = ShopMgr.FindMinigameShop(iD);
		if (minigameShopTemplateInfo == null || minigameShopTemplateInfo.Type != 1)
		{
			return 1;
		}
		int num2 = num * minigameShopTemplateInfo.Price;
		if (player.Actives.Info.SXCrystal >= num2)
		{
			Dictionary<int, int> dictionary = JsonConvert.DeserializeObject<Dictionary<int, int>>(player.Actives.Info.MiniShopBuyCount);
			if (dictionary == null)
			{
				dictionary = new Dictionary<int, int>();
			}
			if (!dictionary.ContainsKey(minigameShopTemplateInfo.ID) || dictionary[minigameShopTemplateInfo.ID] + num <= minigameShopTemplateInfo.LimitCount)
			{
				player.Actives.Info.SXCrystal -= num2;
				if (!dictionary.ContainsKey(minigameShopTemplateInfo.ID))
				{
					dictionary.Add(minigameShopTemplateInfo.ID, num);
				}
				else
				{
					dictionary[minigameShopTemplateInfo.ID] += num;
				}
				player.Actives.Info.MiniShopBuyCount = JsonConvert.SerializeObject(dictionary);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(minigameShopTemplateInfo.ItemID), minigameShopTemplateInfo.Count * num, 105);
				itemInfo.IsBinds = minigameShopTemplateInfo.IsBind;
				itemInfo.ValidDate = minigameShopTemplateInfo.Valid;
				player.AddTemplate(itemInfo);
				player.Actives.SendSXStoreData(dictionary);
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Shop.LimitCount.Msg"));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Shop.NoCrystal.Msg"));
		}
		return 1;
	}
}

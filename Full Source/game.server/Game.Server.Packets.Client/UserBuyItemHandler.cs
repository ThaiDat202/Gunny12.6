using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(44, "购买物品")]
public class UserBuyItemHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		StringBuilder stringBuilder = new StringBuilder();
		eMessageType type = eMessageType.Normal;
		string translateId = "UserBuyItemHandler.Success";
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			packet.ReadInt();
			int iD = packet.ReadInt();
			int num2 = packet.ReadInt();
			string text = packet.ReadString();
			packet.ReadBoolean();
			string text2 = packet.ReadString();
			packet.ReadInt();
			bool isBinds = packet.ReadBoolean();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			if (shopItemInfoById == null || !ShopMgr.IsOnShop(shopItemInfoById.ID) || !shopItemInfoById.CanBuy || !ShopMgr.CanBuy(shopItemInfoById.ShopID, consortiaInfo?.ShopLevel ?? 1, ref isBinds, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches))
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
				return 1;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 102);
			if (shopItemInfoById.BuyType == 0)
			{
				if (num2 == 1)
				{
					itemInfo.ValidDate = shopItemInfoById.AUnit;
				}
				if (num2 == 2)
				{
					itemInfo.ValidDate = shopItemInfoById.BUnit;
				}
				if (num2 == 3)
				{
					itemInfo.ValidDate = shopItemInfoById.CUnit;
				}
			}
			else
			{
				if (num2 == 1)
				{
					itemInfo.Count = shopItemInfoById.AUnit;
				}
				if (num2 == 2)
				{
					itemInfo.Count = shopItemInfoById.BUnit;
				}
				if (num2 == 3)
				{
					itemInfo.Count = shopItemInfoById.CUnit;
				}
			}
			if (itemInfo == null && shopItemInfoById == null)
			{
				continue;
			}
			itemInfo.Color = ((text == null) ? "" : text);
			itemInfo.Skin = ((text2 == null) ? "" : text2);
			itemInfo.IsBinds = isBinds || Convert.ToBoolean(shopItemInfoById.IsBind);
			ShopMgr.SetItemType(shopItemInfoById, num2, ref specialValue);
			if ((specialValue.Money <= 0 || !isBinds) && (specialValue.DDTMoney <= 0 || isBinds) && (specialValue.Offer <= 0 || ShopCondition.isOffer(shopItemInfoById.ShopID)) && (specialValue.DamageScore <= 0 || ShopCondition.isWorldBoss(shopItemInfoById.ShopID)) && (specialValue.PetScore <= 0 || ShopCondition.isPetScrore(shopItemInfoById.ShopID)) && (specialValue.HardCurrency <= 0 || ShopCondition.isLabyrinth(shopItemInfoById.ShopID)) && (specialValue.LeagueMoney <= 0 || ShopCondition.isLeague(shopItemInfoById.ShopID)) && (specialValue.iCount <= 0 || ShopCondition.isSearchGoods(shopItemInfoById.ShopID)) && (specialValue.SummerScore <= 0 || ShopCondition.isCatchInsect(shopItemInfoById.ShopID)) && (specialValue.FishScore <= 0 || ShopCondition.isFishScoreShop(shopItemInfoById.ShopID)) && (specialValue.ElfCoin <= 0 || ShopCondition.isElfShop(shopItemInfoById.ShopID)))
			{
				if (!dictionary.ContainsKey(itemInfo.TemplateID))
				{
					dictionary.Add(itemInfo.TemplateID, itemInfo);
				}
				else
				{
					dictionary[itemInfo.TemplateID].Count += itemInfo.Count;
				}
			}
		}
		int num3 = packet.ReadInt();
		int num4 = specialValue.PayValue();
		bool flag = false;
		if (dictionary.Count == 0)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.NotSell"));
			return 1;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		if (specialValue.iTemplateID > 0 && num4 == 0)
		{
			int itemCount = client.Player.GetItemCount(specialValue.iTemplateID);
			if (itemCount <= 0 || itemCount < specialValue.iCount)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("操作失败"));
			}
			else
			{
				flag = client.Player.RemoveTemplate(specialValue.iTemplateID, specialValue.iCount);
			}
		}
		else if (num4 > 0)
		{
			if (specialValue.Money > client.Player.PlayerCharacter.Money)
			{
				translateId = "UserBuyItemHandler.NoMoney";
			}
			if (specialValue.Gold > client.Player.PlayerCharacter.Gold)
			{
				translateId = "UserBuyItemHandler.NoGold";
			}
			if (specialValue.Offer > client.Player.PlayerCharacter.Offer)
			{
				translateId = "UserBuyItemHandler.NoOffer";
			}
			if (specialValue.DDTMoney > client.Player.PlayerCharacter.DDTMoney)
			{
				translateId = "UserBuyItemHandler.BandMoney";
			}
			if (specialValue.Medal > client.Player.PlayerCharacter.medal)
			{
				translateId = "UserBuyItemHandler.BandMoney";
			}
			if (specialValue.DamageScore > client.Player.PlayerCharacter.damageScores)
			{
				translateId = "UserBuyItemHandler.Fail";
			}
			if (specialValue.PetScore > client.Player.PlayerCharacter.petScore)
			{
				translateId = "UserBuyItemHandler.Fail";
			}
			if (specialValue.HardCurrency > client.Player.PlayerCharacter.hardCurrency)
			{
				translateId = "UserBuyItemHandler.Fail";
			}
			if (specialValue.LeagueMoney > client.Player.PlayerCharacter.LeagueMoney)
			{
				translateId = "UserBuyItemHandler.Fail";
			}
			if (specialValue.SummerScore > client.Player.Extra.Info.SummerScore)
			{
				translateId = "UserBuyItemHandler.Fail";
			}
			if (specialValue.FishScore > client.Player.Home?.Data?.fishScore)
			{
				translateId = "UserBuyItemHandler.Fail";
			}
			if (specialValue.ElfCoin > client.Player.Extra.Info.Coin)
			{
				translateId = "UserBuyItemHandler.NoElfCoin";
			}
			type = eMessageType.ERROR;
			client.Player.RemoveGold(specialValue.Gold);
			client.Player.RemoveMoney((int)specialValue.Money);
			client.Player.RemoveOffer(specialValue.Offer);
			client.Player.RemoveGiftToken(specialValue.DDTMoney);
			client.Player.RemoveDamageScores(specialValue.DamageScore);
			client.Player.RemovePetScore(specialValue.PetScore);
			client.Player.RemoveHardCurrency(specialValue.HardCurrency);
			client.Player.RemoveLeagueMoney(specialValue.LeagueMoney);
			client.Player.RemoveCoin(specialValue.ElfCoin);
			client.Player.RemoveSummerScore(specialValue.SummerScore);
			client.Player.RemoveFishScore(specialValue.FishScore);
			flag = true;
		}
		if (flag)
		{
			string text3 = "";
			foreach (ItemInfo value in dictionary.Values)
			{
				value.IsBinds = true;
				text3 += ((text3 == "") ? value.TemplateID.ToString() : ("," + value.TemplateID));
				int num5 = num3;
				int num6 = num5;
				if ((uint)(num6 - 1) <= 1u)
				{
					if (!AddItemsToStoreBag(client, value))
					{
						client.Player.AddTemplate(value);
					}
					continue;
				}
				if (value.Template.MaxCount == 1)
				{
					for (int j = 0; j < value.Count; j++)
					{
						ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(value.Template, value);
						itemInfo2.Count = 1;
						client.Player.AddTemplate(itemInfo2);
					}
					continue;
				}
				int num7 = 0;
				for (int k = 0; k < value.Count; k++)
				{
					if (num7 == value.Template.MaxCount)
					{
						ItemInfo itemInfo3 = ItemInfo.CloneFromTemplate(value.Template, value);
						itemInfo3.Count = num7;
						client.Player.AddTemplate(itemInfo3);
						num7 = 0;
					}
					num7++;
				}
				if (num7 > 0)
				{
					ItemInfo itemInfo4 = ItemInfo.CloneFromTemplate(value.Template, value);
					itemInfo4.Count = num7;
					client.Player.AddTemplate(itemInfo4);
				}
			}
			client.Player.OnPaid((int)specialValue.Money, specialValue.Gold, specialValue.Offer, specialValue.DDTMoney, specialValue.Medal, stringBuilder.ToString());
		}
		else
		{
			type = eMessageType.ERROR;
			translateId = "UserBuyItemHandler.Fail";
		}
		client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId));
		GSPacketIn gSPacketIn = new GSPacketIn(44, client.Player.PlayerCharacter.ID);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(3);
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}

	private bool MustActiveMoney(int templateID)
	{
		return templateID == 201192;
	}

	private bool AddItemsToStoreBag(GameClient client, ItemInfo item)
	{
		int num = 2;
		if (item.TemplateID == 11018 || item.TemplateID == 11025)
		{
			num = 0;
		}
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(num);
		if (itemAt != null && itemAt.Count < itemAt.Template.MaxCount && itemAt.CanStackedTo(item))
		{
			return client.Player.StoreBag.AddTemplateAt(item, item.Count, num);
		}
		return (itemAt == null) ? client.Player.StoreBag.AddItemTo(item, num) : client.Player.AddTemplate(item, (eBageType)item.GetBagType, item.Count, eGameView.RouletteTypeGet);
	}
}

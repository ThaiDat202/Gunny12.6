using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(78, "物品熔化")]
public class ItemFusionHandler : IPacketHandler
{
	public static Random random = new Random();

	public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		int fusionId = packet.ReadInt();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool isBinds = true;
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		int validDate = 30;
		FusionInfo fusionInfo = FusionMgr.FindItemFusion(fusionId);
		if (fusionInfo == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 1;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(fusionInfo.Item1);
		ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(fusionInfo.Reward);
		if (itemTemplateInfo == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 1;
		}
		if (itemTemplateInfo2 == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 1;
		}
		ItemInfo itemByTemplateID = client.Player.EquipBag.GetItemByTemplateID(itemTemplateInfo.TemplateID);
		int attackCompose = 0;
		int defendCompose = 0;
		int luckCompose = 0;
		int agilityCompose = 0;
		int strengthenLevel = 0;
		int magicExp = 0;
		int magicLevel = 0;
		int curExp = 0;
		int enchantSpiritLevel = 0;
		int goldValidDate = 0;
		bool isGold = false;
		DateTime goldBeginTime = DateTime.Now;
		if (itemTemplateInfo.CategoryID == 7 || itemTemplateInfo.CategoryID == 17 || itemTemplateInfo.CategoryID == 18 || itemTemplateInfo.CategoryID == 64 || itemTemplateInfo.CategoryID == 70)
		{
			attackCompose = itemByTemplateID.AttackCompose;
			defendCompose = itemByTemplateID.DefendCompose;
			luckCompose = itemByTemplateID.LuckCompose;
			agilityCompose = itemByTemplateID.AgilityCompose;
			strengthenLevel = itemByTemplateID.StrengthenLevel;
			magicExp = itemByTemplateID.MagicExp;
			magicLevel = itemByTemplateID.MagicLevel;
			curExp = itemByTemplateID.curExp;
			goldBeginTime = itemByTemplateID.goldBeginTime;
			goldValidDate = itemByTemplateID.goldValidDate;
			isGold = itemByTemplateID.IsGold;
			enchantSpiritLevel = itemByTemplateID.enchantSpiritLevel;
		}
		else if (itemTemplateInfo.CategoryID == 8 || itemTemplateInfo.CategoryID == 9)
		{
			attackCompose = itemByTemplateID.AttackCompose;
			defendCompose = itemByTemplateID.DefendCompose;
			luckCompose = itemByTemplateID.LuckCompose;
			agilityCompose = itemByTemplateID.AgilityCompose;
			strengthenLevel = itemByTemplateID.StrengthenLevel;
			magicExp = itemByTemplateID.MagicExp;
			magicLevel = itemByTemplateID.MagicLevel;
			goldBeginTime = itemByTemplateID.goldBeginTime;
			goldValidDate = itemByTemplateID.goldValidDate;
			isGold = itemByTemplateID.IsGold;
			enchantSpiritLevel = itemByTemplateID.enchantSpiritLevel;
		}
		PlayerInventory equipBag = client.Player.EquipBag;
		if (itemTemplateInfo2.BagType == eBageType.PropBag)
		{
			equipBag = client.Player.PropBag;
			validDate = 0;
		}
		int num3 = client.Player.EquipBag.FindFirstEmptySlot();
		int num4 = client.Player.PropBag.FindFirstEmptySlot();
		if ((num3 == -1 && itemTemplateInfo2.BagType == eBageType.EquipBag) || (num4 == -1 && itemTemplateInfo2.BagType == eBageType.PropBag))
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
			return 0;
		}
		int itemCount = client.Player.GetItemCount(fusionInfo.Item1);
		int num5 = itemCount / fusionInfo.Count1;
		if (num > num5)
		{
			num = num5;
		}
		int num6 = fusionInfo.Count1 * num;
		int count = fusionInfo.Count2 * num;
		int count2 = fusionInfo.Count3 * num;
		int count3 = fusionInfo.Count4 * num;
		bool flag2 = true;
		if (num > 1)
		{
			if (itemCount < num6)
			{
				flag2 = false;
			}
		}
		else if (itemCount < fusionInfo.Count1)
		{
			flag2 = false;
		}
		if (!flag2)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 0;
		}
		num2 = GameProperties.PRICE_FUSION_GOLD * fusionInfo.Count1 * num;
		if (client.Player.PlayerCharacter.Gold < num2)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney"));
			return 0;
		}
		int needPower = fusionInfo.NeedPower;
		if (fusionInfo.FusionType == 101 && client.Player.Home.Data != null && client.Player.Home.Data.energyCount < needPower)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoEnergy"));
			return 0;
		}
		List<ItemInfo> list = new List<ItemInfo>();
		client.Player.RemoveGold(num2);
		if (needPower != 0)
		{
			client.Player.RemoveHomeBarEnergy(needPower);
		}
		client.Player.RemoveTemplate(fusionInfo.Item1, num6);
		client.Player.RemoveTemplate(fusionInfo.Item2, count);
		client.Player.RemoveTemplate(fusionInfo.Item3, count2);
		client.Player.RemoveTemplate(fusionInfo.Item4, count3);
		stringBuilder.Append(itemTemplateInfo2.TemplateID + ",");
		ItemInfo itemInfo = null;
		for (int i = 0; i < num; i++)
		{
			if (random.Next(100000) <= fusionInfo.FusionRate)
			{
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo2, 1, 105);
				itemInfo2.IsBinds = isBinds;
				itemInfo2.ValidDate = validDate;
				if (itemTemplateInfo.CategoryID == 7 || itemTemplateInfo.CategoryID == 17 || itemTemplateInfo.CategoryID == 18 || itemTemplateInfo.CategoryID == 64 || itemTemplateInfo.CategoryID == 70)
				{
					itemInfo2.AttackCompose = attackCompose;
					itemInfo2.DefendCompose = defendCompose;
					itemInfo2.LuckCompose = luckCompose;
					itemInfo2.AgilityCompose = agilityCompose;
					itemInfo2.StrengthenLevel = strengthenLevel;
					itemInfo2.MagicExp = magicExp;
					itemInfo2.MagicLevel = magicLevel;
					itemInfo2.curExp = curExp;
					itemInfo2.goldBeginTime = goldBeginTime;
					itemInfo2.goldValidDate = goldValidDate;
					itemInfo2.IsGold = isGold;
					itemInfo2.enchantSpiritLevel = enchantSpiritLevel;
				}
				else if (itemTemplateInfo.CategoryID == 8 || itemTemplateInfo.CategoryID == 9)
				{
					itemInfo2.AttackCompose = attackCompose;
					itemInfo2.DefendCompose = defendCompose;
					itemInfo2.LuckCompose = luckCompose;
					itemInfo2.AgilityCompose = agilityCompose;
					itemInfo2.StrengthenLevel = strengthenLevel;
					itemInfo2.MagicExp = magicExp;
					itemInfo2.MagicLevel = magicLevel;
					itemInfo2.goldBeginTime = goldBeginTime;
					itemInfo2.goldValidDate = goldValidDate;
					itemInfo2.IsGold = isGold;
					itemInfo2.enchantSpiritLevel = enchantSpiritLevel;
				}
				if (itemTemplateInfo2.MaxCount == 1)
				{
					list.Add(itemInfo2);
				}
				else if (itemInfo == null)
				{
					itemInfo = itemInfo2;
				}
			}
		}
		if (itemInfo != null)
		{
			itemInfo.Count = num;
			list.Add(itemInfo);
		}
		if (list.Count > 0)
		{
			client.Out.SendFusionResult(client.Player, result: true);
			client.Player.OnItemFusion(fusionInfo.FusionType);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (ItemInfo item in list)
			{
				client.Player.AddTemplate(item);
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1", itemTemplateInfo2.Name + " x" + num));
		}
		else
		{
			client.Out.SendFusionResult(client.Player, result: false);
			stringBuilder.Append("false");
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed"));
		}
		return 0;
	}
}

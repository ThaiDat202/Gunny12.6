using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.MagicStone.Handle;

[MagicStoneHandleAttbute(5)]
public class ConverScore : IMagicStoneCommandHadler
{
	public static Random random = new Random();

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int iD = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		int num = packet.ReadInt();
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		eMessageType type = eMessageType.Normal;
		string translateId = "UserBuyItemHandler.Success";
		ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
		bool flag2 = false;
		if (shopItemInfoById == null)
		{
			return false;
		}
		if (shopItemInfoById != null && ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 101)
		{
			flag2 = true;
		}
		if (!flag2)
		{
			Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
			return false;
		}
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		ItemTemplateInfo info = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
		ItemInfo itemInfo = Player.MagicStoneBag.CreateMagicstone(info, 1);
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
		if (dictionary.Values.Count == 0)
		{
			return false;
		}
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		bool flag3 = false;
		if (Player.Extra.Info.ScoreMagicstone < specialValue.MagicstoneScore)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("ConverScore.Msg1"));
			return false;
		}
		Player.RemoveScoreMagicstone(specialValue.MagicstoneScore);
		if (true)
		{
			string text = "";
			foreach (ItemInfo value in dictionary.Values)
			{
				text += ((text == "") ? value.TemplateID.ToString() : ("," + value.TemplateID));
				for (int i = 0; i < itemInfo.Count; i++)
				{
					ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(value.Template, value);
					itemInfo2.Count = 1;
					Player.AddTemplate(itemInfo2);
				}
			}
		}
		else
		{
			type = eMessageType.ERROR;
			translateId = "UserBuyItemHandler.FailByPermission";
		}
		Player.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId));
		GSPacketIn gSPacketIn = new GSPacketIn(258, Player.PlayerId);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(Player.Extra.Info.ScoreMagicstone);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}

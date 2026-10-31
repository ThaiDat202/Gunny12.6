using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(156, "客户端日记")]
public class BuyTransnationalGoodsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int iD = packet.ReadInt();
		PyramidInfo pyramid = client.Player.Actives.Pyramid;
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		eMessageType type = eMessageType.Normal;
		string translateId = "UserBuyItemHandler.Success";
		ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
		if (shopItemInfoById == null)
		{
			return 0;
		}
		bool flag = false;
		if (ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 98)
		{
			flag = true;
		}
		if (!flag)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
			return 1;
		}
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
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
			return 1;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		bool flag2 = false;
		if (pyramid.totalPoint < specialValue.DamageScore)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("BuyTransnationalGoodsHandler.Msg1"));
			return 0;
		}
		pyramid.totalPoint -= specialValue.DamageScore;
		if (true)
		{
			string text = "";
			foreach (ItemInfo value in dictionary.Values)
			{
				text += ((text == "") ? value.TemplateID.ToString() : ("," + value.TemplateID));
				if (value.Template.MaxCount == 1)
				{
					for (int i = 0; i < value.Count; i++)
					{
						ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(value.Template, value);
						itemInfo2.Count = 1;
						client.Player.AddTemplate(itemInfo2);
					}
					continue;
				}
				int num = 0;
				for (int j = 0; j < value.Count; j++)
				{
					if (num == value.Template.MaxCount)
					{
						ItemInfo itemInfo3 = ItemInfo.CloneFromTemplate(value.Template, value);
						itemInfo3.Count = num;
						client.Player.AddTemplate(itemInfo3);
						num = 0;
					}
					num++;
				}
				if (num > 0)
				{
					ItemInfo itemInfo4 = ItemInfo.CloneFromTemplate(value.Template, value);
					itemInfo4.Count = num;
					client.Player.AddTemplate(itemInfo4);
					num = 0;
				}
			}
		}
		else
		{
			type = eMessageType.ERROR;
			translateId = "UserBuyItemHandler.FailByPermission";
		}
		client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId));
		GSPacketIn gSPacketIn = new GSPacketIn(145, client.Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
		gSPacketIn.WriteInt(pyramid.totalPoint);
		gSPacketIn.WriteInt(pyramid.turnPoint);
		gSPacketIn.WriteInt(pyramid.pointRatio);
		gSPacketIn.WriteInt(pyramid.currentLayer);
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}
}

using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(183, "卡片使用")]
public class CardUseHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		for (int i = 0; i < num3; i++)
		{
			int templatID = packet.ReadInt();
			ShopItemInfo shopItemInfo = ShopMgr.FindShopbyTemplateID(templatID);
			if (shopItemInfo != null)
			{
				list.Add(shopItemInfo);
			}
		}
		int num4 = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		int num5 = packet.ReadInt();
		ItemInfo itemInfo = null;
		List<ItemInfo> list2 = new List<ItemInfo>();
		if (!flag && client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(1.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.ManyAction"));
			return 0;
		}
		if (num == -1 && num2 == -1)
		{
			foreach (ShopItemInfo item in list)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item.TemplateID);
				itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				int aValue = item.AValue1;
				itemInfo.ValidDate = item.AUnit;
				if (client.Player.MoneyDirect(aValue))
				{
					list2.Add(itemInfo);
				}
			}
		}
		else
		{
			PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
			itemInfo = inventory.GetItemAt(num2);
			if (itemInfo != null)
			{
				list2.Add(itemInfo);
			}
		}
		if (list2.Count > 0)
		{
			string translateId = "CardUseHandler.Success";
			string text = string.Empty;
			foreach (ItemInfo item2 in list2)
			{
				if (item2.Template.Property1 == 13 || item2.Template.Property1 == 11 || item2.Template.Property1 == 12 || item2.Template.Property1 == 26)
				{
					AbstractBuffer abstractBuffer = BufferList.CreateBuffer(item2.Template, (item2.ValidDate == 0) ? 1 : item2.ValidDate);
					if (abstractBuffer != null)
					{
						abstractBuffer.Start(client.Player);
						if (num2 != -1 && num != -1)
						{
							PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
							inventory.RemoveCountFromStack(item2, 1);
						}
					}
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
				}
				else if (item2.Template.Property5 == 3)
				{
					if (item2.ValidDate != 30)
					{
						item2.ValidDate = item2.Template.Property5 * 10;
					}
					AbstractBuffer abstractBuffer2 = BufferList.CreateBufferMinutes(item2.Template, item2.ValidDate);
					if (abstractBuffer2 != null)
					{
						abstractBuffer2.Start(client.Player);
						if (num2 != -1 && num != -1)
						{
							PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
							inventory.RemoveCountFromStack(item2, 1);
						}
					}
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
				}
				else if (item2.Template.Property1 == 120)
				{
					if (client.Player.PlayerCharacter.GourdExpInfo.TemplateID == 0)
					{
						client.Player.PlayerCharacter.GourdExpInfo.Stage = 1;
						client.Player.PlayerCharacter.GourdExpInfo.IsFrist++;
						client.Player.PlayerCharacter.GourdExpInfo.TemplateID = item2.Template.TemplateID;
						if (num2 != -1 && num != -1)
						{
							PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
							inventory.RemoveItem(item2);
						}
						client.Player.Out.SendGourdExpStorage(client.Player);
					}
				}
				else if (item2.IsValidItem() && itemInfo != null)
				{
					if (item2.Template.Property1 == 21)
					{
						if (item2.TemplateID == 1120412)
						{
							int value = item2.Template.Property2 * itemInfo.Count;
							client.Player.AddExpVips(value);
							if (client.Player.PlayerCharacter.VIPLevel != 15)
							{
								text = "VIP经验增加成功！";
							}
						}
						else
						{
							int num6 = item2.Template.Property2 * itemInfo.Count;
							if (client.Player.PlayerCharacter.GourdExpInfo.TemplateID != 0)
							{
								GourdExpInfoHandler.AddGourdExp(client.Player, num6);
							}
							else
							{
								client.Player.AddGP(num6);
								if (!client.Player.MaxLevel())
								{
									text = "GPDanUser.Success";
								}
							}
						}
						if (item2.Template.CanDelete)
						{
							client.Player.RemoveAt((eBageType)num, num2);
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(text, itemInfo.Template.Property2 * itemInfo.Count));
					}
				}
				else
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail"));
				}
			}
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail"));
		}
		client.Player.LastOpenCard = DateTime.Now;
		return 0;
	}
}

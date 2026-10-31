using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(308, "首饰培养")]
public class EquipBringUpHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(0);
		if (itemInfo == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("EquipBringUpHandler.ItemNotFound"));
			return 0;
		}
		ItemTemplateInfo itemTemplateInfo = itemInfo.Template;
		if (itemTemplateInfo == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("EquipBringUpHandler.DoNotSuport"));
			return 0;
		}
		bool flag = false;
		string translation = LanguageMgr.GetTranslation("EquipBringUpHandler.Update.Fail");
		string text = string.Empty;
		int num2 = 0;
		int num3 = -1;
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				eBageType bagType = (eBageType)packet.ReadInt();
				num3 = packet.ReadInt();
				packet.ReadInt();
				ItemInfo itemAt = client.Player.GetItemAt(bagType, num3);
				if (itemAt != null && itemAt.IsBring() && !itemAt.cellLocked)
				{
					num2 += CalculateExperience(itemInfo.Template, itemAt);
					client.Player.EquipBag.RemoveItemAt(num3);
				}
			}
			if (num2 != 0)
			{
				flag = true;
			}
			else
			{
				itemInfo = client.Player.StoreBag.GetItemAt(0);
			}
		}
		else
		{
			packet.ReadInt();
			num2 = packet.ReadInt();
			packet.ReadInt();
			int value = GameProperties.ItemDevelopPrice * num2;
			if (client.Player.MoneyDirect(value))
			{
				flag = true;
			}
		}
		if (flag)
		{
			itemInfo.IsBinds = true;
			int num4 = num2 + itemInfo.curExp;
			for (int j = itemInfo.curExp; j <= num4; j += itemTemplateInfo?.Property2 ?? 0)
			{
				itemTemplateInfo = ItemMgr.FindItemTemplate(itemTemplateInfo.FusionType);
				if (itemTemplateInfo != null && num4 >= itemTemplateInfo.Property2)
				{
					translation = LanguageMgr.GetTranslation("EquipBringUpHandler.Update.Success2", itemInfo.Template.Name, itemTemplateInfo.Name);
					ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
					client.Player.StoreBag.RemoveItemAt(0);
					client.Player.StoreBag.AddItemTo(itemInfo2, 0);
					itemInfo = itemInfo2;
					if (itemTemplateInfo.Property1 >= 5)
					{
						text = LanguageMgr.GetTranslation("EquipBringUpHandler.congratulation", client.Player.PlayerCharacter.NickName, itemInfo.TemplateID);
					}
				}
				else
				{
					translation = LanguageMgr.GetTranslation("EquipBringUpHandler.Update.Success1", num2);
				}
			}
			itemInfo.curExp = num4;
		}
		client.Player.SendMessage(translation);
		GSPacketIn gSPacketIn = new GSPacketIn(308);
		gSPacketIn.WriteBoolean(flag);
		client.Player.SendTCP(gSPacketIn);
		client.Player.StoreBag.UpdateItem(itemInfo);
		client.Player.StoreBag.SaveNewItemToDatabase();
		if (!string.IsNullOrEmpty(text) && itemInfo.ItemID > 0)
		{
			GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, text, itemInfo.ItemID, itemInfo.TemplateID, client.Player.ZoneId, null);
			GameServer.Instance.LoginServer.SendPacket(packet2);
		}
		return 0;
	}

	private int CalculateExperience(ItemTemplateInfo tagItem, ItemInfo beEatenItem)
	{
		int property = tagItem.Property1;
		int property2 = tagItem.Property3;
		int property3 = beEatenItem.Template.Property1;
		int property4 = beEatenItem.Template.Property2;
		int property5 = beEatenItem.Template.Property3;
		int num = beEatenItem.curExp;
		if (property2 < property5)
		{
			ItemTemplateInfo templeteInfoByLevel = GetTempleteInfoByLevel(property3, tagItem);
			property4 = templeteInfoByLevel.Property2;
			property5 = property2;
			num = property4;
		}
		if (num == 0)
		{
			num = property4;
		}
		return num;
	}

	private ItemTemplateInfo GetTempleteInfoByLevel(int eatenLevel, ItemTemplateInfo tagItemInfo)
	{
		int property = tagItemInfo.Property1;
		if (property > eatenLevel)
		{
			while (property > eatenLevel && tagItemInfo.Property4 != 0)
			{
				tagItemInfo = ItemMgr.FindItemTemplate(tagItemInfo.Property4);
				property = tagItemInfo.Property1;
			}
		}
		else if (property < eatenLevel)
		{
			while (property < eatenLevel && tagItemInfo.FusionType != 0)
			{
				tagItemInfo = ItemMgr.FindItemTemplate(tagItemInfo.FusionType);
				property = tagItemInfo.Property1;
			}
		}
		return tagItemInfo;
	}
}

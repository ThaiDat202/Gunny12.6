using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(106, "铸金系统")]
public class WishBeadEquipHandler : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		int bageType = packet.ReadInt();
		int templateId = packet.ReadInt();
		int slot2 = packet.ReadInt();
		int bageType2 = packet.ReadInt();
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(106, client.Player.PlayerCharacter.ID);
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		PlayerInventory inventory2 = client.Player.GetInventory((eBageType)bageType2);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		ItemInfo itemAt2 = inventory2.GetItemAt(slot2);
		if (itemAt2 == null || itemAt == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("WishBeadEquipHandler.Msg1"));
			gSPacketIn.WriteInt(5);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		if (itemAt2.Count < 1 || itemAt2.TemplateID != num)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("WishBeadEquipHandler.Msg2"));
			gSPacketIn.WriteInt(5);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		if (!CanWishBeat(itemAt2.TemplateID, itemAt.Template.CategoryID))
		{
			gSPacketIn.WriteInt(5);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		double num2 = 20.0;
		GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(templateId);
		itemAt.IsBinds = true;
		if (goldEquipTemplateInfo == null && itemAt.Template.CategoryID == 7)
		{
			gSPacketIn.WriteInt(5);
		}
		else if (itemAt.StrengthenLevel > GameProperties.WishBeadLimitLv && GameProperties.IsWishBeadLimit)
		{
			gSPacketIn.WriteInt(5);
		}
		else if (!itemAt.GoldValidDate())
		{
			if (num2 > (double)random.Next(100))
			{
				itemAt.goldBeginTime = DateTime.Now;
				itemAt.goldValidDate = 7;
				string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation5", client.Player.PlayerCharacter.NickName, itemAt.TemplateID, itemAt.goldValidDate);
				GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemAt.ItemID, itemAt.TemplateID, client.Player.ZoneId, null);
				client.Player.OnItemStrengthen(itemAt.Template.CategoryID, itemAt.goldValidDate);
				GameServer.Instance.LoginServer.SendPacket(packet2);
				if (itemAt.Template.CategoryID == 7)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(goldEquipTemplateInfo.NewTemplateId);
					if (itemTemplateInfo != null)
					{
						itemAt.GoldEquip = itemTemplateInfo;
					}
				}
				client.Player.UpdateItem(itemAt);
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(1);
			}
			inventory2.RemoveCountFromStack(itemAt2, 1);
		}
		else
		{
			gSPacketIn.WriteInt(6);
		}
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}

	private bool CanWishBeat(int beatID, int CategoryID)
	{
		if (beatID == 11560 && CategoryID == 7)
		{
			return true;
		}
		if (beatID == 11561 && CategoryID == 5)
		{
			return true;
		}
		if (beatID == 11562 && CategoryID == 1)
		{
			return true;
		}
		return false;
	}
}

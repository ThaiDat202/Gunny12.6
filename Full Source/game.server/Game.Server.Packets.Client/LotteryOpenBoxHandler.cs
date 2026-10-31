using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(26, "打开物品")]
public class LotteryOpenBoxHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		ProduceBussiness produceBussiness = new ProduceBussiness();
		if (client.Lottery != -1)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LotteryOpenBoxHandler.Msg1"));
			return 1;
		}
		int num = packet.ReadByte();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		List<ItemInfo> list = new List<ItemInfo>();
		PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
		string nickName = client.Player.PlayerCharacter.NickName;
		if (inventory.FindFirstEmptySlot() == -1)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LotteryOpenBoxHandler.Msg2"));
			return 1;
		}
		int num4 = 11456;
		ItemInfo itemByTemplateID = client.Player.GetItemByTemplateID(num4);
		ItemInfo itemByTemplateID2 = client.Player.GetItemByTemplateID(num3);
		if (itemByTemplateID2 == null || itemByTemplateID2.Count < 1)
		{
			Console.WriteLine("eBageType.{0} slot {1} templateID {2}", (eBageType)num, num2, num3);
			return 1;
		}
		if (itemByTemplateID2.Count < 1)
		{
			list = client.Player.CaddyBag.GetItems();
			client.Out.SendTCP(caddygetaward(list, nickName, client.Player.ZoneId));
			return 0;
		}
		List<ItemInfo> list2 = new List<ItemInfo>();
		StringBuilder stringBuilder = new StringBuilder();
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		if (!ItemBoxMgr.CreateItemBox(itemByTemplateID2.TemplateID, list2, ref specialValue))
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("LotteryOpenBoxHandler.Msg3"));
			return 0;
		}
		if (specialValue != null)
		{
			client.Player.DirectAddValue(specialValue);
			client.Player.RemoveTemplate(num3, 1);
			client.Player.OnUsingItem(num3, 1);
		}
		if (list2.Count > 0)
		{
			ItemInfo itemInfo = list2[0];
			int num5 = num3;
			int num6 = num5;
			if (num6 == 112047 || (uint)(num6 - 112100) <= 1u)
			{
				if (itemByTemplateID.Count < 4)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LotteryOpenBoxHandler.Msg4", itemByTemplateID.Template.Name));
					client.Out.SendTCP(caddygetaward(list, nickName, client.Player.ZoneId));
					return 0;
				}
				client.Player.RemoveTemplate(num4, 4);
				client.Player.OnUsingItem(num4, 4);
			}
			inventory.AddItem(itemInfo);
			stringBuilder.Append(itemInfo.Template.Name);
			client.Player.RemoveTemplate(num3, 1);
			client.Player.OnUsingItem(num3, 1);
		}
		else
		{
			GSPacketIn packet2 = new GSPacketIn(28);
			client.Out.SendTCP(packet2);
		}
		list = client.Player.CaddyBag.GetItems();
		client.Out.SendTCP(caddygetaward(list, nickName, client.Player.ZoneId));
		client.Lottery = -1;
		if (!string.IsNullOrEmpty(stringBuilder.ToString()))
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LotteryOpenBoxHandler.Msg5", stringBuilder.ToString()));
		}
		return 1;
	}

	private GSPacketIn caddygetaward(List<ItemInfo> list, string name, int zoneId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(245);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteInt(list.Count);
		foreach (ItemInfo item in list)
		{
			gSPacketIn.WriteString(name);
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteInt(zoneId);
			gSPacketIn.WriteBoolean(val: false);
		}
		return gSPacketIn;
	}
}

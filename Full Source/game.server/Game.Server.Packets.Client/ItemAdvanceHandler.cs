using System;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(138, "物品强化")]
public class ItemAdvanceHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (DateTime.Compare(client.Player.LastItemAdvTime.AddSeconds(1.0), DateTime.Now) > 0)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("GoSlow"));
			return 0;
		}
		client.Player.LastItemAdvTime = DateTime.Now;
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		bool flag2 = packet.ReadBoolean();
		bool flag3 = packet.ReadBoolean();
		GSPacketIn gSPacketIn = new GSPacketIn(138, client.Player.PlayerCharacter.ID);
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
		ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(1);
		if (itemAt == null || itemInfo == null || itemAt.Count <= 0)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemAdvanceHandler.Msg1"));
			return 0;
		}
		int strengthenLevel = itemInfo.StrengthenLevel;
		if (strengthenLevel >= 18)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemAdvanceHandler.Msg2"));
			return 0;
		}
		int count = 1;
		string text = "";
		if (itemInfo.Template.CanStrengthen && itemInfo.Template.CategoryID < 18 && itemInfo.Count == 1)
		{
			flag = flag || itemInfo.IsBinds;
			stringBuilder.Append(itemInfo.ItemID + ":" + itemInfo.TemplateID + ",");
			int num = 0;
			if (strengthenLevel < 15)
			{
				if (itemAt.TemplateID != 11150)
				{
					client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemAdvanceHandler.Msg3"));
					return 0;
				}
			}
			else if (itemAt.TemplateID != 70000000)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemAdvanceHandler.Msg6"));
				return 0;
			}
			flag = flag || itemAt.IsBinds;
			text = text + "," + itemAt.ItemID + ":" + itemAt.Template.Name;
			num = ((itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2);
			stringBuilder.Append("true");
			bool flag4 = false;
			int num2 = random.Next(50000);
			double num3 = itemInfo.StrengthenExp / strengthenLevel;
			if (num3 > (double)(strengthenLevel switch
			{
				15 => random.Next(GameProperties.ExaltRatewith15), 
				16 => random.Next(GameProperties.ExaltRatewith16), 
				17 => random.Next(GameProperties.ExaltRatewith17), 
				_ => random.Next(50000), 
			}))
			{
				itemInfo.IsBinds = flag;
				itemInfo.StrengthenLevel++;
				itemInfo.StrengthenExp = 0;
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(num);
				flag4 = true;
				StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(itemInfo.StrengthenLevel, itemInfo.TemplateID);
				if (strengthenGoodsInfo != null && itemInfo.Template.CategoryID == 7 && strengthenGoodsInfo.GainEquip > itemInfo.TemplateID)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(strengthenGoodsInfo.GainEquip);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
						client.Player.StoreBag.RemoveItemAt(1);
						client.Player.StoreBag.AddItemTo(itemInfo2, 1);
						itemInfo = itemInfo2;
					}
				}
				client.Player.OnItemStrengthen(itemInfo.Template.CategoryID, itemInfo.StrengthenLevel);
			}
			else
			{
				itemInfo.StrengthenExp += num;
				gSPacketIn.WriteByte(1);
				gSPacketIn.WriteInt(num);
			}
			client.Player.StoreBag.RemoveCountFromStack(itemAt, count);
			client.Player.StoreBag.UpdateItem(itemInfo);
			client.Out.SendTCP(gSPacketIn);
			if (flag4 && itemInfo.ItemID > 0)
			{
				string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation2", client.Player.PlayerCharacter.NickName, itemInfo.TemplateID, itemInfo.StrengthenLevel - 12);
				GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemInfo.ItemID, itemInfo.TemplateID, client.Player.ZoneId, null);
				GameServer.Instance.LoginServer.SendPacket(packet2);
			}
			stringBuilder.Append(itemInfo.StrengthenLevel);
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1") + itemAt.Template.Name + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2"));
		}
		if (itemInfo.Place < 31)
		{
			client.Player.EquipBag.UpdatePlayerProperties();
		}
		return 0;
	}
}

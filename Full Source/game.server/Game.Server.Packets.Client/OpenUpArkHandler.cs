using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(63, "打开物品")]
public class OpenUpArkHandler : IPacketHandler
{
	public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int bageType = packet.ReadByte();
		int slot = packet.ReadInt();
		int num = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		GSPacketIn gSPacketIn = packet.Clone();
		gSPacketIn.ClearContext();
		if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 11 && itemAt.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel && itemAt.TemplateID != 112019 && itemAt.TemplateID != 112047 && itemAt.TemplateID != 112100 && itemAt.TemplateID != 112101 && itemAt.TemplateID != 190000 && (itemAt.TemplateID < 12201 || itemAt.TemplateID > 12203))
		{
			if (num < 1 || num > itemAt.Count)
			{
				num = itemAt.Count;
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (!inventory.RemoveCountFromStack(itemAt, num))
			{
				return 0;
			}
			Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
			stringBuilder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start"));
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			for (int i = 0; i < num; i++)
			{
				List<ItemInfo> list = new List<ItemInfo>();
				ItemBoxMgr.CreateItemBox(itemAt.TemplateID, list, ref specialValue);
				foreach (ItemInfo item in list)
				{
					if (!dictionary.Keys.Contains(item.TemplateID))
					{
						dictionary.Add(item.TemplateID, item);
					}
					else
					{
						dictionary[item.TemplateID].Count += item.Count;
					}
				}
			}
			client.Player.DirectAddValue(specialValue);
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				string[] array = stringBuilder.ToString().Split(',');
				for (int j = 0; j < array.Length; j++)
				{
					int num2 = 1;
					for (int k = j + 1; k < array.Length; k++)
					{
						if (array[j].Contains(array[k]) && array[k].Length == array[j].Length)
						{
							num2++;
							array[k] = k.ToString();
						}
					}
					if (num2 > 1)
					{
						array[j] = array[j].Remove(array[j].Length - 1, 1);
						array[j] += num2;
					}
					if (array[j] != j.ToString())
					{
						array[j] += ",";
						stringBuilder2.Append(array[j]);
					}
				}
			}
			stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
			stringBuilder2.Append(".");
			gSPacketIn.WriteString(itemAt.Template.Name);
			gSPacketIn.WriteInt(dictionary.Count);
			foreach (ItemInfo value4 in dictionary.Values)
			{
				gSPacketIn.WriteInt(value4.TemplateID);
				gSPacketIn.WriteInt(value4.Count);
				gSPacketIn.WriteBoolean(value4.IsBinds);
				gSPacketIn.WriteInt(value4.ValidDate);
				gSPacketIn.WriteInt(value4.StrengthenLevel);
				gSPacketIn.WriteInt(value4.AttackCompose);
				gSPacketIn.WriteInt(value4.DefendCompose);
				gSPacketIn.WriteInt(value4.AgilityCompose);
				gSPacketIn.WriteInt(value4.LuckCompose);
				gSPacketIn.WriteInt(value4.MagicAttack);
				gSPacketIn.WriteInt(value4.MagicDefence);
				if (value4.Template.MaxCount == 1)
				{
					for (int l = 0; l < value4.Count; l++)
					{
						ItemInfo itemInfo = value4.Clone();
						itemInfo.Count = 1;
						client.Player.AddTemplate(itemInfo, value4.Template.BagType, itemInfo.Count, eGameView.OtherTypeGet, itemAt.Template.Name);
					}
				}
				else
				{
					client.Player.AddTemplate(value4, value4.Template.BagType, value4.Count, eGameView.OtherTypeGet, itemAt.Template.Name);
				}
				if (GameProperties.VERSION >= 8400)
				{
					gSPacketIn.WriteInt(value4.Hole1);
					gSPacketIn.WriteInt(value4.ItemID);
				}
			}
			if (GameProperties.VERSION >= 8400)
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(itemAt.Template.CategoryID);
				gSPacketIn.WriteInt(0);
			}
			if (dictionary.Count > 0)
			{
				client.Player.SendTCP(gSPacketIn);
			}
		}
		if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 72)
		{
			if (!inventory.RemoveCountFromStack(itemAt, num))
			{
				return 0;
			}
			int value = 0;
			int num3 = 0;
			switch (itemAt.TemplateID)
			{
			case 1120657:
				value = 3200;
				num3 = 1;
				break;
			case 1120656:
				value = 17500;
				num3 = 1;
				break;
			case 1120655:
				value = 96000;
				num3 = 1;
				break;
			case 1120637:
				num3 = 2;
				getJamps(client, num, 9, -1, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120638:
				num3 = 2;
				getJamps(client, num, 16, -1, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120639:
				num3 = 2;
				getJamps(client, num, 25, -1, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120640:
				num3 = 2;
				getJamps(client, num, 9, 1001, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120641:
				num3 = 2;
				getJamps(client, num, 9, 1002, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120642:
				num3 = 2;
				getJamps(client, num, 9, 1003, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120643:
				num3 = 2;
				getJamps(client, num, 9, 1004, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120644:
				num3 = 2;
				getJamps(client, num, 9, 1005, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120645:
				num3 = 2;
				getJamps(client, num, 16, 1001, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120646:
				num3 = 2;
				getJamps(client, num, 16, 1002, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120647:
				num3 = 2;
				getJamps(client, num, 16, 1003, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120648:
				num3 = 2;
				getJamps(client, num, 16, 1004, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120649:
				num3 = 2;
				getJamps(client, num, 16, 1005, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120650:
				num3 = 2;
				getJamps(client, num, 25, 1001, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120651:
				num3 = 2;
				getJamps(client, num, 25, 1002, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120652:
				num3 = 2;
				getJamps(client, num, 25, 1003, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120653:
				num3 = 2;
				getJamps(client, num, 25, 1004, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			case 1120654:
				num3 = 2;
				getJamps(client, num, 25, 1005, ref value);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			}
			if (num3 == 2)
			{
				num = 1;
			}
			int value2 = value * num;
			client.Player.AddJampsCurrency(value2);
			int num4 = num3;
			int num5 = num4;
			if ((uint)(num5 - 1) <= 1u)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("获得 " + value2 + "  探险家点数"));
			}
		}
		if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.TemplateID >= 12201 && itemAt.Template.TemplateID <= 12203)
		{
			if (!inventory.RemoveCountFromStack(itemAt, num))
			{
				return 0;
			}
			int value3 = 0;
			switch (itemAt.Template.TemplateID)
			{
			case 12201:
				value3 = 50 * num;
				break;
			case 12202:
				value3 = 200 * num;
				break;
			case 12203:
				value3 = 500 * num;
				break;
			}
			client.Player.SendMessage("获得 " + value3 + " 魂晶");
			client.Player.AddSoulCrystal(value3);
		}
		return 1;
	}

	private void getJamps(GameClient client, int num, int max, int chapter, ref int value)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < num; i++)
		{
			string text = "";
			JampsChapterItemList jampsChapterItemList = ((chapter == -1) ? JampsManualMgr.getRandomChapter() : JampsManualMgr.getChapter(chapter));
			JampsPageItemList randomPageFromChapter = JampsManualMgr.getRandomPageFromChapter(jampsChapterItemList.ID, max);
			PagesInfo pagesInfo = new PagesInfo();
			pagesInfo.activate = false;
			pagesInfo.pageID = randomPageFromChapter.ID;
			if (JampsManualMgr.randNumber(1, 100) <= 60)
			{
				value += 40;
			}
			else if (JampsManualMgr.randNumber(1, 100) <= 30 || client.Player.PlayerCharacter.explorerManualInfo.activesPage.Count == 0)
			{
				if (client.Player.PlayerCharacter.explorerManualInfo.addPage(pagesInfo))
				{
					text = "获得: " + randomPageFromChapter.Name;
					if (!list.Contains(jampsChapterItemList.ID))
					{
						list.Add(jampsChapterItemList.ID);
					}
					client.Player.SendMessage(text);
				}
				else
				{
					value += 60;
				}
			}
			else if (JampsManualMgr.randNumber(1, 100) <= 15)
			{
				JampsDebrisItemList jampsDebrisItemList = ((chapter == -1) ? JampsManualMgr.getRandomDebrisFromPages(client.Player.PlayerCharacter.explorerManualInfo.activesPage) : JampsManualMgr.getRandomDebrisFromPages(client.Player.PlayerCharacter.explorerManualInfo.activesPage, chapter));
				DebrisInfo debrisInfo = new DebrisInfo();
				debrisInfo.date = DateTime.Now;
				debrisInfo.ID = jampsDebrisItemList.ID;
				debrisInfo.pageID = jampsDebrisItemList.PageID;
				debrisInfo.chapterID = JampsManualMgr.getChapterIDFromDebrisID(jampsDebrisItemList.ID);
				if (debrisInfo.chapterID == -1)
				{
					text = "获得 " + jampsDebrisItemList.Describe;
				}
				else if (client.Player.PlayerCharacter.explorerManualInfo.addDebris(debrisInfo))
				{
					text = "获得 " + jampsDebrisItemList.Describe;
					if (!list.Contains(debrisInfo.chapterID))
					{
						list.Add(debrisInfo.chapterID);
					}
				}
			}
			else
			{
				value += 25;
			}
			if (text != "")
			{
				client.Player.SendMessage(text);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(63);
		gSPacketIn.WriteString("Manual do Explorador");
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteInt(72);
		gSPacketIn.WriteInt(list.Count);
		foreach (int item in list)
		{
			gSPacketIn.WriteInt(item);
		}
		client.SendTCP(gSPacketIn);
	}
}

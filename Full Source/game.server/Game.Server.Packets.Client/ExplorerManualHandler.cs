using System;
using System.Collections.Generic;
using System.Linq;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(377, "ExplorerManualHandler")]
public class ExplorerManualHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		switch (b)
		{
		case 1:
		{
			GSPacketIn gSPacketIn2 = new GSPacketIn(377);
			gSPacketIn2.WriteByte(1);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.manualLevel);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.progress);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.maxProgress);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.activesPage.Count());
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.conditionCount);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Agility);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Armor);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Attack);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Damage);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Defense);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.HP);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Lucky);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.MagicAttack);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.MagicResistance);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Stamina);
			List<PagesInfo> list = client.Player.PlayerCharacter.explorerManualInfo.activesPage.Where((PagesInfo o) => o.activate).ToList();
			gSPacketIn2.WriteInt(list.Count);
			foreach (PagesInfo item in list)
			{
				gSPacketIn2.WriteInt(item.pageID);
			}
			client.SendTCP(gSPacketIn2);
			break;
		}
		case 2:
		{
			GSPacketIn gSPacketIn3 = new GSPacketIn(377);
			gSPacketIn3.WriteByte(2);
			int capitulo = packet.ReadInt();
			DebrisInfo[] array = client.Player.PlayerCharacter.explorerManualInfo.debris.Values.Where((DebrisInfo o) => o.chapterID == capitulo).ToArray();
			int val2 = array.Length;
			gSPacketIn3.WriteInt(val2);
			DebrisInfo[] array2 = array;
			foreach (DebrisInfo debrisInfo in array2)
			{
				gSPacketIn3.WriteInt(debrisInfo.ID);
				gSPacketIn3.WriteDateTime(debrisInfo.date);
			}
			client.SendTCP(gSPacketIn3);
			break;
		}
		case 3:
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			bool flag4 = packet.ReadBoolean();
			GSPacketIn gSPacketIn4 = new GSPacketIn(377);
			gSPacketIn4.WriteByte(3);
			JampsPageItemList pageFromID = JampsManualMgr.getPageFromID(num2);
			if (num == 2)
			{
				gSPacketIn4.WriteBoolean(client.Player.PlayerCharacter.Money >= pageFromID.ActivateCurrency);
				gSPacketIn4.WriteInt(num);
				if (client.Player.PlayerCharacter.Money < pageFromID.ActivateCurrency)
				{
					break;
				}
				client.Player.RemoveMoney(pageFromID.ActivateCurrency);
			}
			else if (num == 1)
			{
				gSPacketIn4.WriteBoolean(client.Player.PlayerCharacter.explorerManualInfo.acivatePage(num2));
				gSPacketIn4.WriteInt(num);
			}
			client.Player.EquipBag.UpdatePlayerProperties();
			client.SendTCP(gSPacketIn4);
			break;
		}
		case 4:
		{
			bool flag = packet.ReadBoolean();
			bool flag2 = packet.ReadBoolean();
			bool flag3 = packet.ReadBoolean();
			int maxLevel = JampsManualMgr.maxLevel;
			JampsUpgradeItemList[] condictionsByLevelAndType = JampsManualMgr.getCondictionsByLevelAndType(client.Player.PlayerCharacter.explorerManualInfo.manualLevel, 1);
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, condictionsByLevelAndType[0].Parameter1);
			while (itemByTemplateID?.Count >= condictionsByLevelAndType[0].Parameter2)
			{
				client.Player.PropBag.BeginChanges();
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, condictionsByLevelAndType[0].Parameter2);
				client.Player.PropBag.CommitChanges();
				client.Player.PlayerCharacter.explorerManualInfo.progress++;
				if (client.Player.PlayerCharacter.explorerManualInfo.progress == client.Player.PlayerCharacter.explorerManualInfo.manualLevel)
				{
					client.Player.PlayerCharacter.explorerManualInfo.progress = 0;
					client.Player.PlayerCharacter.explorerManualInfo.conditionCount = 0;
					client.Player.PlayerCharacter.explorerManualInfo.manualLevel++;
					if (client.Player.PlayerCharacter.explorerManualInfo.manualLevel > maxLevel)
					{
						client.Player.PlayerCharacter.explorerManualInfo.manualLevel = maxLevel;
					}
					break;
				}
				if (!flag3)
				{
					break;
				}
			}
			JampsManualItemList jampsManualItemList = JampsManualMgr._jampsManualItemList[client.Player.PlayerCharacter.explorerManualInfo.manualLevel];
			bool val = client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 6 || client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 11 || client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 16 || client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 21;
			GSPacketIn gSPacketIn = new GSPacketIn(377);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteBoolean(val);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteBoolean(jampsManualItemList.MagicAttack > 0);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.manualLevel);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.progress);
			client.Player.EquipBag.UpdatePlayerProperties();
			client.SendTCP(gSPacketIn);
			break;
		}
		default:
			Console.WriteLine("ExplorerManualHanlder Op:" + b);
			break;
		}
		return 0;
	}
}

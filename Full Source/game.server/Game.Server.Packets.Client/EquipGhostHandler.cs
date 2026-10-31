using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(391, "启灵系统")]
public class EquipGhostHandler : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(1);
		ItemInfo itemAt2 = client.Player.StoreBag.GetItemAt(0);
		ItemInfo itemAt3 = client.Player.StoreBag.GetItemAt(2);
		if (itemAt == null || itemAt3 == null || itemAt3.Template.Property1 != 118)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg1"));
			return 0;
		}
		if (itemAt2 != null && itemAt2.Template.Property1 != 117)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg4"));
			return 0;
		}
		List<SpiritInfo> spirit = SpiritInfoMgr.GetSpirit(itemAt.Template.CategoryID);
		if (spirit.Count <= 0)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg2"));
			return 0;
		}
		UserEquipGhostInfo equip = client.Player.GetGhostEquip(spirit[0].BagType, spirit[0].BagPlace);
		if (equip == null)
		{
			equip = new UserEquipGhostInfo();
			equip.UserID = client.Player.PlayerId;
			equip.BagType = spirit[0].BagType;
			equip.Place = spirit[0].BagPlace;
			equip.Level = 0;
			equip.TotalGhost = 0;
			client.Player.AddEquipGhost(equip);
		}
		SpiritInfo spiritInfo = spirit.SingleOrDefault((SpiritInfo a) => a.Level == equip.Level + 1);
		if (spiritInfo != null)
		{
			double num = ((itemAt2 != null) ? (1.0 + (double)(float)itemAt2.Template.Property2 / 100.0) : 1.0);
			double num2 = 5.0 * Math.Pow(2.0, Math.Pow(2.0, (double)itemAt3.Template.Level - 1.0) + 2.0 - (double)(float)spiritInfo.Level) * num;
			client.Player.StoreBag.RemoveCountFromStack(itemAt3, 1);
			client.Player.StoreBag.RemoveCountFromStack(itemAt2, 1);
			bool val = false;
			if (random.Next(10000) < (int)(num2 * 100.0))
			{
				val = true;
				equip.Level++;
				client.Player.EquipBag.UpdatePlayerProperties();
				client.Out.SendUserSyncEquipGhost(client.Player);
			}
			GSPacketIn gSPacketIn = new GSPacketIn(391);
			gSPacketIn.WriteBoolean(val);
			client.SendTCP(gSPacketIn);
			return 1;
		}
		client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.EquipGhost.Msg3"));
		return 0;
	}
}

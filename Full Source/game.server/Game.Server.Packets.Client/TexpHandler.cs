using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(99, "场景用户离开")]
public class TexpHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int slot = packet.ReadInt();
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(slot);
		TexpInfo texp = client.Player.PlayerCharacter.Texp;
		if (itemAt == null || texp == null || (!itemAt.isTexp() && num <= 4) || (itemAt.Template.CategoryID != 53 && num > 4 && num <= 6) || (!itemAt.isTexpSecondProp() && num > 6))
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("texpSystem.Error"));
			return 0;
		}
		bool flag = false;
		int num4 = client.Player.PlayerCharacter.Grade * 2;
		if (num <= 4)
		{
			if (texp.texpCount <= num4)
			{
				if (num3 + texp.texpCount >= num4)
				{
					num3 = num4 - texp.texpCount;
				}
				switch (num)
				{
				case 0:
					texp.hpTexpExp += itemAt.Template.Property2 * num3;
					break;
				case 1:
					texp.attTexpExp += itemAt.Template.Property2 * num3;
					break;
				case 2:
					texp.defTexpExp += itemAt.Template.Property2 * num3;
					break;
				case 3:
					texp.spdTexpExp += itemAt.Template.Property2 * num3;
					break;
				case 4:
					texp.lukTexpExp += itemAt.Template.Property2 * num3;
					break;
				}
				texp.texpCount += num3;
				texp.texpTaskCount++;
				flag = true;
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("texpSystem.texpCountToplimit"));
			}
		}
		else if (num <= 6)
		{
			if (texp.magicTexpCount <= num4 && num > 4)
			{
				if (num3 + texp.magicTexpCount >= num4)
				{
					num3 = num4 - texp.magicTexpCount;
				}
				switch (num)
				{
				case 5:
					texp.magicAtkTexpExp += itemAt.Template.Property2 * num3;
					break;
				case 6:
					texp.magicDefTexpExp += itemAt.Template.Property2 * num3;
					break;
				}
				texp.magicTexpCount += num3;
				flag = true;
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("texpSystem.magictexpCountToplimit"));
			}
		}
		else if (texp.SecondTexpCount <= num4 && num > 6)
		{
			if (num3 + texp.SecondTexpCount >= num4)
			{
				num3 = num4 - texp.SecondTexpCount;
			}
			switch (num)
			{
			case 7:
				texp.TrainCritTexpExp += itemAt.Template.Property2 * num3;
				break;
			case 8:
				texp.TrainSunderTexpExp += itemAt.Template.Property2 * num3;
				break;
			case 9:
				texp.TrainCritDmgTexpExp += itemAt.Template.Property2 * num3;
				break;
			case 10:
				texp.TrainSpeedTexpExp += itemAt.Template.Property2 * num3;
				break;
			case 11:
				texp.TrainTricKillTexpExp += itemAt.Template.Property2 * num3;
				break;
			case 12:
				texp.TrainDmgTexpExp += itemAt.Template.Property2 * num3;
				break;
			case 13:
				texp.TrainArmorTexpExp += itemAt.Template.Property2 * num3;
				break;
			}
			texp.SecondTexpCount += num3;
			flag = true;
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("texpSystem.SecondTexpCountToplimit"));
		}
		if (flag)
		{
			client.Player.StoreBag.RemoveTemplate(num2, num3);
			client.Player.OnUsingItem(num2, num3);
			texp.texpTaskDate = DateTime.Now;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.UpdateUserTexpInfo(texp);
			}
			client.Player.EquipBag.UpdatePlayerProperties();
		}
		return 0;
	}
}

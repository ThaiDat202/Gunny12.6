using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(237, "场景用户离开")]
public class PlayerDressHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(237, client.Player.PlayerCharacter.ID);
		switch (num)
		{
		case 6:
		{
			PlayerInventory equipBag2 = client.Player.EquipBag;
			List<int> list2 = new List<int>();
			List<ItemInfo> list3 = new List<ItemInfo>();
			int num4 = packet.ReadInt();
			int num5 = 0;
			while (num5 < num4)
			{
				int num6 = -1;
				int num7 = -1;
				if (GameProperties.ShortFoldDress())
				{
					num6 = packet.ReadInt();
					num7 = packet.ReadInt();
				}
				else
				{
					packet.ReadInt();
					num6 = packet.ReadInt();
					packet.ReadInt();
					num7 = packet.ReadInt();
				}
				if (num6 < equipBag2.BeginSlot || num7 < equipBag2.BeginSlot)
				{
					continue;
				}
				ItemInfo itemAt2 = equipBag2.GetItemAt(num6);
				ItemInfo itemAt3 = equipBag2.GetItemAt(num7);
				if (itemAt2 != null && itemAt3 != null && itemAt2.TemplateID == itemAt3.TemplateID)
				{
					if (itemAt2.ValidDate > 0 && itemAt3.IsValidItem())
					{
						if (!itemAt2.IsValidItem())
						{
							itemAt2.BeginDate = DateTime.Now;
						}
						itemAt2.ValidDate += itemAt3.ValidDate;
						itemAt2.ValidDate = ((itemAt2.ValidDate > 365) ? 365 : itemAt2.ValidDate);
					}
					list3.Add(itemAt2);
					list2.Add(itemAt3.Place);
				}
				num5++;
			}
			equipBag2.AllUpdateItem(list3);
			client.Player.EquipBag.RemoveAllItem(list2);
			break;
		}
		case 7:
			gSPacketIn.WriteByte(7);
			client.Player.SendTCP(gSPacketIn);
			break;
		case 2:
		{
			int currentModel = packet.ReadInt();
			client.Player.AvatarBag.SetCurrentModel(currentModel);
			break;
		}
		case 1:
		{
			PlayerInventory equipBag = client.Player.EquipBag;
			List<ItemInfo> list = new List<ItemInfo>();
			int slot = packet.ReadInt();
			int num2 = packet.ReadInt();
			int i = 0;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				for (; i < num2; i++)
				{
					int num3 = packet.ReadInt();
					int slot2 = packet.ReadInt();
					ItemInfo itemAt = equipBag.GetItemAt(slot2);
					if (itemAt != null && itemAt.BagType == num3)
					{
						if (itemAt.ItemID == 0)
						{
							playerBussiness.AddGoods(itemAt);
						}
						itemAt.IsBinds = true;
						list.Add(itemAt);
					}
				}
			}
			client.Player.AvatarBag.SaveDressModel(slot, list);
			client.Player.AvatarBag.UpdateCurrentDressModels();
			break;
		}
		default:
			Console.WriteLine("PlayerDressHandler_cmd:{0} ", (PlayerDressType)num);
			break;
		case 8:
			break;
		}
		return 0;
	}
}

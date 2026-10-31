using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(222, "物品精炼")]
public class EquipRetrieveHandler : IPacketHandler
{
	private Random rnd = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int user = 0;
		PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
		int num = 0;
		bool isBinds = false;
		for (int i = 1; i < 5; i++)
		{
			ItemInfo itemAt = inventory.GetItemAt(i);
			if (itemAt != null)
			{
				inventory.RemoveItemAt(i);
			}
			if (itemAt.IsBinds)
			{
				isBinds = true;
			}
			num += itemAt.Template.Quality;
		}
		switch (num)
		{
		case 8:
		case 12:
		case 15:
		case 20:
			user = num;
			break;
		}
		List<ItemInfo> info = null;
		DropInventory.RetrieveDrop(user, ref info);
		int index = rnd.Next(info.Count);
		int templateID = info[index].TemplateID;
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateID), 1, 105);
		itemInfo.IsBinds = isBinds;
		itemInfo.BeginDate = DateTime.Now;
		if (itemInfo.Template.CategoryID != 11)
		{
			itemInfo.ValidDate = 30;
			itemInfo.IsBinds = true;
		}
		itemInfo.IsBinds = true;
		inventory.AddItemTo(itemInfo, 0);
		return 1;
	}
}

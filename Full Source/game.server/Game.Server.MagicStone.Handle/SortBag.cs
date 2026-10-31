using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.MagicStone.Handle;

[MagicStoneHandleAttbute(9)]
public class SortBag : IMagicStoneCommandHadler
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		PlayerMagicStoneInventory magicStoneBag = Player.MagicStoneBag;
		List<ItemInfo> items = magicStoneBag.GetItems(magicStoneBag.BeginSlot, magicStoneBag.Capalility);
		if (num != items.Count)
		{
			return false;
		}
		magicStoneBag.BeginChanges();
		try
		{
			for (int i = 0; i < num; i++)
			{
				int slot = packet.ReadInt();
				int toSlot = packet.ReadInt();
				ItemInfo itemAt = magicStoneBag.GetItemAt(slot);
				if (itemAt != null)
				{
					magicStoneBag.MoveItem(itemAt.Place, toSlot, itemAt.Count);
				}
			}
		}
		catch (Exception ex)
		{
			log.ErrorFormat("Arrage magicstone bag errror,user id:{0}   msg:{1}", Player.PlayerId, ex.Message);
		}
		finally
		{
			items = magicStoneBag.GetItems(magicStoneBag.BeginSlot, magicStoneBag.Capalility);
			if (magicStoneBag.FindFirstEmptySlot() != -1)
			{
				for (int j = 1; magicStoneBag.FindFirstEmptySlot() < items[items.Count - j].Place; j++)
				{
					magicStoneBag.MoveItem(items[items.Count - j].Place, magicStoneBag.FindFirstEmptySlot(), items[items.Count - j].Count);
				}
			}
			magicStoneBag.CommitChanges();
		}
		return true;
	}
}

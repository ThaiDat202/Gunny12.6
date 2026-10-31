using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerMagicStoneInventory : PlayerInventory
{
	private const int MAGICSTONE_START_BAG = 32;

	public static Random random = new Random();

	public PlayerMagicStoneInventory(GamePlayer player)
		: base(player, saveTodb: true, 144, 41, 32, autoStack: true)
	{
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		try
		{
			base.LoadFromDatabase();
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 0; i < 32; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (m_items[i] != null && !CanEquip(m_items[i].Place))
				{
					int num = FindFirstEmptySlot(32);
					if (num >= 0)
					{
						MoveItem(itemInfo.Place, num, itemInfo.Count);
						continue;
					}
					ItemInfo item = itemInfo.Clone();
					list.Add(item);
					TakeOutItem(itemInfo);
				}
			}
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, "", LanguageMgr.GetTranslation("PlayerMagicStoneInventory.WrongSlot"), eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		finally
		{
			CommitChanges();
		}
	}

	public bool CanEquip(int fromSlot)
	{
		if (m_items[fromSlot] == null)
		{
			return false;
		}
		if (fromSlot < base.BeginSlot - 1)
		{
			return true;
		}
		ItemInfo itemInfo = m_items[fromSlot];
		lock (m_lock)
		{
			for (int i = 0; i < base.BeginSlot - 1; i++)
			{
				if (m_items[i] != null)
				{
					if (m_items[i].SameMagicstone("att") && itemInfo.SameMagicstone("att"))
					{
						return false;
					}
					if (m_items[i].SameMagicstone("def") && itemInfo.SameMagicstone("def"))
					{
						return false;
					}
					if (m_items[i].SameMagicstone("agi") && itemInfo.SameMagicstone("agi"))
					{
						return false;
					}
					if (m_items[i].SameMagicstone("luc") && itemInfo.SameMagicstone("luc"))
					{
						return false;
					}
					if (m_items[i].SameMagicstone("mgatt") && itemInfo.SameMagicstone("mgatt"))
					{
						return false;
					}
					if (m_items[i].SameMagicstone("mgdef") && itemInfo.SameMagicstone("mgdef"))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public override bool MoveItem(int fromSlot, int toSlot, int count)
	{
		if (m_items[fromSlot] == null)
		{
			return false;
		}
		return base.MoveItem(fromSlot, toSlot, count);
	}

	public ItemInfo CreateMagicstone(ItemTemplateInfo info, int level)
	{
		MagicStoneInfo magicStoneInfo = MagicStoneMgr.FindMagicStoneByLevel(info.TemplateID, level);
		if (magicStoneInfo == null)
		{
			return null;
		}
		PropMgInfo[] array = MagicStoneMgr.CreateProp(magicStoneInfo);
		ThreadSafeRandom.ShufferStatic(array);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(info, 1, 102);
		int num = ((array.Length > info.Property3) ? info.Property3 : array.Length);
		for (int i = 0; i < num; i++)
		{
			switch (array[i].key)
			{
			case "att":
				itemInfo.AttackCompose = array[i].value;
				break;
			case "agi":
				itemInfo.AgilityCompose = array[i].value;
				break;
			case "def":
				itemInfo.DefendCompose = array[i].value;
				break;
			case "luc":
				itemInfo.LuckCompose = array[i].value;
				break;
			case "mgatt":
				itemInfo.MagicAttack = array[i].value;
				break;
			case "mgdef":
				itemInfo.MagicDefence = array[i].value;
				break;
			}
		}
		itemInfo.StrengthenLevel = level;
		itemInfo.StrengthenExp = magicStoneInfo.Exp;
		return itemInfo;
	}

	public override void UpdateChangedPlaces()
	{
		int[] array = m_changedPlaces.ToArray();
		bool flag = false;
		int[] array2 = array;
		foreach (int slot in array2)
		{
			if (!IsEquipSlot(slot))
			{
				continue;
			}
			ItemInfo itemAt = GetItemAt(slot);
			if (itemAt != null)
			{
				m_player.OnUsingItem(itemAt.TemplateID, 1);
				itemAt.IsBinds = true;
				if (!itemAt.IsUsed)
				{
					itemAt.IsUsed = true;
					itemAt.BeginDate = DateTime.Now;
				}
			}
			flag = true;
			break;
		}
		base.UpdateChangedPlaces();
		if (flag)
		{
			base.Player.EquipBag.UpdatePlayerProperties();
		}
	}
}

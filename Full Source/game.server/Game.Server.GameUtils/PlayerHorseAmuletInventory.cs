using System;
using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerHorseAmuletInventory : PlayerInventory
{
	public static Random random = new Random();

	private const int HORSEAMULET_START_BAG = 20;

	public PlayerHorseAmuletInventory(GamePlayer player)
		: base(player, saveTodb: true, 167, 42, 20, autoStack: true)
	{
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		try
		{
			base.LoadFromDatabase();
			List<ItemInfo> list = new List<ItemInfo>();
			ItemInfo itemInfo = m_items[19];
			if (m_items[19] != null)
			{
				int num = FindFirstEmptySlot(20);
				if (num >= 0)
				{
					MoveItem(itemInfo.Place, num, itemInfo.Count);
				}
				else
				{
					ItemInfo item = itemInfo.Clone();
					list.Add(item);
					TakeOutItem(itemInfo);
				}
			}
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, "", LanguageMgr.GetTranslation("PlayerHorseAmuletInventory.WrongSlot"), eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		finally
		{
			CommitChanges();
		}
	}

	public bool CanEquipAmulet(int templateId)
	{
		int num = 0;
		ItemInfo itemInfo = null;
		MountTalismansInfo mountTalismansInfo = MountMgr.FindMountTalismans(templateId);
		for (int i = 0; i < 9; i++)
		{
			itemInfo = GetItemAt(i);
			if (itemInfo != null && MountMgr.FindMountTalismans(itemInfo.TemplateID).ExtendType1 == mountTalismansInfo.ExtendType1)
			{
				num++;
				if (num >= 3)
				{
					return false;
				}
			}
		}
		return true;
	}

	public int CanPutInEquipAmulet()
	{
		string[] array = GameProperties.MountAmuletOpenLvl.Split('|');
		ItemInfo itemInfo = null;
		for (int i = 0; i < 9; i++)
		{
			if (int.Parse(array[i].Split(',')[1]) > base.Player.PlayerCharacter.MountLv)
			{
				return -1;
			}
			itemInfo = GetItemAt(i);
			if (itemInfo == null)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetHorseAmuletHp()
	{
		ItemInfo itemInfo = null;
		MountTalismansInfo mountTalismansInfo = null;
		int num = 0;
		for (int i = 0; i < 9; i++)
		{
			itemInfo = GetItemAt(i);
			if (itemInfo != null)
			{
				num += MountMgr.FindMountTalismans(itemInfo.TemplateID)?.BaseType1Value ?? 0;
			}
		}
		return num;
	}

	public int GetHorseAmuletProperty()
	{
		ItemInfo itemInfo = null;
		int num = 0;
		for (int i = 0; i < 9; i++)
		{
			itemInfo = GetItemAt(i);
			if (itemInfo != null)
			{
				num += itemInfo.Hole1;
			}
		}
		return num;
	}

	public void GetHorseAmuletSecondProperty(ref int Crit, ref int Tenacity, ref int SunderArmor, ref int AvoidInjury, ref int Kill, ref int WillFight, ref int ViolenceInjury, ref int Guard, ref int Speed)
	{
		ItemInfo itemInfo = null;
		MountTalismansInfo mountTalismansInfo = null;
		for (int i = 0; i < 9; i++)
		{
			itemInfo = GetItemAt(i);
			if (itemInfo != null)
			{
				mountTalismansInfo = MountMgr.FindMountTalismans(itemInfo.TemplateID);
				switch (mountTalismansInfo.ExtendType1)
				{
				case 1:
					Crit += itemInfo.Hole1;
					break;
				case 2:
					Tenacity += itemInfo.Hole1;
					break;
				case 3:
					SunderArmor += itemInfo.Hole1;
					break;
				case 4:
					AvoidInjury += itemInfo.Hole1;
					break;
				case 5:
					Kill += itemInfo.Hole1;
					break;
				case 6:
					WillFight += itemInfo.Hole1;
					break;
				case 7:
					ViolenceInjury += itemInfo.Hole1;
					break;
				case 8:
					Guard += itemInfo.Hole1;
					break;
				case 9:
					Speed += itemInfo.Hole1;
					break;
				}
			}
		}
	}

	public override bool MoveItem(int fromSlot, int toSlot, int count)
	{
		if (m_items[fromSlot] == null)
		{
			return false;
		}
		if (m_items[toSlot] != null)
		{
			toSlot = FindFirstEmptySlot();
			if (toSlot == -1)
			{
				base.Player.SendMessage(LanguageMgr.GetTranslation("HorseAmuletMove.BagFull"));
				return false;
			}
		}
		return base.MoveItem(fromSlot, toSlot, count);
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

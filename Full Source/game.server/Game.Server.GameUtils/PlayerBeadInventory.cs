using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerBeadInventory : PlayerInventory
{
	private const int BEAD_START = 32;

	private Dictionary<int, UserDrillInfo> m_userDrills;

	public Dictionary<int, UserDrillInfo> UserDrills
	{
		get
		{
			return m_userDrills;
		}
		set
		{
			m_userDrills = value;
		}
	}

	public PlayerBeadInventory(GamePlayer player)
		: base(player, saveTodb: true, 179, 21, 32, autoStack: false)
	{
		m_userDrills = new Dictionary<int, UserDrillInfo>();
	}

	public bool canEquip(int place, int grade, ref int needLv)
	{
		bool result = true;
		switch (place)
		{
		case 6:
			needLv = 15;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		case 7:
			needLv = 18;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		case 8:
			needLv = 21;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		case 9:
			needLv = 24;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		case 10:
			needLv = 27;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		case 11:
			needLv = 30;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		case 12:
			needLv = 33;
			if (grade < needLv)
			{
				result = false;
			}
			return result;
		default:
			return true;
		}
	}

	public int GetDrillLevel(int place)
	{
		for (int i = 0; i < UserDrills.Count; i++)
		{
			if (UserDrills[i].BeadPlace == place)
			{
				return UserDrills[i].HoleLv;
			}
		}
		return 0;
	}

	public bool JudgeLevel(int beadLv, int drillLv)
	{
		switch (drillLv)
		{
		case 1:
			if (beadLv >= 1 && beadLv <= 4)
			{
				return true;
			}
			break;
		case 2:
			if (beadLv >= 1 && beadLv <= 8)
			{
				return true;
			}
			break;
		case 3:
			if (beadLv >= 1 && beadLv <= 12)
			{
				return true;
			}
			break;
		case 4:
			if (beadLv >= 1 && beadLv <= 16)
			{
				return true;
			}
			break;
		case 5:
			if (beadLv >= 1 && beadLv <= 19)
			{
				return true;
			}
			break;
		case 6:
			return true;
		}
		return false;
	}

	public void LoadDrills()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		m_userDrills = playerBussiness.GetPlayerDrillByID(m_player.PlayerCharacter.ID);
		if (m_userDrills.Count != 0)
		{
			return;
		}
		List<int> list = new List<int> { 13, 14, 15, 16, 17, 18 };
		List<int> list2 = new List<int> { 0, 1, 2, 3, 4, 5 };
		for (int i = 0; i < list.Count; i++)
		{
			UserDrillInfo userDrillInfo = new UserDrillInfo
			{
				UserID = m_player.PlayerCharacter.ID,
				BeadPlace = list[i],
				HoleLv = 0,
				HoleExp = 0,
				DrillPlace = list2[i]
			};
			playerBussiness.AddUserUserDrill(userDrillInfo);
			if (!m_userDrills.ContainsKey(userDrillInfo.DrillPlace))
			{
				m_userDrills.Add(userDrillInfo.DrillPlace, userDrillInfo);
			}
		}
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		try
		{
			base.LoadFromDatabase();
			LoadDrills();
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 1; i < 32; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo != null && itemInfo.Place < 13)
				{
					if (m_items[i].Template.Property2 == 1 && m_items[i].Place != 1)
					{
						RemoveItemAt(itemInfo.Place);
					}
					if (m_items[i].Template.Property2 == 2 && (m_items[i].Place == 1 || m_items[i].Place > 3))
					{
						RemoveItemAt(itemInfo.Place);
					}
					if (m_items[i].Template.Property2 == 3 && m_items[i].Place <= 3)
					{
						RemoveItemAt(itemInfo.Place);
					}
				}
				if (m_items[i] == null || m_items[i].Place < 13 || m_items[i].Place > 18)
				{
					continue;
				}
				int drillLevel = GetDrillLevel(m_items[i].Place);
				if (!JudgeLevel(m_items[i].Hole1, drillLevel))
				{
					int num = FindFirstEmptySlot(32);
					if (num >= 0)
					{
						MoveItem(itemInfo.Place, num, itemInfo.Count);
					}
					else
					{
						list.Add(itemInfo);
					}
				}
			}
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, "", "", eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			CheckBugPerolas();
		}
		finally
		{
			CommitChanges();
		}
	}

	private void CheckBugPerolas()
	{
		if (GetItemAt(2) == null || GetItemAt(3) == null)
		{
			return;
		}
		RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(GetItemAt(2).Template.TemplateID, GetItemAt(2).Hole1);
		RuneTemplateInfo runeTemplateInfo2 = RuneMgr.FindRuneByTemplateID(GetItemAt(3).Template.TemplateID, GetItemAt(3).Hole1);
		if (runeTemplateInfo.Type1 == runeTemplateInfo2.Type1 && runeTemplateInfo.Type2 == runeTemplateInfo2.Type2 && runeTemplateInfo.Type3 == runeTemplateInfo2.Type3)
		{
			int num = FindFirstEmptySlot();
			if (num >= 0)
			{
				MoveItem(GetItemAt(3).Place, FindFirstEmptySlot(), GetItemAt(3).Count);
				return;
			}
			m_player.SendItemToMail(GetItemAt(3), "", "", eMailType.ItemOverdue);
			m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			RemoveItemAt(3);
		}
	}

	public override bool MoveItem(int fromSlot, int toSlot, int count)
	{
		bool flag = m_items[fromSlot] != null && base.MoveItem(fromSlot, toSlot, count);
		if (flag)
		{
			CheckBugPerolas();
		}
		return flag;
	}

	public override void SaveToDatabase()
	{
		base.SaveToDatabase();
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		foreach (UserDrillInfo value in m_userDrills.Values)
		{
			playerBussiness.UpdateUserDrillInfo(value);
		}
	}

	public void SendPlayerDrill()
	{
		m_player.Out.SendPlayerDrill(m_player.PlayerCharacter.ID, m_userDrills);
	}

	public override void UpdateChangedPlaces()
	{
		int[] array = m_changedPlaces.ToArray();
		foreach (int slot in array)
		{
			if (IsEquipSlot(slot))
			{
				ItemInfo itemAt = GetItemAt(slot);
				if (itemAt != null)
				{
					itemAt.IsBinds = true;
				}
				break;
			}
		}
		base.UpdateChangedPlaces();
	}

	public void UpdateDrill(int index, UserDrillInfo drill)
	{
		m_userDrills[index] = drill;
	}
}

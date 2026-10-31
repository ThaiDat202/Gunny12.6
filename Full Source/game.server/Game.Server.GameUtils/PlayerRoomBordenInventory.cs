using System.Collections.Generic;
using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerRoomBordenInventory : PlayerInventory
{
	public PlayerRoomBordenInventory(GamePlayer player)
		: base(player, saveTodb: true, 54, 43, 0, autoStack: true)
	{
	}

	public override void LoadFromDatabase()
	{
		base.LoadFromDatabase();
	}

	public override void UpdateChangedPlaces()
	{
		base.UpdateChangedPlaces();
		int currentItemId = 0;
		if (m_player.Extra.Info.curcentRoomBordenTemplateId != 0)
		{
			ItemInfo itemVaildDateByTemplateID = GetItemVaildDateByTemplateID(m_player.Extra.Info.curcentRoomBordenTemplateId);
			if (itemVaildDateByTemplateID == null)
			{
				m_player.Extra.Info.curcentRoomBordenTemplateId = 0;
				currentItemId = 0;
			}
			else
			{
				currentItemId = itemVaildDateByTemplateID.ItemID;
			}
		}
		m_player.Out.SendUpdateRoomBorden(new List<ItemInfo>(), isAdd: true, currentItemId);
	}

	public ItemInfo GetItemVaildDateByTemplateID(int templateId)
	{
		ItemInfo result = null;
		ItemInfo[] array = FindItemsByTempleteID(templateId);
		if (array != null && array.Length != 0)
		{
			ItemInfo[] array2 = array;
			foreach (ItemInfo itemInfo in array2)
			{
				if (itemInfo.IsValidItem() && itemInfo.IsExist)
				{
					result = itemInfo;
					break;
				}
			}
		}
		return result;
	}

	public void ClearVaildItem()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		lock (m_lock)
		{
			for (int i = 0; i < m_items.Length; i++)
			{
				if (m_items[i] != null && !m_items[i].IsValidItem())
				{
					list.Add(m_items[i]);
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (ItemInfo item in list)
		{
			RemoveItem(item);
		}
		m_player.SendMessage(LanguageMgr.GetTranslation("GameServer.Team.RoomBordenBox.VaildDate.Msg1", list.Count));
	}
}

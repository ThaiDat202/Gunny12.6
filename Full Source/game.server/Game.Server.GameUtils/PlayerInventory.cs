using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerInventory : AbstractInventory
{
	public const int EQUIPBAG_START = 31;

	public const int EQUIPBAG_END = 80;

	public const int DRESSBAG_START = 81;

	public const int DRESSBAG_END = 1176;

	protected GamePlayer m_player;

	private bool m_saveToDb;

	private List<ItemInfo> m_removedList = new List<ItemInfo>();

	public GamePlayer Player => m_player;

	public PlayerInventory(GamePlayer player, bool saveTodb, int capibility, int type, int beginSlot, bool autoStack)
		: base(capibility, type, beginSlot, autoStack)
	{
		m_player = player;
		m_saveToDb = saveTodb;
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		ItemInfo[] userBagByType = playerBussiness.GetUserBagByType(m_player.PlayerCharacter.ID, base.BagType);
		List<ItemInfo> list = new List<ItemInfo>();
		BeginChanges();
		try
		{
			ItemInfo[] array = userBagByType;
			foreach (ItemInfo itemInfo in array)
			{
				if (base.BagType == 0 && Equip.isAvatar(itemInfo.Template) && itemInfo.Place > 31 && itemInfo.Place < 80)
				{
					int num = FindFirstEmptySlot(81);
					if (num >= 0)
					{
						MoveItem(itemInfo.Place, num, itemInfo.Count);
					}
					else
					{
						list.Add(itemInfo);
					}
				}
				else if (!AddItem(itemInfo, itemInfo.Place))
				{
					list.Add(itemInfo);
				}
			}
		}
		finally
		{
			CommitChanges();
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, null, LanguageMgr.GetTranslation("PlayerInventory.FullBag.Title"), eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			for (int i = 0; i < m_items.Length; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo != null && itemInfo.IsDirty)
				{
					if (itemInfo.ItemID > 0)
					{
						playerBussiness.UpdateGoods(itemInfo);
					}
					else
					{
						playerBussiness.AddGoods(itemInfo);
					}
				}
			}
		}
		lock (m_removedList)
		{
			foreach (ItemInfo removed in m_removedList)
			{
				if (removed.ItemID > 0)
				{
					playerBussiness.UpdateGoods(removed);
				}
			}
			m_removedList.Clear();
		}
	}

	public virtual void SaveNewItemToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			for (int i = 0; i < m_items.Length; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo != null && itemInfo.IsDirty && itemInfo.ItemID == 0)
				{
					playerBussiness.AddGoods(itemInfo);
				}
			}
		}
	}

	public void ValidItems()
	{
		lock (m_lock)
		{
			for (int i = 0; i < base.Capalility; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo != null && !itemInfo.IsValidItem() && ShopMgr.CanNotBuy(itemInfo.Template))
				{
					RemoveItem(itemInfo);
				}
			}
		}
	}

	public int FindItemEpuipSlot(ItemTemplateInfo item)
	{
		switch (item.CategoryID)
		{
		case 8:
		case 28:
			lock (m_lock)
			{
				if (m_items[7] == null)
				{
					return 7;
				}
			}
			return 8;
		case 9:
		case 29:
			lock (m_lock)
			{
				if (m_items[9] == null)
				{
					return 9;
				}
			}
			return 10;
		case 13:
			return 11;
		case 14:
			return 12;
		case 15:
			return 13;
		case 16:
			return 14;
		case 27:
			return 6;
		case 17:
		case 31:
			return 15;
		case 40:
			return 17;
		case 70:
			return 18;
		case 64:
			return 20;
		default:
			return item.CategoryID - 1;
		}
	}

	public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
	{
		if (temp.CategoryID == 8 || temp.CategoryID == 28)
		{
			return slot == 7 || slot == 8;
		}
		if (temp.CategoryID == 9 || temp.CategoryID == 29)
		{
			if (Equip.isWeddingRing(temp))
			{
				return slot == 9 || slot == 10 || slot == 16;
			}
			return slot == 9 || slot == 10;
		}
		if (temp.CategoryID == 13)
		{
			return slot == 11;
		}
		if (temp.CategoryID == 14)
		{
			return slot == 12;
		}
		if (temp.CategoryID == 15)
		{
			return slot == 13;
		}
		if (temp.CategoryID == 16)
		{
			return slot == 14;
		}
		if (temp.CategoryID == 17 || temp.CategoryID == 31)
		{
			return slot == 15;
		}
		if (temp.CategoryID == 27)
		{
			return slot == 6;
		}
		if (temp.CategoryID == 40)
		{
			return slot == 17;
		}
		if (temp.CategoryID == 70)
		{
			return slot == 18;
		}
		if (temp.CategoryID == 64)
		{
			return slot == 20;
		}
		return temp.CategoryID - 1 == slot;
	}

	public bool IsEquipSlot(int slot)
	{
		return slot >= 0 && slot < base.BeginSlot;
	}

	public override bool AddItemTo(ItemInfo item, int place)
	{
		if (base.AddItemTo(item, place))
		{
			item.UserID = m_player.PlayerCharacter.ID;
			item.IsExist = true;
			return true;
		}
		return false;
	}

	public override bool TakeOutItem(ItemInfo item)
	{
		if (base.TakeOutItem(item))
		{
			if (m_saveToDb)
			{
				lock (m_removedList)
				{
					m_removedList.Add(item);
				}
			}
			return true;
		}
		return false;
	}

	public override bool RemoveItem(ItemInfo item)
	{
		if (base.RemoveItem(item))
		{
			item.IsExist = false;
			if (m_saveToDb)
			{
				lock (m_removedList)
				{
					m_removedList.Add(item);
				}
			}
			return true;
		}
		return false;
	}

	public override void UpdateChangedPlaces()
	{
		int[] array = m_changedPlaces.ToArray();
		int num = 80;
		if (array.Length > num)
		{
			int num2 = array.Length;
			if (array.Length < num)
			{
				num = array.Length;
			}
			for (int i = 0; i < num2; i += num)
			{
				int[] array2 = new int[num];
				if (num2 < i + num)
				{
					num = num2 - i;
				}
				Array.Copy(array, i, array2, 0, num);
				m_player.Out.SendUpdateInventorySlot(this, array2);
			}
		}
		else
		{
			m_player.Out.SendUpdateInventorySlot(this, array);
		}
		base.UpdateChangedPlaces();
	}

	public bool SendAllItemsToMail(string sender, string title, eMailType type)
	{
		if (m_saveToDb)
		{
			BeginChanges();
			try
			{
				using (new PlayerBussiness())
				{
					lock (m_lock)
					{
						List<ItemInfo> items = GetItems();
						m_player.SendItemsToMail(items, null, LanguageMgr.GetTranslation("Você saiu da sociedade"), eMailType.ConsortionEmail);
						foreach (ItemInfo item in items)
						{
							RemoveItem(item);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Send Items Mail Error:" + ex);
			}
			finally
			{
				SaveNewItemToDatabase();
				CommitChanges();
			}
			m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
		}
		return true;
	}

	public bool SendItemsToMail(List<ItemInfo> items, MailInfo mail, PlayerBussiness pb)
	{
		if (mail == null)
		{
			return false;
		}
		if (items.Count > 5)
		{
			return false;
		}
		if (m_saveToDb)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			if (items.Count > 0 && TakeOutItem(items[0]))
			{
				ItemInfo itemInfo = items[0];
				mail.Annex1 = itemInfo.ItemID.ToString();
				mail.Annex1Name = itemInfo.Template.Name;
				stringBuilder.Append("1、" + mail.Annex1Name + "x" + itemInfo.Count + ";");
				list.Add(itemInfo);
			}
			if (items.Count > 1 && TakeOutItem(items[1]))
			{
				ItemInfo itemInfo2 = items[1];
				mail.Annex2 = itemInfo2.ItemID.ToString();
				mail.Annex2Name = itemInfo2.Template.Name;
				stringBuilder.Append("2、" + mail.Annex2Name + "x" + itemInfo2.Count + ";");
				list.Add(itemInfo2);
			}
			if (items.Count > 2 && TakeOutItem(items[2]))
			{
				ItemInfo itemInfo3 = items[2];
				mail.Annex3 = itemInfo3.ItemID.ToString();
				mail.Annex3Name = itemInfo3.Template.Name;
				stringBuilder.Append("3、" + mail.Annex3Name + "x" + itemInfo3.Count + ";");
				list.Add(itemInfo3);
			}
			if (items.Count > 3 && TakeOutItem(items[3]))
			{
				ItemInfo itemInfo4 = items[3];
				mail.Annex4 = itemInfo4.ItemID.ToString();
				mail.Annex4Name = itemInfo4.Template.Name;
				stringBuilder.Append("4、" + mail.Annex4Name + "x" + itemInfo4.Count + ";");
				list.Add(itemInfo4);
			}
			if (items.Count > 4 && TakeOutItem(items[4]))
			{
				ItemInfo itemInfo5 = items[4];
				mail.Annex5 = itemInfo5.ItemID.ToString();
				mail.Annex5Name = itemInfo5.Template.Name;
				stringBuilder.Append("5、" + mail.Annex5Name + "x" + itemInfo5.Count + ";");
				list.Add(itemInfo5);
			}
			mail.AnnexRemark = stringBuilder.ToString();
			if (pb.SendMail(mail, m_player.ZoneId))
			{
				return true;
			}
			foreach (ItemInfo item in list)
			{
				AddItem(item);
			}
		}
		return false;
	}
}

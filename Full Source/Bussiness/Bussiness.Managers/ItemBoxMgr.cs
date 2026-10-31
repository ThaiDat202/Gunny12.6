using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ItemBoxMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ItemBoxInfo[] m_itemBox;

	private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;

	private static MarkOpenBoxInfo[] m_markOpenBox;

	private static Dictionary<int, List<MarkOpenBoxInfo>> m_markOpenBoxs;

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool ReLoad()
	{
		try
		{
			ItemBoxInfo[] array = LoadItemBoxDb();
			MarkOpenBoxInfo[] array2 = LoadMarkBoxDb();
			Dictionary<int, List<ItemBoxInfo>> value = LoadItemBoxs(array);
			Dictionary<int, List<MarkOpenBoxInfo>> value2 = LoadMarkBoxs(array2);
			if (array != null)
			{
				Interlocked.Exchange(ref m_itemBox, array);
				Interlocked.Exchange(ref m_itemBoxs, value);
				Interlocked.Exchange(ref m_markOpenBox, array2);
				Interlocked.Exchange(ref m_markOpenBoxs, value2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
			return false;
		}
		return true;
	}

	public static List<ItemBoxInfo> FindLotteryItemBoxByRand(int DateId, int countSelect)
	{
		List<ItemBoxInfo> list = FindLotteryItemBox(DateId);
		List<ItemBoxInfo> list2 = new List<ItemBoxInfo>();
		for (int i = 0; i < countSelect; i++)
		{
			int num = ThreadSafeRandom.NextStatic(0, list.Count);
			if (num < list.Count)
			{
				list2.Add(list[num]);
				list.Remove(list[num]);
			}
		}
		return list2;
	}

	public static List<ItemBoxInfo> FindLotteryItemBox(int DataId)
	{
		if (!m_itemBoxs.ContainsKey(DataId))
		{
			return null;
		}
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		foreach (ItemBoxInfo item in m_itemBoxs[DataId])
		{
			bool flag = true;
			foreach (ItemBoxInfo item2 in list)
			{
				if (item2.TemplateId == item.TemplateId && item2.ItemCount == item.ItemCount)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int medal, ref int exp)
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		List<ItemBoxInfo> list2 = FindItemBox(DateId);
		if (list2 == null)
		{
			return false;
		}
		List<ItemBoxInfo> list3 = list2.Where((ItemBoxInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = 0;
		if (list3.Count < list2.Count)
		{
			maxRound = ThreadSafeRandom.NextStatic((from s in list2
				where !s.IsSelect
				select s.Random).Max());
		}
		List<ItemBoxInfo> list4 = list2.Where((ItemBoxInfo s) => !s.IsSelect && s.Random >= maxRound).ToList();
		int num2 = list4.Count();
		if (num2 > 0)
		{
			int count = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, count);
			foreach (int index in randomUnrepeatArray)
			{
				ItemBoxInfo item = list4[index];
				if (list3 == null)
				{
					list3 = new List<ItemBoxInfo>();
				}
				list3.Add(item);
			}
		}
		foreach (ItemBoxInfo item2 in list3)
		{
			if (item2 == null)
			{
				return false;
			}
			switch (item2.TemplateId)
			{
			case -1100:
				giftToken += item2.ItemCount;
				continue;
			case -300:
				medal += item2.ItemCount;
				continue;
			case -200:
				point += item2.ItemCount;
				continue;
			case -100:
				gold += item2.ItemCount;
				continue;
			case 11107:
				exp += item2.ItemCount;
				continue;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateId), item2.ItemCount, 101);
			if (itemInfo != null)
			{
				itemInfo.Count = item2.ItemCount;
				itemInfo.IsBinds = item2.IsBind;
				itemInfo.ValidDate = item2.ItemValid;
				itemInfo.StrengthenLevel = item2.StrengthenLevel;
				itemInfo.AttackCompose = item2.AttackCompose;
				itemInfo.DefendCompose = item2.DefendCompose;
				itemInfo.AgilityCompose = item2.AgilityCompose;
				itemInfo.LuckCompose = item2.LuckCompose;
				itemInfo.IsTips = item2.IsTips != 0;
				itemInfo.IsLogs = item2.IsLogs;
				if (itemInfos == null)
				{
					itemInfos = new List<ItemInfo>();
				}
				itemInfos.Add(itemInfo);
			}
		}
		return true;
	}

	public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int medal, ref int exp, ref int honor, ref int hardCurrency, ref int leagueMoney, ref int useableScore, ref int prestge, ref int magicStonePoint, ref int loveNum)
	{
		List<ItemBoxInfo> list = FindItemBox(DateId);
		if (list == null)
		{
			return false;
		}
		List<ItemBoxInfo> list2 = list.Where((ItemBoxInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = 0;
		if (list2.Count < list.Count)
		{
			maxRound = ThreadSafeRandom.NextStatic((from s in list
				where !s.IsSelect
				select s.Random).Max());
		}
		List<ItemBoxInfo> list3 = list.Where((ItemBoxInfo s) => !s.IsSelect && s.Random >= maxRound).ToList();
		int count = list3.Count;
		if (count > 0)
		{
			num = ((num > count) ? count : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, count - 1, num);
			foreach (int index in randomUnrepeatArray)
			{
				ItemBoxInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (ItemBoxInfo item2 in list2)
		{
			if (item2 == null)
			{
				bool flag = false;
				return false;
			}
			switch (item2.TemplateId)
			{
			case -1300:
				prestge += item2.ItemCount;
				continue;
			case -1400:
				magicStonePoint += item2.ItemCount;
				continue;
			case -1500:
				loveNum += item2.ItemCount;
				continue;
			case -1000:
				leagueMoney += item2.ItemCount;
				continue;
			case -1100:
				giftToken += item2.ItemCount;
				continue;
			case -1200:
				useableScore += item2.ItemCount;
				continue;
			case -300:
				medal += item2.ItemCount;
				continue;
			case -800:
				honor += item2.ItemCount;
				continue;
			case -900:
				hardCurrency += item2.ItemCount;
				continue;
			case 11107:
				exp += item2.ItemCount;
				continue;
			case -100:
				gold += item2.ItemCount;
				continue;
			case -200:
				point += item2.ItemCount;
				continue;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateId), item2.ItemCount, 101);
			if (itemInfo != null)
			{
				itemInfo.Count = item2.ItemCount;
				itemInfo.IsBinds = item2.IsBind;
				itemInfo.ValidDate = item2.ItemValid;
				itemInfo.StrengthenLevel = item2.StrengthenLevel;
				itemInfo.AttackCompose = item2.AttackCompose;
				itemInfo.DefendCompose = item2.DefendCompose;
				itemInfo.AgilityCompose = item2.AgilityCompose;
				itemInfo.LuckCompose = item2.LuckCompose;
				itemInfo.IsTips = item2.IsTips != 0;
				itemInfo.IsLogs = item2.IsLogs;
				if (itemInfos == null)
				{
					itemInfos = new List<ItemInfo>();
				}
				itemInfos.Add(itemInfo);
			}
		}
		return true;
	}

	public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int exp)
	{
		return CreateItemBox(DateId, null, itemInfos, ref gold, ref point, ref giftToken, ref exp);
	}

	public static bool CreateItemBox(int DateId, List<ItemBoxInfo> tempBox, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int exp)
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		List<ItemBoxInfo> list2 = FindItemBox(DateId);
		if (tempBox != null && tempBox.Count > 0)
		{
			list2 = tempBox;
		}
		if (list2 == null)
		{
			return false;
		}
		List<ItemBoxInfo> list3 = list2.Where((ItemBoxInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = 0;
		if (list3.Count < list2.Count)
		{
			maxRound = ThreadSafeRandom.NextStatic((from s in list2
				where !s.IsSelect
				select s.Random).Max());
		}
		List<ItemBoxInfo> list4 = list2.Where((ItemBoxInfo s) => !s.IsSelect && s.Random >= maxRound).ToList();
		int num2 = list4.Count();
		if (num2 > 0)
		{
			int count = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, count);
			foreach (int index in randomUnrepeatArray)
			{
				ItemBoxInfo item = list4[index];
				if (list3 == null)
				{
					list3 = new List<ItemBoxInfo>();
				}
				list3.Add(item);
			}
		}
		foreach (ItemBoxInfo item2 in list3)
		{
			if (item2 == null)
			{
				return false;
			}
			switch (item2.TemplateId)
			{
			case -300:
				giftToken += item2.ItemCount;
				continue;
			case -200:
				point += item2.ItemCount;
				continue;
			case -100:
				gold += item2.ItemCount;
				continue;
			case 11107:
				exp += item2.ItemCount;
				continue;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateId), item2.ItemCount, 101);
			if (itemInfo != null)
			{
				itemInfo.IsBinds = item2.IsBind;
				itemInfo.ValidDate = item2.ItemValid;
				itemInfo.StrengthenLevel = item2.StrengthenLevel;
				itemInfo.AttackCompose = item2.AttackCompose;
				itemInfo.DefendCompose = item2.DefendCompose;
				itemInfo.AgilityCompose = item2.AgilityCompose;
				itemInfo.LuckCompose = item2.LuckCompose;
				if (itemInfos == null)
				{
					itemInfos = new List<ItemInfo>();
				}
				itemInfos.Add(itemInfo);
			}
		}
		return true;
	}

	public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, SpecialItemDataInfo specialInfo)
	{
		return CreateItemBox(DateId, null, itemInfos, specialInfo);
	}

	public static bool CreateItemBox(int DateId, List<ItemBoxInfo> tempBox, List<ItemInfo> itemInfos, SpecialItemDataInfo specialInfo)
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		List<ItemBoxInfo> list2 = FindItemBox(DateId);
		if (tempBox != null && tempBox.Count > 0)
		{
			list2 = tempBox;
		}
		if (list2 == null)
		{
			return false;
		}
		List<ItemBoxInfo> list3 = list2.Where((ItemBoxInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = 0;
		if (list3.Count < list2.Count)
		{
			maxRound = ThreadSafeRandom.NextStatic((from s in list2
				where !s.IsSelect
				select s.Random).Max());
			if (maxRound <= 0)
			{
				log.Error("ItemBoxMgr Random Error: " + maxRound + " | " + DateId);
				maxRound = (from s in list2
					where !s.IsSelect
					select s.Random).Max();
			}
		}
		List<ItemBoxInfo> list4 = list2.Where((ItemBoxInfo s) => !s.IsSelect && s.Random >= maxRound).ToList();
		int num2 = list4.Count();
		if (num2 > 0)
		{
			int count = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, count);
			foreach (int index in randomUnrepeatArray)
			{
				ItemBoxInfo item = list4[index];
				if (list3 == null)
				{
					list3 = new List<ItemBoxInfo>();
				}
				list3.Add(item);
			}
		}
		foreach (ItemBoxInfo item2 in list3)
		{
			if (item2 == null)
			{
				return false;
			}
			switch (item2.TemplateId)
			{
			case -300:
				specialInfo.GiftToken += item2.ItemCount;
				continue;
			case -200:
				specialInfo.Money += item2.ItemCount;
				continue;
			case -100:
				specialInfo.Gold += item2.ItemCount;
				continue;
			case 11107:
				specialInfo.GP += item2.ItemCount;
				continue;
			}
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateId), item2.ItemCount, 101);
			if (itemInfo != null)
			{
				itemInfo.IsBinds = item2.IsBind;
				itemInfo.ValidDate = item2.ItemValid;
				itemInfo.StrengthenLevel = item2.StrengthenLevel;
				itemInfo.AttackCompose = item2.AttackCompose;
				itemInfo.DefendCompose = item2.DefendCompose;
				itemInfo.AgilityCompose = item2.AgilityCompose;
				itemInfo.LuckCompose = item2.LuckCompose;
				if (itemInfos == null)
				{
					itemInfos = new List<ItemInfo>();
				}
				itemInfos.Add(itemInfo);
			}
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static ItemBoxInfo[] LoadItemBoxDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetItemBoxInfos();
	}

	public static MarkOpenBoxInfo[] LoadMarkBoxDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllMarkOpenBox();
	}

	public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
	{
		Dictionary<int, List<ItemBoxInfo>> dictionary = new Dictionary<int, List<ItemBoxInfo>>();
		foreach (ItemBoxInfo info in itemBoxs)
		{
			if (!dictionary.Keys.Contains(info.ID))
			{
				IEnumerable<ItemBoxInfo> source = itemBoxs.Where((ItemBoxInfo s) => s.ID == info.ID);
				dictionary.Add(info.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<MarkOpenBoxInfo>> LoadMarkBoxs(MarkOpenBoxInfo[] itemBoxs)
	{
		Dictionary<int, List<MarkOpenBoxInfo>> dictionary = new Dictionary<int, List<MarkOpenBoxInfo>>();
		foreach (MarkOpenBoxInfo info in itemBoxs)
		{
			if (!dictionary.Keys.Contains(info.Type))
			{
				IEnumerable<MarkOpenBoxInfo> source = itemBoxs.Where((MarkOpenBoxInfo s) => s.Type == info.Type);
				dictionary.Add(info.Type, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<ItemBoxInfo> FindItemBox(int DataId)
	{
		if (m_itemBoxs.ContainsKey(DataId))
		{
			return m_itemBoxs[DataId];
		}
		return null;
	}

	public static List<MarkOpenBoxInfo> FindMarkBox(int type)
	{
		if (m_markOpenBoxs.ContainsKey(type))
		{
			return m_markOpenBoxs[type];
		}
		return null;
	}

	public static List<ItemInfo> GetAllItemBoxAward(int DataId)
	{
		List<ItemBoxInfo> list = FindItemBox(DataId);
		List<ItemInfo> list2 = new List<ItemInfo>();
		foreach (ItemBoxInfo item in list)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateId), item.ItemCount, 105);
			itemInfo.IsBinds = item.IsBind;
			itemInfo.ValidDate = item.ItemValid;
			list2.Add(itemInfo);
		}
		return list2;
	}

	public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref SpecialItemBoxInfo specialValue)
	{
		List<ItemBoxInfo> list = FindItemBox(DateId);
		if (list == null)
		{
			return false;
		}
		List<ItemBoxInfo> list2 = list.Where((ItemBoxInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = 0;
		if (list2.Count < list.Count)
		{
			maxRound = ThreadSafeRandom.NextStatic((from s in list
				where !s.IsSelect
				select s.Random).Max());
		}
		List<ItemBoxInfo> list3 = list.Where((ItemBoxInfo s) => !s.IsSelect && s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				ItemBoxInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (ItemBoxInfo item2 in list2)
		{
			if (item2 == null)
			{
				return false;
			}
			switch (item2.TemplateId)
			{
			case -100:
				specialValue.Gold += item2.ItemCount;
				continue;
			case -200:
				specialValue.Money += item2.ItemCount;
				continue;
			case -300:
				specialValue.Medal += item2.ItemCount;
				continue;
			case -800:
				specialValue.Honor += item2.ItemCount;
				continue;
			case -900:
				specialValue.HardCurrency += item2.ItemCount;
				continue;
			case -1000:
				specialValue.LeagueMoney += item2.ItemCount;
				continue;
			case -1100:
				specialValue.DDTMoney += item2.ItemCount;
				continue;
			case -1200:
				specialValue.UseableScore += item2.ItemCount;
				continue;
			case -1300:
				specialValue.Prestge += item2.ItemCount;
				continue;
			case -1400:
				specialValue.MagicstoneScore += item2.ItemCount;
				continue;
			case -1500:
				specialValue.LoveScore += item2.ItemCount;
				continue;
			case -1600:
				specialValue.SummerScore += item2.ItemCount;
				continue;
			case -1700:
				specialValue.LaurelScore += item2.ItemCount;
				continue;
			case -1800:
				specialValue.EquestrianScore += item2.ItemCount;
				continue;
			case 11107:
				specialValue.Exp += item2.ItemCount;
				continue;
			case -2100:
				specialValue.CrystalScore += item2.ItemCount;
				continue;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item2.TemplateId);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, item2.ItemCount, 101);
			if (itemInfo == null)
			{
				continue;
			}
			if (itemTemplateInfo.CategoryID == 61)
			{
				ItemInfo magicStoneInfo = MagicStoneMgr.GetMagicStoneInfo(itemTemplateInfo, 1);
				if (magicStoneInfo != null)
				{
					itemInfo.StrengthenLevel = magicStoneInfo.StrengthenLevel;
					itemInfo.StrengthenExp = magicStoneInfo.StrengthenExp;
					itemInfo.AttackCompose = magicStoneInfo.AttackCompose;
					itemInfo.DefendCompose = magicStoneInfo.DefendCompose;
					itemInfo.AgilityCompose = magicStoneInfo.AgilityCompose;
					itemInfo.LuckCompose = magicStoneInfo.LuckCompose;
					itemInfo.MagicAttack = magicStoneInfo.MagicAttack;
					itemInfo.MagicDefence = magicStoneInfo.MagicDefence;
					itemInfo.goodsLock = false;
				}
			}
			else
			{
				itemInfo.StrengthenLevel = item2.StrengthenLevel;
				itemInfo.AttackCompose = item2.AttackCompose;
				itemInfo.DefendCompose = item2.DefendCompose;
				itemInfo.AgilityCompose = item2.AgilityCompose;
				itemInfo.LuckCompose = item2.LuckCompose;
			}
			itemInfo.IsBinds = item2.IsBind;
			itemInfo.ValidDate = item2.ItemValid;
			itemInfo.Count = item2.ItemCount;
			itemInfo.IsTips = item2.IsTips == 1;
			itemInfo.IsLogs = item2.IsLogs;
			if (itemInfos == null)
			{
				itemInfos = new List<ItemInfo>();
			}
			itemInfos.Add(itemInfo);
		}
		return true;
	}

	public static bool CreateMarkBox(int type, List<MarkOpenBoxInfo> itemInfos)
	{
		List<MarkOpenBoxInfo> list = FindMarkBox(type);
		if (list == null)
		{
			return false;
		}
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(list.Select((MarkOpenBoxInfo s) => s.Random).Max());
		List<MarkOpenBoxInfo> list2 = list.Where((MarkOpenBoxInfo s) => s.Random >= maxRound).ToList();
		int num2 = list2.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				MarkOpenBoxInfo item = list2[index];
				itemInfos.Add(item);
			}
		}
		return true;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = random.Next(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}

	public static List<ItemBoxInfo> FindRandomItemBox(int DataId)
	{
		if (m_itemBoxs.ContainsKey(DataId))
		{
			List<ItemBoxInfo> list = new List<ItemBoxInfo>();
			foreach (ItemBoxInfo item in m_itemBoxs[DataId])
			{
				bool flag = true;
				foreach (ItemBoxInfo item2 in list)
				{
					if (item2.TemplateId == item.TemplateId && item2.ItemCount == item.ItemCount)
					{
						flag = false;
						break;
					}
				}
				if (flag && !item.IsSelect)
				{
					list.Add(item);
				}
			}
			return list;
		}
		return null;
	}

	public static List<ItemBoxInfo> FindFixedItemBox(int DataId)
	{
		if (m_itemBoxs.ContainsKey(DataId))
		{
			List<ItemBoxInfo> list = new List<ItemBoxInfo>();
			foreach (ItemBoxInfo item in m_itemBoxs[DataId])
			{
				bool flag = true;
				foreach (ItemBoxInfo item2 in list)
				{
					if (item2.TemplateId == item.TemplateId && item2.ItemCount == item.ItemCount)
					{
						flag = false;
						break;
					}
				}
				if (flag && item.IsSelect)
				{
					list.Add(item);
				}
			}
			return list;
		}
		return null;
	}

	public static List<ItemBoxInfo> FindItemBoxByRand(int DateId, int countSelect)
	{
		List<ItemBoxInfo> list = FindRandomItemBox(DateId);
		List<ItemBoxInfo> list2 = FindFixedItemBox(DateId);
		List<ItemBoxInfo> list3 = new List<ItemBoxInfo>();
		foreach (ItemBoxInfo item in list2)
		{
			list3.Add(item);
		}
		countSelect -= list2.Count;
		for (int i = 0; i < countSelect; i++)
		{
			int num = ThreadSafeRandom.NextStatic(0, list.Count);
			if (num < list.Count)
			{
				list3.Add(list[num]);
				list.Remove(list[num]);
			}
		}
		return list3;
	}

	public static void CreateMarkDataBox(int typeBox)
	{
		int minQuaity = 1;
		int maxQuaity = 2;
		if (typeBox == 1)
		{
			minQuaity = 3;
			maxQuaity = 4;
		}
		List<ItemTemplateInfo> markWithQuality = ItemMgr.GetMarkWithQuality(minQuaity, maxQuaity);
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		foreach (ItemTemplateInfo item in markWithQuality)
		{
			produceBussiness.AddMarkBox(item, typeBox, 1);
		}
	}
}

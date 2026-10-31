using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public static class AccumulActiveLoginMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static AccumulAtiveLoginAwardInfo[] m_AccumulAtiveLoginAward;

	private static Dictionary<int, List<AccumulAtiveLoginAwardInfo>> m_AccumulAtiveLoginAwards = new Dictionary<int, List<AccumulAtiveLoginAwardInfo>>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			AccumulAtiveLoginAwardInfo[] array = LoadAccumulAtiveLoginAwardDb();
			Dictionary<int, List<AccumulAtiveLoginAwardInfo>> value = LoadAccumulAtiveLoginAwards(array);
			if (array != null)
			{
				Interlocked.Exchange(ref m_AccumulAtiveLoginAward, array);
				Interlocked.Exchange(ref m_AccumulAtiveLoginAwards, value);
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

	public static bool Init()
	{
		return ReLoad();
	}

	public static AccumulAtiveLoginAwardInfo[] LoadAccumulAtiveLoginAwardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAccumulAtiveLoginAwardInfos();
	}

	public static Dictionary<int, List<AccumulAtiveLoginAwardInfo>> LoadAccumulAtiveLoginAwards(AccumulAtiveLoginAwardInfo[] AccumulAtiveLoginAwards)
	{
		Dictionary<int, List<AccumulAtiveLoginAwardInfo>> dictionary = new Dictionary<int, List<AccumulAtiveLoginAwardInfo>>();
		foreach (AccumulAtiveLoginAwardInfo info in AccumulAtiveLoginAwards)
		{
			if (!dictionary.Keys.Contains(info.Count))
			{
				IEnumerable<AccumulAtiveLoginAwardInfo> source = AccumulAtiveLoginAwards.Where((AccumulAtiveLoginAwardInfo s) => s.Count == info.Count);
				dictionary.Add(info.Count, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<AccumulAtiveLoginAwardInfo> FindAccumulAtiveLoginAward(int Count)
	{
		if (m_AccumulAtiveLoginAwards.ContainsKey(Count))
		{
			return m_AccumulAtiveLoginAwards[Count];
		}
		return null;
	}

	public static List<ItemInfo> GetAllAccumulAtiveLoginAward(int Count)
	{
		List<AccumulAtiveLoginAwardInfo> list = FindAccumulAtiveLoginAward(Count);
		List<ItemInfo> list2 = new List<ItemInfo>();
		if (list != null)
		{
			foreach (AccumulAtiveLoginAwardInfo item in list)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.RewardItemID), item.RewardItemCount, 105);
				itemInfo.IsBinds = item.IsBind;
				itemInfo.ValidDate = item.RewardItemValid;
				itemInfo.StrengthenLevel = item.StrengthenLevel;
				itemInfo.AttackCompose = item.AttackCompose;
				itemInfo.DefendCompose = item.DefendCompose;
				itemInfo.AgilityCompose = item.AgilityCompose;
				itemInfo.LuckCompose = item.LuckCompose;
				list2.Add(itemInfo);
			}
		}
		return list2;
	}

	public static List<ItemInfo> GetSelecedAccumulAtiveLoginAward(int ID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<AccumulAtiveLoginAwardInfo> list2 = FindAccumulAtiveLoginAward(7);
		if (list2 != null)
		{
			foreach (AccumulAtiveLoginAwardInfo item in list2)
			{
				if (ID == item.ID)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.RewardItemID), item.RewardItemCount, 105);
					itemInfo.IsBinds = item.IsBind;
					itemInfo.ValidDate = item.RewardItemValid;
					itemInfo.StrengthenLevel = item.StrengthenLevel;
					itemInfo.AttackCompose = item.AttackCompose;
					itemInfo.DefendCompose = item.DefendCompose;
					itemInfo.AgilityCompose = item.AgilityCompose;
					itemInfo.LuckCompose = item.LuckCompose;
					list.Add(itemInfo);
					break;
				}
			}
		}
		return list;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ActiveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ActiveInfo> m_activeinfo = new Dictionary<int, ActiveInfo>();

	private static Dictionary<int, List<ActiveConvertItemInfo>> m_activeConvertItem = new Dictionary<int, List<ActiveConvertItemInfo>>();

	private static Dictionary<int, List<ActiveAwardInfo>> m_activeAwards = new Dictionary<int, List<ActiveAwardInfo>>();

	private static Dictionary<int, List<ActivitySystemItemInfo>> m_ActivitySystemItems = new Dictionary<int, List<ActivitySystemItemInfo>>();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ActiveInfo> dictionary = LoadActiveInfoDb();
			Dictionary<int, List<ActiveConvertItemInfo>> value = LoadActiveCondictionDb(dictionary);
			Dictionary<int, List<ActiveAwardInfo>> value2 = LoadActiveGoodDb(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_activeinfo, dictionary);
				Interlocked.Exchange(ref m_activeConvertItem, value);
				Interlocked.Exchange(ref m_activeAwards, value2);
			}
			ActivitySystemItemInfo[] array = LoadActivitySystemItemDb();
			Dictionary<int, List<ActivitySystemItemInfo>> value3 = LoadActivitySystemItems(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_ActivitySystemItems, value3);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("ActiveMgr", exception);
		}
		return false;
	}

	public static void UpdateCurrentServerActive()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		playerBussiness.DeleteAllActive();
		foreach (ActiveInfo value in m_activeinfo.Values)
		{
			playerBussiness.AddActive(value);
			if (!m_activeAwards.ContainsKey(value.ActiveID))
			{
				continue;
			}
			List<ActiveAwardInfo> list = m_activeAwards[value.ActiveID];
			foreach (ActiveAwardInfo item in list)
			{
				playerBussiness.AddActiveAward(item);
			}
		}
	}

	public static ActiveConvertItemInfo GetActiveConvertItem(int id, int templateID, int index)
	{
		if (m_activeConvertItem.ContainsKey(id))
		{
			List<ActiveConvertItemInfo> list = m_activeConvertItem[id];
			foreach (ActiveConvertItemInfo item in list)
			{
				if (item.TemplateID == templateID && item.ItemType == GetNeedGoodsAward(index))
				{
					return item;
				}
			}
		}
		return null;
	}

	public static List<ActiveConvertItemInfo> GetActiveConvertItemAward(int id, int index)
	{
		List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
		if (m_activeConvertItem.ContainsKey(id))
		{
			List<ActiveConvertItemInfo> list2 = m_activeConvertItem[id];
			foreach (ActiveConvertItemInfo item in list2)
			{
				if (item.ItemType == GetGoodsAward(index))
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static List<ActiveConvertItemInfo> FindActiveConvertItem(int id)
	{
		if (m_activeConvertItem.ContainsKey(id))
		{
			return m_activeConvertItem[id];
		}
		return null;
	}

	public static int GetGoodsAward(int index)
	{
		return index switch
		{
			1 => 3, 
			2 => 5, 
			3 => 7, 
			_ => 1, 
		};
	}

	public static int GetNeedGoodsAward(int index)
	{
		return index switch
		{
			1 => 2, 
			2 => 4, 
			3 => 6, 
			_ => 0, 
		};
	}

	public static ActivitySystemItemInfo[] LoadActivitySystemItemDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllActivitySystemItem();
	}

	public static Dictionary<int, List<ActivitySystemItemInfo>> LoadActivitySystemItems(ActivitySystemItemInfo[] ActivitySystemItem)
	{
		Dictionary<int, List<ActivitySystemItemInfo>> dictionary = new Dictionary<int, List<ActivitySystemItemInfo>>();
		foreach (ActivitySystemItemInfo info in ActivitySystemItem)
		{
			if (!dictionary.Keys.Contains(info.ActivityType))
			{
				IEnumerable<ActivitySystemItemInfo> source = ActivitySystemItem.Where((ActivitySystemItemInfo s) => s.ActivityType == info.ActivityType);
				dictionary.Add(info.ActivityType, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<ActivitySystemItemInfo> FindActivitySystemItem(int ActivityType)
	{
		if (m_ActivitySystemItems.ContainsKey(ActivityType))
		{
			List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
			foreach (ActivitySystemItemInfo item in m_ActivitySystemItems[ActivityType])
			{
				list.Add(item);
			}
			return list;
		}
		return null;
	}

	public static List<ActivitySystemItemInfo> GetActivitySystemItemByLayer(int layer)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(8);
		foreach (ActivitySystemItemInfo item in list2)
		{
			if (item.Quality == layer)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> GetGrowthPackage(int layer)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(20);
		foreach (ActivitySystemItemInfo item in list2)
		{
			if (item.Quality == layer)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindChickActivePakage(int quality)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(40);
		foreach (ActivitySystemItemInfo item in list2)
		{
			if (item.Quality == quality)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindSignBuffPackage(int quality)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(112);
		foreach (ActivitySystemItemInfo item in list2)
		{
			if (item.Quality == quality)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindLoginDevicePackage(int quality)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(110);
		foreach (ActivitySystemItemInfo item in list2)
		{
			if (item.Quality == quality)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindMinesRandomExchange(int total)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(156);
		for (int i = 0; i < total; i++)
		{
			int num = ThreadSafeRandom.NextStatic(list2.Count);
			if (num < list2.Count)
			{
				list.Add(list2[num]);
				list2.RemoveAt(num);
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindLotteryTicketPackage(int quality)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> list2 = FindActivitySystemItem(114);
		foreach (ActivitySystemItemInfo item in list2)
		{
			if (item.Quality == quality)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static Dictionary<int, ActiveInfo> LoadActiveInfoDb()
	{
		Dictionary<int, ActiveInfo> dictionary = new Dictionary<int, ActiveInfo>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			ActiveInfo[] allActives = activeBussiness.GetAllActives();
			ActiveInfo[] array = allActives;
			foreach (ActiveInfo activeInfo in array)
			{
				if (activeInfo.ActiveID >= 0 && !dictionary.ContainsKey(activeInfo.ActiveID))
				{
					dictionary.Add(activeInfo.ActiveID, activeInfo);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<ActiveConvertItemInfo>> LoadActiveCondictionDb(Dictionary<int, ActiveInfo> Actives)
	{
		Dictionary<int, List<ActiveConvertItemInfo>> dictionary = new Dictionary<int, List<ActiveConvertItemInfo>>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			ActiveConvertItemInfo[] allActiveConvertItem = activeBussiness.GetAllActiveConvertItem();
			foreach (ActiveInfo active in Actives.Values)
			{
				IEnumerable<ActiveConvertItemInfo> source = allActiveConvertItem.Where((ActiveConvertItemInfo s) => s.ActiveID == active.ActiveID);
				dictionary.Add(active.ActiveID, source.ToList());
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<ActiveAwardInfo>> LoadActiveGoodDb(Dictionary<int, ActiveInfo> Actives)
	{
		Dictionary<int, List<ActiveAwardInfo>> dictionary = new Dictionary<int, List<ActiveAwardInfo>>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			ActiveAwardInfo[] allActiveAwardInfo = activeBussiness.GetAllActiveAwardInfo();
			foreach (ActiveInfo Active in Actives.Values)
			{
				IEnumerable<ActiveAwardInfo> source = allActiveAwardInfo.Where((ActiveAwardInfo s) => s.ActiveID == Active.ActiveID);
				dictionary.Add(Active.ActiveID, source.ToList());
			}
		}
		return dictionary;
	}

	public static ActiveInfo GetSingleActive(int id)
	{
		if (m_activeinfo.Count == 0)
		{
			Init();
		}
		if (m_activeinfo.ContainsKey(id))
		{
			return m_activeinfo[id];
		}
		return null;
	}

	public static List<ActiveAwardInfo> GetActiveAward(int id)
	{
		if (m_activeinfo.Count == 0)
		{
			Init();
		}
		if (m_activeAwards.ContainsKey(id))
		{
			return m_activeAwards[id];
		}
		return null;
	}
}

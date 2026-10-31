using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ActivityHalloweenItemsMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<ActivityHalloweenItemsInfo>> m_activityHalloweenItemss = new Dictionary<int, List<ActivityHalloweenItemsInfo>>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			ActivityHalloweenItemsInfo[] array = LoadActivityHalloweenItemsDb();
			Dictionary<int, List<ActivityHalloweenItemsInfo>> value = LoadActivityHalloweenItemss(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_activityHalloweenItemss, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad ActivityHalloweenItems", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static ActivityHalloweenItemsInfo[] LoadActivityHalloweenItemsDb()
	{
		using ActiveBussiness activeBussiness = new ActiveBussiness();
		return activeBussiness.GetAllActivityHalloweenItems();
	}

	public static Dictionary<int, List<ActivityHalloweenItemsInfo>> LoadActivityHalloweenItemss(ActivityHalloweenItemsInfo[] ActivityHalloweenItems)
	{
		Dictionary<int, List<ActivityHalloweenItemsInfo>> dictionary = new Dictionary<int, List<ActivityHalloweenItemsInfo>>();
		foreach (ActivityHalloweenItemsInfo info in ActivityHalloweenItems)
		{
			if (!dictionary.Keys.Contains(info.rewardLevel))
			{
				IEnumerable<ActivityHalloweenItemsInfo> source = ActivityHalloweenItems.Where((ActivityHalloweenItemsInfo s) => s.rewardLevel == info.rewardLevel);
				dictionary.Add(info.rewardLevel, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<ActivityHalloweenItemsInfo> FindActivityHalloweenItems(int rewardLevel)
	{
		if (m_activityHalloweenItemss.ContainsKey(rewardLevel))
		{
			return m_activityHalloweenItemss[rewardLevel];
		}
		return null;
	}
}

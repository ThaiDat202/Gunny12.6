using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class AmuletGradeItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, AmuletGradeItemInfo> m_amuletGradeItems = new Dictionary<int, AmuletGradeItemInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, AmuletGradeItemInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_amuletGradeItems, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("AmuletGradeItemMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, AmuletGradeItemInfo> LoadFromDatabase()
	{
		Dictionary<int, AmuletGradeItemInfo> dictionary = new Dictionary<int, AmuletGradeItemInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			AmuletGradeItemInfo[] allAmuletGradeItem = produceBussiness.GetAllAmuletGradeItem();
			AmuletGradeItemInfo[] array = allAmuletGradeItem;
			foreach (AmuletGradeItemInfo amuletGradeItemInfo in array)
			{
				if (!dictionary.ContainsKey(amuletGradeItemInfo.WahsLevel))
				{
					dictionary.Add(amuletGradeItemInfo.WahsLevel, amuletGradeItemInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<AmuletGradeItemInfo> GetAllAmuletGradeItem()
	{
		if (m_amuletGradeItems.Count == 0)
		{
			Init();
		}
		return m_amuletGradeItems.Values.ToList();
	}

	public static AmuletGradeItemInfo FindAmuletGradeItem(int id)
	{
		if (m_amuletGradeItems.Count == 0)
		{
			Init();
		}
		if (m_amuletGradeItems.ContainsKey(id))
		{
			return m_amuletGradeItems[id];
		}
		return null;
	}

	public static AmuletGradeItemInfo FindAmuletGradeByGp(int exp)
	{
		AmuletGradeItemInfo amuletGradeItemInfo = FindAmuletGradeItem(10);
		if (amuletGradeItemInfo != null && exp >= amuletGradeItemInfo.WahsTimes)
		{
			return amuletGradeItemInfo;
		}
		for (int i = 1; i <= m_amuletGradeItems.Count; i++)
		{
			if (m_amuletGradeItems.ContainsKey(i) && exp < m_amuletGradeItems[i].WahsTimes)
			{
				return (i == 1) ? null : m_amuletGradeItems[i - 1];
			}
		}
		return null;
	}
}

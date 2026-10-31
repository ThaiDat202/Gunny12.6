using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class AmuletItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, AmuletItemInfo> m_amuletItems = new Dictionary<int, AmuletItemInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static int maxLevel = 0;

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, AmuletItemInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_amuletItems, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("AmuletItemMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, AmuletItemInfo> LoadFromDatabase()
	{
		Dictionary<int, AmuletItemInfo> dictionary = new Dictionary<int, AmuletItemInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			AmuletItemInfo[] allAmuletItem = produceBussiness.GetAllAmuletItem();
			AmuletItemInfo[] array = allAmuletItem;
			foreach (AmuletItemInfo amuletItemInfo in array)
			{
				if (amuletItemInfo.Level > maxLevel)
				{
					maxLevel = amuletItemInfo.Level;
				}
				if (!dictionary.ContainsKey(amuletItemInfo.Level))
				{
					dictionary.Add(amuletItemInfo.Level, amuletItemInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<AmuletItemInfo> GetAllAmuletItem()
	{
		if (m_amuletItems.Count == 0)
		{
			Init();
		}
		return m_amuletItems.Values.ToList();
	}

	public static AmuletItemInfo FindAmuletItem(int id)
	{
		if (m_amuletItems.ContainsKey(id))
		{
			return m_amuletItems[id];
		}
		return null;
	}
}

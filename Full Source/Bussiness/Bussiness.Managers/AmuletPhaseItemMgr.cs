using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class AmuletPhaseItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, AmuletPhaseItemInfo> m_amuletPhaseItems = new Dictionary<int, AmuletPhaseItemInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, AmuletPhaseItemInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_amuletPhaseItems, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("AmuletPhaseItemMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, AmuletPhaseItemInfo> LoadFromDatabase()
	{
		Dictionary<int, AmuletPhaseItemInfo> dictionary = new Dictionary<int, AmuletPhaseItemInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			AmuletPhaseItemInfo[] allAmuletPhaseItem = produceBussiness.GetAllAmuletPhaseItem();
			AmuletPhaseItemInfo[] array = allAmuletPhaseItem;
			foreach (AmuletPhaseItemInfo amuletPhaseItemInfo in array)
			{
				if (!dictionary.ContainsKey(amuletPhaseItemInfo.Phase))
				{
					dictionary.Add(amuletPhaseItemInfo.Phase, amuletPhaseItemInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<AmuletPhaseItemInfo> GetAllAmuletPhaseItem()
	{
		if (m_amuletPhaseItems.Count == 0)
		{
			Init();
		}
		return m_amuletPhaseItems.Values.ToList();
	}

	public static AmuletPhaseItemInfo FindAmuletPhaseItem(int id)
	{
		if (m_amuletPhaseItems.ContainsKey(id))
		{
			return m_amuletPhaseItems[id];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class FightLabDropItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<FightLabDropItemInfo>> m_FightLabDropItems;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			FightLabDropItemInfo[] array = LoadFightLabDropItemDb();
			Dictionary<int, List<FightLabDropItemInfo>> value = LoadFightLabDropItems(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_FightLabDropItems, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad FightLabDropItem", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static FightLabDropItemInfo[] LoadFightLabDropItemDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllFightLabDropItem();
	}

	public static Dictionary<int, List<FightLabDropItemInfo>> LoadFightLabDropItems(FightLabDropItemInfo[] FightLabDropItem)
	{
		Dictionary<int, List<FightLabDropItemInfo>> dictionary = new Dictionary<int, List<FightLabDropItemInfo>>();
		foreach (FightLabDropItemInfo info in FightLabDropItem)
		{
			if (!dictionary.Keys.Contains(info.ID))
			{
				IEnumerable<FightLabDropItemInfo> source = FightLabDropItem.Where((FightLabDropItemInfo s) => s.ID == info.ID);
				dictionary.Add(info.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<FightLabDropItemInfo> FindFightLabDropItem(int DataId)
	{
		if (m_FightLabDropItems.ContainsKey(DataId))
		{
			return m_FightLabDropItems[DataId];
		}
		return null;
	}
}

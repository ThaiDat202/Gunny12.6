using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class EveryDayActiveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, EveryDayActivePointTemplateInfo> m_EveryDayActivePointTemplates;

	private static Dictionary<int, EveryDayActiveProgressInfo> m_EveryDayActiveProgresss;

	private static Dictionary<int, List<EveryDayActiveRewardTemplateInfo>> m_EveryDayActiveRewardTemplates;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			EveryDayActivePointTemplateInfo[] array = LoadEveryDayActivePointTemplateDb();
			Dictionary<int, EveryDayActivePointTemplateInfo> value = LoadEveryDayActivePointTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_EveryDayActivePointTemplates, value);
			}
			EveryDayActiveProgressInfo[] array2 = LoadEveryDayActiveProgressDb();
			Dictionary<int, EveryDayActiveProgressInfo> value2 = LoadEveryDayActiveProgresss(array2);
			if (array2.Length != 0)
			{
				Interlocked.Exchange(ref m_EveryDayActiveProgresss, value2);
			}
			EveryDayActiveRewardTemplateInfo[] array3 = LoadEveryDayActiveRewardTemplateDb();
			Dictionary<int, List<EveryDayActiveRewardTemplateInfo>> value3 = LoadEveryDayActiveRewardTemplates(array3);
			if (array3.Length != 0)
			{
				Interlocked.Exchange(ref m_EveryDayActiveRewardTemplates, value3);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad EveryDayActivePointTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static EveryDayActiveRewardTemplateInfo[] LoadEveryDayActiveRewardTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllEveryDayActiveRewardTemplate();
	}

	public static Dictionary<int, List<EveryDayActiveRewardTemplateInfo>> LoadEveryDayActiveRewardTemplates(EveryDayActiveRewardTemplateInfo[] EveryDayActiveRewardTemplate)
	{
		Dictionary<int, List<EveryDayActiveRewardTemplateInfo>> dictionary = new Dictionary<int, List<EveryDayActiveRewardTemplateInfo>>();
		foreach (EveryDayActiveRewardTemplateInfo info in EveryDayActiveRewardTemplate)
		{
			if (!dictionary.Keys.Contains(info.ID))
			{
				IEnumerable<EveryDayActiveRewardTemplateInfo> source = EveryDayActiveRewardTemplate.Where((EveryDayActiveRewardTemplateInfo s) => s.RewardID == info.RewardID);
				dictionary.Add(info.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<EveryDayActiveRewardTemplateInfo> FindEveryDayActiveRewardTemplate(int ID)
	{
		if (m_EveryDayActiveRewardTemplates.ContainsKey(ID))
		{
			return m_EveryDayActiveRewardTemplates[ID];
		}
		return null;
	}

	public static EveryDayActiveProgressInfo[] LoadEveryDayActiveProgressDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllEveryDayActiveProgress();
	}

	public static Dictionary<int, EveryDayActiveProgressInfo> LoadEveryDayActiveProgresss(EveryDayActiveProgressInfo[] EveryDayActiveProgress)
	{
		Dictionary<int, EveryDayActiveProgressInfo> dictionary = new Dictionary<int, EveryDayActiveProgressInfo>();
		foreach (EveryDayActiveProgressInfo everyDayActiveProgressInfo in EveryDayActiveProgress)
		{
			if (!dictionary.Keys.Contains(everyDayActiveProgressInfo.ID))
			{
				dictionary.Add(everyDayActiveProgressInfo.ID, everyDayActiveProgressInfo);
			}
		}
		return dictionary;
	}

	public static EveryDayActiveProgressInfo FindEveryDayActiveProgress(int ID)
	{
		if (m_EveryDayActiveProgresss.ContainsKey(ID))
		{
			return m_EveryDayActiveProgresss[ID];
		}
		return null;
	}

	public static EveryDayActivePointTemplateInfo[] LoadEveryDayActivePointTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllEveryDayActivePointTemplate();
	}

	public static Dictionary<int, EveryDayActivePointTemplateInfo> LoadEveryDayActivePointTemplates(EveryDayActivePointTemplateInfo[] EveryDayActivePointTemplate)
	{
		Dictionary<int, EveryDayActivePointTemplateInfo> dictionary = new Dictionary<int, EveryDayActivePointTemplateInfo>();
		foreach (EveryDayActivePointTemplateInfo everyDayActivePointTemplateInfo in EveryDayActivePointTemplate)
		{
			if (!dictionary.Keys.Contains(everyDayActivePointTemplateInfo.ID))
			{
				dictionary.Add(everyDayActivePointTemplateInfo.ID, everyDayActivePointTemplateInfo);
			}
		}
		return dictionary;
	}

	public static EveryDayActivePointTemplateInfo FindEveryDayActivePointTemplate(int ID)
	{
		if (m_EveryDayActivePointTemplates.ContainsKey(ID))
		{
			return m_EveryDayActivePointTemplates[ID];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class SubActiveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static Dictionary<int, SubActiveConditionInfo> m_SubActiveConditionInfo = new Dictionary<int, SubActiveConditionInfo>();

	public static Dictionary<int, List<SubActiveInfo>> m_SubActiveInfo = new Dictionary<int, List<SubActiveInfo>>();

	public static SubActiveConditionInfo GetSubActiveInfo(ItemInfo item)
	{
		foreach (List<SubActiveInfo> value in m_SubActiveInfo.Values)
		{
			foreach (SubActiveInfo item2 in value)
			{
				if (!IsValid(item2))
				{
					continue;
				}
				foreach (SubActiveConditionInfo value2 in m_SubActiveConditionInfo.Values)
				{
					if (item2.ActiveID == value2.ActiveID && item2.SubID == value2.SubID && value2.ConditionID == item.TemplateID)
					{
						return value2;
					}
				}
			}
		}
		SubActiveConditionInfo subActiveConditionInfo = null;
		return null;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool IsValid(SubActiveInfo info)
	{
		DateTime startTime = info.StartTime;
		DateTime endTime = info.EndTime;
		return info.StartTime.Ticks <= DateTime.Now.Ticks && info.EndTime.Ticks >= DateTime.Now.Ticks;
	}

	public static Dictionary<int, SubActiveConditionInfo> LoadSubActiveConditionDb(Dictionary<int, List<SubActiveInfo>> conditions)
	{
		Dictionary<int, SubActiveConditionInfo> dictionary = new Dictionary<int, SubActiveConditionInfo>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			foreach (int key in conditions.Keys)
			{
				SubActiveConditionInfo[] allSubActiveCondition = activeBussiness.GetAllSubActiveCondition(key);
				foreach (SubActiveConditionInfo subActiveConditionInfo in allSubActiveCondition)
				{
					if (key == subActiveConditionInfo.ActiveID && !dictionary.ContainsKey(subActiveConditionInfo.ID))
					{
						dictionary.Add(subActiveConditionInfo.ID, subActiveConditionInfo);
					}
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<SubActiveInfo>> LoadSubActiveDb()
	{
		Dictionary<int, List<SubActiveInfo>> dictionary = new Dictionary<int, List<SubActiveInfo>>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			SubActiveInfo[] allSubActive = activeBussiness.GetAllSubActive();
			foreach (SubActiveInfo subActiveInfo in allSubActive)
			{
				List<SubActiveInfo> list = new List<SubActiveInfo>();
				if (!dictionary.ContainsKey(subActiveInfo.ActiveID))
				{
					list.Add(subActiveInfo);
					dictionary.Add(subActiveInfo.ActiveID, list);
				}
				else
				{
					dictionary[subActiveInfo.ActiveID].Add(subActiveInfo);
				}
			}
		}
		return dictionary;
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, List<SubActiveInfo>> dictionary = LoadSubActiveDb();
			Dictionary<int, SubActiveConditionInfo> value = LoadSubActiveConditionDb(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_SubActiveInfo, dictionary);
				Interlocked.Exchange(ref m_SubActiveConditionInfo, value);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("QuestMgr", exception);
		}
		return false;
	}
}

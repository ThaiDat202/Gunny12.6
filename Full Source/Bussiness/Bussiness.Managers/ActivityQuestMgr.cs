using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ActivityQuestMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ActivityQuestInfo> m_ActivityQuestinfo = new Dictionary<int, ActivityQuestInfo>();

	private static Dictionary<int, List<AtivityQuestConditionInfo>> m_ActivityQuestcondiction = new Dictionary<int, List<AtivityQuestConditionInfo>>();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ActivityQuestInfo> dictionary = LoadActivityQuestInfoDb();
			Dictionary<int, List<AtivityQuestConditionInfo>> value = LoadActivityQuestCondictionDb(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_ActivityQuestinfo, dictionary);
				Interlocked.Exchange(ref m_ActivityQuestcondiction, value);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("ActivityQuestMgr", exception);
		}
		return false;
	}

	public static Dictionary<int, ActivityQuestInfo> LoadActivityQuestInfoDb()
	{
		Dictionary<int, ActivityQuestInfo> dictionary = new Dictionary<int, ActivityQuestInfo>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			ActivityQuestInfo[] allActivityQuest = activeBussiness.GetAllActivityQuest();
			ActivityQuestInfo[] array = allActivityQuest;
			foreach (ActivityQuestInfo activityQuestInfo in array)
			{
				if (!dictionary.ContainsKey(activityQuestInfo.ID))
				{
					dictionary.Add(activityQuestInfo.ID, activityQuestInfo);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<AtivityQuestConditionInfo>> LoadActivityQuestCondictionDb(Dictionary<int, ActivityQuestInfo> ActivityQuests)
	{
		Dictionary<int, List<AtivityQuestConditionInfo>> dictionary = new Dictionary<int, List<AtivityQuestConditionInfo>>();
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			AtivityQuestConditionInfo[] allAtivityQuestCondition = activeBussiness.GetAllAtivityQuestCondition();
			foreach (ActivityQuestInfo ActivityQuest in ActivityQuests.Values)
			{
				IEnumerable<AtivityQuestConditionInfo> source = allAtivityQuestCondition.Where((AtivityQuestConditionInfo s) => s.QuestID == ActivityQuest.ID);
				dictionary.Add(ActivityQuest.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static ActivityQuestInfo GetSingleActivityQuest(int id)
	{
		if (m_ActivityQuestinfo.Count == 0)
		{
			Init();
		}
		if (m_ActivityQuestinfo.ContainsKey(id))
		{
			return m_ActivityQuestinfo[id];
		}
		return null;
	}
}

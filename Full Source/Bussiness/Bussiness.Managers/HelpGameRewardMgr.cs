using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class HelpGameRewardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<HelpGameRewardInfo>> m_HelpGameRewards;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			HelpGameRewardInfo[] array = LoadHelpGameRewardDb();
			Dictionary<int, List<HelpGameRewardInfo>> value = LoadHelpGameRewards(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_HelpGameRewards, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad HelpGameReward", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static HelpGameRewardInfo[] LoadHelpGameRewardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllHelpGameReward();
	}

	public static Dictionary<int, List<HelpGameRewardInfo>> LoadHelpGameRewards(HelpGameRewardInfo[] HelpGameReward)
	{
		Dictionary<int, List<HelpGameRewardInfo>> dictionary = new Dictionary<int, List<HelpGameRewardInfo>>();
		foreach (HelpGameRewardInfo info in HelpGameReward)
		{
			if (!dictionary.Keys.Contains(info.MissionID))
			{
				IEnumerable<HelpGameRewardInfo> source = HelpGameReward.Where((HelpGameRewardInfo s) => s.MissionID == info.MissionID);
				dictionary.Add(info.MissionID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<HelpGameRewardInfo> FindHelpGameReward(int missionID)
	{
		if (m_HelpGameRewards.ContainsKey(missionID))
		{
			return m_HelpGameRewards[missionID];
		}
		return null;
	}
}

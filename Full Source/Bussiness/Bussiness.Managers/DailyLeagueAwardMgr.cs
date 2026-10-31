using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class DailyLeagueAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<DailyLeagueAwardInfo>> m_DailyLeagueAwards;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			DailyLeagueAwardInfo[] array = LoadDailyLeagueAwardDb();
			Dictionary<int, List<DailyLeagueAwardInfo>> value = LoadDailyLeagueAwards(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_DailyLeagueAwards, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad DailyLeagueAward", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static DailyLeagueAwardInfo[] LoadDailyLeagueAwardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllDailyLeagueAward();
	}

	public static Dictionary<int, List<DailyLeagueAwardInfo>> LoadDailyLeagueAwards(DailyLeagueAwardInfo[] DailyLeagueAward)
	{
		Dictionary<int, List<DailyLeagueAwardInfo>> dictionary = new Dictionary<int, List<DailyLeagueAwardInfo>>();
		foreach (DailyLeagueAwardInfo info in DailyLeagueAward)
		{
			if (!dictionary.Keys.Contains(info.Class))
			{
				IEnumerable<DailyLeagueAwardInfo> source = DailyLeagueAward.Where((DailyLeagueAwardInfo s) => s.Class == info.Class);
				dictionary.Add(info.Class, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<DailyLeagueAwardInfo> FindDailyLeagueAward(int Class)
	{
		if (m_DailyLeagueAwards.ContainsKey(Class))
		{
			return m_DailyLeagueAwards[Class];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class DiceLevelAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<DiceLevelAwardInfo>> m_diceLevelAwards;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			DiceLevelAwardInfo[] array = LoadDiceLevelAwardDb();
			Dictionary<int, List<DiceLevelAwardInfo>> value = LoadDiceLevelAwards(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_diceLevelAwards, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad DiceLevelAwardMgr", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static DiceLevelAwardInfo[] LoadDiceLevelAwardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetDiceLevelAwardInfos();
	}

	public static Dictionary<int, List<DiceLevelAwardInfo>> LoadDiceLevelAwards(DiceLevelAwardInfo[] DiceLevelAwards)
	{
		Dictionary<int, List<DiceLevelAwardInfo>> dictionary = new Dictionary<int, List<DiceLevelAwardInfo>>();
		foreach (DiceLevelAwardInfo info in DiceLevelAwards)
		{
			if (!dictionary.Keys.Contains(info.DiceLevel))
			{
				IEnumerable<DiceLevelAwardInfo> source = DiceLevelAwards.Where((DiceLevelAwardInfo s) => s.DiceLevel == info.DiceLevel);
				dictionary.Add(info.DiceLevel, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<DiceLevelAwardInfo> FindDiceLevelAward(int DataId)
	{
		if (m_diceLevelAwards.ContainsKey(DataId))
		{
			return m_diceLevelAwards[DataId];
		}
		return null;
	}
}

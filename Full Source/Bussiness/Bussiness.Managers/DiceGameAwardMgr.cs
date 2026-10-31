using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class DiceGameAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<DiceGameAwardInfo>> m_DiceGameAwards;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			DiceGameAwardInfo[] array = LoadDiceGameAwardDb();
			Dictionary<int, List<DiceGameAwardInfo>> value = LoadDiceGameAwards(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_DiceGameAwards, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad DiceGameAward", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static DiceGameAwardInfo[] LoadDiceGameAwardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllDiceGameAward();
	}

	public static Dictionary<int, List<DiceGameAwardInfo>> LoadDiceGameAwards(DiceGameAwardInfo[] DiceGameAward)
	{
		Dictionary<int, List<DiceGameAwardInfo>> dictionary = new Dictionary<int, List<DiceGameAwardInfo>>();
		foreach (DiceGameAwardInfo info in DiceGameAward)
		{
			if (!dictionary.Keys.Contains(info.rank))
			{
				IEnumerable<DiceGameAwardInfo> source = DiceGameAward.Where((DiceGameAwardInfo s) => s.rank == info.rank);
				dictionary.Add(info.rank, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<DiceGameAwardInfo> FindDiceGameAward(int DataId)
	{
		if (m_DiceGameAwards.ContainsKey(DataId))
		{
			return m_DiceGameAwards[DataId];
		}
		return null;
	}
}

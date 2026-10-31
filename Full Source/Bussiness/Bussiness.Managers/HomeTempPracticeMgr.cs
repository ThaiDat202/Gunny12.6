using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using EntityDatabase.ServerModels;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class HomeTempPracticeMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, HomeTempPracticeInfo> m_homeTempPractices = new Dictionary<int, HomeTempPracticeInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	private static ServerData db = new ServerData();

	private static List<TS_HomeTempAdvance_Template> TS_HomeTempAdvance_Template;

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			TS_HomeTempAdvance_Template = db.TS_HomeTempAdvance_Template.ToList();
			Dictionary<int, HomeTempPracticeInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_homeTempPractices, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("HomeTempPracticeMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, HomeTempPracticeInfo> LoadFromDatabase()
	{
		Dictionary<int, HomeTempPracticeInfo> dictionary = new Dictionary<int, HomeTempPracticeInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			HomeTempPracticeInfo[] allHomeTempPractice = produceBussiness.GetAllHomeTempPractice();
			HomeTempPracticeInfo[] array = allHomeTempPractice;
			foreach (HomeTempPracticeInfo homeTempPracticeInfo in array)
			{
				if (!dictionary.ContainsKey(homeTempPracticeInfo.Level))
				{
					dictionary.Add(homeTempPracticeInfo.Level, homeTempPracticeInfo);
				}
			}
		}
		return dictionary;
	}

	public static int MaxLevel()
	{
		return m_homeTempPractices.Count - 1;
	}

	public static int FindSetsBuildExp(int exp)
	{
		List<HomeTempPracticeInfo> allHomeTempPractice = GetAllHomeTempPractice();
		for (int i = 0; i < allHomeTempPractice.Count; i++)
		{
			if (exp <= allHomeTempPractice[i].Exp)
			{
				return allHomeTempPractice[(i - 1 >= 0) ? (i - 1) : 0].Level;
			}
		}
		return 0;
	}

	public static List<HomeTempPracticeInfo> GetAllHomeTempPractice()
	{
		return m_homeTempPractices.Values.ToList();
	}

	public static HomeTempPracticeInfo FindHomeTempPractice(int id)
	{
		if (m_homeTempPractices.ContainsKey(id))
		{
			return m_homeTempPractices[id];
		}
		return null;
	}

	public static void BuildProp(int level, ref int att, ref int def, ref int blood, ref int luck, ref int mgDef)
	{
		HomeTempPracticeInfo homeTempPracticeInfo = FindHomeTempPractice(level);
		if (homeTempPracticeInfo != null)
		{
			att += homeTempPracticeInfo.Attack;
			def += homeTempPracticeInfo.Defence;
			blood += homeTempPracticeInfo.Blood;
			luck += homeTempPracticeInfo.Luck;
			mgDef += homeTempPracticeInfo.MagicDefence;
		}
	}

	public static void BuildProp(int level, ref int blood, ref int mgDef, ref int tenacity, ref int avoidInjury, ref int trcRvt, ref int guardian)
	{
		TS_HomeTempAdvance_Template tS_HomeTempAdvance_Template = TS_HomeTempAdvance_Template.FirstOrDefault((TS_HomeTempAdvance_Template p) => p.Level == level);
		if (tS_HomeTempAdvance_Template != null)
		{
			blood += tS_HomeTempAdvance_Template.Blood;
			mgDef += tS_HomeTempAdvance_Template.MagicDefend;
			tenacity += tS_HomeTempAdvance_Template.Toughness;
			avoidInjury += tS_HomeTempAdvance_Template.AvoidInjury;
			trcRvt += tS_HomeTempAdvance_Template.TricRevolt;
			guardian += tS_HomeTempAdvance_Template.Guardian;
		}
	}

	public static void BuildProp(int level, ref int blood, ref int mgDef)
	{
		TS_HomeTempAdvance_Template tS_HomeTempAdvance_Template = TS_HomeTempAdvance_Template.FirstOrDefault((TS_HomeTempAdvance_Template p) => p.Level == level);
		if (tS_HomeTempAdvance_Template != null)
		{
			blood += tS_HomeTempAdvance_Template.Blood;
			mgDef += tS_HomeTempAdvance_Template.MagicDefend;
		}
	}

	public static void BuildProp(int level, ref double guard)
	{
		HomeTempPracticeInfo homeTempPracticeInfo = FindHomeTempPractice(level);
		if (homeTempPracticeInfo != null)
		{
			guard += homeTempPracticeInfo.Guard;
		}
	}
}

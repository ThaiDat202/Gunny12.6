using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class FairBattleRewardMgr
{
	private static readonly ILog ilog_0;

	private static Dictionary<int, FairBattleRewardInfo> dictionary_0;

	private static Random random_0;

	static FairBattleRewardMgr()
	{
		ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
	}

	public static FairBattleRewardInfo FindLevel(int Level)
	{
		try
		{
			if (dictionary_0.ContainsKey(Level))
			{
				return dictionary_0[Level];
			}
		}
		catch
		{
		}
		return null;
	}

	public static FairBattleRewardInfo GetBattleDataByPrestige(int Prestige)
	{
		for (int num = dictionary_0.Values.Count - 1; num >= 0; num--)
		{
			if (Prestige >= dictionary_0[num].Prestige)
			{
				return dictionary_0[num];
			}
		}
		return null;
	}

	public static int GetGP(int level)
	{
		if (MaxLevel() <= level || level <= 0)
		{
			return 0;
		}
		return FindLevel(level - 1).Prestige;
	}

	public static int GetLevel(int GP)
	{
		if (GP >= FindLevel(MaxLevel()).Prestige)
		{
			return MaxLevel();
		}
		for (int i = 1; i <= MaxLevel(); i++)
		{
			if (GP < FindLevel(i).Prestige)
			{
				if (i - 1 == 0)
				{
					return 1;
				}
				return i - 1;
			}
		}
		return 1;
	}

	public static bool Init()
	{
		try
		{
			dictionary_0 = new Dictionary<int, FairBattleRewardInfo>();
			random_0 = new Random();
			return smethod_0(dictionary_0);
		}
		catch (Exception ex)
		{
			Exception exception = ex;
			if (ilog_0.IsErrorEnabled)
			{
				ilog_0.Error("FairBattleRewardMgr", exception);
			}
			return false;
		}
	}

	public static int MaxLevel()
	{
		if (dictionary_0 == null)
		{
			Init();
		}
		return dictionary_0.Values.Count;
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, FairBattleRewardInfo> aorpNqAKIug0IVviOi = new Dictionary<int, FairBattleRewardInfo>();
			if (smethod_0(aorpNqAKIug0IVviOi))
			{
				try
				{
					dictionary_0 = aorpNqAKIug0IVviOi;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception ex)
		{
			Exception exception = ex;
			if (ilog_0.IsErrorEnabled)
			{
				ilog_0.Error("FairBattleMgr", exception);
			}
		}
		return false;
	}

	private static bool smethod_0(Dictionary<int, FairBattleRewardInfo> aorpNqAKIug0IVviOi)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			FairBattleRewardInfo[] allFairBattleReward = produceBussiness.GetAllFairBattleReward();
			foreach (FairBattleRewardInfo fairBattleRewardInfo in allFairBattleReward)
			{
				if (!aorpNqAKIug0IVviOi.ContainsKey(fairBattleRewardInfo.Level))
				{
					aorpNqAKIug0IVviOi.Add(fairBattleRewardInfo.Level, fairBattleRewardInfo);
				}
			}
		}
		return true;
	}
}

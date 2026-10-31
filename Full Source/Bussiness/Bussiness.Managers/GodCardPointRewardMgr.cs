using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class GodCardPointRewardMgr
{
	private static readonly ILog log;

	private static Dictionary<int, GodCardPointRewardInfo> dictionary_0;

	private static ThreadSafeRandom threadSafeRandom_0;

	static GodCardPointRewardMgr()
	{
		log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		dictionary_0 = new Dictionary<int, GodCardPointRewardInfo>();
		threadSafeRandom_0 = new ThreadSafeRandom();
	}

	public static GodCardPointRewardInfo FindGodCardPointReward(int id)
	{
		if (dictionary_0.Count == 0)
		{
			ReLoad();
		}
		if (!dictionary_0.ContainsKey(id))
		{
			return null;
		}
		return dictionary_0[id];
	}

	public static List<GodCardPointRewardInfo> GetAllGodCardPointReward()
	{
		if (dictionary_0.Count == 0)
		{
			ReLoad();
		}
		return dictionary_0.Values.ToList();
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, GodCardPointRewardInfo> dictionary = smethod_0();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref dictionary_0, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("GodCardPointRewardMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, GodCardPointRewardInfo> smethod_0()
	{
		Dictionary<int, GodCardPointRewardInfo> dictionary = new Dictionary<int, GodCardPointRewardInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GodCardPointRewardInfo[] allGodCardPointReward = produceBussiness.GetAllGodCardPointReward();
			foreach (GodCardPointRewardInfo godCardPointRewardInfo in allGodCardPointReward)
			{
				if (!dictionary.ContainsKey(godCardPointRewardInfo.ID))
				{
					dictionary.Add(godCardPointRewardInfo.ID, godCardPointRewardInfo);
				}
			}
		}
		return dictionary;
	}
}

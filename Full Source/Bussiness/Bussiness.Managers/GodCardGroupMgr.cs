using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class GodCardGroupMgr
{
	private static readonly ILog log;

	private static Dictionary<int, GodCardGroupInfo> dictionary_0;

	private static Dictionary<int, List<GodCardGroupDetailInfo>> dictionary_1;

	private static ThreadSafeRandom threadSafeRandom_0;

	static GodCardGroupMgr()
	{
		log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		dictionary_0 = new Dictionary<int, GodCardGroupInfo>();
		dictionary_1 = new Dictionary<int, List<GodCardGroupDetailInfo>>();
		threadSafeRandom_0 = new ThreadSafeRandom();
	}

	private static Dictionary<int, GodCardGroupInfo> DyUsiesBv0()
	{
		Dictionary<int, GodCardGroupInfo> dictionary = new Dictionary<int, GodCardGroupInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GodCardGroupInfo[] allGodCardGroup = produceBussiness.GetAllGodCardGroup();
			foreach (GodCardGroupInfo godCardGroupInfo in allGodCardGroup)
			{
				if (!dictionary.ContainsKey(godCardGroupInfo.GroupID))
				{
					dictionary.Add(godCardGroupInfo.GroupID, godCardGroupInfo);
				}
			}
		}
		return dictionary;
	}

	public static GodCardGroupInfo FindGodCardGroup(int id)
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

	public static List<GodCardGroupDetailInfo> FindGodCardGroupDetail(int id)
	{
		if (!dictionary_1.ContainsKey(id))
		{
			return new List<GodCardGroupDetailInfo>();
		}
		return dictionary_1[id];
	}

	public static List<GodCardGroupInfo> GetAllGodCardGroup()
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

	public static Dictionary<int, List<GodCardGroupDetailInfo>> LoadCardGroupDetailDb(Dictionary<int, GodCardGroupInfo> godCardGroupInfos)
	{
		Dictionary<int, List<GodCardGroupDetailInfo>> dictionary = new Dictionary<int, List<GodCardGroupDetailInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GodCardGroupDetailInfo[] allGodCardGroupDetail = produceBussiness.GetAllGodCardGroupDetail();
			foreach (GodCardGroupInfo value in godCardGroupInfos.Values)
			{
				IEnumerable<GodCardGroupDetailInfo> source = allGodCardGroupDetail.Where((GodCardGroupDetailInfo s) => s.GroupID == value.GroupID);
				dictionary.Add(value.GroupID, source.ToList());
			}
		}
		return dictionary;
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, GodCardGroupInfo> dictionary = DyUsiesBv0();
			Dictionary<int, List<GodCardGroupDetailInfo>> value = LoadCardGroupDetailDb(dictionary);
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref dictionary_0, dictionary);
				Interlocked.Exchange(ref dictionary_1, value);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("GodCardGroupMgr init error:", exception);
		}
		return false;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class CommunalActiveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, CommunalActiveInfo> m_communalActives = new Dictionary<int, CommunalActiveInfo>();

	private static Dictionary<int, List<CommunalActiveExpInfo>> m_CommunalActiveExps = new Dictionary<int, List<CommunalActiveExpInfo>>();

	private static Dictionary<int, List<CommunalActiveAwardInfo>> m_CommunalActiveAwards = new Dictionary<int, List<CommunalActiveAwardInfo>>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			CommunalActiveInfo[] array = LoadCommunalActiveDb();
			Dictionary<int, CommunalActiveInfo> value = LoadCommunalActives(array);
			Dictionary<int, List<CommunalActiveExpInfo>> value2 = LoadCommunalActiveExps(array);
			Dictionary<int, List<CommunalActiveAwardInfo>> value3 = LoadCommunalActiveAwards(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_communalActives, value);
				Interlocked.Exchange(ref m_CommunalActiveExps, value2);
				Interlocked.Exchange(ref m_CommunalActiveAwards, value3);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad CommunalActive", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static Dictionary<int, List<CommunalActiveAwardInfo>> LoadCommunalActiveAwards(CommunalActiveInfo[] CommunalActives)
	{
		Dictionary<int, List<CommunalActiveAwardInfo>> dictionary = new Dictionary<int, List<CommunalActiveAwardInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			CommunalActiveAwardInfo[] allCommunalActiveAward = produceBussiness.GetAllCommunalActiveAward();
			foreach (CommunalActiveInfo info in CommunalActives)
			{
				if (!dictionary.Keys.Contains(info.ActiveID))
				{
					IEnumerable<CommunalActiveAwardInfo> source = allCommunalActiveAward.Where((CommunalActiveAwardInfo s) => s.ActiveID == info.ActiveID);
					dictionary.Add(info.ActiveID, source.ToList());
				}
			}
		}
		return dictionary;
	}

	public static List<CommunalActiveAwardInfo> FindCommunalActiveAward(int ActiveID)
	{
		if (m_CommunalActiveAwards.ContainsKey(ActiveID))
		{
			return m_CommunalActiveAwards[ActiveID];
		}
		return null;
	}

	public static Dictionary<int, List<CommunalActiveExpInfo>> LoadCommunalActiveExps(CommunalActiveInfo[] CommunalActives)
	{
		Dictionary<int, List<CommunalActiveExpInfo>> dictionary = new Dictionary<int, List<CommunalActiveExpInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			CommunalActiveExpInfo[] allCommunalActiveExp = produceBussiness.GetAllCommunalActiveExp();
			foreach (CommunalActiveInfo info in CommunalActives)
			{
				if (!dictionary.Keys.Contains(info.ActiveID))
				{
					IEnumerable<CommunalActiveExpInfo> source = allCommunalActiveExp.Where((CommunalActiveExpInfo s) => s.ActiveID == info.ActiveID);
					dictionary.Add(info.ActiveID, source.ToList());
				}
			}
		}
		return dictionary;
	}

	public static List<CommunalActiveExpInfo> FindCommunalActiveExp(int ActiveID)
	{
		if (m_CommunalActiveExps.ContainsKey(ActiveID))
		{
			return m_CommunalActiveExps[ActiveID];
		}
		return null;
	}

	public static CommunalActiveInfo[] LoadCommunalActiveDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllCommunalActive();
	}

	public static Dictionary<int, CommunalActiveInfo> LoadCommunalActives(CommunalActiveInfo[] CommunalActive)
	{
		Dictionary<int, CommunalActiveInfo> dictionary = new Dictionary<int, CommunalActiveInfo>();
		foreach (CommunalActiveInfo communalActiveInfo in CommunalActive)
		{
			if (!dictionary.Keys.Contains(communalActiveInfo.ActiveID))
			{
				dictionary.Add(communalActiveInfo.ActiveID, communalActiveInfo);
			}
		}
		return dictionary;
	}

	public static CommunalActiveInfo FindCommunalActive(int ActiveID)
	{
		if (m_communalActives.ContainsKey(ActiveID))
		{
			return m_communalActives[ActiveID];
		}
		return null;
	}

	public static CommunalActiveInfo[] GetAllCommunalActives()
	{
		return m_communalActives.Values.ToArray();
	}
}

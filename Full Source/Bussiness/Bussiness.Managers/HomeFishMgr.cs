using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class HomeFishMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, HomeFishInfo> m_HomeFishs = new Dictionary<int, HomeFishInfo>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			HomeFishInfo[] array = LoadHomeFishDb();
			Dictionary<int, HomeFishInfo> value = LoadHomeFishs(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_HomeFishs, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad HomeFish", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static HomeFishInfo[] LoadHomeFishDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllHomeFish();
	}

	public static Dictionary<int, HomeFishInfo> LoadHomeFishs(HomeFishInfo[] HomeFish)
	{
		Dictionary<int, HomeFishInfo> dictionary = new Dictionary<int, HomeFishInfo>();
		foreach (HomeFishInfo homeFishInfo in HomeFish)
		{
			if (!dictionary.Keys.Contains(homeFishInfo.ID))
			{
				dictionary.Add(homeFishInfo.ID, homeFishInfo);
			}
		}
		return dictionary;
	}

	public static HomeFishInfo FindHomeFish(int ID)
	{
		if (m_HomeFishs.ContainsKey(ID))
		{
			return m_HomeFishs[ID];
		}
		return null;
	}

	public static List<HomeFishInfo> GetAllHomeFish()
	{
		return m_HomeFishs.Values.ToList();
	}
}

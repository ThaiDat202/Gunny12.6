using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class LoveLevelMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, LoveLevelInfo> m_loveLevels = new Dictionary<int, LoveLevelInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static int MaxLevel => m_loveLevels.Count;

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, LoveLevelInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_loveLevels, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("LoveLevelMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, LoveLevelInfo> LoadFromDatabase()
	{
		Dictionary<int, LoveLevelInfo> dictionary = new Dictionary<int, LoveLevelInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			LoveLevelInfo[] allLoveLevel = produceBussiness.GetAllLoveLevel();
			LoveLevelInfo[] array = allLoveLevel;
			foreach (LoveLevelInfo loveLevelInfo in array)
			{
				if (!dictionary.ContainsKey(loveLevelInfo.Level))
				{
					dictionary.Add(loveLevelInfo.Level, loveLevelInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<LoveLevelInfo> GetAllLoveLevel()
	{
		if (m_loveLevels.Count == 0)
		{
			Init();
		}
		return m_loveLevels.Values.ToList();
	}

	public static LoveLevelInfo FindLoveLevel(int lv)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_loveLevels.ContainsKey(lv))
			{
				return m_loveLevels[lv];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static LoveLevelInfo GetLevel(int GP)
	{
		LoveLevelInfo loveLevelInfo = FindLoveLevel(MaxLevel);
		if (loveLevelInfo == null)
		{
			return null;
		}
		if (GP >= loveLevelInfo.Exp)
		{
			return loveLevelInfo;
		}
		for (int i = 1; i <= MaxLevel; i++)
		{
			loveLevelInfo = FindLoveLevel(i);
			if (loveLevelInfo != null && GP < loveLevelInfo.Exp)
			{
				return loveLevelInfo;
			}
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public static class PveInfoMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, PveInfo> m_pveInfos = new Dictionary<int, PveInfo>();

	private static ReaderWriterLock m_lock = new ReaderWriterLock();

	private static Random m_rand = new Random();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, PveInfo> dictionary = LoadFromDatabase();
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_pveInfos, dictionary);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("PveInfoMgr", exception);
		}
		return false;
	}

	public static Dictionary<int, PveInfo> LoadFromDatabase()
	{
		Dictionary<int, PveInfo> dictionary = new Dictionary<int, PveInfo>();
		using (PveBussiness pveBussiness = new PveBussiness())
		{
			PveInfo[] allPveInfos = pveBussiness.GetAllPveInfos();
			PveInfo[] array = allPveInfos;
			foreach (PveInfo pveInfo in array)
			{
				if (!dictionary.ContainsKey(pveInfo.ID))
				{
					dictionary.Add(pveInfo.ID, pveInfo);
				}
			}
		}
		return dictionary;
	}

	public static PveInfo GetPveInfoById(int id)
	{
		if (m_pveInfos.ContainsKey(id))
		{
			return m_pveInfos[id];
		}
		return null;
	}

	public static PveInfo[] GetPveInfo()
	{
		if (m_pveInfos == null)
		{
			Init();
		}
		return m_pveInfos.Values.ToArray();
	}

	public static PveInfo GetPveInfoByType(eRoomType roomType, int levelLimits)
	{
		switch (roomType)
		{
		case eRoomType.Dungeon:
		case eRoomType.FightLib:
		case eRoomType.Freshman:
		case eRoomType.AcademyDungeon:
		case eRoomType.Lanbyrinth:
		case eRoomType.ConsortiaBoss:
		case eRoomType.ActivityDungeon:
		case eRoomType.SpecialActivityDungeon:
		case eRoomType.Element_PVE:
			foreach (PveInfo value in m_pveInfos.Values)
			{
				if (value.Type == (int)roomType)
				{
					return value;
				}
			}
			break;
		case eRoomType.Exploration:
			foreach (PveInfo value2 in m_pveInfos.Values)
			{
				if (value2.Type == (int)roomType && value2.LevelLimits == levelLimits)
				{
					return value2;
				}
			}
			break;
		}
		return null;
	}

	public static PveInfo GetRandomPve()
	{
		List<PveInfo> list = m_pveInfos.Values.Where((PveInfo x) => x.Type == 4).ToList();
		return list[m_rand.Next(0, list.Count())];
	}
}

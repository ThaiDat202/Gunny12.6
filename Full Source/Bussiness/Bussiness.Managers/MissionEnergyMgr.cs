using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public static class MissionEnergyMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MissionEnergyInfo> m_missionEnergyInfos = new Dictionary<int, MissionEnergyInfo>();

	private static ReaderWriterLock m_lock = new ReaderWriterLock();

	private static Random m_rand = new Random();

	public static bool Init()
	{
		return Reload();
	}

	public static bool Reload()
	{
		try
		{
			Dictionary<int, MissionEnergyInfo> dictionary = LoadFromDatabase();
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_missionEnergyInfos, dictionary);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("MissionEnergyInfoMgr", exception);
		}
		return false;
	}

	private static Dictionary<int, MissionEnergyInfo> LoadFromDatabase()
	{
		Dictionary<int, MissionEnergyInfo> dictionary = new Dictionary<int, MissionEnergyInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MissionEnergyInfo[] allMissionEnergyInfo = produceBussiness.GetAllMissionEnergyInfo();
			MissionEnergyInfo[] array = allMissionEnergyInfo;
			foreach (MissionEnergyInfo missionEnergyInfo in array)
			{
				if (!dictionary.ContainsKey(missionEnergyInfo.Count))
				{
					dictionary.Add(missionEnergyInfo.Count, missionEnergyInfo);
				}
			}
		}
		return dictionary;
	}

	public static MissionEnergyInfo GetMissionEnergyInfo(int id)
	{
		if (m_missionEnergyInfos.ContainsKey(id))
		{
			return m_missionEnergyInfos[id];
		}
		return null;
	}
}

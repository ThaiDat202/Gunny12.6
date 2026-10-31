using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class MountTalismansMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MountTalismansInfo> m_mountTalismanss = new Dictionary<int, MountTalismansInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, MountTalismansInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_mountTalismanss, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("MountTalismansMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, MountTalismansInfo> LoadFromDatabase()
	{
		Dictionary<int, MountTalismansInfo> dictionary = new Dictionary<int, MountTalismansInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MountTalismansInfo[] allMountTalismans = produceBussiness.GetAllMountTalismans();
			MountTalismansInfo[] array = allMountTalismans;
			foreach (MountTalismansInfo mountTalismansInfo in array)
			{
				if (!dictionary.ContainsKey(mountTalismansInfo.ID))
				{
					dictionary.Add(mountTalismansInfo.ID, mountTalismansInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<MountTalismansInfo> GetAllMountTalismans()
	{
		if (m_mountTalismanss.Count == 0)
		{
			Init();
		}
		return m_mountTalismanss.Values.ToList();
	}

	public static MountTalismansInfo FindMountTalismans(int id)
	{
		if (m_mountTalismanss.Count == 0)
		{
			Init();
		}
		if (m_mountTalismanss.ContainsKey(id))
		{
			return m_mountTalismanss[id];
		}
		return null;
	}
}

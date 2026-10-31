using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class PetBreakMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, PetBreakInfo> m_petBreaks = new Dictionary<int, PetBreakInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, PetBreakInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_petBreaks, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("PetBreakMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, PetBreakInfo> LoadFromDatabase()
	{
		Dictionary<int, PetBreakInfo> dictionary = new Dictionary<int, PetBreakInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			PetBreakInfo[] allPetBreak = produceBussiness.GetAllPetBreak();
			PetBreakInfo[] array = allPetBreak;
			foreach (PetBreakInfo petBreakInfo in array)
			{
				if (!dictionary.ContainsKey(petBreakInfo.BreakGrade))
				{
					dictionary.Add(petBreakInfo.BreakGrade, petBreakInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<PetBreakInfo> GetAllPetBreak()
	{
		if (m_petBreaks.Count == 0)
		{
			Init();
		}
		return m_petBreaks.Values.ToList();
	}

	public static PetBreakInfo FindPetBreak(int id)
	{
		if (m_petBreaks.Count == 0)
		{
			Init();
		}
		if (m_petBreaks.ContainsKey(id))
		{
			return m_petBreaks[id];
		}
		return null;
	}
}

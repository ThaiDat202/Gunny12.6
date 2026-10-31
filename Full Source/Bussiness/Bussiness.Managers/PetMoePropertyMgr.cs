using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class PetMoePropertyMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, PetMoePropertyInfo> m_PetMoePropertys = new Dictionary<int, PetMoePropertyInfo>();

	private static Random random = new Random();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			PetMoePropertyInfo[] array = LoadPetMoePropertyDb();
			Dictionary<int, PetMoePropertyInfo> value = LoadPetMoePropertys(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_PetMoePropertys, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad PetMoeProperty", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static PetMoePropertyInfo[] LoadPetMoePropertyDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllPetMoeProperty();
	}

	public static Dictionary<int, PetMoePropertyInfo> LoadPetMoePropertys(PetMoePropertyInfo[] PetMoeProperty)
	{
		Dictionary<int, PetMoePropertyInfo> dictionary = new Dictionary<int, PetMoePropertyInfo>();
		foreach (PetMoePropertyInfo petMoePropertyInfo in PetMoeProperty)
		{
			if (!dictionary.Keys.Contains(petMoePropertyInfo.Level))
			{
				dictionary.Add(petMoePropertyInfo.Level, petMoePropertyInfo);
			}
		}
		return dictionary;
	}

	public static PetMoePropertyInfo FindPetMoeProperty(int Level)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_PetMoePropertys.ContainsKey(Level))
			{
				return m_PetMoePropertys[Level];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static int FindMaxLevel()
	{
		return m_PetMoePropertys.Count;
	}

	public static PetMoePropertyInfo FindPetMoePropertyByGp(int exp)
	{
		PetMoePropertyInfo petMoePropertyInfo = FindPetMoeProperty(FindMaxLevel());
		if (petMoePropertyInfo != null && exp >= petMoePropertyInfo.Exp)
		{
			return petMoePropertyInfo;
		}
		for (int i = 1; i <= m_PetMoePropertys.Count; i++)
		{
			if (m_PetMoePropertys.ContainsKey(i) && exp < m_PetMoePropertys[i].Exp)
			{
				return (i == 1) ? null : m_PetMoePropertys[i - 1];
			}
		}
		return null;
	}
}

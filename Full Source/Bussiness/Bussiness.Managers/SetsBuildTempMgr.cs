using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class SetsBuildTempMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, SetsBuildTempInfo> m_setsBuildTemps = new Dictionary<int, SetsBuildTempInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, SetsBuildTempInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_setsBuildTemps, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("SetsBuildTempMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, SetsBuildTempInfo> LoadFromDatabase()
	{
		Dictionary<int, SetsBuildTempInfo> dictionary = new Dictionary<int, SetsBuildTempInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			SetsBuildTempInfo[] allSetsBuildTemp = produceBussiness.GetAllSetsBuildTemp();
			SetsBuildTempInfo[] array = allSetsBuildTemp;
			foreach (SetsBuildTempInfo setsBuildTempInfo in array)
			{
				if (!dictionary.ContainsKey(setsBuildTempInfo.Level))
				{
					dictionary.Add(setsBuildTempInfo.Level, setsBuildTempInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<SetsBuildTempInfo> GetAllSetsBuildTemp()
	{
		if (m_setsBuildTemps.Count == 0)
		{
			Init();
		}
		return m_setsBuildTemps.Values.ToList();
	}

	public static SetsBuildTempInfo FindSetsBuildTemp(int id)
	{
		if (m_setsBuildTemps.Count == 0)
		{
			Init();
		}
		if (m_setsBuildTemps.ContainsKey(id))
		{
			return m_setsBuildTemps[id];
		}
		return null;
	}

	public static int SetsBuildMax()
	{
		int count = m_setsBuildTemps.Count;
		return m_setsBuildTemps[count].Exp;
	}

	public static int SetsBuildMaxByType(int type)
	{
		return m_setsBuildTemps.Values.Where((SetsBuildTempInfo t) => t.SetsType == type).Max((SetsBuildTempInfo t) => t.Exp);
	}

	public static SetsBuildTempInfo FindSetsBuildExp(int exp)
	{
		List<SetsBuildTempInfo> allSetsBuildTemp = GetAllSetsBuildTemp();
		SetsBuildTempInfo setsBuildTempInfo = null;
		for (int i = 0; i < allSetsBuildTemp.Count; i++)
		{
			setsBuildTempInfo = allSetsBuildTemp[i];
			if (exp <= setsBuildTempInfo.Exp)
			{
				return allSetsBuildTemp[(i - 1 >= 0) ? (i - 1) : 0];
			}
		}
		return setsBuildTempInfo;
	}

	public static SetsBuildTempInfo FindNextSetsBuildExp(int exp)
	{
		List<SetsBuildTempInfo> allSetsBuildTemp = GetAllSetsBuildTemp();
		SetsBuildTempInfo setsBuildTempInfo = null;
		for (int i = 0; i < allSetsBuildTemp.Count; i++)
		{
			setsBuildTempInfo = allSetsBuildTemp[i];
			if (exp < setsBuildTempInfo.Exp)
			{
				return setsBuildTempInfo;
			}
		}
		return setsBuildTempInfo;
	}

	public static void GetSetsBuildProp(int exp, ref int def, ref int blood, ref int luck, ref int agi, ref int mgDef)
	{
		List<SetsBuildTempInfo> allSetsBuildTemp = GetAllSetsBuildTemp();
		SetsBuildTempInfo setsBuildTempInfo = null;
		for (int i = 0; i < allSetsBuildTemp.Count; i++)
		{
			setsBuildTempInfo = allSetsBuildTemp[i];
			if (setsBuildTempInfo != null && exp >= setsBuildTempInfo.Exp)
			{
				def += setsBuildTempInfo.DefenceGrow;
				blood += setsBuildTempInfo.BloodGrow;
				luck += setsBuildTempInfo.LuckGrow;
				agi += setsBuildTempInfo.AgilityGrow;
				mgDef += setsBuildTempInfo.MagicDefenceGrow;
			}
		}
	}

	public static void GetSetsBuildProp(int exp, ref double guard)
	{
		List<SetsBuildTempInfo> allSetsBuildTemp = GetAllSetsBuildTemp();
		SetsBuildTempInfo setsBuildTempInfo = null;
		for (int i = 0; i < allSetsBuildTemp.Count; i++)
		{
			setsBuildTempInfo = allSetsBuildTemp[i];
			if (setsBuildTempInfo != null && exp >= setsBuildTempInfo.Exp)
			{
				guard += setsBuildTempInfo.GuardGrow;
			}
		}
	}
}

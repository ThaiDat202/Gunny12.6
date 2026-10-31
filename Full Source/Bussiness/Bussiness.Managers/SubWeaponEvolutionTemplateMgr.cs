using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class SubWeaponEvolutionTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, SubWeaponEvolutionTemplateInfo> m_subWeaponEvolutionTemplates = new Dictionary<int, SubWeaponEvolutionTemplateInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, SubWeaponEvolutionTemplateInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_subWeaponEvolutionTemplates, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("SubWeaponEvolutionTemplateMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, SubWeaponEvolutionTemplateInfo> LoadFromDatabase()
	{
		Dictionary<int, SubWeaponEvolutionTemplateInfo> dictionary = new Dictionary<int, SubWeaponEvolutionTemplateInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			SubWeaponEvolutionTemplateInfo[] allSubWeaponEvolutionTemplate = produceBussiness.GetAllSubWeaponEvolutionTemplate();
			SubWeaponEvolutionTemplateInfo[] array = allSubWeaponEvolutionTemplate;
			foreach (SubWeaponEvolutionTemplateInfo subWeaponEvolutionTemplateInfo in array)
			{
				if (!dictionary.ContainsKey(subWeaponEvolutionTemplateInfo.Level))
				{
					dictionary.Add(subWeaponEvolutionTemplateInfo.Level, subWeaponEvolutionTemplateInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<SubWeaponEvolutionTemplateInfo> GetAllSubWeaponEvolutionTemplate()
	{
		if (m_subWeaponEvolutionTemplates.Count == 0)
		{
			Init();
		}
		return m_subWeaponEvolutionTemplates.Values.ToList();
	}

	public static SubWeaponEvolutionTemplateInfo FindSubWeaponEvolutionTemplate(int id)
	{
		if (m_subWeaponEvolutionTemplates.Count == 0)
		{
			Init();
		}
		if (m_subWeaponEvolutionTemplates.ContainsKey(id))
		{
			return m_subWeaponEvolutionTemplates[id];
		}
		return null;
	}

	public static SubWeaponEvolutionTemplateInfo FindSubWeaponEvolutionByGp(int exp)
	{
		SubWeaponEvolutionTemplateInfo subWeaponEvolutionTemplateInfo = FindSubWeaponEvolutionTemplate(20);
		if (subWeaponEvolutionTemplateInfo != null && exp >= subWeaponEvolutionTemplateInfo.Exp)
		{
			return subWeaponEvolutionTemplateInfo;
		}
		for (int i = 1; i <= m_subWeaponEvolutionTemplates.Count; i++)
		{
			if (m_subWeaponEvolutionTemplates.ContainsKey(i) && exp < m_subWeaponEvolutionTemplates[i].Exp)
			{
				return (i == 1) ? null : m_subWeaponEvolutionTemplates[i - 1];
			}
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class GuardCoreMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, GuardCoreTemplateInfo> m_guardCoreTemplates = new Dictionary<int, GuardCoreTemplateInfo>();

	private static Dictionary<int, GuardCoreLevelTemplateInfo> m_guardCoreLevelTemplates = new Dictionary<int, GuardCoreLevelTemplateInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, GuardCoreTemplateInfo> dictionary = LoadFromDatabase();
			Dictionary<int, GuardCoreLevelTemplateInfo> dictionary2 = LoadFromDatabase2();
			if (dictionary.Values.Count > 0 && dictionary2.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_guardCoreTemplates, dictionary);
				Interlocked.Exchange(ref m_guardCoreLevelTemplates, dictionary2);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("GuardCoreTemplateMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, GuardCoreLevelTemplateInfo> LoadFromDatabase2()
	{
		Dictionary<int, GuardCoreLevelTemplateInfo> dictionary = new Dictionary<int, GuardCoreLevelTemplateInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GuardCoreLevelTemplateInfo[] allGuardCoreLevelTemplate = produceBussiness.GetAllGuardCoreLevelTemplate();
			GuardCoreLevelTemplateInfo[] array = allGuardCoreLevelTemplate;
			foreach (GuardCoreLevelTemplateInfo guardCoreLevelTemplateInfo in array)
			{
				if (!dictionary.ContainsKey(guardCoreLevelTemplateInfo.Grade))
				{
					dictionary.Add(guardCoreLevelTemplateInfo.Grade, guardCoreLevelTemplateInfo);
				}
			}
		}
		return dictionary;
	}

	private static Dictionary<int, GuardCoreTemplateInfo> LoadFromDatabase()
	{
		Dictionary<int, GuardCoreTemplateInfo> dictionary = new Dictionary<int, GuardCoreTemplateInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GuardCoreTemplateInfo[] allGuardCoreTemplate = produceBussiness.GetAllGuardCoreTemplate();
			GuardCoreTemplateInfo[] array = allGuardCoreTemplate;
			foreach (GuardCoreTemplateInfo guardCoreTemplateInfo in array)
			{
				if (!dictionary.ContainsKey(guardCoreTemplateInfo.ID))
				{
					dictionary.Add(guardCoreTemplateInfo.ID, guardCoreTemplateInfo);
				}
			}
		}
		return dictionary;
	}

	public static int MaxLevel()
	{
		return m_guardCoreLevelTemplates.Count;
	}

	public static List<GuardCoreLevelTemplateInfo> GetAllGuardCoreLevelTemplate()
	{
		return m_guardCoreLevelTemplates.Values.ToList();
	}

	public static GuardCoreLevelTemplateInfo FindGuardCoreLevelTemplate(int id)
	{
		if (m_guardCoreLevelTemplates.ContainsKey(id))
		{
			return m_guardCoreLevelTemplates[id];
		}
		return null;
	}

	public static List<GuardCoreTemplateInfo> GetAllGuardCoreTemplate()
	{
		return m_guardCoreTemplates.Values.ToList();
	}

	public static GuardCoreTemplateInfo FindGuardCoreTemplate(int id)
	{
		if (m_guardCoreTemplates.ContainsKey(id))
		{
			return m_guardCoreTemplates[id];
		}
		return null;
	}
}

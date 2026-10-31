using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class MaxLevelTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MaxLevelTemplateInfo> m_maxLevelTemplates = new Dictionary<int, MaxLevelTemplateInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, MaxLevelTemplateInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_maxLevelTemplates, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("MaxLevelTemplateMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, MaxLevelTemplateInfo> LoadFromDatabase()
	{
		Dictionary<int, MaxLevelTemplateInfo> dictionary = new Dictionary<int, MaxLevelTemplateInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MaxLevelTemplateInfo[] allMaxLevelTemplate = produceBussiness.GetAllMaxLevelTemplate();
			MaxLevelTemplateInfo[] array = allMaxLevelTemplate;
			foreach (MaxLevelTemplateInfo maxLevelTemplateInfo in array)
			{
				if (!dictionary.ContainsKey(maxLevelTemplateInfo.Level))
				{
					dictionary.Add(maxLevelTemplateInfo.Level, maxLevelTemplateInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<MaxLevelTemplateInfo> GetAllMaxLevelTemplate()
	{
		if (m_maxLevelTemplates.Count == 0)
		{
			Init();
		}
		return m_maxLevelTemplates.Values.ToList();
	}

	public static int MaxLevel()
	{
		return m_maxLevelTemplates.Count;
	}

	public static MaxLevelTemplateInfo FindMaxLevelTemplate(int id)
	{
		if (m_maxLevelTemplates.Count == 0)
		{
			Init();
		}
		if (m_maxLevelTemplates.ContainsKey(id))
		{
			return m_maxLevelTemplates[id];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class RuneAdvanceTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, RuneAdvanceTemplateInfo> m_runeAdvanceTemplates = new Dictionary<int, RuneAdvanceTemplateInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, RuneAdvanceTemplateInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_runeAdvanceTemplates, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("RuneAdvanceTemplateMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, RuneAdvanceTemplateInfo> LoadFromDatabase()
	{
		Dictionary<int, RuneAdvanceTemplateInfo> dictionary = new Dictionary<int, RuneAdvanceTemplateInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			RuneAdvanceTemplateInfo[] allRuneAdvanceTemplate = produceBussiness.GetAllRuneAdvanceTemplate();
			RuneAdvanceTemplateInfo[] array = allRuneAdvanceTemplate;
			foreach (RuneAdvanceTemplateInfo runeAdvanceTemplateInfo in array)
			{
				if (!dictionary.ContainsKey(runeAdvanceTemplateInfo.AdvancedTempId))
				{
					dictionary.Add(runeAdvanceTemplateInfo.AdvancedTempId, runeAdvanceTemplateInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<RuneAdvanceTemplateInfo> GetAllRuneAdvanceTemplate()
	{
		if (m_runeAdvanceTemplates.Count == 0)
		{
			Init();
		}
		return m_runeAdvanceTemplates.Values.ToList();
	}

	public static RuneAdvanceTemplateInfo FindRuneAdvanceTemplate(int id)
	{
		if (m_runeAdvanceTemplates.Count == 0)
		{
			Init();
		}
		if (m_runeAdvanceTemplates.ContainsKey(id))
		{
			return m_runeAdvanceTemplates[id];
		}
		return null;
	}
}

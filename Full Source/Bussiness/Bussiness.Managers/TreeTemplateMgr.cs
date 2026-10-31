using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class TreeTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TreeTemplateInfo> _treeTemplates = new Dictionary<int, TreeTemplateInfo>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			TreeTemplateInfo[] array = LoadTreeTemplateDb();
			Dictionary<int, TreeTemplateInfo> value = LoadTreeTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref _treeTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad TreeTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static TreeTemplateInfo[] LoadTreeTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllTreeTemplate();
	}

	public static Dictionary<int, TreeTemplateInfo> LoadTreeTemplates(TreeTemplateInfo[] treeTemplate)
	{
		Dictionary<int, TreeTemplateInfo> dictionary = new Dictionary<int, TreeTemplateInfo>();
		foreach (TreeTemplateInfo treeTemplateInfo in treeTemplate)
		{
			if (!dictionary.Keys.Contains(treeTemplateInfo.Level))
			{
				dictionary.Add(treeTemplateInfo.Level, treeTemplateInfo);
			}
		}
		return dictionary;
	}

	public static TreeTemplateInfo FindTreeTemplate(int level)
	{
		if (_treeTemplates.ContainsKey(level))
		{
			return _treeTemplates[level];
		}
		return null;
	}

	public static TreeTemplateInfo FindNextTreeTemplate(int level)
	{
		if (level > MaxTreeTemplate())
		{
			level = MaxTreeTemplate();
		}
		TreeTemplateInfo treeTemplateInfo = _treeTemplates[level];
		if (treeTemplateInfo == null)
		{
			level = _treeTemplates.Count - 1;
			treeTemplateInfo = _treeTemplates[level];
		}
		return treeTemplateInfo;
	}

	public static int MaxTreeTemplate()
	{
		return _treeTemplates.Count - 1;
	}
}

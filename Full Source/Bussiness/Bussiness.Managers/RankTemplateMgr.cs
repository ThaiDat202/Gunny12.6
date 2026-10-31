using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class RankTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, RankTemplateInfo> m_RankTemplates;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			RankTemplateInfo[] array = LoadRankTemplateDb();
			Dictionary<int, RankTemplateInfo> value = LoadRankTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_RankTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad RankTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static RankTemplateInfo[] LoadRankTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllRankTemplate();
	}

	public static Dictionary<int, RankTemplateInfo> LoadRankTemplates(RankTemplateInfo[] RankTemplate)
	{
		Dictionary<int, RankTemplateInfo> dictionary = new Dictionary<int, RankTemplateInfo>();
		foreach (RankTemplateInfo rankTemplateInfo in RankTemplate)
		{
			if (!dictionary.Keys.Contains(rankTemplateInfo.ID))
			{
				dictionary.Add(rankTemplateInfo.ID, rankTemplateInfo);
			}
		}
		return dictionary;
	}

	public static RankTemplateInfo FindRankTemplate(int ID)
	{
		if (m_RankTemplates.ContainsKey(ID))
		{
			return m_RankTemplates[ID];
		}
		return null;
	}
}

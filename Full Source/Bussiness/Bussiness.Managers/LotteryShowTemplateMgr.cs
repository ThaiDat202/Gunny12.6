using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class LotteryShowTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<LotteryShowTemplateInfo>> m_LotteryShowTemplates;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			LotteryShowTemplateInfo[] array = LoadLotteryShowTemplateDb();
			Dictionary<int, List<LotteryShowTemplateInfo>> value = LoadLotteryShowTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_LotteryShowTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad LotteryShowTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static LotteryShowTemplateInfo[] LoadLotteryShowTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllLotteryShowTemplate();
	}

	public static Dictionary<int, List<LotteryShowTemplateInfo>> LoadLotteryShowTemplates(LotteryShowTemplateInfo[] LotteryShowTemplate)
	{
		Dictionary<int, List<LotteryShowTemplateInfo>> dictionary = new Dictionary<int, List<LotteryShowTemplateInfo>>();
		foreach (LotteryShowTemplateInfo info in LotteryShowTemplate)
		{
			if (!dictionary.Keys.Contains(info.BoxType))
			{
				IEnumerable<LotteryShowTemplateInfo> source = LotteryShowTemplate.Where((LotteryShowTemplateInfo s) => s.BoxType == info.BoxType);
				dictionary.Add(info.BoxType, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<LotteryShowTemplateInfo> FindLotteryShowTemplate(int DataId)
	{
		if (m_LotteryShowTemplates.ContainsKey(DataId))
		{
			return m_LotteryShowTemplates[DataId];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class AllQuestionsMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<AllQuestionsInfo>> m_AllQuestionss;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			AllQuestionsInfo[] array = LoadAllQuestionsDb();
			Dictionary<int, List<AllQuestionsInfo>> value = LoadAllQuestionss(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_AllQuestionss, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad AllQuestions", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static AllQuestionsInfo[] LoadAllQuestionsDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllAllQuestions();
	}

	public static Dictionary<int, List<AllQuestionsInfo>> LoadAllQuestionss(AllQuestionsInfo[] AllQuestions)
	{
		Dictionary<int, List<AllQuestionsInfo>> dictionary = new Dictionary<int, List<AllQuestionsInfo>>();
		foreach (AllQuestionsInfo info in AllQuestions)
		{
			if (!dictionary.Keys.Contains(info.QuestionCatalogID))
			{
				IEnumerable<AllQuestionsInfo> source = AllQuestions.Where((AllQuestionsInfo s) => s.QuestionCatalogID == info.QuestionCatalogID);
				dictionary.Add(info.QuestionCatalogID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<AllQuestionsInfo> FindAllQuestions(int DataId)
	{
		if (m_AllQuestionss.ContainsKey(DataId))
		{
			return m_AllQuestionss[DataId];
		}
		return null;
	}
}

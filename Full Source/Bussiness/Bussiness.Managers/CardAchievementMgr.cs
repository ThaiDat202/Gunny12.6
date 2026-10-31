using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class CardAchievementMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, CardAchievementInfo> m_cardAchievements = new Dictionary<int, CardAchievementInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, CardAchievementInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_cardAchievements, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("CardAchievementMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, CardAchievementInfo> LoadFromDatabase()
	{
		Dictionary<int, CardAchievementInfo> dictionary = new Dictionary<int, CardAchievementInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			CardAchievementInfo[] allCardAchievement = produceBussiness.GetAllCardAchievement();
			CardAchievementInfo[] array = allCardAchievement;
			foreach (CardAchievementInfo cardAchievementInfo in array)
			{
				if (!dictionary.ContainsKey(cardAchievementInfo.AchievementID))
				{
					dictionary.Add(cardAchievementInfo.AchievementID, cardAchievementInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<CardAchievementInfo> GetAllCardAchievement()
	{
		if (m_cardAchievements.Count == 0)
		{
			Init();
		}
		return m_cardAchievements.Values.ToList();
	}

	public static CardAchievementInfo FindCardAchievement(int id)
	{
		if (m_cardAchievements.Count == 0)
		{
			Init();
		}
		if (m_cardAchievements.ContainsKey(id))
		{
			return m_cardAchievements[id];
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class GodCardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, GodCardInfo> m_godCards = new Dictionary<int, GodCardInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, GodCardInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_godCards, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("GodCardMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, GodCardInfo> LoadFromDatabase()
	{
		Dictionary<int, GodCardInfo> dictionary = new Dictionary<int, GodCardInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GodCardInfo[] allGodCard = produceBussiness.GetAllGodCard();
			GodCardInfo[] array = allGodCard;
			foreach (GodCardInfo godCardInfo in array)
			{
				if (!dictionary.ContainsKey(godCardInfo.ID))
				{
					dictionary.Add(godCardInfo.ID, godCardInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<GodCardInfo> GetAllGodCard()
	{
		if (m_godCards.Count == 0)
		{
			Init();
		}
		return m_godCards.Values.ToList();
	}

	public static GodCardInfo FindGodCard(int id)
	{
		if (m_godCards.Count == 0)
		{
			Init();
		}
		if (m_godCards.ContainsKey(id))
		{
			return m_godCards[id];
		}
		return null;
	}
}

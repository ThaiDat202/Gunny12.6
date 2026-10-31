using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class QiYuanAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, QiYuanAwardInfo> m_qiYuanAwards = new Dictionary<int, QiYuanAwardInfo>();

	private static Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>> m_qiYuanTowerAwards = new Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>>();

	public static string QiYuanRewardBoxTemplateIds = "";

	public static string QiYuanTreasureBoxTemplateIds = "";

	private static ThreadSafeRandom rand = new ThreadSafeRandom();

	public static Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>> TowerAwards => m_qiYuanTowerAwards;

	public static bool ReLoad()
	{
		try
		{
			QiYuanAwardInfo[] array = LoadDb();
			Dictionary<int, QiYuanAwardInfo> value = LoadItems(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_qiYuanAwards, value);
				Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>> dictionary = new Dictionary<QiYuanAwardInfo, List<QiYuanAwardInfo>>();
				List<string> list = null;
				List<QiYuanAwardInfo> list2 = FindQiYuanAward(eQiYuanAwardType.REWARD_BOX);
				if (list2.Count > 0)
				{
					list = new List<string>();
					foreach (QiYuanAwardInfo item in list2)
					{
						list.Add(item.TemplateID.ToString());
					}
					QiYuanRewardBoxTemplateIds = string.Join(",", list);
				}
				list2 = FindQiYuanAward(eQiYuanAwardType.TREASURE_BOX);
				if (list2.Count > 0)
				{
					list = new List<string>();
					foreach (QiYuanAwardInfo item2 in list2)
					{
						list.Add(item2.TemplateID.ToString());
					}
					QiYuanTreasureBoxTemplateIds = string.Join(",", list);
				}
				Dictionary<int, QiYuanAwardInfo> dictionary2 = m_qiYuanAwards.Where((KeyValuePair<int, QiYuanAwardInfo> a) => a.Value.Type == 8).ToDictionary((KeyValuePair<int, QiYuanAwardInfo> a) => a.Key, (KeyValuePair<int, QiYuanAwardInfo> a) => a.Value);
				foreach (KeyValuePair<int, QiYuanAwardInfo> key in dictionary2)
				{
					if (key.Value.Rank == 0 && !dictionary.ContainsKey(key.Value))
					{
						List<QiYuanAwardInfo> value2 = dictionary2.Values.Where((QiYuanAwardInfo a) => a.Rank == key.Value.ID).ToList();
						dictionary.Add(key.Value, value2);
					}
				}
				Interlocked.Exchange(ref m_qiYuanTowerAwards, dictionary);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad QiYuanAwardMgr", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static QiYuanAwardInfo[] LoadDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllQiYuanAward();
	}

	public static Dictionary<int, QiYuanAwardInfo> LoadItems(QiYuanAwardInfo[] QiYuanData)
	{
		Dictionary<int, QiYuanAwardInfo> dictionary = new Dictionary<int, QiYuanAwardInfo>();
		foreach (QiYuanAwardInfo qiYuanAwardInfo in QiYuanData)
		{
			if (!dictionary.Keys.Contains(qiYuanAwardInfo.ID))
			{
				dictionary.Add(qiYuanAwardInfo.ID, qiYuanAwardInfo);
			}
		}
		return dictionary;
	}

	public static List<QiYuanAwardInfo> FindQiYuanAward(eQiYuanAwardType type)
	{
		return m_qiYuanAwards.Values.Where((QiYuanAwardInfo a) => a.Type == (int)type).ToList();
	}

	public static QiYuanAwardInfo FindQiYuanAward(eQiYuanAwardType type, int rank)
	{
		List<QiYuanAwardInfo> list = FindQiYuanAward(type);
		if (list != null && list.Count > 0)
		{
			return list.SingleOrDefault((QiYuanAwardInfo a) => a.Rank == rank);
		}
		return null;
	}

	public static QiYuanAwardInfo FindQiYuanAward(int id)
	{
		if (m_qiYuanAwards.ContainsKey(id))
		{
			return m_qiYuanAwards[id];
		}
		return null;
	}

	public static QiYuanAwardInfo[] GetRandom(eQiYuanAwardType type)
	{
		List<QiYuanAwardInfo> list = new List<QiYuanAwardInfo>();
		List<QiYuanAwardInfo> source = FindQiYuanAward(type);
		int num = 1;
		int maxRound = rand.Next(source.Select((QiYuanAwardInfo s) => s.Probability).Max());
		List<QiYuanAwardInfo> list2 = source.Where((QiYuanAwardInfo s) => s.Probability >= maxRound).ToList();
		int num2 = list2.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = EventAwardMgr.GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				QiYuanAwardInfo item = list2[index];
				list.Add(item);
			}
		}
		return list.ToArray();
	}
}

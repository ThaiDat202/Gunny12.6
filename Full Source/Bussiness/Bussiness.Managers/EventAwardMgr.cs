using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class EventAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static EventAwardInfo[] m_eventAward;

	private static Dictionary<int, List<EventAwardInfo>> m_EventAwards;

	private static Random rand = new Random();

	public static bool ReLoad()
	{
		try
		{
			EventAwardInfo[] array = LoadEventAwardDb();
			Dictionary<int, List<EventAwardInfo>> value = LoadEventAwards(array);
			if (array != null)
			{
				Interlocked.Exchange(ref m_eventAward, array);
				Interlocked.Exchange(ref m_EventAwards, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static EventAwardInfo[] LoadEventAwardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetEventAwardInfos();
	}

	public static Dictionary<int, List<EventAwardInfo>> LoadEventAwards(EventAwardInfo[] EventAwards)
	{
		Dictionary<int, List<EventAwardInfo>> dictionary = new Dictionary<int, List<EventAwardInfo>>();
		foreach (EventAwardInfo info in EventAwards)
		{
			if (!dictionary.Keys.Contains(info.ActivityType))
			{
				IEnumerable<EventAwardInfo> source = EventAwards.Where((EventAwardInfo s) => s.ActivityType == info.ActivityType);
				dictionary.Add(info.ActivityType, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<EventAwardInfo> FindEventAward(eEventType DataId)
	{
		if (m_EventAwards.ContainsKey((int)DataId))
		{
			return m_EventAwards[(int)DataId];
		}
		return new List<EventAwardInfo>();
	}

	public static List<ItemInfo> GetEventAwardByType(eEventType DataId)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<EventAwardInfo> list2 = new List<EventAwardInfo>();
		List<EventAwardInfo> list3 = FindEventAward(DataId);
		if (list3.Count == 0)
		{
			return null;
		}
		list2 = list3.Where((EventAwardInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = rand.Next(list3.Select((EventAwardInfo s) => s.Random).Max());
		List<EventAwardInfo> list4 = list3.Where((EventAwardInfo s) => s.Random >= maxRound && !s.IsSelect).ToList();
		int num2 = list4.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				EventAwardInfo item = list4[index];
				list2.Add(item);
			}
		}
		foreach (EventAwardInfo item2 in list2)
		{
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item2.TemplateID);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, item2.Count, 105);
			itemInfo.TemplateID = item2.TemplateID;
			itemInfo.IsBinds = item2.IsBinds;
			itemInfo.ValidDate = item2.ValidDate;
			itemInfo.Count = item2.Count;
			itemInfo.StrengthenLevel = item2.StrengthenLevel;
			itemInfo.AttackCompose = item2.AttackCompose;
			itemInfo.DefendCompose = item2.DefendCompose;
			itemInfo.AgilityCompose = item2.AgilityCompose;
			itemInfo.LuckCompose = item2.LuckCompose;
			itemInfo.MagicAttack = item2.MagicAttack;
			itemInfo.MagicDefence = item2.MagicDefence;
			list.Add(itemInfo);
		}
		return list;
	}

	public static EventAwardInfo[] CreateSearchGoodsAward(int count)
	{
		Dictionary<int, EventAwardInfo> dictionary = new Dictionary<int, EventAwardInfo>();
		for (int i = 0; i < count; i++)
		{
			EventAwardInfo[] randomAward = GetRandomAward(eEventType.SEARCH_GOODS);
			if (randomAward.Length != 0)
			{
				EventAwardInfo eventAwardInfo = randomAward[0];
				if (!dictionary.ContainsKey(eventAwardInfo.TemplateID))
				{
					dictionary.Add(eventAwardInfo.TemplateID, eventAwardInfo);
				}
			}
		}
		return dictionary.Values.ToArray();
	}

	public static EventAwardInfo[] CreateMagpieBridgeAward(int count)
	{
		Dictionary<int, EventAwardInfo> dictionary = new Dictionary<int, EventAwardInfo>();
		for (int i = 0; i < count; i++)
		{
			EventAwardInfo[] randomAward = GetRandomAward(eEventType.MAGPIE_BRIDGE);
			if (randomAward.Length != 0)
			{
				EventAwardInfo eventAwardInfo = randomAward[0];
				if (!dictionary.ContainsKey(eventAwardInfo.TemplateID))
				{
					dictionary.Add(eventAwardInfo.TemplateID, eventAwardInfo);
				}
			}
		}
		return dictionary.Values.ToArray();
	}

	public static EventAwardInfo[] GetRandomAward(eEventType type)
	{
		List<EventAwardInfo> list = new List<EventAwardInfo>();
		List<EventAwardInfo> source = FindEventAward(eEventType.SEARCH_GOODS);
		int num = 1;
		int maxRound = rand.Next(source.Select((EventAwardInfo s) => s.Random).Max());
		List<EventAwardInfo> list2 = source.Where((EventAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list2.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				EventAwardInfo item = list2[index];
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public static List<NewChickenBoxItemInfo> GetNewChickenBoxAward(eEventType DataId)
	{
		List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
		List<EventAwardInfo> list2 = new List<EventAwardInfo>();
		List<EventAwardInfo> source = FindEventAward(DataId);
		int num = 1;
		int maxRound = rand.Next(source.Select((EventAwardInfo s) => s.Random).Max());
		List<EventAwardInfo> list3 = source.Where((EventAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				EventAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (EventAwardInfo item2 in list2)
		{
			NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
			newChickenBoxItemInfo.TemplateID = item2.TemplateID;
			newChickenBoxItemInfo.IsBinds = item2.IsBinds;
			newChickenBoxItemInfo.ValidDate = item2.ValidDate;
			newChickenBoxItemInfo.Count = item2.Count;
			newChickenBoxItemInfo.StrengthenLevel = item2.StrengthenLevel;
			newChickenBoxItemInfo.AttackCompose = 0;
			newChickenBoxItemInfo.DefendCompose = 0;
			newChickenBoxItemInfo.AgilityCompose = 0;
			newChickenBoxItemInfo.LuckCompose = 0;
			newChickenBoxItemInfo.Quality = ItemMgr.FindItemTemplate(item2.TemplateID)?.Quality ?? 2;
			newChickenBoxItemInfo.IsSelected = false;
			newChickenBoxItemInfo.IsSeeded = false;
			list.Add(newChickenBoxItemInfo);
		}
		return list;
	}

	public static List<EventAwardInfo> GetDiceAward(eEventType DataId)
	{
		List<EventAwardInfo> list = new List<EventAwardInfo>();
		List<EventAwardInfo> source = FindEventAward(DataId);
		int num = 1;
		int maxRound = rand.Next(source.Select((EventAwardInfo s) => s.Random).Max());
		List<EventAwardInfo> list2 = source.Where((EventAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list2.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				EventAwardInfo item = list2[index];
				list.Add(item);
			}
		}
		return list;
	}

	public static List<CardsTakeOutInfo> GetFightFootballTimeAward(eEventType DataId)
	{
		List<CardsTakeOutInfo> list = new List<CardsTakeOutInfo>();
		List<EventAwardInfo> list2 = new List<EventAwardInfo>();
		List<EventAwardInfo> source = FindEventAward(DataId);
		int num = 1;
		int maxRound = rand.Next(source.Select((EventAwardInfo s) => s.Random).Max());
		List<EventAwardInfo> list3 = source.Where((EventAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				EventAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (EventAwardInfo item2 in list2)
		{
			CardsTakeOutInfo cardsTakeOutInfo = new CardsTakeOutInfo();
			cardsTakeOutInfo.templateID = item2.TemplateID;
			cardsTakeOutInfo.count = item2.Count;
			list.Add(cardsTakeOutInfo);
		}
		return list;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = rand.Next(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}
}

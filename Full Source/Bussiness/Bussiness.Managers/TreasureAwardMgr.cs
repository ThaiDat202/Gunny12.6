using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class TreasureAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TreasureAwardInfo> _treasureAward;

	private static ThreadSafeRandom rand = new ThreadSafeRandom();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, TreasureAwardInfo> treasureAward = new Dictionary<int, TreasureAwardInfo>();
			if (Load(treasureAward))
			{
				try
				{
					_treasureAward = treasureAward;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TreasureAwardMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_treasureAward = new Dictionary<int, TreasureAwardInfo>();
			rand = new ThreadSafeRandom();
			return Load(_treasureAward);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TreasureAwardMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<int, TreasureAwardInfo> treasureAward)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			TreasureAwardInfo[] allTreasureAward = produceBussiness.GetAllTreasureAward();
			TreasureAwardInfo[] array = allTreasureAward;
			foreach (TreasureAwardInfo treasureAwardInfo in array)
			{
				if (!treasureAward.ContainsKey(treasureAwardInfo.ID))
				{
					treasureAward.Add(treasureAwardInfo.ID, treasureAwardInfo);
				}
			}
		}
		return true;
	}

	public static TreasureAwardInfo FindTreasureAwardInfo(int ID)
	{
		try
		{
			if (_treasureAward.ContainsKey(ID))
			{
				return _treasureAward[ID];
			}
		}
		catch
		{
		}
		return null;
	}

	public static List<TreasureAwardInfo> GetTreasureInfos()
	{
		if (_treasureAward == null)
		{
			Init();
		}
		List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
		foreach (TreasureAwardInfo value in _treasureAward.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public static List<TreasureDataInfo> CreateTreasureData(int UserID)
	{
		List<TreasureDataInfo> list = new List<TreasureDataInfo>();
		Dictionary<int, TreasureDataInfo> dictionary = new Dictionary<int, TreasureDataInfo>();
		int num = 0;
		while (list.Count < 16)
		{
			List<TreasureDataInfo> treasureData = GetTreasureData();
			int index = rand.Next(treasureData.Count);
			TreasureDataInfo treasureDataInfo = treasureData[index];
			treasureDataInfo.UserID = UserID;
			if (!dictionary.Keys.Contains(treasureDataInfo.TemplateID))
			{
				dictionary.Add(treasureDataInfo.TemplateID, treasureDataInfo);
				list.Add(treasureDataInfo);
			}
			num++;
		}
		return list;
	}

	public static List<TreasureDataInfo> GetTreasureData()
	{
		List<TreasureDataInfo> list = new List<TreasureDataInfo>();
		List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
		List<TreasureAwardInfo> treasureInfos = GetTreasureInfos();
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(treasureInfos.Select((TreasureAwardInfo s) => s.Random).Max());
		List<TreasureAwardInfo> list3 = treasureInfos.Where((TreasureAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				TreasureAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (TreasureAwardInfo item2 in list2)
		{
			TreasureDataInfo treasureDataInfo = new TreasureDataInfo();
			treasureDataInfo.ID = 0;
			treasureDataInfo.UserID = 0;
			treasureDataInfo.TemplateID = item2.TemplateID;
			treasureDataInfo.Count = item2.Count;
			treasureDataInfo.ValidDate = item2.Validate;
			treasureDataInfo.pos = -1;
			treasureDataInfo.BeginDate = DateTime.Now;
			treasureDataInfo.IsExit = true;
			list.Add(treasureDataInfo);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GypsyShop;

public class GypsyShopMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Random rand;

	protected static Timer _dbTimer;

	private static bool _openOrClose;

	private static bool _freshTime;

	private static Dictionary<int, List<MysteryShopInfo>> m_MysteryShops = new Dictionary<int, List<MysteryShopInfo>>();

	public static bool OpenOrClose => _openOrClose;

	public static void BeginTimer()
	{
		int num = 60000;
		if (_dbTimer == null)
		{
			_dbTimer = new Timer(GypsyTimeCheck, null, num, num);
		}
		else
		{
			_dbTimer.Change(num, num);
		}
	}

	protected static void GypsyTimeCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			int num = int.Parse(GameProperties.MysteryShopOpenTime.Split('|')[0]);
			int num2 = int.Parse(GameProperties.MysteryShopOpenTime.Split('|')[1]);
			int hour = DateTime.Now.Hour;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			if (hour >= num2 && hour < num && OpenOrClose)
			{
				_openOrClose = false;
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Actives.SendGypsyShopOpenClose(open: false);
				}
			}
			else if (hour >= num && !OpenOrClose)
			{
				_openOrClose = true;
				GamePlayer[] array2 = allPlayers;
				foreach (GamePlayer gamePlayer in array2)
				{
					if (gamePlayer != null && gamePlayer != null)
					{
						gamePlayer.Actives.ResetMysteryShop();
						gamePlayer.Actives.SendGypsyShopOpenClose(open: true);
					}
				}
			}
			int mysteryShopFreshTime = GameProperties.MysteryShopFreshTime;
			if (mysteryShopFreshTime == hour && !_freshTime)
			{
				_freshTime = true;
				GamePlayer[] array3 = allPlayers;
				foreach (GamePlayer gamePlayer2 in array3)
				{
					if (gamePlayer2 != null)
					{
						gamePlayer2?.Actives.RefreshMysteryShopByHour();
					}
				}
			}
			if (hour != mysteryShopFreshTime && _freshTime)
			{
				_freshTime = false;
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Gypsy TimeCheck: " + ex);
		}
	}

	public static void StopAllTimer()
	{
		if (_dbTimer != null)
		{
			_dbTimer.Dispose();
			_dbTimer = null;
		}
	}

	public static bool Init()
	{
		try
		{
			_openOrClose = false;
			rand = new Random();
			MysteryShopInfo[] array = LoadMysteryShopDb();
			Dictionary<int, List<MysteryShopInfo>> value = LoadMysteryShops(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_MysteryShops, value);
			}
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad MysteryShop", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
	}

	public static MysteryShopInfo[] LoadMysteryShopDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllMysteryShop();
	}

	public static Dictionary<int, List<MysteryShopInfo>> LoadMysteryShops(MysteryShopInfo[] MysteryShop)
	{
		Dictionary<int, List<MysteryShopInfo>> dictionary = new Dictionary<int, List<MysteryShopInfo>>();
		foreach (MysteryShopInfo info in MysteryShop)
		{
			if (!dictionary.Keys.Contains(info.LableType))
			{
				IEnumerable<MysteryShopInfo> source = MysteryShop.Where((MysteryShopInfo s) => s.LableType == info.LableType);
				dictionary.Add(info.LableType, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<MysteryShopInfo> FindMysteryShop(int LableType)
	{
		List<MysteryShopInfo> list = new List<MysteryShopInfo>();
		if (m_MysteryShops.ContainsKey(LableType))
		{
			List<MysteryShopInfo> list2 = m_MysteryShops[LableType];
			foreach (MysteryShopInfo item in list2)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<MysteryShopInfo> GetRateMysteryShop()
	{
		List<MysteryShopInfo> list = new List<MysteryShopInfo>();
		List<MysteryShopInfo> list2 = FindMysteryShop(2);
		int num = ((list2.Count > 6) ? 6 : list2.Count);
		for (int i = 0; i < num; i++)
		{
			MysteryShopInfo item = list2[i];
			list.Add(item);
		}
		return list;
	}

	public static List<MysteryShopInfo> GetMysteryShop()
	{
		List<MysteryShopInfo> list = new List<MysteryShopInfo>();
		List<MysteryShopInfo> list2 = FindMysteryShop(2);
		int num = 0;
		while (list.Count < 8)
		{
			List<MysteryShopInfo> rateAward = GetRateAward();
			foreach (MysteryShopInfo item in rateAward)
			{
				foreach (MysteryShopInfo item2 in list2)
				{
					if (item.InfoID == item2.InfoID)
					{
						item.Quality = 1;
						break;
					}
				}
				list.Add(item);
			}
			num++;
		}
		return list;
	}

	public static List<MysteryShopInfo> GetRateAward()
	{
		List<MysteryShopInfo> list = new List<MysteryShopInfo>();
		List<MysteryShopInfo> source = FindMysteryShop(1);
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(source.Select((MysteryShopInfo s) => s.Random).Max());
		List<MysteryShopInfo> list2 = source.Where((MysteryShopInfo s) => s.Random >= maxRound).ToList();
		int num2 = list2.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				MysteryShopInfo item = list2[index];
				list.Add(item);
			}
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

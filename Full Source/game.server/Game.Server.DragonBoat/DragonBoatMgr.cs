using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.DragonBoat;

public class DragonBoatMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static CommunalActiveInfo m_communalActives;

	private static Dictionary<int, ActiveSystemInfo> m_localTotalScore = new Dictionary<int, ActiveSystemInfo>();

	private static List<ActiveSystemInfo> m_areaTotalScore = new List<ActiveSystemInfo>();

	public static readonly int DRAGONBOAT_CHIP = 11690;

	public static readonly int KINGSTATUE_CHIP = 11771;

	public static readonly int LINSHI_CHIP = 201309;

	private static int m_completeExp = 0;

	protected static Timer _dbTimer;

	protected static Timer _expTimer;

	public static bool Init()
	{
		try
		{
			CommunalActiveInfo[] allCommunalActives = CommunalActiveMgr.GetAllCommunalActives();
			CommunalActiveInfo[] array = allCommunalActives;
			foreach (CommunalActiveInfo communalActiveInfo in array)
			{
				if (communalActiveInfo.IsOpen)
				{
					m_communalActives = communalActiveInfo;
				}
			}
			using (new ProduceBussiness())
			{
				if (m_communalActives != null && m_communalActives.BeginTime.Date > DateTime.Now.Date && m_communalActives.CanReset)
				{
					m_communalActives.CanReset = false;
					m_communalActives.CanSendAward = true;
				}
			}
			return LoadTotalScore();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DragonBoatMgr =>Int", exception);
			}
			return false;
		}
		finally
		{
			BeginTimer();
		}
	}

	public static void UpdateCommunalActives()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		if (m_communalActives != null)
		{
			produceBussiness.UpdateCommunalActive(m_communalActives);
		}
	}

	public static void StartNewEvent()
	{
		Init();
	}

	public static void EndEvent(bool sendAward)
	{
		if (m_communalActives != null)
		{
			StopAllTimer();
			m_communalActives.BeginTime = DateTime.Now.AddDays(-7.0);
			m_communalActives.EndTime = DateTime.Now.AddDays(-3.0);
			m_communalActives.CanSendAward = false;
			m_communalActives.CanReset = true;
			UpdateCommunalActives();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer?.Out.SendDragonBoat(gamePlayer.PlayerCharacter);
			}
			LoadTotalScore();
			if (sendAward)
			{
				SendDragonBoatAward();
			}
		}
	}

	public static void SendDragonBoatAward()
	{
		if (m_communalActives == null || !m_communalActives.CanSendAward)
		{
			return;
		}
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			List<ActiveSystemInfo> list = SelectTopTenCurrenServer();
			List<ItemInfo> list2 = new List<ItemInfo>();
			string text = "";
			foreach (ActiveSystemInfo item in list)
			{
				PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(item.UserID);
				if (userSingleByUserID != null && item.myRank <= 10)
				{
					text = LanguageMgr.GetTranslation("DragonBoatAward.Curr", item.myRank);
					list2 = GetAwardInfos(1, item.myRank);
					WorldEventMgr.SendItemsToMails(list2, item.UserID, item.NickName, item.ZoneID, text);
				}
			}
			if (m_communalActives.CanAwardArea)
			{
				List<ActiveSystemInfo> list3 = SelectTopTenAllServer();
				foreach (ActiveSystemInfo item2 in list3)
				{
					PlayerInfo userSingleByUserID2 = playerBussiness.GetUserSingleByUserID(item2.UserID);
					if (userSingleByUserID2 != null && item2.myRank <= 10)
					{
						text = LanguageMgr.GetTranslation("DragonBoatAward.All", item2.myRank);
						list2 = GetAwardInfos(2, item2.myRank);
						WorldEventMgr.SendItemsToMails(list2, item2.UserID, item2.NickName, item2.ZoneID, text);
					}
				}
			}
			m_communalActives.CanSendAward = false;
			m_communalActives.CanReset = true;
		}
		UpdateCommunalActives();
		Console.WriteLine("===============Send DragonBoat award completed!=================");
	}

	private static bool LoadTotalScore()
	{
		bool result = false;
		try
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			ActiveSystemInfo[] allActiveSystemData = playerBussiness.GetAllActiveSystemData();
			ActiveSystemInfo[] array = allActiveSystemData;
			foreach (ActiveSystemInfo activeSystemInfo in array)
			{
				if (!m_localTotalScore.ContainsKey(activeSystemInfo.UserID))
				{
					m_localTotalScore.Add(activeSystemInfo.UserID, activeSystemInfo);
				}
				else
				{
					m_localTotalScore[activeSystemInfo.UserID] = activeSystemInfo;
				}
			}
			UpdateRank();
			result = true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DragonBoatMgr =>LoadLocalTotalScore", exception);
			}
		}
		try
		{
			AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
			List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
			AreaConfigInfo[] array2 = allAreaConfig;
			foreach (AreaConfigInfo areaConfigInfo in array2)
			{
				using PlayerBussiness playerBussiness2 = new PlayerBussiness(areaConfigInfo);
				ActiveSystemInfo[] allActiveSystemData2 = playerBussiness2.GetAllActiveSystemData();
				ActiveSystemInfo[] array3 = allActiveSystemData2;
				foreach (ActiveSystemInfo activeSystemInfo2 in array3)
				{
					if (activeSystemInfo2.totalScore != 0)
					{
						activeSystemInfo2.ZoneID = areaConfigInfo.AreaID;
						activeSystemInfo2.ZoneName = areaConfigInfo.AreaName;
						list.Add(activeSystemInfo2);
					}
				}
			}
			UpdateAreaRank(list);
			result = true;
		}
		catch (Exception exception2)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DragonBoatMgr =>LoadAreaTotalScore", exception2);
			}
		}
		UpdateBoatCompleteExp();
		return result;
	}

	public static bool UpdateTotalBoatExp(ActiveSystemInfo asi)
	{
		List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
		foreach (ActiveSystemInfo item in m_areaTotalScore)
		{
			if (item.UserID != asi.UserID || item.ZoneID != asi.ZoneID)
			{
				list.Add(item);
			}
		}
		list.Add(asi);
		UpdateAreaRank(list);
		return true;
	}

	public static void UpdateAreaRank(List<ActiveSystemInfo> list)
	{
		if (m_communalActives == null)
		{
			return;
		}
		m_areaTotalScore.Clear();
		IOrderedEnumerable<ActiveSystemInfo> orderedEnumerable = list.OrderByDescending((ActiveSystemInfo pair) => pair.totalScore);
		int num = 1;
		foreach (ActiveSystemInfo item in orderedEnumerable)
		{
			if (item.totalScore >= m_communalActives.MinScore)
			{
				item.myRank = num;
				m_areaTotalScore.Add(item);
				num++;
			}
		}
	}

	public static bool IsDragonBoatOpen()
	{
		return m_communalActives != null;
	}

	public static bool UpdateLocalBoatExp(ActiveSystemInfo info)
	{
		UpdateTotalBoatExp(info);
		if (!m_localTotalScore.Keys.Contains(info.UserID))
		{
			m_localTotalScore.Add(info.UserID, info);
		}
		else
		{
			m_localTotalScore[info.UserID] = info;
		}
		UpdateRank();
		return true;
	}

	public static void UpdateRank()
	{
		if (m_communalActives == null)
		{
			return;
		}
		IOrderedEnumerable<KeyValuePair<int, ActiveSystemInfo>> orderedEnumerable = m_localTotalScore.OrderByDescending(delegate(KeyValuePair<int, ActiveSystemInfo> pair)
		{
			KeyValuePair<int, ActiveSystemInfo> keyValuePair = pair;
			return keyValuePair.Value.totalScore;
		});
		int num = 1;
		foreach (KeyValuePair<int, ActiveSystemInfo> item in orderedEnumerable)
		{
			if (item.Value.totalScore >= m_communalActives.MinScore)
			{
				m_localTotalScore[item.Value.UserID].myRank = num;
				num++;
			}
		}
	}

	public static int boatCompleteExp()
	{
		return m_completeExp;
	}

	public static void UpdateBoatCompleteExp()
	{
		m_completeExp = 0;
		foreach (ActiveSystemInfo value in m_localTotalScore.Values)
		{
			m_completeExp += value.totalScore;
		}
	}

	public static int ReduceToemUpGrace()
	{
		List<CommunalActiveExpInfo> list = CommunalActiveMgr.FindCommunalActiveExp(OpenType());
		int num = boatCompleteExp();
		if (num >= list[list.Count - 1].Exp)
		{
			return 30;
		}
		return 0;
	}

	public static bool IsContinuous()
	{
		if (m_communalActives == null)
		{
			return false;
		}
		if (m_communalActives.EndTime.Date < DateTime.Now.Date)
		{
			return false;
		}
		return true;
	}

	public static int periodType()
	{
		if (!IsContinuous())
		{
			return 2;
		}
		List<CommunalActiveExpInfo> list = CommunalActiveMgr.FindCommunalActiveExp(OpenType());
		int num = 1250000;
		if (list.Count == 6)
		{
			num = list[5].Exp;
		}
		else
		{
			Console.WriteLine("Communal Active Exp Error, count: {0}", list.Count);
		}
		int num2 = boatCompleteExp();
		if (num2 >= num)
		{
			return 2;
		}
		return 1;
	}

	public static int AddExpPlus()
	{
		List<CommunalActiveExpInfo> list = CommunalActiveMgr.FindCommunalActiveExp(OpenType());
		int num = boatCompleteExp();
		foreach (CommunalActiveExpInfo item in list)
		{
			if (num >= item.Exp)
			{
				return item.AddExpPlus;
			}
		}
		return 0;
	}

	public static int DayMaxScore()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.DayMaxScore;
		}
		return -1;
	}

	public static int MinScore()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.MinScore;
		}
		return 13000;
	}

	public static string AddPropertyByMoney()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.AddPropertyByMoney;
		}
		return "100:10,10";
	}

	public static string AddPropertyByProp()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.AddPropertyByProp;
		}
		return "1:10,10";
	}

	public static int OpenType()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.ActiveID;
		}
		return 1;
	}

	public static DateTime BeginDate()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.BeginTime;
		}
		return DateTime.Now;
	}

	public static DateTime EndDate()
	{
		if (m_communalActives != null)
		{
			return m_communalActives.EndTime;
		}
		return DateTime.Now;
	}

	public static List<ItemInfo> GetAwardInfos(int area, int rank)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		if (m_communalActives != null)
		{
			List<CommunalActiveAwardInfo> list2 = CommunalActiveMgr.FindCommunalActiveAward(m_communalActives.ActiveID);
			foreach (CommunalActiveAwardInfo item in list2)
			{
				if (item.IsArea == area && item.RandID == rank)
				{
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item.TemplateID);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, item.Count, 105);
					itemInfo.IsBinds = item.IsBind;
					itemInfo.ValidDate = item.ValidDate;
					list.Add(itemInfo);
				}
			}
		}
		return list;
	}

	public static int FindMyRank(int ID)
	{
		if (m_localTotalScore.ContainsKey(ID))
		{
			int myRank = m_localTotalScore[ID].myRank;
			return (myRank == 0) ? (-1) : myRank;
		}
		return -1;
	}

	public static int FindAreaMyRank(int ID, int zoneID)
	{
		foreach (ActiveSystemInfo item in m_areaTotalScore)
		{
			if (item.UserID == ID && item.ZoneID == zoneID)
			{
				return (item.myRank == 0) ? (-1) : item.myRank;
			}
		}
		return -1;
	}

	public static List<ActiveSystemInfo> SelectTopTenCurrenServer()
	{
		List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
		IEnumerable<KeyValuePair<int, ActiveSystemInfo>> enumerable = m_localTotalScore.Where(delegate(KeyValuePair<int, ActiveSystemInfo> pair)
		{
			KeyValuePair<int, ActiveSystemInfo> keyValuePair2 = pair;
			return keyValuePair2.Value.totalScore >= MinScore();
		}).OrderByDescending(delegate(KeyValuePair<int, ActiveSystemInfo> pair)
		{
			KeyValuePair<int, ActiveSystemInfo> keyValuePair = pair;
			return keyValuePair.Value.totalScore;
		}).Take(10);
		foreach (KeyValuePair<int, ActiveSystemInfo> item in enumerable)
		{
			list.Add(item.Value);
		}
		return list;
	}

	public static List<ActiveSystemInfo> SelectTopTenAllServer()
	{
		return (from pair in m_areaTotalScore
			where pair.totalScore >= MinScore()
			orderby pair.totalScore descending
			select pair).Take(10).ToList();
	}

	public static void BeginTimer()
	{
		int num = 1800000;
		if (_dbTimer == null)
		{
			_dbTimer = new Timer(dbTimeCheck, null, num, num);
		}
		else
		{
			_dbTimer.Change(num, num);
		}
		num = 60000;
		if (_expTimer == null)
		{
			_expTimer = new Timer(BoatExpCheck, null, num, num);
		}
		else
		{
			_expTimer.Change(num, num);
		}
	}

	protected static void dbTimeCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			if (IsDragonBoatOpen())
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Actives.SaveActiveToDatabase();
				}
				if (LoadTotalScore())
				{
					if (!IsContinuous())
					{
						GamePlayer[] array2 = allPlayers;
						foreach (GamePlayer gamePlayer in array2)
						{
							gamePlayer?.Out.SendDragonBoat(gamePlayer.PlayerCharacter);
						}
						SendDragonBoatAward();
					}
					Console.WriteLine("=======Reload communal active Success!========");
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Communal Active: " + ex);
		}
	}

	protected static void BoatExpCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateBoatCompleteExp();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("Communal Active: " + ex);
		}
	}

	public static void StopAllTimer()
	{
		if (_dbTimer != null)
		{
			_dbTimer.Change(-1, -1);
			_dbTimer.Dispose();
			_dbTimer = null;
		}
		if (_expTimer != null)
		{
			_expTimer.Change(-1, -1);
			_expTimer.Dispose();
			_expTimer = null;
		}
	}
}

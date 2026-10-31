using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class ActiveSystemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Random rand;

	private static Dictionary<int, LanternriddlesInfo> m_lanternriddlesInfo = new Dictionary<int, LanternriddlesInfo>();

	private static List<LuckStarRewardRecordInfo> m_recordList = new List<LuckStarRewardRecordInfo>();

	protected static Timer m_lanternriddlesScanTimer;

	private static bool m_sendOpenToClient;

	private static bool m_sendCloseToClient;

	private static bool m_lanternriddlesOpen;

	private static bool m_IsBattleGoundOpen;

	private static bool m_IsLeagueOpen;

	private static bool m_IsFightFootballTime;

	private static int m_luckStarCountDown;

	private static Dictionary<int, HalloweenRankInfo> m_HalloweenRanks = new Dictionary<int, HalloweenRankInfo>();

	public static List<LuckStarRewardRecordInfo> RecordList => m_recordList;

	public static bool LanternriddlesOpen => m_lanternriddlesOpen;

	public static bool IsBattleGoundOpen
	{
		get
		{
			return m_IsBattleGoundOpen;
		}
		set
		{
			m_IsBattleGoundOpen = value;
		}
	}

	public static bool IsLeagueOpen
	{
		get
		{
			return m_IsLeagueOpen;
		}
		set
		{
			m_IsLeagueOpen = value;
		}
	}

	public static bool IsFightFootballTime
	{
		get
		{
			return m_IsFightFootballTime;
		}
		set
		{
			m_IsFightFootballTime = value;
		}
	}

	public static DateTime EndDate => DateTime.Now.AddMilliseconds(GameProperties.LightRiddleAnswerTime * 1000);

	public static List<HappyRechargeReward> HappyRechargeRewards { get; set; }

	public static object PrayIndianaAward { get; private set; }

	public static List<HappyRechargeTicketReward> HappyTicketRewards { get; set; }

	public static bool Init()
	{
		try
		{
			rand = new Random();
			m_IsBattleGoundOpen = false;
			m_IsLeagueOpen = false;
			m_IsFightFootballTime = false;
			m_lanternriddlesOpen = false;
			m_sendOpenToClient = true;
			m_sendCloseToClient = true;
			m_luckStarCountDown = Math.Abs(60 - DateTime.Now.Minute - 30);
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				HappyTicketRewards = produceBussiness.GetRewardHappyTicket();
				HappyRechargeRewards = produceBussiness.HappyRechargeReward();
				PrayIndianaAward = produceBussiness.HappyRechargeReward();
			}
			BeginTimer();
			return LoadEventData();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ActiveSystemMgr", exception);
			}
			return false;
		}
	}

	public static bool LoadEventData()
	{
		if (!ReloadHalloweenRank())
		{
			return false;
		}
		return true;
	}

	public static void AddOrUpdateLanternriddles(int playerID, LanternriddlesInfo Lanternriddles)
	{
		Lanternriddles.QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
		if (!m_lanternriddlesInfo.ContainsKey(playerID))
		{
			m_lanternriddlesInfo.Add(playerID, Lanternriddles);
		}
		else
		{
			m_lanternriddlesInfo[playerID] = Lanternriddles;
		}
	}

	public static LanternriddlesInfo EnterLanternriddles(int playerID)
	{
		if (!m_lanternriddlesInfo.ContainsKey(playerID))
		{
			LanternriddlesInfo lanternriddlesInfo = new LanternriddlesInfo();
			lanternriddlesInfo.PlayerID = playerID;
			lanternriddlesInfo.QuestionIndex = 1;
			lanternriddlesInfo.QuestionView = 30;
			lanternriddlesInfo.DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
			lanternriddlesInfo.DoublePrice = GameProperties.LightRiddleComboMoney;
			lanternriddlesInfo.HitFreeCount = GameProperties.LightRiddleFreeHitNum;
			lanternriddlesInfo.HitPrice = GameProperties.LightRiddleHitMoney;
			lanternriddlesInfo.QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
			lanternriddlesInfo.MyInteger = 0;
			lanternriddlesInfo.QuestionNum = 0;
			lanternriddlesInfo.Option = -1;
			lanternriddlesInfo.IsHint = false;
			lanternriddlesInfo.IsDouble = false;
			lanternriddlesInfo.EndDate = EndDate;
			m_lanternriddlesInfo.Add(playerID, lanternriddlesInfo);
			return lanternriddlesInfo;
		}
		return GetLanternriddlesInfo(playerID);
	}

	public static LanternriddlesInfo GetLanternriddlesInfo(int playerID)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			if (m_lanternriddlesInfo[playerID].CanNextQuest)
			{
				m_lanternriddlesInfo[playerID].EndDate = EndDate;
				m_lanternriddlesInfo[playerID].QuestionIndex++;
			}
			if (m_lanternriddlesInfo[playerID].EndDate.Date != DateTime.Now.Date)
			{
				m_lanternriddlesInfo[playerID].EndDate = DateTime.Now;
				m_lanternriddlesInfo[playerID].QuestionIndex = 1;
				m_lanternriddlesInfo[playerID].QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
				m_lanternriddlesInfo[playerID].DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
				m_lanternriddlesInfo[playerID].DoublePrice = GameProperties.LightRiddleComboMoney;
				m_lanternriddlesInfo[playerID].HitFreeCount = GameProperties.LightRiddleFreeHitNum;
				m_lanternriddlesInfo[playerID].HitPrice = GameProperties.LightRiddleHitMoney;
				m_lanternriddlesInfo[playerID].MyInteger = 0;
				m_lanternriddlesInfo[playerID].QuestionNum = 0;
				m_lanternriddlesInfo[playerID].Option = -1;
				m_lanternriddlesInfo[playerID].IsHint = false;
				m_lanternriddlesInfo[playerID].IsDouble = false;
				m_lanternriddlesInfo[playerID].EndDate = EndDate;
			}
			return m_lanternriddlesInfo[playerID];
		}
		return CreateNullLanternriddlesInfo(playerID);
	}

	public static LanternriddlesInfo CreateNullLanternriddlesInfo(int playerID)
	{
		LanternriddlesInfo lanternriddlesInfo = new LanternriddlesInfo();
		lanternriddlesInfo.PlayerID = playerID;
		lanternriddlesInfo.QuestionIndex = 30;
		lanternriddlesInfo.QuestionView = 30;
		lanternriddlesInfo.DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
		lanternriddlesInfo.DoublePrice = GameProperties.LightRiddleComboMoney;
		lanternriddlesInfo.HitFreeCount = GameProperties.LightRiddleFreeHitNum;
		lanternriddlesInfo.HitPrice = GameProperties.LightRiddleHitMoney;
		lanternriddlesInfo.MyInteger = 0;
		lanternriddlesInfo.QuestionNum = 0;
		lanternriddlesInfo.Option = -1;
		lanternriddlesInfo.IsHint = true;
		lanternriddlesInfo.IsDouble = true;
		lanternriddlesInfo.EndDate = DateTime.Now;
		return lanternriddlesInfo;
	}

	public static LanternriddlesInfo GetLanternriddles(int playerID)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			return m_lanternriddlesInfo[playerID];
		}
		return null;
	}

	public static void UpdateLanternriddles(int playerID, LanternriddlesInfo info)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID) && info != null)
		{
			m_lanternriddlesInfo[playerID] = info;
		}
	}

	public static void LanternriddlesAnswer(int playerID, int option)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			m_lanternriddlesInfo[playerID].Option = option;
		}
	}

	private static bool CanOpenLanternriddles()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.LightRiddleBeginDate);
		DateTime dateTime2 = Convert.ToDateTime(GameProperties.LightRiddleEndDate);
		return DateTime.Now.Date < dateTime2.Date;
	}

	public static bool ReloadHalloweenRank()
	{
		try
		{
			HalloweenRankInfo[] array = LoadHalloweenRankDb();
			Dictionary<int, HalloweenRankInfo> value = LoadHalloweenRanks(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_HalloweenRanks, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad HalloweenRank", exception);
			}
			return false;
		}
		return true;
	}

	public static HalloweenRankInfo[] LoadHalloweenRankDb()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		return playerBussiness.GetAllHalloweenRank();
	}

	public static Dictionary<int, HalloweenRankInfo> LoadHalloweenRanks(HalloweenRankInfo[] HalloweenRank)
	{
		Dictionary<int, HalloweenRankInfo> dictionary = new Dictionary<int, HalloweenRankInfo>();
		foreach (HalloweenRankInfo halloweenRankInfo in HalloweenRank)
		{
			if (!dictionary.Keys.Contains(halloweenRankInfo.UserID))
			{
				dictionary.Add(halloweenRankInfo.UserID, halloweenRankInfo);
			}
		}
		return dictionary;
	}

	public static int FindHalloweenRank(int UserID)
	{
		if (m_HalloweenRanks.ContainsKey(UserID))
		{
			HalloweenRankInfo halloweenRankInfo = m_HalloweenRanks[UserID];
			return halloweenRankInfo.rank;
		}
		return 0;
	}

	public static void UpdateLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
	{
		AddRewardRecord(PlayerID, nickName, TemplateID, Count, isVip);
		GameServer.Instance.LoginServer.SendLuckStarRewardRecord(PlayerID, nickName, TemplateID, Count, isVip);
	}

	public static void AddRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
	{
		if (m_recordList.Count > 10)
		{
			m_recordList.Clear();
		}
		LuckStarRewardRecordInfo luckStarRewardRecordInfo = new LuckStarRewardRecordInfo();
		luckStarRewardRecordInfo.PlayerID = PlayerID;
		luckStarRewardRecordInfo.nickName = nickName;
		luckStarRewardRecordInfo.useStarNum = 1;
		luckStarRewardRecordInfo.TemplateID = TemplateID;
		luckStarRewardRecordInfo.Count = Count;
		luckStarRewardRecordInfo.isVip = isVip;
		m_recordList.Add(luckStarRewardRecordInfo);
	}

	public static void UpdateIsFightFootballTime(bool open)
	{
		m_IsFightFootballTime = open;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer?.Out.SendFightFootballTimeOpenClose(gamePlayer.PlayerCharacter.ID, open);
		}
	}

	public static void UpdateIsLeagueOpen(bool open)
	{
		m_IsLeagueOpen = open;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != null)
			{
				if (open)
				{
					gamePlayer.Out.SendLeagueNotice(gamePlayer.PlayerCharacter.ID, gamePlayer.BattleData.MatchInfo.restCount, gamePlayer.BattleData.maxCount, 1);
				}
				else
				{
					gamePlayer.Out.SendLeagueNotice(gamePlayer.PlayerCharacter.ID, gamePlayer.BattleData.MatchInfo.restCount, gamePlayer.BattleData.maxCount, 2);
				}
			}
		}
	}

	public static void UpdateIsBattleGoundOpen(bool open)
	{
		m_IsBattleGoundOpen = open;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer?.Out.SendBattleGoundOpen(gamePlayer.PlayerCharacter.ID);
		}
	}

	public static List<ItemInfo> GetPyramidAward(int layer)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<ActivitySystemItemInfo> list2 = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> activitySystemItemByLayer = ActiveMgr.GetActivitySystemItemByLayer(layer);
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(activitySystemItemByLayer.Select((ActivitySystemItemInfo s) => s.Probability).Max());
		List<ActivitySystemItemInfo> list3 = activitySystemItemByLayer.Where((ActivitySystemItemInfo s) => s.Probability >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				ActivitySystemItemInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (ActivitySystemItemInfo item2 in list2)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateID), item2.Count, 102);
			itemInfo.TemplateID = item2.TemplateID;
			itemInfo.IsBinds = item2.IsBind;
			itemInfo.ValidDate = item2.ValidDate;
			itemInfo.Count = item2.Count;
			itemInfo.StrengthenLevel = item2.StrengthLevel;
			itemInfo.AttackCompose = 0;
			itemInfo.DefendCompose = 0;
			itemInfo.AgilityCompose = 0;
			itemInfo.LuckCompose = 0;
			list.Add(itemInfo);
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

	public static void BeginTimer()
	{
		int num = 60000;
		if (m_lanternriddlesScanTimer == null)
		{
			m_lanternriddlesScanTimer = new Timer(LanternriddlesScan, null, num, num);
		}
		else
		{
			m_lanternriddlesScanTimer.Change(num, num);
		}
	}

	protected static void LanternriddlesScan(object sender)
	{
		try
		{
			log.Info("Begin Lanternriddles CheckPeriod....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			if (CanOpenLanternriddles())
			{
				int hour = DateTime.Now.Hour;
				DateTime dateTime = Convert.ToDateTime(GameProperties.LightRiddleBeginTime);
				DateTime dateTime2 = Convert.ToDateTime(GameProperties.LightRiddleEndTime);
				int hour2 = dateTime.Hour;
				int hour3 = dateTime2.Hour;
				if (hour >= hour2 && hour < hour3)
				{
					m_lanternriddlesOpen = true;
					if (m_sendOpenToClient)
					{
						LanternriddlesOpenClose();
						m_sendOpenToClient = false;
					}
				}
				else
				{
					m_lanternriddlesOpen = false;
					if (hour >= hour3 && m_sendCloseToClient)
					{
						LanternriddlesOpenClose();
						m_sendCloseToClient = false;
					}
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End Lanternriddles CheckPeriod....");
		}
		catch (Exception exception)
		{
			log.Error("lanternriddlesScan ", exception);
		}
	}

	public static void LanternriddlesOpenClose()
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer?.Out.SendLanternriddlesOpen(gamePlayer.PlayerId, m_lanternriddlesOpen);
		}
	}

	public static void StopAllTimer()
	{
		if (m_lanternriddlesScanTimer != null)
		{
			m_lanternriddlesScanTimer.Dispose();
			m_lanternriddlesScanTimer = null;
		}
	}
}

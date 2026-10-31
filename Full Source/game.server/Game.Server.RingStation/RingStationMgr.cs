using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.RingStation.Battle;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.RingStation;

public class RingStationMgr
{
	private static Random UpdhooLvMxf;

	private static Random rand;

	private static readonly ILog log;

	protected static object m_lock;

	private static Dictionary<int, VirtualGamePlayer> m_ringPlayers;

	private static RingStationBattleServer m_server;

	private static readonly string weaklessGuildProgressStr;

	private static VirtualPlayerInfo m_normalPlayer;

	private static string[] _names;

	private static List<VirtualPlayerInfo> m_vplayers;

	private static Dictionary<int, UserRingStationInfo> m_ringstation;

	private static List<UserRingStationInfo> m_ranks;

	private static Dictionary<int, List<RingstationBattleFieldInfo>> m_battleFields;

	private static RingstationConfigInfo m_congfig;

	protected static Timer m_statusScanTimer;

	public static RingstationConfigInfo ConfigInfo => m_congfig;

	public static VirtualPlayerInfo NormalPlayer
	{
		get
		{
			return m_normalPlayer;
		}
		set
		{
			m_normalPlayer = value;
		}
	}

	public static RingStationBattleServer RingStationBattle => m_server;

	static RingStationMgr()
	{
		rand = new Random();
		log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		m_lock = new object();
		m_ringPlayers = new Dictionary<int, VirtualGamePlayer>();
		weaklessGuildProgressStr = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
		m_normalPlayer = new VirtualPlayerInfo();
		m_vplayers = new List<VirtualPlayerInfo>();
		m_ringstation = new Dictionary<int, UserRingStationInfo>();
		m_ranks = new List<UserRingStationInfo>();
		m_battleFields = new Dictionary<int, List<RingstationBattleFieldInfo>>();
		UpdhooLvMxf = new Random();
	}

	public static bool Init()
	{
		bool result = false;
		try
		{
			BattleServer server = BattleMgr.GetServer(1);
			if (server == null)
			{
				return false;
			}
			m_server = new RingStationBattleServer(RingStationConfiguration.ServerID, server.Ip, server.Port, "1,7road");
			if (m_server != null)
			{
				_names = GameProperties.VirtualName.Split(',');
				lock (m_lock)
				{
					m_ringPlayers.Clear();
				}
				m_server.Start();
				if (!SetupVirtualPlayer())
				{
					return false;
				}
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					m_congfig = playerBussiness.GetAllRingstationConfig();
					if (m_congfig == null)
					{
						m_congfig = new RingstationConfigInfo
						{
							buyCount = 10,
							buyPrice = 8000,
							cdPrice = 10000,
							AwardTime = DateTime.Now.AddDays(3.0),
							AwardNum = 450,
							AwardFightWin = "1-50,25|51-100,20|101-1000000,15",
							AwardFightLost = "1-50,15|51-100,10|101-1000000,5",
							ChampionText = "",
							ChallengeNum = 10,
							IsFirstUpdateRank = true
						};
						playerBussiness.AddRingstationConfig(m_congfig);
					}
				}
				result = true;
			}
		}
		catch (Exception exception)
		{
			log.Error("RingStationMgr Init", exception);
		}
		return result;
	}

	public static bool ReLoadBattleField()
	{
		try
		{
			RingstationBattleFieldInfo[] array = LoadRingstationBattleFieldDb();
			Dictionary<int, List<RingstationBattleFieldInfo>> value = LoadRingstationBattleFields(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_battleFields, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad RingstationBattleField", exception);
			}
			return false;
		}
		return true;
	}

	public static RingstationBattleFieldInfo[] LoadRingstationBattleFieldDb()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		return playerBussiness.GetAllRingstationBattleField();
	}

	public static Dictionary<int, List<RingstationBattleFieldInfo>> LoadRingstationBattleFields(RingstationBattleFieldInfo[] RingstationBattleField)
	{
		Dictionary<int, List<RingstationBattleFieldInfo>> dictionary = new Dictionary<int, List<RingstationBattleFieldInfo>>();
		foreach (RingstationBattleFieldInfo info in RingstationBattleField)
		{
			if (!dictionary.Keys.Contains(info.UserID))
			{
				IEnumerable<RingstationBattleFieldInfo> source = RingstationBattleField.Where((RingstationBattleFieldInfo s) => s.UserID == info.UserID);
				dictionary.Add(info.UserID, source.ToList());
			}
		}
		return dictionary;
	}

	public static UserRingStationInfo[] GetRingStationRanks()
	{
		List<UserRingStationInfo> list = new List<UserRingStationInfo>();
		foreach (UserRingStationInfo rank in m_ranks)
		{
			list.Add(rank);
			if (list.Count >= 50)
			{
				break;
			}
		}
		return list.ToArray();
	}

	public static bool UpdateRingBattleFields(RingstationBattleFieldInfo dareFlag, RingstationBattleFieldInfo successFlag)
	{
		int num = -1;
		int num2 = -1;
		int rank = 0;
		int rank2 = 0;
		bool flag = false;
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			if (dareFlag != null)
			{
				num = dareFlag.UserID;
			}
			if (successFlag != null)
			{
				num2 = successFlag.UserID;
			}
			UserRingStationInfo singleRingStationInfos = GetSingleRingStationInfos(num);
			if (singleRingStationInfos != null)
			{
				if (singleRingStationInfos.Rank == 0)
				{
					singleRingStationInfos.Rank = m_ranks.Count + 1;
				}
				if (singleRingStationInfos.ChallengeNum > 0)
				{
					singleRingStationInfos.ChallengeNum--;
					singleRingStationInfos.ChallengeTime = DateTime.Now;
					singleRingStationInfos.ChallengeTime = DateTime.Now.AddMinutes(10.0);
				}
				if (dareFlag != null && dareFlag.SuccessFlag)
				{
					singleRingStationInfos.Total++;
				}
				rank = singleRingStationInfos.Rank;
			}
			UserRingStationInfo singleRingStationInfos2 = GetSingleRingStationInfos(num2);
			if (singleRingStationInfos2 != null)
			{
				rank2 = singleRingStationInfos2.Rank;
			}
			if (dareFlag != null)
			{
				if (singleRingStationInfos != null)
				{
					if (dareFlag.SuccessFlag && singleRingStationInfos2 != null && singleRingStationInfos.Rank > singleRingStationInfos2.Rank)
					{
						singleRingStationInfos.Rank = rank2;
						singleRingStationInfos2.Rank = rank;
						flag = true;
					}
					UpdateRingStationInfo(singleRingStationInfos);
				}
				lock (m_lock)
				{
					if (m_battleFields.ContainsKey(num))
					{
						playerBussiness.AddRingstationBattleField(dareFlag);
						m_battleFields[num].Add(dareFlag);
					}
					else
					{
						playerBussiness.AddRingstationBattleField(dareFlag);
						List<RingstationBattleFieldInfo> value = new List<RingstationBattleFieldInfo> { dareFlag };
						m_battleFields.Add(num, value);
					}
				}
			}
			if (successFlag != null)
			{
				if (singleRingStationInfos2 != null)
				{
					singleRingStationInfos2.OnFight = false;
					UpdateRingStationFight(singleRingStationInfos2);
				}
				lock (m_lock)
				{
					if (m_battleFields.ContainsKey(num2))
					{
						playerBussiness.AddRingstationBattleField(successFlag);
						m_battleFields[num2].Add(successFlag);
					}
					else
					{
						playerBussiness.AddRingstationBattleField(successFlag);
						List<RingstationBattleFieldInfo> value = new List<RingstationBattleFieldInfo> { successFlag };
						m_battleFields.Add(num2, value);
					}
				}
			}
			if (flag)
			{
				if (dareFlag.Level == singleRingStationInfos.Rank)
				{
					dareFlag.Level = 0;
				}
				else
				{
					dareFlag.Level = singleRingStationInfos.Rank;
				}
				UpdateRingStationInfo(singleRingStationInfos);
				if (successFlag != null)
				{
					successFlag.Level = ((successFlag.Level != singleRingStationInfos2.Rank) ? singleRingStationInfos2.Rank : 0);
				}
				UpdateRingStationInfo(singleRingStationInfos2);
			}
		}
		return true;
	}

	public static RingstationBattleFieldInfo[] GetRingBattleFields(int playerId)
	{
		List<RingstationBattleFieldInfo> list = new List<RingstationBattleFieldInfo>();
		lock (m_lock)
		{
			if (m_battleFields.ContainsKey(playerId))
			{
				List<RingstationBattleFieldInfo> collection = m_battleFields[playerId];
				list.AddRange(collection);
			}
		}
		return list.OrderByDescending((RingstationBattleFieldInfo pair) => pair.BattleTime).Take(10).ToArray();
	}

	public static bool ReLoadUserRingStation()
	{
		try
		{
			UserRingStationInfo[] array = LoadUserRingStationDb();
			Dictionary<int, UserRingStationInfo> value = LoadUserRingStations(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_ringstation, value);
				m_ranks = (from pair in array
					where pair.Rank != 0
					orderby pair.Rank
					select pair).ToList();
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad All UserRingStation", exception);
			}
			return false;
		}
		return true;
	}

	public static UserRingStationInfo[] LoadUserRingStationDb()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		return playerBussiness.GetAllUserRingStation();
	}

	public static Dictionary<int, UserRingStationInfo> LoadUserRingStations(UserRingStationInfo[] UserRingStation)
	{
		Dictionary<int, UserRingStationInfo> dictionary = new Dictionary<int, UserRingStationInfo>();
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			foreach (UserRingStationInfo userRingStationInfo in UserRingStation)
			{
				if (dictionary.Keys.Contains(userRingStationInfo.UserID))
				{
					continue;
				}
				try
				{
					userRingStationInfo.Info = playerBussiness.GetUserSingleByUserID(userRingStationInfo.UserID);
					if (userRingStationInfo.Info != null)
					{
						userRingStationInfo.WeaponID = GetWeaponId(userRingStationInfo.Info.Style);
						dictionary.Add(userRingStationInfo.UserID, userRingStationInfo);
					}
				}
				catch
				{
				}
			}
		}
		return dictionary;
	}

	public static void LoadRingStationInfo(PlayerInfo player, int dame, int guard)
	{
		if (player == null)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		if (m_ringstation.ContainsKey(player.ID))
		{
			bool flag = false;
			UserRingStationInfo userRingStationInfo = m_ringstation[player.ID];
			if (dame != userRingStationInfo.BaseDamage && userRingStationInfo.BaseGuard != guard)
			{
				userRingStationInfo.BaseDamage = dame;
				userRingStationInfo.BaseGuard = guard;
				userRingStationInfo.BaseEnergy = (int)(1.0 - (double)player.Agility * 0.001);
				flag = true;
			}
			int weaponId = GetWeaponId(player.Style);
			if (userRingStationInfo.WeaponID != weaponId)
			{
				userRingStationInfo.WeaponID = weaponId;
				flag = true;
			}
			if (flag)
			{
				playerBussiness.UpdateUserRingStation(userRingStationInfo);
			}
		}
		else
		{
			UserRingStationInfo userRingStationInfo2 = new UserRingStationInfo
			{
				UserID = player.ID,
				WeaponID = GetWeaponId(player.Style),
				BaseDamage = dame,
				BaseGuard = guard,
				BaseEnergy = 500,
				signMsg = LanguageMgr.GetTranslation("RingStation.signMsg"),
				ChallengeNum = ConfigInfo.ChallengeNum,
				buyCount = ConfigInfo.buyCount,
				ChallengeTime = DateTime.Now,
				LastDate = DateTime.Now,
				Info = player
			};
			playerBussiness.AddUserRingStation(userRingStationInfo2);
			m_ringstation.Add(player.ID, userRingStationInfo2);
		}
	}

	public static int GetWeaponId(string style)
	{
		if (!string.IsNullOrEmpty(style))
		{
			string[] array = style.Split(',');
			string text = array[6];
			if (text.IndexOf("|", StringComparison.Ordinal) != -1)
			{
				return int.Parse(text.Split('|')[0]);
			}
		}
		return 7008;
	}

	public static UserRingStationInfo GetRingStationChallenge(int playerId, int rank, ref bool isAutoBot)
	{
		lock (m_lock)
		{
			if (m_ringstation.ContainsKey(playerId) && rank != 0)
			{
				return m_ringstation[playerId];
			}
		}
		isAutoBot = true;
		return BaseRingStationChallenges(playerId);
	}

	public static void SetChallenge(int playerId, bool onFight)
	{
		lock (m_lock)
		{
			if (m_ringstation.ContainsKey(playerId))
			{
				m_ringstation[playerId].OnFight = onFight;
			}
		}
	}

	public static UserRingStationInfo GetSingleRingStationInfos(int playerId)
	{
		lock (m_lock)
		{
			if (m_ringstation.ContainsKey(playerId))
			{
				return m_ringstation[playerId];
			}
		}
		return null;
	}

	public static bool UpdateRingStationInfo(UserRingStationInfo ring)
	{
		if (ring == null)
		{
			return false;
		}
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			lock (m_lock)
			{
				if (m_ringstation.ContainsKey(ring.UserID))
				{
					m_ringstation[ring.UserID] = ring;
					return playerBussiness.UpdateUserRingStation(ring);
				}
			}
		}
		return false;
	}

	public static bool UpdateRingStationFight(UserRingStationInfo ring)
	{
		if (ring == null)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_ringstation.ContainsKey(ring.UserID))
			{
				m_ringstation[ring.UserID] = ring;
				return true;
			}
		}
		return false;
	}

	public static List<UserRingStationInfo> FindRingStationInfoByRank(int userId, int min, int max)
	{
		return (from info in m_ringstation.Values
			where info.UserID != userId
			where info.Rank >= min && info.Rank <= max
			select info).ToList();
	}

	public static UserRingStationInfo[] GetRingStationInfos(int userId, int rank)
	{
		NormalPlayer = GetVirtualPlayerInfo();
		Dictionary<int, UserRingStationInfo> dictionary = new Dictionary<int, UserRingStationInfo>();
		int num = 5;
		if (rank > 0)
		{
			int num2 = rank;
			int num3 = rank - num;
			if (num2 <= 0)
			{
				num2 = m_ranks.Count;
				num3 = m_ranks.Count - num;
			}
			else if (num3 <= 0)
			{
				num2 = 1;
				num3 = num;
			}
			List<UserRingStationInfo> list = FindRingStationInfoByRank(userId, num2, num3);
			if (list.Count == 4)
			{
				int num4 = 0;
				while (dictionary.Count < 4)
				{
					UserRingStationInfo userRingStationInfo = list[rand.Next(list.Count)];
					if (userRingStationInfo != null)
					{
						if (!dictionary.ContainsKey(userRingStationInfo.UserID))
						{
							dictionary.Add(userRingStationInfo.UserID, userRingStationInfo);
						}
						list.Remove(userRingStationInfo);
					}
					num4++;
				}
			}
		}
		if (dictionary.Count == 0)
		{
			UserRingStationInfo userRingStationInfo2 = BaseRingStationChallenges(0);
			dictionary.Add(userRingStationInfo2.Info.ID, userRingStationInfo2);
		}
		return dictionary.Values.ToArray();
	}

	public static bool AddPlayer(int playerId, VirtualGamePlayer player)
	{
		lock (m_lock)
		{
			if (m_ringPlayers.ContainsKey(playerId))
			{
				return true;
			}
			m_ringPlayers.Add(playerId, player);
		}
		return true;
	}

	public static bool RemovePlayer(int playerId)
	{
		lock (m_lock)
		{
			if (m_ringPlayers.ContainsKey(playerId))
			{
				return m_ringPlayers.Remove(playerId);
			}
		}
		return false;
	}

	public static VirtualGamePlayer GetPlayerById(int playerId)
	{
		VirtualGamePlayer result = null;
		lock (m_lock)
		{
			if (m_ringPlayers.ContainsKey(playerId))
			{
				result = m_ringPlayers[playerId];
			}
		}
		return result;
	}

	public static void BeginTimer()
	{
		int num = 60000;
		if (m_statusScanTimer == null)
		{
			m_statusScanTimer = new Timer(StatusScan, null, num, num);
		}
		else
		{
			m_statusScanTimer.Change(num, num);
		}
	}

	protected static void StatusScan(object sender)
	{
		try
		{
			log.Info("Begin Scan RingStation Info....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			bool flag = false;
			if (ReLoadUserRingStation())
			{
				List<UserRingStationInfo> list = m_ringstation.Values.ToList();
				if (ConfigInfo.IsFirstUpdateRank && list.Count > 10)
				{
					List<UserRingStationInfo> list2 = list.OrderByDescending((UserRingStationInfo pair) => pair.Total).ToList();
					for (int i = 0; i < list2.Count; i++)
					{
						UserRingStationInfo userRingStationInfo = list2[i];
						userRingStationInfo.Rank = i + 1;
						UpdateRingStationInfo(userRingStationInfo);
					}
					ConfigInfo.IsFirstUpdateRank = false;
					flag = true;
				}
				m_ranks = (from pair in list
					where pair.Rank != 0
					orderby pair.Rank
					select pair).ToList();
				if (m_ranks.Count > 0)
				{
					UserRingStationInfo userRingStationInfo2 = m_ranks[0];
					if (userRingStationInfo2.Info != null)
					{
						ConfigInfo.ChampionText = userRingStationInfo2.Info.NickName;
						flag = true;
					}
				}
				if (ConfigInfo.IsEndTime())
				{
					lock (m_lock)
					{
						m_congfig.AwardTime = DateTime.Now;
						m_congfig.AwardTime = DateTime.Now.AddDays(3.0);
						flag = true;
					}
					if (list.Count > 0)
					{
						foreach (UserRingStationInfo item in list)
						{
							item.ReardEnable = true;
							UpdateRingStationInfo(item);
						}
					}
				}
				if (flag)
				{
					using PlayerBussiness playerBussiness = new PlayerBussiness();
					playerBussiness.UpdateRingstationConfig(ConfigInfo);
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End Scan RingStation Info....");
		}
		catch (Exception exception)
		{
			log.Error("StatusScan ", exception);
		}
	}

    public static bool SetupVirtualPlayer()
    {
        int[] weaponArr = new int[]
        {
                7015, 7016, 7017, 7018, 7019, 7020, 7021, 7022, 7023, 7026, 7027, 7028, 7031, 7032, 7031, 7046, 7047,
                7048, 7049, 7050, 7051, 7053, 7058, 7059, 7060,
                70151, 70161, 70171, 70181, 70191, 70201, 70211, 70221, 70231, 70261, 70271, 70281, 70311, 70321, 70311,
                70461, 70471, 70481, 70491, 70501, 70511, 70531, 70581, 70591, 70601,
                70152, 70162, 70172, 70182, 70192, 70202, 70212, 70222, 70232, 70262, 70272, 70282, 70312, 70322, 70312,
                70462, 70472, 70482, 70492, 70502, 70512, 70532, 70582, 70592, 70602,
                70153, 70163, 70173, 70183, 70193, 70203, 70213, 70223, 70233, 70263, 70273, 70283, 70313, 70323, 70313,
                70463, 70473, 70483, 70493, 70503, 70513, 70533, 70583, 70593, 70603,
                70154, 70164, 70174, 70184, 70194, 70204, 70214, 70224, 70234, 70264, 70274, 70284, 70314, 70324, 70314,
                70464, 70474, 70484, 70494, 70504, 70514, 70534, 70584, 70594, 70604
        };
        int[] headArr = new int[]
        {
                1515, 1517, 1522, 1523, 1524, 1525, 1528, 1539, 1542, 1544, 1545, 1550, 1551, 1552, 1555, 1576, 1577,
                1578,
                1582, 1588, 1592, 1593, 1594, 1595, 1596, 1598
        };
        int[] glassArr = new int[]
        {
                2324, 2332, 2334, 2335, 2336, 2337, 2338, 2345, 2348, 2354, 2357, 2400, 2401, 2402, 2403, 2404, 2405,
                2407,
                2408, 2409, 2413, 2416, 2417, 2420, 2422, 2423
        };
        int[] hairArr = new int[]
        {
                3309, 3310, 3311, 3312, 3314, 3315, 3316, 3317, 3318, 3319, 3320, 3321, 3322, 3323, 3324, 3325, 3326,
                3330,
                3331, 3333, 3334, 3335, 3336, 3337, 3338, 3339
        };
        int[] effArr = new int[]
        {
                4321, 4323, 4327, 4329, 4332, 4338, 4339, 4347, 4351, 4353, 4355, 4401, 4402, 4403, 4404, 4405, 4406,
                4407,
                4408, 4409, 4410, 4412, 4413, 4414, 4415, 4416
        };
        int[] clothArr = new int[]
        {
                5569, 5570, 5572, 5575, 5576, 5577, 5578, 5579, 5580, 5582, 5583, 5584, 5585, 5586, 5588, 5589, 5591,
                5592,
                5594, 5596, 5600, 5601, 5602, 5603, 5604, 5605
        };
        int[] faceArr = new int[]
        {
                6255, 6256, 6257, 6258, 6259, 6260, 6261, 6262, 6263, 6264, 6265, 6266, 6267, 6268, 6269, 6270, 6271,
                6272,
                6273, 6274, 6275, 6276, 6277, 6278, 6279, 6280
        };
        int[] wingArr = new int[]
        {
                15009, 15018, 15019, 15020, 15021, 15022, 15023, 15024, 15025, 15026, 15027, 15028, 15029, 15049, 15031,
                15032, 15033, 15034, 15035, 15036, 15037, 15038, 15039, 15040, 15041, 15042
        };
        int count = weaponArr.Length;
        int h = 0, g = 0, ha = 0, e = 0, c = 0, f = 0, w = 0;
        for (int i = 0; i < count; i++)
        {
            ItemTemplateInfo temwe = ItemMgr.FindItemTemplate(weaponArr[i]);
            ItemTemplateInfo temhe = ItemMgr.FindItemTemplate(headArr[h]);
            ItemTemplateInfo temgl = ItemMgr.FindItemTemplate(glassArr[g]);
            ItemTemplateInfo temha = ItemMgr.FindItemTemplate(hairArr[ha]);
            ItemTemplateInfo temef = ItemMgr.FindItemTemplate(effArr[e]);
            ItemTemplateInfo temcl = ItemMgr.FindItemTemplate(clothArr[c]);
            ItemTemplateInfo temfa = ItemMgr.FindItemTemplate(faceArr[f]);
            ItemTemplateInfo temwi = ItemMgr.FindItemTemplate(wingArr[w]);
            if (temwe != null && temhe != null && temgl != null && temha != null && temef != null &&
                temcl != null && temfa != null && temwi != null)
            {
                string swe = $"{weaponArr[i]}|{temwe.Pic}";
                string she = $"{headArr[h]}|{temhe.Pic}";
                string sgl = $"{glassArr[g]}|{temgl.Pic}";
                string sha = $"{hairArr[ha]}|{temha.Pic}";
                string sef = $"{effArr[e]}|{temef.Pic}";
                string scl = $"{clothArr[c]}|{temcl.Pic}";
                string sfa = $"{faceArr[f]}|{temfa.Pic}";
                string swi = $"{wingArr[w]}|{temwi.Pic}";
                string style = $"{she},{sgl},{sha},{sef},{scl},{sfa},{swe},,{swi},,,,,,,,,";

                VirtualPlayerInfo info = new VirtualPlayerInfo
                {
                    Style = style,
                    Weapon = weaponArr[i]
                };
                m_vplayers.Add(info);
            }

            h++;
            g++;
            ha++;
            e++;
            c++;
            f++;
            w++;
            if (h > headArr.Length - 1)
                h = 0;
            if (g > glassArr.Length - 1)
                g = 0;
            if (ha > hairArr.Length - 1)
                ha = 0;
            if (e > effArr.Length - 1)
                e = 0;
            if (c > clothArr.Length - 1)
                c = 0;
            if (f > faceArr.Length - 1)
                f = 0;
            if (w > wingArr.Length - 1)
                w = 0;
        }

        return m_vplayers.Count > Math.Abs(count / 2);
    }

	public static VirtualPlayerInfo GetVirtualPlayerInfo()
	{
		int index = rand.Next(m_vplayers.Count);
		return m_vplayers[index];
	}

	public static int CreateRingStationChallenge(UserRingStationInfo player, int roomtype, int gametype)
	{
		int iD = player.Info.ID;
		BaseRoomRingStation baseRoomRingStation = new BaseRoomRingStation(RingStationConfiguration.NextRoomId())
		{
			RoomType = roomtype,
			GameType = gametype,
			PickUpNpcId = iD,
			IsAutoBot = true,
			IsFreedom = false
		};
		VirtualGamePlayer virtualGamePlayer = new VirtualGamePlayer
		{
			NickName = player.Info.NickName,
			GP = ((player.Info.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(player.Info.GP)),
			Grade = player.Info.Grade,
			Attack = player.Info.Attack,
			Defence = player.Info.Defence,
			Luck = player.Info.Luck,
			Agility = player.Info.Agility,
			hp = player.Info.hp,
			FightPower = player.Info.FightPower,
			BaseAttack = player.BaseDamage,
			BaseDefence = player.BaseGuard,
			BaseAgility = player.BaseEnergy,
			BaseBlood = player.Info.hp,
			Style = player.Info.Style,
			Colors = player.Info.Colors,
			Hide = player.Info.Hide,
			TemplateID = player.WeaponID,
			StrengthLevel = 1,
			WeaklessGuildProgressStr = weaklessGuildProgressStr,
			ID = iD
		};
		if (m_server != null)
		{
			AddPlayer(virtualGamePlayer.ID, virtualGamePlayer);
			baseRoomRingStation.AddPlayer(virtualGamePlayer);
			m_server.AddRoom(baseRoomRingStation);
		}
		return iD;
	}

	public static void CreateAutoBot(GamePlayer player, int roomtype, int gametype, int npcId, int playerCount)
	{
		BaseRoomRingStation baseRoomRingStation = new BaseRoomRingStation(RingStationConfiguration.NextRoomId())
		{
			RoomType = roomtype,
			GameType = gametype,
			PickUpNpcId = npcId,
			IsAutoBot = true,
			IsFreedom = true
		};
		for (int i = 0; i < playerCount; i++)
		{
			VirtualGamePlayer virtualGamePlayer = new VirtualGamePlayer
			{
				NickName = _names[rand.Next(_names.Length)] + npcId + i,
				GP = ((player.PlayerCharacter.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(player.PlayerCharacter.GP)),
				Grade = player.PlayerCharacter.Grade,
				Attack = player.PlayerCharacter.Attack + player.PlayerCharacter.MagicAttack,
				Defence = player.PlayerCharacter.Defence + player.PlayerCharacter.MagicDefence,
				Luck = player.PlayerCharacter.Luck,
				Agility = player.PlayerCharacter.Agility,
				hp = player.PlayerCharacter.hp,
				FightPower = player.PlayerCharacter.FightPower,
				BaseAttack = player.BaseAttack,
				BaseDefence = player.BaseDefence,
				BaseAgility = player.GetBaseAgility(),
				BaseBlood = player.PlayerCharacter.hp
			};
			VirtualPlayerInfo virtualPlayerInfo = GetVirtualPlayerInfo();
			virtualGamePlayer.Style = virtualPlayerInfo.Style;
			virtualGamePlayer.Colors = ",,,,,,,,,,,,,,,";
			virtualGamePlayer.Hide = 1111111111;
			virtualGamePlayer.TemplateID = virtualPlayerInfo.Weapon;
			virtualGamePlayer.StrengthLevel = player.MainWeapon.StrengthenLevel;
			virtualGamePlayer.WeaklessGuildProgressStr = weaklessGuildProgressStr;
			virtualGamePlayer.ID = npcId;
			AddPlayer(virtualGamePlayer.ID, virtualGamePlayer);
			baseRoomRingStation.AddPlayer(virtualGamePlayer);
		}
		if (m_server != null)
		{
			m_server.AddRoom(baseRoomRingStation);
		}
	}

	public static int GetAutoBot(GamePlayer player, int roomtype, int gametype, int playerCount)
	{
		int num = RingStationConfiguration.NextPlayerID();
		int[] array = new int[4]
		{
			num,
			num + 1,
			num + 2,
			num + 3
		};
		BaseRoomRingStation baseRoomRingStation = new BaseRoomRingStation(RingStationConfiguration.NextRoomId())
		{
			RoomType = roomtype,
			GameType = gametype,
			PickUpNpcId = num,
			IsAutoBot = true,
			IsFreedom = false
		};
		for (int i = 0; i < playerCount; i++)
		{
			VirtualGamePlayer virtualGamePlayer = new VirtualGamePlayer
			{
				NickName = RingStationConfiguration.RandomName[UpdhooLvMxf.Next(0, RingStationConfiguration.RandomName.Length)],
				Grade = 50,
				Attack = player.PlayerCharacter.Attack * 3 / 2,
				Defence = player.PlayerCharacter.Defence * 3 / 2,
				Luck = player.PlayerCharacter.Luck * 3 / 2,
				Agility = player.PlayerCharacter.Agility * 8 / 10,
				MagicAttack = player.PlayerCharacter.MagicAttack * 3 / 2,
				MagicDefence = player.PlayerCharacter.MagicDefence * 3 / 2,
				FireAttack = player.PlayerCharacter.ElementPro.FireAttack * 3 / 2,
				FireDefence = player.PlayerCharacter.ElementPro.FireDefence * 3 / 2,
				WaterAttack = player.PlayerCharacter.ElementPro.WaterAttack * 3 / 2,
				WaterDefence = player.PlayerCharacter.ElementPro.WaterDefence * 3 / 2,
				LandAttack = player.PlayerCharacter.ElementPro.LandAttack * 3 / 2,
				LandDefence = player.PlayerCharacter.ElementPro.LandDefence * 3 / 2,
				WindAttack = player.PlayerCharacter.ElementPro.WindAttack * 3 / 2,
				WindDefence = player.PlayerCharacter.ElementPro.WindDefence * 3 / 2,
				hp = player.PlayerCharacter.hp * 3 / 2,
				FightPower = player.PlayerCharacter.FightPower,
				BaseAttack = player.GetBaseAttack(),
				BaseDefence = player.GetBaseAttack(),
				BaseAgility = player.GetBaseAgility(),
				BaseBlood = player.PlayerCharacter.hp,
				VIPLevel = 15
			};
			VirtualPlayerInfo virtualPlayerInfo = GetVirtualPlayerInfo();
			virtualGamePlayer.Style = virtualPlayerInfo.Style;
			virtualGamePlayer.Colors = ",,,,,,,,,,,,,,,";
			virtualGamePlayer.Hide = 1111111111;
			virtualGamePlayer.TemplateID = virtualPlayerInfo.Weapon;
			virtualGamePlayer.StrengthLevel = 15;
			virtualGamePlayer.WeaklessGuildProgressStr = weaklessGuildProgressStr;
			virtualGamePlayer.ID = array[i];
			AddPlayer(virtualGamePlayer.ID, virtualGamePlayer);
			baseRoomRingStation.AddPlayer(virtualGamePlayer);
		}
		if (m_server != null)
		{
			m_server.AddRoom(baseRoomRingStation);
		}
		return num;
	}

	public static void CreateBaseAutoBot(int roomtype, int gametype, int npcId)
	{
		BaseRoomRingStation baseRoomRingStation = new BaseRoomRingStation(RingStationConfiguration.NextRoomId())
		{
			RoomType = roomtype,
			GameType = gametype,
			PickUpNpcId = npcId,
			IsAutoBot = true,
			IsFreedom = true
		};
		VirtualGamePlayer virtualGamePlayer = new VirtualGamePlayer
		{
			NickName = _names[rand.Next(_names.Length)] + npcId,
			GP = 1283,
			Grade = 5,
			Attack = 100,
			Defence = 100,
			Luck = 100,
			Agility = 100,
			hp = 3000,
			FightPower = 1200L,
			BaseAttack = 200.0,
			BaseDefence = 120.0,
			BaseAgility = 540.0,
			BaseBlood = 1000.0
		};
		VirtualPlayerInfo virtualPlayerInfo = GetVirtualPlayerInfo();
		virtualGamePlayer.Style = virtualPlayerInfo.Style;
		virtualGamePlayer.Colors = ",,,,,,,,,,,,,,,";
		virtualGamePlayer.Hide = 1111111111;
		virtualGamePlayer.TemplateID = virtualPlayerInfo.Weapon;
		virtualGamePlayer.StrengthLevel = 0;
		virtualGamePlayer.WeaklessGuildProgressStr = "R/O/DeABAtgWdWsIAAAAAAAAgCAECwAAAAAAABgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
		virtualGamePlayer.ID = npcId;
		if (m_server != null)
		{
			AddPlayer(virtualGamePlayer.ID, virtualGamePlayer);
			baseRoomRingStation.AddPlayer(virtualGamePlayer);
			m_server.AddRoom(baseRoomRingStation);
		}
	}

	public static UserRingStationInfo BaseRingStationChallenges(int id)
	{
		UserRingStationInfo userRingStationInfo = new UserRingStationInfo
		{
			Rank = 0,
			WeaponID = NormalPlayer.Weapon,
			signMsg = LanguageMgr.GetTranslation("BaseRingStationChallenges.Msg2"),
			BaseDamage = 242,
			BaseGuard = 120,
			BaseEnergy = 540
		};
		PlayerInfo playerInfo2 = (userRingStationInfo.Info = new PlayerInfo
		{
			ID = ((id == 0) ? RingStationConfiguration.NextPlayerID() : id),
			UserName = "NormalInfo",
			NickName = LanguageMgr.GetTranslation("BaseRingStationChallenges.Msg1"),
			typeVIP = 1,
			VIPLevel = 1,
			Grade = 25,
			Sex = false,
			Style = NormalPlayer.Style,
			Colors = ",,,,,,,,,,,,,,,",
			Skin = "",
			ConsortiaName = "",
			Hide = 1111111111,
			Offer = 0,
			Win = 0,
			Total = 0,
			Escape = 0,
			Repute = 0,
			Nimbus = 0,
			GP = 1437053L,
			FightPower = 14370,
			AchievementPoint = 0,
			Attack = 225,
			Defence = 160,
			Agility = 50,
			Luck = 60,
			hp = 3500,
			IsAutoBot = true
		});
		return userRingStationInfo;
	}

	public static void StopAllTimer()
	{
		if (m_statusScanTimer != null)
		{
			m_statusScanTimer.Change(-1, -1);
			m_statusScanTimer.Dispose();
			m_statusScanTimer = null;
		}
	}

	public static List<VirtualGamePlayer> GetAllPlayer()
	{
		List<VirtualGamePlayer> list = new List<VirtualGamePlayer>();
		lock (m_lock)
		{
			foreach (VirtualGamePlayer value in m_ringPlayers.Values)
			{
				list.Add(value);
			}
		}
		return list;
	}
}

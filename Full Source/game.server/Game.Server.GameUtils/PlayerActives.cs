using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GypsyShop;
using Game.Server.Managers;
using Game.Server.Quests;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerActives
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected Timer _christmasTimer;

	protected Timer _labyrinthTimer;

	protected Timer _lightriddleTimer;

	protected GamePlayer m_player;

	private PyramidConfigInfo m_pyramidConfig;

	private PyramidInfo m_pyramid;

	private UserChristmasInfo m_christmas;

	private ActiveSystemInfo m_activeInfo;

	private NewChickenBoxItemInfo[] m_ChickenBoxRewards;

	private List<NewChickenBoxItemInfo> m_removeChickenBoxRewards;

	private int m_flushPrice;

	private int[] m_eagleEyePrice;

	private int[] m_openCardPrice;

	private int m_freeFlushTime;

	private ThreadSafeRandom rand = new ThreadSafeRandom();

	private bool m_saveToDb;

	private readonly int defaultCoins = 1000;

	private readonly int flushCoins = 15;

	private readonly int ChikenBoxCount = 18;

	private readonly int LuckyStartBoxCount = 14;

	public readonly int coinTemplateID = 201193;

	private int m_labyrinthCountDown = GameProperties.WarriorFamRaidTimeRemain;

	private List<GodCardUser> m_listCards;

	private List<GodCardGroupUser> m_listExchanges;

	private List<int> m_listAwards;

	private BoguAdventureDataInfo m_boguAdventure;

	private BoguCeilInfo[] m_boguAdventureCell;

	private int[] m_awardCount;

	private readonly int NeedBox1GoguAward = GameProperties.NeedBox1GoguAward;

	private readonly int NeedBox2GoguAward = GameProperties.NeedBox2GoguAward;

	private readonly int NeedBox3GoguAward = GameProperties.NeedBox3GoguAward;

	public readonly int findMinePrice = GameProperties.FindMinePrice;

	public readonly int revivePrice = GameProperties.RevivePrice;

	public readonly int resetPrice = GameProperties.ResetPrice;

	public readonly int SXColumnLength = 7;

	public readonly int SXRowLength = 7;

	public readonly int MemoryGameMaxCell = 16;

	public readonly int halloweenCard = 201453;

	public readonly int MINE = -1;

	public readonly int SPACE = -2;

	public readonly int SIGN = 1;

	public readonly int OPEN = 2;

	public readonly int NOT_OPEN = 3;

	private readonly int MAP_WIDTH = 10;

	private readonly int MAP_HEIGHT = 7;

	private TreasurePuzzleDataInfo[] m_treasurepuzzle;

	private readonly int holeNeed = 20;

	private GypsyItemDataInfo[] m_mysteryShop;

	public int QXDropId = 0;

	public int DropId = 0;

	private CryptBossItemInfo[] m_cryptBoss;

	private readonly string[] cryptBossOpenDay = GameProperties.CryptBossOpenDay.Split('|');

	private readonly int[] m_cryptBossAward1 = new int[5] { 1120186, 1120187, 1120188, 1120189, 1120190 };

	private readonly int[] m_cryptBossAward2 = new int[5] { 1120191, 1120192, 1120193, 1120194, 1120195 };

	private readonly int[] m_cryptBossAward3 = new int[5] { 1120196, 1120197, 1120198, 1120199, 1120200 };

	private readonly int[] m_cryptBossAward4 = new int[5] { 1120201, 1120202, 1120203, 1120204, 1120205 };

	private readonly int[] m_cryptBossAward5 = new int[5] { 1120206, 1120207, 1120208, 1120209, 1120210 };

	private readonly int[] m_cryptBossAward6 = new int[5] { 1120211, 1120212, 1120213, 1120214, 1120215 };

	private int m_freeRefreshBoxCount;

	private int m_freeEyeCount;

	private int m_freeOpenCardCount;

	private DateTime m_luckyBegindate;

	private DateTime m_luckyEnddate;

	private int m_minUseNum;

	private NewChickenBoxItemInfo[] m_LuckyStartRewards;

	private NewChickenBoxItemInfo m_award;

	public DateTime LuckyStartStartTurn;

	private int _lightriddleColdown = 15;

	private bool _lightriddleStart = false;

	public GamePlayer Player => m_player;

	public PyramidConfigInfo PyramidConfig
	{
		get
		{
			return m_pyramidConfig;
		}
		set
		{
			m_pyramidConfig = value;
		}
	}

	public PyramidInfo Pyramid
	{
		get
		{
			return m_pyramid;
		}
		set
		{
			m_pyramid = value;
		}
	}

	public UserChristmasInfo Christmas
	{
		get
		{
			return m_christmas;
		}
		set
		{
			m_christmas = value;
		}
	}

	public ActiveSystemInfo Info
	{
		get
		{
			return m_activeInfo;
		}
		set
		{
			m_activeInfo = value;
		}
	}

	public NewChickenBoxItemInfo[] ChickenBoxRewards
	{
		get
		{
			return m_ChickenBoxRewards;
		}
		set
		{
			m_ChickenBoxRewards = value;
		}
	}

	public int flushPrice
	{
		get
		{
			return m_flushPrice;
		}
		set
		{
			m_flushPrice = value;
		}
	}

	public int[] eagleEyePrice
	{
		get
		{
			return m_eagleEyePrice;
		}
		set
		{
			m_eagleEyePrice = value;
		}
	}

	public int[] openCardPrice
	{
		get
		{
			return m_openCardPrice;
		}
		set
		{
			m_openCardPrice = value;
		}
	}

	public int freeFlushTime
	{
		get
		{
			return m_freeFlushTime;
		}
		set
		{
			m_freeFlushTime = value;
		}
	}

	public List<GodCardUser> ListCards => m_listCards;

	public List<GodCardGroupUser> ListExchanges => m_listExchanges;

	public List<int> ListAwards => m_listAwards;

	public BoguAdventureDataInfo BoguAdventure
	{
		get
		{
			return m_boguAdventure;
		}
		set
		{
			m_boguAdventure = value;
		}
	}

	public BoguCeilInfo[] BoguAdventureCell
	{
		get
		{
			return m_boguAdventureCell;
		}
		set
		{
			m_boguAdventureCell = value;
		}
	}

	public int[] AwardCount
	{
		get
		{
			return m_awardCount;
		}
		set
		{
			m_awardCount = value;
		}
	}

	public TreasurePuzzleDataInfo[] Treasurepuzzle
	{
		get
		{
			return m_treasurepuzzle;
		}
		set
		{
			m_treasurepuzzle = value;
		}
	}

	public GypsyItemDataInfo[] MysteryShop
	{
		get
		{
			return m_mysteryShop;
		}
		set
		{
			m_mysteryShop = value;
		}
	}

	public CryptBossItemInfo[] CryptBoss => m_cryptBoss;

	public UserRewardQuestInfo RewardTask { get; set; }

	public int freeRefreshBoxCount
	{
		get
		{
			return m_freeRefreshBoxCount;
		}
		set
		{
			m_freeRefreshBoxCount = value;
		}
	}

	public int freeEyeCount
	{
		get
		{
			return m_freeEyeCount;
		}
		set
		{
			m_freeEyeCount = value;
		}
	}

	public int freeOpenCardCount
	{
		get
		{
			return m_freeOpenCardCount;
		}
		set
		{
			m_freeOpenCardCount = value;
		}
	}

	public DateTime LuckyBegindate
	{
		get
		{
			return m_luckyBegindate;
		}
		set
		{
			m_luckyBegindate = value;
		}
	}

	public DateTime LuckyEnddate
	{
		get
		{
			return m_luckyEnddate;
		}
		set
		{
			m_luckyEnddate = value;
		}
	}

	public int minUseNum
	{
		get
		{
			return m_minUseNum;
		}
		set
		{
			m_minUseNum = value;
		}
	}

	public NewChickenBoxItemInfo Award
	{
		get
		{
			return m_award;
		}
		set
		{
			m_award = value;
		}
	}

	public bool LightriddleStart => _lightriddleStart;

	public PlayerActives(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_eagleEyePrice = GameProperties.ConvertStringArrayToIntArray("NewChickenEagleEyePrice");
		m_openCardPrice = GameProperties.ConvertStringArrayToIntArray("NewChickenOpenCardPrice");
		m_flushPrice = GameProperties.NewChickenFlushPrice;
		m_freeFlushTime = 120;
		m_removeChickenBoxRewards = new List<NewChickenBoxItemInfo>();
		m_freeEyeCount = 0;
		m_freeOpenCardCount = 0;
		m_freeRefreshBoxCount = 0;
		SetupPyramidConfig();
		SetupLuckyStart();
		m_listCards = new List<GodCardUser>();
		m_listExchanges = new List<GodCardGroupUser>();
		m_listAwards = new List<int>();
	}

	public int IsAcquireAward(int type)
	{
		int result = 1;
		switch (type)
		{
		case 0:
			if (BoguAdventure.openCount >= NeedBox1GoguAward)
			{
				result = 0;
			}
			if (BoguAdventure.isAcquireAward1 == 1)
			{
				result = 1;
			}
			break;
		case 1:
			if (BoguAdventure.openCount >= NeedBox2GoguAward)
			{
				result = 0;
			}
			if (BoguAdventure.isAcquireAward2 == 1)
			{
				result = 1;
			}
			break;
		case 2:
			if (BoguAdventure.openCount >= NeedBox2GoguAward)
			{
				result = 0;
			}
			if (BoguAdventure.isAcquireAward3 == 1)
			{
				result = 1;
			}
			break;
		}
		return result;
	}

	public int SaveListCard(int Id, int count)
	{
		int num = 0;
		lock (m_lock)
		{
			for (int i = 0; i < ListCards.Count; i++)
			{
				if (m_listCards[i].CardId == Id)
				{
					m_listCards[i].Count += count;
					num = m_listCards[i].Count;
				}
			}
			if (num == 0)
			{
				m_listCards.Add(new GodCardUser
				{
					CardId = Id,
					Count = 1
				});
			}
		}
		return num;
	}

	public int FindCardCount(int Id)
	{
		for (int i = 0; i < ListCards.Count; i++)
		{
			if (ListCards[i].CardId == Id)
			{
				return ListCards[i].Count;
			}
		}
		return 0;
	}

	public bool CheckCard(GodCardGroupDetailInfo[] listItems)
	{
		lock (m_lock)
		{
			foreach (GodCardGroupDetailInfo godCardGroupDetailInfo in listItems)
			{
				for (int j = 0; j < ListCards.Count; j++)
				{
					if (m_listCards[j].CardId == godCardGroupDetailInfo.CardID && (m_listCards[j].Count <= 0 || m_listCards[j].Count < godCardGroupDetailInfo.Number))
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	public int RemoveListCard(int Id, int count)
	{
		lock (m_lock)
		{
			for (int i = 0; i < ListCards.Count; i++)
			{
				if (m_listCards[i].CardId == Id && m_listCards[i].Count > 0 && m_listCards[i].Count >= count)
				{
					m_listCards[i].Count -= count;
					return m_listCards[i].Count;
				}
			}
		}
		return count;
	}

	public int SaveListCardExchanges(int Id, int count)
	{
		int num = 0;
		lock (m_lock)
		{
			for (int i = 0; i < ListExchanges.Count; i++)
			{
				if (m_listExchanges[i].GroupId == Id)
				{
					m_listExchanges[i].Count += count;
					num = m_listExchanges[i].Count;
				}
			}
			if (num == 0)
			{
				m_listExchanges.Add(new GodCardGroupUser
				{
					GroupId = Id,
					Count = 1
				});
			}
		}
		return num;
	}

	public bool CheckCardExchangesCount(GodCardGroupDetailInfo[] groups)
	{
		foreach (GodCardGroupDetailInfo godCardGroupDetailInfo in groups)
		{
			if (FindCardCount(godCardGroupDetailInfo.CardID) <= 0)
			{
				return false;
			}
		}
		return true;
	}

	public int FindCardExchangesCount(int Id)
	{
		for (int i = 0; i < ListExchanges.Count; i++)
		{
			if (ListExchanges[i].GroupId == Id)
			{
				return ListExchanges[i].Count;
			}
		}
		return 0;
	}

	public int CountOpenCanTakeBoxGoguAdventure(int type)
	{
		return type switch
		{
			0 => NeedBox1GoguAward, 
			1 => NeedBox2GoguAward, 
			2 => NeedBox3GoguAward, 
			_ => 0, 
		};
	}

	public virtual bool LoadBoguAdventureFromDatabase()
	{
		if (m_saveToDb)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			try
			{
				m_boguAdventure = playerBussiness.GetAllBoguAdventureDataByID(Player.PlayerCharacter.ID);
				if (m_boguAdventure == null)
				{
					CreateBoguAdventureDataInfo();
					m_boguAdventureCell = CreateRandomBoguMap();
					m_awardCount = new int[MAP_WIDTH * MAP_HEIGHT];
					UpdateAroundMineCount();
				}
				else
				{
					if (string.IsNullOrEmpty(BoguAdventure.awardCount))
					{
						m_awardCount = new int[MAP_WIDTH * MAP_HEIGHT];
					}
					else
					{
						m_awardCount = JsonConvert.DeserializeObject<int[]>(BoguAdventure.awardCount);
					}
					if (string.IsNullOrEmpty(BoguAdventure.cellInfo))
					{
						m_boguAdventureCell = CreateRandomBoguMap();
					}
					else
					{
						m_boguAdventureCell = JsonConvert.DeserializeObject<BoguCeilInfo[]>(BoguAdventure.cellInfo);
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (BoguAdventure.lastEnterGame.Date < DateTime.Now.Date)
				{
					ResetBoguAdventureEveryDay();
				}
				SerializeBoguAdventure();
			}
		}
		return BoguAdventure != null;
	}

	public void SerializeBoguAdventure()
	{
		lock (m_lock)
		{
			if (m_boguAdventure != null)
			{
				if (m_awardCount != null && m_awardCount.Length >= MAP_WIDTH * MAP_HEIGHT)
				{
					m_boguAdventure.awardCount = JsonConvert.SerializeObject(m_awardCount);
				}
				if (BoguAdventureCell != null && BoguAdventureCell.Length >= MAP_WIDTH * MAP_HEIGHT)
				{
					m_boguAdventure.cellInfo = JsonConvert.SerializeObject(BoguAdventureCell);
				}
			}
		}
	}

	public virtual bool LoadDDPlayFromDatabase()
	{
		if (m_saveToDb)
		{
			using (new PlayerBussiness())
			{
				try
				{
				}
				catch
				{
					return false;
				}
			}
		}
		return true;
	}

	public virtual bool LoadGypsyItemDataFromDatabase()
	{
		if (m_saveToDb)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			try
			{
				if (m_mysteryShop == null)
				{
					m_mysteryShop = playerBussiness.GetAllGypsyItemDataByID(Player.PlayerCharacter.ID);
					if (MysteryShop.Length < 8)
					{
						m_mysteryShop = null;
						RefreshMysteryShop();
					}
				}
			}
			catch
			{
				return false;
			}
		}
		return true;
	}

	public virtual void SaveBoguAdventureDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		SerializeBoguAdventure();
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_boguAdventure != null && m_boguAdventure.IsDirty)
			{
				if (BoguAdventure.ID > 0)
				{
					playerBussiness.UpdateBoguAdventureData(BoguAdventure);
				}
				else
				{
					playerBussiness.AddBoguAdventureData(BoguAdventure);
				}
			}
		}
	}

	public void UpdateAroundMineCount()
	{
		lock (m_lock)
		{
			if (m_boguAdventureCell != null)
			{
				for (int i = 0; i < m_boguAdventureCell.Length; i++)
				{
					m_boguAdventureCell[i].MineCount = GetTotalMineAround(i + 1).Length;
				}
			}
		}
	}

	public void CreateBoguAdventureDataInfo()
	{
		lock (m_lock)
		{
			m_boguAdventure = new BoguAdventureDataInfo();
			m_boguAdventure.UserID = Player.PlayerCharacter.ID;
			m_boguAdventure.currentIndex = 0;
			m_boguAdventure.hp = 2;
			m_boguAdventure.isAcquireAward1 = 0;
			m_boguAdventure.isAcquireAward2 = 0;
			m_boguAdventure.isAcquireAward3 = 0;
			m_boguAdventure.openCount = 0;
			m_boguAdventure.isFreeReset = true;
			m_boguAdventure.resetCount = GameProperties.resetCount;
			m_boguAdventure.cellInfo = "";
			m_boguAdventure.awardCount = "";
			m_boguAdventure.lastEnterGame = DateTime.Now;
		}
		SaveBoguAdventureDatabase();
	}

	public void ResetBoguAdventureEveryDay()
	{
		lock (m_lock)
		{
			if (m_boguAdventure != null)
			{
				m_boguAdventure.isAcquireAward1 = 0;
				m_boguAdventure.isAcquireAward2 = 0;
				m_boguAdventure.isAcquireAward3 = 0;
				m_boguAdventure.isFreeReset = true;
				m_boguAdventure.resetCount = GameProperties.resetCount;
				m_boguAdventure.lastEnterGame = DateTime.Now;
			}
		}
		SaveBoguAdventureDatabase();
	}

	public void ResetAllBoguAdventure()
	{
		lock (m_lock)
		{
			if (m_boguAdventure != null)
			{
				m_boguAdventure.currentIndex = 0;
				m_boguAdventure.hp = 2;
				m_boguAdventure.openCount = 0;
				m_boguAdventure.cellInfo = "";
				m_boguAdventure.awardCount = "";
			}
		}
		m_boguAdventureCell = CreateRandomBoguMap();
		m_awardCount = new int[MAP_WIDTH * MAP_HEIGHT];
		UpdateAroundMineCount();
		SerializeBoguAdventure();
		SaveBoguAdventureDatabase();
	}

	public void SendBoguAdventureEnter()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(90);
		gSPacketIn.WriteInt(BoguAdventure.currentIndex);
		gSPacketIn.WriteInt(BoguAdventure.hp);
		gSPacketIn.WriteInt(BoguAdventure.isAcquireAward1);
		gSPacketIn.WriteInt(BoguAdventure.isAcquireAward2);
		gSPacketIn.WriteInt(BoguAdventure.isAcquireAward3);
		gSPacketIn.WriteInt(BoguAdventure.openCount);
		gSPacketIn.WriteInt(findMinePrice);
		gSPacketIn.WriteInt(revivePrice);
		gSPacketIn.WriteInt(resetPrice);
		gSPacketIn.WriteBoolean(BoguAdventure.isFreeReset);
		gSPacketIn.WriteInt(BoguAdventure.resetCount);
		gSPacketIn.WriteInt(BoguAdventureCell.Length);
		BoguCeilInfo[] boguAdventureCell = BoguAdventureCell;
		foreach (BoguCeilInfo boguCeilInfo in boguAdventureCell)
		{
			gSPacketIn.WriteInt(boguCeilInfo.Index);
			gSPacketIn.WriteInt(boguCeilInfo.State);
			gSPacketIn.WriteInt(boguCeilInfo.Result);
			gSPacketIn.WriteInt(boguCeilInfo.MineCount);
		}
		int num = 3;
		for (int j = 0; j < num; j++)
		{
			gSPacketIn.WriteInt(CountOpenCanTakeBoxGoguAdventure(j));
			List<BoguAdventureRewardInfo> list = ActiveSystermAwardMgr.FindBoguAdventureReward(num - j);
			gSPacketIn.WriteInt(list.Count);
			foreach (BoguAdventureRewardInfo item in list)
			{
				gSPacketIn.WriteInt(item.TemplateID);
				gSPacketIn.WriteInt(item.Count);
			}
		}
		Player.SendTCP(gSPacketIn);
	}

	public void SendUpdateFreeCount()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(281);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(Info.updateFreeCounts);
		gSPacketIn.WriteInt(Info.updateWorshipedCounts);
		gSPacketIn.WriteInt(Info.update200State);
		gSPacketIn.WriteInt(int.Parse(GameProperties.WorshipMoonPriceInfo.Split('|')[1]));
		Player.SendTCP(gSPacketIn);
	}

	public void SendHalloweenEnter()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(116);
		gSPacketIn.WriteInt(Player.GetHalloweenCardCount());
		gSPacketIn.WriteInt(ActiveSystemMgr.FindHalloweenRank(Player.PlayerCharacter.ID));
		gSPacketIn.WriteDateTime(DateTime.Now);
		Player.SendTCP(gSPacketIn);
	}

	public bool UpdateAwardCount(int index)
	{
		if (index < 0)
		{
			return false;
		}
		if (index > MAP_WIDTH * MAP_HEIGHT)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_awardCount != null && m_awardCount[index - 1] == 0)
			{
				m_awardCount[index - 1] = index;
				return true;
			}
		}
		if (IsAcquireAward(0) == 0)
		{
			SendBoguAdventureEnter();
		}
		if (IsAcquireAward(1) == 0)
		{
			SendBoguAdventureEnter();
		}
		if (IsAcquireAward(2) == 0)
		{
			SendBoguAdventureEnter();
		}
		return false;
	}

	public void UpdateCeilBoguMap(BoguCeilInfo ceil)
	{
		lock (m_lock)
		{
			if (m_boguAdventureCell != null)
			{
				m_boguAdventureCell[ceil.Index - 1] = ceil;
			}
		}
		SaveBoguAdventureDatabase();
	}

	public BoguCeilInfo[] GetTotalMineAroundNotOpen(int index)
	{
		List<BoguCeilInfo> list = new List<BoguCeilInfo>();
		int[] countAroundIndex = GetCountAroundIndex(index);
		foreach (int index2 in countAroundIndex)
		{
			BoguCeilInfo boguCeilInfo = FindCeilBoguMap(index2);
			if (boguCeilInfo != null && boguCeilInfo.Result == MINE && boguCeilInfo.State == NOT_OPEN)
			{
				list.Add(boguCeilInfo);
			}
		}
		return list.ToArray();
	}

	private BoguCeilInfo[] CreateRandomBoguMap()
	{
		BoguCeilInfo boguCeilInfo = new BoguCeilInfo();
		BoguCeilInfo[] array = new BoguCeilInfo[MAP_WIDTH * MAP_HEIGHT];
		int[] source = RandomMine();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new BoguCeilInfo
			{
				Index = i + 1,
				State = NOT_OPEN,
				Result = (source.Contains(i + 1) ? MINE : SPACE),
				MineCount = 0
			};
		}
		return array;
	}

	public BoguCeilInfo[] GetTotalMineAround(int index)
	{
		List<BoguCeilInfo> list = new List<BoguCeilInfo>();
		int[] countAroundIndex = GetCountAroundIndex(index);
		foreach (int index2 in countAroundIndex)
		{
			BoguCeilInfo boguCeilInfo = FindCeilBoguMap(index2);
			if (boguCeilInfo != null && boguCeilInfo.Result == MINE)
			{
				list.Add(boguCeilInfo);
			}
		}
		return list.ToArray();
	}

	public BoguCeilInfo FindCeilBoguMap(int index)
	{
		if (m_boguAdventureCell == null)
		{
			return null;
		}
		BoguCeilInfo[] boguAdventureCell = m_boguAdventureCell;
		foreach (BoguCeilInfo boguCeilInfo in boguAdventureCell)
		{
			if (boguCeilInfo.Index == index)
			{
				return boguCeilInfo;
			}
		}
		return null;
	}

	public int[] GetCountAroundIndex(int index)
	{
		List<int> list = new List<int>();
		int[] source = new int[7] { 1, 11, 21, 31, 41, 51, 61 };
		int[] source2 = new int[7] { 10, 20, 30, 40, 50, 60, 70 };
		int num = MAP_WIDTH * MAP_HEIGHT;
		if (index > 0 && index <= num)
		{
			int num2 = index - 1;
			if (num2 >= 1 && num2 <= num)
			{
				list.Add(num2);
			}
			int num3 = index - 9;
			if (num3 >= 1 && num3 <= num)
			{
				list.Add(num3);
			}
			int num4 = index - 10;
			if (num4 >= 1 && num4 <= num)
			{
				list.Add(num4);
			}
			int num5 = index - 11;
			if (num5 >= 1 && num5 <= num)
			{
				list.Add(num5);
			}
			int num6 = index + 1;
			if (num6 >= 1 && num6 <= num)
			{
				list.Add(num6);
			}
			int num7 = index + 9;
			if (num7 >= 1 && num7 <= num)
			{
				list.Add(num7);
			}
			int num8 = index + 10;
			if (num8 >= 1 && num8 <= num)
			{
				list.Add(num8);
			}
			int num9 = index + 11;
			if (num9 >= 1 && num9 <= num)
			{
				list.Add(num9);
			}
			if (source.Contains(index))
			{
				list.Remove(num2);
				list.Remove(num7);
				list.Remove(num5);
			}
			if (source2.Contains(index))
			{
				list.Remove(num6);
				list.Remove(num3);
				list.Remove(num9);
			}
		}
		return list.ToArray();
	}

	private int[] RandomMine()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		for (int i = 1; i <= MAP_WIDTH * MAP_HEIGHT; i++)
		{
			list2.Add(i);
		}
		for (int num = 25; num > 0; num--)
		{
			int index = rand.Next(0, list2.Count - 1);
			list.Add(list2[index]);
			list2.RemoveAt(index);
		}
		return list.ToArray();
	}

	public void TreasurepuzzleGetAward(int current)
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_treasurepuzzle.Length; i++)
			{
				if (current == i)
				{
					m_treasurepuzzle[i].IsGetAward = true;
				}
				m_treasurepuzzle[i].hole1Have = 0;
				m_treasurepuzzle[i].hole2Have = 0;
				m_treasurepuzzle[i].hole3Have = 0;
				m_treasurepuzzle[i].hole4Have = 0;
				m_treasurepuzzle[i].hole5Have = 0;
				m_treasurepuzzle[i].hole6Have = 0;
			}
		}
	}

	public int AddHole(int hole, int count)
	{
		int num = 0;
		lock (m_lock)
		{
			for (int i = 0; i < count; i++)
			{
				switch (hole)
				{
				case 1:
					if (m_treasurepuzzle[0].hole1Have < m_treasurepuzzle[0].hole1Need)
					{
						m_treasurepuzzle[0].hole1Have++;
						num++;
					}
					else if (m_treasurepuzzle[1].hole1Have < m_treasurepuzzle[1].hole1Need)
					{
						m_treasurepuzzle[1].hole1Have++;
						num++;
					}
					else if (m_treasurepuzzle[2].hole1Have < m_treasurepuzzle[2].hole1Need)
					{
						m_treasurepuzzle[2].hole1Have++;
						num++;
					}
					break;
				case 2:
					if (m_treasurepuzzle[0].hole2Have < m_treasurepuzzle[0].hole2Need)
					{
						m_treasurepuzzle[0].hole2Have++;
						num++;
					}
					else if (m_treasurepuzzle[1].hole2Have < m_treasurepuzzle[1].hole2Need)
					{
						m_treasurepuzzle[1].hole2Have++;
						num++;
					}
					else if (m_treasurepuzzle[2].hole2Have < m_treasurepuzzle[2].hole2Need)
					{
						m_treasurepuzzle[2].hole2Have++;
						num++;
					}
					break;
				case 3:
					if (m_treasurepuzzle[0].hole3Have < m_treasurepuzzle[0].hole3Need)
					{
						m_treasurepuzzle[0].hole3Have++;
						num++;
					}
					else if (m_treasurepuzzle[1].hole3Have < m_treasurepuzzle[1].hole3Need)
					{
						m_treasurepuzzle[1].hole3Have++;
						num++;
					}
					else if (m_treasurepuzzle[2].hole3Have < m_treasurepuzzle[2].hole3Need)
					{
						m_treasurepuzzle[2].hole3Have++;
						num++;
					}
					break;
				case 4:
					if (m_treasurepuzzle[0].hole4Have < m_treasurepuzzle[0].hole4Need)
					{
						m_treasurepuzzle[0].hole4Have++;
						num++;
					}
					else if (m_treasurepuzzle[1].hole4Have < m_treasurepuzzle[1].hole4Need)
					{
						m_treasurepuzzle[1].hole4Have++;
						num++;
					}
					else if (m_treasurepuzzle[2].hole4Have < m_treasurepuzzle[2].hole4Need)
					{
						m_treasurepuzzle[2].hole4Have++;
						num++;
					}
					break;
				case 5:
					if (m_treasurepuzzle[0].hole5Have < m_treasurepuzzle[0].hole5Need)
					{
						m_treasurepuzzle[0].hole5Have++;
						num++;
					}
					else if (m_treasurepuzzle[1].hole5Have < m_treasurepuzzle[1].hole5Need)
					{
						m_treasurepuzzle[1].hole5Have++;
						num++;
					}
					else if (m_treasurepuzzle[2].hole5Have < m_treasurepuzzle[2].hole5Need)
					{
						m_treasurepuzzle[2].hole5Have++;
						num++;
					}
					break;
				case 6:
					if (m_treasurepuzzle[0].hole6Have < m_treasurepuzzle[0].hole6Need)
					{
						m_treasurepuzzle[0].hole6Have++;
						num++;
					}
					else if (m_treasurepuzzle[1].hole6Have < m_treasurepuzzle[1].hole6Need)
					{
						m_treasurepuzzle[1].hole6Have++;
						num++;
					}
					else if (m_treasurepuzzle[2].hole6Have < m_treasurepuzzle[2].hole6Need)
					{
						m_treasurepuzzle[2].hole6Have++;
						num++;
					}
					break;
				}
			}
		}
		return num++;
	}

	public void CreateTreasurepuzzleDataInfo()
	{
		lock (m_lock)
		{
			m_treasurepuzzle = new TreasurePuzzleDataInfo[3];
			for (int i = 1; i <= m_treasurepuzzle.Length; i++)
			{
				int num = holeNeed * i;
				TreasurePuzzleDataInfo treasurePuzzleDataInfo = new TreasurePuzzleDataInfo();
				treasurePuzzleDataInfo.UserID = Player.PlayerCharacter.ID;
				treasurePuzzleDataInfo.PuzzleID = i;
				treasurePuzzleDataInfo.hole1Need = num;
				treasurePuzzleDataInfo.hole1Have = 0;
				treasurePuzzleDataInfo.hole2Need = num;
				treasurePuzzleDataInfo.hole2Have = 0;
				treasurePuzzleDataInfo.hole3Need = num;
				treasurePuzzleDataInfo.hole3Have = 0;
				treasurePuzzleDataInfo.hole4Need = num;
				treasurePuzzleDataInfo.hole4Have = 0;
				treasurePuzzleDataInfo.hole5Need = num;
				treasurePuzzleDataInfo.hole5Have = 0;
				treasurePuzzleDataInfo.hole6Need = num;
				treasurePuzzleDataInfo.hole6Have = 0;
				m_treasurepuzzle[i - 1] = treasurePuzzleDataInfo;
			}
		}
		SaveTreasurepuzzleDatabase();
	}

	public virtual bool LoadTreasurePuzzleFromDatabase()
	{
		if (m_saveToDb)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			try
			{
				TreasurePuzzleDataInfo[] allTreasurePuzzleDataByID = playerBussiness.GetAllTreasurePuzzleDataByID(Player.PlayerCharacter.ID);
				if (allTreasurePuzzleDataByID.Length == 0)
				{
					CreateTreasurepuzzleDataInfo();
				}
				else
				{
					lock (m_lock)
					{
						m_treasurepuzzle = allTreasurePuzzleDataByID;
						for (int i = 1; i <= m_treasurepuzzle.Length; i++)
						{
							int num = holeNeed * i;
							m_treasurepuzzle[i - 1].hole1Need = num;
							m_treasurepuzzle[i - 1].hole2Need = num;
							m_treasurepuzzle[i - 1].hole3Need = num;
							m_treasurepuzzle[i - 1].hole4Need = num;
							m_treasurepuzzle[i - 1].hole5Need = num;
							m_treasurepuzzle[i - 1].hole6Need = num;
						}
					}
				}
			}
			catch
			{
			}
		}
		return Treasurepuzzle != null && Treasurepuzzle.Length >= 3;
	}

	public virtual void SaveTreasurepuzzleDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			for (int i = 0; i < Treasurepuzzle.Length; i++)
			{
				if (m_treasurepuzzle[i] != null && m_treasurepuzzle[i].IsDirty)
				{
					if (m_treasurepuzzle[i].ID > 0)
					{
						playerBussiness.UpdateTreasurePuzzleData(m_treasurepuzzle[i]);
					}
					else
					{
						playerBussiness.AddTreasurePuzzleData(m_treasurepuzzle[i]);
					}
				}
			}
		}
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		try
		{
			if (IsChristmasOpen())
			{
				m_christmas = playerBussiness.GetSingleUserChristmas(Player.PlayerCharacter.ID);
				if (m_christmas == null)
				{
					CreateChristmasInfo(Player.PlayerCharacter.ID);
				}
			}
			m_activeInfo = playerBussiness.GetSingleActiveSystem(Player.PlayerCharacter.ID);
			if (m_activeInfo == null)
			{
				CreateActiveSystemInfo();
			}
			else
			{
				lock (m_lock)
				{
					m_activeInfo.NickName = Player.PlayerCharacter.NickName;
					m_activeInfo.ZoneName = Player.ZoneName;
					m_activeInfo.ZoneID = Player.ZoneId;
					if (string.IsNullOrEmpty(m_activeInfo.cardListCard))
					{
						m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
						m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
						m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);
					}
				}
			}
			SynSignBuff();
			SynFightPowerSign();
		}
		finally
		{
			SetupCryptBossOpenDay();
			m_listCards = JsonConvert.DeserializeObject<List<GodCardUser>>(Info.cardListCard);
			m_listExchanges = JsonConvert.DeserializeObject<List<GodCardGroupUser>>(Info.cardListExchange);
			m_listAwards = JsonConvert.DeserializeObject<List<int>>(Info.cardListAward);
		}
	}

	public virtual void SaveActiveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_activeInfo != null && m_activeInfo.IsDirty)
			{
				if (m_activeInfo.ID > 0)
				{
					playerBussiness.UpdateActiveSystem(m_activeInfo);
				}
				else
				{
					playerBussiness.AddActiveSystem(m_activeInfo);
				}
			}
		}
	}

	public virtual void SaveToDatabase()
	{
		if (m_saveToDb)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			lock (m_lock)
			{
				if (m_cryptBoss != null)
				{
					m_activeInfo.CryptBoss = JsonConvert.SerializeObject(m_cryptBoss);
				}
				if (m_activeInfo != null && m_activeInfo.IsDirty)
				{
					m_activeInfo.cardListCard = JsonConvert.SerializeObject(ListCards);
					m_activeInfo.cardListExchange = JsonConvert.SerializeObject(ListExchanges);
					m_activeInfo.cardListAward = JsonConvert.SerializeObject(ListAwards);
					if (m_activeInfo.ID > 0)
					{
						playerBussiness.UpdateActiveSystem(m_activeInfo);
					}
					else
					{
						playerBussiness.AddActiveSystem(m_activeInfo);
					}
				}
				if (m_pyramid != null && m_pyramid.IsDirty)
				{
					if (m_pyramid.ID > 0)
					{
						playerBussiness.UpdatePyramid(m_pyramid);
					}
					else
					{
						playerBussiness.AddPyramid(m_pyramid);
					}
				}
				if (m_christmas != null && m_christmas.IsDirty)
				{
					if (m_christmas.ID > 0)
					{
						playerBussiness.UpdateUserChristmas(m_christmas);
					}
					else
					{
						playerBussiness.AddUserChristmas(m_christmas);
					}
				}
				if (m_ChickenBoxRewards != null)
				{
					NewChickenBoxItemInfo[] chickenBoxRewards = m_ChickenBoxRewards;
					foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
					{
						if (newChickenBoxItemInfo != null && newChickenBoxItemInfo.IsDirty)
						{
							if (newChickenBoxItemInfo.ID > 0)
							{
								playerBussiness.UpdateNewChickenBox(newChickenBoxItemInfo);
							}
							else
							{
								playerBussiness.AddNewChickenBox(newChickenBoxItemInfo);
							}
						}
					}
				}
				if (m_mysteryShop != null)
				{
					GypsyItemDataInfo[] mysteryShop = m_mysteryShop;
					foreach (GypsyItemDataInfo gypsyItemDataInfo in mysteryShop)
					{
						if (gypsyItemDataInfo != null && gypsyItemDataInfo.IsDirty)
						{
							if (gypsyItemDataInfo.ID > 0)
							{
								playerBussiness.UpdateGypsyItemData(gypsyItemDataInfo);
							}
							else
							{
								playerBussiness.AddGypsyItemData(gypsyItemDataInfo);
							}
						}
					}
				}
			}
			if (m_removeChickenBoxRewards.Count > 0)
			{
				foreach (NewChickenBoxItemInfo removeChickenBoxReward in m_removeChickenBoxRewards)
				{
					playerBussiness.UpdateNewChickenBox(removeChickenBoxReward);
				}
			}
		}
		LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(m_player.PlayerCharacter.ID);
		GameServer.Instance.LoginServer.SendLightriddleInfo(lanternriddles);
	}

	public GypsyItemDataInfo GetMysteryShopByID(int ID)
	{
		GypsyItemDataInfo[] mysteryShop = MysteryShop;
		foreach (GypsyItemDataInfo gypsyItemDataInfo in mysteryShop)
		{
			if (gypsyItemDataInfo.GypsyID == ID)
			{
				return gypsyItemDataInfo;
			}
		}
		return null;
	}

	public bool UpdateMysteryShopByID(int ID)
	{
		lock (m_lock)
		{
			for (int i = 0; i < MysteryShop.Length; i++)
			{
				if (m_mysteryShop[i].GypsyID == ID)
				{
					m_mysteryShop[i].CanBuy = 0;
				}
			}
		}
		return true;
	}

	public void RefreshMysteryShopByHour()
	{
		DateTime now = DateTime.Now;
		if (GameProperties.MysteryShopFreshTime == now.Hour && Info.LastRefresh.Date < now.Date)
		{
			Info.LastRefresh = now;
			RefreshMysteryShop();
		}
	}

	public void RefreshMysteryShop()
	{
		List<MysteryShopInfo> mysteryShop = GypsyShopMgr.GetMysteryShop();
		bool flag = false;
		lock (m_lock)
		{
			if (m_mysteryShop == null)
			{
				m_mysteryShop = new GypsyItemDataInfo[8];
				flag = true;
			}
			int num = 0;
			foreach (MysteryShopInfo item in mysteryShop)
			{
				if (num < MysteryShop.Length)
				{
					if (flag)
					{
						GypsyItemDataInfo gypsyItemDataInfo = new GypsyItemDataInfo();
						gypsyItemDataInfo.UserID = m_player.PlayerId;
						gypsyItemDataInfo.GypsyID = item.ID;
						gypsyItemDataInfo.InfoID = item.InfoID;
						gypsyItemDataInfo.Unit = item.Unit;
						gypsyItemDataInfo.Num = item.Num;
						gypsyItemDataInfo.Price = item.Price;
						gypsyItemDataInfo.CanBuy = item.CanBuy;
						gypsyItemDataInfo.Quality = item.Quality;
						m_mysteryShop[num] = gypsyItemDataInfo;
					}
					else
					{
						m_mysteryShop[num].GypsyID = item.ID;
						m_mysteryShop[num].InfoID = item.InfoID;
						m_mysteryShop[num].Unit = item.Unit;
						m_mysteryShop[num].Num = item.Num;
						m_mysteryShop[num].Price = item.Price;
						m_mysteryShop[num].CanBuy = item.CanBuy;
						m_mysteryShop[num].Quality = item.Quality;
					}
				}
				num++;
			}
		}
	}

	public void ResetMysteryShop()
	{
		if (m_activeInfo == null)
		{
			return;
		}
		lock (m_lock)
		{
			m_activeInfo.CurRefreshedTimes = 0;
		}
	}

	public GSPacketIn SendGypsyShopPlayerInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(278, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(Info.CurRefreshedTimes);
		gSPacketIn.WriteInt(MysteryShop.Length);
		GypsyItemDataInfo[] mysteryShop = MysteryShop;
		foreach (GypsyItemDataInfo gypsyItemDataInfo in mysteryShop)
		{
			gSPacketIn.WriteInt(gypsyItemDataInfo.GypsyID);
			gSPacketIn.WriteInt(gypsyItemDataInfo.Unit);
			gSPacketIn.WriteInt(gypsyItemDataInfo.Price);
			gSPacketIn.WriteInt(gypsyItemDataInfo.Num);
			if (GameProperties.VERSION >= 8200)
			{
				gSPacketIn.WriteInt(gypsyItemDataInfo.type);
			}
			gSPacketIn.WriteInt(gypsyItemDataInfo.InfoID);
			gSPacketIn.WriteInt(gypsyItemDataInfo.CanBuy);
			gSPacketIn.WriteInt(gypsyItemDataInfo.Quality);
		}
		Player.Out.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void SendEvent()
	{
		if (IsChickenBoxOpen())
		{
			m_player.Out.SendChickenBoxOpen(m_player.PlayerId, flushPrice, openCardPrice, eagleEyePrice);
		}
		SendCryptBossInitAllData();
		SendHalloweenOpenClose();
		SendDDPlayOpenClose();
		SendMagpieBridgeOpenClose();
		SendCatchInsectOpenClose();
		if (GypsyShopMgr.OpenOrClose)
		{
			SendGypsyShopOpenClose(open: true);
			RefreshMysteryShopByHour();
		}
		SendHorseRaceOpenClose();
		SendPolarRegionOpenClose();
		SendGodCardRaiseOpenClose();
		SendSanXiaoOpenClose();
		SendMemoryGameOpenClose();
		SendDDTQiYuanOpenClose();
		SendCityBattleOpenClose();
		SendPrayIndianaOpenClose();
		SendHappyRechargeOpenClose();
	}

	public void SendGodCardRaiseOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(32);
		gSPacketIn.WriteBoolean(GlobalConstants.IsOpenActive(eActive.GodCardRaise));
		gSPacketIn.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
		Player.SendTCP(gSPacketIn);
	}

	public void SendSanXiaoOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteBoolean(GlobalConstants.IsOpenActive(eActive.SanXiao));
		gSPacketIn.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
		gSPacketIn.WriteInt(0);
		Player.SendTCP(gSPacketIn);
	}

	public void SendMemoryGameOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteBoolean(GlobalConstants.IsOpenActive(eActive.MemoryGame));
		gSPacketIn.WriteDateTime(Convert.ToDateTime(GameProperties.MemoryGameEndTime));
		Player.SendTCP(gSPacketIn);
	}

	public void SendDDTQiYuanOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(40);
		gSPacketIn.WriteBoolean(GlobalConstants.IsOpenActive(eActive.QiYuan));
		gSPacketIn.WriteDateTime(Convert.ToDateTime(GameProperties.QiYuanEndTime));
		Player.SendTCP(gSPacketIn);
	}

	public GSPacketIn SendSurvivalModeOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(320);
		gSPacketIn.WriteBoolean(GlobalConstants.IsOpenActive(eActive.SurvivalMode));
		Player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendPolarRegionOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(305);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteBoolean(val: true);
		Player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendRenshenOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(298);
		gSPacketIn.WriteByte(13);
		gSPacketIn.WriteBoolean(val: true);
		Player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendPetIslandOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(186);
		gSPacketIn.WriteBoolean(val: true);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendHappyRechargeOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(178);
		gSPacketIn.WriteBoolean(val: true);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendPrayIndianaOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(170);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteDateTime(DateTime.Now);
		gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendTreasureLostOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(144);
		gSPacketIn.WriteBoolean(val: true);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendHorseRaceOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(160);
		gSPacketIn.WriteBoolean(GlobalConstants.IsOpenActive(eActive.HorseRace));
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendGypsyShopOpenClose(bool open)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(278, m_player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteBoolean(open);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendCatchInsectOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(128);
		gSPacketIn.WriteBoolean(IsSummerOpen());
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendMagpieBridgeOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(276, m_player.PlayerId);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteBoolean(IsMagpieBridgeOpen());
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendDDPlayOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(74);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.DDPlayBeginDate));
		gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.DDPlayEndDate));
		gSPacketIn.WriteInt(GameProperties.DDPlayMoney);
		gSPacketIn.WriteInt(GameProperties.ExchangeFold);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendCloudBuyLotteryOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(117);
		gSPacketIn.WriteBoolean(val: true);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendHalloweenOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(115);
		gSPacketIn.WriteBoolean(IsHalloweenOpen());
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendBoguAdventureOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(89);
		gSPacketIn.WriteBoolean(val: true);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendTreasurePuzzleOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(102);
		gSPacketIn.WriteBoolean(val: true);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendCryptBossInitAllData()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(275, m_player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(CryptBoss.Length);
		CryptBossItemInfo[] cryptBoss = CryptBoss;
		foreach (CryptBossItemInfo cryptBossItemInfo in cryptBoss)
		{
			gSPacketIn.WriteInt(cryptBossItemInfo.id);
			gSPacketIn.WriteInt(cryptBossItemInfo.star);
			gSPacketIn.WriteInt(cryptBossItemInfo.state);
		}
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLuckStarClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(26);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendCloudBuyLotteryUpdateInfos(CloudBuyLotteryInfo info, bool isGame)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(122);
		gSPacketIn.WriteInt(info.templateId);
		gSPacketIn.WriteInt(info.templatedIdCount);
		gSPacketIn.WriteInt(info.validDate);
		gSPacketIn.WriteString(info.property);
		gSPacketIn.WriteInt(info.maxNum);
		gSPacketIn.WriteInt(info.currentNum);
		gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.CloudBuyLotteryEndDate));
		gSPacketIn.WriteInt(m_activeInfo.luckCount);
		gSPacketIn.WriteInt(m_activeInfo.remainTimes);
		gSPacketIn.WriteBoolean(isGame);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendCloudBuyLotteryEnter(CloudBuyLotteryInfo info, bool isGame)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(118);
		gSPacketIn.WriteInt(info.templateId);
		gSPacketIn.WriteInt(info.templatedIdCount);
		gSPacketIn.WriteInt(info.validDate);
		gSPacketIn.WriteString(info.property);
		string[] array = info.buyItemsArr.Split(',');
		gSPacketIn.WriteInt(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('|');
			gSPacketIn.WriteInt(int.Parse(array2[0]));
			gSPacketIn.WriteInt(int.Parse(array2[1]));
		}
		gSPacketIn.WriteInt(info.buyMoney);
		gSPacketIn.WriteInt(info.maxNum);
		gSPacketIn.WriteInt(info.currentNum);
		gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.CloudBuyLotteryEndDate));
		gSPacketIn.WriteInt(m_activeInfo.luckCount);
		gSPacketIn.WriteInt(m_activeInfo.remainTimes);
		gSPacketIn.WriteBoolean(isGame);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendCityBattleOpenClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(368, m_player.PlayerId);
		gSPacketIn.WriteByte(0);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
		return gSPacketIn;
	}

	public void ResetLuckCount()
	{
		lock (m_activeInfo)
		{
			m_activeInfo.luckCount = 0;
		}
	}

	public CryptBossItemInfo GetCryptBossData(int id)
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_cryptBoss.Length; i++)
			{
				if (m_cryptBoss[i].id == id)
				{
					return m_cryptBoss[i];
				}
			}
		}
		return null;
	}

	private void SetupCryptBossOpenDay()
	{
		List<CryptBossItemInfo> list = new List<CryptBossItemInfo>();
		if (string.IsNullOrEmpty(m_activeInfo.CryptBoss))
		{
			for (int i = 0; i < cryptBossOpenDay.Length; i++)
			{
				string[] array = cryptBossOpenDay[i].Split(',');
				CryptBossItemInfo cryptBossItemInfo = new CryptBossItemInfo();
				cryptBossItemInfo.id = int.Parse(array[0]);
				cryptBossItemInfo.star = 0;
				cryptBossItemInfo.state = GetState(array);
				list.Add(cryptBossItemInfo);
			}
		}
		else
		{
			list = JsonConvert.DeserializeObject<List<CryptBossItemInfo>>(m_activeInfo.CryptBoss);
		}
		lock (m_lock)
		{
			m_cryptBoss = list.ToArray();
		}
	}

	public void ResetCryptBossData()
	{
		if (m_cryptBoss == null)
		{
			return;
		}
		lock (m_lock)
		{
			for (int i = 0; i < CryptBoss.Length; i++)
			{
				if (CryptBoss[i] != null)
				{
					int id = m_cryptBoss[i].id;
					string[] openDay = GetOpenDay(id);
					m_cryptBoss[i].state = GetState(openDay);
				}
			}
		}
	}

	private string[] GetOpenDay(int id)
	{
		string[] array = cryptBossOpenDay;
		foreach (string text in array)
		{
			string[] array2 = text.Split(',');
			if (id.ToString() == array2[0])
			{
				return array2;
			}
		}
		return null;
	}

	private int GetState(string[] days)
	{
		if (days == null)
		{
			return 2;
		}
		for (int i = 1; i < days.Length; i++)
		{
			string s = days[i];
			if (DateTime.Now.DayOfWeek == (DayOfWeek)int.Parse(s))
			{
				return 1;
			}
		}
		return 2;
	}

	public void UpdateStar(int id)
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_cryptBoss.Length; i++)
			{
				CryptBossItemInfo cryptBossItemInfo = m_cryptBoss[i];
				if (cryptBossItemInfo.id == id)
				{
					if (cryptBossItemInfo.star < 5)
					{
						cryptBossItemInfo.star++;
					}
					cryptBossItemInfo.state = 2;
					SendUpdateSingleData(cryptBossItemInfo);
					m_cryptBoss[i] = cryptBossItemInfo;
					SendCryptBossAward(id, cryptBossItemInfo.star, Player.ZoneId);
				}
			}
		}
	}

	public GSPacketIn SendUpdateSingleData(CryptBossItemInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(275, m_player.PlayerId);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(info.id);
		gSPacketIn.WriteInt(info.star);
		gSPacketIn.WriteInt(info.state);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void SendCryptBossAward(int id, int star, int zoneId)
	{
		int templateId = 0;
		switch (id)
		{
		case 1:
			if (star > 0 && star <= m_cryptBossAward1.Length)
			{
				templateId = m_cryptBossAward1[star - 1];
			}
			break;
		case 2:
			if (star > 0 && star <= m_cryptBossAward2.Length)
			{
				templateId = m_cryptBossAward2[star - 1];
			}
			break;
		case 3:
			if (star > 0 && star <= m_cryptBossAward3.Length)
			{
				templateId = m_cryptBossAward3[star - 1];
			}
			break;
		case 4:
			if (star > 0 && star <= m_cryptBossAward4.Length)
			{
				templateId = m_cryptBossAward4[star - 1];
			}
			break;
		case 5:
			if (star > 0 && star <= m_cryptBossAward5.Length)
			{
				templateId = m_cryptBossAward5[star - 1];
			}
			break;
		case 6:
			if (star > 0 && star <= m_cryptBossAward6.Length)
			{
				templateId = m_cryptBossAward6[star - 1];
			}
			break;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
			itemInfo.Count = 1;
			itemInfo.ValidDate = 0;
			itemInfo.IsBinds = true;
			list.Add(itemInfo);
			string translation = LanguageMgr.GetTranslation("GamePlayer.Msg23", id, star);
			WorldEventMgr.SendItemsToMails(list, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, zoneId, translation);
			QXDropId = 0;
		}
	}

	private void SetupPyramidConfig()
	{
		lock (m_lock)
		{
			m_pyramidConfig = new PyramidConfigInfo();
			m_pyramidConfig.isOpen = IsPyramidOpen();
			m_pyramidConfig.isScoreExchange = !IsPyramidOpen();
			m_pyramidConfig.beginTime = Convert.ToDateTime(GameProperties.PyramidBeginTime);
			m_pyramidConfig.endTime = Convert.ToDateTime(GameProperties.PyramidEndTime);
			m_pyramidConfig.freeCount = 3;
			m_pyramidConfig.revivePrice = GameProperties.ConvertStringArrayToIntArray("PyramidRevivePrice");
			m_pyramidConfig.turnCardPrice = GameProperties.PyramydTurnCardPrice;
		}
	}

	public bool IsHalloweenOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.HalloweenEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsChristmasOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.ChristmasEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsChickenBoxOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.NewChickenEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsLuckStarActivityOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.LuckStarActivityEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsPyramidOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.PyramidEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsYearMonsterOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.YearMonsterEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsWorshipTheMoonOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.WorshipMoonEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsMagpieBridgeOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.TanabataActiveEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsSummerOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.SummerAcitveEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public NewChickenBoxItemInfo GetAward(int pos)
	{
		NewChickenBoxItemInfo[] chickenBoxRewards = m_ChickenBoxRewards;
		foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
		{
			if (newChickenBoxItemInfo.Position == pos && !newChickenBoxItemInfo.IsSelected)
			{
				return newChickenBoxItemInfo;
			}
		}
		return null;
	}

	public NewChickenBoxItemInfo ViewAward(int pos)
	{
		NewChickenBoxItemInfo[] chickenBoxRewards = m_ChickenBoxRewards;
		foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
		{
			if (newChickenBoxItemInfo.Position == pos && !newChickenBoxItemInfo.IsSeeded)
			{
				return newChickenBoxItemInfo;
			}
		}
		return null;
	}

	public bool UpdateChickenBoxAward(NewChickenBoxItemInfo box)
	{
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			if (m_ChickenBoxRewards[i].Position == box.Position)
			{
				m_ChickenBoxRewards[i] = box;
				return true;
			}
		}
		return false;
	}

	public void LoadChickenBox()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		m_ChickenBoxRewards = playerBussiness.GetSingleNewChickenBox(Player.PlayerCharacter.ID);
		if (m_ChickenBoxRewards.Length == 0)
		{
			PayFlushView();
		}
	}

	public void EnterChickenBox()
	{
		if (m_ChickenBoxRewards == null)
		{
			LoadChickenBox();
		}
	}

	public void RandomPosition()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			list.Add(m_ChickenBoxRewards[i].Position);
		}
		ThreadSafeRandom.ShufferStatic(m_ChickenBoxRewards);
		for (int j = 0; j < list.Count; j++)
		{
			m_ChickenBoxRewards[j].Position = list[j];
		}
	}

	public NewChickenBoxItemInfo[] CreateChickenBoxAward(int count, eEventType DataId)
	{
		List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
		Dictionary<int, NewChickenBoxItemInfo> dictionary = new Dictionary<int, NewChickenBoxItemInfo>();
		int num = 0;
		int num2 = 0;
		while (list.Count < count)
		{
			List<NewChickenBoxItemInfo> newChickenBoxAward = EventAwardMgr.GetNewChickenBoxAward(DataId);
			if (newChickenBoxAward.Count > 0)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = newChickenBoxAward[0];
				if (!dictionary.Keys.Contains(newChickenBoxItemInfo.TemplateID))
				{
					dictionary.Add(newChickenBoxItemInfo.TemplateID, newChickenBoxItemInfo);
					newChickenBoxItemInfo.Position = num;
					list.Add(newChickenBoxItemInfo);
					num++;
				}
			}
			num2++;
		}
		return list.ToArray();
	}

	public void PayFlushView()
	{
		m_activeInfo.lastFlushTime = DateTime.Now;
		m_activeInfo.isShowAll = true;
		m_activeInfo.canOpenCounts = 5;
		m_activeInfo.canEagleEyeCounts = 5;
		RemoveChickenBoxRewards();
		m_ChickenBoxRewards = CreateChickenBoxAward(ChikenBoxCount, eEventType.CHICKEN_BOX);
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			m_ChickenBoxRewards[i].UserID = Player.PlayerCharacter.ID;
		}
	}

	public void RemoveChickenBoxRewards()
	{
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			NewChickenBoxItemInfo newChickenBoxItemInfo = m_ChickenBoxRewards[i];
			if (newChickenBoxItemInfo != null && newChickenBoxItemInfo.ID > 0)
			{
				newChickenBoxItemInfo.Position = -1;
				m_removeChickenBoxRewards.Add(newChickenBoxItemInfo);
			}
		}
	}

	public bool IsFreeFlushTime()
	{
		DateTime lastFlushTime = Info.lastFlushTime;
		DateTime dateTime = lastFlushTime.AddMinutes(freeFlushTime);
		TimeSpan timeSpan = DateTime.Now - Info.lastFlushTime;
		double num = (dateTime - lastFlushTime).TotalMinutes - timeSpan.TotalMinutes;
		return num > 0.0;
	}

	public bool LoadPyramid()
	{
		if (m_pyramid == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			m_pyramid = playerBussiness.GetSinglePyramid(Player.PlayerCharacter.ID);
			if (m_pyramid == null)
			{
				CreatePyramidInfo();
			}
		}
		return true;
	}

	public void CreatePyramidInfo()
	{
		lock (m_lock)
		{
			m_pyramid = new PyramidInfo();
			m_pyramid.ID = 0;
			m_pyramid.UserID = Player.PlayerCharacter.ID;
			m_pyramid.currentLayer = 1;
			m_pyramid.maxLayer = 1;
			m_pyramid.totalPoint = 0;
			m_pyramid.turnPoint = 0;
			m_pyramid.pointRatio = 0;
			m_pyramid.currentFreeCount = 0;
			m_pyramid.currentReviveCount = 0;
			m_pyramid.isPyramidStart = false;
			m_pyramid.LayerItems = "";
		}
	}

	public void ResetChristmas()
	{
		lock (m_lock)
		{
			if (m_christmas != null)
			{
				m_christmas.dayPacks = 0;
				m_christmas.AvailTime = 0;
				m_christmas.isEnter = false;
			}
		}
	}

	public void ResetTanabata()
	{
		lock (m_lock)
		{
			m_activeInfo.activityTanabataNum = 0;
		}
	}

	public void ResetDragonBoatDayScore()
	{
		lock (m_lock)
		{
			m_activeInfo.dayScore = 0;
		}
	}

	public void ResetSanXiao()
	{
		lock (m_lock)
		{
			m_activeInfo.SXStepRemain = GameProperties.SanXiaoResetCount;
			m_activeInfo.MemoryGameCount = GameProperties.MemoryGameResetCount;
		}
	}

	public void ResetGodCard()
	{
		lock (m_lock)
		{
			if (DateTime.Now.Date > Convert.ToDateTime(GameProperties.SanXiaoEndTime).Date.AddDays(1.0))
			{
				Random random = new Random();
				m_activeInfo.cardScore = 0;
				m_activeInfo.cardFreeCount = 0;
				m_activeInfo.cardChipCount = 0;
				m_activeInfo.cardListCard = "";
				m_activeInfo.cardListAward = "";
				m_activeInfo.cardListExchange = "";
			}
			else
			{
				m_activeInfo.cardFreeCount = 0;
			}
		}
	}

	public void ResetActive()
	{
		ResetChristmas();
		ResetCryptBossData();
		if (Info != null)
		{
			ResetDragonBoatDayScore();
			ResetTanabata();
			ResetMysteryShop();
			SynSignBuff();
			ResetSanXiao();
			ResetGodCard();
		}
	}

	public void SendQXAward()
	{
		if (QXDropId <= 0)
		{
			return;
		}
		int templateId = 0;
		switch (QXDropId)
		{
		case 70001:
			templateId = 112308;
			break;
		case 70002:
			templateId = 112309;
			break;
		case 70003:
			templateId = 112310;
			break;
		case 70006:
			templateId = 112311;
			break;
		case 70007:
			templateId = 112312;
			break;
		case 70008:
			templateId = 112313;
			break;
		case 70009:
			templateId = 112314;
			break;
		case 70010:
			templateId = 112365;
			break;
		case 70011:
			templateId = 112366;
			break;
		case 70012:
			templateId = 1120299;
			break;
		case 70013:
			templateId = 1120526;
			break;
		case 70014:
			templateId = 1121554;
			break;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
			itemInfo.Count = 1;
			itemInfo.ValidDate = 0;
			itemInfo.IsBinds = true;
			list.Add(itemInfo);
			int num = QXDropId - 70000;
			int qXDropId = QXDropId;
			int num2 = qXDropId;
			if ((uint)(num2 - 70006) <= 8u)
			{
				num -= 2;
			}
			string translation = LanguageMgr.GetTranslation("GamePlayer.Msg22", num);
			WorldEventMgr.SendItemsToMails(list, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, Player.ZoneId, translation);
			QXDropId = 0;
		}
	}

	public void RemoveGetDragonBoatAward()
	{
		lock (m_lock)
		{
			m_activeInfo.CanGetGift = false;
		}
	}

	public void BeginChristmasTimer()
	{
		int num = 60000;
		if (_christmasTimer == null)
		{
			_christmasTimer = new Timer(ChristmasTimeCheck, null, num, num);
		}
		else
		{
			_christmasTimer.Change(num, num);
		}
	}

	protected void ChristmasTimeCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateChristmasTime();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("ChristmasTimeCheck: " + ex);
		}
	}

	public void StopChristmasTimer()
	{
		if (_christmasTimer != null)
		{
			_christmasTimer.Dispose();
			_christmasTimer = null;
		}
	}

	public void UpdateChristmasTime()
	{
		TimeSpan timeSpan = DateTime.Now - Christmas.gameBeginTime;
		double num = (Christmas.gameEndTime - Christmas.gameBeginTime).TotalMinutes - timeSpan.TotalMinutes;
		lock (m_christmas)
		{
			m_christmas.AvailTime = (((int)num >= 0) ? ((int)num) : 0);
		}
	}

	public void AddTime(int min)
	{
		lock (m_christmas)
		{
			m_christmas.AvailTime += min;
		}
	}

	private void BeginLabyrinthTimer()
	{
		int num = 1000;
		if (_labyrinthTimer == null)
		{
			_labyrinthTimer = new Timer(LabyrinthCheck, null, num, num);
		}
		else
		{
			_labyrinthTimer.Change(num, num);
		}
	}

	protected void LabyrinthCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateLabyrinthTime();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("LabyrinthCheck: " + ex);
		}
	}

	public void StopLabyrinthTimer()
	{
		if (_labyrinthTimer != null)
		{
			_labyrinthTimer.Change(-1, -1);
			_labyrinthTimer.Dispose();
			_labyrinthTimer = null;
		}
	}

	public void UpdateLabyrinthTime()
	{
		UserLabyrinthInfo labyrinth = Player.Labyrinth;
		labyrinth.isCleanOut = true;
		labyrinth.isInGame = true;
		if (labyrinth.remainTime > 0 && labyrinth.currentRemainTime > 0)
		{
			labyrinth.remainTime--;
			labyrinth.currentRemainTime--;
			m_labyrinthCountDown--;
		}
		if (m_labyrinthCountDown == 0)
		{
			GetLabyrinthAward();
			m_labyrinthCountDown = 120;
			labyrinth.currentFloor++;
			if (labyrinth.currentFloor > labyrinth.myProgress)
			{
				labyrinth.currentFloor = labyrinth.myProgress;
				labyrinth.isCleanOut = false;
				labyrinth.isInGame = false;
				labyrinth.completeChallenge = false;
				labyrinth.remainTime = 0;
				labyrinth.currentRemainTime = 0;
				labyrinth.cleanOutAllTime = 0;
				StopLabyrinthTimer();
			}
		}
		Player.Out.SendLabyrinthUpdataInfoOld(Player.PlayerId, labyrinth);
	}

	public void CleantOutLabyrinth()
	{
		BeginLabyrinthTimer();
	}

	private void GetLabyrinthAward()
	{
		int num = m_player.Labyrinth.currentFloor - 1;
		int[] array = m_player.CreateExps();
		int num2 = array[num];
		string text = m_player.LabyrinthGolds[num];
		int num3 = int.Parse(text.Split('|')[0]);
		int num4 = int.Parse(text.Split('|')[1]);
		ItemInfo itemByTemplateID = m_player.PropBag.GetItemByTemplateID(0, 11916);
		if (itemByTemplateID == null || !m_player.RemoveTemplate(11916, 1))
		{
			m_player.Labyrinth.isDoubleAward = false;
		}
		if (m_player.Labyrinth.isDoubleAward)
		{
			int num5 = 2;
			num2 *= num5;
			num3 *= num5;
			num4 *= num5;
		}
		m_player.Labyrinth.accumulateExp += num2;
		List<ItemInfo> list = new List<ItemInfo>();
		if (CanGetBigAward())
		{
			list = m_player.CopyDrop(2, 40002);
			m_player.AddTemplate(list, num3);
			m_player.AddHardCurrency(num4);
		}
		m_player.AddGP(num2, canDupeExp: false, canAddTop: false);
		SendPlusCleantOutInfo(m_player.Labyrinth.currentFloor, num2, num4, list);
	}

	private bool CanGetBigAward()
	{
		bool result = false;
		for (int i = 0; i <= m_player.Labyrinth.myProgress; i += 2)
		{
			if (i == m_player.Labyrinth.currentFloor)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void SendPlusCleantOutInfo(int FamRaidLevel, int exp, int HardCurrency, List<ItemInfo> lists)
	{
		if (lists == null)
		{
			lists = new List<ItemInfo>();
		}
		GSPacketIn gSPacketIn = new GSPacketIn(131, m_player.PlayerId);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(FamRaidLevel);
		gSPacketIn.WriteInt(exp);
		gSPacketIn.WriteInt(lists.Count);
		foreach (ItemInfo list in lists)
		{
			gSPacketIn.WriteInt(list.TemplateID);
			gSPacketIn.WriteInt(list.Count);
		}
		gSPacketIn.WriteInt(HardCurrency);
		m_player.SendTCP(gSPacketIn);
	}

	public void StopCleantOutLabyrinth()
	{
		UserLabyrinthInfo labyrinth = Player.Labyrinth;
		labyrinth.isCleanOut = false;
		Player.Out.SendLabyrinthUpdataInfoOld(Player.PlayerId, labyrinth);
		StopLabyrinthTimer();
	}

	public void SpeededUpCleantOutLabyrinth()
	{
		UserLabyrinthInfo labyrinth = Player.Labyrinth;
		labyrinth.isCleanOut = false;
		labyrinth.isInGame = false;
		labyrinth.completeChallenge = false;
		labyrinth.remainTime = 0;
		labyrinth.currentRemainTime = 0;
		labyrinth.cleanOutAllTime = 0;
		for (int i = labyrinth.currentFloor; i <= labyrinth.myProgress; i++)
		{
			GetLabyrinthAward();
			labyrinth.currentFloor++;
		}
		labyrinth.currentFloor = labyrinth.myProgress;
		Player.Out.SendLabyrinthUpdataInfoOld(Player.PlayerId, labyrinth);
		StopLabyrinthTimer();
	}

	public bool AvailTime()
	{
		DateTime gameBeginTime = Christmas.gameBeginTime;
		DateTime gameEndTime = Christmas.gameEndTime;
		TimeSpan timeSpan = DateTime.Now - gameBeginTime;
		double num = (gameEndTime - gameBeginTime).TotalMinutes - timeSpan.TotalMinutes;
		return num > 0.0;
	}

	public void CreateActiveSystemInfo()
	{
		lock (m_lock)
		{
			int[] value = new int[6];
			m_activeInfo = new ActiveSystemInfo
			{
				ID = 0,
				UserID = Player.PlayerCharacter.ID,
				useableScore = 0,
				totalScore = 0,
				AvailTime = 0,
				NickName = Player.PlayerCharacter.NickName,
				ZoneName = Player.ZoneName,
				dayScore = 0,
				CanGetGift = true,
				canOpenCounts = 5,
				canEagleEyeCounts = 5,
				lastFlushTime = DateTime.Now,
				isShowAll = true,
				ActiveMoney = 0,
				activityTanabataNum = 0,
				LuckystarCoins = defaultCoins,
				DDPlayPoint = 0,
				updateFreeCounts = int.Parse(GameProperties.WorshipMoonPriceInfo.Split('|')[0]),
				updateWorshipedCounts = 0,
				update200State = 0,
				lastEnterWorshiped = DateTime.Now,
				luckCount = 0,
				remainTimes = 0,
				ChallengeNum = GameProperties.YearMonsterFightNum,
				BuyBuffNum = GameProperties.YearMonsterFightNum,
				lastEnterYearMonter = DateTime.Now,
				DamageNum = 0,
				LastRefresh = DateTime.Now,
				CurRefreshedTimes = 0,
				ChickActiveData = "0",
				CurrentSelectIndex = 1,
				loginDays = 1,
				wellAddRate = 2,
				lastLoginDays = DateTime.Now.Date,
				fightPowerSignData = JsonConvert.SerializeObject(value),
				SXMapInfoData = "",
				SXCrystal = 0,
				SXScore = 0,
				SXStepRemain = GameProperties.SanXiaoResetCount,
				SXRewardsGet = "",
				MiniShopBuyCount = "",
				MemoryGameCount = GameProperties.MemoryGameResetCount,
				MemoryGameScore = 0,
				MemoryGameAwardGetData = "",
				MemoryGameData = "",
				cardScore = 0,
				cardChipCount = 0,
				cardFreeCount = 0,
				cardListCard = JsonConvert.SerializeObject(ListCards),
				cardListExchange = JsonConvert.SerializeObject(ListExchanges),
				cardListAward = JsonConvert.SerializeObject(ListAwards),
				rewardQuestData = "",
				promoteLevel = 0,
				godLevel = 1
			};
			CreateYearMonterBoxState();
			SetupCryptBossOpenDay();
			CreateSXMap();
		}
	}

	private void SetRewardData()
	{
		RewardTask = JsonConvert.DeserializeObject<UserRewardQuestInfo>(m_activeInfo.rewardQuestData);
		if (RewardTask == null)
		{
			CreateRandomRewardTask();
		}
	}

	public void CreateRandomRewardTask()
	{
		List<QuestInfo> list = QuestMgr.GetAllRewardQuest().ToList();
		int index = rand.Next(list.Count);
		string msg = "";
		if (RewardTask == null)
		{
			RewardTask = new UserRewardQuestInfo();
			RewardTask.buyTimes = 0;
			RewardTask.times = 0;
		}
		else if (RewardTask.status != 2)
		{
			BaseQuest baseQuest = Player.QuestInventory.FindQuest(RewardTask.questID);
			if (baseQuest != null)
			{
				Player.QuestInventory.RemoveQuest(baseQuest);
			}
		}
		RewardTask.questID = list[index].ID;
		RewardTask.multiple = 1;
		RewardTask.status = 0;
		Player.QuestInventory.AddQuest(list[index], out msg);
	}

	public void SynSignBuff()
	{
		if (!(m_activeInfo.lastLoginDays.Date < DateTime.Now.Date))
		{
			return;
		}
		int days = (DateTime.Now.Date - m_activeInfo.lastLoginDays.Date).Days;
		m_activeInfo.lastLoginDays = DateTime.Now.Date;
		if (days > 1)
		{
			m_activeInfo.wellAddRate -= 3 * days;
			if (m_activeInfo.wellAddRate < 0)
			{
				m_activeInfo.wellAddRate = 0;
			}
		}
		m_activeInfo.loginDays++;
		m_activeInfo.wellAddRate += 2;
		if (m_activeInfo.wellAddRate > GameProperties.MaxWellSignBuff)
		{
			m_activeInfo.wellAddRate = GameProperties.MaxWellSignBuff;
		}
	}

	public void SynFightPowerSign()
	{
		if (m_activeInfo.fightPowerSignData == null)
		{
			m_activeInfo.fightPowerSignData = "";
		}
		List<int> prop = GameProperties.getProp(GameProperties.FightPowerSignData);
		int[] array = JsonConvert.DeserializeObject<int[]>(m_activeInfo.fightPowerSignData);
		if (array == null || array.Length == 0)
		{
			array = new int[6];
		}
		for (int i = 0; i < prop.Count; i++)
		{
			if (array[i] == 0 && m_player.PlayerCharacter.FightPower >= prop[i])
			{
				array[i] = 1;
			}
		}
		m_activeInfo.fightPowerSignData = JsonConvert.SerializeObject(array);
	}

	public string DefaultChickActiveData()
	{
		return "0,0," + DateTime.Now.ToString() + "," + DateTime.Now.ToShortDateString() + "," + DateTime.Now.ToShortDateString() + "," + DateTime.Now.ToShortDateString() + ",0";
	}

	public UserChickActiveInfo GetChickActiveData()
	{
		UserChickActiveInfo userChickActiveInfo = null;
		if (m_activeInfo.ChickActiveData == "0")
		{
			m_activeInfo.ChickActiveData = DefaultChickActiveData();
		}
		string[] array = m_activeInfo.ChickActiveData.Split(',');
		if (array.Length != 0)
		{
			userChickActiveInfo = new UserChickActiveInfo();
			userChickActiveInfo.IsKeyOpened = int.Parse(array[0]);
			userChickActiveInfo.KeyOpenedType = int.Parse(array[1]);
			userChickActiveInfo.KeyOpenedTime = DateTime.Parse(array[2]);
			userChickActiveInfo.EveryDay = DateTime.Parse(array[3]);
			userChickActiveInfo.Weekly = DateTime.Parse(array[4]);
			userChickActiveInfo.AfterThreeDays = DateTime.Parse(array[5]);
			userChickActiveInfo.CurrentLvAward = int.Parse(array[6]);
		}
		return userChickActiveInfo;
	}

	public bool SaveChickActiveData(UserChickActiveInfo data)
	{
		if (data != null)
		{
			string[] value = new string[7]
			{
				data.IsKeyOpened.ToString(),
				data.KeyOpenedType.ToString(),
				data.KeyOpenedTime.ToString(),
				data.EveryDay.ToShortDateString(),
				data.Weekly.ToShortDateString(),
				data.AfterThreeDays.ToShortDateString(),
				data.CurrentLvAward.ToString()
			};
			m_activeInfo.ChickActiveData = string.Join(",", value);
			return true;
		}
		return false;
	}

	public void SendUpdateChickActivation()
	{
		UserChickActiveInfo chickActiveData = GetChickActiveData();
		if (chickActiveData != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(84);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(chickActiveData.IsKeyOpened);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteDateTime(chickActiveData.KeyOpenedTime);
			gSPacketIn.WriteInt(chickActiveData.KeyOpenedType);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Monday) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Tuesday) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Wednesday) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Thursday) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Friday) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Saturday) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != 0) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.AfterThreeDays.Day >= DateTime.Now.Day || !chickActiveData.OnThreeDay(DateTime.Now)) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.AfterThreeDays.Day >= DateTime.Now.Day || !chickActiveData.OnThreeDay(DateTime.Now)) ? 1 : 0);
			gSPacketIn.WriteInt((chickActiveData.AfterThreeDays.Day >= DateTime.Now.Day || !chickActiveData.OnThreeDay(DateTime.Now)) ? 1 : 0);
			gSPacketIn.WriteInt((!(chickActiveData.Weekly < chickActiveData.StartOfWeek(DateTime.Now, DayOfWeek.Saturday)) || DateTime.Now.DayOfWeek != DayOfWeek.Saturday) ? 1 : 0);
			gSPacketIn.WriteInt(chickActiveData.CurrentLvAward);
			m_player.SendTCP(gSPacketIn);
		}
	}

	public bool YearMonterValidate()
	{
		if (Info.lastEnterYearMonter.Date < DateTime.Now.Date)
		{
			lock (m_lock)
			{
				m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
				m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
				m_activeInfo.lastEnterYearMonter = DateTime.Now;
				m_activeInfo.DamageNum = 0;
				CreateYearMonterBoxState();
			}
		}
		return true;
	}

	public void CreateYearMonterBoxState()
	{
		string[] array = GameProperties.YearMonsterBoxInfo.Split('|');
		int num = array.Length;
		string[] array2 = new string[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = int.Parse(array[i].Split(',')[1]) * 10000;
			if (num2 <= m_activeInfo.DamageNum)
			{
				array2[i] = "2";
			}
			else
			{
				array2[i] = "1";
			}
		}
		m_activeInfo.BoxState = string.Join("-", array2);
	}

	public void SetYearMonterBoxState(int id)
	{
		string[] array = m_activeInfo.BoxState.Split('-');
		int num = array.Length;
		string[] array2 = new string[num];
		for (int i = 0; i < num; i++)
		{
			if (i == id)
			{
				array2[i] = "3";
			}
			else
			{
				array2[i] = array[i];
			}
		}
		m_activeInfo.BoxState = string.Join("-", array2);
	}

	public void CreateChristmasInfo(int UserID)
	{
		lock (m_lock)
		{
			m_christmas = new UserChristmasInfo();
			m_christmas.ID = 0;
			m_christmas.UserID = UserID;
			m_christmas.count = 0;
			m_christmas.exp = 0;
			m_christmas.awardState = 0;
			m_christmas.lastPacks = 1100;
			m_christmas.packsNumber = -1;
			m_christmas.gameBeginTime = DateTime.Now;
			m_christmas.gameEndTime = DateTime.Now.AddMinutes(60.0);
			m_christmas.isEnter = false;
			m_christmas.dayPacks = 0;
			m_christmas.AvailTime = 0;
		}
	}

	public void SendChickenBoxItemList()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteDateTime(Info.lastFlushTime);
		gSPacketIn.WriteInt(freeFlushTime);
		gSPacketIn.WriteInt(freeRefreshBoxCount);
		gSPacketIn.WriteInt(freeEyeCount);
		gSPacketIn.WriteInt(freeOpenCardCount);
		gSPacketIn.WriteBoolean(Info.isShowAll);
		gSPacketIn.WriteInt(ChickenBoxRewards.Length);
		NewChickenBoxItemInfo[] chickenBoxRewards = ChickenBoxRewards;
		foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
		{
			gSPacketIn.WriteInt(newChickenBoxItemInfo.TemplateID);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.StrengthenLevel);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.Count);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.ValidDate);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.AttackCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.DefendCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.AgilityCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.LuckCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.Position);
			gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSelected);
			gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSeeded);
			gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsBinds);
		}
		m_player.SendTCP(gSPacketIn);
	}

	private void SetupLuckyStart()
	{
		m_luckyBegindate = DateTime.Parse(GameProperties.LuckStarActivityBeginDate);
		m_luckyEnddate = DateTime.Parse(GameProperties.LuckStarActivityEndDate);
		m_minUseNum = GameProperties.MinUseNum;
		LuckyStartStartTurn = DateTime.Now;
	}

	public void CreateLuckyStartAward()
	{
		m_LuckyStartRewards = CreateChickenBoxAward(LuckyStartBoxCount, eEventType.LUCKY_STAR);
		NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
		newChickenBoxItemInfo.TemplateID = coinTemplateID;
		newChickenBoxItemInfo.StrengthenLevel = 0;
		newChickenBoxItemInfo.Count = 1;
		newChickenBoxItemInfo.IsBinds = true;
		newChickenBoxItemInfo.Quality = 1;
		m_LuckyStartRewards[0] = newChickenBoxItemInfo;
		ThreadSafeRandom.ShufferStatic(m_LuckyStartRewards);
	}

	public void ChangeLuckyStartAwardPlace()
	{
		ThreadSafeRandom.ShufferStatic(m_LuckyStartRewards);
	}

	public void SendLuckStarAllGoodsInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(21);
		gSPacketIn.WriteInt(m_activeInfo.LuckystarCoins);
		gSPacketIn.WriteDateTime(LuckyBegindate);
		gSPacketIn.WriteDateTime(LuckyEnddate);
		gSPacketIn.WriteInt(minUseNum);
		int num = m_LuckyStartRewards.Length;
		int i = 0;
		gSPacketIn.WriteInt(num);
		for (; i < num; i++)
		{
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].TemplateID);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].StrengthenLevel);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].Count);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].ValidDate);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].AttackCompose);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].DefendCompose);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].AgilityCompose);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].LuckCompose);
			gSPacketIn.WriteBoolean(m_LuckyStartRewards[i].IsBinds);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].Quality);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendLuckStarRewardRecord()
	{
		List<LuckStarRewardRecordInfo> recordList = ActiveSystemMgr.RecordList;
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(22);
		gSPacketIn.WriteInt(recordList.Count);
		foreach (LuckStarRewardRecordInfo item in recordList)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteString(item.nickName);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendUpdateReward()
	{
		if (Award.TemplateID != coinTemplateID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
			gSPacketIn.WriteInt(24);
			gSPacketIn.WriteInt(Award.TemplateID);
			gSPacketIn.WriteInt(Award.Count);
			gSPacketIn.WriteString(m_player.PlayerCharacter.NickName);
			m_player.SendTCP(gSPacketIn);
		}
	}

	private void GetAward()
	{
		int num = rand.Next(m_LuckyStartRewards.Length - 1);
		NewChickenBoxItemInfo newChickenBoxItemInfo = m_LuckyStartRewards[num];
		if (newChickenBoxItemInfo.TemplateID == coinTemplateID && rand.Next(100) > 3)
		{
			List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
			NewChickenBoxItemInfo[] luckyStartRewards = m_LuckyStartRewards;
			foreach (NewChickenBoxItemInfo newChickenBoxItemInfo2 in luckyStartRewards)
			{
				if (newChickenBoxItemInfo2.TemplateID != coinTemplateID)
				{
					list.Add(newChickenBoxItemInfo2);
				}
			}
			num = rand.Next(list.Count - 1);
			newChickenBoxItemInfo = list[num];
		}
		lock (m_lock)
		{
			m_award = newChickenBoxItemInfo;
		}
	}

	public void SendLuckStarTurnGoodsInfo()
	{
		GetAward();
		m_activeInfo.LuckystarCoins += flushCoins;
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(23);
		gSPacketIn.WriteInt(m_activeInfo.LuckystarCoins);
		gSPacketIn.WriteInt(Award.TemplateID);
		gSPacketIn.WriteInt(Award.StrengthenLevel);
		gSPacketIn.WriteInt(Award.Count);
		gSPacketIn.WriteInt(Award.ValidDate);
		gSPacketIn.WriteInt(Award.AttackCompose);
		gSPacketIn.WriteInt(Award.DefendCompose);
		gSPacketIn.WriteInt(Award.AgilityCompose);
		gSPacketIn.WriteInt(Award.LuckCompose);
		gSPacketIn.WriteBoolean(Award.IsBinds);
		m_player.SendTCP(gSPacketIn);
		if (Award.TemplateID == coinTemplateID)
		{
			if (GameProperties.IsActiveMoney)
			{
				m_player.AddActiveMoney(m_activeInfo.LuckystarCoins);
			}
			else
			{
				m_player.LogAddMoney(AddMoneyType.LuckyStar, AddMoneyType.LuckyStar_Add, m_player.PlayerCharacter.ID, m_activeInfo.LuckystarCoins, m_player.PlayerCharacter.Money);
				m_player.AddMoney(m_activeInfo.LuckystarCoins);
			}
			m_activeInfo.LuckystarCoins = defaultCoins;
		}
		ActiveSystemMgr.UpdateLuckStarRewardRecord(m_player.PlayerCharacter.ID, m_player.PlayerCharacter.NickName, Award.TemplateID, Award.Count, m_player.PlayerCharacter.typeVIP);
	}

	public void SendLuckStarRewardRank()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(27);
		List<LuckyStartToptenAwardInfo> luckyStartToptenAward = WorldEventMgr.GetLuckyStartToptenAward();
		gSPacketIn.WriteInt(luckyStartToptenAward.Count);
		foreach (LuckyStartToptenAwardInfo item in luckyStartToptenAward)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.StrengthenLevel);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteInt(item.Validate);
			gSPacketIn.WriteInt(item.AttackCompose);
			gSPacketIn.WriteInt(item.DefendCompose);
			gSPacketIn.WriteInt(item.AgilityCompose);
			gSPacketIn.WriteInt(item.LuckCompose);
			gSPacketIn.WriteBoolean(item.IsBinds);
			gSPacketIn.WriteInt(item.Type);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public GSPacketIn SendLightriddleRank(int myRank, List<RankingLightriddleInfo> list)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(42);
		gSPacketIn.WriteInt(myRank);
		gSPacketIn.WriteInt(list.Count);
		foreach (RankingLightriddleInfo item in list)
		{
			gSPacketIn.WriteInt(item.Rank);
			gSPacketIn.WriteString(item.NickName);
			gSPacketIn.WriteByte((byte)item.TypeVIP);
			gSPacketIn.WriteInt(item.Integer);
			List<LuckyStartToptenAwardInfo> lanternriddlesAwardByRank = WorldEventMgr.GetLanternriddlesAwardByRank(item.Rank);
			gSPacketIn.WriteInt(lanternriddlesAwardByRank.Count);
			foreach (LuckyStartToptenAwardInfo item2 in lanternriddlesAwardByRank)
			{
				gSPacketIn.WriteInt(item2.TemplateID);
				gSPacketIn.WriteInt(item2.Count);
				gSPacketIn.WriteBoolean(item2.IsBinds);
				gSPacketIn.WriteInt(item2.Validate);
			}
		}
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLightriddleQuestion(LanternriddlesInfo Lanternriddles)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(38);
		gSPacketIn.WriteInt(Lanternriddles.QuestionIndex);
		gSPacketIn.WriteInt(Lanternriddles.GetQuestionID);
		gSPacketIn.WriteInt(Lanternriddles.QuestionView);
		gSPacketIn.WriteDateTime(Lanternriddles.EndDate);
		gSPacketIn.WriteInt(Lanternriddles.DoubleFreeCount);
		gSPacketIn.WriteInt(Lanternriddles.DoublePrice);
		gSPacketIn.WriteInt(Lanternriddles.HitFreeCount);
		gSPacketIn.WriteInt(Lanternriddles.HitPrice);
		gSPacketIn.WriteInt(Lanternriddles.MyInteger);
		gSPacketIn.WriteInt(Lanternriddles.QuestionNum);
		gSPacketIn.WriteInt(Lanternriddles.Option);
		gSPacketIn.WriteBoolean(Lanternriddles.IsHint);
		gSPacketIn.WriteBoolean(Lanternriddles.IsDouble);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLightriddleAnswerResult(bool Iscorrect, int option, string award)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(39);
		gSPacketIn.WriteBoolean(Iscorrect);
		gSPacketIn.WriteBoolean(Iscorrect);
		gSPacketIn.WriteInt(option);
		gSPacketIn.WriteString(award);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void BeginLightriddleTimer()
	{
		int num = 1000;
		if (_lightriddleTimer == null)
		{
			_lightriddleTimer = new Timer(LightriddleCheck, null, num, num);
		}
		else
		{
			_lightriddleTimer.Change(num, num);
		}
	}

	protected void LightriddleCheck(object sender)
	{
		try
		{
			if (_lightriddleStart)
			{
				_lightriddleStart = true;
			}
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			if (_lightriddleColdown > 0)
			{
				_lightriddleColdown--;
				if (_lightriddleColdown == 1)
				{
					LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(m_player.PlayerId);
					if (lanternriddles == null)
					{
						StopLightriddleTimer();
						return;
					}
					LightriddleQuestInfo getCurrentQuestion = lanternriddles.GetCurrentQuestion;
					string translation = LanguageMgr.GetTranslation("Lightriddle.Msg1");
					string translation2 = LanguageMgr.GetTranslation("Lightriddle.Msg2");
					string award = LanguageMgr.GetTranslation("Lightriddle.Msg3");
					int gp = 1000;
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(100100);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 105);
					bool iscorrect = false;
					if (getCurrentQuestion != null)
					{
						if (lanternriddles.IsHint)
						{
							lanternriddles.Option = getCurrentQuestion.OptionTrue;
						}
						if (lanternriddles.Option == getCurrentQuestion.OptionTrue)
						{
							itemInfo.Count = 5;
							gp = 10000;
							lanternriddles.MyInteger += 29;
							lanternriddles.QuestionNum++;
							if (lanternriddles.IsDouble)
							{
								translation = LanguageMgr.GetTranslation("Lightriddle.Msg4");
								lanternriddles.MyInteger += 29;
							}
							award = translation;
							iscorrect = true;
						}
						else
						{
							itemInfo.Count = 1;
							award = translation2;
						}
					}
					if (lanternriddles.Option > 0)
					{
						m_player.AddGP(gp, canDupeExp: false, canAddTop: false);
						itemInfo.IsBinds = true;
						m_player.AddTemplate(itemInfo);
						SendLightriddleAnswerResult(iscorrect, lanternriddles.Option, award);
						GameServer.Instance.LoginServer.SendLightriddleUpateRank(lanternriddles.MyInteger, m_player.PlayerCharacter);
					}
					GameServer.Instance.LoginServer.SendLightriddleInfo(lanternriddles);
				}
			}
			else
			{
				LanternriddlesInfo lanternriddles2 = ActiveSystemMgr.GetLanternriddles(m_player.PlayerId);
				if (lanternriddles2 == null)
				{
					StopLightriddleTimer();
					return;
				}
				if (lanternriddles2.CanNextQuest)
				{
					lanternriddles2.QuestionIndex++;
					lanternriddles2.Option = -1;
					lanternriddles2.IsHint = false;
					lanternriddles2.IsDouble = false;
					lanternriddles2.EndDate = ActiveSystemMgr.EndDate;
					SendLightriddleQuestion(lanternriddles2);
					_lightriddleColdown = 15;
					GameServer.Instance.LoginServer.SendLightriddleRank(m_player.PlayerCharacter.NickName, m_player.PlayerCharacter.ID);
				}
				else
				{
					lanternriddles2.QuestionIndex = lanternriddles2.QuestionView;
					lanternriddles2.IsHint = true;
					lanternriddles2.IsDouble = true;
					lanternriddles2.EndDate = DateTime.Now;
					SendLightriddleQuestion(lanternriddles2);
					StopLightriddleTimer();
				}
				GameServer.Instance.LoginServer.SendLightriddleInfo(lanternriddles2);
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("LabyrinthCheck: " + ex);
		}
	}

	public void StopLightriddleTimer()
	{
		if (_lightriddleTimer != null)
		{
			_lightriddleColdown = 15;
			_lightriddleStart = false;
			_lightriddleTimer.Dispose();
			_lightriddleTimer = null;
		}
	}

	private Dictionary<int, Dictionary<int, SXCellDataInfo>> InitSXMap()
	{
		Dictionary<int, Dictionary<int, SXCellDataInfo>> dictionary = new Dictionary<int, Dictionary<int, SXCellDataInfo>>();
		for (int i = 0; i < SXRowLength; i++)
		{
			Dictionary<int, SXCellDataInfo> value = new Dictionary<int, SXCellDataInfo>();
			dictionary.Add(i, value);
			for (int j = 0; j < SXColumnLength; j++)
			{
				dictionary[i].Add(j, new SXCellDataInfo(i, j));
			}
		}
		return dictionary;
	}

	public Dictionary<int, Dictionary<int, SXCellDataInfo>> CreateSXMap()
	{
		Dictionary<int, Dictionary<int, SXCellDataInfo>> dictionary = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(m_activeInfo.SXMapInfoData);
		if (dictionary == null)
		{
			dictionary = InitSXMap();
		}
		for (int i = 0; i < SXRowLength; i++)
		{
			for (int j = 0; j < SXColumnLength; j++)
			{
				int num = rand.Next(1, 6);
				int type = dictionary[Math.Max(0, i - 1)][j].type;
				if (type == num)
				{
					num = (type + 2) % 5 + 1;
				}
				type = dictionary[i][Math.Max(0, j - 2)].type;
				if (type == num)
				{
					num = (num + 2) % 5 + 1;
				}
				dictionary[i][j].type = num;
			}
		}
		m_activeInfo.SXMapInfoData = JsonConvert.SerializeObject(dictionary);
		return dictionary;
	}

	public void SendSXRequireMap()
	{
		Dictionary<int, Dictionary<int, SXCellDataInfo>> dictionary = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, SXCellDataInfo>>>(m_activeInfo.SXMapInfoData);
		if (dictionary == null)
		{
			dictionary = CreateSXMap();
		}
		GSPacketIn gSPacketIn = new GSPacketIn(329, m_player.PlayerId);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(SXRowLength * SXColumnLength);
		for (int i = 0; i < SXRowLength; i++)
		{
			for (int j = 0; j < SXColumnLength; j++)
			{
				gSPacketIn.WriteInt(i);
				gSPacketIn.WriteInt(j);
				gSPacketIn.WriteInt(dictionary[i][j].type);
			}
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendSXData()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(m_activeInfo.SXScore);
		gSPacketIn.WriteInt(m_activeInfo.SXStepRemain);
		gSPacketIn.WriteInt(m_activeInfo.SXCrystal);
		gSPacketIn.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
		m_player.SendTCP(gSPacketIn);
	}

	public void SendSXBuyTime()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(15);
		gSPacketIn.WriteInt(m_activeInfo.SXStepRemain);
		m_player.SendTCP(gSPacketIn);
	}

	public void SendSXRewardsData(List<int> datas)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(17);
		gSPacketIn.WriteInt(datas.Count);
		foreach (int data in datas)
		{
			gSPacketIn.WriteInt(data);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendSXStoreData(Dictionary<int, int> datas)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(18);
		gSPacketIn.WriteInt(m_activeInfo.SXCrystal);
		gSPacketIn.WriteInt(datas.Count);
		foreach (KeyValuePair<int, int> data in datas)
		{
			gSPacketIn.WriteInt(data.Key);
			gSPacketIn.WriteInt(data.Value);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendSXGainedDropItems(List<ItemInfo> listItems)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(19);
		gSPacketIn.WriteInt(listItems.Count);
		foreach (ItemInfo listItem in listItems)
		{
			gSPacketIn.WriteInt(listItem.TemplateID);
			gSPacketIn.WriteInt(listItem.Count);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public List<MemoryGameInfo> CreateMemoryGameData()
	{
		List<MemoryGameInfo> list = new List<MemoryGameInfo>();
		List<ItemBoxInfo> list2 = ItemBoxMgr.FindItemBoxByRand(GameProperties.MemoryGameAwardBox, MemoryGameMaxCell / 2);
		List<int> list3 = new List<int>();
		for (int i = 1; i <= MemoryGameMaxCell; i++)
		{
			list3.Add(i);
		}
		foreach (ItemBoxInfo item in list2)
		{
			MemoryGameInfo memoryGameInfo = new MemoryGameInfo();
			memoryGameInfo.TemplateID = item.TemplateId;
			memoryGameInfo.count = item.ItemCount;
			memoryGameInfo.isGet = false;
			memoryGameInfo.show1 = false;
			memoryGameInfo.show2 = false;
			int index = rand.Next(list3.Count);
			memoryGameInfo.place1 = list3[index];
			list3.RemoveAt(index);
			index = rand.Next(list3.Count);
			memoryGameInfo.place2 = list3[index];
			list3.RemoveAt(index);
			list.Add(memoryGameInfo);
		}
		m_activeInfo.MemoryGameData = JsonConvert.SerializeObject(list);
		return list;
	}

	public void SendMemoryGameData(List<MemoryGameInfo> memoryGameData, bool isOld)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteInt(m_activeInfo.MemoryGameCount);
		gSPacketIn.WriteInt(m_activeInfo.MemoryGameScore);
		gSPacketIn.WriteInt(memoryGameData.Count);
		foreach (MemoryGameInfo memoryGameDatum in memoryGameData)
		{
			gSPacketIn.WriteInt(memoryGameDatum.TemplateID);
			gSPacketIn.WriteInt(memoryGameDatum.count);
			gSPacketIn.WriteInt(memoryGameDatum.place1);
			gSPacketIn.WriteInt(memoryGameDatum.place2);
			gSPacketIn.WriteBoolean(memoryGameDatum.isGet);
			gSPacketIn.WriteBoolean(memoryGameDatum.show1);
			gSPacketIn.WriteBoolean(memoryGameDatum.show2);
		}
		gSPacketIn.WriteBoolean(isOld);
		List<int> list = JsonConvert.DeserializeObject<List<int>>(m_activeInfo.MemoryGameAwardGetData);
		if (list != null)
		{
			gSPacketIn.WriteInt(list.Count);
			foreach (int item in list)
			{
				gSPacketIn.WriteInt(item);
			}
		}
		else
		{
			gSPacketIn.WriteInt(0);
		}
		m_player.SendTCP(gSPacketIn);
	}
}

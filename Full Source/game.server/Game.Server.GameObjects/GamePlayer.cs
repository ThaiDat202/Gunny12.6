using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Academy;
using Game.Server.Achievements;
using Game.Server.ActiveSystem;
using Game.Server.AnotherDimension;
using Game.Server.AvatarCollection;
using Game.Server.BombKing;
using Game.Server.Buffer;
using Game.Server.BuffHorseRace;
using Game.Server.CampBattle;
using Game.Server.CardAchievements;
using Game.Server.CardNewSystem;
using Game.Server.CollectionTask;
using Game.Server.Consortia;
using Game.Server.ConsortiaTask;
using Game.Server.DDTQiYuan;
using Game.Server.DevilTurn;
using Game.Server.DragonBoat;
using Game.Server.Elf;
using Game.Server.Emblem;
using Game.Server.EquipAmulet;
using Game.Server.Farm;
using Game.Server.GameRoom;
using Game.Server.Games;
using Game.Server.GameUtils;
using Game.Server.GodCardRaise;
using Game.Server.GuardCore;
using Game.Server.GuildBattle;
using Game.Server.GypsyShop;
using Game.Server.HomeAdorn;
using Game.Server.HomeBar;
using Game.Server.HomeFight;
using Game.Server.HomeSystem;
using Game.Server.HomeTemple;
using Game.Server.Horse;
using Game.Server.HorseAmulet;
using Game.Server.HotSpringRooms;
using Game.Server.Labyrinth;
using Game.Server.MagicHouse;
using Game.Server.MagicStone;
using Game.Server.MagpieBridge;
using Game.Server.Managers;
using Game.Server.MemoryGame;
using Game.Server.MoneyTree;
using Game.Server.NewHall;
using Game.Server.Packets;
using Game.Server.Packets.Client;
using Game.Server.Pet;
using Game.Server.Quests;
using Game.Server.Renshen;
using Game.Server.RingStation;
using Game.Server.Rooms;
using Game.Server.SanXiao;
using Game.Server.SceneMarryRooms;
using Game.Server.Statics;
using Game.Server.Team;
using Game.Server.WonderFul;
using Game.Server.WorshipTheMoon;
using Game.Server.Zodiac;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameObjects;

public class GamePlayer : IGamePlayer
{
	public delegate void PlayerEventHandle(GamePlayer player);

	public delegate void PlayerItemPropertyEventHandle(int templateID, int count);

	public delegate void PlayerGameOverEventHandle(AbstractGame game, bool isWin, int gainXp);

	public delegate void PlayerMissionOverEventHandle(AbstractGame game, int missionId, bool isWin);

	public delegate void PlayerMissionFullOverEventHandle(AbstractGame game, int missionId, bool isWin, int turnNum);

	public delegate void PlayerMissionTurnOverEventHandle(AbstractGame game, int missionId, int turnNum);

	public delegate void PlayerQuestOneKeyFinishEventHandle(QuestConditionInfo condition);

	public delegate void PlayerItemStrengthenEventHandle(int categoryID, int level);

	public delegate void PlayerGodTempleEventHandle(int star, int level);

	public delegate void PlayerBuildGpgradeEventHandle(int level);

	public delegate void PlayerMountMasterEventHandle(int level);

	public delegate void PlayerShopEventHandle(int money, int gold, int offer, int gifttoken, int medal, string payGoods);

	public delegate void PlayerAdoptPetEventHandle();

	public delegate void PlayerNewGearEventHandle(int CategoryID);

	public delegate void PlayerCropPrimaryEventHandle();

	public delegate void PlayerSeedFoodPetEventHandle();

	public delegate void PlayerUserToemGemstoneEventHandle(int type);

	public delegate void PlayerCollectionTaskEventHandle(int type);

	public delegate void PlayerUpLevelPetEventHandle();

	public delegate void PlayerItemInsertEventHandle();

	public delegate void PlayerItemFusionEventHandle(int fusionType);

	public delegate void PlayerItemMeltEventHandle(int categoryID);

	public delegate void PlayerGameKillEventHandel(AbstractGame game, int type, int id, bool isLiving, int demage);

	public delegate void PlayerOwnConsortiaEventHandle();

	public delegate void PlayerItemComposeEventHandle(int composeType);

	public delegate void PlayerAchievementFinish(AchievementDataInfo info);

	public delegate void PlayerFightAddOffer(int offer);

	public delegate void PlayerFightOneBloodIsWin(eRoomType roomType);

	public delegate void PlayerVIPUpgrade(int level, int exp);

	public delegate void PlayerHotSpingExpAdd(int minutes, int exp);

	public delegate void PlayerGoldCollection(int value);

	public delegate void PlayerAcademyEventHandle(GamePlayer friendly, int type);

	public delegate void PlayerGiftTokenCollection(int value);

	public delegate void PlayerPropertisChange(PlayerInfo player);

	public delegate void PlayerQuestFinish(QuestDataInfo data, QuestInfo info);

	public delegate void PlayerUseBugle(int value);

	public delegate void GameKillDropEventHandel(AbstractGame game, int type, int npcId, bool playResult);

	public delegate void PlayerFundHandler(int count, int type);

	public delegate void DonateRiches(int value, int type);

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public int LastGetMailID;

	public bool IsLoadingMail = false;

	public ItemInfo LastTakeCardItem;

	private ePlayerState m_playerState = ePlayerState.Offline;

	protected BaseGame m_game;

	private int m_playerId;

	protected GameClient m_client;

	protected Player m_players;

	private PlayerInfo m_character;

	private string m_account;

	private int m_immunity = 255;

	public int FightPower;

	private List<int> m_questColect;

	private bool m_isMinor;

	private bool m_isAASInfo;

	private long m_pingTime;

	private int m_loveSkillId;

	private char[] m_pvepermissions;

	public long PingStart;

	private List<BufferInfo> m_fightBuffInfo;

	public UserPropertiesInfo MarkProperties;

	public MarkChipItemInfo MarkRefineItemTemp;

	private UsersPetInfo m_pet;

	public List<User_GmActivityCondition> GmActivityConditions = new List<User_GmActivityCondition>();

	public List<User_GmActivityReward> GmActivityRewards = new List<User_GmActivityReward>();

	public Random rand;

	private PlayerEquipInventory m_equipBag;

	private PlayerAvataInventory m_avatarBag;

	private PlayerInventory m_propBag;

	private PlayerInventory m_fightBag;

	private PlayerInventory m_consortiaBag;

	private PlayerMagicHouse m_magicHouse;

	private PlayerInventory m_storeBag;

	private PlayerInventory m_tempBag;

	private PlayerInventory m_caddyBag;

	private PlayerInventory m_farmBag;

	private PlayerInventory m_vegetable;

	private PlayerInventory m_food;

	private PlayerInventory m_petEgg;

	private PlayerInventory m_minesBag;

	private PlayerRoomBordenInventory m_RoomBordenBag;

	private PlayerBeadInventory m_beadBag;

	private PlayerMagicStoneInventory m_magicStone;

	private PlayerHorseAmuletInventory m_horseAmulet;

	private CardInventory m_cardBag;

	private PlayerFarm m_farm;

	private PetInventory m_petBag;

	private PlayerTreasure m_treasure;

	private PlayerProperty m_playerProp;

	private PlayerRank m_rank;

	private PlayerDice m_dice;

	private PlayerBattle m_battle;

	private PlayerExtra m_extra;

	private PlayerHomeSysterm m_home;

	private PlayerBank m_bank;

	private PlayerMines m_mines;

	private PlayerMark m_marks;

	private Dictionary<string, UserEquipGhostInfo> m_equipGhostList;

	private PlayerTrailElite m_trailElite;

	private PlayerKingGrade m_kingGrade;

	private PlayerRoulette m_roulette;

	private PlayerHorse m_horse;

	private PlayerActives m_actives;

	private AchievementInventory m_achievementInventory;

	private CardAchievementInventory m_cardAchievementInventory;

	private QuestInventory m_questInventory;

	private BufferList m_bufferList;

	private UserLabyrinthInfo m_Labyrinth;

	private List<UserGemStone> m_GemStone;

	private List<ItemInfo> m_equipEffect;

	private List<ItemInfo> m_horseAmuletEffect;

	private int m_changed;

	public readonly int[] EquipPlace = new int[15]
	{
		1, 2, 3, 4, 5, 6, 11, 13, 14, 15,
		16, 17, 18, 19, 20
	};

	public double GPAddPlus;

	public double OfferAddPlus = 1.0;

	public double GuildRichAddPlus = 1.0;

	public DateTime LastChatTime;

	public string LastChatMsg;

	public DateTime LastFigUpTime;

	public DateTime LastDrillUpTime;

	public DateTime LastOpenPack;

	public DateTime LastOpenGrowthPackage;

	public DateTime LastOpenChristmasPackage;

	public DateTime LastOpenCard;

	public DateTime LastEnterWorldBoss;

	public DateTime WaitingProcessor;

	public bool KickProtect;

	public RingstationBattleFieldInfo DareFlag;

	public RingstationBattleFieldInfo SuccessFlag;

	private double m_basedame = 0.0;

	private double m_baseGuard = 0.0;

	public int WashFightPower = 0;

	private ItemInfo m_MainWeapon;

	private BatleConfigInfo m_batleConfig;

	private ItemInfo m_healstone;

	private ItemInfo m_currentWeapon3D;

	private ItemInfo m_currentSecondWeapon;

	private ItemInfo m_currentAmulet;

	public bool IsUpdateEnterModePoint;

	public readonly string[] LabyrinthGolds = new string[60]
	{
		"0|0", "2|2", "0|0", "2|2", "0|0", "2|3", "0|0", "3|3", "0|0", "3|4",
		"0|0", "3|4", "0|0", "4|5", "0|0", "4|5", "0|0", "4|6", "0|0", "5|6",
		"0|0", "5|7", "0|0", "5|7", "0|0", "6|8", "0|0", "6|8", "0|0", "6|10",
		"0|0", "8|10", "0|0", "8|11", "0|0", "8|11", "0|0", "10|12", "0|0", "10|12",
		"0|0", "10|12", "0|0", "10|13", "0|0", "10|13", "0|0", "12|14", "0|0", "12|14",
		"0|0", "14|15", "0|0", "14|15", "0|0", "14|15", "0|0", "16|16", "0|0", "16|16"
	};

	private List<int> _viFarms;

	private Dictionary<int, int> _friends;

	private BaseRoom _currentRoom;

	private HotSpringRoom m_currentHotSpringRoom;

	private BaseHorseRaceRoom m_currentHorseRaceRoom;

	public UserHorseRaceInfo CurrentHorseRaceInfo;

	public int CurrentRoomIndex;

	public int CurrentRoomTeam;

	protected int m_gameId;

	public int[] newAmuletType = new int[4];

	public int[] newAmuletValue = new int[4];

	public Dictionary<int, int> newAmuletEffects = new Dictionary<int, int>();

	public int WorldBossMap;

	private bool m_hideAllFriend;

	public byte States;

	public bool isPowerFullUsed;

	public int winningStreak;

	public Dictionary<int, CardsTakeOutInfo> Card = new Dictionary<int, CardsTakeOutInfo>();

	public CardsTakeOutInfo[] CardsTakeOut = new CardsTakeOutInfo[9];

	public int canTakeOut;

	public int takeoutCount;

	public int PosX;

	public int PosY;

	public int CollectionTaskPosX;

	public int CollectionTaskPosY;

	public int X;

	public int Y;

	public int Direction = 3;

	public int MarryMap;

	public DateTime EnterRoomTime = DateTime.Now;

	public int EffectiveTime;

	private BaseSevenDoubleRoom m_currentSevenDoubleRoom;

	private MarryRoom m_currentMarryRoom;

	private static char[] permissionChars = new char[7] { '1', '3', '7', 'F', 'F', 'F', 'F' };

	public int LabyrinthTime = 120;

	private Dictionary<string, object> m_tempProperties = new Dictionary<string, object>();

	protected GameRoomLogicProcessor m_gameroomProcessor = new GameRoomLogicProcessor();

	private GameRoomProcessor m_gameRoom;

	protected AvatarCollectionLogicProcessor m_avatarCollectionProcessor = new AvatarCollectionLogicProcessor();

	private AvatarCollectionProcessor m_avatarCollection;

	protected MagicStoneLogicProcessor m_magicStoneProcessor = new MagicStoneLogicProcessor();

	private MagicStoneProcessor m_magicStoneHandler;

	protected NewHallLogicProcessor m_newHallProcessor = new NewHallLogicProcessor();

	private NewHallProcessor m_newHallHandler;

	protected CollectionTaskLogicProcessor m_collectionTaskProcessor = new CollectionTaskLogicProcessor();

	private CollectionTaskProcessor m_collectionTaskHandler;

	protected HorseLogicProcessor m_horseProcessor = new HorseLogicProcessor();

	private HorseProcessor m_horseHandler;

	protected RingStationLogicProcessor m_ringStationProcessor = new RingStationLogicProcessor();

	private RingStationProcessor m_ringStationHandler;

	protected FarmLogicProcessor m_farmProcessor = new FarmLogicProcessor();

	private FarmProcessor m_farmHandler;

	protected PetLogicProcessor m_petProcessor = new PetLogicProcessor();

	private PetProcessor m_petHandler;

	protected BombKingLogicProcessor m_bombKingProcessor = new BombKingLogicProcessor();

	private BombKingProcessor m_bombKingHandler;

	protected MagicHouseLogicProcessor m_magicHouseProcessor = new MagicHouseLogicProcessor();

	private MagicHouseProcessor m_magicHouseHandler;

	protected DragonBoatLogicProcessor m_dragonBoatProcessor = new DragonBoatLogicProcessor();

	private DragonBoatProcessor m_dragonBoatHandler;

	protected ActiveSystemLogicProcessor m_activeSystemProcessor = new ActiveSystemLogicProcessor();

	private ActiveSystemProcessor m_activeSystemHandler;

	protected WorshipTheMoonLogicProcessor m_worshipTheMoonProcessor = new WorshipTheMoonLogicProcessor();

	private WorshipTheMoonProcessor m_worshipTheMoonHandler;

	protected MagpieBridgeLogicProcessor m_magpieBridgeProcessor = new MagpieBridgeLogicProcessor();

	private MagpieBridgeProcessor m_magpieBridgeHandler;

	protected GypsyShopLogicProcessor m_gypsyShopProcessor = new GypsyShopLogicProcessor();

	private GypsyShopProcessor m_gypsyShopHandler;

	protected HomeSystemLogicProcessor m_homeSystemProcessor = new HomeSystemLogicProcessor();

	private HomeSystemProcessor m_homeSystemHandler;

	protected HomeBarLogicProcessor m_HomeBarProcessor = new HomeBarLogicProcessor();

	private HomeBarProcessor m_HomeBarHandler;

	protected HomeAdornLogicProcessor m_HomeAdornProcessor = new HomeAdornLogicProcessor();

	private HomeAdornProcessor m_HomeAdornHandler;

	protected HomeFightLogicProcessor m_HomeFightProcessor = new HomeFightLogicProcessor();

	private HomeFightProcessor m_HomeFightHandler;

	protected HotSpringRoomsLogicProcessor m_HotSpringRoomsProcessor = new HotSpringRoomsLogicProcessor();

	private HotSpringRoomsProcessor m_HotSpringRoomsHandler;

	protected AnotherDimensionLogicProcessor m_AnotherDimensionProcessor = new AnotherDimensionLogicProcessor();

	private AnotherDimensionProcessor m_AnotherDimensionHandler;

	protected ZodiacLogicProcessor m_ZodiacProcessor = new ZodiacLogicProcessor();

	private ZodiacProcessor m_ZodiacHandler;

	protected RenshenLogicProcessor m_RenshenProcessor = new RenshenLogicProcessor();

	private RenshenProcessor m_RenshenHandler;

	protected MoneyTreeLogicProcessor m_MoneyTreeProcessor = new MoneyTreeLogicProcessor();

	private MoneyTreeProcessor m_MoneyTreeHandler;

	protected ConsortiaLogicProcessor m_consortiaProcessor = new ConsortiaLogicProcessor();

	private ConsortiaProcessor m_consortia;

	protected HomeTempleLogicProcessor m_HomeTempleProcessor = new HomeTempleLogicProcessor();

	private HomeTempleProcessor m_HomeTemple;

	protected GuardCoreLogicProcessor m_GuardCoreProcessor = new GuardCoreLogicProcessor();

	private GuardCoreProcessor m_GuardCore;

	protected SanXiaoLogicProcessor m_SanXiaoProcessor = new SanXiaoLogicProcessor();

	private SanXiaoProcessor m_SanXiao;

	protected GodCardRaiseLogicProcessor m_GodCardRaiseProcessor = new GodCardRaiseLogicProcessor();

	private GodCardRaiseProcessor m_GodCardRaise;

	protected MemoryGameLogicProcessor m_MemoryGameProcessor = new MemoryGameLogicProcessor();

	private MemoryGameProcessor m_MemoryGame;

	protected HorseAmuletLogicProcessor m_HorseAmuletProcessor = new HorseAmuletLogicProcessor();

	private HorseAmuletProcessor m_HorseAmulet;

	protected EquipAmuletLogicProcessor m_EquipAmuletProcessor = new EquipAmuletLogicProcessor();

	private EquipAmuletProcessor m_EquipAmulet;

	protected QiYuanLogicProcessor m_QiYuanProcessor = new QiYuanLogicProcessor();

	private QiYuanProcessor m_QiYuan;

	protected CardNewSystemLogicProcessor m_cardNewSystemLogicProcessor = new CardNewSystemLogicProcessor();

	protected AcademyLogicProcessor m_AcademyLogicProcessor = new AcademyLogicProcessor();

	protected LabyrinthLogicProcessor m_LabyrinthHandlerLogicProcessor = new LabyrinthLogicProcessor();

	public List<ItemInfo> LotteryAwardList;

	protected ElfLogicProcessor m_elfLogicProcessor = new ElfLogicProcessor();

	protected TeamLogicProcessor m_TeamLogicProcessor = new TeamLogicProcessor();

	protected DevilTurnLogicProcessor m_DevilTurnLogicProcessor = new DevilTurnLogicProcessor();

	protected EmblemLogicProcessor m_EmblemLogicProcessor = new EmblemLogicProcessor();

	protected ConsortiaTaskLogicProcessor m_consortiaTaskLogicProcessor = new ConsortiaTaskLogicProcessor();

	public bool DebugMode => Client.Server.Configuration.DebugMode;

	public int ZoneId => GameServer.Instance.Configuration.AreaId;

	public string ZoneName
	{
		get
		{
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(ZoneId);
			return (areaConfigInfo == null) ? GameServer.Instance.Configuration.AreaName : areaConfigInfo.AreaName;
		}
	}

	public int battleId { get; set; }

	public int battleStage { get; set; }

	public int BombKingDelay { get; set; }

	public int TeamCampBattle { get; set; }

	public int GuildBattleEnemyId { get; set; }

	public bool IsLoginDevice { get; set; }

	public ePlayerState PlayerState
	{
		get
		{
			return m_playerState;
		}
		set
		{
			m_playerState = value;
		}
	}

	public virtual bool IsRobot => false;

	public BaseGame game
	{
		get
		{
			return m_game;
		}
		set
		{
			m_game = value;
		}
	}

	public int Immunity
	{
		get
		{
			return m_immunity;
		}
		set
		{
			m_immunity = value;
		}
	}

	public int PlayerId => m_playerId;

	public string Account => m_account;

	public PlayerInfo PlayerCharacter => m_character;

	public UserMatchInfo MatchInfo => m_battle.MatchInfo;

	public UserPvePowerBuffInfo PvePowerBuff
	{
		get
		{
			return m_extra.PvePowerBuff;
		}
		set
		{
			m_extra.PvePowerBuff = value;
		}
	}

	public GameClient Client => m_client;

	public Player Players => m_players;

	public virtual bool IsActive => m_client.IsConnected;

	public virtual IPacketLib Out => m_client.Out;

	public List<int> QuestColect
	{
		get
		{
			return m_questColect;
		}
		set
		{
			m_questColect = value;
		}
	}

	public bool IsMinor
	{
		get
		{
			return m_isMinor;
		}
		set
		{
			m_isMinor = value;
		}
	}

	public bool IsAASInfo
	{
		get
		{
			return m_isAASInfo;
		}
		set
		{
			m_isAASInfo = value;
		}
	}

	public long PingTime
	{
		get
		{
			return m_pingTime;
		}
		set
		{
			m_pingTime = value;
			GSPacketIn pkg = Out.SendNetWork(this, m_pingTime);
			if (_currentRoom != null)
			{
				_currentRoom.SendToAll(pkg, this);
			}
		}
	}

	public int LovelSKillID
	{
		get
		{
			return m_loveSkillId;
		}
		set
		{
			m_loveSkillId = value;
		}
	}

	public List<BufferInfo> FightBuffs
	{
		get
		{
			return m_fightBuffInfo;
		}
		set
		{
			m_fightBuffInfo = value;
		}
	}

	public UsersPetInfo Pet => m_pet;

	public int[] HorseSkillEquip => m_horse.GetHorseSkillEquip;

	public List<Sys_Users_Emblem> EmblemInfos { get; set; }

	public List<Sys_Users_Scroll> ScrollInfos { get; set; }

	public int[] PetSealSkillEquip
	{
		get
		{
			List<int> list = new List<int>();
			foreach (UsersPetLacrarInfo item in PetBag.lacresPet)
			{
				if (!item.isSeal)
				{
					continue;
				}
				foreach (StoreFineSealLockInfo item2 in item.lockArr)
				{
					if (item2.skillId > 0)
					{
						list.Add(item2.skillId);
					}
				}
			}
			return list.ToArray();
		}
	}

	public PlayerTrailElite TrailElite => m_trailElite;

	public PlayerKingGrade KingGrade => m_kingGrade;

	public PlayerRoulette Roulette => m_roulette;

	public PlayerHomeSysterm Home => m_home;

	public PlayerHorse Horse => m_horse;

	public PlayerMagicStoneInventory MagicStoneBag => m_magicStone;

	public PlayerHorseAmuletInventory HorseAmuletBag => m_horseAmulet;

	public PlayerExtra Extra => m_extra;

	public PlayerBattle BattleData => m_battle;

	public PlayerActives Actives => m_actives;

	public PlayerDice Dice => m_dice;

	public PlayerProperty PlayerProp => m_playerProp;

	public PlayerRank Rank => m_rank;

	public PlayerBank Bank => m_bank;

	public PlayerMark Marks => m_marks;

	public PlayerMines Mines => m_mines;

	public PlayerTreasure Treasure => m_treasure;

	public PetInventory PetBag => m_petBag;

	public PlayerFarm Farm => m_farm;

	public PlayerEquipInventory EquipBag => m_equipBag;

	public PlayerAvataInventory AvatarBag => m_avatarBag;

	public PlayerInventory PropBag => m_propBag;

	public PlayerInventory MinesBag => m_minesBag;

	public PlayerRoomBordenInventory RoomBordenBag => m_RoomBordenBag;

	public PlayerInventory FightBag => m_fightBag;

	public PlayerInventory TempBag => m_tempBag;

	public PlayerInventory ConsortiaBag => m_consortiaBag;

	public PlayerMagicHouse MagicHouse => m_magicHouse;

	public PlayerInventory StoreBag => m_storeBag;

	public PlayerInventory CaddyBag => m_caddyBag;

	public PlayerInventory FarmBag => m_farmBag;

	public PlayerInventory Vegetable => m_vegetable;

	public PlayerInventory Food => m_food;

	public PlayerInventory PetEgg => m_petEgg;

	public PlayerBeadInventory BeadBag => m_beadBag;

	public CardInventory CardBag => m_cardBag;

	public AchievementInventory AchievementInventory => m_achievementInventory;

	public CardAchievementInventory CardAchievementInventory => m_cardAchievementInventory;

	public QuestInventory QuestInventory => m_questInventory;

	public BufferList BufferList => m_bufferList;

	public UserLabyrinthInfo Labyrinth
	{
		get
		{
			return m_Labyrinth;
		}
		set
		{
			m_Labyrinth = value;
		}
	}

	public List<UserGemStone> GemStone
	{
		get
		{
			return m_GemStone;
		}
		set
		{
			m_GemStone = value;
		}
	}

	public List<ItemInfo> EquipEffect
	{
		get
		{
			return m_equipEffect;
		}
		set
		{
			m_equipEffect = value;
		}
	}

	public List<ItemInfo> HorseAmuletEquipEffect
	{
		get
		{
			return m_horseAmuletEffect;
		}
		set
		{
			m_horseAmuletEffect = value;
		}
	}

	public bool CanUseProp { get; set; }

	public bool IsCrossZone { get; set; }

	public int CaddyType { get; set; }

	public int Level
	{
		get
		{
			return m_character.Grade;
		}
		set
		{
			if (value != m_character.Grade)
			{
				m_character.Grade = value;
				OnLevelUp(value);
				OnPropertiesChanged();
				LoadPvePermission();
			}
		}
	}

	public double BaseAttack
	{
		get
		{
			return m_basedame;
		}
		set
		{
			m_basedame = value;
			PlayerCharacter.BaseAttack = (int)value;
		}
	}

	public double BaseDefence
	{
		get
		{
			return m_baseGuard;
		}
		set
		{
			m_baseGuard = value;
			PlayerCharacter.BaseDefence = (int)value;
		}
	}

	public int LevelPlusBlood => LevelMgr.LevelPlusBlood(m_character.Grade);

	public ItemInfo MainWeapon => m_MainWeapon;

	public BatleConfigInfo BatleConfig => m_batleConfig;

	public ItemInfo Healstone
	{
		get
		{
			if (m_healstone == null)
			{
				return null;
			}
			return m_healstone;
		}
	}

	public ItemInfo Weapon3D
	{
		get
		{
			if (m_currentWeapon3D == null)
			{
				return null;
			}
			return m_currentWeapon3D;
		}
	}

	public ItemInfo SecondWeapon
	{
		get
		{
			if (m_currentSecondWeapon == null)
			{
				return null;
			}
			return m_currentSecondWeapon;
		}
	}

	public ItemInfo Amulet
	{
		get
		{
			if (m_currentAmulet == null)
			{
				return null;
			}
			if (m_currentAmulet.Template.CategoryID != 70)
			{
				return null;
			}
			return m_currentAmulet;
		}
	}

	public UserWonderFulActivityManager UserWonderFulActivityManager { get; set; }

	public string ProcessLabyrinthAward { get; set; }

	public List<int> ViFarms => _viFarms;

	public Dictionary<int, int> Friends => _friends;

	public BaseRoom CurrentRoom
	{
		get
		{
			return _currentRoom;
		}
		set
		{
			BaseRoom baseRoom = Interlocked.Exchange(ref _currentRoom, value);
			if (baseRoom != null)
			{
				RoomMgr.ExitRoom(baseRoom, this);
			}
		}
	}

	public HotSpringRoom CurrentHotSpringRoom
	{
		get
		{
			return m_currentHotSpringRoom;
		}
		set
		{
			m_currentHotSpringRoom = value;
		}
	}

	public BaseHorseRaceRoom CurrentHorseRaceRoom
	{
		get
		{
			return m_currentHorseRaceRoom;
		}
		set
		{
			BaseHorseRaceRoom baseHorseRaceRoom = Interlocked.Exchange(ref m_currentHorseRaceRoom, value);
			if (baseHorseRaceRoom != null)
			{
				RoomMgr.ExitHorseRaceRoom(baseHorseRaceRoom, this);
			}
		}
	}

	public int GamePlayerId
	{
		get
		{
			return m_gameId;
		}
		set
		{
			m_gameId = value;
		}
	}

	public int TempGameId { get; set; }

	public long WorldbossBood { get; set; }

	public long AllWorldDameBoss { get; set; }

	public bool HideAllFriend
	{
		get
		{
			return m_hideAllFriend;
		}
		set
		{
			m_hideAllFriend = value;
		}
	}

	public BaseSevenDoubleRoom CurrentSevenDoubleRoom
	{
		get
		{
			return m_currentSevenDoubleRoom;
		}
		set
		{
			m_currentSevenDoubleRoom = value;
		}
	}

	public MarryRoom CurrentMarryRoom
	{
		get
		{
			return m_currentMarryRoom;
		}
		set
		{
			m_currentMarryRoom = value;
		}
	}

	public bool IsInMarryRoom => m_currentMarryRoom != null;

	public int ServerID { get; set; }

	public Dictionary<string, object> TempProperties => m_tempProperties;

	public HappyRechargeInfo HappyRechargeData { get; set; }

	public GameRoomProcessor GameRoom => m_gameRoom;

	public AvatarCollectionProcessor AvatarCollection => m_avatarCollection;

	public MagicStoneProcessor MagicStoneHandler => m_magicStoneHandler;

	public NewHallProcessor NewHallHandler => m_newHallHandler;

	public CollectionTaskProcessor CollectionTaskHandler => m_collectionTaskHandler;

	public HorseProcessor HorseHandler => m_horseHandler;

	public RingStationProcessor RingStationHandler => m_ringStationHandler;

	public FarmProcessor FarmHandler => m_farmHandler;

	public PetProcessor PetHandler => m_petHandler;

	public BombKingProcessor BombKingHandler => m_bombKingHandler;

	public MagicHouseProcessor MagicHouseHandler => m_magicHouseHandler;

	public DragonBoatProcessor DragonBoatHandler => m_dragonBoatHandler;

	public ActiveSystemProcessor ActiveSystemHandler => m_activeSystemHandler;

	public WorshipTheMoonProcessor WorshipTheMoonHandler => m_worshipTheMoonHandler;

	public MagpieBridgeProcessor MagpieBridgeHandler => m_magpieBridgeHandler;

	public GypsyShopProcessor GypsyShopHandler => m_gypsyShopHandler;

	public HomeSystemProcessor HomeSystemHandler => m_homeSystemHandler;

	public HomeBarProcessor HomeBarHandler => m_HomeBarHandler;

	public HomeAdornProcessor HomeAdornHandler => m_HomeAdornHandler;

	public HomeFightProcessor HomeFightHandler => m_HomeFightHandler;

	public HotSpringRoomsProcessor HotSpringRoomsHandler => m_HotSpringRoomsHandler;

	public AnotherDimensionProcessor AnotherDimensionHandler => m_AnotherDimensionHandler;

	public ZodiacProcessor ZodiacHandler => m_ZodiacHandler;

	public RenshenProcessor RenshenHandler => m_RenshenHandler;

	public MoneyTreeProcessor MoneyTreeHandler => m_MoneyTreeHandler;

	public ConsortiaProcessor Consortia => m_consortia;

	public HomeTempleProcessor HomeTemple => m_HomeTemple;

	public GuardCoreProcessor GuardCore => m_GuardCore;

	public SanXiaoProcessor SanXiao => m_SanXiao;

	public GodCardRaiseProcessor GodCardRaise => m_GodCardRaise;

	public MemoryGameProcessor MemoryGame => m_MemoryGame;

	public HorseAmuletProcessor HorseAmulet => m_HorseAmulet;

	public EquipAmuletProcessor EquipAmulet => m_EquipAmulet;

	public QiYuanProcessor DDTQiYuan => m_QiYuan;

	public CardNewSystemProcessor CardNewSystemHandler { get; private set; }

	public AcademyProcessor AcademyHandler { get; private set; }

	public LabyrinthProcessor LabyrinthHandler { get; private set; }

	public int DamagePlus { get; internal set; }

	public int GuardPlus { get; internal set; }

	public int Lottery { get; internal set; }

	public List<ItemBoxInfo> LotteryItems { get; internal set; }

	public int LotteryID { get; internal set; }

	public DateTime LastItemAdvTime { get; internal set; }

	public DateTime LastMoveGoodsAllTime { get; internal set; }

	public DateTime LastItemComposeTime { get; internal set; }

	public object LastRequestTime { get; internal set; }

	public ElfProcessor ElfHandler { get; private set; }

	public TeamProcessor TeamHandler { get; private set; }

	public DevilTurnProcessor DevilTurnHandler { get; private set; }

	public EmblemProcessor EmblemHandler { get; private set; }

	public ConsortiaTaskProcessor ConsortiaTaskHandler { get; private set; }

	public event PlayerEventHandle UseBuffer;

	public event PlayerEventHandle LevelUp;

	public event PlayerItemPropertyEventHandle AfterUsingItem;

	public event PlayerGameOverEventHandle GameOver;

	public event PlayerMissionOverEventHandle MissionOver;

	public event PlayerMissionFullOverEventHandle MissionFullOver;

	public event PlayerMissionTurnOverEventHandle MissionTurnOver;

	public event PlayerQuestOneKeyFinishEventHandle QuestOneKeyFinish;

	public event PlayerItemStrengthenEventHandle ItemStrengthen;

	public event PlayerGodTempleEventHandle GodTempleImmolation;

	public event PlayerBuildGpgradeEventHandle BuildGpgrade;

	public event PlayerMountMasterEventHandle MountMaster;

	public event PlayerShopEventHandle Paid;

	public event PlayerAdoptPetEventHandle AdoptPetEvent;

	public event PlayerNewGearEventHandle NewGearEvent;

	public event PlayerCropPrimaryEventHandle CropPrimaryEvent;

	public event PlayerSeedFoodPetEventHandle SeedFoodPetEvent;

	public event PlayerUserToemGemstoneEventHandle UserToemGemstonetEvent;

	public event PlayerCollectionTaskEventHandle CollectionTaskEvent;

	public event PlayerUpLevelPetEventHandle UpLevelPetEvent;

	public event PlayerItemInsertEventHandle ItemInsert;

	public event PlayerItemFusionEventHandle ItemFusion;

	public event PlayerItemMeltEventHandle ItemMelt;

	public event PlayerGameKillEventHandel AfterKillingLiving;

	public event PlayerOwnConsortiaEventHandle GuildChanged;

	public event PlayerItemComposeEventHandle ItemCompose;

	public event PlayerAchievementFinish AchievementFinishEvent;

	public event PlayerFightAddOffer FightAddOfferEvent;

	public event PlayerFightOneBloodIsWin FightOneBloodIsWin;

	public event PlayerVIPUpgrade VIPUpgrade;

	public event PlayerHotSpingExpAdd HotSpingExpAdd;

	public event PlayerGoldCollection GoldCollect;

	public event PlayerAcademyEventHandle AcademyEvent;

	public event PlayerGiftTokenCollection GiftTokenCollect;

	public event PlayerEventHandle PingTimeOnline;

	public event PlayerPropertisChange PropertisChange;

	public event PlayerQuestFinish QuestFinishEvent;

	public event PlayerUseBugle UseBugle;

	public event GameKillDropEventHandel GameKillDrop;

	public event PlayerFundHandler AddFund;

	public event DonateRiches Riches;

	public GamePlayer(int playerId, string account, GameClient client, PlayerInfo info)
	{
		m_playerId = playerId;
		m_account = account;
		m_client = client;
		m_character = info;
		m_equipBag = new PlayerEquipInventory(this);
		m_beadBag = new PlayerBeadInventory(this);
		m_avatarBag = new PlayerAvataInventory(this);
		m_magicStone = new PlayerMagicStoneInventory(this);
		m_horseAmulet = new PlayerHorseAmuletInventory(this);
		m_propBag = new PlayerInventory(this, saveTodb: true, 49, 1, 0, autoStack: true);
		m_minesBag = new PlayerInventory(this, saveTodb: true, 16, 52, 1, autoStack: true);
		m_RoomBordenBag = new PlayerRoomBordenInventory(this);
		m_consortiaBag = new PlayerInventory(this, saveTodb: true, 100, 11, 0, autoStack: true);
		m_magicHouse = new PlayerMagicHouse(this);
		m_storeBag = new PlayerInventory(this, saveTodb: true, 20, 12, 0, autoStack: true);
		m_fightBag = new PlayerInventory(this, saveTodb: false, 3, 3, 0, autoStack: false);
		m_tempBag = new PlayerInventory(this, saveTodb: false, 100, 4, 0, autoStack: true);
		m_caddyBag = new PlayerInventory(this, saveTodb: false, 30, 5, 0, autoStack: true);
		m_farmBag = new PlayerInventory(this, saveTodb: true, 30, 13, 0, autoStack: true);
		m_vegetable = new PlayerInventory(this, saveTodb: true, 30, 14, 0, autoStack: true);
		m_food = new PlayerInventory(this, saveTodb: true, 30, 34, 0, autoStack: true);
		m_petEgg = new PlayerInventory(this, saveTodb: true, 30, 35, 0, autoStack: true);
		m_cardBag = new CardInventory(this, saveTodb: true, 100, 0);
		m_farm = new PlayerFarm(this, saveTodb: true, 30, 0);
		m_petBag = new PetInventory(this, saveTodb: true, 20, 8, 0);
		m_treasure = new PlayerTreasure(this, saveTodb: true);
		m_rank = new PlayerRank(this, saveTodb: true);
		m_bank = new PlayerBank(this);
		m_mines = new PlayerMines(this, saveTodb: true);
		m_marks = new PlayerMark(this, saveTodb: true);
		m_playerProp = new PlayerProperty(this);
		m_dice = new PlayerDice(this, saveTodb: true);
		m_battle = new PlayerBattle(this, saveTodb: true);
		m_actives = new PlayerActives(this, saveTodb: true);
		m_extra = new PlayerExtra(this, saveTodb: true);
		m_horse = new PlayerHorse(this, saveTodb: true);
		m_home = new PlayerHomeSysterm(this, saveTodb: true);
		m_roulette = new PlayerRoulette(this, saveTodb: true);
		m_kingGrade = new PlayerKingGrade(this, saveTodb: true);
		m_trailElite = new PlayerTrailElite(this, saveTodb: true);
		m_batleConfig = new BatleConfigInfo();
		m_questInventory = new QuestInventory(this);
		m_achievementInventory = new AchievementInventory(this);
		m_cardAchievementInventory = new CardAchievementInventory(this);
		m_bufferList = new BufferList(this);
		m_fightBuffInfo = new List<BufferInfo>();
		m_equipEffect = new List<ItemInfo>();
		m_GemStone = new List<UserGemStone>();
		m_horseAmuletEffect = new List<ItemInfo>();
		m_questColect = new List<int>();
		m_equipGhostList = new Dictionary<string, UserEquipGhostInfo>();
		m_Labyrinth = null;
		ResetLottery();
		GPAddPlus = 1.0;
		X = 646;
		Y = 1241;
		PosX = 1560;
		PosY = 500;
		MarryMap = 0;
		LastChatTime = DateTime.Today;
		LastFigUpTime = DateTime.Today;
		LastDrillUpTime = DateTime.Today;
		LastOpenPack = DateTime.Today;
		LastOpenGrowthPackage = DateTime.Now;
		LastOpenChristmasPackage = DateTime.Now;
		WaitingProcessor = DateTime.Now;
		battleId = 0;
		battleStage = 0;
		BombKingDelay = 0;
		TeamCampBattle = 0;
		GuildBattleEnemyId = 0;
		LastTakeCardItem = null;
		m_hideAllFriend = GameProperties.IsOpenOrCloseEyes;
	}

	public int RefreshLeagueGetReward(int awardGot, int Score)
	{
		if (awardGot <= 0)
		{
			return 0;
		}
		MatchInfo.leagueItemsGet = awardGot;
		MatchInfo.weeklyScore -= Score;
		OnPropertiesChanged();
		return awardGot;
	}

	public void AddLog(string type, string content)
	{
	}

	public void RemoveLotteryItems(int templateId, int count)
	{
		foreach (ItemBoxInfo lotteryItem in LotteryItems)
		{
			if (lotteryItem.TemplateId == templateId && lotteryItem.ItemCount == count)
			{
				LotteryItems.Remove(lotteryItem);
				break;
			}
		}
	}

	public void ResetLottery()
	{
		Lottery = -1;
		LotteryID = 0;
		LotteryItems = new List<ItemBoxInfo>();
		LotteryAwardList = new List<ItemInfo>();
	}

	internal void AddBadLuckCaddy(int v)
	{
		throw new NotImplementedException();
	}

	internal bool UsePayBuff(object caddy_Good)
	{
		throw new NotImplementedException();
	}

	public PlayerInventory GetInventory(eBageType bageType)
	{
		return bageType switch
		{
			eBageType.EquipBag => m_equipBag, 
			eBageType.PropBag => m_propBag, 
			eBageType.FightBag => m_fightBag, 
			eBageType.TempBag => m_tempBag, 
			eBageType.Consortia => m_consortiaBag, 
			eBageType.Store => m_storeBag, 
			eBageType.CaddyBag => m_caddyBag, 
			eBageType.FarmBag => m_farmBag, 
			eBageType.Vegetable => m_vegetable, 
			eBageType.Food => m_food, 
			eBageType.PetEgg => m_petEgg, 
			eBageType.BeadBag => m_beadBag, 
			eBageType.MagicStoneBag => m_magicStone, 
			eBageType.MagicHouseBag => m_magicHouse, 
			eBageType.HorseAmuletBag => m_horseAmulet, 
			eBageType.MineBag => m_minesBag, 
			eBageType.RoomBordenBag => m_RoomBordenBag, 
			eBageType.MarkBag => m_marks, 
			_ => throw new NotSupportedException($"Did not support this type bag: {bageType} PlayerID: {PlayerCharacter.ID} Nickname: {PlayerCharacter.NickName}"), 
		};
	}

	public string GetInventoryName(eBageType bageType)
	{
		return bageType switch
		{
			eBageType.EquipBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.Equip"), 
			eBageType.PropBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.Prop"), 
			eBageType.BeadBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.BeadBag"), 
			eBageType.FightBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.FightBag"), 
			eBageType.FarmBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.FarmBag"), 
			_ => bageType.ToString(), 
		};
	}

	public PlayerInventory GetItemInventory(ItemTemplateInfo template)
	{
		return GetInventory(template.BagType);
	}

	public ItemInfo GetItemAt(eBageType bagType, int place)
	{
		PlayerInventory inventory = GetInventory(bagType);
		return inventory.GetItemAt(place);
	}

	public ItemInfo GetItemByTemplateID(int templateId)
	{
		ItemInfo itemByTemplateID = EquipBag.GetItemByTemplateID(EquipBag.BeginSlot, templateId);
		if (itemByTemplateID != null)
		{
			return itemByTemplateID;
		}
		itemByTemplateID = PropBag.GetItemByTemplateID(0, templateId);
		if (itemByTemplateID != null)
		{
			return itemByTemplateID;
		}
		return ConsortiaBag.GetItemByTemplateID(0, templateId);
	}

	public int GetHalloweenCardCount()
	{
		if (m_actives == null)
		{
			return 0;
		}
		if (!Actives.IsHalloweenOpen())
		{
			return 0;
		}
		return GetAllItemCount(Actives.halloweenCard);
	}

	public int GetAllItemCount(int templateId)
	{
		int num = m_propBag.GetItemCount(templateId) + m_equipBag.GetItemCount(templateId) + m_consortiaBag.GetItemCount(templateId);
		if (templateId == Actives.halloweenCard)
		{
			GameServer.Instance.LoginServer.SendHalloweenRank(PlayerId, m_character.NickName, num, m_character.typeVIP);
		}
		return num;
	}

	public int GetItemCount(int templateId)
	{
		return m_propBag.GetItemCount(templateId) + m_equipBag.GetItemCount(templateId);
	}

	public bool AddItem(ItemInfo item)
	{
		if (item.Template.CategoryID == 86)
		{
			return ElfMgr.AddSkillBook(this, item);
		}
		if (item.Template.BagType == eBageType.EquipBag)
		{
			return m_equipBag.AddItem(item);
		}
		AbstractInventory itemInventory = GetItemInventory(item.Template);
		return itemInventory.AddItem(item, itemInventory.BeginSlot);
	}

	public bool StackItemToAnother(ItemInfo item)
	{
		if (item.Template.CategoryID == 86)
		{
			return ElfMgr.StackSkillBook(this, item);
		}
		if (item.Template.BagType == eBageType.EquipBag)
		{
			return m_equipBag.StackItemToAnother(item);
		}
		AbstractInventory itemInventory = GetItemInventory(item.Template);
		return itemInventory.StackItemToAnother(item);
	}

	public void UpdateItem(ItemInfo item)
	{
		if (item.BagType == 0)
		{
			m_equipBag.UpdateItem(item);
		}
		if (item.BagType == 1)
		{
			m_propBag.UpdateItem(item);
		}
		if (item.BagType == 12)
		{
			m_storeBag.UpdateItem(item);
		}
	}

	public bool RemoveItem(ItemInfo item)
	{
		if (item.BagType == m_farmBag.BagType)
		{
			return m_farmBag.RemoveItem(item);
		}
		if (item.BagType == m_propBag.BagType)
		{
			return m_propBag.RemoveItem(item);
		}
		if (item.BagType == m_beadBag.BagType)
		{
			return m_beadBag.RemoveItem(item);
		}
		if (item.BagType == m_fightBag.BagType)
		{
			return m_fightBag.RemoveItem(item);
		}
		return m_equipBag.RemoveItem(item);
	}

	public void ClearFightBuffOneMatch()
	{
		List<BufferInfo> list = new List<BufferInfo>();
		foreach (BufferInfo fightBuff in FightBuffs)
		{
			if (fightBuff != null)
			{
				int type = fightBuff.Type;
				int num = type;
				if ((uint)(num - 400) <= 6u)
				{
					list.Add(fightBuff);
				}
			}
		}
		foreach (BufferInfo item in list)
		{
			FightBuffs.Remove(item);
		}
		list.Clear();
	}

	public void UpdateFightBuff(BufferInfo info)
	{
		int num = -1;
		for (int i = 0; i < FightBuffs.Count; i++)
		{
			if (info != null && info.Type == FightBuffs[i].Type)
			{
				FightBuffs[i] = info;
				num = info.Type;
			}
		}
		if (num == -1)
		{
			FightBuffs.Add(info);
		}
	}

	public void UpdateCampBattleResult(eGameType gameType, int bossId, int totalPlayersTeam, bool isWin)
	{
	}

	public void UpdatePveResult(string type, int value, bool option)
	{
		int num = 0;
		int num2 = 0;
		string text = "";
		switch (type)
		{
		case "worldboss":
			num = value / 40000;
			num2 = value / 100;
			text = LanguageMgr.GetTranslation("GamePlayer.Msg20", num, num2);
			AddDamageScores(num);
			RoomMgr.WorldBossRoom.UpdateRank(num, num2, PlayerCharacter.NickName);
			RoomMgr.WorldBossRoom.ReduceBlood(value);
			if (option)
			{
				RoomMgr.WorldBossRoom.FightOver();
			}
			break;
		case "consortiaboss":
			num = value / 800;
			num2 = value / 1200;
			text = LanguageMgr.GetTranslation("GamePlayer.Msg21", num, num2);
			AddRichesOffer(num);
			ConsortiaBossMgr.UpdateBlood(PlayerCharacter.ConsortiaID, value);
			ConsortiaBossMgr.UpdateRank(PlayerCharacter.ConsortiaID, value, num, num2, PlayerCharacter.NickName, PlayerCharacter.ID);
			break;
		case "yearmonter":
			Actives.Info.DamageNum = value;
			Actives.CreateYearMonterBoxState();
			break;
		case "qx":
			if (!option)
			{
				Actives.QXDropId = value;
				Actives.SendQXAward();
			}
			break;
		case "cryotboss":
			if (option)
			{
				Actives.UpdateStar(value);
			}
			break;
		}
		AddHonor(num2);
		if (!string.IsNullOrEmpty(text))
		{
			SendMessage(text);
		}
	}

	public void SendItemNotice(ItemInfo info, int battleType, string Name)
	{
		if (info == null)
		{
			return;
		}
		int num = 0;
		switch (battleType)
		{
		case 0:
		case 1:
			num = 2;
			break;
		case 2:
		case 3:
		case 4:
			num = 1;
			break;
		default:
			num = 3;
			break;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(14);
		gSPacketIn.WriteString(PlayerCharacter.NickName);
		gSPacketIn.WriteInt(battleType);
		gSPacketIn.WriteInt(info.TemplateID);
		gSPacketIn.WriteBoolean(info.IsBinds);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteInt(info.Count);
		if (num == 3)
		{
			gSPacketIn.WriteString(Name);
		}
		if (info.IsTips)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.Out.SendTCP(gSPacketIn);
			}
		}
	}

	public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView, string Name)
	{
		if (cloneItem != null)
		{
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			if (ShopMgr.FindSpecialItemInfo(cloneItem, ref specialValue))
			{
				DirectAddValue(specialValue);
				return true;
			}
			if (cloneItem.Template.CategoryID == 37)
			{
				Home.BuyAndActives(cloneItem.Template);
			}
			else
			{
				List<ItemInfo> list = new List<ItemInfo>();
				cloneItem.Count = count;
				if (!StackItemToAnother(cloneItem) && !AddItem(cloneItem))
				{
					list.Add(cloneItem);
				}
				BagFullSendToMail(list);
				if (Name != "no")
				{
					SendItemNotice(cloneItem, (int)gameView, Name);
					SendMessage(LanguageMgr.GetTranslation("AddTemplate.Notice", (cloneItem.Template == null) ? "null" : cloneItem.Template.Name, cloneItem.Count));
				}
			}
			return true;
		}
		return false;
	}

	public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView)
	{
		if (eBageType.FightBag == bagType)
		{
			return FightBag.AddItem(cloneItem);
		}
		if (eGameView.dungeonTypeGet == gameView || gameView == eGameView.BatleTypeGet)
		{
			LastTakeCardItem = cloneItem;
			return AddTemplate(cloneItem, bagType, count, gameView, "pve");
		}
		return AddTemplate(cloneItem, bagType, count, gameView, "no");
	}

	public bool AddTemplate(ItemInfo cloneItem)
	{
		return AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eGameView.OtherTypeGet);
	}

	public bool AddTemplate(ItemInfo cloneItem, string name)
	{
		return AddTemplate(cloneItem, (eBageType)cloneItem.BagType, cloneItem.Count, eGameView.OtherTypeGet, name);
	}

	public bool AddTemplate(List<ItemInfo> infos, int count)
	{
		if (infos != null)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (ItemInfo info in infos)
			{
				info.IsBinds = true;
				info.Count = count;
				if (!StackItemToAnother(info) && !AddItem(info))
				{
					list.Add(info);
				}
			}
			BagFullSendToMail(list);
			return true;
		}
		return false;
	}

	public bool AddTemplate(List<ItemInfo> infos)
	{
		if (infos != null && infos.Count > 0)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (ItemInfo info in infos)
			{
				info.IsBinds = true;
				if (!StackItemToAnother(info) && !AddItem(info))
				{
					list.Add(info);
				}
			}
			BagFullSendToMail(list);
			return true;
		}
		return false;
	}

	public void BagFullSendToMail(List<ItemInfo> infos)
	{
		if (infos.Count > 0)
		{
			bool flag = false;
			using (new PlayerBussiness())
			{
				flag = SendItemsToMail(infos, "", LanguageMgr.GetTranslation("GamePlayer.Msg19"), eMailType.BuyItem);
			}
			if (flag)
			{
				Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
	}

	public bool FindEmptySlot(eBageType bagType)
	{
		PlayerInventory inventory = GetInventory(bagType);
		inventory.FindFirstEmptySlot();
		return inventory.FindFirstEmptySlot() > 0;
	}

	public bool RemoveTemplate(int templateId, int count)
	{
		int itemCount = m_equipBag.GetItemCount(0, templateId);
		int itemCount2 = m_propBag.GetItemCount(templateId);
		int itemCount3 = m_consortiaBag.GetItemCount(templateId);
		int itemCount4 = m_magicHouse.GetItemCount(templateId);
		int num = itemCount + itemCount2 + itemCount3;
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null && num >= count)
		{
			if (itemCount > 0 && count > 0 && RemoveTempate(eBageType.EquipBag, itemTemplateInfo, (itemCount > count) ? count : itemCount))
			{
				count = ((count >= itemCount) ? (count - itemCount) : 0);
			}
			if (itemCount2 > 0 && count > 0 && RemoveTempate(eBageType.PropBag, itemTemplateInfo, (itemCount2 > count) ? count : itemCount2))
			{
				count = ((count >= itemCount2) ? (count - itemCount2) : 0);
			}
			if (itemCount3 > 0 && count > 0 && RemoveTempate(eBageType.Consortia, itemTemplateInfo, (itemCount3 > count) ? count : itemCount3))
			{
				count = ((count >= itemCount3) ? (count - itemCount3) : 0);
			}
			if (itemCount4 > 0 && count > 0 && RemoveTempate(eBageType.MagicHouseBag, itemTemplateInfo, (itemCount4 > count) ? count : itemCount4))
			{
				count = ((count >= itemCount4) ? (count - itemCount4) : 0);
			}
			if (count == 0)
			{
				return true;
			}
			if (log.IsErrorEnabled)
			{
				log.Error($"Item Remove Error, PlayerId {m_playerId} Remove TemplateId {templateId} Is Not Zero!");
			}
		}
		return false;
	}

	public bool RemoveTemplate(int templateId, int count, bool canRenew, bool canTransfer)
	{
		int itemCount = m_equipBag.GetItemCount(0, templateId, canRenew, canTransfer);
		int itemCount2 = m_propBag.GetItemCount(templateId, canRenew, canTransfer);
		int itemCount3 = m_consortiaBag.GetItemCount(templateId, canRenew, canTransfer);
		int itemCount4 = m_magicHouse.GetItemCount(templateId, canRenew, canTransfer);
		int num = itemCount + itemCount2 + itemCount3;
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null && num >= count)
		{
			if (itemCount > 0 && count > 0 && RemoveTempate(eBageType.EquipBag, itemTemplateInfo, (itemCount > count) ? count : itemCount, canRenew, canTransfer))
			{
				count = ((count >= itemCount) ? (count - itemCount) : 0);
			}
			if (itemCount2 > 0 && count > 0 && RemoveTempate(eBageType.PropBag, itemTemplateInfo, (itemCount2 > count) ? count : itemCount2))
			{
				count = ((count >= itemCount2) ? (count - itemCount2) : 0);
			}
			if (itemCount3 > 0 && count > 0 && RemoveTempate(eBageType.Consortia, itemTemplateInfo, (itemCount3 > count) ? count : itemCount3))
			{
				count = ((count >= itemCount3) ? (count - itemCount3) : 0);
			}
			if (itemCount4 > 0 && count > 0 && RemoveTempate(eBageType.MagicHouseBag, itemTemplateInfo, (itemCount4 > count) ? count : itemCount4))
			{
				count = ((count >= itemCount4) ? (count - itemCount4) : 0);
			}
			if (count == 0)
			{
				return true;
			}
			if (log.IsErrorEnabled)
			{
				log.Error($"Item Remove Error, PlayerId {m_playerId} Remove TemplateId {templateId} Is Not Zero!");
			}
		}
		return false;
	}

	public bool RemoveTempate(eBageType bagType, ItemTemplateInfo template, int count, bool canRenew, bool canTransfer)
	{
		return GetInventory(bagType)?.RemoveTemplate(template.TemplateID, count, canRenew, canTransfer) ?? false;
	}

	public bool RemoveTempate(eBageType bagType, ItemTemplateInfo template, int count)
	{
		return GetInventory(bagType)?.RemoveTemplate(template.TemplateID, count) ?? false;
	}

	public bool RemoveTemplate(ItemTemplateInfo template, int count)
	{
		return GetItemInventory(template)?.RemoveTemplate(template.TemplateID, count) ?? false;
	}

	public bool ClearTempBag()
	{
		TempBag.ClearBag();
		return true;
	}

	public bool ClearFightBag()
	{
		FightBag.ClearBag();
		return true;
	}

	public void ClearCaddyBag()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < CaddyBag.Capalility; i++)
		{
			ItemInfo itemAt = CaddyBag.GetItemAt(i);
			if (itemAt != null)
			{
				ItemInfo itemInfo = ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
				itemInfo.Count = 1;
				list.Add(itemInfo);
			}
		}
		CaddyBag.ClearBag();
		AddTemplate(list);
	}

	public void ClearStoreBag()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < StoreBag.Capalility; i++)
		{
			ItemInfo itemAt = StoreBag.GetItemAt(i);
			if (itemAt != null)
			{
				ItemInfo itemInfo = ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
				itemInfo.Count = itemAt.Count;
				list.Add(itemInfo);
			}
		}
		StoreBag.ClearBag();
		AddTemplate(list);
	}

	public void ValidateItems()
	{
		EquipBag.ValidItems();
		ConsortiaBag.ValidItems();
	}

	public bool IsConsortia()
	{
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(PlayerCharacter.ConsortiaID);
		return consortiaInfo != null;
	}

	public void OnUseBuffer()
	{
		if (this.UseBuffer != null)
		{
			this.UseBuffer(this);
		}
	}

	public void AddBeadEffect(ItemInfo item)
	{
		m_equipEffect.Add(item);
	}

	public void AddHorseAmuletEffect(ItemInfo item)
	{
		m_horseAmuletEffect.Add(item);
	}

	public void BeginAllChanges()
	{
		BeginChanges();
		m_bufferList.BeginChanges();
		m_equipBag.BeginChanges();
		m_propBag.BeginChanges();
		m_beadBag.BeginChanges();
		m_farmBag.BeginChanges();
		m_vegetable.BeginChanges();
		m_cardBag.BeginChanges();
	}

	public void CommitAllChanges()
	{
		CommitChanges();
		m_bufferList.CommitChanges();
		m_equipBag.CommitChanges();
		m_propBag.CommitChanges();
		m_beadBag.BeginChanges();
		m_farmBag.CommitChanges();
		m_vegetable.CommitChanges();
		m_cardBag.CommitChanges();
	}

	public void BeginChanges()
	{
		Interlocked.Increment(ref m_changed);
	}

	public void CommitChanges()
	{
		Interlocked.Decrement(ref m_changed);
		OnPropertiesChanged();
	}

	protected void OnPropertiesChanged()
	{
		if (m_changed <= 0)
		{
			if (m_changed < 0)
			{
				log.Error("Player changed count < 0");
				Thread.VolatileWrite(ref m_changed, 0);
			}
			UpdateProperties();
		}
	}

	public void UpdateBadgeId(int Id)
	{
		m_character.badgeID = Id;
	}

	public void UpdateTimeBox(int receiebox, int receieGrade, int needGetBoxTime)
	{
		m_character.receiebox = receiebox;
		m_character.receieGrade = receieGrade;
		m_character.needGetBoxTime = needGetBoxTime;
	}

	public string GetFightFootballStyle(int team)
	{
		if (team == 1)
		{
			return CreateFightFootballStyle(team);
		}
		return CreateFightFootballStyle(team);
	}

	public string CreateFightFootballStyle(int team)
	{
		ItemInfo itemAt = m_equipBag.GetItemAt(0);
		string text = ((itemAt == null) ? "" : (itemAt.TemplateID + "|" + itemAt.Template.Pic));
		string text2 = text;
		string text3 = text;
		for (int i = 0; i < EquipPlace.Length; i++)
		{
			text2 += ",";
			text3 += ",";
			if (EquipPlace[i] == 11)
			{
				if (PlayerCharacter.Sex)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(13573);
					text2 = text2 + itemTemplateInfo.TemplateID + "|" + itemTemplateInfo.Pic;
					itemTemplateInfo = ItemMgr.FindItemTemplate(13572);
					text3 = text3 + itemTemplateInfo.TemplateID + "|" + itemTemplateInfo.Pic;
				}
				else
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(13575);
					text2 = text2 + itemTemplateInfo.TemplateID + "|" + itemTemplateInfo.Pic;
					itemTemplateInfo = ItemMgr.FindItemTemplate(13574);
					text3 = text3 + itemTemplateInfo.TemplateID + "|" + itemTemplateInfo.Pic;
				}
			}
			else if (EquipPlace[i] == 6)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(70396);
				string text4 = itemTemplateInfo.TemplateID + "|" + itemTemplateInfo.Pic;
				text2 += text4;
				text3 += text4;
			}
			else
			{
				itemAt = m_equipBag.GetItemAt(EquipPlace[i]);
				if (itemAt != null)
				{
					string text5 = itemAt.TemplateID + "|" + itemAt.Pic;
					text2 += text5;
					text3 += text5;
				}
			}
		}
		if (team == 1)
		{
			return text2;
		}
		return text3;
	}

	public void checkBugGold()
	{
		if (PlayerCharacter.Gold < 0)
		{
			PlayerCharacter.Gold = 0;
		}
		if (PlayerCharacter.DDTMoney < 0)
		{
			PlayerCharacter.DDTMoney = 0;
		}
		if (PlayerCharacter.Money < 0)
		{
			PlayerCharacter.Money = 0;
		}
		if (PlayerCharacter.myHonor < 0)
		{
			PlayerCharacter.myHonor = 0;
		}
		if (PlayerCharacter.medal < 0)
		{
			PlayerCharacter.medal = 0;
		}
	}

	public void UpdateProperties()
	{
		checkBugGold();
		Out.SendUpdatePrivateInfo(m_character);
		GSPacketIn pkg = Out.SendUpdatePublicPlayer(m_character, m_battle.MatchInfo);
		if (_currentRoom != null)
		{
			_currentRoom.SendToAll(pkg, this);
		}
	}

	public int DragonBoatAddExpPlus()
	{
		return DragonBoatMgr.AddExpPlus();
	}

	public bool DragonBoatOpen()
	{
		return DragonBoatMgr.IsContinuous();
	}

	public string TcpEndPoint()
	{
		return Client.TcpEndpoint.Split(':')[0];
	}

	public bool MaxLevel()
	{
		return Level == LevelMgr.MaxLevel && PlayerCharacter.guardCoreGrade >= GuardCoreMgr.MaxLevel();
	}

	public int AddScoreBombKing(int value)
	{
		int num = 0;
		bool win = false;
		if (value > 1)
		{
			num = 10 + 3 * (BombKingMgr.turn + 1) + 200 + value - 1;
			win = true;
		}
		else
		{
			num = 10 + 3 * (BombKingMgr.turn + 1) + 100;
		}
		BombKingMgr.AddScore(this, num, win);
		battleStage = 1;
		battleId = 0;
		if (BombKingMgr.battleStage < 3)
		{
			Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BombKing.Msg.Game.ReceivedPoint", num));
		}
		else if (value > 1)
		{
			Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BombKing.Msg.Game.PassNextState"));
		}
		else
		{
			Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BombKing.Msg.Game.LostNextState"));
		}
		return value;
	}

	public int AddScoreCampBattle(int value)
	{
		return 0;
	}

	public int AddGold(int value)
	{
		if (value > 0)
		{
			m_character.Gold += value;
			if (m_character.Gold <= int.MinValue)
			{
				m_character.Gold = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg4"));
			}
			this.AddFund?.Invoke(value, 0);
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveGold(int value)
	{
		if (value > 0 && value <= m_character.Gold)
		{
			m_character.Gold -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddMoney(int value)
	{
		long num = m_character.Money;
		if (value > 0)
		{
			num += value;
			if (num > int.MaxValue)
			{
				m_character.Money = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg5"));
			}
			else
			{
				m_character.Money += LogMgr.AddMoney(m_character.ID, value);
				this.AddFund?.Invoke(value, 1);
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddCupomPVP(int value)
	{
		if (GameProperties.contasPorIp == -1)
		{
			return AddMoney(value);
		}
		int num = 0;
		string text = Client.TcpEndpoint.Split(':')[0];
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			if (gamePlayer.Client.TcpEndpoint.Split(':')[0] == text)
			{
				num++;
			}
		}
		if (num > GameProperties.contasPorIp)
		{
			SendMessage("请下线另外一个号,竞技无法读取账号,要求一个号打竞技（不然没有点卷奖励）");
			return 0;
		}
		return AddMoney(value);
	}

	public int RemoveMoney(int value)
	{
		int result = 0;
		if (value < 0 || value > m_character.Money)
		{
			return result;
		}
		AddExpVip(value / 100);
		m_character.Money -= value;
		result = LogMgr.RemoveMoney(m_character.ID, value);
		if (result == 0)
		{
			m_character.Money = LogMgr.GetMoney(m_character.ID);
		}
		OnPropertiesChanged();
		return result;
	}

	public void AddJampsCurrency(int value)
	{
		m_character.jampsCurrency += value;
		if (m_character.jampsCurrency <= int.MinValue)
		{
			m_character.jampsCurrency = int.MaxValue;
		}
		OnPropertiesChanged();
	}

	public int AddSoulCrystal(int value)
	{
		long num = m_extra.Info.SoulCrystal;
		if (value > 0)
		{
			num += value;
			if (num > int.MaxValue)
			{
				m_extra.Info.SoulCrystal = int.MaxValue;
			}
			else
			{
				m_extra.Info.SoulCrystal = (int)num;
			}
			Out.SendUpdateSoulCrystal(m_extra.Info);
			return value;
		}
		return 0;
	}

	public int RemoveSoulCrystal(int value)
	{
		if (value < 0 || value > m_extra.Info.SoulCrystal)
		{
			return 0;
		}
		m_extra.Info.SoulCrystal -= value;
		Out.SendUpdateSoulCrystal(m_extra.Info);
		return value;
	}

	public void UpdateTrailElite(bool isWin)
	{
		if (TrailElite.Data.trailEliteLevel < WorldMgr.TrailEliteAward.Length)
		{
			TrailElite.UpdateTrailElite(isWin);
		}
	}

	public int AddHonor(int value)
	{
		if (value > 0)
		{
			m_character.myHonor += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddAchievementPoint(int value)
	{
		if (value > 0)
		{
			m_character.AchievementPoint += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveHonor(int value)
	{
		if (value > 0)
		{
			m_character.myHonor -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddSummerScore(int value)
	{
		if (value > 0)
		{
			Extra.Info.SummerScore += value;
			Extra.Info.Score += value;
			return value;
		}
		return 0;
	}

	public int RemoveSummerScore(int value)
	{
		if (value > 0)
		{
			Extra.Info.SummerScore -= value;
			return value;
		}
		return 0;
	}

	public int AddTotem(int value)
	{
		if (value > 0)
		{
			m_character.totemId = value;
			OnPropertiesChanged();
			return value;
		}
		return m_character.totemId;
	}

	public int AddMaxHonor(int value)
	{
		if (value > 0)
		{
			m_character.MaxBuyHonor += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddBoatScore(int value)
	{
		if (value > 0)
		{
			Actives.Info.useableScore += value;
			Actives.Info.totalScore += value;
			return value;
		}
		return 0;
	}

	public int AddScoreCityBattle(int value)
	{
		return AddScoreCityBattle(value, isSend: false);
	}

	public int AddScoreCityBattle(int value, bool isSend)
	{
		if (value > 0 && WorldMgr.CheckOpenCityBattle())
		{
			GSPacketIn gSPacketIn = new GSPacketIn(368, PlayerId);
			gSPacketIn.WriteByte(7);
			gSPacketIn.WriteInt(value);
			gSPacketIn.WriteBoolean(isSend);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
		}
		return value;
	}

	public bool RemoveMissionEnergy(int value)
	{
		if (!GameProperties.IsMissionEnergyEnable)
		{
			return true;
		}
		if (value > 0 && value <= Extra.Info.MissionEnergy)
		{
			Extra.Info.MissionEnergy -= value;
			Out.SendMissionEnergy(Extra.Info);
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg6", value));
			return true;
		}
		return false;
	}

	public bool MissionEnergyEmpty(int value)
	{
		if (!GameProperties.IsMissionEnergyEnable)
		{
			return true;
		}
		if (value > Extra.Info.MissionEnergy)
		{
			return false;
		}
		return true;
	}

	public int AddMissionEnergy(int value)
	{
		if (!GameProperties.IsMissionEnergyEnable)
		{
			return 0;
		}
		if (value > 0)
		{
			Extra.Info.MissionEnergy += value;
			Out.SendMissionEnergy(Extra.Info);
			return value;
		}
		return 0;
	}

	public int AddEnergyTotal(int value)
	{
		if (!GameProperties.IsMissionEnergyEnable)
		{
			return 0;
		}
		if (Extra.Info.EnergyTotal > 150)
		{
			return 0;
		}
		if (value > 0)
		{
			Extra.Info.EnergyTotal += value;
			return value;
		}
		return 0;
	}

	public int AddScoreMagicstone(int value)
	{
		if (value > 0)
		{
			m_extra.Info.ScoreMagicstone += value;
			if (m_extra.Info.ScoreMagicstone <= int.MinValue)
			{
				m_extra.Info.ScoreMagicstone = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg7"));
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveScoreMagicstone(int value)
	{
		if (value > 0 && value <= m_extra.Info.ScoreMagicstone)
		{
			m_extra.Info.ScoreMagicstone -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public bool ActiveMoneyEnable(int value)
	{
		if (GameProperties.IsActiveMoney)
		{
			if (value < 1)
			{
				return false;
			}
			if (Actives.Info.ActiveMoney >= value)
			{
				RemoveActiveMoney(value);
				RemoveMoney(value);
				return true;
			}
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg8", Actives.Info.ActiveMoney));
			return false;
		}
		return MoneyDirect(value);
	}

	public int AddActiveMoney(int value)
	{
		if (value > 0 && GameProperties.IsActiveMoney)
		{
			Actives.Info.ActiveMoney += value;
			if (Actives.Info.ActiveMoney <= int.MinValue)
			{
				Actives.Info.ActiveMoney = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg9"));
			}
			else
			{
				SendHideMessage(LanguageMgr.GetTranslation("GamePlayer.Msg1", value, Actives.Info.ActiveMoney));
			}
			return value;
		}
		return 0;
	}

	public int RemoveActiveMoney(int value)
	{
		if (value > 0 && value <= Actives.Info.ActiveMoney)
		{
			Actives.Info.ActiveMoney -= value;
			SendHideMessage(LanguageMgr.GetTranslation("GamePlayer.Msg2", value, Actives.Info.ActiveMoney));
			return value;
		}
		return 0;
	}

	public int AddLeagueMoney(int value)
	{
		if (value > 0)
		{
			m_character.LeagueMoney += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveLeagueMoney(int value)
	{
		if (value > 0 && value <= m_character.LeagueMoney)
		{
			m_character.LeagueMoney -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddHardCurrency(int value)
	{
		if (value > 0)
		{
			m_character.hardCurrency += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveHardCurrency(int value)
	{
		if (value > 0 && value <= m_character.hardCurrency)
		{
			m_character.hardCurrency -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public void AddPrestige(bool isWin, eRoomType roomType)
	{
		if (roomType == eRoomType.RingStation)
		{
			UserRingStationInfo singleRingStationInfos = RingStationMgr.GetSingleRingStationInfos(PlayerCharacter.ID);
			if (singleRingStationInfos != null)
			{
				int num = RingStationMgr.ConfigInfo.AwardBattleByRank(singleRingStationInfos.Rank, isWin);
				string translation = LanguageMgr.GetTranslation("Ringstasion.BattleLost", num);
				if (isWin)
				{
					num = RingStationMgr.ConfigInfo.AwardBattleByRank(singleRingStationInfos.Rank, isWin);
					translation = LanguageMgr.GetTranslation("Ringstasion.BattleWin", num);
				}
				AddLeagueMoney(num);
				SendMessage(translation);
			}
		}
		if (roomType == eRoomType.FightGround)
		{
			BattleData.AddPrestige(isWin);
		}
	}

	public int AddHomeBarEnergy(int value)
	{
		return AddHomeBarEnergy(value, -1, 0);
	}

	public int AddHomeBarEnergy(int value, int playerType)
	{
		return AddHomeBarEnergy(value, -1, playerType);
	}

	public int AddHomeBarEnergy(int value, int hostId, int playerType)
	{
		if (Home.Data == null)
		{
			return 0;
		}
		if (value > 0)
		{
			Home.Data.energyCount += value;
			Home.SendHomeBarInfo(playerType, hostId);
			return value;
		}
		return 0;
	}

	public void UpdateDungeonCount(int mapId)
	{
		m_extra.UpdateDungeonCount(mapId);
	}

	public int RemoveHomeBarEnergy(int value)
	{
		if (Home.Data == null)
		{
			return 0;
		}
		if (value > 0 && value <= Home.Data.energyCount)
		{
			Home.Data.energyCount -= value;
			Home.SendHomeBarUpdateEnergy();
			return value;
		}
		return 0;
	}

	public void RingstationResult(bool isWin)
	{
		if (DareFlag != null)
		{
			DareFlag.SuccessFlag = isWin;
		}
		if (SuccessFlag != null)
		{
			SuccessFlag.SuccessFlag = !isWin;
		}
		RingStationMgr.UpdateRingBattleFields(DareFlag, SuccessFlag);
	}

	public void UpdateHonor(string honor)
	{
		UserRankInfo singleRank = Rank.GetSingleRank(honor);
		if (singleRank != null)
		{
			PlayerCharacter.honorId = singleRank.NewTitleID;
			PlayerCharacter.Honor = honor;
			EquipBag.UpdatePlayerProperties();
		}
		else
		{
			SendMessage(LanguageMgr.GetTranslation("UpdateHonor.Fail"));
		}
	}

	public void UpdateRestCount()
	{
		BattleData.Update();
	}

	public void RemoveFistGetPet()
	{
		m_character.IsFistGetPet = false;
		m_character.LastRefreshPet = DateTime.Now.AddDays(-1.0);
	}

	public void RemoveLastRefreshPet()
	{
		m_character.LastRefreshPet = DateTime.Now;
	}

	public void UpdateAnswerSite(int id)
	{
		if (PlayerCharacter.AnswerSite < id)
		{
			PlayerCharacter.AnswerSite = id;
		}
		UpdateWeaklessGuildProgress();
		Out.SendWeaklessGuildProgress(PlayerCharacter);
	}

	public void UpdateWeaklessGuildProgress()
	{
		if (PlayerCharacter.weaklessGuildProgress == null)
		{
			PlayerCharacter.weaklessGuildProgress = Convert.FromBase64String(PlayerCharacter.WeaklessGuildProgressStr);
		}
		PlayerCharacter.CheckLevelFunction();
		if (PlayerCharacter.Grade == 1)
		{
			PlayerCharacter.OpenFunction(Step.POP_MOVE);
		}
		if (PlayerCharacter.IsOldPlayer)
		{
			PlayerCharacter.OpenFunction(Step.OLD_PLAYER);
		}
		PlayerCharacter.WeaklessGuildProgressStr = Convert.ToBase64String(PlayerCharacter.weaklessGuildProgress);
	}

	public bool canUpLv(int exp, int curLv)
	{
		List<int> list = GameProperties.VIPExp();
		if (exp >= list[0] && curLv == 0)
		{
			return true;
		}
		if (exp >= list[1] && curLv == 1)
		{
			return true;
		}
		if (exp >= list[2] && curLv == 2)
		{
			return true;
		}
		if (exp >= list[3] && curLv == 3)
		{
			return true;
		}
		if (exp >= list[4] && curLv == 4)
		{
			return true;
		}
		if (exp >= list[5] && curLv == 5)
		{
			return true;
		}
		if (exp >= list[6] && curLv == 6)
		{
			return true;
		}
		if (exp >= list[7] && curLv == 7)
		{
			return true;
		}
		if (exp >= list[8] && curLv == 8)
		{
			return true;
		}
		if (exp >= list[9] && curLv == 9)
		{
			return true;
		}
		if (exp >= list[10] && curLv == 10)
		{
			return true;
		}
		if (exp >= list[11] && curLv == 11)
		{
			return true;
		}
		if (exp >= list[12] && curLv == 12)
		{
			return true;
		}
		if (exp >= list[13] && curLv == 13)
		{
			return true;
		}
		if (exp >= list[14] && curLv == 14)
		{
			return true;
		}
		return false;
	}

	public void AddExpVip(int value)
	{
		List<int> list = GameProperties.VIPExp();
		m_character.VIPExp += value;
		for (int i = 0; i < list.Count; i++)
		{
			int vIPExp = m_character.VIPExp;
			int vIPLevel = m_character.VIPLevel;
			if (vIPLevel == list.Count)
			{
				m_character.VIPExp = list[list.Count - 1];
				break;
			}
			if (vIPLevel < list.Count && canUpLv(vIPExp, vIPLevel))
			{
				m_character.VIPLevel++;
				if (m_character.VIPLevel >= 1 && PetBag != null)
				{
					PetBag.UpdatePetFiveKillSlot(m_character.VIPLevel);
				}
				List<int> list2 = GameProperties.VIPLevelGiftList();
				if (m_character.VIPLevel <= list2.Count && list2[m_character.VIPLevel - 1] > 0)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list2[m_character.VIPLevel - 1]), 1, 103);
					itemInfo.IsBinds = true;
					SendItemsToMail(new List<ItemInfo> { itemInfo }, LanguageMgr.GetTranslation("GameServer.VIPPackageLevel.Mail.Content", m_character.VIPLevel), LanguageMgr.GetTranslation("GameServer.VIPPackageLevel.Mail.Title", m_character.VIPLevel), eMailType.BuyItem);
				}
			}
		}
		if (m_character.IsVIPExpire())
		{
			Out.SendOpenVIP(PlayerCharacter);
		}
	}

	public void AddExpVips(int value)
	{
		List<int> list = GameProperties.VIPExp();
		if (value > 0)
		{
			m_character.VIPExp += value;
		}
	}

	public void AddHonorExp(int value)
	{
		if (value > 0)
		{
			m_character.HonorInfo.HonorExp += value;
		}
		ServerData serverData = new ServerData();
		int exp = serverData.TS_HonorSystem_Template.FirstOrDefault((TS_HonorSystem_Template e) => e.Level == m_character.HonorInfo.HonorLevel).Exp;
		if (m_character.HonorInfo.HonorExp >= exp)
		{
			m_character.HonorInfo.HonorLevel++;
		}
	}

	public int AddCardSoul(int value)
	{
		if (value > 0)
		{
			m_character.CardSoul += value;
			if (m_character.CardSoul <= int.MinValue)
			{
				m_character.CardSoul = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg10"));
			}
			return value;
		}
		return 0;
	}

	public int RemoveCardSoul(int value)
	{
		if (value > 0 && value <= m_character.CardSoul)
		{
			m_character.CardSoul -= value;
			return value;
		}
		return 0;
	}

	public int AddScore(int value)
	{
		if (value > 0)
		{
			m_character.Score += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveScore(int value)
	{
		if (value > 0 && value <= m_character.Score)
		{
			m_character.Score -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddDamageScores(int value)
	{
		if (value > 0)
		{
			m_character.damageScores += value;
			if (m_character.damageScores <= int.MinValue)
			{
				m_character.damageScores = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg11"));
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveDamageScores(int value)
	{
		if (value > 0 && value <= m_character.damageScores)
		{
			m_character.damageScores -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddPetScore(int value)
	{
		if (value > 0)
		{
			m_character.petScore += value;
			if (m_character.petScore <= int.MinValue)
			{
				m_character.petScore = int.MaxValue;
				SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg12"));
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemovePetScore(int value)
	{
		if (value > 0 && value <= m_character.petScore)
		{
			m_character.petScore -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddmyHonor(int value)
	{
		if (value > 0)
		{
			m_character.myHonor += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemovemyHonor(int value)
	{
		if (value > 0 && value <= m_character.myHonor)
		{
			m_character.myHonor -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddMedal(int value)
	{
		if (value > 0)
		{
			m_character.medal += value;
			this.AddFund?.Invoke(value, 4);
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveMedal(int value)
	{
		if (value > 0 && value <= m_character.medal)
		{
			m_character.medal -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddOffer(int value)
	{
		return AddOffer(value, IsRate: true);
	}

	public int AddOffer(int value2, bool IsRate)
	{
		long num = value2;
		if (num > 0)
		{
			if (AntiAddictionMgr.ISASSon)
			{
				num = (int)((double)num * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
			}
			if (IsRate)
			{
				num *= (((int)OfferAddPlus == 0) ? 1 : ((int)OfferAddPlus));
			}
			long num2 = m_character.Offer + num;
			if (num2 < int.MaxValue)
			{
				m_character.Offer += Convert.ToInt32(num);
			}
			else
			{
				m_character.Offer = int.MaxValue;
			}
			OnPropertiesChanged();
			return (num > int.MaxValue) ? int.MaxValue : Convert.ToInt32(num);
		}
		return 0;
	}

	public int RemoveOffer(int value)
	{
		if (value > 0)
		{
			if (value > m_character.Offer)
			{
				return 0;
			}
			m_character.Offer -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveUserOffer(int value)
	{
		if (value > 0)
		{
			if (value > m_character.RichesOffer)
			{
				return 0;
			}
			int num = value / 2;
			m_character.RichesOffer -= num;
			m_character.RichesRob -= num;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveGiftToken(int value)
	{
		if (value > 0 && value <= m_character.DDTMoney)
		{
			AddExpVip(value / 100);
			m_character.DDTMoney -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public void CreateRandomEnterModeItem()
	{
		if (FightBag.GetItems().Count > 0)
		{
			return;
		}
		List<ItemInfo> list = WorldMgr.CreateRandomPropItem();
		foreach (ItemInfo item in list)
		{
			item.Count = 1;
			FightBag.AddItem(item);
		}
	}

	public int AddEnterModePoint(int value)
	{
		if (value > 0)
		{
			int entertamentPoint = Actives.Info.EntertamentPoint;
			Actives.Info.EntertamentPoint += value;
			WorldMgr.ChatEntertamentModeUpdatePoint(m_character, value);
			List<int[]> entertainmentScoreReward = GameProperties.GetEntertainmentScoreReward();
			for (int i = 0; i <= entertainmentScoreReward.Count; i++)
			{
				int[] array = entertainmentScoreReward[i];
				if (Actives.Info.EntertamentPoint >= array[0] && entertamentPoint < array[0])
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[1]), 1, 101);
					itemInfo.IsBinds = true;
					SendItemToMail(itemInfo, "娱乐模式:积分奖励", "娱乐模式:积分奖励", eMailType.Active);
				}
			}
			return value;
		}
		return 0;
	}

	public bool CanUseBuffGp()
	{
		if (GameProperties.NewbieEnable)
		{
			return true;
		}
		if (Level >= GameProperties.BeginLevel)
		{
			return true;
		}
		return false;
	}

	public void UpdateUserMoney(int value, int type)
	{
		if (Extra != null && Extra.UserInfo != null && value > 0)
		{
			switch (type)
			{
			case 1:
				Extra.UserInfo.UserMoneyPay += value;
				break;
			case 2:
				Extra.UserInfo.UserMoneyWar += value;
				break;
			case 3:
				Extra.UserInfo.UserMoneyShop += value;
				break;
			}
		}
	}

	public int AddGP(int gp)
	{
		return AddGP(gp, canDupeExp: false, canAddTop: true);
	}

	public int AddGP(int gp, bool canDupeExp)
	{
		return AddGP(gp, canDupeExp, canAddTop: true);
	}

	public int AddGP(int gp, bool canDupeExp, bool canAddTop)
	{
		if (gp > 0)
		{
			long num = gp;
			if (CanUseBuffGp())
			{
				if (AntiAddictionMgr.ISASSon)
				{
					num = (long)((double)num * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
				}
				num = (int)((float)num * RateMgr.GetRate(eRateType.Experience_Rate));
				if (canDupeExp)
				{
					long num2 = num;
					if (GPAddPlus > 0.0)
					{
						num = (long)((double)num2 * GPAddPlus);
					}
					if (m_actives.Info.wellAddRate > 0)
					{
						num = num2 * (m_actives.Info.wellAddRate / 100);
					}
				}
				if (PlayerCharacter.GourdExpInfo.TemplateID != 0)
				{
					GourdExpInfoHandler.AddGourdExp(this, gp);
					OnPropertiesChanged();
					return gp;
				}
			}
			if (canAddTop && Extra != null && Extra.UserInfo != null)
			{
				long num3 = Extra.UserInfo.UserAllGP + num;
				Extra.UserInfo.UserAllGP = ((num3 > int.MaxValue) ? int.MaxValue : Convert.ToInt32(num3));
			}
			if (MaxLevel())
			{
				long num4 = num / 100;
				if (num4 > 0)
				{
					AddOffer((num4 > int.MaxValue) ? int.MaxValue : Convert.ToInt32(num4));
					SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg3", num4));
				}
			}
			else
			{
				if (Level != LevelMgr.MaxLevel)
				{
					m_character.GP += gp;
					if (m_character.GP < 1)
					{
						m_character.GP = 1L;
					}
				}
				Level = LevelMgr.GetLevel(m_character.GP);
				UpdateFightPower();
			}
			if (Level >= 70 && GameProperties.VERSION >= 8200)
			{
				KingGrade.AddGP(num);
			}
			OnPropertiesChanged();
			return (num > int.MaxValue) ? int.MaxValue : Convert.ToInt32(num);
		}
		return 0;
	}

	public void UpdateLevel()
	{
		Level = LevelMgr.GetLevel(m_character.GP);
	}

	public int RemoveGP(int gp)
	{
		if (gp > 0)
		{
			m_character.GP -= gp;
			if (m_character.GP < 1)
			{
				m_character.GP = 1L;
			}
			int level = LevelMgr.GetLevel(m_character.GP);
			if (Level > level)
			{
				m_character.GP += gp;
			}
			UpdateLevel();
			return gp;
		}
		return 0;
	}

	public int AddRobRiches(int value)
	{
		if (value > 0)
		{
			if (AntiAddictionMgr.ISASSon)
			{
				value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
			}
			m_character.RichesRob += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddRichesOffer(int value)
	{
		if (value > 0)
		{
			m_character.RichesOffer += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddGiftToken(int value)
	{
		if (value > 0)
		{
			m_character.DDTMoney += value;
			this.AddFund?.Invoke(value, 2);
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public bool GiftTokenDirect(int value)
	{
		if (value <= 0 || PlayerCharacter.DDTMoney < value)
		{
			return false;
		}
		RemoveGiftToken(value);
		return true;
	}

	public bool CanEquip(ItemTemplateInfo item)
	{
		bool flag = true;
		string message = "";
		if (!item.CanEquip)
		{
			flag = false;
			message = LanguageMgr.GetTranslation("Game.Server.GameObjects.NoEquip");
		}
		else if (m_character.Grade < item.NeedLevel)
		{
			flag = false;
			message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanLevel");
		}
		if (!flag)
		{
			Out.SendMessage(eMessageType.ERROR, message);
		}
		return flag;
	}

	public void UpdateBaseProperties(int attack, int defence, int agility, int lucky, int hp, int MagicAttack, int MagicDefence)
	{
		if (attack != m_character.Attack || defence != m_character.Defence || agility != m_character.Agility || lucky != m_character.Luck || hp != m_character.hp || MagicAttack != m_character.MagicDefence || MagicDefence != m_character.MagicAttack)
		{
			m_character.Attack = attack;
			m_character.Defence = defence;
			m_character.Agility = agility;
			m_character.Luck = lucky;
			m_character.MagicAttack = MagicAttack;
			m_character.MagicDefence = MagicDefence;
			m_character.hp = hp;
			OnPropertiesChanged();
		}
	}

	public void UpdateStyle(string style, string color, string skin)
	{
		if (style != m_character.Style || color != m_character.Colors || skin != m_character.Skin)
		{
			m_character.Style = style;
			m_character.Colors = color;
			m_character.Skin = skin;
			OnPropertiesChanged();
		}
	}

	public void UpdateFightPower()
	{
		FightPower = 0;
		int num = 0;
		int hp = PlayerCharacter.hp;
		num += PlayerCharacter.Attack;
		num += PlayerCharacter.Defence;
		num += PlayerCharacter.Agility;
		num += PlayerCharacter.Luck;
		num += PlayerCharacter.MagicAttack;
		num += PlayerCharacter.MagicDefence;
		int num2 = num;
		BaseAttack = GetBaseAttack();
		BaseDefence = GetBaseDefence();
		if (m_currentSecondWeapon != null)
		{
			FightPower += (int)((double)m_currentSecondWeapon.Template.Property7 * Math.Pow(1.1, m_currentSecondWeapon.StrengthenLevel));
			WashFightPower = FightPower;
		}
		int num3 = (int)((double)GetBaseElementProperties() * 1.5);
		int num4 = GetBaseSecondProperties() * 2;
		int num5 = (GameProperties.reduzirFC ? 1000000000 : 200000000);
		FightPower += (int)((double)(num + 1000) * (BaseAttack * BaseAttack * BaseAttack + 3.5 * BaseDefence * BaseDefence * BaseDefence) / (double)num5 + (double)hp * 0.95);
		FightPower += num4 + num3;
		WashFightPower += (int)((double)(num2 + 1000) * (BaseAttack * BaseAttack * BaseAttack + 3.5 * BaseDefence * BaseDefence * BaseDefence) / (double)num5 + (double)hp * 0.95);
		WashFightPower += num4 + num3;
		PlayerCharacter.FightPower = FightPower;
		WashFightPower = ((WashFightPower >= FightPower) ? (WashFightPower - FightPower) : 0);
		if (PlayerCharacter.FightPower < 0)
		{
			PlayerCharacter.FightPower = int.MaxValue;
		}
		if (WashFightPower < 0)
		{
			WashFightPower = int.MaxValue;
		}
		if (PlayerCharacter.FightPower >= 10000000)
		{
			PlayerCharacter.AcademyInfo.IsMaster = true;
		}
		if (PlayerCharacter.FightPower >= 10000000 && PlayerCharacter.AcademyInfo.IsFirst && PlayerCharacter.AcademyInfo.MasterID == 0)
		{
			PlayerCharacter.AcademyInfo.IsFirst = false;
			AcademyMgr.SendAwards(this, PlayerCharacter.FightPower);
		}
		if (PlayerCharacter.AcademyInfo.MasterID != 0)
		{
			AcademyMgr.UpdateAwardApp(this, PlayerCharacter.FightPower);
		}
		m_actives.SynFightPowerSign();
		Out.SendSignBuffInfo(Actives.Info);
	}

	public void UpdateHide(int hide)
	{
		if (hide != m_character.Hide)
		{
			m_character.Hide = hide;
			OnPropertiesChanged();
		}
	}

	public void UpdateWeapon(ItemInfo item)
	{
		if (item != null && item != m_MainWeapon)
		{
			m_MainWeapon = item;
			OnPropertiesChanged();
		}
	}

	public void UpdateBatleConfig(string type, int value)
	{
		switch (type)
		{
		case "MagicHouse":
			m_batleConfig.MagicHouse = value;
			break;
		case "PetFormReduceDamage":
			m_batleConfig.PetFormReduceDamage = value;
			break;
		case "CatchInsect":
			m_batleConfig.CakeStatus = value == 1;
			break;
		}
	}

	public void UpdatePet(UsersPetInfo pet)
	{
		m_pet = pet;
	}

	public void UpdateHealstone(ItemInfo item)
	{
		if (item != null && item.Template.Property2 > 0)
		{
			m_healstone = item;
		}
	}

	public void UpdateReduceDame(ItemInfo item)
	{
		if (item != null && item.Template != null)
		{
			PlayerCharacter.ReduceDamePlus = item.Template.Property1;
		}
	}

	public bool RemoveHealstone()
	{
		ItemInfo itemAt = m_equipBag.GetItemAt(18);
		if (itemAt != null && itemAt.Count > 0)
		{
			if (itemAt.Template.CategoryID == 70)
			{
				return false;
			}
			return m_equipBag.RemoveCountFromStack(itemAt, 1);
		}
		return false;
	}

	public void UpdateWeapon3D(ItemInfo item)
	{
		if (item != m_currentWeapon3D)
		{
			m_currentWeapon3D = item;
			OnPropertiesChanged();
		}
	}

	public void UpdateSecondWeapon(ItemInfo item)
	{
		if (item != m_currentSecondWeapon)
		{
			m_currentSecondWeapon = item;
			OnPropertiesChanged();
		}
	}

	public void UpdateAmulet(ItemInfo item)
	{
		if (item != m_currentAmulet)
		{
			m_currentAmulet = item;
			OnPropertiesChanged();
		}
	}

	public void HideEquip(int categoryID, bool hide)
	{
		if (categoryID >= 0 && categoryID < 10)
		{
			EquipShowImp(categoryID, (!hide) ? 1 : 2);
		}
	}

	public bool IsLimitCount(int count)
	{
		if (!GameProperties.IsLimitCount)
		{
			return false;
		}
		if (count > GameProperties.LimitCount)
		{
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg13", GameProperties.LimitCount));
			return true;
		}
		return false;
	}

	public bool IsLimitMoney(int count)
	{
		if (!GameProperties.IsLimitMoney)
		{
			return false;
		}
		if (count > GameProperties.LimitMoney)
		{
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg14", GameProperties.LimitMoney));
			return true;
		}
		return false;
	}

	public bool IsLimitAuction()
	{
		if (!GameProperties.IsLimitAuction)
		{
			return false;
		}
		if (Extra.Info.FreeAddAutionCount >= GameProperties.LimitAuction)
		{
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg15", GameProperties.LimitAuction));
			return true;
		}
		Extra.Info.FreeAddAutionCount++;
		return false;
	}

	public bool IsLimitMail()
	{
		if (!GameProperties.IsLimitMail)
		{
			return false;
		}
		if (Extra.Info.FreeSendMailCount >= GameProperties.LimitMail)
		{
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg16", GameProperties.LimitMail));
			return true;
		}
		Extra.Info.FreeSendMailCount++;
		return false;
	}

	public bool IsLimitPresentGoods()
	{
		if (!GameProperties.IsLimitPresentGoods)
		{
			return false;
		}
		if (Extra.Info.FreePresentGoodsCount >= GameProperties.LimitMail)
		{
			SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg20", GameProperties.LimitPresentGoods));
			return true;
		}
		Extra.Info.FreePresentGoodsCount++;
		return false;
	}

	public void ApertureEquip(int level)
	{
		EquipShowImp(0, (level < 5) ? 1 : ((level < 7) ? 2 : 3));
	}

	private void EquipShowImp(int categoryId, int para)
	{
		UpdateHide((int)((double)m_character.Hide + Math.Pow(10.0, categoryId) * (double)(para - m_character.Hide / (int)Math.Pow(10.0, categoryId) % 10)));
	}

	public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int spareMoney)
	{
		LogMgr.LogMoneyAdd((LogMoneyType)masterType, (LogMoneyType)sonType, userId, moneys, spareMoney, 0, 0, 0, 0, "", 0, 0);
	}

	public bool Login()
	{
		if (WorldMgr.AddPlayer(m_character.ID, this))
		{
			try
			{
				if (LoadFromDatabase())
				{
					Out.SendLoginSuccess();
					if (GameProperties.VERSION >= 11000)
					{
						Out.SendPolarRegion(isOpen: true);
					}
					Out.SendUpdatePublicPlayer(PlayerCharacter, BattleData.MatchInfo);
					Out.SendWeaklessGuildProgress(PlayerCharacter);
					Out.SendNecklaceStrength(PlayerCharacter);
					Out.SendMissionEnergy(Extra.Info);
					Out.SendUpdateOneKeyFinish(PlayerCharacter);
					Out.SendDateTime();
					Out.SendOpenCloseBraveDoor(this, isOpen: true);
					Out.SendDailyAward(PlayerCharacter);
					Out.SendUserSyncEquipGhost(this);
					UserWonderFulActivityManager = new UserWonderFulActivityManager(this);
					if (PlayerCharacter.MountsType != 0)
					{
						Out.SendchangeHorse(PlayerCharacter.MountsType);
					}
					int iD = PlayerCharacter.ID;
					Out.SendLuckStoneEnable(iD);
					Out.SendActivityList(iD);
					Out.SendOpenVIP(PlayerCharacter);
					Out.SendKingBlessMain(Extra);
					Out.SendDeedMain(Extra);
					SendPkgLimitGrate();
					Out.SendGetSpree(PlayerCharacter);
					Out.SendUpdateAllData(PlayerCharacter);
					if (GameMgr.CampBattle.IsOpenCampBattle)
					{
						SendTCP(GameMgr.CampBattle.CampBattleOpenClosePkg());
					}
					if (GameProperties.IsPromotePackageOpen)
					{
						Out.SendGrowthPackageIsOpen(iD);
						Out.SendGrowthPackageOpen(iD, Actives.Info.AvailTime);
					}
					Out.SendUpdateAreaInfo();
					Out.SendEdictumVersion();
					Roulette.SendRoulette();
					LoadMarryMessage();
					Actives.SendEvent();
					TrailElite.SendTrailEliteInfo();
					if (GameProperties.VERSION < 11000)
					{
						Dice.SendDiceActiveOpen();
					}
					AvatarBag.UpdateInfo();
					BeadBag.SendPlayerDrill();
					Horse.UpdateHorsePacket();
					MagicHouse.Init();
					Rank.SendUserRanks();
					Farm.LoadFarmLand();
					PetBag.UpdateEatPets();
					Out.SendPvePowerBuffResponse(Extra.PvePowerBuff);
					Out.SendUpdateSoulCrystal(Extra.Info);
					SetupProcessor();
					m_playerState = ePlayerState.Online;
					EquipBag.UpdatePlayerProperties();
					GameServer.Instance.LoginServer.SendGetLightriddleInfo(m_character.ID);
					GSPacketIn gSPacketIn = new GSPacketIn(68);
					gSPacketIn.WriteByte(38);
					gSPacketIn.WriteInt(PetBag.ItemAwakens.Count);
					for (int i = 0; i < PetBag.ItemAwakens.Count; i++)
					{
						gSPacketIn.WriteInt(PetBag.ItemAwakens[i].itemId);
						gSPacketIn.WriteInt(PetBag.ItemAwakens[i].skillId1);
						gSPacketIn.WriteInt(PetBag.ItemAwakens[i].skillId2);
						gSPacketIn.WriteInt(PetBag.ItemAwakens[i].petId);
					}
					SendTCP(gSPacketIn);
					Out.SendPetLacrarInfo(this);
					if (GameProperties.VERSION >= 11000)
					{
						DevilTurnMgr.Out.SendDevilTurnOpen(this);
					}
					TeamMgr.Out.SendUpdateDayActive(this);
					TeamMgr.Out.SendOpenClose(this);
					GSPacketIn gSPacketIn2 = new GSPacketIn(341);
					gSPacketIn2.WriteByte(1);
					gSPacketIn2.WriteBoolean(val: true);
					SendTCP(gSPacketIn2);
					ConsortiaTaskMgr.AddPlayer(this);
					return true;
				}
				WorldMgr.RemovePlayer(m_character.ID);
			}
			catch (Exception exception)
			{
				log.Error("Error Login!", exception);
			}
		}
		return false;
	}

	public void SendPkgLimitGrate()
	{
		int iD = PlayerCharacter.ID;
		if (PlayerCharacter.Grade >= 20)
		{
			if (RoomMgr.WorldBossRoom.worldOpen)
			{
				Out.SendOpenWorldBoss(X, Y);
			}
			if (ActiveSystemMgr.IsLeagueOpen)
			{
				Out.SendLeagueNotice(iD, BattleData.MatchInfo.restCount, BattleData.maxCount, 1);
			}
			else
			{
				Out.SendLeagueNotice(iD, BattleData.MatchInfo.restCount, BattleData.maxCount, 2);
			}
			if (ActiveSystemMgr.IsFightFootballTime)
			{
				Out.SendFightFootballTimeOpenClose(iD, result: true);
			}
			Out.SendSevenDoubleOpenClose(iD, result: true);
		}
		if (PlayerCharacter.Grade >= 15)
		{
			if (ActiveSystemMgr.IsBattleGoundOpen)
			{
				Out.SendBattleGoundOpen(iD);
			}
			if (DragonBoatMgr.IsDragonBoatOpen())
			{
				Out.SendDragonBoat(PlayerCharacter);
			}
			if (ActiveSystemMgr.LanternriddlesOpen)
			{
				Out.SendLanternriddlesOpen(iD, isOpen: true);
			}
		}
		if (PlayerCharacter.Grade >= 10 && Actives.IsChristmasOpen())
		{
			Out.SendOpenOrCloseChristmas(Actives.Christmas.lastPacks, Actives.IsChristmasOpen());
		}
		if (PlayerCharacter.Grade >= 13 && Actives.IsPyramidOpen())
		{
			Out.SendPyramidOpenClose(Actives.PyramidConfig);
		}
		if (Actives.IsYearMonsterOpen())
		{
			Out.SendCatchBeastOpen(iD, isOpen: true);
		}
		Out.SendOpenEntertainmentMode();
		Out.SendUesedFinishTime(PlayerCharacter.uesedFinishTime);
		Out.SendSignBuffInfo(m_actives.Info);
		Out.SendGMBuff(m_actives.Info);
		if (PlayerCharacter.Grade >= 15)
		{
			Out.SendOpenWorshipTheMoon(Actives.IsWorshipTheMoonOpen());
		}
		SendUpdatePlayerFigSpirit();
		Out.SendTotemUpgradeInfo(m_character.GetTotemUpgrade());
	}

	public void SendUpdatePlayerFigSpirit()
	{
		if (PlayerCharacter.Grade >= 30)
		{
			Out.SendPlayerFigSpiritinit(PlayerCharacter.ID, GemStone);
		}
	}

	public void SendUpdatePublicPlayer()
	{
		Out.SendUpdatePublicPlayer(PlayerCharacter, BattleData.MatchInfo);
	}

	public void LoadMarryMessage()
	{
		Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		MarryApplyInfo[] playerMarryApply = playerBussiness.GetPlayerMarryApply(PlayerCharacter.ID);
		if (playerMarryApply == null)
		{
			return;
		}
		MarryApplyInfo[] array = playerMarryApply;
		foreach (MarryApplyInfo marryApplyInfo in array)
		{
			switch (marryApplyInfo.ApplyType)
			{
			case 1:
				Out.SendPlayerMarryApply(this, marryApplyInfo.ApplyUserID, marryApplyInfo.ApplyUserName, marryApplyInfo.LoveProclamation, marryApplyInfo.ID);
				break;
			case 2:
				Out.SendMarryApplyReply(this, marryApplyInfo.ApplyUserID, marryApplyInfo.ApplyUserName, marryApplyInfo.ApplyResult, isApplicant: true, marryApplyInfo.ID);
				if (!marryApplyInfo.ApplyResult)
				{
					Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
				}
				break;
			case 3:
				Out.SendPlayerDivorceApply(this, result: true, isProposer: false);
				break;
			}
		}
	}

	public void AddLoadingScore(int value)
	{
		if (PlayerCharacter != null)
		{
			PlayerCharacter.LoadingScore += value;
		}
	}

	public void ChargeToUser()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		int money = 0;
		playerBussiness.ChargeToUser(m_character.UserName, ref money, m_character.NickName);
		if (money <= 0)
		{
			return;
		}
		bool flag = false;
		if (GameProperties.IsDDTMoneyActive)
		{
			AddGiftToken(money);
			if (money > 0)
			{
				flag = SendMailToUser(playerBussiness, LanguageMgr.GetTranslation("Charge.DDTMoney.Content", money), LanguageMgr.GetTranslation("Charge.DDTMoney.Title"), eMailType.Manage);
			}
		}
		else
		{
			int money2 = m_character.Money;
			UpdateUserMoney(money, 1);
			flag = SendMailToUser(playerBussiness, LanguageMgr.GetTranslation("Charge.Money.Content", money), LanguageMgr.GetTranslation("Charge.Money.Title"), eMailType.Manage);
			LogAddMoney(AddMoneyType.Charge, AddMoneyType.Charge_RMB, PlayerCharacter.ID, money, PlayerCharacter.Money);
			AddMoney(money);
		}
		if (flag)
		{
			Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
		}
	}

	public virtual bool LoadFromDatabase()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(m_character.ID);
		if (userSingleByUserID == null)
		{
			Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid"));
			Client.Disconnect();
			return false;
		}
		m_character = userSingleByUserID;
		m_character.Texp = playerBussiness.GetUserTexpInfoSingle(m_character.ID);
		m_character.PveEpicPermission = "1-2-3-4-5-6-7-8-9-10-11-12-13";
		m_character.ZoneID = ZoneId;
		m_character.ZoneName = ZoneName;
		if (userSingleByUserID.Grade >= 20)
		{
			LoadGemStone(playerBussiness);
		}
		HappyRechargeData = playerBussiness.GetHappyRechargeData(PlayerCharacter.ID) ?? new HappyRechargeInfo
		{
			UserID = PlayerCharacter.ID,
			LotteryCount = 0,
			LotteryTicket = 0
		};
		if (LogMgr.IsExitLogMoney(m_character.ID, m_character.Money))
		{
			m_character.Money = LogMgr.GetMoney(m_character.ID);
		}
		m_equipGhostList = JsonConvert.DeserializeObject<Dictionary<string, UserEquipGhostInfo>>(PlayerCharacter.GhostEquipList) ?? new Dictionary<string, UserEquipGhostInfo>();
		BattleData.LoadFromDatabase();
		KingGrade.LoadFromDatabase();
		TrailElite.LoadFromDatabase();
		userSingleByUserID.explorerManualInfo = playerBussiness.InitExplorerManualInfo(m_character.ID);
		userSingleByUserID.explorerManualInfo.activesPage = playerBussiness.InitExplorerManualInfoPages(m_character.ID);
		userSingleByUserID.explorerManualInfo.debris = playerBussiness.InitExplorerManualDebris(m_character.ID);
		ChargeToUser();
		int[] updatedSlots = new int[3] { 0, 1, 2 };
		Out.SendUpdateInventorySlot(FightBag, updatedSlots);
		UpdateWeaklessGuildProgress();
		UpdateItemForUser(1);
		CheckOpenGuardCore();
		ResetTimeBox();
		UpdatePet(m_petBag.GetPetIsEquip());
		UpdatePetFrom();
		m_pvepermissions = (string.IsNullOrEmpty(m_character.PvePermission) ? InitPvePermission() : m_character.PvePermission.ToCharArray());
		LoadPvePermission();
		_friends = new Dictionary<int, int>();
		_friends = playerBussiness.GetFriendsIDAll(m_character.ID);
		_viFarms = new List<int>();
		m_character.State = 1;
		if (!m_character.isAttest)
		{
			m_character.isAttest = true;
		}
		ElfMgr.Out.SendElfCoin(this);
		UpdateLevel();
		ClearStoreBag();
		ClearCaddyBag();
		Reset(saveToDb: true);
		return true;
	}

	public void SaveEquipGhost()
	{
		lock (m_equipGhostList)
		{
			m_character.GhostEquipList = JsonConvert.SerializeObject(m_equipGhostList);
		}
	}

	public void AddEquipGhost(UserEquipGhostInfo equipGhost)
	{
		lock (m_equipGhostList)
		{
			if (!m_equipGhostList.ContainsKey(equipGhost.BagType + "_" + equipGhost.Place))
			{
				m_equipGhostList.Add(equipGhost.BagType + "_" + equipGhost.Place, equipGhost);
			}
		}
	}

	public UserEquipGhostInfo GetGhostEquip(int bagType, int place)
	{
		lock (m_equipGhostList)
		{
			if (m_equipGhostList.ContainsKey(bagType + "_" + place))
			{
				return m_equipGhostList[bagType + "_" + place];
			}
			return null;
		}
	}

	public List<UserEquipGhostInfo> GetAllEquipGhost()
	{
		List<UserEquipGhostInfo> list = new List<UserEquipGhostInfo>();
		lock (m_equipGhostList)
		{
			foreach (UserEquipGhostInfo value in m_equipGhostList.Values)
			{
				list.Add(value);
			}
		}
		return list;
	}

	private void CheckVIPGiftWeek()
	{
		if (!m_character.IsVIPOldPlayerScan)
		{
			m_character.IsVIPOldPlayerScan = true;
			if (m_character.VIPLevel >= 1)
			{
				List<int> list = GameProperties.VIPLevelGiftList();
				List<ItemInfo> list2 = new List<ItemInfo>();
				for (int i = 0; i < m_character.VIPLevel; i++)
				{
					if (list[i] > 0)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list[i]), 1, 103);
						itemInfo.IsBinds = true;
						list2.Add(itemInfo);
					}
				}
				if (list2.Count > 0)
				{
					SendItemsToMail(list2, LanguageMgr.GetTranslation("GameServer.VIPRePackageLevel.Mail.Content", m_character.VIPLevel), LanguageMgr.GetTranslation("GameServer.VIPRePackageLevel.Mail.Title"), eMailType.BuyItem);
				}
			}
		}
		if (m_character.VIPGiftWeekDate.GetWeekOfYear() != DateTime.Now.GetWeekOfYear())
		{
			List<int> list3 = GameProperties.VIPSendFireRedList();
			if (m_character.VIPLevel <= list3.Count && list3[m_character.VIPLevel - 1] > 0)
			{
				m_character.VIPGiftWeekDate = DateTime.Now;
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(list3[m_character.VIPLevel - 1]), 1, 103);
				itemInfo2.IsBinds = true;
				SendItemsToMail(new List<ItemInfo> { itemInfo2 }, LanguageMgr.GetTranslation("GameServer.VIPPackageWeek.Mail.Content", m_character.VIPLevel), LanguageMgr.GetTranslation("GameServer.VIPPackageWeek.Mail.Title", m_character.VIPLevel), eMailType.BuyItem);
			}
		}
	}

	public void Reset(bool saveToDb)
	{
		ChecVipkExpireDay();
		if (GameProperties.VERSION >= 8900)
		{
			CheckVIPGiftWeek();
		}
		if (m_character.Texp != null && m_character.Texp.IsValidadteTexp())
		{
			m_character.Texp.texpCount = 0;
			m_character.Texp.magicTexpCount = 0;
			m_character.Texp.SecondTexpCount = 0;
		}
		if (m_character.totemId > TotemMgr.MaxTotem())
		{
			m_character.totemId = TotemMgr.MaxTotem();
		}
		if (saveToDb)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.UpdatePlayer(PlayerCharacter);
			}
		}
	}

	public bool UpdatePlayer()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		playerBussiness.UpdatePlayer(PlayerCharacter);
		return true;
	}

	public void AccumulativeUpdate()
	{
		if (m_character.accumulativeLoginDays <= 7)
		{
			if (m_character.accumulativeLoginDays == 0)
			{
				m_character.accumulativeLoginDays = 1;
			}
			else
			{
				m_character.accumulativeLoginDays++;
			}
		}
	}

	public void CompleteAllBuriedQuest()
	{
		QuestInfo[] allBuriedQuest = QuestMgr.GetAllBuriedQuest();
		QuestInfo[] array = allBuriedQuest;
		foreach (QuestInfo questInfo in array)
		{
			if (questInfo == null)
			{
				continue;
			}
			List<QuestConditionInfo> questCondiction = QuestMgr.GetQuestCondiction(questInfo);
			foreach (QuestConditionInfo item in questCondiction)
			{
				OnQuestOneKeyFinishEvent(item);
			}
		}
	}

	public void SendConsortiaBossOpenClose(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129, PlayerCharacter.ID);
		gSPacketIn.WriteByte(31);
		gSPacketIn.WriteByte((byte)type);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaBossInfo(ConsortiaInfo info)
	{
		RankingPersonInfo rankingPersonInfo = null;
		List<RankingPersonInfo> list = new List<RankingPersonInfo>();
		foreach (RankingPersonInfo value in info.RankList.Values)
		{
			if (value.Name == PlayerCharacter.NickName)
			{
				rankingPersonInfo = value;
			}
			else
			{
				list.Add(value);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129, PlayerCharacter.ID);
		gSPacketIn.WriteByte(30);
		gSPacketIn.WriteByte((byte)info.bossState);
		gSPacketIn.WriteBoolean(rankingPersonInfo != null);
		if (rankingPersonInfo != null)
		{
			gSPacketIn.WriteInt(rankingPersonInfo.ID);
			gSPacketIn.WriteInt(rankingPersonInfo.TotalDamage);
			gSPacketIn.WriteInt(rankingPersonInfo.Honor);
			gSPacketIn.WriteInt(rankingPersonInfo.Damage);
		}
		gSPacketIn.WriteByte((byte)list.Count);
		foreach (RankingPersonInfo item in list)
		{
			gSPacketIn.WriteString(item.Name);
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteInt(item.TotalDamage);
			gSPacketIn.WriteInt(item.Honor);
			gSPacketIn.WriteInt(item.Damage);
		}
		gSPacketIn.WriteByte((byte)info.extendAvailableNum);
		gSPacketIn.WriteDateTime(info.endTime);
		gSPacketIn.WriteInt(info.callBossLevel);
		SendTCP(gSPacketIn);
	}

	public GSPacketIn UpdateGoodsCount()
	{
		return Out.SendUpdateGoodsCount(PlayerCharacter, null, null);
	}

	public void LoadGemStone(PlayerBussiness db)
	{
		lock (m_GemStone)
		{
			m_GemStone = db.GetSingleGemStones(m_character.ID);
			if (m_GemStone.Count == 0)
			{
				List<int> list = new List<int> { 11, 5, 2, 3, 13 };
				List<int> list2 = new List<int> { 100002, 100003, 100001, 100004, 100005 };
				for (int i = 0; i < list.Count; i++)
				{
					UserGemStone item = new UserGemStone
					{
						ID = 0,
						UserID = m_character.ID,
						FigSpiritId = list2[i],
						FigSpiritIdValue = "0,0,0|0,0,1|0,0,2",
						EquipPlace = list[i]
					};
					m_GemStone.Add(item);
					db.AddUserGemStone(item);
				}
			}
		}
	}

	public UserGemStone GetGemStone(int place)
	{
		return m_GemStone.FirstOrDefault((UserGemStone g) => place == g.EquipPlace);
	}

	public void UpdateGemStone(int place, UserGemStone gem)
	{
		lock (m_GemStone)
		{
			for (int i = 0; i < m_GemStone.Count; i++)
			{
				if (place == m_GemStone[i].EquipPlace)
				{
					m_GemStone[i] = gem;
					break;
				}
			}
		}
	}

	public void UpdateItemForUser(object state)
	{
		m_horse.LoadFromDatabase();
		m_beadBag.LoadFromDatabase();
		m_magicStone.LoadFromDatabase();
		m_horseAmulet.LoadFromDatabase();
		m_magicHouse.LoadFromDatabase();
		m_farmBag.LoadFromDatabase();
		m_petBag.LoadFromDatabase();
		m_minesBag.LoadFromDatabase();
		m_cardBag.LoadFromDatabase();
		m_cardAchievementInventory.LoadFromDatabase(m_character.ID);
		LoadPlayerInfoFromDatabase();
		m_rank.LoadFromDatabase();
		m_equipBag.LoadFromDatabase();
		m_avatarBag.LoadFromDatabase();
		m_propBag.LoadFromDatabase();
		m_storeBag.LoadFromDatabase();
		m_consortiaBag.LoadFromDatabase();
		m_questInventory.LoadFromDatabase(m_character.ID);
		m_achievementInventory.LoadFromDatabase(m_character.ID);
		m_bufferList.LoadFromDatabase(m_character.ID);
		m_treasure.LoadFromDatabase();
		m_dice.LoadFromDatabase();
		m_actives.LoadFromDatabase();
		m_extra.LoadFromDatabase();
		m_bank.LoadFromDatabase();
		m_mines.LoadFromDatabase();
		m_marks.LoadFromDatabase();
		m_RoomBordenBag.LoadFromDatabase();
	}

	public void LoadPlayerInfoFromDatabase()
	{
		PlayerData playerData = new PlayerData();
		PlayerCharacter.ElementPro = playerData.Sys_Users_ElementPro.FirstOrDefault((Sys_Users_ElementPro p) => p.UserID == PlayerId);
		if (PlayerCharacter.ElementPro == null)
		{
			PlayerCharacter.ElementPro = new Sys_Users_ElementPro();
			PlayerCharacter.ElementPro.UserID = PlayerId;
			playerData.Sys_Users_ElementPro.Add(PlayerCharacter.ElementPro);
		}
		PlayerCharacter.SecondPro = playerData.Sys_Users_SecondPro.FirstOrDefault((Sys_Users_SecondPro p) => p.UserID == PlayerId);
		if (PlayerCharacter.SecondPro == null)
		{
			PlayerCharacter.SecondPro = new Sys_Users_SecondPro();
			PlayerCharacter.SecondPro.UserID = PlayerId;
			playerData.Sys_Users_SecondPro.Add(PlayerCharacter.SecondPro);
		}
		PlayerCharacter.CardInfo = playerData.Sys_Users_CardInfo.FirstOrDefault((Sys_Users_CardInfo p) => p.UserID == PlayerCharacter.ID);
		if (PlayerCharacter.CardInfo == null)
		{
			PlayerCharacter.CardInfo = new Sys_Users_CardInfo();
			PlayerCharacter.CardInfo.UserID = PlayerCharacter.ID;
			PlayerCharacter.CardInfo.CardLevel = 1;
			PlayerCharacter.CardInfo.Point = 0;
			PlayerCharacter.CardInfo.Armor = 0;
			PlayerCharacter.CardInfo.Crits = 0;
			PlayerCharacter.CardInfo.Trick = 0;
			PlayerCharacter.CardInfo.Viole = 0;
			PlayerCharacter.CardInfo.Attack = 0;
			PlayerCharacter.CardInfo.Defence = 0;
			PlayerCharacter.CardInfo.Agility = 0;
			PlayerCharacter.CardInfo.Lucky = 0;
			PlayerCharacter.CardInfo.SuitLevel = 1;
			PlayerCharacter.CardInfo.CardMainStr = "";
			PlayerCharacter.CardInfo.CardSuitStr = "";
			playerData.Sys_Users_CardInfo.Add(PlayerCharacter.CardInfo);
		}
		PlayerCharacter.ActiveCards = playerData.Sys_Users_CardActive.Where((Sys_Users_CardActive p) => p.UserID == PlayerId).ToList();
		PlayerCharacter.HonorInfo = playerData.Sys_Users_Honor.FirstOrDefault((Sys_Users_Honor p) => p.UserID == PlayerCharacter.ID);
		if (PlayerCharacter.HonorInfo == null)
		{
			PlayerCharacter.HonorInfo = new Sys_Users_Honor();
			PlayerCharacter.HonorInfo.UserID = PlayerCharacter.ID;
			PlayerCharacter.HonorInfo.HonorLevel = 1;
			PlayerCharacter.HonorInfo.Count = 3;
			PlayerCharacter.HonorInfo.GoodPeopleCardTime = DateTime.Now;
			playerData.Sys_Users_Honor.Add(PlayerCharacter.HonorInfo);
		}
		PlayerCharacter.GourdExpInfo = playerData.Sys_Users_GourdExp.FirstOrDefault((Sys_Users_GourdExp p) => p.UserID == PlayerCharacter.ID);
		if (PlayerCharacter.GourdExpInfo == null)
		{
			PlayerCharacter.GourdExpInfo = new Sys_Users_GourdExp();
			PlayerCharacter.GourdExpInfo.UserID = PlayerCharacter.ID;
			playerData.Sys_Users_GourdExp.Add(PlayerCharacter.GourdExpInfo);
		}
		PlayerCharacter.DevilTurn = playerData.Sys_Users_DevilTurn.FirstOrDefault((Sys_Users_DevilTurn p) => p.UserID == PlayerCharacter.ID);
		if (PlayerCharacter.DevilTurn == null)
		{
			PlayerCharacter.DevilTurn = new Sys_Users_DevilTurn();
			PlayerCharacter.DevilTurn.UserID = PlayerCharacter.ID;
			PlayerCharacter.DevilTurn.getGiftProgress = string.Empty;
			PlayerCharacter.DevilTurn.freeTimes = GameProperties.DevilTreasureFreeLotteryCount;
			PlayerCharacter.DevilTurn.freeDate = DateTime.Now;
			playerData.Sys_Users_DevilTurn.Add(PlayerCharacter.DevilTurn);
		}
		PlayerCharacter.DevilTurnRank = playerData.Sys_Users_DevilTurn_Rank.FirstOrDefault((Sys_Users_DevilTurn_Rank p) => p.UserID == PlayerCharacter.ID);
		if (PlayerCharacter.DevilTurnRank == null)
		{
			PlayerCharacter.DevilTurnRank = new Sys_Users_DevilTurn_Rank();
			PlayerCharacter.DevilTurnRank.UserID = PlayerCharacter.ID;
			PlayerCharacter.DevilTurnRank.NickName = PlayerCharacter.NickName;
			PlayerCharacter.DevilTurnRank.Score = 0;
			PlayerCharacter.DevilTurnRank.ServerId = GameServer.Instance.Configuration.ServerID;
			PlayerCharacter.DevilTurnRank.ServerName = GameServer.Instance.Configuration.ServerName;
			playerData.Sys_Users_DevilTurn_Rank.Add(PlayerCharacter.DevilTurnRank);
		}
		PlayerCharacter.ListBoxs = playerData.Sys_Users_ListBox.Where((Sys_Users_ListBox p) => p.UserID == PlayerId).ToList();
		if (PlayerCharacter.ListBoxs.Count == 0)
		{
			PlayerCharacter.ListBoxs.Add(new Sys_Users_ListBox
			{
				UserID = PlayerId,
				BoxId = 12711,
				Count = 0,
				OpenDate = Convert.ToDateTime("1970-01-01")
			});
			PlayerCharacter.ListBoxs.Add(new Sys_Users_ListBox
			{
				UserID = PlayerId,
				BoxId = 12712,
				Count = 0,
				OpenDate = Convert.ToDateTime("1970-01-01")
			});
			PlayerCharacter.ListBoxs.Add(new Sys_Users_ListBox
			{
				UserID = PlayerId,
				BoxId = 12713,
				Count = 0,
				OpenDate = Convert.ToDateTime("1970-01-01")
			});
		}
		PlayerCharacter.HonorRecords = playerData.Sys_Users_HonorRecord.Where((Sys_Users_HonorRecord p) => p.UserID == PlayerId).ToList();
		EmblemInfos = playerData.Sys_Users_Emblem.Where((Sys_Users_Emblem p) => p.UserID == PlayerId && p.isExist).ToList();
		ScrollInfos = playerData.Sys_Users_Scroll.Where((Sys_Users_Scroll p) => p.UserID == PlayerId).ToList();
		PlayerCharacter.ElfInfo = playerData.Sys_Users_Elf.Where((Sys_Users_Elf p) => p.UserID == PlayerId).ToList();
		PlayerCharacter.SkillBooks = playerData.Sys_Users_SkillBooks.Where((Sys_Users_SkillBooks p) => p.UserID == PlayerId).ToList();
		UpdateCardInfo();
		ElfMgr.Out.SendUpdateElf(this);
		ElfMgr.Out.SendSkillBooks(this);
		if (GameProperties.VERSION >= 12100)
		{
            ElfMgr.Out.SendUpdateElfEquipLvInfo(this);
            ElfMgr.Out.SendUpdateElfEquipInfo(this);
        }
		EmblemMgr.Out.SendUpdateEmblem(this);
		EmblemMgr.SendProInfoHandler(this);
		EmblemMgr.Out.SendUpdateScroll(this);
		PlayerCharacter.Team = (from q in playerData.Sys_Users_Team
			join p in playerData.Sys_Users_Team_User on q.teamID equals p.teamID
			where p.UserID == PlayerId
			select q).FirstOrDefault();
		if (PlayerCharacter.Team == null)
		{
			PlayerCharacter.Team = new Sys_Users_Team();
			PlayerCharacter.Team.teamID = 0;
		}
		PlayerCharacter.TeamUser = playerData.Sys_Users_Team_User.FirstOrDefault((Sys_Users_Team_User p) => p.UserID == PlayerId);
		PlayerCharacter.TeamSelfActive = playerData.Sys_Users_TeamSelfActive.Where((Sys_Users_TeamSelfActive p) => p.UserID == PlayerId).ToList();
		PlayerCharacter.TeamDayActive = playerData.Sys_Users_TeamDayActive.FirstOrDefault((Sys_Users_TeamDayActive p) => p.UserID == PlayerId);
		if (PlayerCharacter.TeamDayActive == null)
		{
			PlayerCharacter.TeamDayActive = new Sys_Users_TeamDayActive();
			PlayerCharacter.TeamDayActive.UserID = PlayerId;
			PlayerCharacter.TeamDayActive.selfActive = 0;
			PlayerCharacter.TeamDayActive.selfAllActive = 0;
			PlayerCharacter.TeamDayActive.selfScore = 0;
		}
		PlayerCharacter.SwornInfo = playerData.Sys_Users_Sworn.FirstOrDefault((Sys_Users_Sworn p) => p.UserID == PlayerId);
		if (PlayerCharacter.SwornInfo == null)
		{
			PlayerCharacter.SwornInfo = new Sys_Users_Sworn();
			PlayerCharacter.SwornInfo.UserID = PlayerCharacter.ID;
			PlayerCharacter.SwornInfo.swornId = 1;
			playerData.Sys_Users_Sworn.Add(PlayerCharacter.SwornInfo);
		}
		PlayerCharacter.ButterflyInfo = playerData.Sys_Users_Butterfly.FirstOrDefault((Sys_Users_Butterfly p) => p.UserID == PlayerId);
		if (PlayerCharacter.ButterflyInfo == null)
		{
			PlayerCharacter.ButterflyInfo = new Sys_Users_Butterfly();
			PlayerCharacter.ButterflyInfo.UserID = PlayerCharacter.ID;
			PlayerCharacter.ButterflyInfo.isHasButterfly = 1;
			PlayerCharacter.ButterflyInfo.level = 1;
			PlayerCharacter.ButterflyInfo.MoonAll = 100;
			PlayerCharacter.ButterflyInfo.taskDate = DateTime.Now.AddDays(-7.0);
			playerData.Sys_Users_Butterfly.Add(PlayerCharacter.ButterflyInfo);
		}
		PlayerCharacter.LabyrinthInfo = playerData.Sys_Users_Labyrinth.FirstOrDefault((Sys_Users_Labyrinth p) => p.UserID == PlayerId);
		if (PlayerCharacter.LabyrinthInfo == null)
		{
			PlayerCharacter.LabyrinthInfo = new Sys_Users_Labyrinth
			{
				UserID = PlayerCharacter.ID,
				maxWeekFinishLevel = 0,
				currentFinishLevel = 1,
				sweepTotalTimeRemaining = 0,
				sweepTotalExp = 0,
				sweepTotalTime = 0,
				sweepNeedMoney = 5000,
				isUseDoubleProp = false,
				isPlayGame = false,
				isSweep = false,
				isMultiple = false,
				buffList = "",
				nowContinueCount = 2,
				maxFinishLevelTurn = 0,
				maxFinishLevel = 0,
				tryAgainMoney = 5000,
				LastDate = DateTime.Now,
				ProcessAward = ""
			};
		}
		GmActivityConditions = playerData.User_GmActivityCondition.Where((User_GmActivityCondition p) => p.UserID == PlayerId).ToList();
		GmActivityRewards = playerData.User_GmActivityReward.Where((User_GmActivityReward p) => p.UserID == PlayerId).ToList();
		PlayerCharacter.AcademyInfo = playerData.Sys_Users_Academy.FirstOrDefault((Sys_Users_Academy p) => p.UserID == PlayerId);
		if (PlayerCharacter.AcademyInfo == null)
		{
			PlayerCharacter.AcademyInfo = new Sys_Users_Academy();
			PlayerCharacter.AcademyInfo.UserID = PlayerCharacter.ID;
			PlayerCharacter.AcademyInfo.IsFirst = true;
			PlayerCharacter.AcademyInfo.MasterOrApprentices = "";
			PlayerCharacter.AcademyInfo.HonourOfMaster = "";
			PlayerCharacter.AcademyInfo.FreezesDate = DateTime.Now.AddDays(-7.0);
			playerData.Sys_Users_Academy.Add(PlayerCharacter.AcademyInfo);
		}
		playerData.SaveChanges();
	}

	private void SavePlayerInfoToDatabase()
	{
		PlayerData playerData = new PlayerData();
		if (PlayerCharacter.CardInfo != null)
		{
			playerData.Entry(PlayerCharacter.CardInfo).State = EntityState.Modified;
		}
		if (PlayerCharacter.AcademyInfo != null)
		{
			playerData.Entry(PlayerCharacter.AcademyInfo).State = EntityState.Modified;
		}
		if (PlayerCharacter.HonorInfo != null)
		{
			playerData.Entry(PlayerCharacter.HonorInfo).State = EntityState.Modified;
		}
		if (PlayerCharacter.ElementPro != null)
		{
			playerData.Entry(PlayerCharacter.ElementPro).State = EntityState.Modified;
		}
		if (PlayerCharacter.SecondPro != null)
		{
			playerData.Entry(PlayerCharacter.SecondPro).State = EntityState.Modified;
		}
		if (PlayerCharacter.GourdExpInfo != null)
		{
			playerData.Entry(PlayerCharacter.GourdExpInfo).State = EntityState.Modified;
		}
		if (PlayerCharacter.DevilTurn != null)
		{
			playerData.Entry(PlayerCharacter.DevilTurn).State = EntityState.Modified;
		}
		if (PlayerCharacter.ButterflyInfo != null)
		{
			playerData.Entry(PlayerCharacter.ButterflyInfo).State = EntityState.Modified;
		}
		if (PlayerCharacter.DevilTurnRank != null)
		{
			playerData.Entry(PlayerCharacter.DevilTurnRank).State = EntityState.Modified;
		}
		if (PlayerCharacter.SwornInfo != null)
		{
			playerData.Entry(PlayerCharacter.SwornInfo).State = EntityState.Modified;
		}
		foreach (Sys_Users_CardActive activeCard in PlayerCharacter.ActiveCards)
		{
			if (activeCard.ID == 0)
			{
				playerData.Sys_Users_CardActive.Add(activeCard);
			}
			else
			{
				playerData.Entry(activeCard).State = EntityState.Modified;
			}
		}
		foreach (Sys_Users_HonorRecord honorRecord in PlayerCharacter.HonorRecords)
		{
			if (honorRecord.ID == 0)
			{
				playerData.Sys_Users_HonorRecord.Add(honorRecord);
			}
			else
			{
				playerData.Entry(honorRecord).State = EntityState.Modified;
			}
		}
		foreach (Sys_Users_Elf item in PlayerCharacter.ElfInfo)
		{
			if (item.ID == 0)
			{
				playerData.Sys_Users_Elf.Add(item);
			}
			else
			{
				playerData.Entry(item).State = EntityState.Modified;
			}
		}
		foreach (Sys_Users_SkillBooks skillBook in PlayerCharacter.SkillBooks)
		{
			if (skillBook.ID == 0)
			{
				playerData.Sys_Users_SkillBooks.Add(skillBook);
			}
			else
			{
				playerData.Entry(skillBook).State = EntityState.Modified;
			}
		}
		foreach (Sys_Users_Emblem emblemInfo in EmblemInfos)
		{
			if (emblemInfo.ID == 0)
			{
				playerData.Sys_Users_Emblem.Add(emblemInfo);
			}
			else
			{
				playerData.Entry(emblemInfo).State = EntityState.Modified;
			}
		}
		foreach (Sys_Users_Scroll scrollInfo in ScrollInfos)
		{
			if (scrollInfo.ID == 0)
			{
				playerData.Sys_Users_Scroll.Add(scrollInfo);
			}
			else
			{
				playerData.Entry(scrollInfo).State = EntityState.Modified;
			}
		}
		foreach (Sys_Users_ListBox listBox in PlayerCharacter.ListBoxs)
		{
			if (listBox.ID == 0)
			{
				playerData.Sys_Users_ListBox.Add(listBox);
			}
			else
			{
				playerData.Entry(listBox).State = EntityState.Modified;
			}
		}
		foreach (User_GmActivityCondition gmActivityCondition in GmActivityConditions)
		{
			if (gmActivityCondition.ID == 0)
			{
				playerData.User_GmActivityCondition.Add(gmActivityCondition);
			}
			else
			{
				playerData.Entry(gmActivityCondition).State = EntityState.Modified;
			}
		}
		foreach (User_GmActivityReward gmActivityReward in GmActivityRewards)
		{
			if (gmActivityReward.ID == 0)
			{
				playerData.User_GmActivityReward.Add(gmActivityReward);
			}
			else
			{
				playerData.Entry(gmActivityReward).State = EntityState.Modified;
			}
		}
		playerData.SaveChanges();
	}

	public void UpdateCardInfo()
	{
		int hp = 0;
		int attack = 0;
		int def = 0;
		int agi = 0;
		int luck = 0;
		int mgattack = 0;
		int mgdef = 0;
		int basedame = 0;
		int basedef = 0;
		TS_CardMain tS_CardMain = CardNewSystemMgr.CardMains.FirstOrDefault((TS_CardMain p) => p.Level == PlayerCharacter.CardInfo.CardLevel);
		if (tS_CardMain == null)
		{
			throw new NullReferenceException($"Can not found cardMain with Level = {PlayerCharacter.CardInfo.CardLevel}");
		}
		CardNewSystemMgr.GetSuitValue(this, ref hp, ref attack, ref def, ref agi, ref luck, ref mgattack, ref mgdef, ref basedame, ref basedef, addPercent: false);
		PlayerCharacter.CardInfo.CardSuitStr = $"{hp}|{attack}|{def}|{agi}|{luck}|{mgattack}|{mgdef}|0|0|0|0|0|{basedame}|{basedef}";
		PlayerCharacter.CardInfo.CardMainStr = $"{tS_CardMain.Attack + PlayerCharacter.CardInfo.Attack}|{tS_CardMain.Agility + PlayerCharacter.CardInfo.Agility}|{PlayerCharacter.CardInfo.Armor}|{PlayerCharacter.CardInfo.Trick}|{tS_CardMain.Defence + PlayerCharacter.CardInfo.Defence}|{tS_CardMain.Lucky + PlayerCharacter.CardInfo.Lucky}|{PlayerCharacter.CardInfo.Crits}|{PlayerCharacter.CardInfo.Viole}";
		EquipBag.UpdatePlayerProperties();
	}

	public int GetBaseElementProperties()
	{
		int fireAtt = 0;
		int waterAtt = 0;
		int windAtt = 0;
		int landAtt = 0;
		int fireDef = 0;
		int waterDef = 0;
		int windDef = 0;
		int landDef = 0;
		List<Sys_Users_Emblem> list = EmblemInfos.Where((Sys_Users_Emblem e) => e.equipIndex != 0 && e.equipPos != 0 && e.isExist).ToList();
		if (list != null)
		{
			EmblemMgr.CalculateProps(list, ref fireAtt, ref waterAtt, ref windAtt, ref landAtt, ref fireDef, ref waterDef, ref windDef, ref landDef);
			PlayerProp.UpadateBaseProp(isSelf: true, "FireAttack", "Emblem", fireAtt);
			PlayerProp.UpadateBaseProp(isSelf: true, "WaterAttack", "Emblem", waterAtt);
			PlayerProp.UpadateBaseProp(isSelf: true, "WindAttack", "Emblem", windAtt);
			PlayerProp.UpadateBaseProp(isSelf: true, "LandAttack", "Emblem", landAtt);
			PlayerProp.UpadateBaseProp(isSelf: true, "FireDefence", "Emblem", fireDef);
			PlayerProp.UpadateBaseProp(isSelf: true, "WaterDefence", "Emblem", waterAtt);
			PlayerProp.UpadateBaseProp(isSelf: true, "WindDefence", "Emblem", windDef);
			PlayerProp.UpadateBaseProp(isSelf: true, "LandDefence", "Emblem", landDef);
		}
		Sys_Users_Elf elf = PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight);
		if (elf != null)
		{
			Elf_Template_Info elf_Template_Info = ElfMgr.Elfs.FirstOrDefault((Elf_Template_Info e) => e.TemplateID == elf.TempID);
			if (elf_Template_Info == null)
			{
				throw new NullReferenceException($"elfTemplate is null with templateId = {elf.TempID}");
			}
			switch (elf_Template_Info.ElfType)
			{
			case 1:
				fireAtt += elf.AttackValue;
				PlayerProp.UpadateBaseProp(isSelf: true, "FireAttack", "Elf", elf.AttackValue);
				break;
			case 2:
				windAtt += elf.AttackValue;
				PlayerProp.UpadateBaseProp(isSelf: true, "WindAttack", "Elf", elf.AttackValue);
				break;
			case 3:
				landAtt += elf.AttackValue;
				PlayerProp.UpadateBaseProp(isSelf: true, "LandAttack", "Elf", elf.AttackValue);
				break;
			case 4:
				waterAtt += elf.AttackValue;
				PlayerProp.UpadateBaseProp(isSelf: true, "WaterAttack", "Elf", elf.AttackValue);
				break;
			}
			fireDef += elf.HuoValue;
			waterDef += elf.ShuiValue;
			windDef += elf.FengValue;
			landDef += elf.TuValue;
			PlayerProp.UpadateBaseProp(isSelf: true, "FireDefence", "Elf", elf.HuoValue);
			PlayerProp.UpadateBaseProp(isSelf: true, "WaterDefence", "Elf", elf.ShuiValue);
			PlayerProp.UpadateBaseProp(isSelf: true, "WindDefence", "Elf", elf.FengValue);
			PlayerProp.UpadateBaseProp(isSelf: true, "LandDefence", "Elf", elf.TuValue);
		}
		PlayerCharacter.ElementPro.FireAttack = fireAtt;
		PlayerCharacter.ElementPro.WaterAttack = waterAtt;
		PlayerCharacter.ElementPro.WindAttack = windAtt;
		PlayerCharacter.ElementPro.LandAttack = landAtt;
		PlayerCharacter.ElementPro.FireDefence = fireDef;
		PlayerCharacter.ElementPro.WaterDefence = waterDef;
		PlayerCharacter.ElementPro.WindDefence = windDef;
		PlayerCharacter.ElementPro.LandDefence = landDef;
		return fireAtt + waterAtt + windAtt + landAtt + fireDef + waterDef + windDef + landDef;
	}

	public void GetAmuletSecondProperties(ItemInfo item, ref double Crit, ref double Tenacity, ref double SunderArmor, ref double AvoidInjury, ref double Kill, ref double WillFight, ref double ViolenceInjury, ref double Guard, ref double Speed)
	{
		if (item == null)
		{
			return;
		}
		AmuletItemInfo amuletItemInfo = AmuletItemMgr.FindAmuletItem(Amulet.StrengthenLevel);
		if (amuletItemInfo != null)
		{
			AmuletPhaseItemInfo amuletPhaseItemInfo = AmuletPhaseItemMgr.FindAmuletPhaseItem(amuletItemInfo.WashsStep);
			if (amuletPhaseItemInfo != null)
			{
				Crit = amuletPhaseItemInfo.GetProperty(1, Amulet.Crit());
				Tenacity = amuletPhaseItemInfo.GetProperty(2, Amulet.Tenacity());
				SunderArmor = amuletPhaseItemInfo.GetProperty(3, Amulet.SunderArmor());
				AvoidInjury = amuletPhaseItemInfo.GetProperty(4, Amulet.AvoidInjury());
				Kill = amuletPhaseItemInfo.GetProperty(5, Amulet.KillPow());
				WillFight = amuletPhaseItemInfo.GetProperty(6, Amulet.WillFight());
				ViolenceInjury = amuletPhaseItemInfo.GetProperty(7, Amulet.ViolenceInjury());
				Guard = amuletPhaseItemInfo.GetProperty(8, Amulet.AmuletGuard());
				Speed = (int)amuletPhaseItemInfo.GetProperty(9, Amulet.Speed());
			}
		}
	}

	public int GetBaseSecondProperties()
	{
		int blood = 0;
		int mgDef = 0;
		int num = 0;
		int tenacity = 0;
		int num2 = 0;
		int avoidInjury = 0;
		int num3 = 0;
		int trcRvt = 0;
		int num4 = 0;
		int guardian = 0;
		int num5 = 0;
		int Crit = 0;
		int Tenacity = 0;
		int SunderArmor = 0;
		int AvoidInjury = 0;
		int Kill = 0;
		int WillFight = 0;
		int ViolenceInjury = 0;
		int Speed = 0;
		int Guard = 0;
		double Crit2 = 0.0;
		double Tenacity2 = 0.0;
		double SunderArmor2 = 0.0;
		double AvoidInjury2 = 0.0;
		double Kill2 = 0.0;
		double WillFight2 = 0.0;
		double ViolenceInjury2 = 0.0;
		double Speed2 = 0.0;
		double Guard2 = 0.0;
		Sys_Users_SecondPro equipSecondProperties = Marks.GetEquipSecondProperties();
		if (Actives.Info != null)
		{
			HomeTempPracticeMgr.BuildProp(Actives.Info.promoteLevel, ref blood, ref mgDef, ref tenacity, ref avoidInjury, ref trcRvt, ref guardian);
		}
		HorseAmuletBag.GetHorseAmuletSecondProperty(ref Crit, ref Tenacity, ref SunderArmor, ref AvoidInjury, ref Kill, ref WillFight, ref ViolenceInjury, ref Guard, ref Speed);
		GetAmuletSecondProperties(Amulet, ref Crit2, ref Tenacity2, ref SunderArmor2, ref AvoidInjury2, ref Kill2, ref WillFight2, ref ViolenceInjury2, ref Guard2, ref Speed2);
		num += equipSecondProperties.Crit;
		num2 += equipSecondProperties.SunderArmor;
		num4 += equipSecondProperties.ViolenceInjury;
		num5 += equipSecondProperties.Speed;
		num3 += equipSecondProperties.Kill;
		if (PetBag.lacresPet != null)
		{
			foreach (UsersPetLacrarInfo item in PetBag.lacresPet)
			{
				if (!item.isSeal)
				{
					continue;
				}
				foreach (StoreFineSealLockInfo item2 in item.lockArr)
				{
					switch (item2.proType)
					{
					case 1:
						num += item2.proNum;
						break;
					case 3:
						num2 += item2.proNum;
						break;
					case 5:
						num3 += item2.proNum;
						break;
					case 7:
						num4 += item2.proNum;
						break;
					case 9:
						num5 += item2.proNum;
						break;
					case 39:
						PlayerCharacter.CurrentSealSkill = item2.skillId;
						break;
					}
				}
			}
		}
		if (PlayerCharacter.Texp != null)
		{
			num += ExerciseMgr.GetExercise(PlayerCharacter.Texp.TrainCritTexpExp).TrainCrit;
			num2 += ExerciseMgr.GetExercise(PlayerCharacter.Texp.TrainSunderTexpExp).TrainSunder;
			num4 += ExerciseMgr.GetExercise(PlayerCharacter.Texp.TrainCritDmgTexpExp).TrainCritDmg;
			num5 += ExerciseMgr.GetExercise(PlayerCharacter.Texp.spdTexpExp).TrainSpeed;
			num3 += ExerciseMgr.GetExercise(PlayerCharacter.Texp.TrainTricKillTexpExp).TrainTricKill;
		}
		if (PlayerCharacter.CardInfo != null)
		{
			num += PlayerCharacter.CardInfo.Crits;
			num3 += PlayerCharacter.CardInfo.Trick;
			num4 += PlayerCharacter.CardInfo.Viole;
			num2 += PlayerCharacter.CardInfo.Armor;
		}
		PlayerCharacter.SecondPro.Crit = num + Crit + (int)Crit2;
		PlayerCharacter.SecondPro.Tenacity = tenacity + Tenacity + (int)Tenacity2;
		PlayerCharacter.SecondPro.SunderArmor = num2 + SunderArmor + (int)SunderArmor2;
		PlayerCharacter.SecondPro.AvoidInjury = avoidInjury + AvoidInjury + (int)AvoidInjury2;
		PlayerCharacter.SecondPro.Kill = num3 + Kill + (int)Kill2;
		PlayerCharacter.SecondPro.WillFight = trcRvt + WillFight + (int)WillFight2;
		PlayerCharacter.SecondPro.ViolenceInjury = num4 + ViolenceInjury + (int)ViolenceInjury2;
		PlayerCharacter.SecondPro.Guard = guardian + Guard + (int)Guard2;
		PlayerCharacter.SecondPro.Speed = num5 + Speed + (int)Speed2;
		return num * 2400 + tenacity * 2000 + num2 * 2000 + avoidInjury * 2000 + num3 * 500 + trcRvt * 2000 + num4 * 1200 + guardian * 2000 + num5 * 2000;
	}

	public void saveTeamSelfActive()
	{
		PlayerData playerData = new PlayerData();
		foreach (Sys_Users_TeamSelfActive item in PlayerCharacter.TeamSelfActive)
		{
			if (item.ID == 0)
			{
				playerData.Sys_Users_TeamSelfActive.Add(item);
			}
			else
			{
				playerData.Entry(item).State = EntityState.Modified;
			}
		}
		playerData.SaveChanges();
	}

	public void saveTeamDayActive()
	{
		PlayerData playerData = new PlayerData();
		if (PlayerCharacter.TeamDayActive.ID == 0)
		{
			playerData.Sys_Users_TeamDayActive.Add(PlayerCharacter.TeamDayActive);
		}
		else
		{
			playerData.Entry(PlayerCharacter.TeamDayActive).State = EntityState.Modified;
		}
		playerData.SaveChanges();
	}

	public void resetTeamSelfActive()
	{
		if (PlayerCharacter.TeamSelfActive.FirstOrDefault() == null || !(PlayerCharacter.TeamSelfActive.FirstOrDefault().LastDate < DateTime.Now.Date))
		{
			return;
		}
		foreach (Sys_Users_TeamSelfActive item in PlayerCharacter.TeamSelfActive)
		{
			item.LastDate = DateTime.Now.AddDays(7.0);
			item.haveScore = 0;
			item.Times = 0;
		}
	}

	public bool ResetTimeBox()
	{
		if (PlayerCharacter != null && PlayerCharacter.IsValidadteTimeBox())
		{
			PlayerCharacter.VipShop = "";
			PlayerCharacter.TimeBox = DateTime.Now;
			PlayerCharacter.receiebox = 0;
			if (PlayerCharacter.damageScores > 0)
			{
				SendMessage("你的世界Boss积分已转化为经验或功勋.");
				AddGP(PlayerCharacter.damageScores);
				PlayerCharacter.damageScores = 0;
			}
			ServerData serverData = new ServerData();
			TS_Butterfly tS_Butterfly = serverData.TS_Butterfly.FirstOrDefault((TS_Butterfly i) => i.Grades == PlayerCharacter.ButterflyInfo.level);
			PlayerCharacter.ButterflyInfo.MoonAll += tS_Butterfly.DayFeeling;
			PlayerCharacter.ButterflyInfo.touchCount = 0;
			PlayerCharacter.MaxBuyHonor = 0;
			PlayerCharacter.GetSoulCount = 30;
			PlayerCharacter.uesedFinishTime = 10000;
			PlayerCharacter.horseRaceCanRaceTime = GameProperties.HorseGameEachDayMaxCount;
			PlayerCharacter.ticketNum = 1;
			PlayerCharacter.buyStiveNum = 0;
			PlayerCharacter.LoadingScore = 0;
			Farm.ResetFarmProp();
			BattleData.Reset();
			Actives.ResetActive();
			Actives.Info.activityTanabataNum = 0;
			Extra.ResetUsersExtra();
			Dice.Reset();
			m_magicHouse.Reset();
			m_RoomBordenBag.ClearVaildItem();
			PlayerCharacter.isClicked = false;
			AccumulativeUpdate();
			CompleteAllBuriedQuest();
			QuestInventory.ResetDailyQuest();
			PlayerCharacter.HonorInfo.IsGet = false;
			PlayerCharacter.HonorInfo.Count = 3;
			PlayerCharacter.TeamDayActive.selfActive = 0;
			PlayerCharacter.TeamDayActive.selfScore = 0;
			PlayerCharacter.DevilTurn.freeTimes = GameProperties.DevilTreasureFreeLotteryCount;
			PlayerCharacter.DevilTurn.freeDate = DateTime.Now;
			return true;
		}
		return false;
	}

	public void UpdatePetFrom()
	{
		List<PetFormActiveListInfo> allPetForms = PetBag.GetAllPetForms();
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(24);
		gSPacketIn.WriteInt(allPetForms.Count);
		foreach (PetFormActiveListInfo item in allPetForms)
		{
			gSPacketIn.WriteInt(item.TemplateID);
		}
		gSPacketIn.WriteInt(0);
		SendTCP(gSPacketIn);
	}

	public void SendVipMerryDiscountInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(169);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteString(GameProperties.PRICE_MARRY_ROOM.Split(',')[0] + "|" + GameProperties.PRICE_MARRY_ROOM.Split(',')[1] + "|" + GameProperties.PRICE_DIVORCED_DISCOUNT);
		gSPacketIn.WriteString(DateTime.Now.ToString("yyyy-MM-dd"));
		gSPacketIn.WriteString(DateTime.Now.ToString("yyyy-MM-dd"));
		SendTCP(gSPacketIn);
	}

	public void CheckOpenGuardCore()
	{
		SendVipMerryDiscountInfo();
		if (PlayerCharacter.Grade >= 50 && PlayerCharacter.guardCoreID == 0)
		{
			PlayerCharacter.guardCoreID = 20101;
			PlayerCharacter.guardCoreGrade = 1;
		}
	}

	public void ChecVipkExpireDay()
	{
		m_character.CanTakeVipReward = !m_character.IsVIPExpire() && m_character.IsLastVIPPackTime();
	}

	public void LastVIPPackTime()
	{
		m_character.LastVIPPackTime = DateTime.Now;
		m_character.CanTakeVipReward = false;
	}

	public void OpenVIP(int reneval_days, DateTime expireDayOut)
	{
		if (m_character.typeVIP == 0)
		{
			Extra.CreateSaveLifeBuff();
		}
		int vIPLevel = m_character.VIPLevel;
		if (vIPLevel < 6 && reneval_days == 180)
		{
			m_character.typeVIP = 1;
			m_character.VIPLevel = 6;
			m_character.VIPExp = 10000;
			m_character.VIPExpireDay = expireDayOut;
			m_character.VIPLastDate = DateTime.Now;
			m_character.VIPNextLevelDaysNeeded = 0;
			m_character.CanTakeVipReward = true;
		}
		else if (vIPLevel < 4 && reneval_days == 90)
		{
			m_character.typeVIP = 1;
			m_character.VIPLevel = 4;
			m_character.VIPExp = 2000;
			m_character.VIPExpireDay = expireDayOut;
			m_character.VIPLastDate = DateTime.Now;
			m_character.VIPNextLevelDaysNeeded = 0;
			m_character.CanTakeVipReward = true;
		}
		else
		{
			m_character.typeVIP = 1;
			m_character.VIPLevel = 1;
			m_character.VIPExp = 0;
			m_character.VIPExpireDay = expireDayOut;
			m_character.VIPLastDate = DateTime.Now;
			m_character.VIPNextLevelDaysNeeded = 0;
			m_character.CanTakeVipReward = true;
		}
	}

	public void ContinousVIP(DateTime expireDayOut)
	{
		m_character.VIPExpireDay = expireDayOut;
	}

	public bool isDoubleAward()
	{
		if (PlayerCharacter.LabyrinthInfo == null)
		{
			return false;
		}
		return PlayerCharacter.LabyrinthInfo.isUseDoubleProp;
	}

	public void OutLabyrinth(bool isWin)
	{
		if (!isWin && PlayerCharacter.LabyrinthInfo != null && PlayerCharacter.LabyrinthInfo.currentFinishLevel > 1)
		{
			SendLabyrinthTryAgain();
		}
		ResetLabyrinth();
	}

	public void ResetLabyrinth()
	{
		if (PlayerCharacter.LabyrinthInfo != null)
		{
			PlayerCharacter.LabyrinthInfo.isPlayGame = false;
		}
	}

	public UserLabyrinthInfo LoadLabyrinth(int sType)
	{
		if (m_Labyrinth == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			m_Labyrinth = playerBussiness.GetSingleLabyrinth(m_character.ID);
			if (m_Labyrinth == null)
			{
				m_Labyrinth = new UserLabyrinthInfo
				{
					UserID = m_character.ID,
					sType = sType,
					myProgress = 0,
					myRanking = 0,
					completeChallenge = true,
					isDoubleAward = false,
					currentFloor = 1,
					accumulateExp = 0,
					remainTime = 0,
					currentRemainTime = 0,
					cleanOutAllTime = 0,
					cleanOutGold = 50,
					tryAgainComplete = true,
					isInGame = false,
					isCleanOut = false,
					serverMultiplyingPower = false,
					LastDate = DateTime.Now,
					ProcessAward = InitProcessAward()
				};
				playerBussiness.AddUserLabyrinth(m_Labyrinth);
			}
			else
			{
				ProcessLabyrinthAward = m_Labyrinth.ProcessAward;
				m_Labyrinth.sType = sType;
			}
		}
		return Labyrinth;
	}

	public string InitProcessAward()
	{
		string[] array = new string[99];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = i.ToString();
		}
		ProcessLabyrinthAward = string.Join("-", array);
		return ProcessLabyrinthAward;
	}

	public string CompleteGetAward(int floor)
	{
		string[] array = new string[floor];
		for (int i = 0; i < floor; i++)
		{
			array[i] = "i";
		}
		string[] array2 = m_Labyrinth.ProcessAward.Split('-');
		string text = string.Join("-", array);
		for (int j = floor; j < array2.Length; j++)
		{
			text = text + "-" + array2[j];
		}
		return text;
	}

	public void CalculatorClearnOutLabyrinth()
	{
		if (m_Labyrinth != null)
		{
			int num = 0;
			for (int i = m_Labyrinth.currentFloor; i <= m_Labyrinth.myProgress; i++)
			{
				num += 2;
			}
			num *= 60;
			m_Labyrinth.remainTime = num;
			m_Labyrinth.currentRemainTime = num;
			m_Labyrinth.cleanOutAllTime = num;
		}
	}

	public int[] CreateExps()
	{
		int[] array = new int[99];
		int num = 660;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = num;
			num += 690;
		}
		return array;
	}

	public void UpdateLabyrinth(int floor, int mMissionInfoId, bool bigAward)
	{
		LabyrinthMgr.CollectLabyrinthReward(this, PlayerCharacter.LabyrinthInfo.maxFinishLevel < PlayerCharacter.LabyrinthInfo.currentFinishLevel + 1);
		if (PlayerCharacter.LabyrinthInfo.maxFinishLevel < PlayerCharacter.LabyrinthInfo.currentFinishLevel)
		{
			PlayerCharacter.LabyrinthInfo.maxFinishLevel = PlayerCharacter.LabyrinthInfo.currentFinishLevel;
		}
		if (PlayerCharacter.LabyrinthInfo.maxFinishLevelTurn < PlayerCharacter.LabyrinthInfo.currentFinishLevel)
		{
			PlayerCharacter.LabyrinthInfo.maxFinishLevelTurn = PlayerCharacter.LabyrinthInfo.currentFinishLevel;
		}
		if (PlayerCharacter.LabyrinthInfo.maxWeekFinishLevel < PlayerCharacter.LabyrinthInfo.currentFinishLevel)
		{
			PlayerCharacter.LabyrinthInfo.maxWeekFinishLevel = PlayerCharacter.LabyrinthInfo.currentFinishLevel;
		}
		PlayerCharacter.LabyrinthInfo.currentFinishLevel++;
		Out.SendLabyrinthUpdataInfo(this);
	}

	public List<ItemInfo> CopyDrop(int sessionId, int mMissionInfoId)
	{
		List<ItemInfo> info = null;
		DropInventory.CopyDrop(mMissionInfoId, sessionId, ref info);
		return info;
	}

	public bool MoneyDirect(int value)
	{
		AddExpVip(value / 100);
		if (GameProperties.IsDDTMoneyActive)
		{
			return MoneyDirect(MoneyType.DDTMoney, value);
		}
		return MoneyDirect(MoneyType.DDTMoney, value);
	}

	public bool MoneyDirect(MoneyType type, int value)
	{
		if (value < 0)
		{
			return false;
		}
		if (type == MoneyType.Money)
		{
			if (PlayerCharacter.Money < value)
			{
				SendInsufficientMoney(0);
				return false;
			}
			RemoveMoney(value);
			return true;
		}
		if (PlayerCharacter.DDTMoney < value)
		{
			RemoveMoney(value);
			SendMessage(LanguageMgr.GetTranslation("绑点不足，扣除点券"));
			return true;
		}
		RemoveGiftToken(value);
		return true;
	}

	public int AddCoin(int value)
	{
		long num = m_extra.Info.Coin;
		if (value <= 0)
		{
			return 0;
		}
		long num2 = num + value;
		m_extra.Info.Coin = (int)((num2 <= int.MaxValue) ? num2 : int.MaxValue);
		Out.SendUpdateCoin(m_extra.Info);
		return value;
	}

	public int RemoveCoin(int value)
	{
		if (value < 0 || value > m_extra.Info.Coin)
		{
			return 0;
		}
		m_extra.Info.Coin -= value;
		Out.SendUpdateCoin(m_extra.Info);
		return value;
	}

	public bool UpdateSummonCount(int value, int type)
	{
		if (type == 1)
		{
			switch (value)
			{
			case 1:
				if (m_extra.Info.SummonCount1 == 1)
				{
					m_extra.Info.SummonCount1 = 10;
					return true;
				}
				m_extra.Info.SummonCount1--;
				return false;
			case 10:
				return true;
			}
		}
		if (type == 2)
		{
			switch (value)
			{
			case 1:
				if (m_extra.Info.SummonCount2 == 1)
				{
					m_extra.Info.SummonCount2 = 50;
					return true;
				}
				m_extra.Info.SummonCount2--;
				return false;
			case 10:
				if (m_extra.Info.SummonCount2 <= 10)
				{
					m_extra.Info.SummonCount2 = 40 + m_extra.Info.SummonCount2;
					return true;
				}
				m_extra.Info.SummonCount2 -= 10;
				return false;
			}
		}
		return false;
	}

	public int GetSummonCount(int type)
	{
		return type switch
		{
			1 => m_extra.Info.SummonCount1, 
			2 => m_extra.Info.SummonCount2, 
			_ => 0, 
		};
	}

	public virtual bool SaveIntoDatabase()
	{
		try
		{
			m_character.Money = LogMgr.GetMoney(m_character.ID);
			SaveEquipGhost();
			if (m_character.IsDirty)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				playerBussiness.UpdatePlayer(m_character);
				playerBussiness.updateExplorerManualInfo(m_character);
				playerBussiness.updateExplorerManualInfoPages(m_character);
				playerBussiness.updateExplorerManualInfoDebris(m_character);
				playerBussiness.UpdateHappyRechargeData(HappyRechargeData);
				if (m_Labyrinth != null)
				{
					playerBussiness.UpdateLabyrinthInfo(m_Labyrinth);
				}
				foreach (UserGemStone item in m_GemStone)
				{
					playerBussiness.UpdateGemStoneInfo(item);
				}
			}
			TrailElite.SaveToDatabase();
			KingGrade.SaveToDatabase();
			EquipBag.SaveToDatabase();
			PropBag.SaveToDatabase();
			ConsortiaBag.SaveToDatabase();
			MagicHouse.SaveToDatabase();
			BeadBag.SaveToDatabase();
			MagicStoneBag.SaveToDatabase();
			HorseAmuletBag.SaveToDatabase();
			MinesBag.SaveToDatabase();
			RoomBordenBag.SaveToDatabase();
			FarmBag.SaveToDatabase();
			PetBag.SaveToDatabase(saveAdopt: true);
			CardBag.SaveToDatabase();
			AvatarBag.SaveToDatabase();
			StoreBag.SaveToDatabase();
			Farm.SaveToDatabase();
			Treasure.SaveToDatabase();
			Rank.SaveToDatabase();
			QuestInventory.SaveToDatabase();
			AchievementInventory.SaveToDatabase();
			CardAchievementInventory.SaveToDatabase();
			BufferList.SaveToDatabase();
			BattleData.SaveToDatabase();
			Actives.SaveToDatabase();
			Dice.SaveToDatabase();
			Extra.SaveToDatabase();
			Horse.SaveToDatabase();
			Home.SaveToDatabase();
			Bank.SaveToDatabase();
			Mines.SaveToDatabase();
			Marks.SaveToDatabase();
			SavePlayerInfoToDatabase();
			saveTeamSelfActive();
			saveTeamDayActive();
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Error saving all datas of playerID: " + m_character.ID + " , " + m_character.NickName + "!", exception);
			return false;
		}
	}

	public bool SaveNewItemToDatabase()
	{
		try
		{
			EquipBag.SaveNewItemToDatabase();
			PropBag.SaveNewItemToDatabase();
			BeadBag.SaveNewItemToDatabase();
			ConsortiaBag.SaveNewItemToDatabase();
			MagicHouse.SaveNewItemToDatabase();
			MagicStoneBag.SaveNewItemToDatabase();
			HorseAmuletBag.SaveNewItemToDatabase();
			RoomBordenBag.SaveNewItemToDatabase();
			MinesBag.SaveNewItemToDatabase();
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Error saving bags of playerID: " + m_character.ID + " , " + m_character.NickName + "!", exception);
			return false;
		}
	}

	public virtual bool Quit()
	{
		try
		{
			try
			{
				if (CurrentRoom != null)
				{
					CurrentRoom.RemovePlayerUnsafe(this);
					CurrentRoom = null;
				}
				else
				{
					RoomMgr.WaitingRoom.RemovePlayer(this);
				}
				if (CurrentMarryRoom != null)
				{
					CurrentMarryRoom.RemovePlayer(this);
					CurrentMarryRoom = null;
				}
				if (m_currentSevenDoubleRoom != null)
				{
					CurrentSevenDoubleRoom.RemovePlayer(this);
					CurrentSevenDoubleRoom = null;
				}
				if (m_currentHotSpringRoom != null)
				{
					CurrentHotSpringRoom.RemovePlayer(this);
					CurrentHotSpringRoom = null;
				}
				if (LotteryAwardList.Count > 0 && Lottery != -1)
				{
					SendItemsToMail(LotteryAwardList, "", LanguageMgr.GetTranslation("Game.Server.Lottery.Oversea.MailTitle"), eMailType.BuyItem);
					ResetLottery();
				}
				RoomMgr.WorldBossRoom.RemovePlayer(this);
				RoomMgr.ChristmasRoom.SetMonterDie(PlayerCharacter.ID);
				RoomMgr.ChristmasRoom.RemovePlayer(this);
				WorldMgr.NewHallRooms.RemovePlayer(PlayerId);
				WorldMgr.CollectionTaskRooms.RemovePlayer(PlayerId);
				Actives.StopChristmasTimer();
				Actives.StopLabyrinthTimer();
				Actives.StopLightriddleTimer();
				ConsortiaTaskMgr.RemovePlayer(this);
			}
			catch (Exception exception)
			{
				log.Error("Player exit Game Error!", exception);
			}
			m_character.State = 0;
			SaveIntoDatabase();
		}
		catch (Exception exception2)
		{
			log.Error("Player exit Error!!!", exception2);
		}
		finally
		{
			string text = $"Player {m_character.NickName}<{m_character.ID}> quit game! Save all data to db complete.";
			Console.WriteLine(text);
			if (PlayerCharacter.Money >= 1000000)
			{
				LogMgr.LogMoneyAdd((LogMoneyType)0, (LogMoneyType)0, PlayerCharacter.ID, PlayerCharacter.Money, PlayerCharacter.Money, 0, 0, 0, 0, text, 0, 0);
			}
			WorldMgr.RemovePlayer(m_character.ID);
		}
		return true;
	}

	public bool CanActive(string name)
	{
		return false;
	}

	public void ViFarmsAdd(int playerID)
	{
		if (!_viFarms.Contains(playerID))
		{
			_viFarms.Add(playerID);
		}
	}

	public void ViFarmsRemove(int playerID)
	{
		if (_viFarms.Contains(playerID))
		{
			_viFarms.Remove(playerID);
		}
	}

	public void FriendsAdd(int playerID, int relation)
	{
		if (!_friends.ContainsKey(playerID))
		{
			_friends.Add(playerID, relation);
		}
		else
		{
			_friends[playerID] = relation;
		}
	}

	public void FriendsRemove(int playerID)
	{
		if (_friends.ContainsKey(playerID))
		{
			_friends.Remove(playerID);
		}
	}

	public bool IsBlackFriend(int playerID)
	{
		if (_friends == null)
		{
			return true;
		}
		if (_friends.ContainsKey(playerID))
		{
			return _friends[playerID] == 1;
		}
		return false;
	}

	public void ClearConsortia()
	{
		PlayerCharacter.ClearConsortia();
		OnPropertiesChanged();
		QuestInventory.ClearConsortiaQuest();
		string translation = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender");
		string translation2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
		ConsortiaBag.SendAllItemsToMail(translation, translation2, eMailType.StoreCanel);
	}

	public int AddHorseRaceTimes(int value)
	{
		if (value > 0)
		{
			m_character.horseRaceCanRaceTime += value;
			Out.SendUpdateCountHorseRace(m_character.horseRaceCanRaceTime);
			return value;
		}
		return 0;
	}

	public int RemoveHorseRaceTimes(int value)
	{
		if (value > 0)
		{
			m_character.horseRaceCanRaceTime -= value;
			Out.SendUpdateCountHorseRace(m_character.horseRaceCanRaceTime);
			return value;
		}
		return 0;
	}

	public void AddRuneProperty(ItemInfo item, ref double defence, ref double attack)
	{
		RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
		if (runeTemplateInfo == null)
		{
			return;
		}
		string[] array = runeTemplateInfo.Attribute1.Split('|');
		string[] array2 = runeTemplateInfo.Attribute2.Split('|');
		string[] array3 = runeTemplateInfo.Attribute3.Split('|');
		int num = 0;
		int num2 = 0;
		if (item.Hole1 > runeTemplateInfo.BaseLevel)
		{
			if (array.Length > 1)
			{
				num = 1;
			}
			if (array2.Length > 1)
			{
				num2 = 1;
			}
		}
		int num3 = Convert.ToInt32(array[num]);
		int num4 = Convert.ToInt32(array2[num2]);
		switch (runeTemplateInfo.Type1)
		{
		case 35:
			attack += num3;
			break;
		case 36:
			if (array3.Length != 0)
			{
				defence += num3;
				int type = runeTemplateInfo.Type2;
				int num5 = type;
				if (num5 == 35)
				{
					attack += num4;
				}
			}
			else
			{
				defence += num3;
			}
			break;
		}
	}

	public double GetHertAddition(double para1, double para2)
	{
		double a = para1 * Math.Pow(1.1, para2) - para1;
		return Math.Round(a);
	}

	public List<ItemInfo> GetAllEquipItems()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < m_equipBag.BeginSlot; i++)
		{
			ItemInfo itemAt = m_equipBag.GetItemAt(i);
			if (itemAt != null)
			{
				list.Add(itemAt);
			}
		}
		return list;
	}

	public List<ItemInfo> GetStrengthenItems()
	{
		int[] array = new int[3] { 0, 5, 6 };
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < array.Length; i++)
		{
			ItemInfo itemAt = m_equipBag.GetItemAt(array[i]);
			if (itemAt != null)
			{
				list.Add(itemAt);
			}
		}
		return list;
	}

	public int GetTotalTotemUpgradePlus()
	{
		int num = 0;
		foreach (KeyValuePair<int, int> item in m_character.GetTotemUpgrade())
		{
			TS_UpgradeTemplate totemUpGradeInfo = ItemMgr.GetTotemUpGradeInfo(item.Key, item.Value);
			if (totemUpGradeInfo != null)
			{
				num += totemUpGradeInfo.Data / 10;
			}
		}
		return num / 5;
	}

	public double GetBaseAttack()
	{
		double num = 0.0;
		double defence = 0.0;
		double attack = 0.0;
		double num2 = 0.0;
		double avatarDame = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		double num5 = 0.0;
		int num6 = 0;
		int attack2 = 0;
		int def = 0;
		int agi = 0;
		int luck = 0;
		int hp = 0;
		int mgattack = 0;
		int mgdef = 0;
		int basedame = 0;
		int basedef = 0;
		ItemInfo itemAt = m_equipBag.GetItemAt(6);
		if (itemAt != null)
		{
			double num7 = itemAt.Template.Property7;
			int num8 = (itemAt.IsGold ? 1 : 0);
			double para = itemAt.StrengthenLevel + num8;
			num5 = GetHertAddition(num7, para) + num7;
			UserEquipGhostInfo ghostEquip = GetGhostEquip(itemAt.BagType, itemAt.Place);
			if (ghostEquip != null)
			{
				num5 += num7 / 200.0 * Math.Pow(ghostEquip.Level, 1.2) / 100.0 * num5;
			}
		}
		if (MarkProperties == null)
		{
			Marks.GetEquipProperties();
		}
		UserRankInfo singleRank = Rank.GetSingleRank(PlayerCharacter.Honor);
		if (singleRank != null)
		{
			num += (double)singleRank.Damage;
		}
		List<ItemInfo> allEquipItems = GetAllEquipItems();
		foreach (ItemInfo item in allEquipItems)
		{
			SubActiveConditionInfo subActiveInfo = SubActiveMgr.GetSubActiveInfo(item);
			if (subActiveInfo != null)
			{
				num += (double)subActiveInfo.GetValue("6");
			}
		}
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt2 = m_beadBag.GetItemAt(i);
			if (itemAt2 != null)
			{
				AddRuneProperty(itemAt2, ref defence, ref attack);
			}
		}
		double avatarGuard = 0.0;
		AvatarBag.AddBasePropAvatarColection(ref avatarGuard, ref avatarDame);
		if (Horse.Info.curUseHorse > 0)
		{
			int curLevel = Horse.Info.curLevel;
			MountTemplateInfo mountTemplate = MountMgr.GetMountTemplate(curLevel);
			num3 += (double)mountTemplate.AddDamage;
		}
		MountDrawDataInfo[] getPicCherishs = Horse.GetPicCherishs;
		MountDrawDataInfo[] array = getPicCherishs;
		foreach (MountDrawDataInfo mountDrawDataInfo in array)
		{
			MountDrawTemplateInfo mountDrawTemplateInfo = MountMgr.FindMountDrawInfo(mountDrawDataInfo.ID);
			if (mountDrawTemplateInfo != null)
			{
				int[] array2 = MountMgr.calculateRate(mountDrawDataInfo);
				num4 += (double)array2[0];
			}
		}
		int num9 = 0;
		foreach (UsersPetLacrarInfo item2 in PetBag.lacresPet)
		{
			if (!item2.isSeal)
			{
				continue;
			}
			foreach (StoreFineSealLockInfo item3 in item2.lockArr)
			{
				if (item3.proType == 36)
				{
					num9 += item3.proNum;
				}
			}
		}
		num6 = ExerciseMgr.GetExercise(PlayerCharacter.Texp.TrainDmgTexpExp).TrainDmg;
		CardNewSystemMgr.GetSuitValue(this, ref hp, ref attack2, ref def, ref agi, ref luck, ref mgattack, ref mgdef, ref basedame, ref basedef);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Bead", attack);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Suit", num2);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Avatar", avatarDame);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Horse", num3);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "HorsePicCherish", num4);
		if (MarkProperties != null)
		{
			PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Mark", MarkProperties.Damage);
		}
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Texp", num6);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "CardAchievement", CardAchievementInventory.Property[5]);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "SeloPet", num9);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Card", basedame);
		num += (double)CardAchievementInventory.Property[5];
		double num10 = TotemMgr.GetTotemProp(m_character.totemId, "dam", GetTotalTotemUpgradePlus());
		double num11 = 0.0;
		num11 = ((MarkProperties == null) ? (num + attack + num2 + avatarDame + num3 + num4 + (double)num6 + (double)PlayerCharacter.explorerManualInfo.Damage + (double)num9 + num10 + num5 + (double)basedame) : (num + attack + num2 + avatarDame + num3 + num4 + (double)num6 + (double)MarkProperties.Damage + (double)PlayerCharacter.explorerManualInfo.Damage + (double)num9 + num10 + num5 + (double)basedame));
		PlayerProp.totalDamage = (int)num11;
		return (int)num11;
	}

	public double GetBaseDefence()
	{
		double guard = 0.0;
		double defence = 0.0;
		double num = 0.0;
		double avatarDame = 0.0;
		double avatarGuard = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		double guard2 = 0.0;
		int num5 = 0;
		int attack = 0;
		int def = 0;
		int agi = 0;
		int luck = 0;
		int hp = 0;
		int mgattack = 0;
		int mgdef = 0;
		int basedame = 0;
		int basedef = 0;
		CardNewSystemMgr.GetSuitValue(this, ref hp, ref attack, ref def, ref agi, ref luck, ref mgattack, ref mgdef, ref basedame, ref basedef);
		if (MarkProperties == null)
		{
			Marks.GetEquipProperties();
		}
		SetsBuildTempMgr.GetSetsBuildProp(PlayerCharacter.fineSuitExp, ref guard);
		HomeTempPracticeMgr.BuildProp(Actives.Info.CurrentLevel, ref guard2);
		UserRankInfo singleRank = Rank.GetSingleRank(PlayerCharacter.Honor);
		if (singleRank != null)
		{
			guard += (double)singleRank.Guard;
		}
		List<ItemInfo> allEquipItems = GetAllEquipItems();
		foreach (ItemInfo item in allEquipItems)
		{
			SubActiveConditionInfo subActiveInfo = SubActiveMgr.GetSubActiveInfo(item);
			if (subActiveInfo != null)
			{
				guard += (double)subActiveInfo.GetValue("7");
			}
		}
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt = m_beadBag.GetItemAt(i);
			if (itemAt != null)
			{
				AddRuneProperty(itemAt, ref defence, ref avatarDame);
			}
		}
		AvatarBag.AddBasePropAvatarColection(ref avatarGuard, ref avatarDame);
		if (Horse.Info.curUseHorse > 0)
		{
			int curLevel = Horse.Info.curLevel;
			MountTemplateInfo mountTemplate = MountMgr.GetMountTemplate(curLevel);
			num2 += (double)mountTemplate.AddDamage;
		}
		MountDrawDataInfo[] getPicCherishs = Horse.GetPicCherishs;
		MountDrawDataInfo[] array = getPicCherishs;
		foreach (MountDrawDataInfo mountDrawDataInfo in array)
		{
			MountDrawTemplateInfo mountDrawTemplateInfo = MountMgr.FindMountDrawInfo(mountDrawDataInfo.ID);
			if (mountDrawTemplateInfo != null)
			{
				int[] array2 = MountMgr.calculateRate(mountDrawDataInfo);
				num3 += (double)array2[1];
			}
		}
		EatPetsInfo eatPets = PetBag.EatPets;
		if (eatPets != null)
		{
			PetMoePropertyInfo petMoePropertyInfo = PetMoePropertyMgr.FindPetMoeProperty(eatPets.hatLevel);
			if (petMoePropertyInfo != null)
			{
				num4 += (double)petMoePropertyInfo.Guard;
			}
		}
		num5 = ExerciseMgr.GetExercise(PlayerCharacter.Texp.TrainArmorTexpExp).TrainArmor;
		int num6 = 0;
		foreach (UsersPetLacrarInfo item2 in PetBag.lacresPet)
		{
			if (!item2.isSeal)
			{
				continue;
			}
			foreach (StoreFineSealLockInfo item3 in item2.lockArr)
			{
				if (item3.proType == 35)
				{
					num6 += item3.proNum;
				}
			}
		}
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Bead", defence);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Suit", num);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Avatar", avatarGuard);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Horse", num2);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "HorsePicCherish", num3);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Pet", num4);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Temple", guard2);
		if (MarkProperties != null)
		{
			PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Mark", MarkProperties.Guard);
		}
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Texp", num5);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "CardAchievement", CardAchievementInventory.Property[3]);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "SeloPet", num6);
		guard += (double)CardAchievementInventory.Property[3];
		guard += (double)TotemMgr.GetTotemProp(m_character.totemId, "gua", GetTotalTotemUpgradePlus());
		double num7 = 0.0;
		int[] array3 = new int[6] { 1, 3, 5, 13, 11, 2 };
		List<ItemInfo> list = new List<ItemInfo>();
		for (int k = 0; k < array3.Length; k++)
		{
			ItemInfo itemAt2 = m_equipBag.GetItemAt(array3[k]);
			if (itemAt2 != null)
			{
				double num8 = itemAt2.Template.Property7;
				int num9 = (itemAt2.GoldValidDate() ? 1 : 0);
				double para = itemAt2.StrengthenLevel + num9;
				double num10 = GetHertAddition(num8, para) + num8;
				UserEquipGhostInfo ghostEquip = GetGhostEquip(itemAt2.BagType, itemAt2.Place);
				if (ghostEquip != null)
				{
					num10 += num8 / 60.0 * Math.Pow(ghostEquip.Level, 1.2) / 100.0 * num10;
				}
				guard += num10;
			}
		}
		ItemInfo itemAt3 = m_equipBag.GetItemAt(0);
		if (itemAt3 != null)
		{
			num7 = itemAt3.Template.Property7;
			int num11 = (itemAt3.GoldValidDate() ? 1 : 0);
			double para2 = itemAt3.StrengthenLevel + num11;
			double num12 = GetHertAddition(num7, para2) + num7;
			UserEquipGhostInfo ghostEquip2 = GetGhostEquip(itemAt3.BagType, itemAt3.Place);
			if (ghostEquip2 != null)
			{
				num12 += num7 / 60.0 * Math.Pow(ghostEquip2.Level, 1.2) / 10.0 * num12;
			}
			guard += num12;
		}
		ItemInfo itemAt4 = m_equipBag.GetItemAt(4);
		if (itemAt4 != null)
		{
			num7 = itemAt4.Template.Property7;
			int num13 = (itemAt4.GoldValidDate() ? 1 : 0);
			double para3 = itemAt4.StrengthenLevel + num13;
			double num14 = GetHertAddition(num7, para3) + num7;
			UserEquipGhostInfo ghostEquip3 = GetGhostEquip(itemAt4.BagType, itemAt4.Place);
			if (ghostEquip3 != null)
			{
				num14 += num7 / 60.0 * Math.Pow(ghostEquip3.Level, 1.2) / 10.0 * num14;
			}
			guard += num14;
		}
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Card", basedef);
		double num15 = 0.0;
		num15 = ((MarkProperties == null) ? (guard + defence + num + avatarGuard + num2 + num3 + guard2 + (double)num5 + (double)PlayerCharacter.explorerManualInfo.Armor + (double)num6 * 0.5 + num4 + (double)basedef) : (guard + defence + num + avatarGuard + num2 + num3 + guard2 + (double)num5 + (double)MarkProperties.Guard + (double)PlayerCharacter.explorerManualInfo.Armor + (double)num6 * 0.5 + num4 + (double)basedef));
		PlayerProp.totalArmor = (int)num15;
		return (int)num15;
	}

	public double GetBaseAgility()
	{
		return 1.0 - (double)m_character.Agility * 0.001;
	}

	public double GetBaseBlood()
	{
		ItemInfo itemAt = EquipBag.GetItemAt(12);
		if (itemAt == null)
		{
			return 1.0;
		}
		int necklaceLevelByGP = NecklaceStrengthMgr.GetNecklaceLevelByGP(PlayerCharacter.necklaceExp);
		StrengThenExp strengThenExp = NecklaceStrengthMgr.FindStrengThenExpInfo((necklaceLevelByGP == -1) ? 12 : necklaceLevelByGP);
		return (100.0 + (double)itemAt.Template.Property1 + (double)strengThenExp.NecklaceStrengthPlus) / 100.0;
	}

	public double GetGoldBlood()
	{
		ItemInfo itemAt = EquipBag.GetItemAt(0);
		ItemInfo itemAt2 = EquipBag.GetItemAt(4);
		double num = 1.0;
		if (itemAt != null)
		{
			GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(-1);
			if (itemAt.GoldValidDate())
			{
				num += (double)goldEquipTemplateInfo.Boold;
			}
		}
		if (itemAt2 != null)
		{
			GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(-1);
			if (itemAt2.GoldValidDate())
			{
				num += (double)goldEquipTemplateInfo.Boold;
			}
		}
		return num;
	}

	public bool RemoveAt(eBageType bagType, int place)
	{
		return GetInventory(bagType)?.RemoveItemAt(place) ?? false;
	}

	public void UpdateBarrier(int barrier, string pic)
	{
		if (CurrentRoom != null)
		{
			CurrentRoom.Pic = pic;
			CurrentRoom.barrierNum = barrier;
			CurrentRoom.currentFloor = barrier;
		}
	}

	public void UpdateConsortiaBattle(int leftBlood, bool isWin, int tieStatus)
	{
		int winId = (isWin ? PlayerId : GuildBattleEnemyId);
		int lostId = ((!isWin) ? PlayerId : GuildBattleEnemyId);
		UserGuildBattleInfo userGuildBattleInfo = GameMgr.GuildBattle.FindUser(PlayerId);
		if (userGuildBattleInfo == null)
		{
			return;
		}
		if (tieStatus != -1)
		{
			GameMgr.GuildBattle.UpdateScoreMatch(winId, lostId);
			if (isWin)
			{
				m_character.ReduceStartBlood = leftBlood;
			}
			else
			{
				m_character.ReduceStartBlood = m_character.hp;
				GameMgr.GuildBattle.AddCountDownRevive(userGuildBattleInfo, 30);
			}
		}
		GuildBattleEnemyId = 0;
		m_character.ActivePowFirstGame = false;
		if (userGuildBattleInfo.IsActive)
		{
			GameMgr.GuildBattle.SendUpdateSceneInfo(userGuildBattleInfo);
			GameMgr.GuildBattle.SendUpdatePlayerStatus(userGuildBattleInfo);
		}
	}

	public bool DeletePropItem(int place)
	{
		FightBag.RemoveItemAt(place);
		return true;
	}

	public bool UseKingBlessHelpStraw(eRoomType roomType)
	{
		if (roomType == eRoomType.Lanbyrinth || roomType == eRoomType.Dungeon || roomType == eRoomType.Element_PVE)
		{
			if (Extra.UseKingBless(4))
			{
				return true;
			}
			if (BufferList.UserSaveLifeBuff())
			{
				return true;
			}
		}
		return false;
	}

	public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
	{
		if (bag == 1)
		{
			ItemTemplateInfo itemTemplateInfo = PropItemMgr.FindFightingProp(templateId);
			if (!isLiving || itemTemplateInfo == null)
			{
				return false;
			}
			OnUsingItem(itemTemplateInfo.TemplateID, 1);
			if (place == -1 && CanUseProp)
			{
				return true;
			}
			ItemInfo itemAt = GetItemAt(eBageType.PropBag, place);
			if (itemAt == null || !itemAt.IsValidItem() || itemAt.Count < 0)
			{
				return false;
			}
			itemAt.Count--;
			UpdateItem(itemAt);
			return true;
		}
		ItemInfo itemAt2 = GetItemAt(eBageType.FightBag, place);
		if (itemAt2 == null || itemAt2.TemplateID != templateId)
		{
			return false;
		}
		if (IsSpecialPropItem(templateId))
		{
			if (templateId == 10615 || templateId == 10616)
			{
				itemAt2 = GetItemAt(eBageType.PropBag, place);
				if (itemAt2 != null)
				{
					RemoveTemplate(templateId, 1);
				}
			}
			return true;
		}
		OnUsingItem(itemAt2.TemplateID, 1);
		return RemoveAt(eBageType.FightBag, place);
	}

	public bool IsSpecialPropItem(int templateId)
	{
		if ((uint)(templateId - 10467) <= 2u || templateId == 10471 || (uint)(templateId - 10615) <= 1u)
		{
			return true;
		}
		return false;
	}

	public bool ClearPropItem()
	{
		FightBag.ClearBag();
		return true;
	}

	public void Disconnect()
	{
		m_client.Disconnect();
	}

	public virtual void SendTCP(GSPacketIn pkg)
	{
		if (m_client.IsConnected)
		{
			m_client.SendTCP(pkg);
		}
	}

	public void ClearFootballCard()
	{
		for (int i = 0; i < CardsTakeOut.Length; i++)
		{
			CardsTakeOut[i] = null;
		}
	}

	public void TakeFootballCard(CardsTakeOutInfo card)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < CardsTakeOut.Length; i++)
		{
			if (card.place == i)
			{
				CardsTakeOut[i] = card;
				CardsTakeOut[i].IsTake = true;
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(card.templateID);
				if (itemTemplateInfo != null)
				{
					list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, card.count, 110));
				}
				takeoutCount--;
				break;
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (ItemInfo item in list)
		{
			AddTemplate(list);
		}
	}

	public void ShowAllFootballCard()
	{
		for (int i = 0; i < CardsTakeOut.Length; i++)
		{
			if (CardsTakeOut[i] == null)
			{
				CardsTakeOut[i] = Card[i];
				if (takeoutCount > 0)
				{
					TakeFootballCard(Card[i]);
				}
			}
		}
	}

	public void FootballTakeOut(bool isWin)
	{
		if (isWin)
		{
			canTakeOut = 2;
			takeoutCount = 2;
		}
		else
		{
			canTakeOut = 1;
			takeoutCount = 1;
		}
	}

	public void ResetRoom(bool isWin, string parram)
	{
		if (CurrentRoom == null)
		{
			return;
		}
		if (parram == "CreateInstanceFail")
		{
			CurrentRoom.SendCancelPickUp();
			SendMessage(LanguageMgr.GetTranslation(parram));
			return;
		}
		if (parram == "SurvivalModeClose" && CurrentRoom.RoomType == eRoomType.SurvivalMode)
		{
			CurrentRoom.SendCancelPickUp();
			SendMessage(LanguageMgr.GetTranslation(parram));
		}
		if (CurrentRoom.RoomType == eRoomType.Cryptboss)
		{
			RoomMgr.ExitRoom(CurrentRoom, this);
		}
		if (CurrentRoom.RoomType == eRoomType.ActivityDungeon)
		{
			Actives.SendQXAward();
		}
		if (CurrentRoom.RoomType == eRoomType.FarmBoss)
		{
			Farm.SendFarmPoultryAward(isWin);
		}
		if (CurrentRoom.RoomType == eRoomType.Dungeon || CurrentRoom.RoomType == eRoomType.Element_PVE)
		{
			CurrentRoom.Pic = "";
			CurrentRoom.MapId = 10000;
			CurrentRoom.currentFloor = 0;
			CurrentRoom.isOpenBoss = false;
			CurrentRoom.SendRoomSetupChange(CurrentRoom);
		}
		if (CurrentRoom.RoomType != eRoomType.CampBattleBattle)
		{
			return;
		}
		CampBattlePlayerInfo player = GameMgr.CampBattle.GetPlayer(ZoneId, PlayerId);
		if (player != null)
		{
			string[] array = parram.Split(',');
			if (array[0] == "pvp")
			{
			}
			if (isWin)
			{
				player.StateType = CampBattlePlayerState.NORMAL;
				return;
			}
			player.TimerCount = 60;
			player.StateType = CampBattlePlayerState.DEAD;
		}
	}

	public int AddLoveScore(int value, int type)
	{
		if (!PlayerCharacter.IsMarried)
		{
			return 0;
		}
		if (value > 0 && Extra.CanRingExp(type))
		{
			m_character.RingExp += value;
			OnPropertiesChanged();
			Extra.UpdateLoveScoreLimit(type);
			return value;
		}
		return 0;
	}

	public void UpdatePublicPlayer(string tempStyle = "")
	{
		PlayerCharacter.tempStyle = tempStyle;
		GSPacketIn pkg = Out.SendUpdatePublicPlayer(m_character, m_battle.MatchInfo);
		if (_currentRoom != null)
		{
			_currentRoom.SendToAll(pkg, this);
		}
	}

	public void DirectAddValue(SpecialItemBoxInfo values)
	{
		if (values.Money > 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.Money + LanguageMgr.GetTranslation("OpenUpArkHandler.Money"));
			LogAddMoney(AddMoneyType.UpArk, AddMoneyType.UpArk_Open, PlayerCharacter.ID, (int)values.Money, PlayerCharacter.Money);
			AddMoney((int)values.Money);
		}
		if (values.Gold != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.Gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold"));
			AddGold(values.Gold);
		}
		if (values.DDTMoney != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.DDTMoney + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken"));
			AddGiftToken(values.DDTMoney);
		}
		if (values.Medal != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.Medal + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal"));
			AddMedal(values.Medal);
		}
		if (values.Exp != 0)
		{
			if (Level == LevelMgr.MaxLevel)
			{
				int num = values.Exp / 100;
				if (num > 0)
				{
					AddOffer(num);
					SendMessage(eMessageType.notAgain, LanguageMgr.GetTranslation("OpenUpArkHandler.MaxExp", num));
				}
			}
			else
			{
				AddGP(values.Exp, canDupeExp: true, canAddTop: false);
				SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.Exp + LanguageMgr.GetTranslation("OpenUpArkHandler.Exp"));
			}
		}
		if (values.Honor != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.Honor + LanguageMgr.GetTranslation("OpenUpArkHandler.honor"));
			AddHonor(values.Honor);
		}
		if (values.HardCurrency != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.HardCurrency + LanguageMgr.GetTranslation("OpenUpArkHandler.hardCurrency"));
			AddHardCurrency(values.HardCurrency);
		}
		if (values.LeagueMoney != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.LeagueMoney + LanguageMgr.GetTranslation("OpenUpArkHandler.leagueMoney"));
			AddLeagueMoney(values.LeagueMoney);
		}
		if (values.UseableScore != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.UseableScore + LanguageMgr.GetTranslation("OpenUpArkHandler.useableScore"));
			Actives.Info.useableScore += values.UseableScore;
		}
		if (values.Prestge != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.Prestge + LanguageMgr.GetTranslation("OpenUpArkHandler.prestge"));
			MatchInfo.addDayPrestge += values.Prestge;
			MatchInfo.totalPrestige += values.Prestge;
		}
		if (values.LoveScore != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.LoveScore + LanguageMgr.GetTranslation("OpenUpArkHandler.LoveScore"));
			Farm.AddLoveScore(values.LoveScore);
		}
		if (values.MagicstoneScore != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.MagicstoneScore + LanguageMgr.GetTranslation("OpenUpArkHandler.MagicstoneScore"));
			Extra.Info.ScoreMagicstone += values.MagicstoneScore;
		}
		if (values.SummerScore != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.Start") + values.SummerScore + LanguageMgr.GetTranslation("OpenUpArkHandler.SummerScore"));
			Extra.Info.SummerScore += values.SummerScore;
			Extra.Info.Score += values.SummerScore;
		}
		if (values.RingScore != 0)
		{
			SendMessage(LanguageMgr.GetTranslation("OpenUpArkHandler.RingScore", values.RingScore));
			AddLoveScore(values.RingScore, -1);
		}
	}

	public void DirectRemoveValue(SpecialItemBoxInfo values)
	{
		if (values.Money > 0)
		{
			RemoveMoney((int)values.Money);
		}
		if (values.Gold != 0)
		{
			RemoveGold(values.Gold);
		}
		if (values.DDTMoney != 0)
		{
			RemoveGiftToken(values.DDTMoney);
		}
		if (values.Offer != 0)
		{
			RemoveOffer(values.Offer);
		}
		if (values.Medal != 0)
		{
			RemoveMedal(values.Medal);
		}
		if (values.Exp != 0)
		{
			RemoveGP(values.Exp);
		}
		if (values.Honor != 0)
		{
			RemoveHonor(values.Honor);
		}
		if (values.HardCurrency != 0)
		{
			RemoveHardCurrency(values.HardCurrency);
		}
		if (values.LeagueMoney != 0)
		{
			RemoveLeagueMoney(values.LeagueMoney);
		}
		if (values.UseableScore != 0)
		{
			Actives.Info.useableScore -= values.UseableScore;
		}
		if (values.PetScore != 0)
		{
			RemovePetScore(values.PetScore);
		}
		if (values.DamageScore != 0)
		{
			RemoveDamageScores(values.DamageScore);
		}
		if (values.Prestge != 0)
		{
			MatchInfo.addDayPrestge -= values.Prestge;
			MatchInfo.totalPrestige -= values.Prestge;
		}
		if (values.LoveScore != 0)
		{
			Farm.RemoveLoveScore(values.LoveScore);
		}
		if (values.MagicstoneScore != 0)
		{
			Extra.Info.ScoreMagicstone -= values.MagicstoneScore;
		}
		if (values.SummerScore != 0)
		{
			Extra.Info.SummerScore -= values.SummerScore;
		}
		if (values.FishScore != 0 && Home.Data != null)
		{
			Home.Data.fishScore -= values.FishScore;
			Home.SendUpdateFishPlayerInfo();
		}
	}

	public int RemoveFishScore(int value)
	{
		if (value > 0)
		{
			Home.Data.fishScore -= value;
			Home.SendUpdateFishPlayerInfo();
			return value;
		}
		return 0;
	}

	public void LoadMarryProp()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		MarryProp marryProp = playerBussiness.GetMarryProp(PlayerCharacter.ID);
		PlayerCharacter.IsMarried = marryProp.IsMarried;
		PlayerCharacter.SpouseID = marryProp.SpouseID;
		PlayerCharacter.SpouseName = marryProp.SpouseName;
		PlayerCharacter.IsCreatedMarryRoom = marryProp.IsCreatedMarryRoom;
		PlayerCharacter.SelfMarryRoomID = marryProp.SelfMarryRoomID;
		PlayerCharacter.IsGotRing = marryProp.IsGotRing;
		Out.SendMarryProp(this, marryProp);
	}

	public override string ToString()
	{
		return $"Id:{PlayerId} nickname:{PlayerCharacter.NickName} room:{CurrentRoom} ";
	}

	public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
	{
		return ConsortiaMgr.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
	}

	public void SendConsortiaFight(int consortiaID, int riches, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(158);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(riches);
		gSPacketIn.WriteString(msg);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public void LoadPvePermission()
	{
		PveInfo[] pveInfo = PveInfoMgr.GetPveInfo();
		PveInfo[] array = pveInfo;
		foreach (PveInfo pveInfo2 in array)
		{
			if (m_character.Grade > pveInfo2.LevelLimits)
			{
				bool flag = SetPvePermission(pveInfo2.ID, eHardLevel.Easy);
				if (flag)
				{
					flag = SetPvePermission(pveInfo2.ID, eHardLevel.Normal);
				}
				if (flag)
				{
					SetPvePermission(pveInfo2.ID, eHardLevel.Hard);
				}
			}
		}
	}

	public char[] InitPvePermission()
	{
		char[] array = new char[50];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = '1';
		}
		return array;
	}

	public string ConverterPvePermission(char[] chArray)
	{
		string text = "";
		for (int i = 0; i < chArray.Length; i++)
		{
			text += chArray[i];
		}
		return text;
	}

	public bool SetPvePermission(int copyId, eHardLevel hardLevel)
	{
		try
		{
			if (copyId > m_pvepermissions.Length || copyId <= 0 || hardLevel == eHardLevel.Terror || m_pvepermissions[copyId - 1] != permissionChars[(int)hardLevel])
			{
				return true;
			}
			m_pvepermissions[copyId - 1] = permissionChars[(int)(hardLevel + 1)];
			m_character.PvePermission = ConverterPvePermission(m_pvepermissions);
			OnPropertiesChanged();
		}
		catch
		{
			log.ErrorFormat("SetPvePermission copyId {0}, hardLevel {1}, Fail!", copyId, hardLevel);
			return false;
		}
		return true;
	}

	public bool IsPvePermission(int copyId, eHardLevel hardLevel)
	{
		return true;
	}

	public bool IsPveEpicPermission(int copyId)
	{
		string pveEpicPermission = m_character.PveEpicPermission;
		if (pveEpicPermission.Length > 0)
		{
			string[] source = pveEpicPermission.Split('-');
			return source.Any((string loc5) => loc5 == copyId.ToString());
		}
		return false;
	}

	public void SendInsufficientMoney(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(88, PlayerId);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteBoolean(val: false);
		SendTCP(gSPacketIn);
	}

	public void SendMessage(string msg)
	{
		SendMessage(eMessageType.Normal, msg);
	}

	public void SendMessage(eMessageType type, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt((int)type);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendPrivateChat(int receiverId, string receiver, string sender, string msg, bool isAutoReply)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(37, PlayerCharacter.ID);
		gSPacketIn.WriteInt(receiverId);
		gSPacketIn.WriteString(receiver);
		gSPacketIn.WriteString(sender);
		gSPacketIn.WriteString(msg);
		gSPacketIn.WriteBoolean(isAutoReply);
		SendTCP(gSPacketIn);
	}

	public void SendAreaPrivateChat(string sender, string zoneName, string msg, int zoneId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(107, PlayerCharacter.ID);
		gSPacketIn.WriteString(zoneName);
		gSPacketIn.WriteString(sender);
		gSPacketIn.WriteString(msg);
		gSPacketIn.WriteInt(zoneId);
		SendTCP(gSPacketIn);
	}

	public void SendHideMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public int LabyrinthTryAgainMoney()
	{
		for (int i = 0; i < Labyrinth.myProgress; i += 2)
		{
			if (Labyrinth.currentFloor == i)
			{
				return GameProperties.WarriorFamRaidPriceBig;
			}
		}
		return GameProperties.WarriorFamRaidPriceSmall;
	}

	public void SendLabyrinthTryAgain()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(131, PlayerId);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(PlayerCharacter.LabyrinthInfo.tryAgainMoney);
		SendTCP(gSPacketIn);
	}

	public bool SendMoneyMailToUser(PlayerBussiness pb, string content, string title, int money, eMailType type)
	{
		MailInfo mail = new MailInfo
		{
			Content = content,
			Title = title,
			Gold = 0,
			IsExist = true,
			Money = money,
			GiftToken = 0,
			Receiver = PlayerCharacter.NickName,
			ReceiverID = PlayerCharacter.ID,
			Sender = PlayerCharacter.NickName,
			SenderID = PlayerCharacter.ID,
			Type = (int)type,
			Annex1 = "",
			Annex1Name = ""
		};
		return pb.SendMail(mail, ZoneId);
	}

	public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type)
	{
		using PlayerBussiness pb = new PlayerBussiness();
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (ItemInfo item in items)
		{
			if (item.Template.MaxCount == 1)
			{
				for (int i = 0; i < item.Count; i++)
				{
					ItemInfo itemInfo = ItemInfo.CloneFromTemplate(item.Template, item);
					itemInfo.Count = 1;
					list.Add(itemInfo);
				}
			}
			else
			{
				list.Add(item);
			}
		}
		return SendItemsToMail(list, content, title, type, pb);
	}

	public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb)
	{
		bool result = true;
		for (int i = 0; i < items.Count; i += 5)
		{
			MailInfo mailInfo = new MailInfo
			{
				Title = (title ?? LanguageMgr.GetTranslation("Game.Server.GameUtils.Title")),
				Gold = 0,
				IsExist = true,
				Money = 0,
				Receiver = PlayerCharacter.NickName,
				ReceiverID = PlayerId,
				Sender = PlayerCharacter.NickName,
				SenderID = PlayerId,
				Type = (int)type,
				GiftToken = 0
			};
			List<ItemInfo> list = new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			content = ((content != null) ? LanguageMgr.GetTranslation(content) : "");
			int num = i;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo.Annex1 = itemInfo.ItemID.ToString();
				mailInfo.Annex1Name = itemInfo.Template.Name;
				stringBuilder.Append("1、" + mailInfo.Annex1Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("1、" + mailInfo.Annex1Name + "x" + itemInfo.Count + ";");
			}
			num = i + 1;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo.Annex2 = itemInfo.ItemID.ToString();
				mailInfo.Annex2Name = itemInfo.Template.Name;
				stringBuilder.Append("2、" + mailInfo.Annex2Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("2、" + mailInfo.Annex2Name + "x" + itemInfo.Count + ";");
			}
			num = i + 2;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo.Annex3 = itemInfo.ItemID.ToString();
				mailInfo.Annex3Name = itemInfo.Template.Name;
				stringBuilder.Append("3、" + mailInfo.Annex3Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("3、" + mailInfo.Annex3Name + "x" + itemInfo.Count + ";");
			}
			num = i + 3;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo.Annex4 = itemInfo.ItemID.ToString();
				mailInfo.Annex4Name = itemInfo.Template.Name;
				stringBuilder.Append("4、" + mailInfo.Annex4Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("4、" + mailInfo.Annex4Name + "x" + itemInfo.Count + ";");
			}
			num = i + 4;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo.Annex5 = itemInfo.ItemID.ToString();
				mailInfo.Annex5Name = itemInfo.Template.Name;
				stringBuilder.Append("5、" + mailInfo.Annex5Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("5、" + mailInfo.Annex5Name + "x" + itemInfo.Count + ";");
			}
			mailInfo.AnnexRemark = stringBuilder.ToString();
			if (content != "")
			{
				mailInfo.Content = content;
			}
			else
			{
				mailInfo.Content = stringBuilder2.ToString();
			}
			if (pb.SendMail(mailInfo, ZoneId))
			{
				foreach (ItemInfo item in list)
				{
					TakeOutItem(item);
				}
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public bool SendItemToMail(int templateId, string content, string title)
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo == null)
		{
			return false;
		}
		if (content == "")
		{
			content = itemTemplateInfo.Name + "x1";
		}
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 104);
		itemInfo.IsBinds = true;
		return SendItemToMail(itemInfo, content, title, eMailType.Active);
	}

	public bool SendItemToMail(ItemInfo item, string content, string title, eMailType type)
	{
		using PlayerBussiness pb = new PlayerBussiness();
		return SendItemToMail(item, pb, content, title, type);
	}

	public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, string content, string title, eMailType type)
	{
		MailInfo mailInfo = new MailInfo
		{
			Content = (content ?? LanguageMgr.GetTranslation("Game.Server.GameUtils.Content")),
			Title = (title ?? LanguageMgr.GetTranslation("Game.Server.GameUtils.Title")),
			Gold = 0,
			IsExist = true,
			Money = 0,
			GiftToken = 0,
			Receiver = PlayerCharacter.NickName,
			ReceiverID = PlayerCharacter.ID,
			Sender = PlayerCharacter.NickName,
			SenderID = PlayerCharacter.ID,
			Type = (int)type
		};
		if (item.ItemID == 0)
		{
			pb.AddGoods(item);
		}
		mailInfo.Annex1 = item.ItemID.ToString();
		mailInfo.Annex1Name = item.Template.Name;
		if (pb.SendMail(mailInfo, ZoneId))
		{
			TakeOutItem(item);
			return true;
		}
		return false;
	}

	public bool SendMailToUser(PlayerBussiness pb, string content, string title, eMailType type)
	{
		MailInfo mail = new MailInfo
		{
			Content = content,
			Title = title,
			Gold = 0,
			IsExist = true,
			Money = 0,
			GiftToken = 0,
			Receiver = PlayerCharacter.NickName,
			ReceiverID = PlayerCharacter.ID,
			Sender = PlayerCharacter.NickName,
			SenderID = PlayerCharacter.ID,
			Type = (int)type,
			Annex1 = "",
			Annex1Name = ""
		};
		if (pb.SendMail(mail, ZoneId))
		{
			return true;
		}
		return false;
	}

	public void ResetNewDayLabyrinth()
	{
		if (PlayerCharacter.LabyrinthInfo.LastDate.GetWeekOfMonth() != DateTime.Now.Date.GetWeekOfMonth())
		{
			PlayerCharacter.LabyrinthInfo.maxWeekFinishLevel = 0;
		}
		if (!(PlayerCharacter.LabyrinthInfo.LastDate >= DateTime.Now.Date))
		{
			PlayerCharacter.LabyrinthInfo.isPlayGame = false;
			PlayerCharacter.LabyrinthInfo.isSweep = false;
			PlayerCharacter.LabyrinthInfo.currentFinishLevel = 1;
			PlayerCharacter.LabyrinthInfo.nowContinueCount = 2;
			PlayerCharacter.LabyrinthInfo.sweepTotalExp = 0;
			PlayerCharacter.LabyrinthInfo.LastDate = DateTime.Now;
		}
	}

	public void CalculateSweepTime()
	{
		if (PlayerCharacter.LabyrinthInfo != null)
		{
			int num = 0;
			for (int i = PlayerCharacter.LabyrinthInfo.currentFinishLevel; i <= PlayerCharacter.LabyrinthInfo.maxWeekFinishLevel; i++)
			{
				num += 2;
			}
			num *= 60;
			PlayerCharacter.LabyrinthInfo.sweepTotalTimeRemaining = num;
			PlayerCharacter.LabyrinthInfo.sweepTotalTime = num;
		}
	}

	public bool TakeOutItem(ItemInfo item)
	{
		if (item.BagType == m_propBag.BagType)
		{
			return m_propBag.TakeOutItem(item);
		}
		if (item.BagType == m_fightBag.BagType)
		{
			return m_fightBag.TakeOutItem(item);
		}
		if (item.BagType == m_consortiaBag.BagType)
		{
			return m_consortiaBag.TakeOutItem(item);
		}
		if (item.BagType == m_beadBag.BagType)
		{
			return m_beadBag.TakeOutItem(item);
		}
		return m_equipBag.TakeOutItem(item);
	}

	public bool RemoveCountFromStack(ItemInfo item, int count)
	{
		if (item.BagType == m_propBag.BagType)
		{
			return m_propBag.RemoveCountFromStack(item, count);
		}
		if (item.BagType == m_consortiaBag.BagType)
		{
			return m_consortiaBag.RemoveCountFromStack(item, count);
		}
		if (item.BagType == m_beadBag.BagType)
		{
			return m_beadBag.RemoveCountFromStack(item, count);
		}
		return m_equipBag.RemoveCountFromStack(item, count);
	}

	public int isInLimitTimes()
	{
		int num = 10000;
		if (!PlayerCharacter.IsVIPExpire())
		{
			num += int.Parse(GameProperties.VIPQuestFinishDirect.Split('|')[PlayerCharacter.VIPLevel - 1]);
		}
		return num - PlayerCharacter.uesedFinishTime;
	}

	public void AddGift(eGiftType type)
	{
		ItemTemplateInfo itemTemplateInfo = null;
		List<ItemInfo> list = new List<ItemInfo>();
		string[] array = null;
		bool testActive = GameProperties.TestActive;
		switch (type)
		{
		case eGiftType.NEWBIE:
			if (!GameProperties.NewbieEnable && m_character.Grade < GameProperties.BeginLevel)
			{
				LevelInfo levelInfo = LevelMgr.FindLevel(GameProperties.BeginLevel);
				if (levelInfo != null)
				{
					AddGP(levelInfo.GP);
				}
				SendMessage(LanguageMgr.GetTranslation("AddGift.RemoveNewbie"));
			}
			break;
		case eGiftType.MONEY:
			if (testActive)
			{
				AddMoney(GameProperties.FreeMoney);
			}
			break;
		case eGiftType.SMALL_EXP:
			array = GameProperties.FreeExp.Split('|');
			itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(array[0]));
			if (itemTemplateInfo != null)
			{
				list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, Convert.ToInt32(array[1]), 102));
			}
			break;
		case eGiftType.BIG_EXP:
			array = GameProperties.BigExp.Split('|');
			itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(array[0]));
			if (itemTemplateInfo != null && testActive)
			{
				list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, Convert.ToInt32(array[1]), 102));
			}
			break;
		case eGiftType.PET_EXP:
			array = GameProperties.PetExp.Split('|');
			itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(array[0]));
			if (itemTemplateInfo != null && testActive)
			{
				list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, Convert.ToInt32(array[1]), 102));
			}
			break;
		}
		foreach (ItemInfo item in list)
		{
			item.IsBinds = true;
			AddTemplate(item, item.Template.BagType, item.Count, eGameView.dungeonTypeGet);
		}
	}

	public void OnLevelUp(int grade)
	{
		if (this.LevelUp != null)
		{
			this.LevelUp(this);
		}
	}

	private void SetupProcessor()
	{
		m_EquipAmulet = new EquipAmuletProcessor(m_EquipAmuletProcessor);
		m_HorseAmulet = new HorseAmuletProcessor(m_HorseAmuletProcessor);
		m_MemoryGame = new MemoryGameProcessor(m_MemoryGameProcessor);
		m_GodCardRaise = new GodCardRaiseProcessor(m_GodCardRaiseProcessor);
		m_SanXiao = new SanXiaoProcessor(m_SanXiaoProcessor);
		m_GuardCore = new GuardCoreProcessor(m_GuardCoreProcessor);
		m_gameRoom = new GameRoomProcessor(m_gameroomProcessor);
		m_consortia = new ConsortiaProcessor(m_consortiaProcessor);
		m_activeSystemHandler = new ActiveSystemProcessor(m_activeSystemProcessor);
		m_avatarCollection = new AvatarCollectionProcessor(m_avatarCollectionProcessor);
		m_magicStoneHandler = new MagicStoneProcessor(m_magicStoneProcessor);
		m_newHallHandler = new NewHallProcessor(m_newHallProcessor);
		m_collectionTaskHandler = new CollectionTaskProcessor(m_collectionTaskProcessor);
		m_horseHandler = new HorseProcessor(m_horseProcessor);
		m_ringStationHandler = new RingStationProcessor(m_ringStationProcessor);
		m_farmHandler = new FarmProcessor(m_farmProcessor);
		m_petHandler = new PetProcessor(m_petProcessor);
		m_bombKingHandler = new BombKingProcessor(m_bombKingProcessor);
		m_magicHouseHandler = new MagicHouseProcessor(m_magicHouseProcessor);
		m_dragonBoatHandler = new DragonBoatProcessor(m_dragonBoatProcessor);
		m_worshipTheMoonHandler = new WorshipTheMoonProcessor(m_worshipTheMoonProcessor);
		m_magpieBridgeHandler = new MagpieBridgeProcessor(m_magpieBridgeProcessor);
		m_gypsyShopHandler = new GypsyShopProcessor(m_gypsyShopProcessor);
		m_homeSystemHandler = new HomeSystemProcessor(m_homeSystemProcessor);
		m_HomeBarHandler = new HomeBarProcessor(m_HomeBarProcessor);
		m_HomeAdornHandler = new HomeAdornProcessor(m_HomeAdornProcessor);
		m_HomeFightHandler = new HomeFightProcessor(m_HomeFightProcessor);
		m_HotSpringRoomsHandler = new HotSpringRoomsProcessor(m_HotSpringRoomsProcessor);
		m_AnotherDimensionHandler = new AnotherDimensionProcessor(m_AnotherDimensionProcessor);
		m_ZodiacHandler = new ZodiacProcessor(m_ZodiacProcessor);
		m_RenshenHandler = new RenshenProcessor(m_RenshenProcessor);
		m_MoneyTreeHandler = new MoneyTreeProcessor(m_MoneyTreeProcessor);
		m_HomeTemple = new HomeTempleProcessor(m_HomeTempleProcessor);
		m_QiYuan = new QiYuanProcessor(m_QiYuanProcessor);
		CardNewSystemHandler = new CardNewSystemProcessor(m_cardNewSystemLogicProcessor);
		ElfHandler = new ElfProcessor(m_elfLogicProcessor);
		EmblemHandler = new EmblemProcessor(m_EmblemLogicProcessor);
		DevilTurnHandler = new DevilTurnProcessor(m_DevilTurnLogicProcessor);
		TeamHandler = new TeamProcessor(m_TeamLogicProcessor);
		LabyrinthHandler = new LabyrinthProcessor(m_LabyrinthHandlerLogicProcessor);
		ConsortiaTaskHandler = new ConsortiaTaskProcessor(m_consortiaTaskLogicProcessor);
		AcademyHandler = new AcademyProcessor(m_AcademyLogicProcessor);
	}

	public void OnUsingItem(int templateID, int count)
	{
		if (this.AfterUsingItem != null)
		{
			this.AfterUsingItem(templateID, count);
		}
	}

	public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
	{
		PlayerData db = new PlayerData();
		if (game.RoomType == eRoomType.TEAM_ROOM)
		{
			int AddMoney = 0;
			if (isWin)
			{
				TeamMgr.AddScore(db, this, 20, 3, "战斗获胜,获得20点活跃值，30点排位分", ref AddMoney);
				TeamMgr.AddBattleScore(db, this, 30);
			}
			else
			{
				TeamMgr.AddScore(db, this, 5, 3, "战斗失败,获得5点活跃值，扣除5点排位分", ref AddMoney);
				TeamMgr.AddBattleScore(db, this, -5);
			}
		}
		if (game.RoomType == eRoomType.Match)
		{
			if (isWin)
			{
				m_character.Win++;
			}
			m_character.Total++;
			AddLoveScore(1, 0);
			UpdateTrailElite(isWin);
		}
		if (game.RoomType == eRoomType.Dungeon || game.RoomType == eRoomType.Element_PVE)
		{
			AddLoveScore(5, 1);
		}
		if (this.GameOver != null)
		{
			this.GameOver(game, isWin, gainXp);
		}
		ClearFightBuffOneMatch();
	}

	public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum)
	{
		if (this.MissionOver != null)
		{
			this.MissionOver(game, missionId, isWin);
		}
		if (this.MissionFullOver != null)
		{
			this.MissionFullOver(game, missionId, isWin, turnNum);
		}
		if (this.MissionTurnOver != null && isWin)
		{
			this.MissionTurnOver(game, missionId, turnNum);
		}
	}

	public void OnQuestOneKeyFinishEvent(QuestConditionInfo condition)
	{
		if (this.QuestOneKeyFinish != null)
		{
			this.QuestOneKeyFinish(condition);
		}
	}

	public void OnItemStrengthen(int categoryID, int level)
	{
		if (this.ItemStrengthen != null)
		{
			this.ItemStrengthen(categoryID, level);
		}
	}

	public void OnGodTempleImmolation(int star, int level)
	{
		if (this.GodTempleImmolation != null)
		{
			this.GodTempleImmolation(star, level);
		}
	}

	public void OnBuildGpgrade(int level)
	{
		if (this.BuildGpgrade != null)
		{
			this.BuildGpgrade(level);
		}
	}

	public void OnMountMaster(int level)
	{
		if (this.MountMaster != null)
		{
			this.MountMaster(level);
		}
	}

	public void OnPaid(int money, int gold, int offer, int gifttoken, int medal, string payGoods)
	{
		if (this.Paid != null)
		{
			this.Paid(money, gold, offer, gifttoken, medal, payGoods);
		}
	}

	public void OnAdoptPetEvent()
	{
		if (this.AdoptPetEvent != null)
		{
			this.AdoptPetEvent();
		}
	}

	public void OnNewGearEvent(int CategoryID)
	{
		if (this.NewGearEvent != null)
		{
			this.NewGearEvent(CategoryID);
		}
	}

	public void OnCropPrimaryEvent()
	{
		if (this.CropPrimaryEvent != null)
		{
			this.CropPrimaryEvent();
		}
	}

	public void OnSeedFoodPetEvent()
	{
		if (this.SeedFoodPetEvent != null)
		{
			this.SeedFoodPetEvent();
		}
	}

	public void OnUserToemGemstoneEvent(int type)
	{
		if (this.UserToemGemstonetEvent != null)
		{
			this.UserToemGemstonetEvent(type);
		}
	}

	public void OnCollectionTaskEvent(int type)
	{
		if (this.CollectionTaskEvent != null)
		{
			this.CollectionTaskEvent(type);
		}
	}

	public void OnUpLevelPetEvent()
	{
		if (this.UpLevelPetEvent != null)
		{
			this.UpLevelPetEvent();
		}
	}

	public void OnItemInsert()
	{
		if (this.ItemInsert != null)
		{
			this.ItemInsert();
		}
	}

	public void OnItemFusion(int fusionType)
	{
		if (this.ItemFusion != null)
		{
			this.ItemFusion(fusionType);
		}
	}

	public void OnItemMelt(int categoryID)
	{
		if (this.ItemMelt != null)
		{
			this.ItemMelt(categoryID);
		}
	}

	public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage)
	{
		if (this.AfterKillingLiving != null)
		{
			this.AfterKillingLiving(game, type, id, isLiving, damage);
		}
		if (this.GameKillDrop != null && !isLiving)
		{
			this.GameKillDrop(game, type, id, isLiving);
		}
	}

	public void OnGuildChanged()
	{
		if (this.GuildChanged != null)
		{
			this.GuildChanged();
		}
	}

	public void OnItemCompose(int composeType)
	{
		if (this.ItemCompose != null)
		{
			this.ItemCompose(composeType);
		}
	}

	public void OnAchievementFinish(AchievementDataInfo info)
	{
		if (this.AchievementFinishEvent != null)
		{
			this.AchievementFinishEvent(info);
		}
	}

	public void OnFightAddOffer(int offer)
	{
		if (this.FightAddOfferEvent != null)
		{
			this.FightAddOfferEvent(offer);
		}
	}

	public void OnFightOneBloodIsWin(eRoomType roomType)
	{
		if (this.FightOneBloodIsWin != null)
		{
			this.FightOneBloodIsWin(roomType);
		}
	}

	public void AddVipIntegral(int value)
	{
		if (value > 0)
		{
			PlayerCharacter.VipIntegral += value;
			OnPropertiesChanged();
		}
	}

	public int RemoveVipIntegral(int value)
	{
		if (value > 0 && PlayerCharacter.VipIntegral >= value)
		{
			PlayerCharacter.VipIntegral -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public void OnVIPUpgrade(int level, int exp)
	{
		if (this.VIPUpgrade != null)
		{
			this.VIPUpgrade(level, exp);
		}
	}

	public void SendRefreshInfoVipShop()
	{
		if (PlayerCharacter.VipShop == null)
		{
			PlayerCharacter.VipShop = "";
			return;
		}
		string[] array = PlayerCharacter.VipShop.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);
		GSPacketIn gSPacketIn = new GSPacketIn(312, PlayerId);
		gSPacketIn.WriteInt(array.Length);
		if (array.Length != 0)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(',');
				gSPacketIn.WriteInt(int.Parse(array3[0]));
				gSPacketIn.WriteInt(int.Parse(array3[1]));
			}
		}
		Out.SendTCP(gSPacketIn);
	}

	public void OnHotSpingExpAdd(int minutes, int exp)
	{
		if (this.HotSpingExpAdd != null)
		{
			this.HotSpingExpAdd(minutes, exp);
		}
	}

	public void OnGoldCollect(int value)
	{
		if (this.GoldCollect != null)
		{
			this.GoldCollect(value);
		}
	}

	public void OnAcademyEvent(GamePlayer friendly, int type)
	{
		if (this.AcademyEvent != null)
		{
			this.AcademyEvent(friendly, type);
		}
	}

	public void OnGiftTokenCollect(int value)
	{
		if (this.GiftTokenCollect != null)
		{
			this.GiftTokenCollect(value);
		}
	}

	public void OnPingTimeOnline()
	{
		if (this.PingTimeOnline != null)
		{
			this.PingTimeOnline(this);
		}
	}

	public void OnPropertisChange()
	{
		if (this.PropertisChange != null)
		{
			this.PropertisChange(PlayerCharacter);
		}
	}

	public void OnQuestFinish(QuestDataInfo data, QuestInfo info)
	{
		if (this.QuestFinishEvent != null)
		{
			this.QuestFinishEvent(data, info);
		}
	}

	public void OnUseBugle(int value)
	{
		if (this.UseBugle != null)
		{
			this.UseBugle(value);
		}
	}

	internal int GetMedalNum()
	{
		throw new NotImplementedException();
	}

	public void OnDonateRiches(int value, int type)
	{
		if (this.Riches != null)
		{
			this.Riches(value, type);
		}
	}
}

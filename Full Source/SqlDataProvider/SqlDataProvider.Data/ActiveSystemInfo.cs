using System;

namespace SqlDataProvider.Data;

public class ActiveSystemInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _myRank;

	private string _nickName;

	private int _zoneID;

	private string _zoneName;

	private int _totalScore;

	private int _useableScore;

	private int _entertamentPoint;

	private int _dayScore;

	private bool _CanGetGift;

	private int _AvailTime;

	private int _canOpenCounts;

	private int _canEagleEyeCounts;

	private DateTime _lastFlushTime;

	private bool _isShowAll;

	private int _isBuy;

	private int _activeMoney;

	private int _activityTanabataNum;

	private int _challengeNum;

	private int _buyBuffNum;

	private int _damageNum;

	private int _luckystarCoins;

	private DateTime _lastEnterYearMonter;

	private string _boxState;

	private string _CryptBoss;

	private int _dDPlayPoint;

	private DateTime _lastEnterWorshiped;

	private int _updateFreeCounts;

	private int _updateWorshipedCounts;

	private int _update200State;

	private int _luckCount;

	private int _remainTimes;

	private DateTime _lastRefresh;

	private int _curRefreshedTimes;

	private string _chickActiveData;

	private int _currentSelectIndex;

	private int _currentLevel;

	private int _homeTempleExp;

	private int _loginDays;

	private DateTime _lastLoginDays;

	private int _wellAddRate;

	private string _fightPowerSignData;

	private string _SXMapInfoData;

	private int _SXStepRemain;

	private int _SXScore;

	private int _SXCrystal;

	private string _SXRewardsGet;

	private string _MiniShopBuyCount;

	private int _MemoryGameScore;

	private int _MemoryGameCount;

	private string _MemoryGameData;

	private string _MemoryGameAwardGetData;

	private int _cardScore;

	private int _cardFreeCount;

	private int _cardChipCount;

	private string _cardListCard;

	private string _cardListExchange;

	private string _cardListAward;

	private string _rewardQuestData;

	private int _promoteLevel;

	private int _godLevel;

	public int ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
			_isDirty = true;
		}
	}

	public int UserID
	{
		get
		{
			return _userID;
		}
		set
		{
			_userID = value;
			_isDirty = true;
		}
	}

	public int myRank
	{
		get
		{
			return _myRank;
		}
		set
		{
			_myRank = value;
			_isDirty = true;
		}
	}

	public string NickName
	{
		get
		{
			return _nickName;
		}
		set
		{
			_nickName = value;
			_isDirty = true;
		}
	}

	public int ZoneID
	{
		get
		{
			return _zoneID;
		}
		set
		{
			_zoneID = value;
			_isDirty = true;
		}
	}

	public string ZoneName
	{
		get
		{
			return _zoneName;
		}
		set
		{
			_zoneName = value;
			_isDirty = true;
		}
	}

	public int totalScore
	{
		get
		{
			return _totalScore;
		}
		set
		{
			_totalScore = value;
			_isDirty = true;
		}
	}

	public int useableScore
	{
		get
		{
			return _useableScore;
		}
		set
		{
			_useableScore = value;
			_isDirty = true;
		}
	}

	public int EntertamentPoint
	{
		get
		{
			return _entertamentPoint;
		}
		set
		{
			_entertamentPoint = value;
			_isDirty = true;
		}
	}

	public int dayScore
	{
		get
		{
			return _dayScore;
		}
		set
		{
			_dayScore = value;
			_isDirty = true;
		}
	}

	public bool CanGetGift
	{
		get
		{
			return _CanGetGift;
		}
		set
		{
			_CanGetGift = value;
			_isDirty = true;
		}
	}

	public int AvailTime
	{
		get
		{
			return _AvailTime;
		}
		set
		{
			_AvailTime = value;
			_isDirty = true;
		}
	}

	public int canOpenCounts
	{
		get
		{
			return _canOpenCounts;
		}
		set
		{
			_canOpenCounts = value;
			_isDirty = true;
		}
	}

	public int canEagleEyeCounts
	{
		get
		{
			return _canEagleEyeCounts;
		}
		set
		{
			_canEagleEyeCounts = value;
			_isDirty = true;
		}
	}

	public DateTime lastFlushTime
	{
		get
		{
			return _lastFlushTime;
		}
		set
		{
			_lastFlushTime = value;
			_isDirty = true;
		}
	}

	public bool isShowAll
	{
		get
		{
			return _isShowAll;
		}
		set
		{
			_isShowAll = value;
			_isDirty = true;
		}
	}

	public int isBuy
	{
		get
		{
			return _isBuy;
		}
		set
		{
			_isBuy = value;
			_isDirty = true;
		}
	}

	public int ActiveMoney
	{
		get
		{
			return _activeMoney;
		}
		set
		{
			_activeMoney = value;
			_isDirty = true;
		}
	}

	public int activityTanabataNum
	{
		get
		{
			return _activityTanabataNum;
		}
		set
		{
			_activityTanabataNum = value;
			_isDirty = true;
		}
	}

	public int ChallengeNum
	{
		get
		{
			return _challengeNum;
		}
		set
		{
			_challengeNum = value;
			_isDirty = true;
		}
	}

	public int BuyBuffNum
	{
		get
		{
			return _buyBuffNum;
		}
		set
		{
			_buyBuffNum = value;
			_isDirty = true;
		}
	}

	public int DamageNum
	{
		get
		{
			return _damageNum;
		}
		set
		{
			_damageNum = value;
			_isDirty = true;
		}
	}

	public int LuckystarCoins
	{
		get
		{
			return _luckystarCoins;
		}
		set
		{
			_luckystarCoins = value;
			_isDirty = true;
		}
	}

	public DateTime lastEnterYearMonter
	{
		get
		{
			return _lastEnterYearMonter;
		}
		set
		{
			_lastEnterYearMonter = value;
			_isDirty = true;
		}
	}

	public string BoxState
	{
		get
		{
			return _boxState;
		}
		set
		{
			_boxState = value;
			_isDirty = true;
		}
	}

	public string CryptBoss
	{
		get
		{
			return _CryptBoss;
		}
		set
		{
			_CryptBoss = value;
			_isDirty = true;
		}
	}

	public int DDPlayPoint
	{
		get
		{
			return _dDPlayPoint;
		}
		set
		{
			_dDPlayPoint = value;
			_isDirty = true;
		}
	}

	public DateTime lastEnterWorshiped
	{
		get
		{
			return _lastEnterWorshiped;
		}
		set
		{
			_lastEnterWorshiped = value;
			_isDirty = true;
		}
	}

	public int updateFreeCounts
	{
		get
		{
			return _updateFreeCounts;
		}
		set
		{
			_updateFreeCounts = value;
			_isDirty = true;
		}
	}

	public int updateWorshipedCounts
	{
		get
		{
			return _updateWorshipedCounts;
		}
		set
		{
			_updateWorshipedCounts = value;
			_isDirty = true;
		}
	}

	public int update200State
	{
		get
		{
			return _update200State;
		}
		set
		{
			_update200State = value;
			_isDirty = true;
		}
	}

	public int luckCount
	{
		get
		{
			return _luckCount;
		}
		set
		{
			_luckCount = value;
			_isDirty = true;
		}
	}

	public int remainTimes
	{
		get
		{
			return _remainTimes;
		}
		set
		{
			_remainTimes = value;
			_isDirty = true;
		}
	}

	public DateTime LastRefresh
	{
		get
		{
			return _lastRefresh;
		}
		set
		{
			_lastRefresh = value;
			_isDirty = true;
		}
	}

	public int CurRefreshedTimes
	{
		get
		{
			return _curRefreshedTimes;
		}
		set
		{
			_curRefreshedTimes = value;
			_isDirty = true;
		}
	}

	public string ChickActiveData
	{
		get
		{
			return _chickActiveData;
		}
		set
		{
			_chickActiveData = value;
			_isDirty = true;
		}
	}

	public int CurrentSelectIndex
	{
		get
		{
			return _currentSelectIndex;
		}
		set
		{
			_currentSelectIndex = value;
			_isDirty = true;
		}
	}

	public int CurrentLevel
	{
		get
		{
			return _currentLevel;
		}
		set
		{
			_currentLevel = value;
			_isDirty = true;
		}
	}

	public int HomeTempleExp
	{
		get
		{
			return _homeTempleExp;
		}
		set
		{
			_homeTempleExp = value;
			_isDirty = true;
		}
	}

	public int loginDays
	{
		get
		{
			return _loginDays;
		}
		set
		{
			_loginDays = value;
			_isDirty = true;
		}
	}

	public DateTime lastLoginDays
	{
		get
		{
			return _lastLoginDays;
		}
		set
		{
			_lastLoginDays = value;
			_isDirty = true;
		}
	}

	public int wellAddRate
	{
		get
		{
			return _wellAddRate;
		}
		set
		{
			_wellAddRate = value;
			_isDirty = true;
		}
	}

	public string fightPowerSignData
	{
		get
		{
			return _fightPowerSignData;
		}
		set
		{
			_fightPowerSignData = value;
			_isDirty = true;
		}
	}

	public string SXMapInfoData
	{
		get
		{
			return _SXMapInfoData;
		}
		set
		{
			_SXMapInfoData = value;
			_isDirty = true;
		}
	}

	public int SXStepRemain
	{
		get
		{
			return _SXStepRemain;
		}
		set
		{
			_SXStepRemain = value;
			_isDirty = true;
		}
	}

	public int SXScore
	{
		get
		{
			return _SXScore;
		}
		set
		{
			_SXScore = value;
			_isDirty = true;
		}
	}

	public int SXCrystal
	{
		get
		{
			return _SXCrystal;
		}
		set
		{
			_SXCrystal = value;
			_isDirty = true;
		}
	}

	public string SXRewardsGet
	{
		get
		{
			return _SXRewardsGet;
		}
		set
		{
			_SXRewardsGet = value;
			_isDirty = true;
		}
	}

	public string MiniShopBuyCount
	{
		get
		{
			return _MiniShopBuyCount;
		}
		set
		{
			_MiniShopBuyCount = value;
			_isDirty = true;
		}
	}

	public int MemoryGameScore
	{
		get
		{
			return _MemoryGameScore;
		}
		set
		{
			_MemoryGameScore = value;
			_isDirty = true;
		}
	}

	public int MemoryGameCount
	{
		get
		{
			return _MemoryGameCount;
		}
		set
		{
			_MemoryGameCount = value;
			_isDirty = true;
		}
	}

	public string MemoryGameData
	{
		get
		{
			return _MemoryGameData;
		}
		set
		{
			_MemoryGameData = value;
			_isDirty = true;
		}
	}

	public string MemoryGameAwardGetData
	{
		get
		{
			return _MemoryGameAwardGetData;
		}
		set
		{
			_MemoryGameAwardGetData = value;
			_isDirty = true;
		}
	}

	public int cardScore
	{
		get
		{
			return _cardScore;
		}
		set
		{
			_cardScore = value;
			_isDirty = true;
		}
	}

	public int cardFreeCount
	{
		get
		{
			return _cardFreeCount;
		}
		set
		{
			_cardFreeCount = value;
			_isDirty = true;
		}
	}

	public int cardChipCount
	{
		get
		{
			return _cardChipCount;
		}
		set
		{
			_cardChipCount = value;
			_isDirty = true;
		}
	}

	public string cardListCard
	{
		get
		{
			return _cardListCard;
		}
		set
		{
			_cardListCard = value;
			_isDirty = true;
		}
	}

	public string cardListExchange
	{
		get
		{
			return _cardListExchange;
		}
		set
		{
			_cardListExchange = value;
			_isDirty = true;
		}
	}

	public string cardListAward
	{
		get
		{
			return _cardListAward;
		}
		set
		{
			_cardListAward = value;
			_isDirty = true;
		}
	}

	public string rewardQuestData
	{
		get
		{
			return _rewardQuestData;
		}
		set
		{
			_rewardQuestData = value;
			_isDirty = true;
		}
	}

	public int promoteLevel
	{
		get
		{
			return _promoteLevel;
		}
		set
		{
			_promoteLevel = value;
			_isDirty = true;
		}
	}

	public int godLevel
	{
		get
		{
			return _godLevel;
		}
		set
		{
			_godLevel = value;
			_isDirty = true;
		}
	}
}

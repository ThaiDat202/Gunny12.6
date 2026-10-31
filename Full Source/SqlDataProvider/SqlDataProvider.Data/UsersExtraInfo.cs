using System;

namespace SqlDataProvider.Data;

public class UsersExtraInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _starlevel;

	private int _nowPosition;

	private int _freeCount;

	private int _score;

	private int _summerScore;

	private int _prizeStatus;

	private bool _cakeStatus;

	private string _catchInsectGetPrize;

	private int _scoreMagicstone;

	private int _normalFightNum;

	private int _hardFightNum;

	private bool _isDoubleScore;

	private string _magpieBridgeItems;

	private int _nowPositionMB;

	private int _lastNum;

	private int _magpieNum;

	private int _freeSendMailCount;

	private int _freePresentGoodsCount;

	private int _freeAddAutionCount;

	private int _missionEnergy;

	private int _energyTotal;

	private int _buyEnergyCount;

	private string _searchGoodItems;

	private string _kingBlessInfo;

	private DateTime _kingBlessEnddate;

	private int _kingBlessIndex;

	private string _deedInfo;

	private DateTime _deedEnddate;

	private int _deedIndex;

	private int _curentDressModel;

	private string _dressModelArr;

	private int _coupleNum;

	private int _dungeonNum;

	private int _propsNum;

	private DateTime _silverTime;

	private int _silverCount;

	private DateTime _goldTime;

	private string _dungeonCount;

	private bool _loginDeviceDownAward;

	private bool _loginDeviceDailyAward;

	private int _curcentRoomBordenTemplateId;

	private string _pvePowerBuffData;

	private int _SoulCrystal;

	private DateTime _FreeTimeMark;

	private int _FreeCountMark;

	private int _Coin;

	private int _SummonCount1;

	private int _SummonCount2;

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

	public int starlevel
	{
		get
		{
			return _starlevel;
		}
		set
		{
			_starlevel = value;
			_isDirty = true;
		}
	}

	public int nowPosition
	{
		get
		{
			return _nowPosition;
		}
		set
		{
			_nowPosition = value;
			_isDirty = true;
		}
	}

	public int FreeCount
	{
		get
		{
			return _freeCount;
		}
		set
		{
			_freeCount = value;
			_isDirty = true;
		}
	}

	public int Score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
			_isDirty = true;
		}
	}

	public int SummerScore
	{
		get
		{
			return _summerScore;
		}
		set
		{
			_summerScore = value;
			_isDirty = true;
		}
	}

	public int PrizeStatus
	{
		get
		{
			return _prizeStatus;
		}
		set
		{
			_prizeStatus = value;
			_isDirty = true;
		}
	}

	public bool CakeStatus
	{
		get
		{
			return _cakeStatus;
		}
		set
		{
			_cakeStatus = value;
			_isDirty = true;
		}
	}

	public string CatchInsectGetPrize
	{
		get
		{
			return _catchInsectGetPrize;
		}
		set
		{
			_catchInsectGetPrize = value;
			_isDirty = true;
		}
	}

	public int ScoreMagicstone
	{
		get
		{
			return _scoreMagicstone;
		}
		set
		{
			_scoreMagicstone = value;
			_isDirty = true;
		}
	}

	public int NormalFightNum
	{
		get
		{
			return _normalFightNum;
		}
		set
		{
			_normalFightNum = value;
			_isDirty = true;
		}
	}

	public int HardFightNum
	{
		get
		{
			return _hardFightNum;
		}
		set
		{
			_hardFightNum = value;
			_isDirty = true;
		}
	}

	public bool IsDoubleScore
	{
		get
		{
			return _isDoubleScore;
		}
		set
		{
			_isDoubleScore = value;
			_isDirty = true;
		}
	}

	public string MagpieBridgeItems
	{
		get
		{
			return _magpieBridgeItems;
		}
		set
		{
			_magpieBridgeItems = value;
			_isDirty = true;
		}
	}

	public int NowPositionMB
	{
		get
		{
			return _nowPositionMB;
		}
		set
		{
			_nowPositionMB = value;
			_isDirty = true;
		}
	}

	public int LastNum
	{
		get
		{
			return _lastNum;
		}
		set
		{
			_lastNum = value;
			_isDirty = true;
		}
	}

	public int MagpieNum
	{
		get
		{
			return _magpieNum;
		}
		set
		{
			_magpieNum = value;
			_isDirty = true;
		}
	}

	public int FreeSendMailCount
	{
		get
		{
			return _freeSendMailCount;
		}
		set
		{
			_freeSendMailCount = value;
			_isDirty = true;
		}
	}

	public int FreePresentGoodsCount
	{
		get
		{
			return _freePresentGoodsCount;
		}
		set
		{
			_freePresentGoodsCount = value;
			_isDirty = true;
		}
	}

	public int FreeAddAutionCount
	{
		get
		{
			return _freeAddAutionCount;
		}
		set
		{
			_freeAddAutionCount = value;
			_isDirty = true;
		}
	}

	public int MissionEnergy
	{
		get
		{
			return _missionEnergy;
		}
		set
		{
			_missionEnergy = value;
			_isDirty = true;
		}
	}

	public int EnergyTotal
	{
		get
		{
			return _energyTotal;
		}
		set
		{
			_energyTotal = value;
			_isDirty = true;
		}
	}

	public int buyEnergyCount
	{
		get
		{
			return _buyEnergyCount;
		}
		set
		{
			_buyEnergyCount = value;
			_isDirty = true;
		}
	}

	public string SearchGoodItems
	{
		get
		{
			return _searchGoodItems;
		}
		set
		{
			_searchGoodItems = value;
			_isDirty = true;
		}
	}

	public string KingBlessInfo
	{
		get
		{
			return _kingBlessInfo;
		}
		set
		{
			_kingBlessInfo = value;
			_isDirty = true;
		}
	}

	public DateTime KingBlessEnddate
	{
		get
		{
			return _kingBlessEnddate;
		}
		set
		{
			_kingBlessEnddate = value;
			_isDirty = true;
		}
	}

	public int KingBlessIndex
	{
		get
		{
			return _kingBlessIndex;
		}
		set
		{
			_kingBlessIndex = value;
			_isDirty = true;
		}
	}

	public string DeedInfo
	{
		get
		{
			return _deedInfo;
		}
		set
		{
			_deedInfo = value;
			_isDirty = true;
		}
	}

	public DateTime DeedEnddate
	{
		get
		{
			return _deedEnddate;
		}
		set
		{
			_deedEnddate = value;
			_isDirty = true;
		}
	}

	public int DeedIndex
	{
		get
		{
			return _deedIndex;
		}
		set
		{
			_deedIndex = value;
			_isDirty = true;
		}
	}

	public int CurentDressModel
	{
		get
		{
			return _curentDressModel;
		}
		set
		{
			_curentDressModel = value;
			_isDirty = true;
		}
	}

	public string DressModelArr
	{
		get
		{
			return _dressModelArr;
		}
		set
		{
			_dressModelArr = value;
			_isDirty = true;
		}
	}

	public int coupleNum
	{
		get
		{
			return _coupleNum;
		}
		set
		{
			_coupleNum = value;
			_isDirty = true;
		}
	}

	public int dungeonNum
	{
		get
		{
			return _dungeonNum;
		}
		set
		{
			_dungeonNum = value;
			_isDirty = true;
		}
	}

	public int propsNum
	{
		get
		{
			return _propsNum;
		}
		set
		{
			_propsNum = value;
			_isDirty = true;
		}
	}

	public DateTime silverTime
	{
		get
		{
			return _silverTime;
		}
		set
		{
			_silverTime = value;
			_isDirty = true;
		}
	}

	public int silverCount
	{
		get
		{
			return _silverCount;
		}
		set
		{
			_silverCount = value;
			_isDirty = true;
		}
	}

	public DateTime goldTime
	{
		get
		{
			return _goldTime;
		}
		set
		{
			_goldTime = value;
			_isDirty = true;
		}
	}

	public string dungeonCount
	{
		get
		{
			return _dungeonCount;
		}
		set
		{
			_dungeonCount = value;
			_isDirty = true;
		}
	}

	public bool loginDeviceDownAward
	{
		get
		{
			return _loginDeviceDownAward;
		}
		set
		{
			_loginDeviceDownAward = value;
			_isDirty = true;
		}
	}

	public bool loginDeviceDailyAward
	{
		get
		{
			return _loginDeviceDailyAward;
		}
		set
		{
			_loginDeviceDailyAward = value;
			_isDirty = true;
		}
	}

	public int curcentRoomBordenTemplateId
	{
		get
		{
			return _curcentRoomBordenTemplateId;
		}
		set
		{
			_curcentRoomBordenTemplateId = value;
			_isDirty = true;
		}
	}

	public string pvePowerBuffData
	{
		get
		{
			return _pvePowerBuffData;
		}
		set
		{
			_pvePowerBuffData = value;
			_isDirty = true;
		}
	}

	public int SoulCrystal
	{
		get
		{
			return _SoulCrystal;
		}
		set
		{
			_SoulCrystal = value;
			_isDirty = true;
		}
	}

	public DateTime FreeTimeMark
	{
		get
		{
			return _FreeTimeMark;
		}
		set
		{
			_FreeTimeMark = value;
			_isDirty = true;
		}
	}

	public int FreeCountMark
	{
		get
		{
			return _FreeCountMark;
		}
		set
		{
			_FreeCountMark = value;
			_isDirty = true;
		}
	}

	public int Coin
	{
		get
		{
			return _Coin;
		}
		set
		{
			_Coin = value;
			_isDirty = true;
		}
	}

	public int SummonCount1
	{
		get
		{
			return _SummonCount1;
		}
		set
		{
			_SummonCount1 = value;
			_isDirty = true;
		}
	}

	public int SummonCount2
	{
		get
		{
			return _SummonCount2;
		}
		set
		{
			_SummonCount2 = value;
			_isDirty = true;
		}
	}

	public bool KingBlessRenevalDays(int value, int index)
	{
		if (index < _kingBlessIndex)
		{
			return false;
		}
		if (_kingBlessEnddate < DateTime.Now)
		{
			_kingBlessEnddate = DateTime.Now;
		}
		_kingBlessEnddate = _kingBlessEnddate.AddDays(value);
		_kingBlessIndex = index;
		return true;
	}

	public bool DeedRenevalDays(int value, int index)
	{
		if (_deedEnddate < DateTime.Now)
		{
			_deedEnddate = DateTime.Now;
		}
		_deedEnddate = _deedEnddate.AddDays(value);
		_deedIndex = index;
		return true;
	}
}

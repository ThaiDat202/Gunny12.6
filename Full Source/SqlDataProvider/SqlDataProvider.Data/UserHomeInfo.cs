using System;

namespace SqlDataProvider.Data;

public class UserHomeInfo : DataObject
{
	private int _id;

	private int _userID;

	private int _houseStage;

	private int _energyCount;

	private int _drinkCount;

	private DateTime _cdEndTime;

	private int _pourCount;

	private int _tasteCount;

	private int _enableFight;

	private int _gold;

	private int _blood;

	private string _yardAdornInfo;

	private string _houseAdornInfo;

	private string _logTaste;

	private DateTime _lastLogin;

	private int _fishGameTime;

	private int _fishScore;

	private int _dayScore;

	private int _maxScore;

	private int _remainCount;

	private string _moneyTrees;

	private int _numRedPkgRemain;

	public int ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
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

	public int houseStage
	{
		get
		{
			return _houseStage;
		}
		set
		{
			_houseStage = value;
			_isDirty = true;
		}
	}

	public int energyCount
	{
		get
		{
			return _energyCount;
		}
		set
		{
			_energyCount = value;
			_isDirty = true;
		}
	}

	public int drinkCount
	{
		get
		{
			return _drinkCount;
		}
		set
		{
			_drinkCount = value;
			_isDirty = true;
		}
	}

	public DateTime cdEndTime
	{
		get
		{
			return _cdEndTime;
		}
		set
		{
			_cdEndTime = value;
			_isDirty = true;
		}
	}

	public int pourCount
	{
		get
		{
			return _pourCount;
		}
		set
		{
			_pourCount = value;
			_isDirty = true;
		}
	}

	public int tasteCount
	{
		get
		{
			return _tasteCount;
		}
		set
		{
			_tasteCount = value;
			_isDirty = true;
		}
	}

	public int enableFight
	{
		get
		{
			return _enableFight;
		}
		set
		{
			_enableFight = value;
			_isDirty = true;
		}
	}

	public int Gold
	{
		get
		{
			return _gold;
		}
		set
		{
			_gold = value;
			_isDirty = true;
		}
	}

	public int Blood
	{
		get
		{
			return _blood;
		}
		set
		{
			_blood = value;
			_isDirty = true;
		}
	}

	public string yardAdornInfo
	{
		get
		{
			return _yardAdornInfo;
		}
		set
		{
			_yardAdornInfo = value;
			_isDirty = true;
		}
	}

	public string houseAdornInfo
	{
		get
		{
			return _houseAdornInfo;
		}
		set
		{
			_houseAdornInfo = value;
			_isDirty = true;
		}
	}

	public string LogTaste
	{
		get
		{
			return _logTaste;
		}
		set
		{
			_logTaste = value;
			_isDirty = true;
		}
	}

	public DateTime LastLogin
	{
		get
		{
			return _lastLogin;
		}
		set
		{
			_lastLogin = value;
			_isDirty = true;
		}
	}

	public int fishGameTime
	{
		get
		{
			return _fishGameTime;
		}
		set
		{
			_fishGameTime = value;
			_isDirty = true;
		}
	}

	public int fishScore
	{
		get
		{
			return _fishScore;
		}
		set
		{
			_fishScore = value;
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

	public int maxScore
	{
		get
		{
			return _maxScore;
		}
		set
		{
			_maxScore = value;
			_isDirty = true;
		}
	}

	public int remainCount
	{
		get
		{
			return _remainCount;
		}
		set
		{
			_remainCount = value;
			_isDirty = true;
		}
	}

	public string MoneyTrees
	{
		get
		{
			return _moneyTrees;
		}
		set
		{
			_moneyTrees = value;
			_isDirty = true;
		}
	}

	public int NumRedPkgRemain
	{
		get
		{
			return _numRedPkgRemain;
		}
		set
		{
			_numRedPkgRemain = value;
			_isDirty = true;
		}
	}
}

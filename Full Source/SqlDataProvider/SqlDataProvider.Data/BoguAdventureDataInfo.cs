using System;

namespace SqlDataProvider.Data;

public class BoguAdventureDataInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _currentIndex;

	private int _hp;

	private int _isAcquireAward1;

	private int _isAcquireAward2;

	private int _isAcquireAward3;

	private int _openCount;

	private bool _isFreeReset;

	private int _resetCount;

	private string _cellInfo;

	private string _awardCount;

	private DateTime _lastEnterGame;

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

	public int currentIndex
	{
		get
		{
			return _currentIndex;
		}
		set
		{
			_currentIndex = value;
			_isDirty = true;
		}
	}

	public int hp
	{
		get
		{
			return _hp;
		}
		set
		{
			_hp = value;
			_isDirty = true;
		}
	}

	public int isAcquireAward1
	{
		get
		{
			return _isAcquireAward1;
		}
		set
		{
			_isAcquireAward1 = value;
			_isDirty = true;
		}
	}

	public int isAcquireAward2
	{
		get
		{
			return _isAcquireAward2;
		}
		set
		{
			_isAcquireAward2 = value;
			_isDirty = true;
		}
	}

	public int isAcquireAward3
	{
		get
		{
			return _isAcquireAward3;
		}
		set
		{
			_isAcquireAward3 = value;
			_isDirty = true;
		}
	}

	public int openCount
	{
		get
		{
			return _openCount;
		}
		set
		{
			_openCount = value;
			_isDirty = true;
		}
	}

	public bool isFreeReset
	{
		get
		{
			return _isFreeReset;
		}
		set
		{
			_isFreeReset = value;
			_isDirty = true;
		}
	}

	public int resetCount
	{
		get
		{
			return _resetCount;
		}
		set
		{
			_resetCount = value;
			_isDirty = true;
		}
	}

	public string cellInfo
	{
		get
		{
			return _cellInfo;
		}
		set
		{
			_cellInfo = value;
			_isDirty = true;
		}
	}

	public string awardCount
	{
		get
		{
			return _awardCount;
		}
		set
		{
			_awardCount = value;
			_isDirty = true;
		}
	}

	public DateTime lastEnterGame
	{
		get
		{
			return _lastEnterGame;
		}
		set
		{
			_lastEnterGame = value;
			_isDirty = true;
		}
	}
}

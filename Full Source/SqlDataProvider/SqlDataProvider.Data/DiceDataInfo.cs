namespace SqlDataProvider.Data;

public class DiceDataInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _luckIntegral;

	private int _luckIntegralLevel;

	private int _level;

	private int _freeCount;

	private int _currentPosition;

	private bool _userFirstCell;

	private string _awardArray;

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

	public int LuckIntegral
	{
		get
		{
			return _luckIntegral;
		}
		set
		{
			_luckIntegral = value;
			_isDirty = true;
		}
	}

	public int LuckIntegralLevel
	{
		get
		{
			return _luckIntegralLevel;
		}
		set
		{
			_luckIntegralLevel = value;
			_isDirty = true;
		}
	}

	public int Level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
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

	public int CurrentPosition
	{
		get
		{
			return _currentPosition;
		}
		set
		{
			_currentPosition = value;
			_isDirty = true;
		}
	}

	public bool UserFirstCell
	{
		get
		{
			return _userFirstCell;
		}
		set
		{
			_userFirstCell = value;
			_isDirty = true;
		}
	}

	public string AwardArray
	{
		get
		{
			return _awardArray;
		}
		set
		{
			_awardArray = value;
			_isDirty = true;
		}
	}
}

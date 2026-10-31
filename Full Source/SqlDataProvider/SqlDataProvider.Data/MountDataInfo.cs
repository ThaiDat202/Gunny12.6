namespace SqlDataProvider.Data;

public class MountDataInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _curUseHorse;

	private int _curLevel;

	private int _curExp;

	private string _curHasSkill;

	private string _horsePicCherish;

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

	public int curUseHorse
	{
		get
		{
			return _curUseHorse;
		}
		set
		{
			_curUseHorse = value;
			_isDirty = true;
		}
	}

	public int curLevel
	{
		get
		{
			return _curLevel;
		}
		set
		{
			_curLevel = value;
			_isDirty = true;
		}
	}

	public int curExp
	{
		get
		{
			return _curExp;
		}
		set
		{
			_curExp = value;
			_isDirty = true;
		}
	}

	public string curHasSkill
	{
		get
		{
			return _curHasSkill;
		}
		set
		{
			_curHasSkill = value;
			_isDirty = true;
		}
	}

	public string horsePicCherish
	{
		get
		{
			return _horsePicCherish;
		}
		set
		{
			_horsePicCherish = value;
			_isDirty = true;
		}
	}
}

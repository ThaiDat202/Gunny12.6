namespace SqlDataProvider.Data;

public class KingGradeDataInfo : DataObject
{
	private int _userID;

	private int _ddtKingGrade;

	private int _currentExp;

	private int _usableNum;

	private int _attackLevel;

	private int _defenceLevel;

	private int _agilityLevel;

	private int _luckyLevel;

	private int _magicAttackLevel;

	private int _magicDefenceLevel;

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

	public int ddtKingGrade
	{
		get
		{
			return _ddtKingGrade;
		}
		set
		{
			_ddtKingGrade = value;
			_isDirty = true;
		}
	}

	public int currentExp
	{
		get
		{
			return _currentExp;
		}
		set
		{
			_currentExp = value;
			_isDirty = true;
		}
	}

	public int usableNum
	{
		get
		{
			return _usableNum;
		}
		set
		{
			if (value < 0)
			{
				_usableNum = 0;
			}
			else
			{
				_usableNum = value;
			}
			_isDirty = true;
		}
	}

	public int AttackLevel
	{
		get
		{
			return _attackLevel;
		}
		set
		{
			_attackLevel = value;
			_isDirty = true;
		}
	}

	public int DefenceLevel
	{
		get
		{
			return _defenceLevel;
		}
		set
		{
			_defenceLevel = value;
			_isDirty = true;
		}
	}

	public int AgilityLevel
	{
		get
		{
			return _agilityLevel;
		}
		set
		{
			_agilityLevel = value;
			_isDirty = true;
		}
	}

	public int LuckyLevel
	{
		get
		{
			return _luckyLevel;
		}
		set
		{
			_luckyLevel = value;
			_isDirty = true;
		}
	}

	public int MagicAttackLevel
	{
		get
		{
			return _magicAttackLevel;
		}
		set
		{
			_magicAttackLevel = value;
			_isDirty = true;
		}
	}

	public int MagicDefenceLevel
	{
		get
		{
			return _magicDefenceLevel;
		}
		set
		{
			_magicDefenceLevel = value;
			_isDirty = true;
		}
	}
}

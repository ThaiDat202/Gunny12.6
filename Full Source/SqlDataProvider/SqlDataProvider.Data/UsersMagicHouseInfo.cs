using System;

namespace SqlDataProvider.Data;

public class UsersMagicHouseInfo : DataObject
{
	private int _iD;

	private int _userID;

	private bool _isOpen;

	private bool _isMagicRoomShow;

	private int _magicJuniorLv;

	private int _magicJuniorExp;

	private int _magicMidLv;

	private int _magicMidExp;

	private int _magicSeniorLv;

	private int _magicSeniorExp;

	private int _freeGetCount;

	private DateTime _freeGetTime;

	private int _chargeGetCount;

	private DateTime _chargeGetTime;

	private int _depotCount;

	private int _essence;

	private string _activityWeapons;

	public int ID
	{
		get
		{
			return _iD;
		}
		set
		{
			_iD = value;
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

	public bool isOpen
	{
		get
		{
			return _isOpen;
		}
		set
		{
			_isOpen = value;
			_isDirty = true;
		}
	}

	public bool isMagicRoomShow
	{
		get
		{
			return _isMagicRoomShow;
		}
		set
		{
			_isMagicRoomShow = value;
			_isDirty = true;
		}
	}

	public int magicJuniorLv
	{
		get
		{
			return _magicJuniorLv;
		}
		set
		{
			_magicJuniorLv = value;
			_isDirty = true;
		}
	}

	public int magicJuniorExp
	{
		get
		{
			return _magicJuniorExp;
		}
		set
		{
			_magicJuniorExp = value;
			_isDirty = true;
		}
	}

	public int magicMidLv
	{
		get
		{
			return _magicMidLv;
		}
		set
		{
			_magicMidLv = value;
			_isDirty = true;
		}
	}

	public int magicMidExp
	{
		get
		{
			return _magicMidExp;
		}
		set
		{
			_magicMidExp = value;
			_isDirty = true;
		}
	}

	public int magicSeniorLv
	{
		get
		{
			return _magicSeniorLv;
		}
		set
		{
			_magicSeniorLv = value;
			_isDirty = true;
		}
	}

	public int magicSeniorExp
	{
		get
		{
			return _magicSeniorExp;
		}
		set
		{
			_magicSeniorExp = value;
			_isDirty = true;
		}
	}

	public int freeGetCount
	{
		get
		{
			return _freeGetCount;
		}
		set
		{
			_freeGetCount = value;
			_isDirty = true;
		}
	}

	public DateTime freeGetTime
	{
		get
		{
			return _freeGetTime;
		}
		set
		{
			_freeGetTime = value;
			_isDirty = true;
		}
	}

	public int chargeGetCount
	{
		get
		{
			return _chargeGetCount;
		}
		set
		{
			_chargeGetCount = value;
			_isDirty = true;
		}
	}

	public DateTime chargeGetTime
	{
		get
		{
			return _chargeGetTime;
		}
		set
		{
			_chargeGetTime = value;
			_isDirty = true;
		}
	}

	public int depotCount
	{
		get
		{
			return _depotCount;
		}
		set
		{
			_depotCount = value;
			_isDirty = true;
		}
	}

	public int essence
	{
		get
		{
			return _essence;
		}
		set
		{
			_essence = value;
			_isDirty = true;
		}
	}

	public string activityWeapons
	{
		get
		{
			return _activityWeapons;
		}
		set
		{
			_activityWeapons = value;
			_isDirty = true;
		}
	}
}

using System;
using System.Linq;

namespace SqlDataProvider.Data;

public class UserAvatarColectionInfo : DataObject
{
	private int _ID;

	private int _userID;

	private DateTime _endTime;

	private int _dataId;

	private int _activeCount;

	private int _sex;

	private string _activeDress;

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

	public DateTime endTime
	{
		get
		{
			return _endTime;
		}
		set
		{
			_endTime = value;
			_isDirty = true;
		}
	}

	public bool isValidate => _endTime.Date >= DateTime.Now.Date;

	public int dataId
	{
		get
		{
			return _dataId;
		}
		set
		{
			_dataId = value;
			_isDirty = true;
		}
	}

	public int ActiveCount
	{
		get
		{
			return _activeCount;
		}
		set
		{
			_activeCount = value;
			_isDirty = true;
		}
	}

	public int Sex
	{
		get
		{
			return _sex;
		}
		set
		{
			_sex = value;
			_isDirty = true;
		}
	}

	public string ActiveDress
	{
		get
		{
			return ConvertArray();
		}
		set
		{
			_activeDress = value;
			_isDirty = true;
		}
	}

	public string[] CurrentGroup
	{
		get
		{
			if (string.IsNullOrEmpty(_activeDress))
			{
				return new string[0];
			}
			return _activeDress.Split(',');
		}
	}

	public string ConvertArray()
	{
		if (string.IsNullOrEmpty(_activeDress))
		{
			return "";
		}
		string text = CurrentGroup.Aggregate("", (string current, string id) => current + "," + id);
		return text.Substring(1);
	}

	public bool HaftActive()
	{
		return CurrentGroup.Length >= ActiveCount / 2;
	}

	public bool FullActive()
	{
		return CurrentGroup.Length >= ActiveCount;
	}
}

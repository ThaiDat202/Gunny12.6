using System;

namespace SqlDataProvider.Data;

public class UserBankInfo : DataObject
{
	private int _BankID;

	private int _TempID;

	private int _UserID;

	private int _Amount;

	private DateTime _BeginTime;

	private bool _IsExits;

	public int BankID
	{
		get
		{
			return _BankID;
		}
		set
		{
			_BankID = value;
			_isDirty = true;
		}
	}

	public int TempID
	{
		get
		{
			return _TempID;
		}
		set
		{
			_TempID = value;
			_isDirty = true;
		}
	}

	public int UserID
	{
		get
		{
			return _UserID;
		}
		set
		{
			_UserID = value;
			_isDirty = true;
		}
	}

	public int Amount
	{
		get
		{
			return _Amount;
		}
		set
		{
			_Amount = value;
			_isDirty = true;
		}
	}

	public DateTime BeginTime
	{
		get
		{
			return _BeginTime;
		}
		set
		{
			_BeginTime = value;
			_isDirty = true;
		}
	}

	public bool IsExits
	{
		get
		{
			return _IsExits;
		}
		set
		{
			_IsExits = value;
			_isDirty = true;
		}
	}

	public bool IsAchieve(int month)
	{
		int days = (DateTime.Now - _BeginTime).Days;
		if (days >= month * 30)
		{
			return true;
		}
		return false;
	}
}

using System;

namespace SqlDataProvider.Data;

public class ConsortiaRedPackageInfo : DataObject
{
	private int _ID;

	private int _ConsortiaID;

	private int _UserID;

	private string _NickName;

	private int _Money;

	private int _Package;

	private string _WishWord;

	private string _ReceiveData;

	private DateTime _TimeCreate;

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

	public int ConsortiaID
	{
		get
		{
			return _ConsortiaID;
		}
		set
		{
			_ConsortiaID = value;
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

	public string NickName
	{
		get
		{
			return _NickName;
		}
		set
		{
			_NickName = value;
			_isDirty = true;
		}
	}

	public int Money
	{
		get
		{
			return _Money;
		}
		set
		{
			_Money = value;
			_isDirty = true;
		}
	}

	public int Package
	{
		get
		{
			return _Package;
		}
		set
		{
			_Package = value;
			_isDirty = true;
		}
	}

	public string WishWord
	{
		get
		{
			return _WishWord;
		}
		set
		{
			_WishWord = value;
			_isDirty = true;
		}
	}

	public string ReceiveData
	{
		get
		{
			return _ReceiveData;
		}
		set
		{
			_ReceiveData = value;
			_isDirty = true;
		}
	}

	public DateTime TimeCreate
	{
		get
		{
			return _TimeCreate;
		}
		set
		{
			_TimeCreate = value;
			_isDirty = true;
		}
	}
}

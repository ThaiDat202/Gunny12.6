using System;

namespace SqlDataProvider.Data;

public class ServerDataInfo : DataObject
{
	private string _Name;

	private string _ValString;

	private int _ValInt;

	private DateTime _ValDateTime;

	private byte[] _ValBinary;

	public string Name
	{
		get
		{
			return _Name;
		}
		set
		{
			_Name = value;
			_isDirty = true;
		}
	}

	public string ValString
	{
		get
		{
			return _ValString;
		}
		set
		{
			_ValString = value;
			_isDirty = true;
		}
	}

	public int ValInt
	{
		get
		{
			return _ValInt;
		}
		set
		{
			_ValInt = value;
			_isDirty = true;
		}
	}

	public DateTime ValDateTime
	{
		get
		{
			return _ValDateTime;
		}
		set
		{
			_ValDateTime = value;
			_isDirty = true;
		}
	}

	public byte[] ValBinary
	{
		get
		{
			return _ValBinary;
		}
		set
		{
			_ValBinary = value;
			_isDirty = true;
		}
	}
}

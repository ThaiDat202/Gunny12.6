using System;

namespace SqlDataProvider.Data;

public class UserMinesInfo : DataObject
{
	private int _UserID;

	private int _ToolExp;

	private int _HeadExp;

	private int _ClothExp;

	private int _WeaponExp;

	private int _ShieldExp;

	private DateTime _LastTimeDig;

	private int _TotalTimesDig;

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

	public int ToolExp
	{
		get
		{
			return _ToolExp;
		}
		set
		{
			_ToolExp = value;
			_isDirty = true;
		}
	}

	public int HeadExp
	{
		get
		{
			return _HeadExp;
		}
		set
		{
			_HeadExp = value;
			_isDirty = true;
		}
	}

	public int ClothExp
	{
		get
		{
			return _ClothExp;
		}
		set
		{
			_ClothExp = value;
			_isDirty = true;
		}
	}

	public int WeaponExp
	{
		get
		{
			return _WeaponExp;
		}
		set
		{
			_WeaponExp = value;
			_isDirty = true;
		}
	}

	public int ShieldExp
	{
		get
		{
			return _ShieldExp;
		}
		set
		{
			_ShieldExp = value;
			_isDirty = true;
		}
	}

	public DateTime LastTimeDig
	{
		get
		{
			return _LastTimeDig;
		}
		set
		{
			_LastTimeDig = value;
			_isDirty = true;
		}
	}

	public int TotalTimesDig
	{
		get
		{
			return _TotalTimesDig;
		}
		set
		{
			_TotalTimesDig = value;
			_isDirty = true;
		}
	}
}

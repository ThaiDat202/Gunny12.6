using System;

namespace SqlDataProvider.Data;

public class UserBeautyVoteInfo : DataObject
{
	private int _UserID;

	private string _UserName;

	private string _NickName;

	private int _Rank;

	private int _Count;

	private string _Style;

	private string _Color;

	private DateTime _TimeCreate;

	private bool _IsSendAward;

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

	public string UserName
	{
		get
		{
			return _UserName;
		}
		set
		{
			_UserName = value;
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

	public int Rank
	{
		get
		{
			return _Rank;
		}
		set
		{
			_Rank = value;
			_isDirty = true;
		}
	}

	public int Count
	{
		get
		{
			return _Count;
		}
		set
		{
			_Count = value;
			_isDirty = true;
		}
	}

	public string Style
	{
		get
		{
			return _Style;
		}
		set
		{
			_Style = value;
			_isDirty = true;
		}
	}

	public string Color
	{
		get
		{
			return _Color;
		}
		set
		{
			_Color = value;
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

	public bool IsSendAward
	{
		get
		{
			return _IsSendAward;
		}
		set
		{
			_IsSendAward = value;
			_isDirty = true;
		}
	}
}

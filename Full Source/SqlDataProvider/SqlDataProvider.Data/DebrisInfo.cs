using System;

namespace SqlDataProvider.Data;

public class DebrisInfo
{
	private int _ID;

	private int _chapterID;

	private int _pageID;

	private DateTime _date;

	private bool _isDirty;

	public bool IsDirty
	{
		get
		{
			return _isDirty;
		}
		set
		{
			_isDirty = value;
		}
	}

	[SqlField]
	public int ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_isDirty = true;
			_ID = value;
		}
	}

	[SqlField]
	public int chapterID
	{
		get
		{
			return _chapterID;
		}
		set
		{
			_isDirty = true;
			_chapterID = value;
		}
	}

	[SqlField]
	public int pageID
	{
		get
		{
			return _pageID;
		}
		set
		{
			_isDirty = true;
			_pageID = value;
		}
	}

	[SqlField]
	public DateTime date
	{
		get
		{
			return _date;
		}
		set
		{
			_isDirty = true;
			_date = value;
		}
	}
}

using System;

namespace SqlDataProvider.Data;

public class MoneyTreeInfo
{
	private int _itemId;

	private DateTime _timeGrown;

	private int _priceSpeedUp;

	private int _positon;

	private int _pickDaily;

	public int ItemID
	{
		get
		{
			return _itemId;
		}
		set
		{
			_itemId = value;
		}
	}

	public DateTime timeGrown
	{
		get
		{
			return _timeGrown;
		}
		set
		{
			_timeGrown = value;
		}
	}

	public int priceSpeedUp
	{
		get
		{
			return _priceSpeedUp;
		}
		set
		{
			_priceSpeedUp = value;
		}
	}

	public int positon
	{
		get
		{
			return _positon;
		}
		set
		{
			_positon = value;
		}
	}

	public int PickDaily
	{
		get
		{
			return _pickDaily;
		}
		set
		{
			_pickDaily = value;
		}
	}

	public bool CanDrop()
	{
		int num = DateTime.Compare(_timeGrown, DateTime.Now);
		return num > 0;
	}
}

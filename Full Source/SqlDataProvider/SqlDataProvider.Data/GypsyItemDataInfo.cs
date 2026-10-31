namespace SqlDataProvider.Data;

public class GypsyItemDataInfo : DataObject
{
	private int _iD;

	private int _userID;

	private int _gypsyID;

	private int _infoID;

	private int _unit;

	private int _num;

	private int _type;

	private int _price;

	private int _canBuy;

	private int _quality;

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

	public int GypsyID
	{
		get
		{
			return _gypsyID;
		}
		set
		{
			_gypsyID = value;
			_isDirty = true;
		}
	}

	public int InfoID
	{
		get
		{
			return _infoID;
		}
		set
		{
			_infoID = value;
			_isDirty = true;
		}
	}

	public int Unit
	{
		get
		{
			return _unit;
		}
		set
		{
			_unit = value;
			_isDirty = true;
		}
	}

	public int Num
	{
		get
		{
			return _num;
		}
		set
		{
			_num = value;
			_isDirty = true;
		}
	}

	public int type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
			_isDirty = true;
		}
	}

	public int Price
	{
		get
		{
			return _price;
		}
		set
		{
			_price = value;
			_isDirty = true;
		}
	}

	public int CanBuy
	{
		get
		{
			return _canBuy;
		}
		set
		{
			_canBuy = value;
			_isDirty = true;
		}
	}

	public int Quality
	{
		get
		{
			return _quality;
		}
		set
		{
			_quality = value;
			_isDirty = true;
		}
	}
}

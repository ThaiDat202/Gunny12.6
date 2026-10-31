namespace SqlDataProvider.Data;

public class SysUsersDetailInfo : DataObject
{
	private int _userID;

	private int _userAllGP;

	private int _userMoneyPay;

	private int _userMoneyWar;

	private int _userMoneyShop;

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

	public int UserAllGP
	{
		get
		{
			return _userAllGP;
		}
		set
		{
			_userAllGP = value;
			_isDirty = true;
		}
	}

	public int UserMoneyPay
	{
		get
		{
			return _userMoneyPay;
		}
		set
		{
			_userMoneyPay = value;
			_isDirty = true;
		}
	}

	public int UserMoneyWar
	{
		get
		{
			return _userMoneyWar;
		}
		set
		{
			_userMoneyWar = value;
			_isDirty = true;
		}
	}

	public int UserMoneyShop
	{
		get
		{
			return _userMoneyShop;
		}
		set
		{
			_userMoneyShop = value;
			_isDirty = true;
		}
	}
}

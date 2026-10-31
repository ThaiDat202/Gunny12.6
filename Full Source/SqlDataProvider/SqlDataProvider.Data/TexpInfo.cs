using System;

namespace SqlDataProvider.Data;

public class TexpInfo : DataObject
{
	private int int_1;

	private int int_2;

	private int int_3;

	private int int_4;

	private int int_5;

	private int int_6;

	private int int_7;

	private int int_8;

	private DateTime dateTime_0;

	private int int_9;

	private int int_10;

	private int int_11;

	private int _TrainCritTexpExp;

	private int _TrainSunderTexpExp;

	private int _TrainCritDmgTexpExp;

	private int _TrainSpeedTexpExp;

	private int _TrainTricKillTexpExp;

	private int _TrainDmgTexpExp;

	private int _TrainArmorTexpExp;

	private int _SecondTexpCount;

	private int _ThirdTexpCount;

	private int _TrainAvoidDmgTextExp;

	private int _TrainTricDefTexpExp;

	private int _TrainTenacTexpExp;

	private int _TrainGuardTexpExp;

	private int _TrainFireDefTexpExp;

	private int _TrainWindDefTexpExp;

	private int _TrainWaterDefTexpExp;

	private int _TrainLandDefTexpExp;

	public DateTime lastReset;

	public int resetCount;

	public int attTexpExp
	{
		get
		{
			return int_3;
		}
		set
		{
			int_3 = value;
			_isDirty = true;
		}
	}

	public int defTexpExp
	{
		get
		{
			return int_4;
		}
		set
		{
			int_4 = value;
			_isDirty = true;
		}
	}

	public int hpTexpExp
	{
		get
		{
			return int_5;
		}
		set
		{
			int_5 = value;
			_isDirty = true;
		}
	}

	public int ID { get; set; }

	public int lukTexpExp
	{
		get
		{
			return int_6;
		}
		set
		{
			int_6 = value;
			_isDirty = true;
		}
	}

	public int magicAtkTexpExp
	{
		get
		{
			return int_9;
		}
		set
		{
			int_9 = value;
			_isDirty = true;
		}
	}

	public int magicDefTexpExp
	{
		get
		{
			return int_10;
		}
		set
		{
			int_10 = value;
			_isDirty = true;
		}
	}

	public int magicTexpCount
	{
		get
		{
			return int_11;
		}
		set
		{
			int_11 = value;
			_isDirty = true;
		}
	}

	public int spdTexpExp
	{
		get
		{
			return int_2;
		}
		set
		{
			int_2 = value;
			_isDirty = true;
		}
	}

	public int texpCount
	{
		get
		{
			return int_8;
		}
		set
		{
			int_8 = value;
			_isDirty = true;
		}
	}

	public int texpTaskCount
	{
		get
		{
			return int_7;
		}
		set
		{
			int_7 = value;
			_isDirty = true;
		}
	}

	public DateTime texpTaskDate
	{
		get
		{
			return dateTime_0;
		}
		set
		{
			dateTime_0 = value;
			_isDirty = true;
		}
	}

	public int UserID
	{
		get
		{
			return int_1;
		}
		set
		{
			int_1 = value;
			_isDirty = true;
		}
	}

	public int TrainCritTexpExp
	{
		get
		{
			return _TrainCritTexpExp;
		}
		set
		{
			_TrainCritTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainSunderTexpExp
	{
		get
		{
			return _TrainSunderTexpExp;
		}
		set
		{
			_TrainSunderTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainCritDmgTexpExp
	{
		get
		{
			return _TrainCritDmgTexpExp;
		}
		set
		{
			_TrainCritDmgTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainSpeedTexpExp
	{
		get
		{
			return _TrainSpeedTexpExp;
		}
		set
		{
			_TrainSpeedTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainTricKillTexpExp
	{
		get
		{
			return _TrainTricKillTexpExp;
		}
		set
		{
			_TrainTricKillTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainDmgTexpExp
	{
		get
		{
			return _TrainDmgTexpExp;
		}
		set
		{
			_TrainDmgTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainArmorTexpExp
	{
		get
		{
			return _TrainArmorTexpExp;
		}
		set
		{
			_TrainArmorTexpExp = value;
			_isDirty = true;
		}
	}

	public int SecondTexpCount
	{
		get
		{
			return _SecondTexpCount;
		}
		set
		{
			_SecondTexpCount = value;
			_isDirty = true;
		}
	}

	public int ThirdTexpCount
	{
		get
		{
			return _ThirdTexpCount;
		}
		set
		{
			_ThirdTexpCount = value;
			_isDirty = true;
		}
	}

	public int TrainAvoidDmgTextExp
	{
		get
		{
			return _TrainAvoidDmgTextExp;
		}
		set
		{
			_TrainAvoidDmgTextExp = value;
			_isDirty = true;
		}
	}

	public int TrainTricDefTexpExp
	{
		get
		{
			return _TrainTricDefTexpExp;
		}
		set
		{
			_TrainTricDefTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainTenacTexpExp
	{
		get
		{
			return _TrainTenacTexpExp;
		}
		set
		{
			_TrainTenacTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainGuardTexpExp
	{
		get
		{
			return _TrainGuardTexpExp;
		}
		set
		{
			_TrainGuardTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainFireDefTexpExp
	{
		get
		{
			return _TrainFireDefTexpExp;
		}
		set
		{
			_TrainFireDefTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainWindDefTexpExp
	{
		get
		{
			return _TrainWindDefTexpExp;
		}
		set
		{
			_TrainWindDefTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainWaterDefTexpExp
	{
		get
		{
			return _TrainWaterDefTexpExp;
		}
		set
		{
			_TrainWaterDefTexpExp = value;
			_isDirty = true;
		}
	}

	public int TrainLandDefTexpExp
	{
		get
		{
			return _TrainLandDefTexpExp;
		}
		set
		{
			_TrainLandDefTexpExp = value;
			_isDirty = true;
		}
	}

	public bool IsValidadteTexp()
	{
		return dateTime_0.Date < DateTime.Now.Date;
	}
}

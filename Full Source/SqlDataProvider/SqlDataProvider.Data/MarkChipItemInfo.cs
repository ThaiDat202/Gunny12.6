using System.Collections.Generic;
using Newtonsoft.Json;

namespace SqlDataProvider.Data;

public class MarkChipItemInfo : DataObject
{
	private int _ItemID;

	private int _UserID;

	private int _Type;

	private int _Place;

	private int _TemplateID;

	private int _bornLv;

	private int _hammerLv;

	private int _hLv;

	private string _mainPro;

	private string _props;

	private bool _IsBind;

	private bool _IsExist;

	public int ItemID
	{
		get
		{
			return _ItemID;
		}
		set
		{
			_ItemID = value;
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

	public int Type
	{
		get
		{
			return _Type;
		}
		set
		{
			_Type = value;
			_isDirty = true;
		}
	}

	public int Place
	{
		get
		{
			return _Place;
		}
		set
		{
			_Place = value;
			_isDirty = true;
		}
	}

	public int TemplateID
	{
		get
		{
			return _TemplateID;
		}
		set
		{
			_TemplateID = value;
			_isDirty = true;
		}
	}

	public int bornLv
	{
		get
		{
			return _bornLv;
		}
		set
		{
			_bornLv = value;
			_isDirty = true;
		}
	}

	public int hammerLv
	{
		get
		{
			return _hammerLv;
		}
		set
		{
			_hammerLv = value;
			_isDirty = true;
		}
	}

	public int hLv
	{
		get
		{
			return _hLv;
		}
		set
		{
			if (value >= 3 && value < 6)
			{
				hammerLv = ((bornLv < 2) ? (2 - bornLv) : 0);
			}
			else if (value >= 6 && value < 9)
			{
				hammerLv = ((bornLv < 3) ? (3 - bornLv) : 0);
			}
			else if (value >= 9 && value < 12)
			{
				hammerLv = ((bornLv < 4) ? (4 - bornLv) : 0);
			}
			else if (value >= 12)
			{
				hammerLv = ((bornLv < 5) ? (5 - bornLv) : 0);
			}
			_hLv = value;
			_isDirty = true;
		}
	}

	public string mainPro
	{
		get
		{
			return _mainPro;
		}
		set
		{
			_mainPro = value;
			_isDirty = true;
		}
	}

	public string props
	{
		get
		{
			return _props;
		}
		set
		{
			_props = value;
			_isDirty = true;
		}
	}

	public bool IsBind
	{
		get
		{
			return _IsBind;
		}
		set
		{
			_IsBind = value;
			_isDirty = true;
		}
	}

	public bool IsExist
	{
		get
		{
			return _IsExist;
		}
		set
		{
			_IsExist = value;
			_isDirty = true;
		}
	}

	public MarkProDataInfo MainProp
	{
		get
		{
			if (_mainPro == null)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<MarkProDataInfo>(_mainPro);
		}
	}

	public List<MarkProDataInfo> SecondProps
	{
		get
		{
			if (_props == null)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<List<MarkProDataInfo>>(_props);
		}
	}

	public int EquipType
	{
		get
		{
			if (_Place > 1000)
			{
				return -1;
			}
			if (_Place <= 6)
			{
				return 0;
			}
			if (_Place <= 12)
			{
				return 2;
			}
			if (_Place <= 18)
			{
				return 3;
			}
			if (_Place <= 24)
			{
				return 5;
			}
			if (_Place <= 30)
			{
				return 11;
			}
			if (_Place <= 36)
			{
				return 4;
			}
			return -1;
		}
	}

	public int SlotType
	{
		get
		{
			if (_Place > 1000)
			{
				return -1;
			}
			if (_Place <= 6)
			{
				return 1;
			}
			if (_Place <= 12)
			{
				return 2;
			}
			if (_Place <= 18)
			{
				return 3;
			}
			if (_Place <= 24)
			{
				return 4;
			}
			if (_Place <= 30)
			{
				return 5;
			}
			if (_Place <= 36)
			{
				return 6;
			}
			return -1;
		}
	}

	public MarkChipItemInfo()
	{
		IsExist = true;
		SetMainProp(new MarkProDataInfo());
		SetDefaultSecondProp();
	}

	public void SetMainProp(MarkProDataInfo val)
	{
		mainPro = JsonConvert.SerializeObject(val);
	}

	public void SetSecondProps(List<MarkProDataInfo> val)
	{
		props = JsonConvert.SerializeObject(val);
	}

	public int MinPosition(int slot)
	{
		return slot switch
		{
			1 => 1, 
			2 => 7, 
			3 => 13, 
			4 => 19, 
			5 => 25, 
			6 => 31, 
			_ => -1, 
		};
	}

	public void SetDefaultSecondProp()
	{
		List<MarkProDataInfo> list = new List<MarkProDataInfo>();
		for (int i = 0; i < 4; i++)
		{
			list.Add(new MarkProDataInfo());
		}
		SetSecondProps(list);
	}

	public MarkChipItemInfo Clone()
	{
		MarkChipItemInfo markChipItemInfo = new MarkChipItemInfo();
		markChipItemInfo.ItemID = _ItemID;
		markChipItemInfo.UserID = _UserID;
		markChipItemInfo.Type = _Type;
		markChipItemInfo.Place = _Place;
		markChipItemInfo.TemplateID = _TemplateID;
		markChipItemInfo.bornLv = _bornLv;
		markChipItemInfo.hammerLv = _hammerLv;
		markChipItemInfo.hLv = _hLv;
		markChipItemInfo.mainPro = _mainPro;
		markChipItemInfo.props = _props;
		markChipItemInfo.IsBind = _IsBind;
		markChipItemInfo._IsExist = _IsExist;
		return markChipItemInfo;
	}
}

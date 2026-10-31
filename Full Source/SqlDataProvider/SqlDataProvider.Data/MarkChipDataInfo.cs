using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class MarkChipDataInfo
{
	private ItemInfo _item;

	public int bornLv
	{
		get
		{
			return _item.StrengthenLevel;
		}
		set
		{
			_item.StrengthenLevel = value;
		}
	}

	public int hammerLv
	{
		get
		{
			return _item.StrengthenExp;
		}
		set
		{
			_item.StrengthenExp = value;
		}
	}

	public int hLv
	{
		get
		{
			return _item.AttackCompose;
		}
		set
		{
			_item.AttackCompose = value;
		}
	}

	public MarkProDataInfo MainPro
	{
		get
		{
			MarkProDataInfo markProDataInfo = new MarkProDataInfo();
			markProDataInfo.type = _item.DefendCompose;
			markProDataInfo.value = _item.AgilityCompose;
			markProDataInfo.attachValue = _item.LuckCompose;
			return markProDataInfo;
		}
		set
		{
			if (value != null)
			{
				_item.DefendCompose = value.type;
				_item.AgilityCompose = value.value;
				_item.LuckCompose = value.attachValue;
			}
		}
	}

	public List<MarkProDataInfo> Props
	{
		get
		{
			List<MarkProDataInfo> list = new List<MarkProDataInfo>();
			MarkProDataInfo markProDataInfo = new MarkProDataInfo();
			markProDataInfo.type = _item.Hole1;
			markProDataInfo.value = _item.Hole2;
			markProDataInfo.attachValue = _item.Hole3;
			markProDataInfo.hummerCount = _item.Hole5Exp;
			list.Add(markProDataInfo);
			markProDataInfo = new MarkProDataInfo();
			markProDataInfo.type = _item.Hole4;
			markProDataInfo.value = _item.Hole5;
			markProDataInfo.attachValue = _item.Hole6;
			markProDataInfo.hummerCount = _item.Hole6Exp;
			list.Add(markProDataInfo);
			markProDataInfo = new MarkProDataInfo();
			markProDataInfo.type = _item.ValidDate;
			markProDataInfo.value = _item.StrengthenRefineryLevel;
			markProDataInfo.attachValue = _item.StrengthenTimes;
			markProDataInfo.hummerCount = _item.Hole5Level;
			list.Add(markProDataInfo);
			markProDataInfo = new MarkProDataInfo();
			markProDataInfo.type = int.Parse(_item.Skin.Split('|')[0]);
			markProDataInfo.value = int.Parse(_item.Skin.Split('|')[1]);
			markProDataInfo.attachValue = int.Parse(_item.Skin.Split('|')[2]);
			markProDataInfo.hummerCount = _item.Hole6Level;
			list.Add(markProDataInfo);
			return list;
		}
		set
		{
			if (value == null)
			{
				return;
			}
			int num = 0;
			foreach (MarkProDataInfo item in value)
			{
				switch (num)
				{
				case 0:
					_item.Hole1 = item.type;
					_item.Hole2 = item.value;
					_item.Hole3 = item.attachValue;
					_item.Hole5Exp = item.hummerCount;
					break;
				case 1:
					_item.Hole4 = item.type;
					_item.Hole5 = item.value;
					_item.Hole6 = item.attachValue;
					_item.Hole6Exp = item.hummerCount;
					break;
				case 2:
					_item.ValidDate = item.type;
					_item.StrengthenRefineryLevel = item.value;
					_item.StrengthenTimes = item.attachValue;
					_item.Hole5Level = item.hummerCount;
					break;
				case 3:
					_item.Skin = item.type + "|" + item.value + "|" + item.attachValue;
					_item.Hole6Level = item.hummerCount;
					break;
				}
				num++;
			}
		}
	}

	public MarkChipDataInfo(ItemInfo item)
	{
		_item = item;
	}
}

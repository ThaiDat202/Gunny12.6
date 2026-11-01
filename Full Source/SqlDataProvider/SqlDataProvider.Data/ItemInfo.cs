using System;

namespace SqlDataProvider.Data;

public class ItemInfo : DataObject
{
	private ItemTemplateInfo _template;

	private bool _canTransfer;

	private bool _canRenew;

	private ItemTemplateInfo _goldEquip;

	private int _enchantSpiritLevel;

	private int _itemID;

	private int _userID;

	private int _bagType;

	private int _templateId;

	private int _place;

	private int _count;

	private bool _isJudage;

	private string _color;

	private bool _isExist;

	private int _strengthenLevel;

	private int _strengthenExp;

	private int _attackCompose;

	private int _defendCompose;

	private int _luckCompose;

	private int _agilityCompose;

	private bool _isBinds;

	private bool _isUsed;

	private bool _goodsLock;

	private string _skin;

	private DateTime _beginDate;

	private int _validDate;

	private DateTime _removeDate;

	private int _removeType;

	private int _hole1;

	private int _hole2;

	private int _hole3;

	private int _hole4;

	private int _hole5;

	private int _hole6;

	private int _strengthenTimes;

	private int _StrengthenRefineryLevel;

	private int _hole5Level;

	private int _hole6Level;

	private int _hole5Exp;

	private int _hole6Exp;

	private int _curExp;

	private bool _cellLocked;

	private bool _IsGold;

	private int _goldValidDate;

	private DateTime _goldBeginTime;

	private string _latentEnergyCurStr;

	private string _latentEnergyNewStr;

	private DateTime _latentEnergyEndTime;

	private bool _isTips = false;

	private bool _isLogs = false;

	private int _beadExp;

	private int _beadLevel;

	private bool _beadIsLock;

	private bool _isShowBind;

	private int _Damage;

	private int _Guard;

	private int _Blood;

	private int _Bless;

	private DateTime _advanceDate;

	private bool _avatarActivity;

	private int _magicAttack;

	private int _magicDefence;

	private int _magicExp;

	private int _magicLevel;

	public ItemTemplateInfo Template => _template;

	public ItemTemplateInfo GoldEquip
	{
		get
		{
			if (_goldEquip == null || IsDynamicWeapon())
			{
				return _template;
			}
			return _goldEquip;
		}
		set
		{
			_goldEquip = value;
			_isDirty = true;
		}
	}

	public int enchantSpiritLevel
	{
		get
		{
			return _enchantSpiritLevel;
		}
		set
		{
			_enchantSpiritLevel = value;
			_isDirty = true;
		}
	}

	public int ItemID
	{
		get
		{
			return _itemID;
		}
		set
		{
			_itemID = value;
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

	public int BagType
	{
		get
		{
			return _bagType;
		}
		set
		{
			_bagType = value;
			_isDirty = true;
		}
	}

	public int TemplateID
	{
		get
		{
			return _templateId;
		}
		set
		{
			_templateId = value;
			_isDirty = true;
		}
	}

	public int Place
	{
		get
		{
			return _place;
		}
		set
		{
			_place = value;
			_isDirty = true;
		}
	}

	public int Count
	{
		get
		{
			return _count;
		}
		set
		{
			_count = value;
			_isDirty = true;
		}
	}

	public bool IsJudge
	{
		get
		{
			return _isJudage;
		}
		set
		{
			_isJudage = value;
			_isDirty = true;
		}
	}

	public string Color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
			_isDirty = true;
		}
	}

	public bool IsExist
	{
		get
		{
			return _isExist;
		}
		set
		{
			_isExist = value;
			_isDirty = true;
		}
	}

	public int StrengthenLevel
	{
		get
		{
			if (Template != null && Template.IsAmulet && _strengthenLevel == 0)
			{
				_strengthenLevel = 1;
			}
			return _strengthenLevel;
		}
		set
		{
			_strengthenLevel = value;
			_isDirty = true;
		}
	}

	public int StrengthenExp
	{
		get
		{
			return _strengthenExp;
		}
		set
		{
			_strengthenExp = value;
			_isDirty = true;
		}
	}

	public int AttackCompose
	{
		get
		{
			return _attackCompose;
		}
		set
		{
			_attackCompose = value;
			_isDirty = true;
		}
	}

	public int DefendCompose
	{
		get
		{
			return _defendCompose;
		}
		set
		{
			_defendCompose = value;
			_isDirty = true;
		}
	}

	public int LuckCompose
	{
		get
		{
			return _luckCompose;
		}
		set
		{
			_luckCompose = value;
			_isDirty = true;
		}
	}

	public int AgilityCompose
	{
		get
		{
			return _agilityCompose;
		}
		set
		{
			_agilityCompose = value;
			_isDirty = true;
		}
	}

	public bool IsBinds
	{
		get
		{
			return _isBinds;
		}
		set
		{
			_isBinds = value;
			_isDirty = true;
		}
	}

	public bool IsUsed
	{
		get
		{
			return _isUsed;
		}
		set
		{
			if (_isUsed != value)
			{
				_isUsed = value;
				_isDirty = true;
			}
		}
	}

	public bool goodsLock
	{
		get
		{
			return _goodsLock;
		}
		set
		{
			if (_goodsLock != value)
			{
				_goodsLock = value;
				_isDirty = true;
			}
		}
	}

	public string Skin
	{
		get
		{
			return _skin;
		}
		set
		{
			_skin = value;
			_isDirty = true;
		}
	}

	public DateTime BeginDate
	{
		get
		{
			return _beginDate;
		}
		set
		{
			_beginDate = value;
			_isDirty = true;
		}
	}

	public int ValidDate
	{
		get
		{
			return _validDate;
		}
		set
		{
			_validDate = ((value > 999) ? 365 : value);
			_isDirty = true;
		}
	}

	public DateTime RemoveDate
	{
		get
		{
			return _removeDate;
		}
		set
		{
			_removeDate = value;
			_isDirty = true;
		}
	}

	public int RemoveType
	{
		get
		{
			return _removeType;
		}
		set
		{
			_removeType = value;
			_removeDate = DateTime.Now;
			_isDirty = true;
		}
	}

	public int Hole1
	{
		get
		{
			return _hole1;
		}
		set
		{
			_hole1 = value;
			_isDirty = true;
		}
	}

	public int Hole2
	{
		get
		{
			return _hole2;
		}
		set
		{
			_hole2 = value;
			_isDirty = true;
		}
	}

	public int Hole3
	{
		get
		{
			return _hole3;
		}
		set
		{
			_hole3 = value;
			_isDirty = true;
		}
	}

	public int Hole4
	{
		get
		{
			return _hole4;
		}
		set
		{
			_hole4 = value;
			_isDirty = true;
		}
	}

	public int Hole5
	{
		get
		{
			return _hole5;
		}
		set
		{
			_hole5 = value;
			_isDirty = true;
		}
	}

	public int Hole6
	{
		get
		{
			return _hole6;
		}
		set
		{
			_hole6 = value;
			_isDirty = true;
		}
	}

	public int StrengthenTimes
	{
		get
		{
			return _strengthenTimes;
		}
		set
		{
			_strengthenTimes = value;
			_isDirty = true;
		}
	}

	public int StrengthenRefineryLevel
	{
		get
		{
			return _StrengthenRefineryLevel;
		}
		set
		{
			_StrengthenRefineryLevel = value;
			_isDirty = true;
		}
	}

	public int Hole5Level
	{
		get
		{
			return _hole5Level;
		}
		set
		{
			_hole5Level = value;
			_isDirty = true;
		}
	}

	public int Hole6Level
	{
		get
		{
			return _hole6Level;
		}
		set
		{
			_hole6Level = value;
			_isDirty = true;
		}
	}

	public int Hole5Exp
	{
		get
		{
			return _hole5Exp;
		}
		set
		{
			_hole5Exp = value;
			_isDirty = true;
		}
	}

	public int Hole6Exp
	{
		get
		{
			return _hole6Exp;
		}
		set
		{
			_hole6Exp = value;
			_isDirty = true;
		}
	}

	public int curExp
	{
		get
		{
			return _curExp;
		}
		set
		{
			_curExp = value;
			_isDirty = true;
		}
	}

	public bool cellLocked
	{
		get
		{
			return _cellLocked;
		}
		set
		{
			_cellLocked = value;
			_isDirty = true;
		}
	}

	public bool IsGold
	{
		get
		{
			_IsGold = GoldValidDate();
			return _IsGold;
		}
		set
		{
			_IsGold = value;
			_isDirty = true;
		}
	}

	public int goldValidDate
	{
		get
		{
			return _goldValidDate;
		}
		set
		{
			_goldValidDate = value;
			_isDirty = true;
		}
	}

	public DateTime goldBeginTime
	{
		get
		{
			return _goldBeginTime;
		}
		set
		{
			_goldBeginTime = value;
			_isDirty = true;
		}
	}

	public string latentEnergyCurStr
	{
		get
		{
			return _latentEnergyCurStr;
		}
		set
		{
			_latentEnergyCurStr = value;
			_isDirty = true;
		}
	}

	public string latentEnergyNewStr
	{
		get
		{
			return _latentEnergyNewStr;
		}
		set
		{
			_latentEnergyNewStr = value;
			_isDirty = true;
		}
	}

	public DateTime latentEnergyEndTime
	{
		get
		{
			return _latentEnergyEndTime;
		}
		set
		{
			_latentEnergyEndTime = value;
			_isDirty = true;
		}
	}

	public string Pic
	{
		get
		{
			if (GoldEquip != null)
			{
				return GoldEquip.Pic;
			}
			return _template.Pic;
		}
	}

	public int RefineryLevel
	{
		get
		{
			if (GoldEquip != null)
			{
				return GoldEquip.RefineryLevel;
			}
			return _template.RefineryLevel;
		}
	}

	public int Attack
	{
		get
		{
			int attack = _template.Attack;
			if (GoldEquip != null)
			{
				attack = GoldEquip.Attack;
			}
			return _attackCompose + attack;
		}
	}

	public int Defence
	{
		get
		{
			int defence = _template.Defence;
			if (GoldEquip != null)
			{
				defence = GoldEquip.Defence;
			}
			return _defendCompose + defence;
		}
	}

	public int Agility
	{
		get
		{
			int agility = _template.Agility;
			if (GoldEquip != null)
			{
				agility = GoldEquip.Agility;
			}
			return _agilityCompose + agility;
		}
	}

	public int Luck
	{
		get
		{
			int luck = _template.Luck;
			if (GoldEquip != null)
			{
				luck = GoldEquip.Luck;
			}
			return _luckCompose + luck;
		}
	}

	public bool IsTips
	{
		get
		{
			return _isTips;
		}
		set
		{
			_isTips = value;
		}
	}

	public bool IsLogs
	{
		get
		{
			return _isLogs;
		}
		set
		{
			_isLogs = value;
		}
	}

	public int beadExp
	{
		get
		{
			return _beadExp;
		}
		set
		{
			_beadExp = value;
			_isDirty = true;
		}
	}

	public int beadLevel
	{
		get
		{
			return _beadLevel;
		}
		set
		{
			_beadLevel = value;
			_isDirty = true;
		}
	}

	public bool beadIsLock
	{
		get
		{
			return _beadIsLock;
		}
		set
		{
			_beadIsLock = value;
			_isDirty = true;
		}
	}

	public bool isShowBind
	{
		get
		{
			return _isShowBind;
		}
		set
		{
			_isShowBind = value;
			_isDirty = true;
		}
	}

	public int Damage
	{
		get
		{
			return _Damage;
		}
		set
		{
			_Damage = value;
			_isDirty = true;
		}
	}

	public int Guard
	{
		get
		{
			return _Guard;
		}
		set
		{
			_Guard = value;
			_isDirty = true;
		}
	}

	public int Blood
	{
		get
		{
			return _Blood;
		}
		set
		{
			_Blood = value;
			_isDirty = true;
		}
	}

	public int Bless
	{
		get
		{
			return _Bless;
		}
		set
		{
			_Bless = value;
			_isDirty = true;
		}
	}

	public DateTime AdvanceDate
	{
		get
		{
			return _advanceDate;
		}
		set
		{
			_advanceDate = value;
			_isDirty = true;
		}
	}

	public bool AvatarActivity
	{
		get
		{
			return _avatarActivity;
		}
		set
		{
			if (_avatarActivity != value)
			{
				_avatarActivity = value;
				_isDirty = true;
			}
		}
	}

	public int MagicAttack
	{
		get
		{
			return _magicAttack;
		}
		set
		{
			_magicAttack = value;
			_isDirty = true;
		}
	}

	public int MagicDefence
	{
		get
		{
			return _magicDefence;
		}
		set
		{
			_magicDefence = value;
			_isDirty = true;
		}
	}

	public int MagicExp
	{
		get
		{
			return _magicExp;
		}
		set
		{
			_magicExp = value;
			_isDirty = true;
		}
	}

	public int MagicLevel
	{
		get
		{
			return _magicLevel;
		}
		set
		{
			_magicLevel = value;
			_isDirty = true;
		}
	}

	public int GetBagType => (int)_template.BagType;

	public bool canTransfer
	{
		get
		{
			return _canTransfer;
		}
		set
		{
			_canTransfer = value;
		}
	}

	public bool canRenew
	{
		get
		{
			return _canRenew;
		}
		set
		{
			_canRenew = value;
		}
	}

	public int Int32_0 { get; set; }

	public bool IsAwaken { get; set; }

	public ItemInfo(ItemTemplateInfo temp)
	{
		_template = temp;
		_canRenew = true;
		_canTransfer = true;
	}

	public bool IsDynamicWeapon()
	{
		int templateID = TemplateID;
		int num = templateID;
		if ((uint)(num - 7144) <= 1u || (uint)(num - 71441) <= 3u || (uint)(num - 71451) <= 3u)
		{
			return true;
		}
		return false;
	}

	public bool SameMagicstone(string type)
	{
		return type switch
		{
			"att" => _agilityCompose == 0 && _attackCompose > 0 && _defendCompose == 0 && _luckCompose == 0 && _magicAttack == 0 && _magicDefence == 0, 
			"def" => _agilityCompose == 0 && _attackCompose == 0 && _defendCompose > 0 && _luckCompose == 0 && _magicAttack == 0 && _magicDefence == 0, 
			"agi" => _agilityCompose > 0 && _attackCompose == 0 && _defendCompose == 0 && _luckCompose == 0 && _magicAttack == 0 && _magicDefence == 0, 
			"luc" => _agilityCompose == 0 && _attackCompose == 0 && _defendCompose == 0 && _luckCompose > 0 && _magicAttack == 0 && _magicDefence == 0, 
			"mgatt" => _agilityCompose == 0 && _attackCompose == 0 && _defendCompose == 0 && _luckCompose == 0 && _magicAttack > 0 && _magicDefence == 0, 
			"mgdef" => _agilityCompose == 0 && _attackCompose == 0 && _defendCompose == 0 && _luckCompose == 0 && _magicAttack == 0 && _magicDefence > 0, 
			_ => false, 
		};
	}

	public bool IsEnchant()
	{
		switch (Template.CategoryID)
		{
		case 14:
		case 16:
		case 40:
		case 72:
			return false;
		default:
			return true;
		}
	}

	public bool GoldValidDate()
	{
		int categoryID = Template.CategoryID;
		int num = categoryID;
		if (num == 1 || num == 5 || num == 7)
		{
			if (_goldValidDate != 0)
			{
				return DateTime.Compare(_goldBeginTime.AddDays(_goldValidDate), DateTime.Now) > 0;
			}
			return false;
		}
		return false;
	}

	public bool IsAdvanceDate()
	{
		return _advanceDate.Date < DateTime.Now.Date;
	}

	public bool IsBring()
	{
		if (_template == null)
		{
			return false;
		}
		int categoryID = _template.CategoryID;
		int num = categoryID;
		if ((uint)(num - 8) <= 1u)
		{
			return true;
		}
		return false;
	}

	public ItemInfo Clone()
	{
		return new ItemInfo(_template)
		{
			_userID = _userID,
			_validDate = _validDate,
			_templateId = _templateId,
			_goldEquip = _goldEquip,
			_strengthenLevel = _strengthenLevel,
			_strengthenExp = _strengthenExp,
			_luckCompose = _luckCompose,
			_itemID = 0,
			_isJudage = _isJudage,
			_isExist = _isExist,
			_isBinds = _isBinds,
			_isUsed = _isUsed,
			_defendCompose = _defendCompose,
			_count = _count,
			_color = _color,
			_skin = _skin,
			_beginDate = _beginDate,
			_attackCompose = _attackCompose,
			_agilityCompose = _agilityCompose,
			_bagType = _bagType,
			_isDirty = true,
			_removeDate = _removeDate,
			_removeType = _removeType,
			_hole1 = _hole1,
			_hole2 = _hole2,
			_hole3 = _hole3,
			_hole4 = _hole4,
			_hole5 = _hole5,
			_hole6 = _hole6,
			_hole5Exp = _hole5Exp,
			_hole5Level = _hole5Level,
			_hole6Exp = _hole6Exp,
			_hole6Level = _hole6Level,
			_goldBeginTime = _goldBeginTime,
			_goldValidDate = _goldValidDate,
			_latentEnergyCurStr = _latentEnergyCurStr,
			_latentEnergyNewStr = _latentEnergyNewStr,
			_latentEnergyEndTime = _latentEnergyEndTime,
			_beadExp = _beadExp,
			_beadLevel = _beadLevel,
			_beadIsLock = _beadIsLock,
			_isShowBind = _isShowBind,
			_Damage = _Damage,
			_Guard = _Guard,
			_Bless = _Bless,
			_Blood = _Blood,
			_advanceDate = _advanceDate,
			_avatarActivity = _avatarActivity,
			_goodsLock = _goodsLock,
			_magicAttack = _magicAttack,
			_magicDefence = _magicDefence,
			_magicExp = _magicExp,
			_magicLevel = _magicLevel,
			_curExp = _curExp,
			_cellLocked = _cellLocked,
			canRenew = canRenew,
			canTransfer = canTransfer,
			enchantSpiritLevel = enchantSpiritLevel
		};
	}

	public bool IsValidItem()
	{
		if (_validDate != 0 && _isUsed)
		{
			return DateTime.Compare(_beginDate.AddDays(_validDate), DateTime.Now) > 0;
		}
		return true;
	}

	public bool IsValidLatentEnergy()
	{
		return _latentEnergyEndTime.Date < DateTime.Now.Date;
	}

	public void ResetLatentEnergy()
	{
		_latentEnergyCurStr = "0,0,0,0";
		_latentEnergyNewStr = "0,0,0,0";
	}

	public bool CanLatentEnergy()
	{
		return true;
	}

	public bool CanStackedTo(ItemInfo to)
	{
		if (_template == null)
		{
			return false;
		}
		if (!to.canRenew || !to.canTransfer || !canTransfer || !canRenew)
		{
			return false;
		}
		if (_templateId == to.TemplateID && Template.MaxCount > 1 && _isBinds == to.IsBinds && _isUsed == to._isUsed && (ValidDate == 0 || (BeginDate.Date == to.BeginDate.Date && ValidDate == ValidDate)))
		{
			return true;
		}
		return false;
	}

	public int eqType()
	{
		if (_template == null)
		{
			return 0;
		}
		return _template.CategoryID switch
		{
			51 => 1, 
			52 => 2, 
			_ => 0, 
		};
	}

	public bool IsProp()
	{
		if (_template == null)
		{
			return false;
		}
		switch (_template.CategoryID)
		{
		case 11:
		case 18:
		case 32:
		case 34:
		case 35:
		case 40:
			return true;
		default:
			return false;
		}
	}

	public bool CanEquip()
	{
		if (_template.CategoryID < 10 || (_template.CategoryID >= 13 && _template.CategoryID <= 16))
		{
			return true;
		}
		return false;
	}

	public string GetBagName()
	{
		if (_template == null)
		{
			return "Game.Server.GameObjects.UnKnow";
		}
		switch (_template.CategoryID)
		{
		case 10:
		case 11:
			return "Game.Server.GameObjects.Prop";
		case 12:
			return "Game.Server.GameObjects.Task";
		default:
			return "Game.Server.GameObjects.Equip";
		}
	}

	public static ItemInfo CreateFromTemplate(ItemTemplateInfo goods, int count, int type)
	{
		if (goods == null)
		{
			return null;
		}
		return new ItemInfo(goods)
		{
			AgilityCompose = 0,
			AttackCompose = 0,
			BeginDate = DateTime.Now,
			Color = "",
			Skin = "",
			DefendCompose = 0,
			IsUsed = false,
			IsDirty = false,
			IsExist = true,
			IsJudge = true,
			LuckCompose = 0,
			StrengthenLevel = 0,
			TemplateID = goods.TemplateID,
			ValidDate = 0,
			Count = count,
			IsBinds = (goods.BindType == 1),
			_removeDate = DateTime.Now,
			_removeType = type,
			Hole1 = -1,
			Hole2 = -1,
			Hole3 = -1,
			Hole4 = -1,
			Hole5 = -1,
			Hole6 = -1,
			Hole5Exp = 0,
			Hole5Level = 0,
			Hole6Exp = 0,
			Hole6Level = 0,
			goldValidDate = 0,
			goldBeginTime = DateTime.Now,
			StrengthenExp = 0,
			latentEnergyCurStr = "0,0,0,0",
			latentEnergyNewStr = "0,0,0,0",
			latentEnergyEndTime = DateTime.Now,
			beadExp = 0,
			beadLevel = 0,
			beadIsLock = false,
			isShowBind = false,
			Damage = 0,
			Guard = 0,
			Bless = 0,
			Blood = 0,
			AdvanceDate = DateTime.Now,
			AvatarActivity = false,
			goodsLock = false,
			MagicAttack = 0,
			MagicDefence = 0,
			MagicExp = 0,
			MagicLevel = 0,
			curExp = 0,
			cellLocked = false,
			enchantSpiritLevel = 0
		};
	}

	public static ItemInfo CloneFromTemplate(ItemTemplateInfo goods, ItemInfo item)
	{
		if (goods == null)
		{
			return null;
		}
		ItemInfo item2 = new ItemInfo(goods)
		{
			AgilityCompose = item.AgilityCompose,
			AttackCompose = item.AttackCompose,
			BeginDate = item.BeginDate,
			Color = item.Color,
			Skin = item.Skin,
			DefendCompose = item.DefendCompose,
			IsBinds = item.IsBinds,
			Place = item.Place,
			BagType = item.BagType,
			IsUsed = item.IsUsed,
			IsDirty = item.IsDirty,
			IsExist = item.IsExist,
			IsJudge = item.IsJudge,
			LuckCompose = item.LuckCompose,
			StrengthenExp = item.StrengthenExp,
			StrengthenLevel = item.StrengthenLevel,
			TemplateID = goods.TemplateID,
			ValidDate = item.ValidDate,
			_template = goods,
			Count = item.Count,
			_removeDate = item._removeDate,
			_removeType = item._removeType,
			Hole1 = item.Hole1,
			Hole2 = item.Hole2,
			Hole3 = item.Hole3,
			Hole4 = item.Hole4,
			Hole5 = item.Hole5,
			Hole6 = item.Hole6,
			Hole5Level = item.Hole5Level,
			Hole5Exp = item.Hole5Exp,
			Hole6Level = item.Hole6Level,
			Hole6Exp = item.Hole6Exp,
			goldBeginTime = item.goldBeginTime,
			goldValidDate = item.goldValidDate,
			latentEnergyEndTime = item.latentEnergyEndTime,
			latentEnergyCurStr = item.latentEnergyCurStr,
			latentEnergyNewStr = item.latentEnergyNewStr,
			AdvanceDate = item.AdvanceDate,
			goodsLock = item.goodsLock,
			MagicAttack = item.MagicAttack,
			MagicDefence = item.MagicDefence,
			MagicExp = item.MagicExp,
			MagicLevel = item.MagicLevel,
			curExp = item.curExp,
			cellLocked = item.cellLocked,
			canRenew = item.canRenew,
			canTransfer = item.canTransfer,
			enchantSpiritLevel = item.enchantSpiritLevel
		};
		if (Equip.isAvatar(item.Template))
		{
			item2.AvatarActivity = item.AvatarActivity;
		}
		OpenHole(ref item2);
		return item2;
	}

	public bool IsBead()
	{
		if (_template == null)
		{
			return false;
		}
		if (_template.Property1 == 31 && _template.CategoryID == 11)
		{
			return true;
		}
		return false;
	}

	public bool IsAmulet()
	{
		if (_template == null)
		{
			return false;
		}
		if (_template.CategoryID == 69)
		{
			return true;
		}
		return false;
	}

	public bool IsMagicStone()
	{
		if (_template == null)
		{
			return false;
		}
		if (_template.CategoryID == 61)
		{
			return true;
		}
		return false;
	}

	public bool IsEquipPet()
	{
		if (_template == null)
		{
			return false;
		}
		if (_template.CategoryID == 50 || _template.CategoryID == 51 || _template.CategoryID == 52)
		{
			return true;
		}
		return false;
	}

	public bool IsCard()
	{
		if (_template == null)
		{
			return false;
		}
		return _template.CategoryID switch
		{
			18 => true, 
			11 => _template.TemplateID == 112108 || _template.TemplateID == 112150, 
			_ => false, 
		};
	}

	public bool IsPuzzle()
	{
		int templateId = _templateId;
		int num = templateId;
		if ((uint)(num - 201386) <= 5u)
		{
			return true;
		}
		return false;
	}

	public bool IsRoomBorder()
	{
		if (_template == null)
		{
			return false;
		}
		if (_template.CategoryID == 43)
		{
			return true;
		}
		return false;
	}

	public int GetRemainDate()
	{
		if (ValidDate == 0)
		{
			return int.MaxValue;
		}
		if (!_isUsed)
		{
			return ValidDate;
		}
		int num = DateTime.Compare(_beginDate.AddDays(_validDate), DateTime.Now);
		return (num >= 0) ? num : 0;
	}

	public bool isDrill(int holelv)
	{
		if (_template == null)
		{
			return false;
		}
		switch (_template.TemplateID)
		{
		case 11035:
			if (holelv == 0)
			{
				return true;
			}
			return false;
		case 11036:
			if (holelv == 1)
			{
				return true;
			}
			return false;
		case 11026:
			if (holelv == 2)
			{
				return true;
			}
			return false;
		case 11027:
			if (holelv == 3)
			{
				return true;
			}
			return false;
		case 11034:
			if (holelv == 4)
			{
				return true;
			}
			return false;
		case 29994:
			if (holelv == 5)
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public bool isTexp()
	{
		if (_template == null)
		{
			return false;
		}
		int categoryID = _template.CategoryID;
		int num = categoryID;
		if (num == 20)
		{
			return true;
		}
		return false;
	}

	public bool isTexpSecondProp()
	{
		if (_template == null)
		{
			return false;
		}
		int categoryID = _template.CategoryID;
		int num = categoryID;
		if (num == 78)
		{
			return true;
		}
		return false;
	}

    public bool isTexpThirdProp()
    {
        if (_template == null)
        {
            return false;
        }
        int categoryID = _template.CategoryID;
        int num = categoryID;
        if (num == 88)
        {
            return true;
        }
        return false;
    }

    public bool isGemStone()
	{
		if (_template == null)
		{
			return false;
		}
		int templateID = _template.TemplateID;
		int num = templateID;
		if (num == 100100 || num == 201264)
		{
			return true;
		}
		return false;
	}

	public bool isLove()
	{
		if (_template == null)
		{
			return false;
		}
		int templateID = _template.TemplateID;
		int num = templateID;
		if ((uint)(num - 12536) <= 1u)
		{
			return true;
		}
		return false;
	}

	public int Speed()
	{
		if (Hole1 == 9)
		{
			return Hole5;
		}
		if (Hole2 == 9)
		{
			return Hole6;
		}
		if (Hole3 == 9)
		{
			return Hole5Exp;
		}
		if (Hole4 == 9)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int AmuletGuard()
	{
		if (Hole1 == 8)
		{
			return Hole5;
		}
		if (Hole2 == 8)
		{
			return Hole6;
		}
		if (Hole3 == 8)
		{
			return Hole5Exp;
		}
		if (Hole4 == 8)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int ViolenceInjury()
	{
		if (Hole1 == 7)
		{
			return Hole5;
		}
		if (Hole2 == 7)
		{
			return Hole6;
		}
		if (Hole3 == 7)
		{
			return Hole5Exp;
		}
		if (Hole4 == 7)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int WillFight()
	{
		if (Hole1 == 6)
		{
			return Hole5;
		}
		if (Hole2 == 6)
		{
			return Hole6;
		}
		if (Hole3 == 6)
		{
			return Hole5Exp;
		}
		if (Hole4 == 6)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int KillPow()
	{
		if (Hole1 == 5)
		{
			return Hole5;
		}
		if (Hole2 == 5)
		{
			return Hole6;
		}
		if (Hole3 == 5)
		{
			return Hole5Exp;
		}
		if (Hole4 == 5)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int AvoidInjury()
	{
		if (Hole1 == 4)
		{
			return Hole5;
		}
		if (Hole2 == 4)
		{
			return Hole6;
		}
		if (Hole3 == 4)
		{
			return Hole5Exp;
		}
		if (Hole4 == 4)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int SunderArmor()
	{
		if (Hole1 == 3)
		{
			return Hole5;
		}
		if (Hole2 == 3)
		{
			return Hole6;
		}
		if (Hole3 == 3)
		{
			return Hole5Exp;
		}
		if (Hole4 == 3)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int Tenacity()
	{
		if (Hole1 == 2)
		{
			return Hole5;
		}
		if (Hole2 == 2)
		{
			return Hole6;
		}
		if (Hole3 == 2)
		{
			return Hole5Exp;
		}
		if (Hole4 == 2)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public int Crit()
	{
		if (Hole1 == 1)
		{
			return Hole5;
		}
		if (Hole2 == 1)
		{
			return Hole6;
		}
		if (Hole3 == 1)
		{
			return Hole5Exp;
		}
		if (Hole4 == 1)
		{
			return Hole6Exp;
		}
		return 0;
	}

	public static void OpenHole(ref ItemInfo item)
	{
		string[] array = item.Template.Hole.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			if (item.StrengthenLevel < Convert.ToInt32(array2[0]) || Convert.ToInt32(array2[1]) == -1)
			{
				continue;
			}
			switch (i)
			{
			case 0:
				if (item.Hole1 < 0)
				{
					item.Hole1 = 0;
				}
				break;
			case 1:
				if (item.Hole2 < 0)
				{
					item.Hole2 = 0;
				}
				break;
			case 2:
				if (item.Hole3 < 0)
				{
					item.Hole3 = 0;
				}
				break;
			case 3:
				if (item.Hole4 < 0)
				{
					item.Hole4 = 0;
				}
				break;
			case 4:
				if (item.Hole5 < 0)
				{
					item.Hole5 = 0;
				}
				break;
			case 5:
				if (item.Hole6 < 0)
				{
					item.Hole6 = 0;
				}
				break;
			}
		}
	}
}

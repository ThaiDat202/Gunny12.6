using System.Collections.Generic;
using System.Linq;

namespace SqlDataProvider.Data;

public class ExplorerManualInfo
{
	private int _manualLevel;

	private int _progress;

	private int _maxProgress;

	private int _havePage;

	private int _conditionCount;

	private int _Agility;

	private int _Armor;

	private int _Attack;

	private int _Damage;

	private int _Defense;

	private int _HP;

	private int _Lucky;

	private int _MagicAttack;

	private int _MagicResistance;

	private int _Stamina;

	private int _jampsCurrency;

	private bool _isDirty;

	public List<PagesInfo> activesPage;

	public Dictionary<int, DebrisInfo> debris;

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
	public int manualLevel
	{
		get
		{
			return _manualLevel;
		}
		set
		{
			_isDirty = true;
			_manualLevel = value;
		}
	}

	[SqlField]
	public int progress
	{
		get
		{
			return _progress;
		}
		set
		{
			_isDirty = true;
			_progress = value;
		}
	}

	[SqlField]
	public int maxProgress
	{
		get
		{
			return _maxProgress;
		}
		set
		{
			_isDirty = true;
			_maxProgress = value;
		}
	}

	[SqlField]
	public int havePage
	{
		get
		{
			return _havePage;
		}
		set
		{
			_isDirty = true;
			_havePage = value;
		}
	}

	[SqlField]
	public int conditionCount
	{
		get
		{
			return _conditionCount;
		}
		set
		{
			_isDirty = true;
			_conditionCount = value;
		}
	}

	[SqlField]
	public int Agility
	{
		get
		{
			return _Agility;
		}
		set
		{
			_isDirty = true;
			_Agility = value;
		}
	}

	[SqlField]
	public int Armor
	{
		get
		{
			return _Armor;
		}
		set
		{
			_isDirty = true;
			_Armor = value;
		}
	}

	[SqlField]
	public int Attack
	{
		get
		{
			return _Attack;
		}
		set
		{
			_isDirty = true;
			_Attack = value;
		}
	}

	[SqlField]
	public int Damage
	{
		get
		{
			return _Damage;
		}
		set
		{
			_isDirty = true;
			_Damage = value;
		}
	}

	[SqlField]
	public int Defense
	{
		get
		{
			return _Defense;
		}
		set
		{
			_isDirty = true;
			_Defense = value;
		}
	}

	[SqlField]
	public int HP
	{
		get
		{
			return _HP;
		}
		set
		{
			_isDirty = true;
			_HP = value;
		}
	}

	[SqlField]
	public int Lucky
	{
		get
		{
			return _Lucky;
		}
		set
		{
			_isDirty = true;
			_Lucky = value;
		}
	}

	[SqlField]
	public int MagicAttack
	{
		get
		{
			return _MagicAttack;
		}
		set
		{
			_isDirty = true;
			_MagicAttack = value;
		}
	}

	[SqlField]
	public int MagicResistance
	{
		get
		{
			return _MagicResistance;
		}
		set
		{
			_isDirty = true;
			_MagicResistance = value;
		}
	}

	[SqlField]
	public int Stamina
	{
		get
		{
			return _Stamina;
		}
		set
		{
			_isDirty = true;
			_Stamina = value;
		}
	}

	[SqlField]
	public int jampsCurrency
	{
		get
		{
			return _jampsCurrency;
		}
		set
		{
			_isDirty = true;
			_jampsCurrency = value;
		}
	}

	public bool addPage(PagesInfo p)
	{
		if (activesPage.Where((PagesInfo o) => o.pageID == p.pageID).Count() == 0)
		{
			activesPage.Add(p);
			return true;
		}
		return false;
	}

	public bool acivatePage(int id)
	{
		foreach (PagesInfo item in activesPage)
		{
			if (item.pageID == id)
			{
				item.activate = true;
				return true;
			}
		}
		return false;
	}

	public bool addDebris(DebrisInfo debris)
	{
		if (!this.debris.ContainsKey(debris.ID))
		{
			this.debris.Add(debris.ID, debris);
			return true;
		}
		return false;
	}
}

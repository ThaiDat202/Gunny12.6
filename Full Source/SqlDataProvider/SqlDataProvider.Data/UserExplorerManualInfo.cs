using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SqlDataProvider.Data;

public class UserExplorerManualInfo : DataObject
{
	private int userId;

	private int agility;

	private int armor;

	private int attack;

	private int conditionCount;

	private int damage;

	private int defense;

	private int havePage;

	private int hP;

	private int lucky;

	private int magicAttack;

	private int magicResistance;

	private int manualLevel;

	private int maxProgress;

	private int progress;

	private int stamina;

	private string activesPage;

	private string debris;

	private List<ExplorerPagesInfo> _activesPageList;

	private Dictionary<int, ExplorerDebrisInfo> _debrisList;

	public int UserID
	{
		get
		{
			return userId;
		}
		set
		{
			userId = value;
			_isDirty = true;
		}
	}

	public int Agility
	{
		get
		{
			return agility;
		}
		set
		{
			agility = value;
			_isDirty = true;
		}
	}

	public int Armor
	{
		get
		{
			return armor;
		}
		set
		{
			armor = value;
			_isDirty = true;
		}
	}

	public int Attack
	{
		get
		{
			return attack;
		}
		set
		{
			attack = value;
			_isDirty = true;
		}
	}

	public int ConditionCount
	{
		get
		{
			return conditionCount;
		}
		set
		{
			conditionCount = value;
			_isDirty = true;
		}
	}

	public int Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
			_isDirty = true;
		}
	}

	public int Defense
	{
		get
		{
			return defense;
		}
		set
		{
			defense = value;
			_isDirty = true;
		}
	}

	public int HavePage
	{
		get
		{
			return havePage;
		}
		set
		{
			havePage = value;
			_isDirty = true;
		}
	}

	public int HP
	{
		get
		{
			return hP;
		}
		set
		{
			hP = value;
			_isDirty = true;
		}
	}

	public int Lucky
	{
		get
		{
			return lucky;
		}
		set
		{
			lucky = value;
			_isDirty = true;
		}
	}

	public int MagicAttack
	{
		get
		{
			return magicAttack;
		}
		set
		{
			magicAttack = value;
			_isDirty = true;
		}
	}

	public int MagicResistance
	{
		get
		{
			return magicResistance;
		}
		set
		{
			magicResistance = value;
			_isDirty = true;
		}
	}

	public int ManualLevel
	{
		get
		{
			return manualLevel;
		}
		set
		{
			manualLevel = value;
			_isDirty = true;
		}
	}

	public int MaxProgress
	{
		get
		{
			return maxProgress;
		}
		set
		{
			maxProgress = value;
			_isDirty = true;
		}
	}

	public int Progress
	{
		get
		{
			return progress;
		}
		set
		{
			progress = value;
			_isDirty = true;
		}
	}

	public int Stamina
	{
		get
		{
			return stamina;
		}
		set
		{
			stamina = value;
			_isDirty = true;
		}
	}

	public string ActivesPage
	{
		get
		{
			return activesPage;
		}
		set
		{
			activesPage = value;
			_isDirty = true;
		}
	}

	public string Debris
	{
		get
		{
			return debris;
		}
		set
		{
			debris = value;
			_isDirty = true;
		}
	}

	public List<ExplorerPagesInfo> ActivePageList => _activesPageList;

	public Dictionary<int, ExplorerDebrisInfo> DebrisList => _debrisList;

	public UserExplorerManualInfo()
	{
		_activesPageList = new List<ExplorerPagesInfo>();
		_debrisList = new Dictionary<int, ExplorerDebrisInfo>();
		activesPage = "";
		debris = "";
	}

	public bool AddPage(ExplorerPagesInfo p)
	{
		if (_activesPageList.Where((ExplorerPagesInfo o) => o.pageID == p.pageID).Count() == 0)
		{
			_activesPageList.Add(p);
			ActivesPage = JsonConvert.SerializeObject(_activesPageList);
			return true;
		}
		return false;
	}

	public bool ActivePage(int id)
	{
		bool flag = false;
		foreach (ExplorerPagesInfo activesPage in _activesPageList)
		{
			if (activesPage.pageID == id)
			{
				activesPage.activate = true;
				flag = true;
				break;
			}
		}
		if (flag)
		{
			ActivesPage = JsonConvert.SerializeObject(_activesPageList);
		}
		return flag;
	}

	public bool AddDebris(ExplorerDebrisInfo debris)
	{
		if (!_debrisList.ContainsKey(debris.ID))
		{
			_debrisList.Add(debris.ID, debris);
			Debris = JsonConvert.SerializeObject(_debrisList);
			return true;
		}
		return false;
	}
}

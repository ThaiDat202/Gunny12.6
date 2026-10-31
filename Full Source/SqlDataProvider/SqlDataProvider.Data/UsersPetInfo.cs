using System;
using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class UsersPetInfo : DataObject
{
	private string _skillEquip;

	private string _skill;

	private bool _isSeal;

	private int _ID;

	private int _vipLv;

	private int _petID;

	private int _templateID;

	private string _name;

	private int _userID;

	private int _attack;

	private int _defence;

	private int _luck;

	private int _agility;

	private int _blood;

	private int _damage;

	private int _guard;

	private int _attackGrow;

	private int _defenceGrow;

	private int _luckGrow;

	private int _agilityGrow;

	private int _bloodGrow;

	private int _damageGrow;

	private int _guardGrow;

	private int _level;

	private int _gp;

	private int _maxGP;

	private int _hunger;

	private int _mp;

	private bool _isEquip;

	private int _place;

	private int _currentStarExp;

	private bool _isExit;

	private int _breakGrade;

	private int _breakAttack;

	private int _breakDefence;

	private int _breakAgility;

	private int _breakLuck;

	private int _breakBlood;

	private List<PetEquipInfo> m_peEquips = new List<PetEquipInfo>();

	private string _eqPets;

	private string _baseProp;

	public string SkillEquip
	{
		get
		{
			return _skillEquip;
		}
		set
		{
			_skillEquip = value;
			_isDirty = true;
		}
	}

	public string Skill
	{
		get
		{
			return _skill;
		}
		set
		{
			_skill = value;
			_isDirty = true;
		}
	}

	public bool IsSeal
	{
		get
		{
			return _isSeal;
		}
		set
		{
			_isSeal = value;
			_isDirty = true;
		}
	}

	public int ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
			_isDirty = true;
		}
	}

	public int VIPLevel
	{
		get
		{
			return _vipLv;
		}
		set
		{
			_vipLv = value;
			_isDirty = true;
		}
	}

	public int PetID
	{
		get
		{
			return _petID;
		}
		set
		{
			_petID = value;
			_isDirty = true;
		}
	}

	public int TemplateID
	{
		get
		{
			return _templateID;
		}
		set
		{
			_templateID = value;
			_isDirty = true;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
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

	public int Attack
	{
		get
		{
			return _attack;
		}
		set
		{
			_attack = value;
			_isDirty = true;
		}
	}

	public int Defence
	{
		get
		{
			return _defence;
		}
		set
		{
			_defence = value;
			_isDirty = true;
		}
	}

	public int Luck
	{
		get
		{
			return _luck;
		}
		set
		{
			_luck = value;
			_isDirty = true;
		}
	}

	public int Agility
	{
		get
		{
			return _agility;
		}
		set
		{
			_agility = value;
			_isDirty = true;
		}
	}

	public int Blood
	{
		get
		{
			return _blood;
		}
		set
		{
			_blood = value;
			_isDirty = true;
		}
	}

	public int Damage
	{
		get
		{
			return _damage - ReduceProp(_damage);
		}
		set
		{
			_damage = value;
			_isDirty = true;
		}
	}

	public int Guard
	{
		get
		{
			return _guard - ReduceProp(_guard);
		}
		set
		{
			_guard = value;
			_isDirty = true;
		}
	}

	public int AttackGrow
	{
		get
		{
			return _attackGrow;
		}
		set
		{
			_attackGrow = value;
			_isDirty = true;
		}
	}

	public int DefenceGrow
	{
		get
		{
			return _defenceGrow;
		}
		set
		{
			_defenceGrow = value;
			_isDirty = true;
		}
	}

	public int LuckGrow
	{
		get
		{
			return _luckGrow;
		}
		set
		{
			_luckGrow = value;
			_isDirty = true;
		}
	}

	public int AgilityGrow
	{
		get
		{
			return _agilityGrow;
		}
		set
		{
			_agilityGrow = value;
			_isDirty = true;
		}
	}

	public int BloodGrow
	{
		get
		{
			return _bloodGrow;
		}
		set
		{
			_bloodGrow = value;
			_isDirty = true;
		}
	}

	public int DamageGrow
	{
		get
		{
			return _damageGrow;
		}
		set
		{
			_damageGrow = value;
			_isDirty = true;
		}
	}

	public int GuardGrow
	{
		get
		{
			return _guardGrow;
		}
		set
		{
			_guardGrow = value;
			_isDirty = true;
		}
	}

	public int Level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
			_isDirty = true;
		}
	}

	public int GP
	{
		get
		{
			return _gp;
		}
		set
		{
			_gp = value;
			_isDirty = true;
		}
	}

	public int MaxGP
	{
		get
		{
			return _maxGP;
		}
		set
		{
			_maxGP = value;
			_isDirty = true;
		}
	}

	public int Hunger
	{
		get
		{
			return _hunger;
		}
		set
		{
			_hunger = value;
			_isDirty = true;
		}
	}

	public int PetHappyStar
	{
		get
		{
			double num = (double)_hunger / 10000.0 * 100.0;
			int result = 3;
			if (num < 80.0)
			{
				result = 2;
			}
			if (num < 60.0)
			{
				result = 1;
			}
			return result;
		}
	}

	public int MP
	{
		get
		{
			return _mp;
		}
		set
		{
			_mp = value;
			_isDirty = true;
		}
	}

	public bool IsEquip
	{
		get
		{
			return _isEquip;
		}
		set
		{
			_isEquip = value;
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

	public int currentStarExp
	{
		get
		{
			return _currentStarExp;
		}
		set
		{
			_currentStarExp = value;
			_isDirty = true;
		}
	}

	public bool IsExit
	{
		get
		{
			return _isExit;
		}
		set
		{
			_isExit = value;
			_isDirty = true;
		}
	}

	public int breakGrade
	{
		get
		{
			return _breakGrade;
		}
		set
		{
			_breakGrade = value;
			_isDirty = true;
		}
	}

	public int breakAttack
	{
		get
		{
			return _breakAttack;
		}
		set
		{
			_breakAttack = value;
			_isDirty = true;
		}
	}

	public int breakDefence
	{
		get
		{
			return _breakDefence;
		}
		set
		{
			_breakDefence = value;
			_isDirty = true;
		}
	}

	public int breakAgility
	{
		get
		{
			return _breakAgility;
		}
		set
		{
			_breakAgility = value;
			_isDirty = true;
		}
	}

	public int breakLuck
	{
		get
		{
			return _breakLuck;
		}
		set
		{
			_breakLuck = value;
			_isDirty = true;
		}
	}

	public int breakBlood
	{
		get
		{
			return _breakBlood;
		}
		set
		{
			_breakBlood = value;
			_isDirty = true;
		}
	}

	public List<PetEquipInfo> PetEquips
	{
		get
		{
			return m_peEquips;
		}
		set
		{
			m_peEquips = value;
		}
	}

	public string eQPets
	{
		get
		{
			return _eqPets;
		}
		set
		{
			_eqPets = value;
			_isDirty = true;
		}
	}

	public string BaseProp
	{
		get
		{
			return _baseProp;
		}
		set
		{
			_baseProp = value;
			_isDirty = true;
		}
	}

	public int TotalAttack => Attack - ReduceProp(Attack) + _breakAttack;

	public int TotalDefence => Defence - ReduceProp(Defence) + _breakDefence;

	public int TotalLuck => Luck - ReduceProp(Luck) + breakLuck;

	public int TotalAgility => Agility - ReduceProp(Agility) + _breakAgility;

	public int TotalBlood => Blood - ReduceProp(Blood) + _breakBlood;

	public string[] GetSkillEquip()
	{
		string[] array = new string[5] { "0,0", "-1,1", "-1,2", "-1,3", "-1,4" };
		if (Level >= 20 && Level < 30)
		{
			array[1] = "0,1";
		}
		if (Level >= 30 && Level < 50)
		{
			array[1] = "0,1";
			array[2] = "0,2";
		}
		if (Level >= 50)
		{
			array[1] = "0,1";
			array[2] = "0,2";
			array[3] = "0,3";
		}
		if (VIPLevel >= 11)
		{
			array[4] = "0,4";
		}
		string[] array2 = SkillEquip.Split('|');
		List<string> list = new List<string>();
		string[] array3 = Skill.Split('|');
		for (int i = 0; i < array3.Length; i++)
		{
			list.Add(array3[i].Split(',')[0]);
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (j < array2.Length && list.Contains(array2[j].Split(',')[0]))
			{
				array[j] = array2[j];
			}
		}
		_skillEquip = array[0];
		for (int k = 1; k < array.Length; k++)
		{
			_skillEquip = _skillEquip + "|" + array[k];
		}
		return array;
	}

	public int MaxLevel()
	{
		return _breakGrade switch
		{
			1 => 63, 
			2 => 65, 
			3 => 68, 
			4 => 70, 
			_ => 60, 
		};
	}

	public int ReduceProp(int val)
	{
		if (PetHappyStar == 2)
		{
			return val * 20 / 100;
		}
		if (PetHappyStar == 1)
		{
			return val * 40 / 100;
		}
		return 0;
	}

	private double[] GetAddedPropArr(int grade, double[] propArr)
	{
		double[] array = new double[propArr.Length];
		array[0] = propArr[0] * Math.Pow(2.0, grade - 1);
		for (int i = 1; i < propArr.Length; i++)
		{
			array[i] = propArr[i] * Math.Pow(1.5, grade - 1);
		}
		return array;
	}

	public void BuildProp()
	{
		double[] array = new double[5]
		{
			BloodGrow * 10,
			AttackGrow,
			DefenceGrow,
			AgilityGrow,
			LuckGrow
		};
		double[] propArr = new double[5] { BloodGrow, AttackGrow, DefenceGrow, AgilityGrow, LuckGrow };
		double[] array2 = array;
		double[] array3 = array;
		double[] array4 = array;
		double[] addedPropArr = GetAddedPropArr(1, propArr);
		double[] addedPropArr2 = GetAddedPropArr(2, propArr);
		double[] addedPropArr3 = GetAddedPropArr(3, propArr);
		double[] array5 = new double[array2.Length];
		if (Level < 30)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] += (double)(Level - 1) * addedPropArr[i];
				array2[i] = Math.Ceiling(array2[i] / 10.0) / 10.0;
			}
			array5 = array2;
		}
		else if (Level < 50)
		{
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j] += (double)(Level - 30) * addedPropArr2[j] + 29.0 * addedPropArr[j];
				array3[j] = Math.Ceiling(array3[j] / 10.0) / 10.0;
			}
			array5 = array3;
		}
		else
		{
			for (int k = 0; k < array4.Length; k++)
			{
				array4[k] += (double)(Level - 50) * addedPropArr3[k] + 20.0 * addedPropArr2[k] + 29.0 * addedPropArr[k];
				array4[k] = Math.Ceiling(array4[k] / 10.0) / 10.0;
			}
			array5 = array4;
		}
		_blood = (int)array5[0];
		_attack = (int)array5[1];
		_defence = (int)array5[2];
		_agility = (int)array5[3];
		_luck = (int)array5[4];
	}
}

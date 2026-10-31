using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class JampsPageItemList
{
	public int ID;

	public int Sort;

	public int DebrisCount;

	public int ChapterID;

	public int Activate_Agile;

	public int Activate_Armor;

	public int Activate_Attack;

	public int Activate_Damage;

	public int Activate_Defence;

	public int Activate_HP;

	public int Activate_Lucky;

	public int Activate_MagicAttack;

	public int Activate_MagicResistance;

	public int Activate_Stamina;

	public int ActivateCurrency;

	public int Collect_Agile;

	public int Collect_Armor;

	public int Collect_Attack;

	public int Collect_Damage;

	public int Collect_Defense;

	public int Collect_HP;

	public int Collect_Lucky;

	public int Collect_MagicAttack;

	public int Collect_MagicResistance;

	public int Collect_Stamina;

	public string Describe;

	public string Name;

	public string ImagePath;

	public Dictionary<int, JampsDebrisItemList> _fragmentos;
}

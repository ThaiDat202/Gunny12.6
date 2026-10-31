namespace Game.Logic.PetSkills;

public class PetEffectInfo
{
	public int CurrentHorseSkill;

	public int CurrentUseSkill;

	public int BallType;

	public int MaxBlood;

	public int DamagePercent;

	public int CritRate;

	public int Delay;

	public bool ActiveGuard;

	public int ReboundDamage;

	public bool DisibleActiveSkill;

	public int ReduceDefendValue;

	public int ReduceCritValue;

	public int AddMaxBloodValue;

	public int AddBloodPercent;

	public int IncreaseAngelicPoint;

	public bool ActiveEffect;

	public double reduceArmorValue;

	public int AddDameValue { get; set; }

	public int AddGuardValue { get; set; }

	public int AddLuckValue { get; set; }

	public int AddMagicAttackValue { get; set; }

	public int BonusAgility { get; set; }

	public int BonusLucky { get; set; }

	public int BonusAttack { get; set; }

	public int BonusDefend { get; set; }

	public int BonusBaseDamage { get; set; }

	public int BonusGuard { get; set; }

	public int BonusPoint { get; set; }

	public bool StopMoving { get; set; }

	public int PetDelay { get; set; }

	public bool IsPetUseSkill { get; set; }

	public bool CritActive { get; set; }

	public bool ActivePetHit { get; set; }
}

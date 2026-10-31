using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class BackDamage_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	public BackDamage_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.BackDamage_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_value = pet_Special_Skills.P1;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.BackDamage_SealPassive) is BackDamage_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeforeTakeDamage += player_BeforeTakeDamage;
		living.EndAttacking += player_EndAttacking;
	}

	public override void OnRemoved(Living living)
	{
		living.BeforeTakeDamage -= player_BeforeTakeDamage;
		living.EndAttacking += player_EndAttacking;
	}

	private void player_BeforeTakeDamage(Living living, Living targe, ref int damageAmount, ref int criticalAmount)
	{
		int value = damageAmount * m_value / 100;
		targe.ReduceBlood(value);
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
	}

	private void player_EndAttacking(Living living)
	{
		living.Game.sendShowPicSkil(living, base.Info, isActive: false);
	}
}

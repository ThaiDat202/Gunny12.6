using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class RecoverBlood_SealPassive : AbstractPetEffect
{
	private int m_random = 0;

	private bool IsPVP = false;

	private int m_value = 0;

	public RecoverBlood_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.RecoverBlood_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_random = pet_Special_Skills.P1;
		m_value = pet_Special_Skills.P2;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.RecoverBlood_SealPassive) is RecoverBlood_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.TakePlayerDamage += player_TakePlayerDamage;
	}

	public override void OnRemoved(Living living)
	{
		living.TakePlayerDamage -= player_TakePlayerDamage;
	}

	private void player_TakePlayerDamage(Living living, Living target, ref int Damage, ref int Crit)
	{
		if (rand.Next(0, 100) < m_random)
		{
			living.Game.AddAction(new LivingSayAction(living, "吸血", 9, 0, 1000));
			living.AddBlood(Damage * m_value / 100);
		}
	}
}

using System;
using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class ReduceTargetBlood_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_random = 0;

	private int m_count = 0;

	private int m_cdMax = 0;

	public ReduceTargetBlood_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.ReduceTargetBlood_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_random = pet_Special_Skills.P1;
		m_value = pet_Special_Skills.P2;
		m_count = pet_Special_Skills.P3;
		m_cdMax = pet_Special_Skills.P3;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.ReduceTargetBlood_SealPassive) is ReduceTargetBlood_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.AfterKillingLiving += player_AfterKillingLiving;
	}

	public override void OnRemoved(Living living)
	{
		living.AfterKillingLiving -= player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (!IsTrigger)
		{
			IsTrigger = true;
			if (!target.FlagUsePetSkill && rand.Next(100) < m_random)
			{
				target.Game.sendShowPicSkil(living, base.Info, isActive: false);
				target.Game.sendShowPicSkil(living, base.Info, isActive: true);
				living.Game.AddAction(new LivingSayAction(living, "灼烧", 9, 0, 1000));
				target.AddEffect(new ContinueReduceBloodEffect(m_count, Convert.ToInt32(target.Blood * m_value / 100), target), 0);
				target.FlagUsePetSkill = true;
			}
		}
		else
		{
			IsTrigger = false;
		}
	}
}

using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class ReduceTargetMp_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_random = 0;

	public ReduceTargetMp_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.ReduceTargetMp_SealPassive, elementID)
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
		if (living.PetEffectList.GetOfType(ePetEffectType.ReduceTargetMp_SealPassive) is ReduceTargetMp_SealPassive)
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
			if (rand.Next(100) < m_random)
			{
				target.Game.sendShowPicSkil(living, base.Info, isActive: false);
				target.Game.sendShowPicSkil(living, base.Info, isActive: true);
				living.Game.AddAction(new LivingSayAction(living, "魔力侵蚀", 9, 0, 1000));
				(target as Player).RemovePetMP(m_value);
			}
		}
		else
		{
			IsTrigger = false;
		}
	}
}

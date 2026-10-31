using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class MoveProhibition_SealPassive : AbstractPetEffect
{
	private int m_random = 0;

	private bool IsPVP = false;

	private int m_turn = 0;

	private int m_skillId = 0;

	private string m_elementId;

	public MoveProhibition_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.MoveProhibition_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_skillId = skilId;
		m_elementId = elementID;
		m_random = pet_Special_Skills.P1;
		m_turn = pet_Special_Skills.P2;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.MoveProhibition_SealPassive) is MoveProhibition_SealPassive)
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

	private void player_AfterKillingLiving(Living living, Living target, int Damage, int Crit)
	{
		if (!IsTrigger)
		{
			IsTrigger = true;
			if (!target.FlagUsePetSkill && rand.Next(100) < m_random)
			{
				target.Game.sendShowPicSkil(target, base.Info, isActive: true);
				living.Game.AddAction(new LivingSayAction(living, "定身", 9, 0, 1000));
				new MoveProhibition_Effect(m_skillId, m_elementId).Start(target);
				target.FlagUsePetSkill = true;
			}
		}
		else
		{
			IsTrigger = false;
		}
	}
}

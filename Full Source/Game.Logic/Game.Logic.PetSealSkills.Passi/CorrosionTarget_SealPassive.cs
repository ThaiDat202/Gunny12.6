using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class CorrosionTarget_SealPassive : AbstractPetEffect
{
	private int m_count = 0;

	private bool IsPVP = false;

	private int m_totalCount = 0;

	private int m_skillId = 0;

	private string m_elementID;

	public CorrosionTarget_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.CorrosionTarget_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_skillId = skilId;
		m_elementID = elementID;
		m_count = pet_Special_Skills.P2;
		m_totalCount = pet_Special_Skills.P3;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.CorrosionTarget_SealPassive) is CorrosionTarget_SealPassive)
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
			if (m_count < m_totalCount)
			{
				living.Game.AddAction(new LivingSayAction(living, "腐蚀", 9, 0, 1000));
				new Corrosion_Effect(m_skillId, m_elementID).Start(target);
			}
			else
			{
				target.FlagUsePetSkill = true;
			}
			IsTrigger = true;
		}
		else
		{
			IsTrigger = false;
		}
	}
}

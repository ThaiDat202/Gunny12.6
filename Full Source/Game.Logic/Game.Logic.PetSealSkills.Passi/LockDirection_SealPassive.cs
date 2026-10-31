using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.PetSkills;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class LockDirection_SealPassive : AbstractPetEffect
{
	private int m_random = 0;

	private int m_count = 0;

	private bool IsPVP = false;

	public LockDirection_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.LockDirection_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_random = pet_Special_Skills.P1;
		m_count = pet_Special_Skills.P2;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.LockDirection_SealPassive) is LockDirection_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		((Player)living).AfterKillingLiving += player_AfterKillingLiving;
	}

	public override void OnRemoved(Living living)
	{
		((Player)living).AfterKillingLiving -= player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (rand.Next(100) < m_random)
		{
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: true);
			living.Game.AddAction(new LivingSayAction(living, "控制姿势", 9, 0, 1000));
			new LockDirectionEffect(m_count).Start(target);
			target.AddPetEffect(new PetControlPosture_LockDirectionEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

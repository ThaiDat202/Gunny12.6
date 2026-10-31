using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class ProhibitionAddBlood_SealPassive : AbstractPetEffect
{
	private int m_random = 0;

	private int m_count = 0;

	private bool IsPVP = false;

	private bool IsUse = false;

	public ProhibitionAddBlood_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.ProhibitionAddBlood_SealPassive, elementID)
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
		if (living.PetEffectList.GetOfType(ePetEffectType.ProhibitionAddBlood_SealPassive) is ProhibitionAddBlood_SealPassive)
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

	private void player_BeginSelfTurn(Living living)
	{
		if (IsUse)
		{
			m_count--;
			if (m_count < 0)
			{
				IsUse = false;
				living.ForbidAidBlood = false;
				living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
				living.BeginSelfTurn -= player_BeginSelfTurn;
			}
		}
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (rand.Next(100) < m_random && !target.ForbidAidBlood && !IsUse)
		{
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: false);
			target.Game.sendShowPicSkil(target, base.ElementInfo, isActive: true);
			living.Game.AddAction(new LivingSayAction(living, "禁疗", 9, 0, 1000));
			target.ForbidAidBlood = true;
			IsUse = true;
			m_count = 2;
			target.BeginSelfTurn += player_BeginSelfTurn;
		}
	}
}

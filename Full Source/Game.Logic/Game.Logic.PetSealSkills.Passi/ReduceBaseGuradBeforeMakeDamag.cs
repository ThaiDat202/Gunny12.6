using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class ReduceBaseGuradBeforeMakeDamage_SealPassive : AbstractPetEffect
{
	private bool IsPVP = false;

	private int m_added = 0;

	private bool IsUse = false;

	public ReduceBaseGuradBeforeMakeDamage_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.ReduceBaseGuradBeforeMakeDamage_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_added = pet_Special_Skills.P1;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.ReduceBaseGuradBeforeMakeDamage_SealPassive) is ReduceBaseGuradBeforeMakeDamage_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeforeMakeDamage += player_BeforeMakeDamage;
	}

	public override void OnRemoved(Living living)
	{
		living.BeforeMakeDamage -= player_BeforeMakeDamage;
	}

	private void player_BeforeMakeDamage(Living living, Living target)
	{
		if (!IsUse)
		{
			IsUse = true;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			double num = target.BaseGuard * (double)m_added / 100.0;
			target.BaseGuard -= num;
			new ResetBaseGuard_SealPassive(1, num).Start(target);
		}
		else
		{
			IsUse = false;
		}
	}
}

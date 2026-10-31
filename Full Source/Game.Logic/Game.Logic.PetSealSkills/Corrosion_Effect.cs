using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills;

public class Corrosion_Effect : AbstractPetEffect
{
	private int m_count = 0;

	private bool IsPVP;

	private int m_value = 0;

	public Corrosion_Effect(int skilId, string ElementID)
		: base(ePetEffectType.Corrosion_Effect, ElementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_value = pet_Special_Skills.P1;
		IsPVP = pet_Special_Skills.IsPVP;
		m_count = pet_Special_Skills.P3;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.Corrosion_Effect) is Corrosion_Effect corrosion_Effect)
		{
			corrosion_Effect.m_count = m_count;
			return true;
		}
		return Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (m_count > 0)
		{
			living.BaseDamage -= living.BaseDamage * (double)m_value / 100.0;
			living.BaseGuard -= living.BaseGuard * (double)m_value / 100.0;
			m_count--;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
		else
		{
			living.BaseDamage = living.OldBaseDamage;
			living.BaseGuard = living.OldBaseGuard;
			living.FlagUsePetSkill = false;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

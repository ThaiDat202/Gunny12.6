using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class AddBlood_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private int m_count = 0;

	private int m_cdMax = 0;

	public AddBlood_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.AddBlood_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_added = pet_Special_Skills.P1;
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
		if (living.PetEffectList.GetOfType(ePetEffectType.AddBlood_SealPassive) is AddBlood_SealPassive)
		{
			return true;
		}
		return base.Start(living);
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
		living.Game.sendShowPicSkil(living, base.Info, isActive: false);
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		if (m_count > 0)
		{
			m_count--;
			return;
		}
		int value = m_added + living.MaxBlood * m_value / 100;
		living.SyncAtTime = true;
		living.AddBlood(value);
		living.SyncAtTime = false;
		m_count = m_cdMax;
	}
}

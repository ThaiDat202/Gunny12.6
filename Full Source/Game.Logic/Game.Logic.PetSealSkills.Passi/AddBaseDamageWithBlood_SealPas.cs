using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class AddBaseDamageWithBlood_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	public AddBaseDamageWithBlood_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.AddBaseDamageWithBlood_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_value = pet_Special_Skills.P1;
		m_added = pet_Special_Skills.P2;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.AddBaseDamageWithBlood_SealPassive) is AddBaseDamageWithBlood_SealPassive)
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
		if (living.Blood * 100 / living.MaxBlood < m_value)
		{
			living.BaseDamage = living.OldBaseDamage;
			living.BaseDamage += living.BaseDamage * (double)m_added / 100.0;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
		else
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
		}
	}
}

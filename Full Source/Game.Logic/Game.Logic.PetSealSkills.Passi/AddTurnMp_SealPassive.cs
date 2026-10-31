using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class AddTurnMp_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	public AddTurnMp_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.AddTurnMp_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_value = pet_Special_Skills.P1;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.AddTurnMp_SealPassive) is AddTurnMp_SealPassive)
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
		(living as Player).AddPetMP(m_value);
	}
}

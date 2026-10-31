using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class AddViolenceInjury_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private int m_added = 0;

	private bool IsPVP = false;

	private bool m_percent = false;

	public AddViolenceInjury_SealPassive(int skilId, string elementID, bool isPercent)
		: base(ePetEffectType.AddViolenceInjury_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_added = pet_Special_Skills.P2;
		IsPVP = pet_Special_Skills.IsPVP;
		m_percent = isPercent;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.AddViolenceInjury_SealPassive) is AddViolenceInjury_SealPassive)
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
		if (m_value == 0)
		{
			m_value = m_added;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.ViolenceInjury += living.ViolenceInjury * (double)m_value / 100.0;
			}
			else
			{
				living.ViolenceInjury += m_value;
			}
		}
	}
}

using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class ResetDefence_SealPassive : AbstractPetEffect
{
	private double m_value = 0.0;

	private int m_count = 0;

	public ResetDefence_SealPassive(int count, double value)
		: base(ePetEffectType.ResetDefence_SealPassive, "")
	{
		m_count = count;
		m_value = value;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ResetDefence_SealPassive) is ResetDefence_SealPassive)
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
		m_count--;
		if (m_count < 0)
		{
			living.Defence += m_value;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

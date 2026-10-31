using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetStopMovingEquip : AbstractPetEffect
{
	private int m_count;

	public PetStopMovingEquip(int count, string elementID)
		: base(ePetEffectType.PetStopMovingEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetStopMovingEquip) is PetStopMovingEquip petStopMovingEquip)
		{
			petStopMovingEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.SpeedMultX(0);
		living.PetEffects.StopMoving = true;
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.SpeedMultX(3);
		living.PetEffects.StopMoving = false;
		living.BeginSelfTurn -= player_BeginFitting;
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
		}
	}
}

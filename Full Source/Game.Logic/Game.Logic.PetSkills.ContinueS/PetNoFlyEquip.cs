using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetNoFlyEquip : AbstractPetEffect
{
	private int m_count;

	public PetNoFlyEquip(int count, string elementID)
		: base(ePetEffectType.PetNoFlyEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetNoFlyEquip) is PetNoFlyEquip petNoFlyEquip)
		{
			petNoFlyEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.NoFly(value: true);
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.NoFly(value: false);
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

using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceCritRateEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	public PetReduceCritRateEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceCritRateEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1430":
			m_value = 20;
			break;
		case "1431":
			m_value = 30;
			break;
		case "1510":
			m_value = 35;
			break;
		case "2667":
			m_value = 60;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceCritRateEquip) is PetReduceCritRateEquip petReduceCritRateEquip)
		{
			petReduceCritRateEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		(living as Player).PetEffects.CritRate -= m_value;
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		(living as Player).PetEffects.CritRate += m_value;
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddCritRateEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	public PetAddCritRateEquip(int count, string elementID)
		: base(ePetEffectType.PetAddCritRateEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1212":
		case "1421":
			m_value = 20;
			break;
		case "1507":
			m_value = 25;
			break;
		case "1198":
			m_value = 30;
			break;
		case "1199":
		case "1213":
		case "1491":
			m_value = 50;
			break;
		case "1499":
			m_value = 75;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddCritRateEquip) is PetAddCritRateEquip petAddCritRateEquip)
		{
			petAddCritRateEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		(living as Player).PetEffects.CritRate += m_value;
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		(living as Player).PetEffects.CritRate -= m_value;
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

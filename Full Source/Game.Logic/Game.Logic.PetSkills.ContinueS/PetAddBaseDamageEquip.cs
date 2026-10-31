using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBaseDamageEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddBaseDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetAddBaseDamageEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1490":
			m_value = 5.0;
			m_percent = true;
			break;
		case "1483":
			m_value = 6.0;
			m_percent = true;
			break;
		case "1153":
		case "1622":
			m_value = 15.0;
			m_percent = true;
			break;
		case "1154":
			m_value = 25.0;
			m_percent = true;
			break;
		case "1196":
			m_value = 100.0;
			break;
		case "1197":
			m_value = 150.0;
			break;
		case "1744":
			m_value = 35.0;
			m_percent = true;
			break;
		case "1814":
			m_value = 20.0;
			m_percent = true;
			break;
		case "1465":
			m_value = 30.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageEquip) is PetAddBaseDamageEquip petAddBaseDamageEquip)
		{
			petAddBaseDamageEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		if (m_added == 0.0)
		{
			if (m_percent)
			{
				m_added = living.BaseDamage * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.BaseDamage += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BaseDamage -= m_added;
		m_added = 0.0;
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
	}
}

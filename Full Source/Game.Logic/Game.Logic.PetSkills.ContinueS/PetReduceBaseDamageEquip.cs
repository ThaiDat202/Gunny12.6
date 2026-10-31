using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceBaseDamageEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private double m_added;

	public PetReduceBaseDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceBaseDamageEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1201":
			m_value = 100;
			break;
		case "1202":
			m_value = 200;
			break;
		case "1203":
			m_value = 300;
			break;
		case "1651":
			m_value = 20;
			m_percent = true;
			break;
		case "1495":
			m_value = 25;
			m_percent = true;
			break;
		case "1545":
		case "1546":
		case "1547":
		case "1581":
		case "1582":
		case "1583":
		case "1652":
			m_value = 30;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBaseDamageEquip) is PetReduceBaseDamageEquip petReduceBaseDamageEquip)
		{
			petReduceBaseDamageEquip.m_count = m_count;
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
				m_added = living.BaseDamage * (double)m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.BaseDamage -= m_added;
		}
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.BaseDamage += m_added;
		m_added = 0.0;
		living.BeginSelfTurn -= player_BeginFitting;
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			Stop();
		}
	}
}

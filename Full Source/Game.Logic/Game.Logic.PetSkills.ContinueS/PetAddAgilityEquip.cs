using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddAgilityEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddAgilityEquip(int count, string elementID)
		: base(ePetEffectType.PetAddAgilityEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1403":
			m_value = 10.0;
			m_percent = true;
			break;
		case "1404":
			m_value = 20.0;
			m_percent = true;
			break;
		case "3581":
			m_value = 30.0;
			m_percent = true;
			break;
		case "1577":
			m_value = 70.0;
			m_percent = true;
			break;
		case "1618":
			m_value = 80.0;
			m_percent = true;
			break;
		case "1613":
			m_value = 40.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAgilityEquip) is PetAddAgilityEquip petAddAgilityEquip)
		{
			petAddAgilityEquip.m_count = m_count;
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
				m_added = living.Agility * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Agility += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.Agility -= m_added;
		m_added = 0.0;
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count <= 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
	}
}

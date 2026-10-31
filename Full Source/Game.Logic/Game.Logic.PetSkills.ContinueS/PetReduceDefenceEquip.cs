using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceDefenceEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private double m_added;

	public PetReduceDefenceEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceDefenceEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1206":
			m_value = 300;
			break;
		case "1207":
			m_value = 500;
			break;
		case "1600":
		case "1497":
			m_value = 20;
			m_percent = true;
			break;
		case "1564":
		case "1601":
			m_value = 25;
			m_percent = true;
			break;
		case "1565":
			m_value = 40;
			m_percent = true;
			break;
		case "1627":
			m_value = 45;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceDefenceEquip) is PetReduceDefenceEquip petReduceDefenceEquip)
		{
			petReduceDefenceEquip.m_count = m_count;
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
				m_added = living.Defence * (double)m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Defence -= m_added;
		}
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.Defence += m_added;
		m_added = 0.0;
		living.BeginSelfTurn -= player_BeginFitting;
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
	}
}

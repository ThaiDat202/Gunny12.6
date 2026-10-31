using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceAgilityEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private double m_added;

	public PetReduceAgilityEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceAgilityEquip, elementID)
	{
		m_count = count;
		if (elementID == "3580")
		{
			m_value = 50;
			m_percent = true;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAgilityEquip) is PetReduceAgilityEquip petReduceAgilityEquip)
		{
			petReduceAgilityEquip.m_count = m_count;
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
				m_added = living.Agility * (double)m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Agility -= m_added;
		}
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.Agility += m_added;
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

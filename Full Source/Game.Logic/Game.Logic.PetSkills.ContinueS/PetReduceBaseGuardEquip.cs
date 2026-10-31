using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceBaseGuardEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private double m_added;

	public PetReduceBaseGuardEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceBaseGuardEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1155":
			m_value = 500;
			break;
		case "1156":
			m_value = 650;
			break;
		case "1466":
			m_value = 750;
			break;
		case "1220":
			m_value = 100;
			break;
		case "1221":
			m_value = 200;
			break;
		case "1602":
			m_value = 20;
			m_percent = true;
			break;
		case "1501":
			m_value = 35;
			m_percent = true;
			break;
		case "1566":
		case "1603":
			m_value = 25;
			m_percent = true;
			break;
		case "1567":
			m_value = 40;
			m_percent = true;
			break;
		case "1628":
			m_value = 45;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBaseGuardEquip) is PetReduceBaseGuardEquip petReduceBaseGuardEquip)
		{
			petReduceBaseGuardEquip.m_count = m_count;
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
				m_added = living.BaseGuard * (double)m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.BaseGuard -= m_added;
		}
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.BaseGuard += m_added;
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

using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceAttackEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private double m_added;

	public PetReduceAttackEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceAttackEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1254":
			m_value = 100;
			break;
		case "1255":
			m_value = 200;
			break;
		case "1204":
		case "1256":
			m_value = 300;
			break;
		case "1205":
			m_value = 500;
			break;
		case "1334":
			m_value = 20;
			m_percent = true;
			break;
		case "1335":
			m_value = 15;
			m_percent = true;
			break;
		case "1496":
			m_value = 30;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAttackEquip) is PetReduceAttackEquip petReduceAttackEquip)
		{
			petReduceAttackEquip.m_count = m_count;
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
				m_added = living.Attack * (double)m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Attack -= m_added;
		}
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.Attack += m_added;
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

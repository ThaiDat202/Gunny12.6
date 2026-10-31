using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddAttackEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddAttackEquip(int count, string elementID)
		: base(ePetEffectType.PetAddAttackEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1178":
			m_value = 100.0;
			break;
		case "1194":
			m_value = 200.0;
			break;
		case "1179":
		case "1195":
			m_value = 300.0;
			break;
		case "1180":
			m_value = 500.0;
			break;
		case "1489":
			m_value = 5.0;
			m_percent = true;
			break;
		case "1482":
			m_value = 6.0;
			m_percent = true;
			break;
		case "1399":
			m_value = 10.0;
			m_percent = true;
			break;
		case "1400":
			m_value = 20.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAttackEquip) is PetAddAttackEquip petAddAttackEquip)
		{
			petAddAttackEquip.m_count = m_count;
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
				m_added = living.Attack * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Attack += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.Attack -= m_added;
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

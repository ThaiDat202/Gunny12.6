using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBaseGuardEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddBaseGuardEquip(int count, string elementID)
		: base(ePetEffectType.PetAddBaseGuardEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1181":
		case "1257":
		case "1439":
		case "1522":
			m_value = 100.0;
			break;
		case "1182":
		case "1258":
			m_value = 200.0;
			break;
		case "1183":
		case "1440":
		case "1523":
			m_value = 300.0;
			break;
		case "1441":
		case "1524":
			m_value = 500.0;
			break;
		case "2652":
			m_value = 20.0;
			m_percent = true;
			break;
		case "1484":
			m_value = 30.0;
			m_percent = true;
			break;
		case "2657":
			m_value = 35.0;
			m_percent = true;
			break;
		case "2661":
			m_value = 50.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseGuardEquip) is PetAddBaseGuardEquip petAddBaseGuardEquip)
		{
			petAddBaseGuardEquip.m_count = m_count;
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
				m_added = living.BaseGuard * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.BaseGuard += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BaseGuard -= m_added;
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

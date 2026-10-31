using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddSpeedEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddSpeedEquip(int count, string elementID)
		: base(ePetEffectType.PetAddSpeedEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1577":
			m_value = 50.0;
			m_percent = true;
			break;
		case "1613":
			m_value = 40.0;
			m_percent = true;
			break;
		case "1618":
			m_value = 60.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddSpeedEquip) is PetAddSpeedEquip petAddSpeedEquip)
		{
			petAddSpeedEquip.m_count = m_count;
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
				m_added = living.Speed * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Speed += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.Speed -= m_added;
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

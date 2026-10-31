using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddLuckyEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddLuckyEquip(int count, string elementID)
		: base(ePetEffectType.PetAddLuckyEquip, elementID)
	{
		m_count = count;
		if (!(elementID == "1405"))
		{
			if (elementID == "1406")
			{
				m_value = 20.0;
				m_percent = true;
			}
		}
		else
		{
			m_value = 10.0;
			m_percent = true;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddLuckyEquip) is PetAddLuckyEquip petAddLuckyEquip)
		{
			petAddLuckyEquip.m_count = m_count;
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
				m_added = living.Lucky * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.Lucky += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.Lucky -= m_added;
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

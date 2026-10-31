using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceSpeedEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private int m_added;

	public PetReduceSpeedEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceSpeedEquip, elementID)
	{
		m_count = count;
		if (elementID == "1732" || elementID == "1838")
		{
			m_value = 50;
			m_percent = true;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceSpeedEquip) is PetReduceSpeedEquip petReduceSpeedEquip)
		{
			petReduceSpeedEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		if (m_added == 0)
		{
			if (m_percent)
			{
				m_added = (int)(living.Speed * (double)m_value / 100.0);
			}
			else
			{
				m_added = m_value;
			}
			living.Speed -= m_added;
			living.Game.UpdateMaxBlood(living);
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.Speed += m_added;
		m_added = 0;
		living.Game.UpdateMaxBlood(living);
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

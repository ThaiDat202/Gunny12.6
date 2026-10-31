using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceMaxBloodEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private int m_added;

	public PetReduceMaxBloodEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceMaxBloodEquip, elementID)
	{
		m_count = count;
		if (elementID == "2757")
		{
			m_value = 12;
			m_percent = true;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceMaxBloodEquip) is PetReduceMaxBloodEquip petReduceMaxBloodEquip)
		{
			petReduceMaxBloodEquip.m_count = m_count;
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
				m_added = living.MaxBlood * m_value / 100;
			}
			else
			{
				m_added = m_value;
			}
			living.MaxBlood -= m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.MaxBlood += m_added;
		m_added = 0;
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

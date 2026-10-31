using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddMaxBloodEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private int m_added;

	private int m_added1;

	public PetAddMaxBloodEquip(int count, string elementID)
		: base(ePetEffectType.PetAddMaxBloodEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "2755":
			m_value = 10;
			m_percent = true;
			break;
		case "1467":
			m_value = 15;
			m_percent = true;
			break;
		case "3175":
			m_value = 25;
			m_percent = true;
			break;
		case "1735":
		case "1839":
			m_value = 100;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMaxBloodEquip) is PetAddMaxBloodEquip petAddMaxBloodEquip)
		{
			petAddMaxBloodEquip.m_count = m_count;
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
				m_added1 = living.Blood * m_value / 100;
			}
			else
			{
				m_added = (m_added1 = m_value);
			}
			if (m_value != 15)
			{
				living.Blood += m_added1;
			}
			living.MaxBlood += m_added;
			living.Game.UpdateMaxBlood(living);
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.MaxBlood -= m_added;
		if (m_value != 15)
		{
			living.Blood -= m_added1;
		}
		m_added = 0;
		m_added1 = 0;
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

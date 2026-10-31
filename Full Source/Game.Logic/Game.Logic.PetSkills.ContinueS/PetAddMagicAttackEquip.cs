using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddMagicAttackEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private double m_added;

	public PetAddMagicAttackEquip(int count, string elementID)
		: base(ePetEffectType.PetAddMagicAttackEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1407":
			m_value = 10.0;
			m_percent = true;
			break;
		case "1408":
			m_value = 20.0;
			m_percent = true;
			break;
		case "1743":
			m_value = 35.0;
			m_percent = true;
			break;
		case "1813":
			m_value = 20.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicAttackEquip) is PetAddMagicAttackEquip petAddMagicAttackEquip)
		{
			petAddMagicAttackEquip.m_count = m_count;
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
				m_added = living.MagicAttack * m_value / 100.0;
			}
			else
			{
				m_added = m_value;
			}
			living.MagicAttack += m_added;
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.MagicAttack -= m_added;
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

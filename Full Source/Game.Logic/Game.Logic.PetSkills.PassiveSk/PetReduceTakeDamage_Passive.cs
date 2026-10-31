using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceTakeDamage_Passive : AbstractPetEffect
{
	private int m_probability;

	private double m_value;

	private bool m_percent;

	private string m_elementID;

	public PetReduceTakeDamage_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReduceTakeDamage_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
		switch (elementID)
		{
		case "1655":
			m_value = 20.0;
			break;
		case "1656":
			m_value = 30.0;
			break;
		case "1804":
			m_value = 2.0;
			m_percent = true;
			break;
		case "1767":
		case "1805":
			m_value = 3.0;
			m_percent = true;
			break;
		case "1768":
		case "1806":
			m_value = 4.0;
			m_percent = true;
			break;
		case "1769":
			m_value = 5.0;
			m_percent = true;
			break;
		case "1774":
			m_value = 7.0;
			m_percent = true;
			break;
		case "3223":
			m_value = 30.0;
			m_percent = true;
			break;
		case "3490":
			m_value = 60.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTakeDamage_Passive) is PetReduceTakeDamage_Passive petReduceTakeDamage_Passive)
		{
			petReduceTakeDamage_Passive.m_probability = ((m_probability > petReduceTakeDamage_Passive.m_probability) ? m_probability : petReduceTakeDamage_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	public override void OnRemoved(Living player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (m_elementID == "3223" || m_elementID == "3490")
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			damageAmount -= damageAmount * (int)m_value / 100;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
		else
		{
			if (!(living.Game is PVPGame))
			{
				return;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			if (m_elementID == "1655" || m_elementID == "1656")
			{
				if (damageAmount > (int)((double)living.MaxBlood * m_value / 100.0))
				{
					damageAmount = (int)((double)living.MaxBlood * m_value / 100.0);
				}
			}
			else
			{
				double num = m_value;
				if (m_percent)
				{
					num = (double)living.Blood * m_value / 100.0;
				}
				if (m_elementID == "1774")
				{
					num = (double)living.MaxBlood * m_value / 100.0;
				}
				damageAmount -= (int)num;
				if (damageAmount < 0)
				{
					damageAmount = 1;
				}
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
	}
}

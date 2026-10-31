using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceTakeDamageEquip : AbstractPetEffect
{
	private int m_count;

	private double m_value;

	private bool m_percent;

	private string m_elementID;

	public PetReduceTakeDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceTakeDamageEquip, elementID)
	{
		m_count = count;
		m_elementID = elementID;
		switch (elementID)
		{
		case "1033":
		case "1232":
		case "1233":
			m_value = 5.0;
			m_percent = true;
			break;
		case "1032":
		case "1034":
		case "1234":
		case "1443":
		case "1445":
		case "1446":
		case "1428":
		case "1526":
		case "2645":
			m_value = 10.0;
			m_percent = true;
			break;
		case "1380":
		case "1281":
		case "3174":
		case "3584":
		case "2648":
			m_value = 15.0;
			m_percent = true;
			break;
		case "1442":
		case "1444":
		case "1527":
		case "1429":
		case "2650":
		case "1619":
		case "3220":
			m_value = 20.0;
			m_percent = true;
			break;
		case "1381":
		case "1282":
		case "3223":
		case "1509":
			m_value = 30.0;
			m_percent = true;
			break;
		case "1283":
		case "1537":
			m_value = 50.0;
			m_percent = true;
			break;
		case "1804":
			m_value = 1.0;
			m_percent = true;
			break;
		case "1767":
		case "1805":
			m_value = 1.5;
			m_percent = true;
			break;
		case "1768":
		case "1806":
			m_value = 2.0;
			m_percent = true;
			break;
		case "1769":
			m_value = 2.5;
			m_percent = true;
			break;
		case "1774":
			m_value = 3.0;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTakeDamageEquip) is PetReduceTakeDamageEquip petReduceTakeDamageEquip)
		{
			petReduceTakeDamageEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living player)
	{
		player.Game.sendShowPicSkil(player, base.Info, isActive: false);
		player.Game.sendShowPicSkil(player, base.Info, isActive: true);
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.BeginSelfTurn += player_SelfTurn;
	}

	public override void OnRemoved(Living player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.BeginSelfTurn += player_SelfTurn;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		living.Game.sendShowPicSkil(living, base.Info, isActive: false);
		if (m_elementID == "1442" || m_elementID == "1443" || m_elementID == "1444" || m_elementID == "1525")
		{
			if (m_percent)
			{
				damageAmount -= 500 + (int)((double)damageAmount * m_value / 100.0);
			}
			else
			{
				damageAmount -= 500 + (int)m_value;
			}
		}
		else if (m_elementID == "1767" || m_elementID == "1768" || m_elementID == "1804" || m_elementID == "1805" || m_elementID == "1806" || m_elementID == "1774")
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
			if (num < 0.0)
			{
				damageAmount = 1;
			}
			else
			{
				damageAmount = (int)num;
			}
		}
		else if (m_percent)
		{
			damageAmount -= (int)((double)damageAmount * m_value / 100.0);
		}
		else
		{
			damageAmount -= (int)m_value;
		}
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
	}

	private void player_SelfTurn(Living living)
	{
		living.Game.sendShowPicSkil(living, base.Info, isActive: false);
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddTakeDamageEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private string m_elementID;

	public PetAddTakeDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetAddTakeDamageEquip, elementID)
	{
		m_count = count;
		m_elementID = elementID;
		switch (elementID)
		{
		case "1386":
			m_value = 15;
			break;
		case "1433":
			m_value = 20;
			break;
		case "1387":
		case "1511":
			m_value = 30;
			break;
		case "1659":
			m_value = 1;
			break;
		case "1660":
			m_value = 3;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddTakeDamageEquip) is PetAddTakeDamageEquip petAddTakeDamageEquip)
		{
			petAddTakeDamageEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginFitting;
		living.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		living.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (m_elementID == "1659" || m_elementID == "1660")
		{
			damageAmount += (living.MaxBlood - living.Blood) * m_value / 100;
		}
		else
		{
			damageAmount += damageAmount * m_value / 100;
		}
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

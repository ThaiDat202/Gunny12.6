using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddCritDamageEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	public PetAddCritDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetAddCritDamageEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1377":
			m_value = 10;
			m_percent = true;
			break;
		case "1378":
		case "3177":
			m_value = 20;
			m_percent = true;
			break;
		case "1379":
		case "1500":
			m_value = 30;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddCritDamageEquip) is PetAddCritDamageEquip petAddCritDamageEquip)
		{
			petAddCritDamageEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living player)
	{
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
		if (m_percent)
		{
			criticalAmount += criticalAmount * m_value / 100;
		}
		else
		{
			criticalAmount += m_value;
		}
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
	}

	private void player_SelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

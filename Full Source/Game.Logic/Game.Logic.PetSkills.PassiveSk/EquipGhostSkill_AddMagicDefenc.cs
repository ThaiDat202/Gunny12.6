using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class EquipGhostSkill_AddMagicDefence : BasePetEffect
{
	private int m_coldDown = 0;

	private int m_value = 0;

	private double m_added = 0.0;

	public EquipGhostSkill_AddMagicDefence(string elementID)
		: base(ePetEffectType.EquipGhostSkill_AddMagicDefence, elementID)
	{
		m_coldDown = 0;
		switch (elementID)
		{
		case "2401":
			m_value = 2;
			break;
		case "2402":
			m_value = 4;
			break;
		case "2403":
			m_value = 6;
			break;
		case "2404":
			m_value = 8;
			break;
		case "2405":
			m_value = 10;
			break;
		case "2406":
			m_value = 12;
			break;
		case "2407":
			m_value = 14;
			break;
		case "2408":
			m_value = 16;
			break;
		case "2409":
			m_value = 18;
			break;
		case "2410":
			m_value = 20;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.EquipGhostSkill_AddMagicDefence) is EquipGhostSkill_AddMagicDefence)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.AfterKilledByLiving += player_AfterKilledByLiving;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_BeforeTakeDamage(Living living, Living target, ref int damageAmount, ref int criticalAmount)
	{
		if (m_coldDown < 10)
		{
			m_added += living.MagicDefence * (double)m_value / 100.0;
			living.MagicDefence += m_added;
			IsTrigger = true;
			m_coldDown++;
		}
	}

	private void player_BeginSelfTurn(Living living)
	{
		living.MagicDefence -= m_added;
		m_added = 0.0;
		m_coldDown = 0;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (IsTrigger)
		{
			IsTrigger = false;
		}
	}
}

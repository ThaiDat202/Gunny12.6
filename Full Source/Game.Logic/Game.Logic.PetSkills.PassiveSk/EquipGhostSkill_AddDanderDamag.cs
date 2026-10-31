using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class EquipGhostSkill_AddDanderDamage : BasePetEffect
{
	private int m_value = 0;

	private bool isDander = false;

	public EquipGhostSkill_AddDanderDamage(string elementID)
		: base(ePetEffectType.EquipGhostSkill_AddDanderDamage, elementID)
	{
		switch (elementID)
		{
		case "2101":
			m_value = 5;
			break;
		case "2102":
			m_value = 10;
			break;
		case "2103":
			m_value = 15;
			break;
		case "2104":
			m_value = 20;
			break;
		case "2105":
			m_value = 25;
			break;
		case "2106":
			m_value = 30;
			break;
		case "2107":
			m_value = 35;
			break;
		case "2108":
			m_value = 40;
			break;
		case "2109":
			m_value = 45;
			break;
		case "2110":
			m_value = 50;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.EquipGhostSkill_AddDanderDamage) is EquipGhostSkill_AddDanderDamage)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerUseDander += player_UseDander;
		player.TakePlayerDamage += player_TakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerUseDander -= player_UseDander;
		player.TakePlayerDamage -= player_TakeDamage;
	}

	private void player_TakeDamage(Living living, Living target, ref int damageAmount, ref int criticalAmount)
	{
		if (isDander)
		{
			damageAmount += damageAmount * m_value / 100;
			isDander = false;
		}
	}

	private void player_UseDander(Player player)
	{
		isDander = true;
	}
}

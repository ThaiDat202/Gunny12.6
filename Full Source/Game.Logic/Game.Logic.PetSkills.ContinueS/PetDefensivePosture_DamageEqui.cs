using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetDefensivePosture_DamageEquip : BasePetEffect
{
	private int m_count;

	private int m_value;

	public PetDefensivePosture_DamageEquip(int count, string elementID)
		: base(ePetEffectType.PetDefensivePosture_DamageEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1590":
			m_value = 10;
			break;
		case "1554":
		case "1591":
			m_value = 15;
			break;
		case "1555":
			m_value = 25;
			break;
		case "20002":
			m_value = 35;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_DamageEquip) is PetDefensivePosture_DamageEquip petDefensivePosture_DamageEquip)
		{
			petDefensivePosture_DamageEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.BeginSelfTurn -= player_BeginSelfTurn;
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

	private void player_BeforeTakeDamage(Living living, Living targe, ref int damageAmount, ref int criticalAmount)
	{
		damageAmount -= damageAmount * m_value / 100;
	}
}

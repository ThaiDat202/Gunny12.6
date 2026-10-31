using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetDefensivePosture_CritDamageEquip : BasePetEffect
{
	private int m_count;

	private int m_value;

	public PetDefensivePosture_CritDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetDefensivePosture_CritDamageEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1588":
			m_value = 20;
			break;
		case "1552":
		case "1589":
			m_value = 25;
			break;
		case "1553":
			m_value = 35;
			break;
		case "20001":
			m_value = 50;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_CritDamageEquip) is PetDefensivePosture_CritDamageEquip petDefensivePosture_CritDamageEquip)
		{
			petDefensivePosture_CritDamageEquip.m_count = m_count;
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
		criticalAmount -= criticalAmount * m_value / 100;
	}
}

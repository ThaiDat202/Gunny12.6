using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddGuardEquipEffect : BasePlayerEffect
{
	private double m_reduceDamage = 0.0;

	private int m_probability = 0;

	public AddGuardEquipEffect(double reduceDamage, int probability)
		: base(eEffectType.AddGuardEquipEffect)
	{
		m_reduceDamage = reduceDamage;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddGuardEquipEffect) is AddGuardEquipEffect addGuardEquipEffect)
		{
			addGuardEquipEffect.m_probability = m_probability;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AddArmor = true;
		player.BeginSelfTurn += player_SelfTurn;
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.Game.SendPlayerPicture(player, 6, state: true);
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AddArmor = false;
		player.BeginSelfTurn -= player_SelfTurn;
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.Game.SendPlayerPicture(player, 6, state: false);
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		damageAmount -= (int)(m_reduceDamage / 1000.0 * (double)damageAmount);
		if (damageAmount <= 0)
		{
			damageAmount = 1;
		}
	}

	private void player_SelfTurn(Living living)
	{
		m_probability--;
		if (m_probability < 0)
		{
			Stop();
		}
	}
}

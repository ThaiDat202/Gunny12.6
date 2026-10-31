using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddDamageTurnEffect : BasePlayerEffect
{
	private int m_count = 0;

	private int m_probability = 0;

	public AddDamageTurnEffect(int count, int probability)
		: base(eEffectType.AddDamageTurnEffect)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddDamageTurnEffect) is AddDamageTurnEffect addDamageTurnEffect)
		{
			m_probability = ((m_probability > addDamageTurnEffect.m_probability) ? m_probability : addDamageTurnEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.TakePlayerDamage += player_BeforeTakeDamage;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
		player.BeginSelfTurn += player_SelfTurn;
		player.Game.SendPlayerPicture(player, 29, state: true);
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.TakePlayerDamage -= player_BeforeTakeDamage;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
		player.BeginSelfTurn -= player_SelfTurn;
		player.Game.SendPlayerPicture(player, 29, state: false);
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.FlyingPartical = 0;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (IsTrigger)
		{
			damageAmount += m_count;
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

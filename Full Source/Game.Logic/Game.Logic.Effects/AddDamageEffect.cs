using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddDamageEffect : BasePlayerEffect
{
	private int m_count = 0;

	private int m_probability = 0;

	public AddDamageEffect(int count, int probability)
		: base(eEffectType.AddDamageEffect)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddDamageEffect) is AddDamageEffect addDamageEffect)
		{
			m_probability = ((m_probability > addDamageEffect.m_probability) ? m_probability : addDamageEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.TakePlayerDamage += player_BeforeTakeDamage;
		player.PlayerShoot += playerShot;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.TakePlayerDamage -= player_BeforeTakeDamage;
		player.PlayerShoot -= playerShot;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.FlyingPartical = 0;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		m_count = (int)(living.Attack * 0.3);
		if (IsTrigger)
		{
			damageAmount += m_count;
		}
	}

	private void playerShot(Player player)
	{
		IsTrigger = false;
		if (!player.CurrentBall.IsSpecial() && AbstractEffect.rand.Next(100) < m_probability && player.AttackGemLimit == 0)
		{
			player.AttackGemLimit = 2;
			IsTrigger = true;
			player.FlyingPartical = 65;
			player.EffectTrigger = true;
			player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
			player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddDamageEffect.msg"), 9, 0, 1000));
		}
	}
}

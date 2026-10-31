using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class MakeCriticalEffect : BasePlayerEffect
{
	private int m_count = 0;

	private int m_probability = 0;

	public MakeCriticalEffect(int count, int probability)
		: base(eEffectType.MakeCriticalEffect)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.MakeCriticalEffect) is MakeCriticalEffect makeCriticalEffect)
		{
			makeCriticalEffect.m_probability = ((m_probability > makeCriticalEffect.m_probability) ? m_probability : makeCriticalEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.TakePlayerDamage += player_BeforeTakeDamage;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.TakePlayerDamage -= player_BeforeTakeDamage;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.FlyingPartical = 0;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		IsTrigger = false;
		if (!(living as Player).CurrentBall.IsSpecial() && living != source && AbstractEffect.rand.Next(100) < m_probability && living.AttackGemLimit == 0)
		{
			living.AttackGemLimit = 2;
			IsTrigger = true;
			living.EffectTrigger = true;
			criticalAmount = (int)(0.2 + living.Lucky * 1.5E-05 * (double)damageAmount);
			living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("AttackEffect.Success"));
			living.FlyingPartical = 65;
			living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("MakeCriticalEffect.msg"), 9, 0, 1000));
		}
	}
}

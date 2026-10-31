using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

internal class hudun : BasePlayerEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	public hudun(int count, int probability, int type)
		: base(eEffectType.hudun)
	{
		m_count = count;
		m_probability = probability;
		m_type = type;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.hudun) is hudun hudun2)
		{
			hudun2.m_probability = ((m_probability > hudun2.m_probability) ? m_probability : hudun2.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		IsTrigger = false;
		if (AbstractEffect.rand.Next(100) < m_probability && m_count > 0)
		{
			IsTrigger = true;
			living.EffectTrigger = true;
			living.SyncAtTime = true;
			if (damageAmount > m_count)
			{
				m_count = 0;
				living.AddBlood(m_count);
			}
			else
			{
				m_count -= damageAmount;
			}
			living.SyncAtTime = false;
			damageAmount -= damageAmount;
			criticalAmount -= criticalAmount;
			living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("冰骸铠甲"), 9, 0, 1000));
		}
	}
}

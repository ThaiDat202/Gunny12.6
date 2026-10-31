using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseReduceDamageEffect : AbstractHorseEffect
{
	private int m_count;

	private int m_currentId;

	private int m_reducePlus;

	private int m_reduceDamage;

	public HorseReduceDamageEffect(int count, int plusReduce, int damageReduce, int skillId, string elementID)
		: base(eHorseEffectType.HorseReduceDamageEffect, elementID)
	{
		m_count = count;
		m_currentId = skillId;
		m_reducePlus = plusReduce;
		m_reduceDamage = damageReduce;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseReduceDamageEffect) is HorseReduceDamageEffect horseReduceDamageEffect)
		{
			horseReduceDamageEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_SelftTurn;
		living.BeforeTakeDamage += player_takedDamage;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_SelftTurn;
		living.BeforeTakeDamage -= player_takedDamage;
	}

	private void player_SelftTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
		}
	}

	private void player_takedDamage(Living target, Living source, ref int damage, ref int crit)
	{
		int num = (int)((double)damage / 100.0 * (double)m_reducePlus) + m_reduceDamage;
		if (num >= damage || num <= 0)
		{
			damage = 1;
		}
		else
		{
			damage -= num;
		}
	}
}

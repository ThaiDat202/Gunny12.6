using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseAddDamageNextDanderEffect : AbstractHorseEffect
{
	private int m_count;

	private int m_currentId;

	private int m_turnDander;

	public HorseAddDamageNextDanderEffect(int count, int skillId, string elementID)
		: base(eHorseEffectType.HorseAddDamageNextDanderEffect, elementID)
	{
		m_count = count;
		m_currentId = skillId;
		m_turnDander = 0;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseAddDamageNextDanderEffect) is HorseAddDamageNextDanderEffect horseAddDamageNextDanderEffect)
		{
			horseAddDamageNextDanderEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		(living as Player).PlayerUseDander += player_UseDander;
		(living as Player).TakePlayerDamage += player_takedDamage;
	}

	public override void OnRemoved(Living living)
	{
		(living as Player).PlayerUseDander -= player_UseDander;
		(living as Player).TakePlayerDamage -= player_takedDamage;
	}

	private void player_UseDander(Player player)
	{
		m_turnDander++;
	}

	private void player_takedDamage(Living target, Living source, ref int damage, ref int crit)
	{
		if (m_turnDander >= 2)
		{
			int num = damage / 100 * m_count;
			damage += num;
			Stop();
		}
	}
}

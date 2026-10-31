using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseAddDamageEffect : AbstractHorseEffect
{
	private int m_count;

	private int m_currentId;

	private int m_attAdd;

	private int m_attPlus;

	public HorseAddDamageEffect(int count, int attackAdd, int attackPlus, int skillId, string elementID)
		: base(eHorseEffectType.HorseAddDamageEffect, elementID)
	{
		m_count = count;
		m_currentId = skillId;
		m_attAdd = attackAdd;
		m_attPlus = attackPlus;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseAddDamageEffect) is HorseAddDamageEffect horseAddDamageEffect)
		{
			horseAddDamageEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_SelftTurn;
		living.TakePlayerDamage += player_takedDamage;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_SelftTurn;
		living.TakePlayerDamage -= player_takedDamage;
	}

	private void player_SelftTurn(Living living)
	{
		m_count--;
		if (m_count <= 0)
		{
			Stop();
		}
	}

	private void player_takedDamage(Living target, Living source, ref int damage, ref int crit)
	{
		int num = damage / 100 * m_attPlus + m_attAdd;
		damage += num;
	}
}

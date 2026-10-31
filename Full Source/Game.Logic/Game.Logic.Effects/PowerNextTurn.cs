using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class PowerNextTurn : AbstractEffect
{
	private int m_count;

	private int m_damage;

	public PowerNextTurn(int count, int damage)
		: base(eEffectType.PowerNextTurn)
	{
		m_count = count;
		m_damage = damage;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.PowerNextTurn) is PowerNextTurn powerNextTurn)
		{
			powerNextTurn.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		living.SetNiutou(state: false);
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count == 0)
		{
			living.CurrentDamagePlus += m_damage / 100;
			living.SetNiutou(state: true);
		}
		else if (m_count < 0)
		{
			Stop();
		}
	}
}

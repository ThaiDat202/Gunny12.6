using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.ContinuosEffect;

public class ContinuosResistanceSoul : BaseGuardCoreEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_currentId;

	private int m_totalValue = 0;

	private int m_parameter1 = 0;

	private int m_parameter2 = 0;

	private int m_probability = 10000;

	public ContinuosResistanceSoul(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.RestorationSoulContinuos, info)
	{
		m_count = info.KeepTurn;
		m_type = info.Type;
		m_currentId = info.ID;
		m_parameter1 = info.Parameter1;
		m_parameter2 = info.Parameter2;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.RestorationSoulContinuos) is ContinuosResistanceSoul continuosResistanceSoul)
		{
			continuosResistanceSoul.m_probability = ((m_probability > continuosResistanceSoul.m_probability) ? m_probability : continuosResistanceSoul.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += Player_BeginSelfTurn;
		player.BeginNextTurn += Player_BeginNextTurn;
	}

	private void Player_BeginNextTurn(Living living)
	{
		if (m_totalValue == 0)
		{
			m_totalValue = (int)(living.MagicDefence * (double)m_parameter1 / 10.0 / 100.0);
			m_totalValue += m_parameter2;
			living.MagicDefence += m_totalValue;
		}
	}

	private void Player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.MagicDefence -= m_totalValue;
			Stop();
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= Player_BeginSelfTurn;
		player.BeginNextTurn -= Player_BeginNextTurn;
	}
}

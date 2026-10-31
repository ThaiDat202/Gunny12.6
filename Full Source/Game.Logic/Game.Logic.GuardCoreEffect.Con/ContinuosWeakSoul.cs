using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.ContinuosEffect;

public class ContinuosWeakSoul : BaseGuardCoreEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_currentId;

	private int m_totalValue = 0;

	private int m_parameter1 = 0;

	private int m_parameter2 = 0;

	private int m_probability = 10000;

	public ContinuosWeakSoul(GuardCoreTemplateInfo info)
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
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.RestorationSoulContinuos) is ContinuosWeakSoul continuosWeakSoul)
		{
			continuosWeakSoul.m_probability = ((m_probability > continuosWeakSoul.m_probability) ? m_probability : continuosWeakSoul.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += Player_BeginSelfTurn;
	}

	private void Player_BeginSelfTurn(Living living)
	{
		m_count--;
		m_totalValue = m_parameter2;
		if ((m_living as Player).Energy >= 450)
		{
			m_totalValue += (living as Player).Energy * m_parameter1 / 10 / 100;
		}
		(living as Player).Energy += m_totalValue;
		if (m_count < 0)
		{
			Stop();
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= Player_BeginSelfTurn;
	}
}

using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.ContinuosEffect;

public class ContinuosHealthSoul : BaseGuardCoreEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_currentId;

	private int m_totalValue = 0;

	private int m_parameter1 = 0;

	private int m_parameter2 = 0;

	private int m_probability = 10000;

	public ContinuosHealthSoul(GuardCoreTemplateInfo info)
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
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.RestorationSoulContinuos) is ContinuosHealthSoul continuosHealthSoul)
		{
			continuosHealthSoul.m_probability = ((m_probability > continuosHealthSoul.m_probability) ? m_probability : continuosHealthSoul.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		if (m_totalValue == 0)
		{
			m_totalValue = player.MaxBlood * m_parameter1 / 10 / 100 + m_parameter2;
			m_living.ExtendMaxBlood = m_totalValue;
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

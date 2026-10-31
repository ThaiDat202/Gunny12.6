using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.ContinuosEffect;

public class ContinuosRestorationSoul : BaseGuardCoreEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_currentId;

	private int m_parameter1 = 0;

	private int m_parameter2 = 0;

	private int m_probability = 10000;

	public ContinuosRestorationSoul(GuardCoreTemplateInfo info)
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
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.RestorationSoulContinuos) is ContinuosRestorationSoul continuosRestorationSoul)
		{
			continuosRestorationSoul.m_probability = ((m_probability > continuosRestorationSoul.m_probability) ? m_probability : continuosRestorationSoul.m_probability);
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
		if (living.Blood < living.MaxBlood)
		{
			double num = living.MaxBlood * m_parameter1 / 10 / 100;
			living.SyncAtTime = true;
			living.AddBlood(m_parameter2 + (int)num);
			living.SyncAtTime = false;
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= Player_BeginSelfTurn;
	}
}

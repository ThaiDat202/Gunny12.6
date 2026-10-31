using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class HealthSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public HealthSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.HealthSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.HealthSoulEquip) is HealthSoulEffect healthSoulEffect)
		{
			healthSoulEffect.m_probability = ((m_probability > healthSoulEffect.m_probability) ? m_probability : healthSoulEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		if (IsTrigger)
		{
			return;
		}
		List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
		foreach (Player item in allTeamPlayers)
		{
			ContinuosHealthSoul continuosHealthSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.HealthSoulContinuos) as ContinuosHealthSoul;
			if (continuosHealthSoul != null && continuosHealthSoul.Info.ID < m_currentId)
			{
				continuosHealthSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosHealthSoul(base.Info), 0);
			}
			if (continuosHealthSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosHealthSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

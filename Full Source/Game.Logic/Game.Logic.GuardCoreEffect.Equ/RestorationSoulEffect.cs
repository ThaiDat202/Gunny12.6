using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class RestorationSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public RestorationSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.RestorationSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.RestorationSoulEquip) is RestorationSoulEffect restorationSoulEffect)
		{
			restorationSoulEffect.m_probability = ((m_probability > restorationSoulEffect.m_probability) ? m_probability : restorationSoulEffect.m_probability);
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
			ContinuosRestorationSoul continuosRestorationSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.RestorationSoulContinuos) as ContinuosRestorationSoul;
			if (continuosRestorationSoul != null && continuosRestorationSoul.Info.ID < m_currentId)
			{
				continuosRestorationSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosRestorationSoul(base.Info), 0);
			}
			if (continuosRestorationSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosRestorationSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

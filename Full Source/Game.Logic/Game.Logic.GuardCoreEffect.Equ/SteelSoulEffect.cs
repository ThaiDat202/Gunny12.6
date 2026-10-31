using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class SteelSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public SteelSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.SteelSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.SteelSoulEquip) is SteelSoulEffect steelSoulEffect)
		{
			steelSoulEffect.m_probability = ((m_probability > steelSoulEffect.m_probability) ? m_probability : steelSoulEffect.m_probability);
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
			ContinuosSteelSoul continuosSteelSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.SteelSoulContinuos) as ContinuosSteelSoul;
			if (continuosSteelSoul != null && continuosSteelSoul.Info.ID < m_currentId)
			{
				continuosSteelSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosSteelSoul(base.Info), 0);
			}
			if (continuosSteelSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosSteelSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

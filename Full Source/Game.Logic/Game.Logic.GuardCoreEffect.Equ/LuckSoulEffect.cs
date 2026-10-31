using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class LuckSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public LuckSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.LuckSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.LuckSoulEquip) is LuckSoulEffect luckSoulEffect)
		{
			luckSoulEffect.m_probability = ((m_probability > luckSoulEffect.m_probability) ? m_probability : luckSoulEffect.m_probability);
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
		List<Player> allEnemyPlayers = player.Game.GetAllEnemyPlayers(player);
		foreach (Player item in allEnemyPlayers)
		{
			ContinuosLuckSoul continuosLuckSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.LuckSoulContinuos) as ContinuosLuckSoul;
			if (continuosLuckSoul != null && continuosLuckSoul.Info.ID < m_currentId)
			{
				continuosLuckSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosLuckSoul(base.Info), 0);
			}
			if (continuosLuckSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosLuckSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

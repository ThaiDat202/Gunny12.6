using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class WeakSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public WeakSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.WeakSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.WeakSoulEquip) is WeakSoulEffect weakSoulEffect)
		{
			weakSoulEffect.m_probability = ((m_probability > weakSoulEffect.m_probability) ? m_probability : weakSoulEffect.m_probability);
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
			ContinuosWeakSoul continuosWeakSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.WeakSoulContinuos) as ContinuosWeakSoul;
			if (continuosWeakSoul != null && continuosWeakSoul.Info.ID < m_currentId)
			{
				continuosWeakSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosWeakSoul(base.Info), 0);
			}
			if (continuosWeakSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosWeakSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

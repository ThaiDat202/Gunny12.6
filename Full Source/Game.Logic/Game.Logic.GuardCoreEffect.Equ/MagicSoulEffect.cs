using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class MagicSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public MagicSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.MagicSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.MagicSoulEquip) is MagicSoulEffect magicSoulEffect)
		{
			magicSoulEffect.m_probability = ((m_probability > magicSoulEffect.m_probability) ? m_probability : magicSoulEffect.m_probability);
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
			ContinuosMagicSoul continuosMagicSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.MagicSoulContinuos) as ContinuosMagicSoul;
			if (continuosMagicSoul != null && continuosMagicSoul.Info.ID < m_currentId)
			{
				continuosMagicSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosMagicSoul(base.Info), 0);
			}
			if (continuosMagicSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosMagicSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

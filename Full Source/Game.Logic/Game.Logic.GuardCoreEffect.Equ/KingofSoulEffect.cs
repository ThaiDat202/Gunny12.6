using System.Collections.Generic;
using Game.Logic.GuardCoreEffect.ContinuosEffect;
using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffect.EquipEffect;

public class KingofSoulEffect : BaseGuardCoreEffect
{
	private int m_probability = 10000;

	private int m_currentId;

	public KingofSoulEffect(GuardCoreTemplateInfo info)
		: base(eGuardCoreEffectType.KingofSoulEquip, info)
	{
		m_currentId = info.ID;
	}

	public override bool Start(Living living)
	{
		if (living.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.KingofSoulEquip) is KingofSoulEffect kingofSoulEffect)
		{
			kingofSoulEffect.m_probability = ((m_probability > kingofSoulEffect.m_probability) ? m_probability : kingofSoulEffect.m_probability);
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
			ContinuosKingofSoul continuosKingofSoul = item.GuardCoreEffectList.GetOfType(eGuardCoreEffectType.KingofSoulContinuos) as ContinuosKingofSoul;
			if (continuosKingofSoul != null && continuosKingofSoul.Info.ID < m_currentId)
			{
				continuosKingofSoul.Stop();
				item.AddGuardCoreEffect(new ContinuosKingofSoul(base.Info), 0);
			}
			if (continuosKingofSoul == null)
			{
				item.AddGuardCoreEffect(new ContinuosKingofSoul(base.Info), 0);
			}
		}
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
	}
}

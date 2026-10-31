using System.Collections.Generic;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseHideTeamEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseHideTeamEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseHideTeamEquipEffect, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseHideTeamEquipEffect) is HorseHideTeamEquipEffect horseHideTeamEquipEffect)
		{
			horseHideTeamEquipEffect.m_probability = ((m_probability > horseHideTeamEquipEffect.m_probability) ? m_probability : horseHideTeamEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillHorse += player_PlayerBuffSkillHorse;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillHorse -= player_PlayerBuffSkillHorse;
	}

	private void player_PlayerBuffSkillHorse(Living living)
	{
		if (rand.Next(10000) >= m_probability || living.PetEffects.CurrentHorseSkill != m_currentId)
		{
			return;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			if (item.IsLiving)
			{
				item.AddEffect(new HideEffect(m_count), 0);
			}
		}
	}
}

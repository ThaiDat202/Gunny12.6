using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseReviveTeamEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private double m_count = 0.0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseReviveTeamEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseReviveTeamEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "10501":
			m_count = 20.0;
			break;
		case "10502":
			m_count = 26.0;
			break;
		case "10503":
			m_count = 32.0;
			break;
		case "10504":
			m_count = 38.0;
			break;
		case "10505":
			m_count = 44.0;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseReviveTeamEquipEffect) is HorseReviveTeamEquipEffect horseReviveTeamEquipEffect)
		{
			horseReviveTeamEquipEffect.m_probability = ((m_probability > horseReviveTeamEquipEffect.m_probability) ? m_probability : horseReviveTeamEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillHorse += player_AfterBuff;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillHorse -= player_AfterBuff;
	}

	private void player_AfterBuff(Living living)
	{
		if (rand.Next(10000) >= m_probability || living.PetEffects.CurrentHorseSkill != m_currentId)
		{
			return;
		}
		(living as Player).AddDelay(m_delay);
		if (living.IsLiving)
		{
			List<Player> allTeamPlayerDies = living.Game.GetAllTeamPlayerDies(living);
			Player player = null;
			if (allTeamPlayerDies.Count > 1)
			{
				int index = rand.Next(allTeamPlayerDies.Count);
				player = allTeamPlayerDies[index];
			}
			else if (allTeamPlayerDies.Count == 1)
			{
				player = allTeamPlayerDies[0];
			}
			if (player != null)
			{
				int blood = (int)((double)(player.MaxBlood / 100) * m_count);
				player.Revive(blood, living.X, living.Y);
			}
		}
	}
}

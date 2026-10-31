using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseBuffHPTeamEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private double m_count = 0.0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	private int m_hp = 0;

	public HorseBuffHPTeamEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseBuffHPTeamEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "10201":
			m_count = 1.0;
			m_hp = 90;
			break;
		case "10202":
			m_count = 1.5;
			m_hp = 125;
			break;
		case "10203":
			m_count = 2.0;
			m_hp = 160;
			break;
		case "10204":
			m_count = 2.5;
			m_hp = 195;
			break;
		case "10205":
			m_count = 3.0;
			m_hp = 230;
			break;
		case "10206":
			m_count = 3.5;
			m_hp = 275;
			break;
		case "10207":
			m_count = 4.0;
			m_hp = 320;
			break;
		case "10208":
			m_count = 4.5;
			m_hp = 370;
			break;
		case "10209":
			m_count = 5.0;
			m_hp = 420;
			break;
		case "10210":
			m_count = 5.5;
			m_hp = 470;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseBuffHPTeamEquipEffect) is HorseBuffHPTeamEquipEffect horseBuffHPTeamEquipEffect)
		{
			horseBuffHPTeamEquipEffect.m_probability = ((m_probability > horseBuffHPTeamEquipEffect.m_probability) ? m_probability : horseBuffHPTeamEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillHorse += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillHorse -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living)
	{
		if (rand.Next(10000) >= m_probability || living.PetEffects.CurrentHorseSkill != m_currentId || living.ForbidAidBlood)
		{
			return;
		}
		(living as Player).AddDelay(m_delay);
		int num = 0;
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			if (item.IsLiving)
			{
				num = (int)((double)(item.Blood / 100) * m_count + (double)m_hp);
				item.AddBlood(num);
			}
		}
	}
}

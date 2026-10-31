using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseBuffHPEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	private int m_hp = 0;

	public HorseBuffHPEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseBuffHPEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "10101":
			m_count = 3;
			m_hp = 150;
			break;
		case "10102":
			m_count = 4;
			m_hp = 190;
			break;
		case "10103":
			m_count = 5;
			m_hp = 230;
			break;
		case "10104":
			m_count = 6;
			m_hp = 275;
			break;
		case "10105":
			m_count = 7;
			m_hp = 325;
			break;
		case "10106":
			m_count = 8;
			m_hp = 380;
			break;
		case "10107":
			m_count = 9;
			m_hp = 440;
			break;
		case "10108":
			m_count = 10;
			m_hp = 515;
			break;
		case "10109":
			m_count = 11;
			m_hp = 580;
			break;
		case "10110":
			m_count = 12;
			m_hp = 660;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseBuffHPEquipEffect) is HorseBuffHPEquipEffect horseBuffHPEquipEffect)
		{
			horseBuffHPEquipEffect.m_probability = ((m_probability > horseBuffHPEquipEffect.m_probability) ? m_probability : horseBuffHPEquipEffect.m_probability);
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

	private void player_AfterKilledByLiving(Player living)
	{
		if (rand.Next(10000) < m_probability && living.PetEffects.CurrentHorseSkill == m_currentId && !living.ForbidAidBlood)
		{
			if (living.IsLiving)
			{
				living.AddDelay(m_delay);
				int value = living.MaxBlood / 100 * m_count + m_hp;
				living.AddBlood(value);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				int value = living.Game.CurrentLiving.MaxBlood / 100 * m_count + m_hp;
				living.Game.CurrentLiving.AddBlood(value);
			}
		}
	}
}

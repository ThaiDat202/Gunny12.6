using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseReduceDamageEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	private int m_reducePlus = 0;

	private int m_reduceDamage = 0;

	public HorseReduceDamageEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseReduceDamageEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "11301":
			m_reducePlus = 3;
			m_reduceDamage = 65;
			break;
		case "11302":
			m_reducePlus = 5;
			m_reduceDamage = 90;
			break;
		case "11303":
			m_reducePlus = 7;
			m_reduceDamage = 125;
			break;
		case "11304":
			m_reducePlus = 9;
			m_reduceDamage = 165;
			break;
		case "11305":
			m_reducePlus = 12;
			m_reduceDamage = 225;
			break;
		case "11306":
			m_reducePlus = 15;
			m_reduceDamage = 295;
			break;
		case "11307":
			m_reducePlus = 18;
			m_reduceDamage = 395;
			break;
		case "11308":
			m_reducePlus = 21;
			m_reduceDamage = 405;
			break;
		case "11309":
			m_reducePlus = 24;
			m_reduceDamage = 515;
			break;
		case "11310":
			m_reducePlus = 28;
			m_reduceDamage = 625;
			break;
		}
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseReduceDamageEquipEffect) is HorseReduceDamageEquipEffect horseReduceDamageEquipEffect)
		{
			horseReduceDamageEquipEffect.m_probability = ((m_probability > horseReduceDamageEquipEffect.m_probability) ? m_probability : horseReduceDamageEquipEffect.m_probability);
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
		if (rand.Next(10000) < m_probability && living.PetEffects.CurrentHorseSkill == m_currentId)
		{
			if (living.IsLiving)
			{
				new HorseReduceDamageEffect(m_count, m_reducePlus, m_reduceDamage, m_currentId, base.Info.ID.ToString()).Start(living);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				living.Game.CurrentLiving.AddHorseEffect(new HorseReduceDamageEffect(m_count, m_reducePlus, m_reduceDamage, m_currentId, base.Info.ID.ToString()), 0);
			}
		}
	}
}

using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseRemoveGuardEnemyEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseRemoveGuardEnemyEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseRemoveGuardEnemyEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "10301":
			m_count = 7;
			break;
		case "10302":
			m_count = 10;
			break;
		case "10303":
			m_count = 12;
			break;
		case "10304":
			m_count = 15;
			break;
		case "10305":
			m_count = 17;
			break;
		case "10306":
			m_count = 20;
			break;
		case "10307":
			m_count = 22;
			break;
		case "10308":
			m_count = 25;
			break;
		case "10309":
			m_count = 27;
			break;
		case "10310":
			m_count = 30;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseRemoveGuardEnemyEquipEffect) is HorseRemoveGuardEnemyEquipEffect horseRemoveGuardEnemyEquipEffect)
		{
			horseRemoveGuardEnemyEquipEffect.m_probability = ((m_probability > horseRemoveGuardEnemyEquipEffect.m_probability) ? m_probability : horseRemoveGuardEnemyEquipEffect.m_probability);
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
		if (rand.Next(10000) < m_probability && living.PetEffects.CurrentHorseSkill == m_currentId)
		{
			if (living.IsLiving)
			{
				living.AddDelay(m_delay);
				living.IgnoreGuard = m_count;
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				living.Game.CurrentLiving.IgnoreGuard = m_count;
			}
		}
	}
}

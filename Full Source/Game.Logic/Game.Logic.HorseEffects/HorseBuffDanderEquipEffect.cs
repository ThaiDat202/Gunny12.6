using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseBuffDanderEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseBuffDanderEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseBuffDanderEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "10401":
			m_count = 50;
			break;
		case "10402":
			m_count = 60;
			break;
		case "10403":
			m_count = 70;
			break;
		case "10404":
			m_count = 80;
			break;
		case "10405":
			m_count = 120;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseBuffDanderEquipEffect) is HorseBuffDanderEquipEffect horseBuffDanderEquipEffect)
		{
			horseBuffDanderEquipEffect.m_probability = ((m_probability > horseBuffDanderEquipEffect.m_probability) ? m_probability : horseBuffDanderEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillHorse += player_AfterBuffSkill;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillHorse -= player_AfterBuffSkill;
	}

	private void player_AfterBuffSkill(Living living)
	{
		if (rand.Next(10000) < m_probability && living.PetEffects.CurrentHorseSkill == m_currentId && living is Player)
		{
			if (living.IsLiving)
			{
				(living as Player).AddDander(m_count);
				(living as Player).AddDelay(m_delay);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				(living.Game.CurrentLiving as Player).AddDander(m_count);
			}
		}
	}
}

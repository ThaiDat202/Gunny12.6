using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseAddDamageNextDanderEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseAddDamageNextDanderEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseAddDamageNextDanderEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "11401":
			m_count = 2;
			break;
		case "11402":
			m_count = 4;
			break;
		case "11403":
			m_count = 6;
			break;
		case "11404":
			m_count = 8;
			break;
		case "11405":
			m_count = 10;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseAddDamageNextDanderEquipEffect) is HorseAddDamageNextDanderEquipEffect horseAddDamageNextDanderEquipEffect)
		{
			horseAddDamageNextDanderEquipEffect.m_probability = ((m_probability > horseAddDamageNextDanderEquipEffect.m_probability) ? m_probability : horseAddDamageNextDanderEquipEffect.m_probability);
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
			(living as Player).AddDelay(m_delay);
			if (living.IsLiving)
			{
				new HorseAddDamageNextDanderEffect(m_count, m_currentId, base.Info.ID.ToString()).Start(living);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				living.Game.CurrentLiving.AddHorseEffect(new HorseAddDamageNextDanderEffect(m_count, m_currentId, base.Info.ID.ToString()), 0);
			}
		}
	}
}

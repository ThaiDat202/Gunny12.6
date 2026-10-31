using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseAddDamageEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	private int m_attackadd = 0;

	private int m_plusattack = 0;

	public HorseAddDamageEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseAddDamageEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "11101":
			m_attackadd = 100;
			m_plusattack = 4;
			break;
		case "11102":
			m_attackadd = 140;
			m_plusattack = 6;
			break;
		case "11103":
			m_attackadd = 190;
			m_plusattack = 8;
			break;
		case "11104":
			m_attackadd = 245;
			m_plusattack = 10;
			break;
		case "11105":
			m_attackadd = 300;
			m_plusattack = 13;
			break;
		case "11106":
			m_attackadd = 360;
			m_plusattack = 16;
			break;
		case "11107":
			m_attackadd = 435;
			m_plusattack = 19;
			break;
		case "11108":
			m_attackadd = 525;
			m_plusattack = 22;
			break;
		case "11109":
			m_attackadd = 625;
			m_plusattack = 25;
			break;
		case "11110":
			m_attackadd = 730;
			m_plusattack = 30;
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
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseAddDamageEquipEffect) is HorseAddDamageEquipEffect horseAddDamageEquipEffect)
		{
			horseAddDamageEquipEffect.m_probability = ((m_probability > horseAddDamageEquipEffect.m_probability) ? m_probability : horseAddDamageEquipEffect.m_probability);
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
				new HorseAddDamageEffect(m_count, m_attackadd, m_plusattack, m_currentId, base.Info.ID.ToString()).Start(living);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				living.Game.CurrentLiving.AddHorseEffect(new HorseAddDamageEffect(m_count, m_attackadd, m_plusattack, m_currentId, base.Info.ID.ToString()), 0);
			}
		}
	}
}

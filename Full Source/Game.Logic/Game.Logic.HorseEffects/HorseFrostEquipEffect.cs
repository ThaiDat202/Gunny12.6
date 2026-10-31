using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseFrostEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseFrostEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseFrostEquipEffect, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseFrostEquipEffect) is HorseFrostEquipEffect horseFrostEquipEffect)
		{
			horseFrostEquipEffect.m_probability = ((m_probability > horseFrostEquipEffect.m_probability) ? m_probability : horseFrostEquipEffect.m_probability);
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
				(living as Player).SetBall(1);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				(living.Game.CurrentLiving as Player).SetBall(1);
			}
		}
	}
}

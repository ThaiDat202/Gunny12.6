using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseNoHoleEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseNoHoleEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseNoHoleEquipEffect, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseNoHoleEquipEffect) is HorseNoHoleEquipEffect horseNoHoleEquipEffect)
		{
			horseNoHoleEquipEffect.m_probability = ((m_probability > horseNoHoleEquipEffect.m_probability) ? m_probability : horseNoHoleEquipEffect.m_probability);
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
				living.IsNoHole = true;
				living.Game.SendPlayerPicture(living, 5, state: true);
				new NoHole_Active(1).Start(living);
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				living.Game.CurrentLiving.IsNoHole = true;
				living.Game.CurrentLiving.Game.SendPlayerPicture(living.Game.CurrentLiving, 5, state: true);
				new NoHole_Active(1).Start(living.Game.CurrentLiving);
			}
		}
	}
}

using System.Collections.Generic;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseRemoveRandomEffectEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseRemoveRandomEffectEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseRemoveRandomEffectEquipEffect, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseRemoveRandomEffectEquipEffect) is HorseRemoveRandomEffectEquipEffect horseRemoveRandomEffectEquipEffect)
		{
			horseRemoveRandomEffectEquipEffect.m_probability = ((m_probability > horseRemoveRandomEffectEquipEffect.m_probability) ? m_probability : horseRemoveRandomEffectEquipEffect.m_probability);
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
		int[] array = new int[5] { 0, 1, 2, 3, 4 };
		List<Player> allPlayersSameTeam = living.Game.GetAllPlayersSameTeam(living);
		Player player = null;
		if (allPlayersSameTeam.Count > 1)
		{
			int index = rand.Next(allPlayersSameTeam.Count);
			player = allPlayersSameTeam[index];
		}
		else if (allPlayersSameTeam.Count == 1)
		{
			player = allPlayersSameTeam[0];
		}
		if (player != null)
		{
			switch (array[rand.Next(array.Length)])
			{
			case 0:
				player.EffectList.StopEffect(typeof(IceFronzeEffect));
				break;
			case 1:
				player.EffectList.StopEffect(typeof(HideEffect));
				break;
			case 2:
				player.EffectList.StopEffect(typeof(NoHole_Active));
				break;
			case 3:
				player.EffectList.StopEffect(typeof(SealEffect));
				break;
			case 4:
				player.EffectList.StopEffect(typeof(ContinueReduceBloodEffect));
				break;
			}
		}
	}
}

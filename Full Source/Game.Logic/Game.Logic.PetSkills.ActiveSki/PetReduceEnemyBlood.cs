using System;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceEnemyBlood : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_skillId;

	private int distanceRequired = -1;

	public PetReduceEnemyBlood(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceEnemyBlood, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_skillId = skillId;
		switch (elementID)
		{
		case "1717":
		case "1718":
		case "1719":
		case "1801":
		case "1802":
		case "1803":
			distanceRequired = 300;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceEnemyBlood) is PetReduceEnemyBlood petReduceEnemyBlood)
		{
			petReduceEnemyBlood.m_probability = ((m_probability > petReduceEnemyBlood.m_probability) ? m_probability : petReduceEnemyBlood.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	private void player_PlayerBuffSkillPet(Living living)
	{
		if (living.PetEffects.CurrentUseSkill != m_skillId)
		{
			return;
		}
		foreach (Player allEnemyPlayer in living.Game.GetAllEnemyPlayers(living))
		{
			if (distanceRequired == -1 || Math.Abs(living.X - allEnemyPlayer.X) <= distanceRequired)
			{
				for (int i = 0; i < m_count; i++)
				{
					allEnemyPlayer.Game.sendShowPicSkil(allEnemyPlayer, base.Info, isActive: true);
				}
				allEnemyPlayer.AddPetEffect(new PetReduceBloodEquip(m_count, base.ElementInfo.ID.ToString(), living), 0);
			}
		}
	}
}

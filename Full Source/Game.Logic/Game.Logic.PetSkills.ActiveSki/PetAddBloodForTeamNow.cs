using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddBloodForTeamNow : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	private int m_value;

	public PetAddBloodForTeamNow(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddBloodForTeamNow, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1364"))
		{
			if (elementID == "1365")
			{
				m_value = 10;
			}
		}
		else
		{
			m_value = 8;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodForTeamNow) is PetAddBloodForTeamNow petAddBloodForTeamNow)
		{
			petAddBloodForTeamNow.m_probability = ((m_probability > petAddBloodForTeamNow.m_probability) ? m_probability : petAddBloodForTeamNow.m_probability);
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

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_currentId)
		{
			return;
		}
		List<Living> list = player.Game.Map.FindAllNearestSameTeam(player.X, player.Y, 250.0, player);
		foreach (Living item in list)
		{
			item.SyncAtTime = true;
			item.AddBlood(item.MaxBlood * m_value / 100);
			item.SyncAtTime = false;
		}
	}
}

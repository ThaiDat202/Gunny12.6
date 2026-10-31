using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddAgilityForTeam : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetAddAgilityForTeam(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddAgilityForTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAgilityForTeam) is PetAddAgilityForTeam petAddAgilityForTeam)
		{
			petAddAgilityForTeam.m_probability = ((m_probability > petAddAgilityForTeam.m_probability) ? m_probability : petAddAgilityForTeam.m_probability);
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
		if (living.PetEffects.CurrentUseSkill != m_currentId)
		{
			return;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			if (item != living)
			{
				item.Game.sendShowPicSkil(item, base.Info, isActive: true);
				item.AddPetEffect(new PetAddAgilityEquip(m_count, base.ElementInfo.ID.ToString()), 0);
			}
		}
	}
}

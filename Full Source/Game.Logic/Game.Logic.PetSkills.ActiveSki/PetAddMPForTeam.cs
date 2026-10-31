using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddMPForTeam : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	public PetAddMPForTeam(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddMPForTeam, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1268"))
		{
			if (elementID == "1269")
			{
				m_value = 10;
			}
		}
		else
		{
			m_value = 5;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPForTeam) is PetAddMPForTeam petAddMPForTeam)
		{
			petAddMPForTeam.m_probability = ((m_probability > petAddMPForTeam.m_probability) ? m_probability : petAddMPForTeam.m_probability);
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
		if (player.PetEffects.CurrentUseSkill != m_currentId || !(player.Game is PVPGame))
		{
			return;
		}
		List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
		foreach (Player item in allTeamPlayers)
		{
			item.AddPetMP(m_value);
		}
	}
}

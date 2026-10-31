using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceTakeDamageForTeam : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count = 0;

	public PetReduceTakeDamageForTeam(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceDamageForTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceDamageForTeam) is PetReduceTakeDamageForTeam petReduceTakeDamageForTeam)
		{
			petReduceTakeDamageForTeam.m_probability = ((m_probability > petReduceTakeDamageForTeam.m_probability) ? m_probability : petReduceTakeDamageForTeam.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_AfterBuffSkillPetByLiving;
		player.PlayerAnyShellThrow += player_PlayerAnyShellThrow;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_AfterBuffSkillPetByLiving;
		player.PlayerAnyShellThrow -= player_PlayerAnyShellThrow;
	}

	private void player_AfterBuffSkillPetByLiving(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			IsTrigger = true;
		}
	}

	private void player_PlayerAnyShellThrow(Player player)
	{
		if (!IsTrigger)
		{
			return;
		}
		List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
		foreach (Player item in allTeamPlayers)
		{
			if (item.PlayerDetail != player.PlayerDetail)
			{
				item.Game.sendShowPicSkil(item, base.ElementInfo, isActive: true);
				item.AddPetEffect(new PetReduceTakeDamageEquip(m_count, base.ElementInfo.ID.ToString()), 0);
			}
		}
		IsTrigger = false;
	}
}

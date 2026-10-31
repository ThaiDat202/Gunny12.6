using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceTakeDamageForRandPlayer : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count = 0;

	public PetReduceTakeDamageForRandPlayer(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceDamageForTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceDamageForTeam) is PetReduceTakeDamageForRandPlayer petReduceTakeDamageForRandPlayer)
		{
			petReduceTakeDamageForRandPlayer.m_probability = ((m_probability > petReduceTakeDamageForRandPlayer.m_probability) ? m_probability : petReduceTakeDamageForRandPlayer.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_AfterBuffSkillPetByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_AfterBuffSkillPetByLiving;
	}

	private void player_AfterBuffSkillPetByLiving(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
			int index = rand.Next(0, allTeamPlayers.Count);
			allTeamPlayers[index].Game.sendShowPicSkil(allTeamPlayers[index], base.ElementInfo, isActive: true);
			allTeamPlayers[index].AddPetEffect(new PetReduceTakeDamageEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

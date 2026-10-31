using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddBloodForTeam : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	private bool m_flag;

	public PetAddBloodForTeam(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddBloodForTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1392"))
		{
			if (elementID == "1393")
			{
				m_flag = true;
			}
		}
		else
		{
			m_flag = true;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodForTeam) is PetAddBloodForTeam petAddBloodForTeam)
		{
			petAddBloodForTeam.m_probability = ((m_probability > petAddBloodForTeam.m_probability) ? m_probability : petAddBloodForTeam.m_probability);
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
		if (m_flag)
		{
			List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
			{
				foreach (Player item in allTeamPlayers)
				{
					item.Game.sendShowPicSkil(item, base.ElementInfo, isActive: true);
					item.AddPetEffect(new PetAddBloodEquip(m_count, base.ElementInfo.ID.ToString()), 0);
				}
				return;
			}
		}
		List<Living> list = player.Game.Map.FindAllNearestSameTeam(player.X, player.Y, 250.0, player);
		foreach (Living item2 in list)
		{
			item2.Game.sendShowPicSkil(item2, base.ElementInfo, isActive: true);
			item2.AddPetEffect(new PetAddBloodEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

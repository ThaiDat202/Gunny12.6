using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddTeamMPAfterUsePetMP_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetAddTeamMPAfterUsePetMP_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddTeamMPAfterUsePetMP_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddTeamMPAfterUsePetMP_Passive) is PetAddTeamMPAfterUsePetMP_Passive petAddTeamMPAfterUsePetMP_Passive)
		{
			petAddTeamMPAfterUsePetMP_Passive.m_probability = ((m_probability > petAddTeamMPAfterUsePetMP_Passive.m_probability) ? m_probability : petAddTeamMPAfterUsePetMP_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerUsePetMP += player_PlayerUsePetMP;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerUsePetMP -= player_PlayerUsePetMP;
	}

	private void player_PlayerUsePetMP(Player player, int value)
	{
		List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
		foreach (Player item in allTeamPlayers)
		{
			item.AddPetMP(value / 2);
		}
	}
}

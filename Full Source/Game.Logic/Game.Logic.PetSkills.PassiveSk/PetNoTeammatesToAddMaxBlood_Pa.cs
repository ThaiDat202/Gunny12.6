using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetNoTeammatesToAddMaxBlood_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetNoTeammatesToAddMaxBlood_Passive(int probability, string elementID)
		: base(ePetEffectType.PetNoTeammatesToAddMaxBlood_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetNoTeammatesToAddMaxBlood_Passive) is PetNoTeammatesToAddMaxBlood_Passive petNoTeammatesToAddMaxBlood_Passive)
		{
			petNoTeammatesToAddMaxBlood_Passive.m_probability = ((m_probability > petNoTeammatesToAddMaxBlood_Passive.m_probability) ? m_probability : petNoTeammatesToAddMaxBlood_Passive.m_probability);
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
		if (IsTrigger)
		{
			List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
			if (allTeamPlayers.Count == 1)
			{
				player.MaxBlood += player.MaxBlood * 30 / 100;
			}
			IsTrigger = false;
			player.Game.UpdateMaxBlood(player);
		}
	}
}

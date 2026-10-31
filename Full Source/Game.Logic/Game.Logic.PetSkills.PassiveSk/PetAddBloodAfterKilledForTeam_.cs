using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBloodAfterKilledForTeam_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetAddBloodAfterKilledForTeam_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBloodAfterKilledForTeam_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAfterKilledForTeam_Passive) is PetAddBloodAfterKilledForTeam_Passive petAddBloodAfterKilledForTeam_Passive)
		{
			petAddBloodAfterKilledForTeam_Passive.m_probability = ((m_probability > petAddBloodAfterKilledForTeam_Passive.m_probability) ? m_probability : petAddBloodAfterKilledForTeam_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_BeginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginNextTurn -= player_BeginNextTurn;
	}

	private void player_BeginNextTurn(Living living)
	{
		if (IsTrigger)
		{
			return;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.AddPetEffect(new PetAddBloodAfterKilledForTeamEquip(m_probability, base.ElementInfo.ID.ToString()), 0);
		}
		IsTrigger = true;
	}
}

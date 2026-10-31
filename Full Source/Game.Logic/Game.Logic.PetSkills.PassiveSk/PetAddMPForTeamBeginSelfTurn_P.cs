using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPForTeamBeginSelfTurn_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetAddMPForTeamBeginSelfTurn_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMPForTeamBeginSelfTurn_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPForTeamBeginSelfTurn_Passive) is PetAddMPForTeamBeginSelfTurn_Passive petAddMPForTeamBeginSelfTurn_Passive)
		{
			petAddMPForTeamBeginSelfTurn_Passive.m_probability = ((m_probability > petAddMPForTeamBeginSelfTurn_Passive.m_probability) ? m_probability : petAddMPForTeamBeginSelfTurn_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_beginSeftTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_beginSeftTurn;
	}

	public void player_beginSeftTurn(Living living)
	{
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.AddPetMP(2);
		}
	}
}

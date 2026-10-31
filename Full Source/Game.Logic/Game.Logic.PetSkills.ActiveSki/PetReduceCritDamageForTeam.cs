using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceCritDamageForTeam : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	public PetReduceCritDamageForTeam(int probability, string elementID)
		: base(ePetEffectType.PetReduceCritDamageForTeam, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		switch (elementID)
		{
		case "1278":
			m_value = 20;
			break;
		case "1279":
		case "1535":
			m_value = 30;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceCritDamageForTeam) is PetReduceCritDamageForTeam petReduceCritDamageForTeam)
		{
			petReduceCritDamageForTeam.m_probability = ((m_probability > petReduceCritDamageForTeam.m_probability) ? m_probability : petReduceCritDamageForTeam.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_beginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginNextTurn -= player_beginNextTurn;
	}

	public void player_beginNextTurn(Living living)
	{
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			if (item.PetEffects.ReduceCritValue < m_value)
			{
				item.PetEffects.ReduceCritValue = m_value;
			}
		}
	}
}

using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddAttackForTeam_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent = false;

	public PetAddAttackForTeam_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddAttackForTeam_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAttackForTeam_Passive) is PetAddAttackForTeam_Passive petAddAttackForTeam_Passive)
		{
			petAddAttackForTeam_Passive.m_probability = ((m_probability > petAddAttackForTeam_Passive.m_probability) ? m_probability : petAddAttackForTeam_Passive.m_probability);
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
		if (m_value != 0)
		{
			return;
		}
		switch (m_elementID)
		{
		case "1192":
			m_value = 100;
			break;
		case "1193":
			m_value = 300;
			break;
		case "1488":
			m_value = 600;
			break;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.Game.sendShowPicSkil(item, base.Info, isActive: true);
			if (m_percent)
			{
				item.Attack += item.Attack * (double)m_value / 100.0;
			}
			else
			{
				item.Attack += m_value;
			}
		}
	}
}

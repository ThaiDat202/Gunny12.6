using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBaseDamageForTeam_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	public PetAddBaseDamageForTeam_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBaseDamageForTeam_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageForTeam_Passive) is PetAddBaseDamageForTeam_Passive petAddBaseDamageForTeam_Passive)
		{
			petAddBaseDamageForTeam_Passive.m_probability = ((m_probability > petAddBaseDamageForTeam_Passive.m_probability) ? m_probability : petAddBaseDamageForTeam_Passive.m_probability);
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
		string elementID = m_elementID;
		string text = elementID;
		if (!(text == "1264"))
		{
			if (text == "1265")
			{
				m_value = 300;
			}
		}
		else
		{
			m_value = 100;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.Game.sendShowPicSkil(living, base.Info, isActive: true);
			item.BaseDamage += m_value;
		}
	}
}

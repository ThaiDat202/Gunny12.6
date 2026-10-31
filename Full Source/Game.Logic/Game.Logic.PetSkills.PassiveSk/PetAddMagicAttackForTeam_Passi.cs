using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMagicAttackForTeam_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetAddMagicAttackForTeam_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMagicAttackForTeam_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicAttackForTeam_Passive) is PetAddMagicAttackForTeam_Passive petAddMagicAttackForTeam_Passive)
		{
			petAddMagicAttackForTeam_Passive.m_probability = ((m_probability > petAddMagicAttackForTeam_Passive.m_probability) ? m_probability : petAddMagicAttackForTeam_Passive.m_probability);
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
		if (!(text == "1422"))
		{
			if (text == "1424")
			{
				m_value = 10;
				m_percent = true;
			}
		}
		else
		{
			m_value = 5;
			m_percent = true;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.Game.sendShowPicSkil(item, base.Info, isActive: true);
			if (m_percent)
			{
				item.MagicAttack += item.MagicAttack * (double)m_value / 100.0;
			}
			else
			{
				item.MagicAttack += m_value;
			}
		}
	}
}

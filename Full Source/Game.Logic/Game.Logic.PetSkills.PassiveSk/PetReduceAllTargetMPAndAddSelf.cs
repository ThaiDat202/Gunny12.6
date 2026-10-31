using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceAllTargetMPAndAddSelfMD_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private string m_elementID;

	public PetReduceAllTargetMPAndAddSelfMD_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReduceAllTargetMPAndAddSelfMD_Passive, elementID)
	{
		m_elementID = elementID;
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1306"))
		{
			if (elementID == "1307")
			{
				m_value = 2;
			}
		}
		else
		{
			m_value = 1;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAllTargetMPAndAddSelfMD_Passive) is PetReduceAllTargetMPAndAddSelfMD_Passive petReduceAllTargetMPAndAddSelfMD_Passive)
		{
			petReduceAllTargetMPAndAddSelfMD_Passive.m_probability = ((m_probability > petReduceAllTargetMPAndAddSelfMD_Passive.m_probability) ? m_probability : petReduceAllTargetMPAndAddSelfMD_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	public void player_BeginSelfTurn(Living living)
	{
		if (!(living.Game is PVPGame))
		{
			return;
		}
		List<Player> allEnemyPlayers = living.Game.GetAllEnemyPlayers(living);
		foreach (Player item in allEnemyPlayers)
		{
			(living as Player).AddPetMP(m_value);
			item.RemovePetMP(m_value);
		}
		if (!(m_elementID == "1540"))
		{
			return;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item2 in allTeamPlayers)
		{
			if (item2 != living)
			{
				item2.AddPetMP(1);
			}
		}
	}
}

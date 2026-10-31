using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceAllTargetMagicDefence_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetReduceAllTargetMagicDefence_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReduceAllTargetMagicDefence_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAllTargetMagicDefence_Passive) is PetReduceAllTargetMagicDefence_Passive petReduceAllTargetMagicDefence_Passive)
		{
			petReduceAllTargetMagicDefence_Passive.m_probability = ((m_probability > petReduceAllTargetMagicDefence_Passive.m_probability) ? m_probability : petReduceAllTargetMagicDefence_Passive.m_probability);
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
		if (!(text == "1423"))
		{
			if (text == "1425")
			{
				m_value = 15;
				m_percent = true;
			}
		}
		else
		{
			m_value = 10;
			m_percent = true;
		}
		List<Player> allEnemyPlayers = living.Game.GetAllEnemyPlayers(living);
		foreach (Player item in allEnemyPlayers)
		{
			item.Game.sendShowPicSkil(item, base.Info, isActive: true);
			if (m_percent)
			{
				item.MagicDefence -= item.MagicDefence * (double)m_value / 100.0;
			}
			else
			{
				item.MagicDefence -= m_value;
			}
		}
	}
}

using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBloodAfterKilledForTeamEquip : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private bool m_flag;

	public PetAddBloodAfterKilledForTeamEquip(int probability, string elementID)
		: base(ePetEffectType.PetAddBloodAfterKilledForTeamEquip, elementID)
	{
		m_probability = probability;
		if (!(elementID == "1368"))
		{
			if (elementID == "1369")
			{
				m_value = 4;
				m_flag = true;
			}
		}
		else
		{
			m_value = 3;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAfterKilledForTeamEquip) is PetAddBloodAfterKilledForTeamEquip petAddBloodAfterKilledForTeamEquip)
		{
			petAddBloodAfterKilledForTeamEquip.m_probability = ((m_probability > petAddBloodAfterKilledForTeamEquip.m_probability) ? m_probability : petAddBloodAfterKilledForTeamEquip.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		int num = living.MaxBlood * m_value / 10 / 100;
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.SyncAtTime = true;
			if (m_flag && item.Blood <= item.Blood * 20 / 100)
			{
				item.AddBlood(num * 2);
			}
			else
			{
				item.AddBlood(num);
			}
			item.SyncAtTime = false;
		}
	}
}

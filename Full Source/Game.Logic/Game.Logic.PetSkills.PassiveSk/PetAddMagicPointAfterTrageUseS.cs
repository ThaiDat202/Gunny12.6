using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMagicPointAfterTrageUseSkills_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetAddMagicPointAfterTrageUseSkills_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMagicPointAfterTrageUseSkills_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicPointAfterTrageUseSkills_Passive) is PetAddMagicPointAfterTrageUseSkills_Passive petAddMagicPointAfterTrageUseSkills_Passive)
		{
			petAddMagicPointAfterTrageUseSkills_Passive.m_probability = ((m_probability > petAddMagicPointAfterTrageUseSkills_Passive.m_probability) ? m_probability : petAddMagicPointAfterTrageUseSkills_Passive.m_probability);
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
		if (IsTrigger)
		{
			return;
		}
		List<Player> allEnemyPlayers = living.Game.GetAllEnemyPlayers(living);
		foreach (Player item in allEnemyPlayers)
		{
			PetAddMagicPointAfterTrageUseSkillsEquip petAddMagicPointAfterTrageUseSkillsEquip = item.PetEffectList.GetOfType(ePetEffectType.PetAddMagicPointAfterTrageUseSkillsEquip) as PetAddMagicPointAfterTrageUseSkillsEquip;
			if (petAddMagicPointAfterTrageUseSkillsEquip == null)
			{
				item.AddPetEffect(new PetAddMagicPointAfterTrageUseSkillsEquip(m_probability, base.ElementInfo.ID.ToString()), 0);
			}
		}
		IsTrigger = true;
	}
}

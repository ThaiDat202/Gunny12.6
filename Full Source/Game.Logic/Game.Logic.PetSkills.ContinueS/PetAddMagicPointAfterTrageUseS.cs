using System.Collections.Generic;
using Game.Logic.PetSkills.PassiveSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddMagicPointAfterTrageUseSkillsEquip : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private int m_added;

	public PetAddMagicPointAfterTrageUseSkillsEquip(int probability, string elementID)
		: base(ePetEffectType.PetAddMagicPointAfterTrageUseSkillsEquip, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		switch (elementID)
		{
		case "1300":
			m_value = 30;
			break;
		case "1301":
			m_value = 50;
			break;
		case "1534":
			m_value = 65;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicPointAfterTrageUseSkillsEquip) is PetAddMagicPointAfterTrageUseSkillsEquip petAddMagicPointAfterTrageUseSkillsEquip)
		{
			petAddMagicPointAfterTrageUseSkillsEquip.m_probability = ((m_probability > petAddMagicPointAfterTrageUseSkillsEquip.m_probability) ? m_probability : petAddMagicPointAfterTrageUseSkillsEquip.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_AfterBuffSkillPetByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_AfterBuffSkillPetByLiving;
	}

	private void player_AfterBuffSkillPetByLiving(Player player)
	{
		List<Player> allEnemyPlayers = player.Game.GetAllEnemyPlayers(player);
		foreach (Player item in allEnemyPlayers)
		{
			if (item.PetEffectList.GetOfType(ePetEffectType.PetAddMagicPointAfterTrageUseSkills_Passive) is PetAddMagicPointAfterTrageUseSkills_Passive && player.PetMP > 0)
			{
				m_added = player.PetMP * m_value / 100;
				item.AddPetMP((m_added == 0) ? 1 : m_added);
			}
		}
	}
}

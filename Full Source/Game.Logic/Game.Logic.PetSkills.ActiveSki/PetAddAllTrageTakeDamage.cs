using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddAllTrageTakeDamage : BasePetEffect
{
	private int m_count;

	private int m_probability = 0;

	private int m_currentId;

	public PetAddAllTrageTakeDamage(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddAllTrageTakeDamage, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAllTrageTakeDamage) is PetAddAllTrageTakeDamage petAddAllTrageTakeDamage)
		{
			petAddAllTrageTakeDamage.m_probability = ((m_probability > petAddAllTrageTakeDamage.m_probability) ? m_probability : petAddAllTrageTakeDamage.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_currentId || !(player.Game is PVPGame))
		{
			return;
		}
		List<Player> allEnemyPlayers = player.Game.GetAllEnemyPlayers(player);
		foreach (Player item in allEnemyPlayers)
		{
			item.Game.sendShowPicSkil(item, base.Info, isActive: true);
			item.AddPetEffect(new PetAddTakeDamageEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

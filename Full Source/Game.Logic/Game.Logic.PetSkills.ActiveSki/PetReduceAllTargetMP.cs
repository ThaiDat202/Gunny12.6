using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceAllTargetMP : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value;

	public PetReduceAllTargetMP(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceAllTargetMP, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		switch (elementID)
		{
		case "1208":
		case "1427":
			m_value = 10;
			break;
		case "1508":
			m_value = 20;
			break;
		case "1209":
			m_value = 30;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAllTargetMP) is PetReduceAllTargetMP petReduceAllTargetMP)
		{
			petReduceAllTargetMP.m_probability = ((m_probability > petReduceAllTargetMP.m_probability) ? m_probability : petReduceAllTargetMP.m_probability);
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
		if (player.PetEffects.CurrentUseSkill != m_currentId || !(player.Game is PVPGame))
		{
			return;
		}
		List<Player> allEnemyPlayers = player.Game.GetAllEnemyPlayers(player);
		foreach (Player item in allEnemyPlayers)
		{
			item.RemovePetMP(m_value);
		}
	}
}

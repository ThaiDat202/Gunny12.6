using System.Collections.Generic;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceAllTargetBaseGuard : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count;

	public PetReduceAllTargetBaseGuard(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceAllTargetBaseGuard, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAllTargetBaseGuard) is PetReduceAllTargetBaseGuard petReduceAllTargetBaseGuard)
		{
			petReduceAllTargetBaseGuard.m_probability = ((m_probability > petReduceAllTargetBaseGuard.m_probability) ? m_probability : petReduceAllTargetBaseGuard.m_probability);
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
			item.Game.sendShowPicSkil(item, base.ElementInfo, isActive: true);
			item.AddPetEffect(new PetReduceBaseGuardEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

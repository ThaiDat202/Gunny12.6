using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBaseGuardBeforeMove : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetAddBaseGuardBeforeMove(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddBaseGuardBeforeMove, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseGuardBeforeMove) is PetAddBaseGuardBeforeMove petAddBaseGuardBeforeMove)
		{
			petAddBaseGuardBeforeMove.m_probability = ((m_probability > petAddBaseGuardBeforeMove.m_probability) ? m_probability : petAddBaseGuardBeforeMove.m_probability);
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
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
			player.AddPetEffect(new PetAddBaseGuardBeforeMoveEquip(3, m_probability, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

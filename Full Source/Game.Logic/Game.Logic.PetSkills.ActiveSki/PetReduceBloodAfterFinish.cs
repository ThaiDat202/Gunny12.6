using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceBloodAfterFinish : BasePetEffect
{
	private int m_count;

	private int m_skillId;

	private int m_probability;

	public PetReduceBloodAfterFinish(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceBloodAfterFinish, elementID)
	{
		m_count = count;
		m_skillId = skillId;
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	private void player_PlayerBuffSkillPet(Living living_0)
	{
		if (living_0.PetEffects.CurrentUseSkill == m_skillId)
		{
			for (int i = 0; i < m_count; i++)
			{
				living_0.Game.sendShowPicSkil(living_0, base.ElementInfo, isActive: true);
			}
			living_0.AddPetEffect(new PetReduceBloodAfterFinishEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodAfterFinish) is PetReduceBloodAfterFinish petReduceBloodAfterFinish)
		{
			petReduceBloodAfterFinish.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}
}

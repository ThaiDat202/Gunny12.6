using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddMaxBlood : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetAddMaxBlood(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddMaxBlood, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMaxBlood) is PetAddMaxBlood petAddMaxBlood)
		{
			petAddMaxBlood.m_probability = ((m_probability > petAddMaxBlood.m_probability) ? m_probability : petAddMaxBlood.m_probability);
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

	private void player_PlayerBuffSkillPet(Living living)
	{
		if (living.PetEffects.CurrentUseSkill == m_currentId)
		{
			living.PetEffectTrigger = true;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			new PetAddMaxBloodEquip(m_count, base.Info.ID.ToString()).Start(living);
		}
	}
}

using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddBaseGuard : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetAddBaseGuard(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddBaseGuard, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseGuard) is PetAddBaseGuard petAddBaseGuard)
		{
			petAddBaseGuard.m_probability = ((m_probability > petAddBaseGuard.m_probability) ? m_probability : petAddBaseGuard.m_probability);
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
			new PetAddBaseGuardEquip(m_count, base.Info.ID.ToString()).Start(living);
		}
	}
}

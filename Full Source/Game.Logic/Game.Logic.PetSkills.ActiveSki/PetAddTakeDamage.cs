using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddTakeDamage : BasePetEffect
{
	private int m_count;

	private int m_probability = 0;

	private int m_currentId;

	public PetAddTakeDamage(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddTakeDamage, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddTakeDamage) is PetAddTakeDamage petAddTakeDamage)
		{
			petAddTakeDamage.m_probability = ((m_probability > petAddTakeDamage.m_probability) ? m_probability : petAddTakeDamage.m_probability);
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
			player.AddPetEffect(new PetAddTakeDamageEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

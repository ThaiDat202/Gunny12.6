using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetCritRate100 : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetCritRate100(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetCritRate100, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetCritRate100) is PetCritRate100 petCritRate)
		{
			petCritRate.m_probability = ((m_probability > petCritRate.m_probability) ? m_probability : petCritRate.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_AfterBuffSkillPetByLiving;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_AfterBuffSkillPetByLiving;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterBuffSkillPetByLiving(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.PetEffects.CritRate += 100;
		}
	}

	private void player_AfterPlayerShooted(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.PetEffects.CritRate -= 100;
		}
	}
}

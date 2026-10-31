using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetViolentPosture_AddDamageOnHide : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	private int m_count;

	public PetViolentPosture_AddDamageOnHide(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetViolentPosture_AddDamageOnHide, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetViolentPosture_AddDamageOnHide) is PetViolentPosture_AddDamageOnHide petViolentPosture_AddDamageOnHide)
		{
			petViolentPosture_AddDamageOnHide.m_probability = ((m_probability > petViolentPosture_AddDamageOnHide.m_probability) ? m_probability : petViolentPosture_AddDamageOnHide.m_probability);
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
		if (player.PetEffects.CurrentUseSkill == m_currentId && player.IsHide)
		{
			player.AddPetEffect(new PetAddBaseDamageEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}
}

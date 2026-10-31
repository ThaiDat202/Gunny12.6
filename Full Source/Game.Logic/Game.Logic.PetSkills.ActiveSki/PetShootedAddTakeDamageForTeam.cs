using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetShootedAddTakeDamageForTeam : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetShootedAddTakeDamageForTeam(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetShootedAddTakeDamageForTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShootedAddTakeDamageForTeam) is PetShootedAddTakeDamageForTeam petShootedAddTakeDamageForTeam)
		{
			petShootedAddTakeDamageForTeam.m_probability = ((m_probability > petShootedAddTakeDamageForTeam.m_probability) ? m_probability : petShootedAddTakeDamageForTeam.m_probability);
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
			new PetShootedAddTakeDamageForTeamEquip(m_count, base.Info.ID.ToString()).Start(living);
		}
	}
}

using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddBloodAfterKilled : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetAddBloodAfterKilled(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddBloodAfterKilled, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAfterKilled) is PetAddBloodAfterKilled petAddBloodAfterKilled)
		{
			petAddBloodAfterKilled.m_probability = ((m_probability > petAddBloodAfterKilled.m_probability) ? m_probability : petAddBloodAfterKilled.m_probability);
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
			player.AddPetEffect(new PetAddBloodAfterKilledEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}
}

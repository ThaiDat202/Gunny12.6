using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceBlood : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetReduceBlood(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceBlood, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBlood) is PetReduceBlood petReduceBlood)
		{
			petReduceBlood.m_probability = ((m_probability > petReduceBlood.m_probability) ? m_probability : petReduceBlood.m_probability);
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
			player.AddPetEffect(new PetReduceBloodEquip(m_count, base.Info.ID.ToString(), player), 0);
		}
	}
}

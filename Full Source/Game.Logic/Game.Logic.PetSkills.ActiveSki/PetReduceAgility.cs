using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceAgility : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetReduceAgility(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceAgility, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAgility) is PetReduceAgility petReduceAgility)
		{
			petReduceAgility.m_probability = ((m_probability > petReduceAgility.m_probability) ? m_probability : petReduceAgility.m_probability);
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
			player.AddPetEffect(new PetReduceAgilityEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}
}

using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetBlockUseTrident : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_skillId;

	public PetBlockUseTrident(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetBlockUseTrident, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_skillId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetBlockUseTrident) is PetBlockUseTrident petBlockUseTrident)
		{
			petBlockUseTrident.m_probability = ((m_probability > petBlockUseTrident.m_probability) ? m_probability : petBlockUseTrident.m_probability);
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
		if (living.PetEffects.CurrentUseSkill == m_skillId)
		{
			for (int i = 0; i < m_count; i++)
			{
				living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			}
			living.AddPetEffect(new PetBlockUseTridentEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

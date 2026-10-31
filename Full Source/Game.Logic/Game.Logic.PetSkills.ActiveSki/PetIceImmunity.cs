using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetIceImmunity : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_skillId;

	public PetIceImmunity(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetIceImmunity, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_skillId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetIceImmunity) is PetIceImmunity petIceImmunity)
		{
			petIceImmunity.m_probability = ((m_probability > petIceImmunity.m_probability) ? m_probability : petIceImmunity.m_probability);
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
			living.AddPetEffect(new PetIceImmunityEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

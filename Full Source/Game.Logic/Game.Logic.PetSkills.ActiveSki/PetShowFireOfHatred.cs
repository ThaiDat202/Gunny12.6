using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetShowFireOfHatred : BasePetEffect
{
	private int m_skillId;

	private int m_count;

	public PetShowFireOfHatred(int count, int skillId, string elementID)
		: base(ePetEffectType.PetShowFireOfHatred, elementID)
	{
		m_count = count;
		m_skillId = skillId;
	}

	private void player_PlayerBuffSkillPet(Living living)
	{
		if (living.PetEffects.CurrentUseSkill == m_skillId)
		{
			for (int i = 0; i < m_count; i++)
			{
				living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			}
			living.AddPetEffect(new PetShowFireOfHatredEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShowFireOfHatred) is PetShowFireOfHatred)
		{
			return true;
		}
		return base.Start(living);
	}
}

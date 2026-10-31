using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetChangeSuit : BasePetEffect
{
	private int m_count;

	private int m_skillId;

	private string suit;

	public PetChangeSuit(int count, int skillId, string elementID)
		: base(ePetEffectType.PetChangeSuit, elementID)
	{
		m_count = count;
		m_skillId = skillId;
		if (skillId == 576 || skillId == 600)
		{
			suit = "13831|suits628";
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetChangeSuit) is PetChangeSuit petChangeSuit)
		{
			petChangeSuit.m_count = m_count;
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
				living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			}
			living.AddPetEffect(new PetChangeSuitEquip(m_count, suit, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

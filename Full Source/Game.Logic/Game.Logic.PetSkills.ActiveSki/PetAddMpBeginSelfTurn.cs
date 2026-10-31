using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddMpBeginSelfTurn : BasePetEffect
{
	private int m_count;

	private int m_skillId;

	private int m_probability;

	public PetAddMpBeginSelfTurn(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddMpBeginSelfTurn, elementID)
	{
		m_count = count;
		m_skillId = skillId;
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMpBeginSelfTurn) is PetAddMpBeginSelfTurn petAddMpBeginSelfTurn)
		{
			petAddMpBeginSelfTurn.m_probability = ((m_probability > petAddMpBeginSelfTurn.m_probability) ? m_probability : petAddMpBeginSelfTurn.m_probability);
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
			living.AddPetEffect(new PetAddMpBeginSelfTurnEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

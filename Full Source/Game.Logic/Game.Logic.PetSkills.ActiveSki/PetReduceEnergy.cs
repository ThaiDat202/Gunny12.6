using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceEnergy : BasePetEffect
{
	private int m_count;

	private int m_skillId;

	private int m_probability;

	public PetReduceEnergy(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetReduceEnergy, elementID)
	{
		m_count = count;
		m_skillId = skillId;
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceEnergy) is PetReduceEnergy petReduceEnergy)
		{
			petReduceEnergy.m_probability = ((m_probability > petReduceEnergy.m_probability) ? m_probability : petReduceEnergy.m_probability);
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

	private void player_PlayerBuffSkillPet(Living living_0)
	{
		if (living_0.PetEffects.CurrentUseSkill == m_skillId)
		{
			for (int i = 0; i < m_count; i++)
			{
				living_0.Game.sendShowPicSkil(living_0, base.ElementInfo, isActive: true);
			}
			living_0.AddPetEffect(new PetReduceEnergyEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

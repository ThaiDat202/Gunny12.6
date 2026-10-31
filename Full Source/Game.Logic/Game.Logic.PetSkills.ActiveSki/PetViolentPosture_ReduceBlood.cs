using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetViolentPosture_ReduceBlood : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	private int m_value;

	public PetViolentPosture_ReduceBlood(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetViolentPosture_ReduceBlood, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		switch (elementID)
		{
		case "1572":
		case "1573":
		case "1608":
		case "1609":
			m_value = 80;
			break;
		case "1631":
			m_value = 40;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetViolentPosture_ReduceBlood) is PetViolentPosture_ReduceBlood petViolentPosture_ReduceBlood)
		{
			petViolentPosture_ReduceBlood.m_probability = ((m_probability > petViolentPosture_ReduceBlood.m_probability) ? m_probability : petViolentPosture_ReduceBlood.m_probability);
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
			player.SyncAtTime = true;
			player.AddBlood(-(player.Blood * m_value / 100), 1);
			player.SyncAtTime = false;
		}
	}
}

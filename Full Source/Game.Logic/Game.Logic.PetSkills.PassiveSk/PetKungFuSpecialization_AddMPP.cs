using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetKungFuSpecialization_AddMPPassive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private int m_count = 0;

	public PetKungFuSpecialization_AddMPPassive(int probability, string elementID)
		: base(ePetEffectType.PetKungFuSpecialization_AddMPPassive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		switch (elementID)
		{
		case "1575":
			m_value = 4;
			break;
		case "1611":
			m_value = 2;
			break;
		case "1624":
			m_value = 10;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKungFuSpecialization_AddMPPassive) is PetKungFuSpecialization_AddMPPassive petKungFuSpecialization_AddMPPassive)
		{
			petKungFuSpecialization_AddMPPassive.m_probability = ((m_probability > petKungFuSpecialization_AddMPPassive.m_probability) ? m_probability : petKungFuSpecialization_AddMPPassive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerSkip += player_PlayerSkip;
		player.BeginNextTurn += player_BeginNextTurn;
		player.PlayerAfterBuffSkillPet += player_PlayerAfterBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerSkip -= player_PlayerSkip;
		player.BeginNextTurn -= player_BeginNextTurn;
		player.PlayerAfterBuffSkillPet -= player_PlayerAfterBuffSkillPet;
	}

	private void player_BeginNextTurn(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetStealthPostureEquip) is PetStealthPostureEquip)
		{
			((Player)living).AddPetMP(m_value);
			if (m_count < 6 && m_value == 10)
			{
				living.MaxBlood += living.MaxBlood * 10 / 100;
				living.Game.UpdateMaxBlood(living);
				m_count++;
			}
		}
	}

	private void player_PlayerSkip(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_DamageEquip) is PetDefensivePosture_DamageEquip)
		{
			((Player)living).AddPetMP(m_value);
			if (m_count < 6 && m_value == 10)
			{
				living.MaxBlood += living.MaxBlood * 10 / 100;
				living.Game.UpdateMaxBlood(living);
				m_count++;
			}
		}
	}

	private void player_PlayerAfterBuffSkillPet(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKungFuSpecializationEquip) is PetKungFuSpecializationEquip)
		{
			((Player)living).AddPetMP(m_value);
			if (m_count < 6 && m_value == 10)
			{
				living.MaxBlood += living.MaxBlood * 10 / 100;
				living.Game.UpdateMaxBlood(living);
				m_count++;
			}
		}
	}
}

using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetKungFuSpecialization_AddBaseDamageByBloodPassive : BasePetEffect
{
	private int m_probability = 0;

	private string m_elementID;

	public PetKungFuSpecialization_AddBaseDamageByBloodPassive(int probability, string elementID)
		: base(ePetEffectType.PetKungFuSpecialization_AddBaseDamageByBloodPassive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKungFuSpecialization_AddBaseDamageByBloodPassive) is PetKungFuSpecialization_AddBaseDamageByBloodPassive petKungFuSpecialization_AddBaseDamageByBloodPassive)
		{
			petKungFuSpecialization_AddBaseDamageByBloodPassive.m_probability = ((m_probability > petKungFuSpecialization_AddBaseDamageByBloodPassive.m_probability) ? m_probability : petKungFuSpecialization_AddBaseDamageByBloodPassive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerSkip += player_PlayerSkip;
		player.PlayerAfterBuffSkillPet += player_PlayerAfterBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerSkip -= player_PlayerSkip;
		player.PlayerAfterBuffSkillPet -= player_PlayerAfterBuffSkillPet;
	}

	private void player_PlayerSkip(Living living)
	{
		PetDefensivePosture_DamageEquip petDefensivePosture_DamageEquip = living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_DamageEquip) as PetDefensivePosture_DamageEquip;
		PetAddBaseDamageByBloodEquip petAddBaseDamageByBloodEquip = living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageByBloodEquip) as PetAddBaseDamageByBloodEquip;
		if (petDefensivePosture_DamageEquip != null && petAddBaseDamageByBloodEquip == null)
		{
			new PetAddBaseDamageByBlood_NextTurn(1, base.ElementInfo.ID.ToString()).Start(living);
		}
	}

	private void player_PlayerAfterBuffSkillPet(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKungFuSpecializationEquip) is PetKungFuSpecializationEquip)
		{
			new PetAddBaseDamageByBloodEquip(1, base.ElementInfo.ID.ToString()).Start(living);
		}
	}
}

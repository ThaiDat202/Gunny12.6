using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddBloodWithFire : BasePetEffect
{
	private int m_skillId;

	public PetAddBloodWithFire(int skillId, string elementID)
		: base(ePetEffectType.PetAddBloodWithFire, elementID)
	{
		m_skillId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodWithFire) is PetAddBloodWithFire)
		{
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
		PetShowFireofResentmentEquip petShowFireofResentmentEquip = living.PetEffectList.GetOfType(ePetEffectType.PetShowFireofResentmentEquip) as PetShowFireofResentmentEquip;
		PetShowFireOfHatredEquip petShowFireOfHatredEquip = living.PetEffectList.GetOfType(ePetEffectType.PetShowFireOfHatredEquip) as PetShowFireOfHatredEquip;
		if ((petShowFireofResentmentEquip != null || petShowFireOfHatredEquip != null) && living.PetEffects.CurrentUseSkill == m_skillId)
		{
			living.SyncAtTime = true;
			living.AddBlood(m_living.MaxBlood * 30 / 100);
			living.SyncAtTime = false;
			petShowFireofResentmentEquip?.Stop();
			petShowFireOfHatredEquip?.Stop();
			Stop();
		}
	}
}

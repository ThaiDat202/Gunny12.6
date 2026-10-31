using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetUseMPToAddBlood : BasePetEffect
{
	private int m_probability;

	public PetUseMPToAddBlood(int probability, string elementID)
		: base(ePetEffectType.PetUseMPToAddBlood, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetUseMPToAddBlood) is PetUseMPToAddBlood petUseMPToAddBlood)
		{
			petUseMPToAddBlood.m_probability = ((m_probability > petUseMPToAddBlood.m_probability) ? m_probability : petUseMPToAddBlood.m_probability);
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
		int petMP = (living as Player).PetMP;
		(living as Player).RemovePetMP(petMP);
		int value = petMP / 3 * living.MaxBlood / 100;
		living.AddBlood(value);
	}
}

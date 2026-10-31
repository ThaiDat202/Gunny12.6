using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetKungFuSpecialization : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	public PetKungFuSpecialization(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetKungFuSpecialization, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKungFuSpecialization) is PetKungFuSpecialization petKungFuSpecialization)
		{
			petKungFuSpecialization.m_probability = ((m_probability > petKungFuSpecialization.m_probability) ? m_probability : petKungFuSpecialization.m_probability);
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
		int count = 1;
		switch (m_currentId)
		{
		case 447:
		case 448:
		case 455:
		case 468:
		case 469:
		case 476:
		case 498:
			count = 2;
			break;
		}
		new PetKungFuSpecializationEquip(count, base.ElementInfo.ID.ToString()).Start(player);
	}
}

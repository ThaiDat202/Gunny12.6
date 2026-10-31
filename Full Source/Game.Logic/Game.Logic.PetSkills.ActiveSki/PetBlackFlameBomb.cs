using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetBlackFlameBomb : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetBlackFlameBomb(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetBlackFlameBomb, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetBlackFlameBomb) is PetBlackFlameBomb petBlackFlameBomb)
		{
			petBlackFlameBomb.m_probability = ((m_probability > petBlackFlameBomb.m_probability) ? m_probability : petBlackFlameBomb.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerShoot;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerShoot;
	}

	private void player_PlayerShoot(Living living)
	{
		if (living.PetEffects.CurrentUseSkill == m_currentId)
		{
			living.AddPetEffect(new PetBlackFlameBombEquip(m_count, base.ElementInfo.ID.ToString()), 0);
		}
	}
}

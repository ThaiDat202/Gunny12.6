using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddCritRate : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_count = 0;

	public PetAddCritRate(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddCritRate, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_count = count;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddCritRate) is PetAddCritRate petAddCritRate)
		{
			petAddCritRate.m_probability = ((m_probability > petAddCritRate.m_probability) ? m_probability : petAddCritRate.m_probability);
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
			player.PetEffectTrigger = true;
			player.Game.sendShowPicSkil(player, base.Info, isActive: true);
			player.AddPetEffect(new PetAddCritRateEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}
}

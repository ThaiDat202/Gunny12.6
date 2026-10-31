using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetSuckBlood : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetSuckBlood(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetSuckBlood, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1310"))
		{
			if (elementID == "1311")
			{
				m_count = 2;
			}
		}
		else
		{
			m_count = 1;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetSuckBlood) is PetSuckBlood petSuckBlood)
		{
			petSuckBlood.m_probability = ((m_probability > petSuckBlood.m_probability) ? m_probability : petSuckBlood.m_probability);
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
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
			player.AddPetEffect(new PetSuckBloodEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}
}

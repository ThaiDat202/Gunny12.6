using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAlwayAddLuckyBeforeMove : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	private int m_added = 0;

	public PetAlwayAddLuckyBeforeMove(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAlwayAddLuckyBeforeMove, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1344"))
		{
			if (elementID == "1345")
			{
				m_value = 15;
			}
		}
		else
		{
			m_value = 10;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAlwayAddLuckyBeforeMove) is PetAlwayAddLuckyBeforeMove petAlwayAddLuckyBeforeMove)
		{
			petAlwayAddLuckyBeforeMove.m_probability = ((m_probability > petAlwayAddLuckyBeforeMove.m_probability) ? m_probability : petAlwayAddLuckyBeforeMove.m_probability);
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

	protected override void OnPausedOnPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.Lucky -= m_added;
		IsTrigger = false;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId && !IsTrigger)
		{
			IsTrigger = true;
			m_added = (int)(player.Lucky * (double)m_value / 100.0);
			player.Lucky += m_added;
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
		}
	}
}

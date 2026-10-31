using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAlwayAddBaseDamageBeforeMove : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	private int m_added = 0;

	private bool m_percent;

	public PetAlwayAddBaseDamageBeforeMove(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAlwayAddBaseDamageBeforeMove, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		switch (elementID)
		{
		case "1342":
			m_value = 30;
			m_percent = true;
			break;
		case "1343":
			m_value = 45;
			m_percent = true;
			break;
		case "1485":
			m_value = 8;
			m_percent = true;
			break;
		case "1184":
			m_value = 150;
			break;
		case "1185":
			m_value = 300;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAlwayAddBaseDamageBeforeMove) is PetAlwayAddBaseDamageBeforeMove petAlwayAddBaseDamageBeforeMove)
		{
			petAlwayAddBaseDamageBeforeMove.m_probability = ((m_probability > petAlwayAddBaseDamageBeforeMove.m_probability) ? m_probability : petAlwayAddBaseDamageBeforeMove.m_probability);
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
		player.BaseDamage -= m_added;
		IsTrigger = false;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId && !IsTrigger)
		{
			IsTrigger = true;
			if (m_percent)
			{
				m_added = (int)(player.BaseDamage * (double)m_value / 100.0);
			}
			else
			{
				m_added = m_value;
			}
			player.BaseDamage += m_added;
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
		}
	}
}

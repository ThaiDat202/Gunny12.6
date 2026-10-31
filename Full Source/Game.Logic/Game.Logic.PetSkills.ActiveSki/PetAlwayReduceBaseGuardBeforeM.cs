using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAlwayReduceBaseGuardBeforeMove : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	private int m_added = 0;

	public PetAlwayReduceBaseGuardBeforeMove(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAlwayReduceBaseGuardBeforeMove, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1346"))
		{
			if (elementID == "1347")
			{
				m_value = 25;
			}
		}
		else
		{
			m_value = 30;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAlwayReduceBaseGuardBeforeMove) is PetAlwayReduceBaseGuardBeforeMove petAlwayReduceBaseGuardBeforeMove)
		{
			petAlwayReduceBaseGuardBeforeMove.m_probability = ((m_probability > petAlwayReduceBaseGuardBeforeMove.m_probability) ? m_probability : petAlwayReduceBaseGuardBeforeMove.m_probability);
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
		player.BaseGuard += m_added;
		IsTrigger = false;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId && !IsTrigger)
		{
			IsTrigger = true;
			m_added = (int)(player.BaseGuard * (double)m_value / 100.0);
			player.BaseGuard -= m_added;
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
		}
	}
}

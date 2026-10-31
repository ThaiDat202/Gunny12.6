using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAlwayNoHoleBeforeMove : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetAlwayNoHoleBeforeMove(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAlwayNoHoleBeforeMove, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAlwayNoHoleBeforeMove) is PetAlwayNoHoleBeforeMove petAlwayNoHoleBeforeMove)
		{
			petAlwayNoHoleBeforeMove.m_probability = ((m_probability > petAlwayNoHoleBeforeMove.m_probability) ? m_probability : petAlwayNoHoleBeforeMove.m_probability);
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
			player.Game.SendPlayerPicture(player, 5, state: true);
			player.IsNoHole = true;
		}
	}
}

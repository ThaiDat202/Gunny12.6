using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetClearHellIce : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetClearHellIce(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetClearHellIce, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetClearHellIce) is PetClearHellIce petClearHellIce)
		{
			petClearHellIce.m_probability = ((m_probability > petClearHellIce.m_probability) ? m_probability : petClearHellIce.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBeginMoving += player_PlayerBeginMoving;
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBeginMoving -= player_PlayerBeginMoving;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			IsTrigger = true;
		}
	}

	private void player_PlayerBeginMoving(Player player)
	{
		if (IsTrigger)
		{
			if (player.PetEffectList.GetOfType(ePetEffectType.PetAlwayAddBaseDamageBeforeMove) is PetAlwayAddBaseDamageBeforeMove petAlwayAddBaseDamageBeforeMove)
			{
				petAlwayAddBaseDamageBeforeMove.Pause();
			}
			if (player.PetEffectList.GetOfType(ePetEffectType.PetAlwayReduceBloodBeforeMove) is PetAlwayReduceBloodBeforeMove petAlwayReduceBloodBeforeMove)
			{
				petAlwayReduceBloodBeforeMove.Pause();
			}
			IsTrigger = false;
		}
	}
}

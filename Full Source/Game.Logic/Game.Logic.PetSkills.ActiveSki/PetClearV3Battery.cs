using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetClearV3Battery : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetClearV3Battery(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetClearV3Battery, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetClearV3Battery) is PetClearV3Battery petClearV3Battery)
		{
			petClearV3Battery.m_probability = ((m_probability > petClearV3Battery.m_probability) ? m_probability : petClearV3Battery.m_probability);
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
			if (player.PetEffectList.GetOfType(ePetEffectType.PetAlwayAddLuckyBeforeMove) is PetAlwayAddLuckyBeforeMove petAlwayAddLuckyBeforeMove)
			{
				petAlwayAddLuckyBeforeMove.Pause();
			}
			if (player.PetEffectList.GetOfType(ePetEffectType.PetAlwayReduceBaseGuardBeforeMove) is PetAlwayReduceBaseGuardBeforeMove petAlwayReduceBaseGuardBeforeMove)
			{
				petAlwayReduceBaseGuardBeforeMove.Pause();
			}
			player.Game.SendPlayerPicture(player, 5, state: false);
			player.IsNoHole = false;
			IsTrigger = false;
		}
	}
}

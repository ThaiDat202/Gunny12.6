using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAttackPosture_ReduceDander : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	public PetAttackPosture_ReduceDander(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAttackPosture_ReduceDander, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAttackPosture_ReduceDander) is PetAttackPosture_ReduceDander petAttackPosture_ReduceDander)
		{
			petAttackPosture_ReduceDander.m_probability = ((m_probability > petAttackPosture_ReduceDander.m_probability) ? m_probability : petAttackPosture_ReduceDander.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
		player.BeginNextTurn += player_BeginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
		player.BeginNextTurn -= player_BeginNextTurn;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			IsTrigger = true;
			player.PetEffectTrigger = true;
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
			if (m_currentId != 499)
			{
				player.SetDander(player.Dander - 50);
			}
			else
			{
				player.AddDander(40);
			}
		}
	}

	private void player_BeginNextTurn(Living player)
	{
		if (IsTrigger)
		{
			IsTrigger = false;
			player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		}
	}
}

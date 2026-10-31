using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetViolentPosture_ReduceEnemyDefence : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	private int m_count;

	private int m_value;

	private bool m_percent;

	public PetViolentPosture_ReduceEnemyDefence(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetViolentPosture_ReduceEnemyDefence, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		m_count = count;
		switch (elementID)
		{
		case "1600":
		case "1497":
			m_value = 20;
			break;
		case "1564":
		case "1601":
			m_value = 25;
			break;
		case "1565":
			m_value = 40;
			break;
		case "1627":
			m_value = 45;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetViolentPosture_ReduceEnemyDefence) is PetViolentPosture_ReduceEnemyDefence petViolentPosture_ReduceEnemyDefence)
		{
			petViolentPosture_ReduceEnemyDefence.m_probability = ((m_probability > petViolentPosture_ReduceEnemyDefence.m_probability) ? m_probability : petViolentPosture_ReduceEnemyDefence.m_probability);
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

	private void player_BeginSelfTurn(Living living)
	{
		living.IgnoreGuard = 0;
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_currentId)
		{
			return;
		}
		if (player.Game is PVEGame)
		{
			player.IgnoreGuard += m_value;
			player.BeginSelfTurn += player_BeginSelfTurn;
			return;
		}
		foreach (Player allEnemyPlayer in player.Game.GetAllEnemyPlayers(player))
		{
			allEnemyPlayer.AddPetEffect(new PetReduceDefenceEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}
}

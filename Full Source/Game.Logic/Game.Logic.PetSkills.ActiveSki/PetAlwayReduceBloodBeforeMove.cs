using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAlwayReduceBloodBeforeMove : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	private int m_added = 0;

	private bool m_percent = false;

	public PetAlwayReduceBloodBeforeMove(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAlwayReduceBloodBeforeMove, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		switch (elementID)
		{
		case "1186":
			m_value = 150;
			break;
		case "1187":
			m_value = 300;
			break;
		case "1486":
			m_value = 800;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAlwayReduceBloodBeforeMove) is PetAlwayReduceBloodBeforeMove petAlwayReduceBloodBeforeMove)
		{
			petAlwayReduceBloodBeforeMove.m_probability = ((m_probability > petAlwayReduceBloodBeforeMove.m_probability) ? m_probability : petAlwayReduceBloodBeforeMove.m_probability);
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
		IsTrigger = false;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_currentId || IsTrigger)
		{
			return;
		}
		IsTrigger = true;
		if (m_percent)
		{
			m_added = player.MaxBlood * m_value / 100;
		}
		else
		{
			m_added = m_value;
		}
		player.SyncAtTime = true;
		player.AddBlood(-m_added, 1);
		player.SyncAtTime = false;
		if (player.Blood <= 0)
		{
			player.Die();
			if (player.Game.CurrentLiving != null && player.Game.CurrentLiving is Player)
			{
				(player.Game.CurrentLiving as Player).PlayerDetail.OnKillingLiving(player.Game, 2, player.Id, player.IsLiving, m_added);
			}
		}
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
	}
}

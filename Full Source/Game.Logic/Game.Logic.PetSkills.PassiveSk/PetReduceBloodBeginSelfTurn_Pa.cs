using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceBloodBeginSelfTurn_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_added = 0;

	public PetReduceBloodBeginSelfTurn_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReduceBloodBeginSelfTurn_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodBeginSelfTurn_Passive) is PetReduceBloodBeginSelfTurn_Passive petReduceBloodBeginSelfTurn_Passive)
		{
			petReduceBloodBeginSelfTurn_Passive.m_probability = ((m_probability > petReduceBloodBeginSelfTurn_Passive.m_probability) ? m_probability : petReduceBloodBeginSelfTurn_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_added = living.MaxBlood * 33 / 10 / 100;
		living.SyncAtTime = true;
		living.AddBlood(-m_added, 1);
		living.SyncAtTime = false;
		if (living.Blood <= 0)
		{
			living.Die();
			if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player)
			{
				(living.Game.CurrentLiving as Player).PlayerDetail.OnKillingLiving(living.Game, 2, living.Id, living.IsLiving, m_added);
			}
		}
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
	}
}

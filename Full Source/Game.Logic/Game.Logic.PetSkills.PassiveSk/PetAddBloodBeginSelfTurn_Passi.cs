using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBloodBeginSelfTurn_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetAddBloodBeginSelfTurn_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBloodBeginSelfTurn_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodBeginSelfTurn_Passive) is PetAddBloodBeginSelfTurn_Passive petAddBloodBeginSelfTurn_Passive)
		{
			petAddBloodBeginSelfTurn_Passive.m_probability = ((m_probability > petAddBloodBeginSelfTurn_Passive.m_probability) ? m_probability : petAddBloodBeginSelfTurn_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_beginSeftTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_beginSeftTurn;
	}

	public void player_beginSeftTurn(Living living)
	{
		living.SyncAtTime = true;
		living.AddBlood(living.MaxBlood * 2 / 100);
		living.SyncAtTime = false;
	}
}

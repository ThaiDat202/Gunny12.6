using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPBeginSelfTurn_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	public PetAddMPBeginSelfTurn_Passive(int value, int probability, string elementID)
		: base(ePetEffectType.PetAddMPBeginSelfTurn_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_value = value;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPBeginSelfTurn_Passive) is PetAddMPBeginSelfTurn_Passive petAddMPBeginSelfTurn_Passive)
		{
			petAddMPBeginSelfTurn_Passive.m_probability = ((m_probability > petAddMPBeginSelfTurn_Passive.m_probability) ? m_probability : petAddMPBeginSelfTurn_Passive.m_probability);
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
		(living as Player).AddPetMP(m_value);
	}
}

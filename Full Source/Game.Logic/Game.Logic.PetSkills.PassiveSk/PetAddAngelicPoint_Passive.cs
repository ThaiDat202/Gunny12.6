using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddAngelicPoint_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	public PetAddAngelicPoint_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddAngelicPoint_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAngelicPoint_Passive) is PetAddAngelicPoint_Passive petAddAngelicPoint_Passive)
		{
			petAddAngelicPoint_Passive.m_probability = ((m_probability > petAddAngelicPoint_Passive.m_probability) ? m_probability : petAddAngelicPoint_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_BeginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginNextTurn -= player_BeginNextTurn;
	}

	private void player_BeginNextTurn(Living living)
	{
		if (m_value == 0)
		{
			m_value = 300;
			living.PetEffects.IncreaseAngelicPoint = m_value;
		}
	}
}

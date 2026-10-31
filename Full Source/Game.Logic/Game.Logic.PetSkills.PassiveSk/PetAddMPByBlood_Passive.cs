using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPByBlood_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private int m_blood;

	public PetAddMPByBlood_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMPByBlood_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (elementID == "1322")
		{
			m_value = 1;
			m_blood = 3500;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPByBlood_Passive) is PetAddMPByBlood_Passive petAddMPByBlood_Passive)
		{
			petAddMPByBlood_Passive.m_probability = ((m_probability > petAddMPByBlood_Passive.m_probability) ? m_probability : petAddMPByBlood_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		int num = (damageAmount + criticalAmount) / m_blood;
		if (num > 0)
		{
			(living as Player).AddPetMP(num * m_value);
		}
	}
}

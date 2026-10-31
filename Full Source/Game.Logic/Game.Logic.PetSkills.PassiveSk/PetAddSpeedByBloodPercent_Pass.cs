using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddSpeedByBloodPercent_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	public PetAddSpeedByBloodPercent_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddSpeedByBloodPercent_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1649"))
		{
			if (elementID == "1650")
			{
				m_value = 20;
			}
		}
		else
		{
			m_value = 15;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddSpeedByBloodPercent_Passive) is PetAddSpeedByBloodPercent_Passive petAddSpeedByBloodPercent_Passive)
		{
			petAddSpeedByBloodPercent_Passive.m_probability = ((m_probability > petAddSpeedByBloodPercent_Passive.m_probability) ? m_probability : petAddSpeedByBloodPercent_Passive.m_probability);
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
		int num = (damageAmount + criticalAmount) / (living.MaxBlood * 5 / 100);
		while (num > 0)
		{
			num--;
			living.Speed += m_value;
		}
	}
}

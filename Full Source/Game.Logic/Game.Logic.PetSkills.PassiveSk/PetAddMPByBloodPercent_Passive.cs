using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPByBloodPercent_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private int m_blood = 0;

	private bool m_flag;

	public PetAddMPByBloodPercent_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMPByBloodPercent_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1662"))
		{
			if (elementID == "2893")
			{
				m_value = 3;
				m_blood = 5;
				m_flag = true;
			}
		}
		else
		{
			m_value = 2;
			m_blood = 10;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPByBloodPercent_Passive) is PetAddMPByBloodPercent_Passive petAddMPByBloodPercent_Passive)
		{
			petAddMPByBloodPercent_Passive.m_probability = ((m_probability > petAddMPByBloodPercent_Passive.m_probability) ? m_probability : petAddMPByBloodPercent_Passive.m_probability);
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
		int num = (damageAmount + criticalAmount) / (living.MaxBlood * m_blood / 100);
		if (num > 4 && m_flag)
		{
			num = 4;
		}
		(living as Player).AddPetMP(num * m_value);
	}
}

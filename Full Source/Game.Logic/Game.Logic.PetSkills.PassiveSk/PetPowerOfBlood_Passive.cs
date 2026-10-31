using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetPowerOfBlood_Passive : AbstractPetEffect
{
	private int m_probability;

	private int m_value;

	private bool m_percent;

	private int m_added;

	public PetPowerOfBlood_Passive(int probability, string elementID)
		: base(ePetEffectType.PetPowerOfBlood_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "2580"))
		{
			if (elementID == "2581")
			{
				m_value = 50;
				m_percent = true;
			}
		}
		else
		{
			m_value = 70;
			m_percent = true;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPowerOfBlood_Passive) is PetPowerOfBlood_Passive petPowerOfBlood_Passive)
		{
			petPowerOfBlood_Passive.m_probability = ((m_probability > petPowerOfBlood_Passive.m_probability) ? m_probability : petPowerOfBlood_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.BeginNextTurn += player_BeginNextTurn;
	}

	public override void OnRemoved(Living player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.BeginNextTurn -= player_BeginNextTurn;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (living.Game is PVPGame)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			if (m_percent)
			{
				m_added = damageAmount * m_value / 100;
				damageAmount -= m_added;
			}
			else
			{
				damageAmount -= m_value;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
	}

	private void player_BeginNextTurn(Living living)
	{
		living.SyncAtTime = true;
		living.AddBlood(-m_added, 1);
		living.SyncAtTime = false;
		m_added = 0;
	}
}

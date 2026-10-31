using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetSoulDefense_Passive : BasePetEffect
{
	private int m_probability;

	private int m_value;

	private int m_added;

	public PetSoulDefense_Passive(int probability, string elementID)
		: base(ePetEffectType.PetSoulDefense_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "2586"))
		{
			if (elementID == "2587")
			{
				m_value = 40;
			}
		}
		else
		{
			m_value = 20;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetSoulDefense_Passive) is PetSoulDefense_Passive petSoulDefense_Passive)
		{
			petSoulDefense_Passive.m_probability = ((m_probability > petSoulDefense_Passive.m_probability) ? m_probability : petSoulDefense_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.PlayerUsePetMP += player_PlayerUsePetMP;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.PlayerUsePetMP -= player_PlayerUsePetMP;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (living.Game is PVPGame)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			damageAmount -= m_added;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
	}

	private void player_PlayerUsePetMP(Player player, int value)
	{
		if (value > m_value)
		{
			value = m_value;
		}
		m_added = player.MaxBlood * 5 / 10 / 100 * value;
	}
}

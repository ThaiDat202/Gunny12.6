using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddAgility_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent = false;

	public PetAddAgility_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddAgility_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAgility_Passive) is PetAddAgility_Passive petAddAgility_Passive)
		{
			petAddAgility_Passive.m_probability = ((m_probability > petAddAgility_Passive.m_probability) ? m_probability : petAddAgility_Passive.m_probability);
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
			switch (m_elementID)
			{
			case "1190":
			case "3128":
				m_value = 300;
				break;
			case "1191":
				m_value = 500;
				break;
			case "1487":
				m_value = 600;
				break;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.Agility += living.Agility * (double)m_value / 100.0;
			}
			else
			{
				living.Agility += m_value;
			}
		}
	}
}

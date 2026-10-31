using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddDefence_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetAddDefence_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddDefence_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddDefence_Passive) is PetAddDefence_Passive petAddDefence_Passive)
		{
			petAddDefence_Passive.m_probability = ((m_probability > petAddDefence_Passive.m_probability) ? m_probability : petAddDefence_Passive.m_probability);
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
			case "1451":
			case "1530":
				m_value = 6;
				m_percent = true;
				break;
			case "1452":
			case "1531":
				m_value = 10;
				m_percent = true;
				break;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.Defence += living.Defence * (double)m_value / 100.0;
			}
			else
			{
				living.Defence += m_value;
			}
		}
	}
}

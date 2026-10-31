using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBaseGuard_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetAddBaseGuard_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBaseGuard_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseGuard_Passive) is PetAddBaseGuard_Passive petAddBaseGuard_Passive)
		{
			petAddBaseGuard_Passive.m_probability = ((m_probability > petAddBaseGuard_Passive.m_probability) ? m_probability : petAddBaseGuard_Passive.m_probability);
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
			case "1214":
				m_value = 100;
				break;
			case "1215":
				m_value = 200;
				break;
			case "1517":
				m_value = 500;
				break;
			case "2978":
				m_value = 5;
				m_percent = true;
				break;
			case "1449":
			case "1528":
				m_value = 6;
				m_percent = true;
				break;
			case "1450":
			case "1529":
				m_value = 10;
				m_percent = true;
				break;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.BaseGuard += living.BaseGuard * (double)m_value / 100.0;
			}
			else
			{
				living.BaseGuard += m_value;
			}
		}
	}
}

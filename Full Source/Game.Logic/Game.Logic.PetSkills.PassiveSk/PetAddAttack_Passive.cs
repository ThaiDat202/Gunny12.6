using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddAttack_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent = false;

	public PetAddAttack_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddAttack_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddAttack_Passive) is PetAddAttack_Passive petAddAttack_Passive)
		{
			petAddAttack_Passive.m_probability = ((m_probability > petAddAttack_Passive.m_probability) ? m_probability : petAddAttack_Passive.m_probability);
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
			case "1163":
				m_value = 150;
				break;
			case "1164":
				m_value = 300;
				break;
			case "1463":
				m_value = 450;
				break;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.Attack += living.Attack * (double)m_value / 100.0;
			}
			else
			{
				living.Attack += m_value;
			}
		}
	}
}

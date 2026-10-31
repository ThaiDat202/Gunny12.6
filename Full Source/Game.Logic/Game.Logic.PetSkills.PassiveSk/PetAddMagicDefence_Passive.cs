using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMagicDefence_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetAddMagicDefence_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMagicDefence_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicDefence_Passive) is PetAddMagicDefence_Passive petAddMagicDefence_Passive)
		{
			petAddMagicDefence_Passive.m_probability = ((m_probability > petAddMagicDefence_Passive.m_probability) ? m_probability : petAddMagicDefence_Passive.m_probability);
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
			string elementID = m_elementID;
			string text = elementID;
			if (text == "2980")
			{
				m_value = 5;
				m_percent = true;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.MagicDefence += living.MagicDefence * (double)m_value / 100.0;
			}
			else
			{
				living.MagicDefence += m_value;
			}
		}
	}
}

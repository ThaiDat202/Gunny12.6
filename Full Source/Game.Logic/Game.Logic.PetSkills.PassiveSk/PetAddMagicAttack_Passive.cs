using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMagicAttack_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetAddMagicAttack_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMagicAttack_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMagicAttack_Passive) is PetAddMagicAttack_Passive petAddMagicAttack_Passive)
		{
			petAddMagicAttack_Passive.m_probability = ((m_probability > petAddMagicAttack_Passive.m_probability) ? m_probability : petAddMagicAttack_Passive.m_probability);
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
			if (text == "2979")
			{
				m_value = 5;
				m_percent = true;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.MagicAttack += living.MagicAttack * (double)m_value / 100.0;
			}
			else
			{
				living.MagicAttack += m_value;
			}
		}
	}
}

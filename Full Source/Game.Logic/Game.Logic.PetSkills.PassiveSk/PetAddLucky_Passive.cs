using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddLucky_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent = false;

	public PetAddLucky_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddLucky_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddLucky_Passive) is PetAddLucky_Passive petAddLucky_Passive)
		{
			petAddLucky_Passive.m_probability = ((m_probability > petAddLucky_Passive.m_probability) ? m_probability : petAddLucky_Passive.m_probability);
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
		if (m_value != 0)
		{
			return;
		}
		string elementID = m_elementID;
		string text = elementID;
		if (!(text == "1261"))
		{
			if (text == "1262")
			{
				m_value = 500;
			}
		}
		else
		{
			m_value = 300;
		}
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		if (m_percent)
		{
			living.Lucky += living.Lucky * (double)m_value / 100.0;
		}
		else
		{
			living.Lucky += m_value;
		}
	}
}

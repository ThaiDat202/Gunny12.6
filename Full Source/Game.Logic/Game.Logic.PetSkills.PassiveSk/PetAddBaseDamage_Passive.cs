using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBaseDamage_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent;

	public PetAddBaseDamage_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBaseDamage_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamage_Passive) is PetAddBaseDamage_Passive petAddBaseDamage_Passive)
		{
			petAddBaseDamage_Passive.m_probability = ((m_probability > petAddBaseDamage_Passive.m_probability) ? m_probability : petAddBaseDamage_Passive.m_probability);
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
			case "1165":
				m_value = 100;
				break;
			case "1166":
				m_value = 200;
				break;
			case "1464":
				m_value = 300;
				break;
			case "2977":
				m_value = 5;
				m_percent = true;
				break;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.BaseDamage += living.BaseDamage * (double)m_value / 100.0;
			}
			else
			{
				living.BaseDamage += m_value;
			}
		}
	}
}

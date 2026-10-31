using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMaxBlood_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private string m_elementID;

	private bool m_percent = false;

	public PetAddMaxBlood_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMaxBlood_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMaxBlood_Passive) is PetAddMaxBlood_Passive petAddMaxBlood_Passive)
		{
			petAddMaxBlood_Passive.m_probability = ((m_probability > petAddMaxBlood_Passive.m_probability) ? m_probability : petAddMaxBlood_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.GameStarted += player_GameStarted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.GameStarted -= player_GameStarted;
	}

	private void player_GameStarted(Living living)
	{
		if (m_value == 0)
		{
			switch (m_elementID)
			{
			case "1216":
				m_value = 1500;
				break;
			case "1217":
				m_value = 3000;
				break;
			case "1518":
				m_value = 10;
				m_percent = true;
				break;
			case "3492":
				m_value = 50;
				m_percent = true;
				break;
			}
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.MaxBlood += living.MaxBlood * m_value / 100;
			}
			else
			{
				living.MaxBlood += m_value;
			}
			if (m_elementID == "3492")
			{
				living.Blood += living.Blood * m_value / 100;
			}
			living.Game.UpdateMaxBlood(living);
		}
	}
}

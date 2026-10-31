using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAfterPlayerShooted_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private int m_percent = 100;

	public PetAfterPlayerShooted_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAfterPlayerShooted_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		switch (elementID)
		{
		case "1122":
			m_value = 10;
			break;
		case "1123":
			m_value = 20;
			break;
		case "1124":
		case "1542":
		case "2642":
		case "2953":
			m_value = 30;
			break;
		case "3208":
			m_value = 215;
			break;
		case "1139":
		case "1543":
		case "2643":
		case "2954":
			m_value = 40;
			break;
		case "3209":
			m_value = 220;
			break;
		case "1149":
		case "1544":
		case "2644":
		case "2955":
			m_value = 50;
			break;
		case "3210":
			m_value = 225;
			break;
		case "3522":
			m_value = 300;
			break;
		case "1325":
		case "1578":
		case "2570":
		case "2749":
		case "3112":
		case "3166":
		case "3550":
		case "3568":
		case "3806":
			m_value = 15;
			break;
		case "1326":
		case "1579":
		case "2571":
		case "2750":
		case "3113":
		case "3167":
		case "3551":
		case "3569":
		case "3807":
			m_value = 25;
			break;
		case "1327":
		case "1580":
		case "2572":
		case "2751":
		case "3114":
		case "3168":
		case "3552":
		case "3570":
		case "3808":
			m_value = 36;
			break;
		case "1506":
		case "1532":
		case "1494":
		case "1481":
		case "1473":
		case "1462":
			m_value = 65;
			break;
		case "1639":
			m_value = 90;
			m_percent = 30;
			break;
		case "1640":
			m_value = 120;
			m_percent = 30;
			break;
		case "1641":
			m_value = 150;
			m_percent = 30;
			break;
		case "1616":
			m_value = 100;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAfterPlayerShooted_Passive) is PetAfterPlayerShooted_Passive petAfterPlayerShooted_Passive)
		{
			petAfterPlayerShooted_Passive.m_probability = ((m_probability > petAfterPlayerShooted_Passive.m_probability) ? m_probability : petAfterPlayerShooted_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += player_PlayerShoot;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= player_PlayerShoot;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_PlayerShoot(Player player)
	{
		if (rand.Next(100) <= m_percent)
		{
			player.PetEffects.DamagePercent += m_value;
		}
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.PetEffects.DamagePercent -= m_value;
	}
}

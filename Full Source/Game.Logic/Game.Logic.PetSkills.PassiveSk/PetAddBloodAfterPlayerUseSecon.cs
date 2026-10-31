using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBloodAfterPlayerUseSecondWeapon_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	public PetAddBloodAfterPlayerUseSecondWeapon_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBloodAfterPlayerUseSecondWeapon_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1397"))
		{
			if (elementID == "1398")
			{
				m_value = 5;
			}
		}
		else
		{
			m_value = 2;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAfterPlayerUseSecondWeapon_Passive) is PetAddBloodAfterPlayerUseSecondWeapon_Passive petAddBloodAfterPlayerUseSecondWeapon_Passive)
		{
			petAddBloodAfterPlayerUseSecondWeapon_Passive.m_probability = ((m_probability > petAddBloodAfterPlayerUseSecondWeapon_Passive.m_probability) ? m_probability : petAddBloodAfterPlayerUseSecondWeapon_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerUseSecondWeapon += player_PlayerUseSecondWeapon;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerUseSecondWeapon -= player_PlayerUseSecondWeapon;
	}

	private void player_PlayerUseSecondWeapon(Player player, int value)
	{
		if (value == 31)
		{
			int value2 = player.MaxBlood * m_value / 100;
			player.SyncAtTime = true;
			player.AddBlood(value2);
			player.SyncAtTime = false;
		}
	}
}

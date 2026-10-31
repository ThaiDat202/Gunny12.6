using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBloodAfterShootAngel_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	public PetAddBloodAfterShootAngel_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBloodAfterShootAngel_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1372"))
		{
			if (elementID == "1373")
			{
				m_value = 6;
			}
		}
		else
		{
			m_value = 3;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAfterShootAngel_Passive) is PetAddBloodAfterShootAngel_Passive petAddBloodAfterShootAngel_Passive)
		{
			petAddBloodAfterShootAngel_Passive.m_probability = ((m_probability > petAddBloodAfterShootAngel_Passive.m_probability) ? m_probability : petAddBloodAfterShootAngel_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += player_PlayerShoot;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= player_PlayerShoot;
	}

	private void player_PlayerShoot(Player player)
	{
		if (player.IsCure())
		{
			int value = player.MaxBlood * m_value / 100;
			player.SyncAtTime = true;
			player.AddBlood(value);
			player.SyncAtTime = false;
		}
	}
}

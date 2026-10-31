using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceEnemyBlood_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	public PetReduceEnemyBlood_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReduceEnemyBlood_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1372"))
		{
			if (elementID == "1373")
			{
				m_value = 3;
			}
		}
		else
		{
			m_value = 2;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceEnemyBlood_Passive) is PetReduceEnemyBlood_Passive petReduceEnemyBlood_Passive)
		{
			petReduceEnemyBlood_Passive.m_probability = ((m_probability > petReduceEnemyBlood_Passive.m_probability) ? m_probability : petReduceEnemyBlood_Passive.m_probability);
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
		int num = player.Blood * m_value / 100;
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
		List<Living> list = player.Game.Map.FindAllNearestEnemy(player.X, player.Y, 100.0, player);
		foreach (Living item in list)
		{
			item.SyncAtTime = true;
			item.AddBlood(-num, 1);
			item.SyncAtTime = false;
			if (item.Blood <= 0)
			{
				item.Die();
				if (player != null)
				{
					player?.PlayerDetail.OnKillingLiving(player.Game, 2, item.Id, item.IsLiving, num);
				}
			}
		}
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
	}
}

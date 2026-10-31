using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetShareTakeDamageToTeamEquip : BasePetEffect
{
	private int m_count;

	public PetShareTakeDamageToTeamEquip(int count, string elementID)
		: base(ePetEffectType.PetShareTakeDamageToTeamEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShareTakeDamageToTeamEquip) is PetShareTakeDamageToTeamEquip petShareTakeDamageToTeamEquip)
		{
			petShareTakeDamageToTeamEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.Info, isActive: true);
		player.BeginSelfTurn += beginSelfTurn;
		player.TakePlayerDamage += takeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= beginSelfTurn;
		player.TakePlayerDamage -= takeDamage;
	}

	private void takeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		int num = damageAmount / allTeamPlayers.Count;
		int num2 = criticalAmount / allTeamPlayers.Count;
		foreach (Player item in allTeamPlayers)
		{
			if (item == living)
			{
				continue;
			}
			item.SyncAtTime = true;
			item.RemoveBlood(num + num2);
			item.SyncAtTime = false;
			if (item.Blood <= 0)
			{
				item.Die();
				if (source != null && source is Player)
				{
					(source as Player).PlayerDetail.OnKillingLiving(source.Game, 2, item.Id, item.IsLiving, num + num2);
				}
			}
		}
		damageAmount = num;
		criticalAmount = num2;
	}

	private void beginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}

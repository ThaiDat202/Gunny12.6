using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardLockNpc : ABrain
{
	private int m_turn = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		m_turn++;
		if (m_turn >= 4)
		{
			base.Body.Die();
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnDie()
	{
		base.OnDie();
		StopEffect();
	}

	private void StopEffect()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			if (item.EffectList.GetOfType(eEffectType.LockDirectionEffect) is LockDirectionEffect lockDirectionEffect)
			{
				lockDirectionEffect.Stop();
			}
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			LockDirectionEffect lockDirectionEffect = item.EffectList.GetOfType(eEffectType.LockDirectionEffect) as LockDirectionEffect;
			if (lockDirectionEffect == null)
			{
				item.AddEffect(new LockDirectionEffect(200), 0);
			}
		}
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

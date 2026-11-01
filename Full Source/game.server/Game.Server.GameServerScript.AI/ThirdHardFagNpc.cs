using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardFagNpc : ABrain
{
	private int m_turn = 0;

	private int m_strengReduce = 100;

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

	private void StopEffect()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			if (item.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) is ReduceStrengthEffect reduceStrengthEffect)
			{
				reduceStrengthEffect.Stop();
			}
		}
	}

	public override void OnDie()
	{
		base.OnDie();
		StopEffect();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			ReduceStrengthEffect reduceStrengthEffect = item.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) as ReduceStrengthEffect;
			if (reduceStrengthEffect == null)
			{
				item.AddEffect(new ReduceStrengthEffect(200, m_strengReduce), 0);
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

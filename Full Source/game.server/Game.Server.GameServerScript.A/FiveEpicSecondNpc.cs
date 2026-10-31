using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveEpicSecondNpc : ABrain
{
	protected Player m_targer;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		List<Player> list = new List<Player>();
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.Y > 760)
			{
				list.Add(allLivingPlayer);
			}
		}
		if (list.Count > 0)
		{
			int index = base.Game.Random.Next(0, list.Count);
			m_targer = list[index];
			MoveToTarget();
		}
	}

	private void MoveToTarget()
	{
		base.Body.MoveTo(m_targer.X, m_targer.Y, "walk", 1000, 3, BeatTarget);
	}

	private void BeatTarget()
	{
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 50, base.Body.X + 50, "cry", 3000, directDamage: true);
	}

	private void DelayBeat()
	{
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

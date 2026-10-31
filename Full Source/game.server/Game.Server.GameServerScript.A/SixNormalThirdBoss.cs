using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SixNormalThirdBoss : ABrain
{
	private Player m_target = null;

	private int m_targetDis = 0;

	private int m_attackTurn = 0;

	private int npcId = 6132;

	private SimpleBoss m_npc;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		if (m_npc == null)
		{
			m_npc = base.Game.FindLivingTurnBossWithID(npcId)[0];
		}
		if (!m_npc.Config.CompleteStep)
		{
			m_npc.PlayMovie("stand", 0, 0);
			(base.Game as PVEGame).SendObjectFocus(m_npc, 1, 1000, 0);
			m_npc.PlayMovie("beida", 2000, 0);
			base.Body.CallFuction(CallBackUpdateMapping, 2000);
			m_npc.Config.CompleteStep = true;
			base.Body.CallFuction(CallAttackPlayer, 4000);
		}
		else
		{
			CallAttackPlayer();
		}
	}

	private void CallBackUpdateMapping()
	{
		(base.Game as PVEGame).SendLivingActionMapping(m_npc, "stand", "standB");
	}

	private void CallAttackPlayer()
	{
		m_target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		m_targetDis = (int)m_target.Distance(base.Body.X, base.Body.Y);
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (m_attackTurn == 0)
		{
			MoveToPlayerA(m_target);
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			MoveToPlayerB(m_target);
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			MoveToPlayerC(m_target);
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			MoveToPlayerD(m_target);
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			MoveToPlayerE(m_target);
			m_attackTurn++;
		}
		else
		{
			MoveToPlayerF(m_target);
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void MoveToPlayerA(Player player)
	{
		base.Body.PlayMovie("walk", 0, 0);
		if (base.Body.X > player.X)
		{
			base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		else
		{
			base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		base.Body.CallFuction(BeatA, 1000);
	}

	public void BeatA()
	{
		base.Body.PlayMovie("beatA", 0, 0);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 900, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1400, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1900, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 2300, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 2700, null);
	}

	public void MoveToPlayerB(Player player)
	{
		base.Body.PlayMovie("walk", 0, 0);
		if (base.Body.X > player.X)
		{
			base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		else
		{
			base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		base.Body.CallFuction(BeatB, 1000);
	}

	public void BeatB()
	{
		base.Body.PlayMovie("beatB", 0, 0);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 900, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1400, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1900, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 2300, null);
	}

	public void MoveToPlayerC(Player player)
	{
		base.Body.PlayMovie("walk", 0, 0);
		if (base.Body.X > player.X)
		{
			base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		else
		{
			base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		base.Body.CallFuction(BeatC, 1000);
	}

	public void BeatC()
	{
		base.Body.PlayMovie("beatC", 0, 0);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 900, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1400, null);
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1900, null);
	}

	public void MoveToPlayerD(Player player)
	{
		base.Body.PlayMovie("walk", 0, 0);
		if (base.Body.X > player.X)
		{
			base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		else
		{
			base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		base.Body.CallFuction(BeatD, 1000);
	}

	public void BeatD()
	{
		base.Body.PlayMovie("beatD", 0, 0);
		base.Body.CurrentDamagePlus = 2f;
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1200, null);
	}

	public void MoveToPlayerE(Player player)
	{
		base.Body.PlayMovie("walk", 0, 0);
		if (base.Body.X > player.X)
		{
			base.Body.JumpTo(player.X + 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		else
		{
			base.Body.JumpTo(player.X - 150, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		base.Body.CallFuction(BeatE, 1000);
	}

	public void BeatE()
	{
		base.Body.PlayMovie("beatE", 0, 0);
		base.Body.CurrentDamagePlus = 2f;
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1200, null);
	}

	public void MoveToPlayerF(Player player)
	{
		if (base.Body.X > player.X)
		{
			base.Body.JumpTo(player.X + 5, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		else
		{
			base.Body.JumpTo(player.X - 5, base.Body.Y - 475, "Jump", 1000, 0, 5, null, 1);
		}
		base.Body.CallFuction(BeatF, 1000);
	}

	public void BeatF()
	{
		base.Body.PlayMovie("beatF", 0, 0);
		base.Body.CurrentDamagePlus = 2f;
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "cry", 1500, null);
	}
}

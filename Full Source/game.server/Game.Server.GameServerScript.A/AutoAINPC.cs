using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class AutoAINPC : ABrain
{
	private int[] ListBoss = new int[4] { 71081, 71085, 71089, 71093 };

	protected Player m_targer;

	private int m_turn = 0;

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
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && Math.Abs(allFightPlayer.X - base.Body.X) < 150)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 100, base.Body.X + 100);
		}
		else
		{
			m_targer = base.Game.FindRandomPlayer();
			KillAttack(m_targer);
		}
		m_targer = base.Game.FindRandomPlayer();
		if (m_turn == 0)
		{
			KillAttackPlayer1(m_targer, 0);
			m_turn++;
		}
		else if (m_turn == 1)
		{
			KillAttackPlayer1(m_targer, 1);
			m_turn++;
		}
		else
		{
			KillAttackPlayer1(m_targer, 2);
			m_turn = 0;
		}
	}

	private void KillAttackPlayer1(Player player, int t)
	{
		int y = player.Y - 50;
		int x = ((player.X + 300 < base.Body.X) ? (player.X + 300) : ((player.X > base.Body.X + 300) ? (player.X - 300) : ((player.X <= base.Body.X) ? (base.Body.X - 1) : (base.Body.X + 1))));
		switch (t)
		{
		case 0:
			base.Body.MoveTo(x, y, "fly", 2400, "", 6, KillC);
			break;
		case 1:
			base.Body.MoveTo(x, y, "fly", 2400, "", 6, KillB);
			break;
		default:
			base.Body.MoveTo(x, y, "fly", 2400, "", 6, KillA);
			break;
		}
	}

	private void KillA()
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 500, m_targer.X + 500, "cry", 4000, null);
	}

	private void KillB()
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 500, m_targer.X + 500, "cry", 4000, null);
	}

	private void KillC()
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatC", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 500, m_targer.X + 500, "cry", 4000, null);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(Player player)
	{
		int num = ((player.X >= base.Body.X) ? 100 : (-100));
		base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", ((SimpleBoss)base.Body).NpcInfo.Delay, "", ((SimpleBoss)base.Body).NpcInfo.speed, Kill);
	}

	private void Kill()
	{
		if (Math.Abs(m_targer.Y - base.Body.Y) > 150 || Math.Abs(m_targer.X - base.Body.X) > 600)
		{
			base.Body.CurrentDamagePlus = 1f;
			base.Body.PlayMovie("beatC", 2400, 0);
			base.Body.RangeAttacking(m_targer.X - 100, m_targer.X + 100, "cry", 4000, null);
		}
		else
		{
			base.Body.CurrentDamagePlus = 1f;
			base.Body.PlayMovie("beatA", 1200, 0);
			base.Body.RangeAttacking(base.Body.X - 600, base.Body.X + 600, "cry", 4000, null);
		}
	}
}

using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCDSLT5 : ABrain
{
	protected Player m_targer;

	private int m_turn = 0;

	private PhysicalObj moive;

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
			if (allFightPlayer.IsLiving && Math.Abs(allFightPlayer.X - base.Body.X) < 300)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 300, base.Body.X + 300);
		}
		else if (m_turn == 0)
		{
			KillA();
			m_turn++;
		}
		else if (m_turn == 1)
		{
			Attack2();
			m_turn++;
		}
		else
		{
			Attack();
			m_turn = 0;
		}
	}

	private void KillA()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 70f;
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.RangeAttacking(player.X - 300, player.X + 300, "cry", 4000, null);
	}

	public void Attack2()
	{
		base.Body.PlayMovie("beatB", 1200, 0);
		base.Body.CallFuction(KillB, 1000);
	}

	private void KillB()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 70f;
		moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.ten.jianyu", "out", 1, 1);
		base.Body.RangeAttacking(player.X - 300, player.X + 300, "cry", 4000, null);
		base.Body.CallFuction(GoOut, 4000);
	}

	private void GoOut()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	public void Attack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player.X < base.Body.X)
		{
			int num = base.Body.X - (player.X + 289);
			base.Body.MoveTo(base.Body.X - num, player.Y, "walk", 2400, "", 5, KillC);
		}
		else
		{
			int num2 = player.X - 289 - base.Body.X;
			base.Body.MoveTo(base.Body.X + num2, player.Y, "walk", 2400, "", 5, KillC);
		}
	}

	private void KillC()
	{
		base.Body.CurrentDamagePlus = 70f;
		base.Body.PlayMovie("beatC", 2400, 0);
		base.Body.RangeAttacking(base.Body.X - 300, base.Body.X + 300, "cry", 4000, null);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatC", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

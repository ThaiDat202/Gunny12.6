using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCDSLT6 : ABrain
{
	protected Player m_targer;

	private int m_turn = 0;

	private int dame;

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
			if (allFightPlayer.IsLiving && Math.Abs(allFightPlayer.X - base.Body.X) < 150 && Math.Abs(allFightPlayer.Y - base.Body.Y) < 150)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 150, base.Body.X + 150);
			return;
		}
		m_targer = base.Game.FindRandomPlayer();
		if (m_turn == 0)
		{
			TankA();
			Move();
			m_turn++;
		}
		else if (m_turn == 1)
		{
			TankB();
			Move();
			m_turn++;
		}
		else
		{
			KillC();
			Move();
			m_turn = 0;
		}
	}

	private void TankA()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
		base.Body.CallFuction(KillA, 3000);
	}

	private void TankB()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
		base.Body.CallFuction(KillB, 3000);
	}

	private void TankC()
	{
	}

	private void Move()
	{
		base.Body.MoveTo(base.Game.Random.Next(100, 1150), base.Game.Random.Next(175, 720), "fly", 500, "", 12, null);
	}

	private void KillA()
	{
		Player player = base.Game.FindRandomPlayer();
		moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.eleven.055a", "1", 1, 1);
		base.Body.CallFuction(GoOut, 4000);
	}

	private void KillB()
	{
		Player player = base.Game.FindRandomPlayer();
		moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.eleven.055b", "out", 1, 1);
		base.Body.CallFuction(GoOut, 4000);
	}

	private void KillC()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatC", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
		base.Body.CallFuction(KillA, 3000);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void GoOut()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}
}

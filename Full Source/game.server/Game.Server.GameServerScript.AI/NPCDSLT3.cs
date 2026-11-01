using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCDSLT3 : ABrain
{
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
			return;
		}
		m_targer = base.Game.FindRandomPlayer();
		if (m_turn == 0)
		{
			KillA();
			m_turn++;
		}
		else if (m_turn == 1)
		{
			KillB();
			m_turn++;
		}
		else
		{
			KillC();
			m_turn = 0;
		}
	}

	private void KillA()
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 100, m_targer.X + 100, "cry", 4000, null);
	}

	private void KillB()
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 300, m_targer.X + 300, "cry", 4000, null);
	}

	private void KillC()
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatC", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 300, m_targer.X + 300, "cry", 4000, null);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

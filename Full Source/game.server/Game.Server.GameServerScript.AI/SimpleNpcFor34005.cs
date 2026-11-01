using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SimpleNpcFor34005 : ABrain
{
	private int m_attackTurn = 0;

	private bool m_openShield = false;

	private List<PhysicalObj> moives = new List<PhysicalObj>();

	private Player target;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (m_openShield)
		{
			base.Body.Config.IsShield = true;
		}
		else
		{
			base.Body.Config.IsShield = false;
		}
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		RemoveMovie();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnDie()
	{
		base.OnDie();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 200 && allFightPlayer.X < base.Body.X + 200)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 200, base.Body.X + 200);
		}
		else if ((int)base.Body.Properties1 == 0 && !m_openShield)
		{
			m_openShield = true;
			ChangeToA();
		}
		else if ((int)base.Body.Properties1 == 1 && m_openShield)
		{
			m_openShield = false;
			ChangeToNomal(AttackSkill);
		}
		else if ((int)base.Body.Properties1 == 1)
		{
			AttackSkill();
		}
	}

	private void ChangeToA()
	{
		base.Body.PlayMovie("toA", 1500, 3500);
	}

	private void ChangeToNomal(LivingCallBack callBack)
	{
		base.Body.PlayMovie("Ato", 1500, 3000);
		if (callBack != null)
		{
			base.Body.CallFuction(callBack, 3500);
		}
	}

	private void AttackSkill()
	{
		m_attackTurn++;
		switch (m_attackTurn)
		{
		case 1:
			AllAttackPlayer();
			break;
		case 2:
			AllAttackPlayer();
			break;
		case 3:
			AllAttackPlayer();
			m_attackTurn = 0;
			break;
		}
		base.Body.Properties1 = 0;
	}

	private void AllAttackPlayer()
	{
		base.Body.CurrentDamagePlus = 2f;
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.CallFuction(GoMovie, 4000);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 4500, null);
	}

	private void PersonAttack()
	{
		target = base.Game.FindRandomPlayer();
		if (base.Body.ShootPoint(target.X, target.Y, 84, 1200, 10000, 1, 3f, 2000))
		{
			base.Body.PlayMovie("beat", 1000, 0);
		}
	}

	private void PersonAttack2()
	{
		target = base.Game.FindRandomPlayer();
		if (base.Body.ShootPoint(target.X, target.Y, 84, 1200, 10000, 2, 3f, 2000))
		{
			base.Body.PlayMovie("beatA", 1000, 0);
		}
	}

	private void GoMovie()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			moives.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "moive", "asset.game.seven.cao", "in", 1, 0));
		}
	}

	private void RemoveMovie()
	{
		foreach (PhysicalObj moife in moives)
		{
			if (moife != null)
			{
				base.Game.RemovePhysicalObj(moife, sendToClient: true);
			}
		}
		moives = new List<PhysicalObj>();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

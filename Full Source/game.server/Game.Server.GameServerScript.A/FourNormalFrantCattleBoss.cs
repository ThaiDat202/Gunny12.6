using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourNormalFrantCattleBoss : ABrain
{
	private int m_attackTurn = 0;

	private Point m_pointMakeDamage;

	private bool m_isBorn = false;

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
		m_isBorn = false;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
		switch (m_attackTurn)
		{
		case 1:
			if (!m_isBorn)
			{
				((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
				base.Body.PlayMovie("born", 1000, 0);
				base.Body.CallFuction(ClearOldBoss, 1000);
				base.Body.Say("<span class=\"red\">Thực sự là giận lắm rồi. Ta sẽ nghiền nát tất cả!</span>", 0, 1200);
				base.Body.CallFuction(JumpAttack, 4000);
				m_isBorn = true;
			}
			else
			{
				base.Body.CallFuction(RunAttack, 1000);
			}
			break;
		case 10:
			base.Body.CallFuction(JumpAttack, 1000);
			m_attackTurn = 0;
			break;
		default:
			base.Body.CallFuction(RunAttack, 1000);
			break;
		}
	}

	private void JumpAttack()
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			int num = base.Game.Random.Next(100);
			base.Body.PlayMovie("jump", 1000, 0);
			base.Body.BoltMove(m_targer.X, m_targer.Y, 3000);
			((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 3000, 0);
			if (num < 50)
			{
				base.Body.CurrentDamagePlus = 2f;
				base.Body.PlayMovie("fallB", 4000, 0);
			}
			else
			{
				base.Body.PlayMovie("fall", 4000, 0);
				base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 5000, directDamage: true);
			}
		}
	}

	private void RunAttack()
	{
		m_targer = base.Game.FindFarPlayer(base.Body.X, base.Body.Y);
		if (m_targer != null)
		{
			base.Body.ChangeDirection(m_targer, 100);
			if (base.Body.Distance(m_targer.X, m_targer.Y) <= 100.0)
			{
				base.Body.CallFuction(NearAttack, 1000);
			}
			else if (base.Body.Direction == -1)
			{
				base.Body.MoveTo(m_targer.X - 20, m_targer.Y, "walk", 1000, 12, AttackRangeRun);
			}
			else
			{
				base.Body.MoveTo(m_targer.X + 20, m_targer.Y, "walk", 1000, 12, AttackRangeRun);
			}
		}
	}

	private void NearAttack()
	{
		m_pointMakeDamage = new Point(base.Body.X, base.Body.Y);
		if (base.Body.Direction == -1)
		{
			base.Body.MoveTo(m_targer.X - 200, m_targer.Y, "walk", 500, 12, AttackRangeRun);
		}
		else
		{
			base.Body.MoveTo(m_targer.X + 200, m_targer.Y, "walk", 500, 12, AttackRangeRun);
		}
	}

	private void AttackRangeRun()
	{
		base.Body.ChangeDirection(m_targer, 100);
		if (base.Body.X < m_pointMakeDamage.X)
		{
			base.Body.RangeAttacking(base.Body.X, m_pointMakeDamage.X, "cry", 0, directDamage: true);
		}
		else
		{
			base.Body.RangeAttacking(m_pointMakeDamage.X, base.Body.X, "cry", 0, directDamage: true);
		}
	}

	private void ClearOldBoss()
	{
		foreach (SimpleBoss item in base.Game.FindAllTurnBoss())
		{
			if (!item.IsLiving)
			{
				base.Game.RemoveLiving(item.Id);
			}
		}
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

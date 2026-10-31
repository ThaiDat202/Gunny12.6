using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourTerrorFrantCattleBoss : ABrain
{
	private int _mAttackTurn;

	private Point _mPointMakeDamage;

	private bool _mIsBorn;

	protected Player MTarger;

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		_mIsBorn = false;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		switch (_mAttackTurn)
		{
		case 0:
			base.Body.CallFuction(JumpAttack, 4000);
			break;
		case 1:
			base.Body.CallFuction(RunAttack, 1000);
			break;
		case 2:
			base.Body.CallFuction(RunAttack, 1000);
			_mAttackTurn = 0;
			return;
		}
		_mAttackTurn++;
	}

	private void JumpAttack()
	{
		MTarger = base.Game.FindRandomPlayer();
		if (MTarger != null)
		{
			base.Body.PlayMovie("jump", 1000, 0);
			base.Body.BoltMove(MTarger.X, MTarger.Y, 3000);
			((PVEGame)base.Game).SendObjectFocus(MTarger, 1, 3000, 0);
			if (base.Game.Random.Next(100) < 50)
			{
				base.Body.PlayMovie("fallB", 4000, 0);
			}
			else
			{
				base.Body.CurrentDamagePlus = 2f;
				base.Body.PlayMovie("fall", 4000, 0);
			}
			base.Body.RangeAttacking(MTarger.X - 150, MTarger.X + 150, "cry", 5000, directDamage: true);
		}
	}

	private void RunAttack()
	{
		MTarger = base.Game.FindFarPlayer(base.Body.X, base.Body.Y);
		if (MTarger != null)
		{
			base.Body.ChangeDirection(MTarger, 100);
			_mPointMakeDamage = new Point(base.Body.X, base.Body.Y);
			if (base.Body.Distance(MTarger.X, MTarger.Y) <= 100.0)
			{
				base.Body.CallFuction(NearAttack, 1000);
			}
			else if (base.Body.Direction == -1)
			{
				base.Body.MoveTo(MTarger.X - 100, MTarger.Y, "walk", 1000, 12, AttackRangeRun);
			}
			else
			{
				base.Body.MoveTo(MTarger.X + 100, MTarger.Y, "walk", 1000, 12, AttackRangeRun);
			}
		}
	}

	private void NearAttack()
	{
		if (base.Body.Direction == -1)
		{
			base.Body.MoveTo(MTarger.X - 200, MTarger.Y, "walk", 500, 12, AttackRangeRun);
		}
		else
		{
			base.Body.MoveTo(MTarger.X + 200, MTarger.Y, "walk", 500, 12, AttackRangeRun);
		}
	}

	private void AttackRangeRun()
	{
		base.Body.ChangeDirection(MTarger, 100);
		if (base.Body.X < _mPointMakeDamage.X)
		{
			base.Body.RangeAttacking(base.Body.X, _mPointMakeDamage.X, "cry", 0, directDamage: true);
		}
		else
		{
			base.Body.RangeAttacking(_mPointMakeDamage.X, base.Body.X, "cry", 0, directDamage: true);
		}
	}

	public override void OnDie()
	{
		base.OnDie();
		base.Body.Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourHardFrantCattleBoss.msg3"), 1, 1200);
	}
}

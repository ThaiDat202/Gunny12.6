using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SixTerrorFourBoss : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive;

	private Player target;

	private bool haveShield;

	private SimpleNpc shieldNpc;

	private int m_bloodRecover = 600000;

	private int m_countBeatD = 5;

	private int m_npcShield = 6344;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (haveShield && shieldNpc != null && !shieldNpc.IsLiving)
		{
			haveShield = false;
			base.Body.Config.IsShield = false;
			base.Game.RemoveLiving(shieldNpc.Id);
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "shield", "shield");
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "stand");
			(base.Game as PVEGame).SendHideBlood(base.Body, 1);
		}
		else if (haveShield && shieldNpc != null && shieldNpc.IsLiving && (shieldNpc.X != base.Body.X || shieldNpc.Y != base.Body.Y))
		{
			shieldNpc.BoltMove(base.Body.X, base.Body.Y, 0);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
		haveShield = false;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (m_attackTurn == 0)
		{
			BeatD();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			BeatC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			BeatB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			if (!haveShield)
			{
				InX();
			}
			else
			{
				BeatE();
				m_attackTurn++;
			}
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			BeatE();
			m_attackTurn++;
		}
		else if (m_attackTurn == 5)
		{
			AddBlood();
			m_attackTurn++;
		}
		else
		{
			BeatA();
			m_attackTurn = 0;
		}
	}

	private void BeatB()
	{
		target = base.Game.FindRandomPlayer();
		if (base.Body.X > target.X)
		{
			base.Body.MoveTo(target.X + 150, base.Body.Y, "walk", 1000, "", 5, RBeatB);
		}
		else
		{
			base.Body.MoveTo(target.X - 150, base.Body.Y, "walk", 1000, "", 5, RBeatB);
		}
		if (target.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 1200);
		}
		else
		{
			base.Body.ChangeDirection(-1, 1200);
		}
	}

	private void RBeatB()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		base.Body.PlayMovie("beatB", 1500, 0);
		base.Body.BeatDirect(target, "", 4000, 3, 1);
	}

	private void BeatC()
	{
		base.Body.CurrentDamagePlus = 4f;
		base.Body.PlayMovie("beatC", 1500, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void BeatD()
	{
		target = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 3f;
		base.Body.PlayMovie("beatD", 1000, 3000);
		base.Body.JumpTo(target.X, base.Body.Y - 475, "", 1500, 0, 5, GBeatD, 1);
	}

	private void GBeatD()
	{
		base.Body.BeatDirect(target, "", 4000, m_countBeatD, 1);
	}

	private void BeatE()
	{
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.CallFuction(RBeatE, 3200);
	}

	private void RBeatE()
	{
		base.Body.PlayMovie("stand", 8500, 1000);
		m_moive = ((PVEGame)base.Game).Createlayer(0, 0, "moive", "asset.game.six.chang", "out", 1, 0);
		m_moive.PlayMovie("in", 6000, 0);
		m_moive.PlayMovie("out", 9000, 0);
		((PVEGame)base.Game).SendGameFocus(1250, 450, 1, 0, 4000);
		base.Body.CurrentDamagePlus = 3f;
		base.Body.RangeAttacking(base.Body.X - 3000, base.Body.X + 3000, "cry", 7000, null);
	}

	private void BeatA()
	{
		int x = base.Game.Random.Next(900, 1400);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, NextAttack);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 800);
		}
		else
		{
			base.Body.ChangeDirection(-1, 800);
		}
		base.Body.CurrentDamagePlus = 2.5f;
		if (player != null)
		{
			if (base.Body.ShootPoint(player.X, player.Y, 61, 1000, 10000, 5, 3f, 2300))
			{
				base.Body.PlayMovie("beatA", 1500, 0);
			}
			if (base.Body.ShootPoint(player.X, player.Y, 61, 1000, 10000, 5, 3f, 4100))
			{
				base.Body.PlayMovie("beatA", 3300, 0);
			}
		}
	}

	private void AddBlood()
	{
		base.Body.CallFuction(Blood, 2000);
	}

	private void Blood()
	{
		base.Body.SyncAtTime = true;
		base.Body.PlayMovie("shieldB", 0, 2500);
		base.Body.AddBlood(m_bloodRecover);
	}

	private void InX()
	{
		base.Body.SyncAtTime = true;
		base.Body.PlayMovie("inX", 0, 4500);
		base.Body.CallFuction(MakeShield, 5000);
	}

	private void MakeShield()
	{
		LivingConfig config = (base.Game as PVEGame).BaseConfig();
		shieldNpc = (base.Game as PVEGame).CreateNpc(m_npcShield, base.Body.X, base.Body.Y, 1, base.Body.Direction, config);
		haveShield = true;
		base.Body.Config.IsShield = true;
		(base.Game as PVEGame).SendLivingActionMapping(base.Body, "shield", "shieldB");
		(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "standC");
		(base.Game as PVEGame).SendHideBlood(base.Body, 0);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

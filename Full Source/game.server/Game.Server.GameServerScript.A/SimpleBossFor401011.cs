using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SimpleBossFor401011 : ABrain
{
	private int m_attackTurn = 0;

	private SimpleNpc m_forzen;

	private int m_forzenid = 404014;

	private int m_npcShield = 404013;

	protected Player m_targer;

	private int m_fx = 0;

	private int m_tx = 0;

	private List<Point> DangerZonePoint = new List<Point>
	{
		new Point(60, 550),
		new Point(551, 1030),
		new Point(1031, 1500),
		new Point(1501, 2000),
		new Point(2001, 2400)
	};

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
		if (base.Body.State == 10)
		{
			KillAttack(0, base.Game.Map.Info.ForegroundWidth + 1);
			return;
		}
		if (base.Body.Blood <= base.Body.MaxBlood * 50 / 100)
		{
			base.Body.SyncAtTime = true;
			base.Body.ForbidAidBlood = false;
			base.Body.CanAddBlood = true;
			base.Body.AddBlood(base.Body.MaxBlood * 15 / 100);
		}
		if (m_attackTurn == 0)
		{
			AttackA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			AttackB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			AttackC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			AttackD();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void AttackA()
	{
		int x = base.Game.Random.Next(1900, 2100);
		int y = base.Game.Random.Next(411, 600);
		base.Body.MoveTo(x, y, "fly", 3000, "", 12, AttackBloodMax);
	}

	private void AttackC()
	{
		int x = base.Game.Random.Next(490, 997);
		int y = base.Game.Random.Next(411, 700);
		base.Body.MoveTo(x, y, "fly", 3000, "", 12, AllAttackC);
	}

	private void AttackD()
	{
		int index = base.Game.Random.Next(DangerZonePoint.Count);
		m_fx = DangerZonePoint[index].X;
		m_tx = DangerZonePoint[index].Y;
		int x = m_tx - m_fx / 2 + m_fx;
		base.Body.MoveTo(x, base.Body.Y, "fly", 3000, "", 12, AllAttackD);
	}

	private void AllAttackC()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 15f;
			base.Body.PlayMovie("beatC", 1700, 0);
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 4000, null);
		}
	}

	private void AllAttackD()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.CurrentDamagePlus = 50f;
		base.Body.PlayMovie("beatD", 1700, 0);
		base.Body.RangeAttacking(m_fx, m_tx, "cry", 4000, null);
	}

	private void AttackBloodMax()
	{
		Player player = base.Game.FindPlayerByBlood();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 20f;
			if (base.Body.ShootPoint(player.X, player.Y, 11424, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	private void AttackB()
	{
		int x = base.Game.Random.Next(1120, 1720);
		int y = base.Game.Random.Next(646, 750);
		base.Body.MoveTo(x, y, "fly", 3000, "", 12, FrozenA);
	}

	private void FrozenA()
	{
		base.Body.PlayMovie("beatB", 1700, 0);
		base.Body.CallFuction(CreateFrozen, 5000);
	}

	private void CreateFrozen()
	{
		m_targer = base.Game.FindPlayerByBlood();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (m_targer != null)
		{
			m_forzen = ((SimpleBoss)base.Body).CreateChild(m_forzenid, m_targer.X, m_targer.Y, showBlood: true, ((PVEGame)base.Game).BaseConfig());
			m_forzen.Properties1 = m_targer.Id;
			m_forzen.SetRelateDemagemRect(-68, -135, 135, 135);
			int highDelayTurn = base.Game.GetHighDelayTurn();
			m_forzen.CallFuction(BlockAndHidePlayer, 1000);
			((PVEGame)base.Game).SendObjectFocus(m_forzen, 1, 1000, 0);
			m_targer.AddBlood(-(m_targer.Blood * 15 / 100), 1);
			((PVEGame)base.Game).PveGameDelay = highDelayTurn + 1;
		}
	}

	private void BlockAndHidePlayer()
	{
		m_targer.SetVisible(state: false);
		m_targer.BlockTurn = true;
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
	}
}

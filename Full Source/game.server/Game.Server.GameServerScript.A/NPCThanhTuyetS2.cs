using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCThanhTuyetS2 : ABrain
{
	protected Player m_targer;

	private int m_turn = 0;

	private int dame;

	private PhysicalObj moive;

	private int npcID1 = 3312;

	private List<Point> pointCreatePhy = new List<Point>();

	private static readonly string[] SealChat = new string[3] { "你们不是名册上的人", "落雪成碑 ", "开花冰雪里，岂是不知春" };

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
			TankB();
			m_turn++;
		}
		else if (m_turn == 1)
		{
			TankE();
			m_turn++;
		}
		else if (m_turn == 2)
		{
			m_turn++;
			Player player = base.Game.FindRandomPlayer();
			AddBloodA();
			if (player.IsFrost)
			{
				m_targer = player;
				m_targer.RemoveBlood(5000);
				TruMau();
			}
			else
			{
				TankD();
			}
		}
		else if (m_turn == 3)
		{
			TankA();
			m_turn++;
		}
		else if (m_turn == 4)
		{
			TankF();
			m_turn++;
		}
		else
		{
			TankC();
			m_turn = 0;
		}
	}

	private void TankA()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
		base.Body.Say("开花冰雪里，岂是不知春", 1, 4000);
	}

	private void TankB()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.Say("你们不是名册上的人", 1, 4000);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
		base.Body.CallFuction(CreateExplode, 4000);
	}

	private void TankC()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatC", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
	}

	private void TankD()
	{
		dame = base.Game.Random.Next(200, 300);
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatD", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - dame, m_targer.X + dame, "cry", 4000, null);
		base.Body.CallFuction(HieuUngA, 3000);
		base.Body.CallFuction(PhaBang, 4000);
	}

	private void TankE()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.ShootPoint(player.X, player.Y, 1, 1, 1, 1, 1f, 3000);
	}

	private void TankF()
	{
		base.Body.CurrentDamagePlus = 2f;
		int num = base.Game.Random.Next(0, SealChat.Length);
		base.Body.Say(SealChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 0, 1000);
		base.Body.CallFuction(MoveAllPlayer, 2200);
	}

	public void CreateExplode()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			((SimpleBoss)base.Body).CreateBoss(npcID1, player.X, player.Y, player.Direction, 0);
		}
	}

	private void HieuUngA()
	{
		Player player = base.Game.FindRandomPlayer();
		moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "effect.game.fifteen.424a", "out", 1, 1);
		base.Body.CallFuction(GoOut, 4000);
	}

	private void TruMau()
	{
		base.Game.FindRandomPlayer();
		base.Body.PlayMovie("beatD", 2400, 0);
		base.Body.CallFuction(HieuUngA, 3000);
	}

	private void MoveAllPlayer()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			allLivingPlayer.Energy -= 100;
		}
	}

	private void AddBloodA()
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.RangeAttacking(m_targer.X, m_targer.Y, "cry", 3000, null);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (!allLivingPlayer.IsFrost)
			{
				return;
			}
			allLivingPlayer.RemoveBlood(5000);
			base.Body.AddBlood(150000);
		}
		if (m_targer.Blood <= 0)
		{
			m_targer.Die();
		}
	}

	private void PhaBang()
	{
		base.Game.FindRandomPlayer().IsFrost = false;
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10000f;
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
		base.Body.CallFuction(HieuUngA, 3000);
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

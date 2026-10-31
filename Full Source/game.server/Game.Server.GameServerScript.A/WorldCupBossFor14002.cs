using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class WorldCupBossFor14002 : ABrain
{
	private int m_attackTurn = 0;

	private int ballID = 14001;

	public int currentCount = 0;

	public int Dander = 0;

	private PhysicalObj moive;

	private Living target;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		if (base.Body.Direction == -1)
		{
			base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}
		else
		{
			base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		FindBall();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void FindBall()
	{
		List<Living> list = base.Game.FindHoldBall();
		if (list.Count > 0)
		{
			target = list[0];
			if (moive != null && target.X < moive.X + 100 && target.X > moive.X - 100)
			{
				AttackA();
				return;
			}
			GoOut();
			int x = target.X;
			x = ((x >= base.Body.X) ? (x - 160) : (x + 160));
			base.Body.MoveTo(x, target.Y, "walk", 500, "", 7, TargetBall);
		}
	}

	private void TargetBall()
	{
		if (target != null)
		{
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.nine.biaoji", "out", 1, 1);
		}
		base.Body.CallFuction(AttackC, 100);
	}

	private void AttackC()
	{
		base.Body.PlayMovie("beatC", 1000, 0);
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item != null && item.X < moive.X + 100 && item.X > moive.X - 100)
			{
				int reduce = 0;
				item.AddEffect(new ReduceStrengthEffect(2, reduce), 0);
				item.LimitEnergy = true;
				item.TotalCureEnergy = 30;
				item.AddRemoveEnergy(30);
			}
		}
		if (m_attackTurn == 1)
		{
			base.Body.CallFuction(AttackB, 1000);
		}
		else
		{
			m_attackTurn++;
		}
	}

	private void AttackB()
	{
		base.Body.CurrentDamagePlus = 1.8f;
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.CallFuction(MovingPlayer, 1000);
	}

	private void MovingPlayer()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			int x = player.X;
			x = ((x >= base.Game.Map.Info.ForegroundWidth / 2) ? (x - 100) : (x + 100));
			player.StartSpeedMult(x, player.Y);
		}
		base.Body.CallFuction(RangeAttacking, 1000);
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		m_attackTurn = 0;
	}

	private void AttackA()
	{
		base.Body.PlayMovie("beatA", 1500, 0);
		base.Body.CallFuction(shotBall, 1500);
	}

	private void shotBall()
	{
		if (target != null)
		{
			int num = target.X + 200 * base.Body.Direction;
			if (num > base.Game.Map.Info.ForegroundWidth)
			{
				num = base.Game.Map.Info.ForegroundWidth;
			}
			if (num < 1)
			{
				num = 1;
			}
			if (target is Player)
			{
				LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
				livingConfig.IsHelper = true;
				livingConfig.HasTurn = false;
				SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(ballID, target.X, target.Y, 1, -1, livingConfig);
				simpleNpc.MoveTo(num, simpleNpc.Y, "walk", 0, 3);
				base.Game.TakePassBall(-1);
			}
			else if (target is SimpleNpc)
			{
				target.MoveTo(num, target.Y, "walk", 0, 3);
			}
			((PVEGame)base.Game).IsMissBall = true;
		}
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

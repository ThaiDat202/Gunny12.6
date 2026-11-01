using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class CryptBoss2 : ABrain
{
	private int m_attackTurn = 0;

	public int currentCount = 0;

	public int Dander = 0;

	private PhysicalObj moive;

	private Player target;

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
		bool flag = false;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 150 && allFightPlayer.X < base.Body.X + 150)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 150, base.Body.X + 150);
		}
		else if (m_attackTurn == 0)
		{
			AttackA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			AttackB();
			m_attackTurn++;
		}
		else
		{
			AttackC();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatC", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void AttackA()
	{
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.CallFuction(GoShootA, 4000);
	}

	private void GoShootA()
	{
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			((PVEGame)base.Game).SendGameFocus(target, 0, 1000);
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.eleven.055a", "out", 1, 1);
			base.Body.CallFuction(GoOutA, 2000);
			base.Body.CallFuction(RangeAttacking, 1000);
		}
	}

	private void GoOutA()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	private void AttackC()
	{
		Player player = base.Game.FindNearestPlayer(0, 1500);
		if (player != null)
		{
			base.Body.MoveTo(player.X + 110, player.Y, "fly", 1000, "", 18, NextAttackC);
		}
	}

	private void NextAttackC()
	{
		base.Body.CurrentDamagePlus = 0.7f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.CallFuction(RangeAttackingC, 3700);
	}

	private void RangeAttackingC()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		base.Body.CallFuction(Comback, 1500);
	}

	private void AttackB()
	{
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.CallFuction(GoShootB, 4000);
	}

	private void GoShootB()
	{
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			((PVEGame)base.Game).SendGameFocus(target, 0, 1000);
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.eleven.055b", "out", 1, 1);
			base.Body.CallFuction(GoOutB, 2000);
			base.Body.CallFuction(RangeAttacking, 1000);
		}
	}

	private void GoOutB()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	private void Comback()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.MoveTo(796, 361, "fly", 1000, "", 18, ChangeDirection);
	}

	private void ChangeDirection()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
	}

	private void RangeAttacking()
	{
		base.Body.CurrentDamagePlus = 0.9f;
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}
}

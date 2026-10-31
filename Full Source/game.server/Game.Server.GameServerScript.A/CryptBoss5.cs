using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class CryptBoss5 : ABrain
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
		base.Body.CurrentDamagePlus = 2000.5f;
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
	}

	private void AttackA()
	{
		base.Body.CurrentDamagePlus = 2.5f;
		base.Body.PlayMovie("beatA", 500, 0);
		base.Body.CallFuction(RangeAttackingA, 2500);
	}

	private void RangeAttackingA()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}

	private void AttackC()
	{
		base.Body.PlayMovie("beatC", 500, 0);
		base.Body.CallFuction(MovingPlayer, 3500);
	}

	private void MovingPlayer()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player.X > 200)
			{
				player.StartSpeedMult(player.X - 200, player.Y);
			}
		}
		base.Body.CallFuction(RangeAttacking, 1000);
	}

	private void RangeAttacking()
	{
		base.Body.CurrentDamagePlus = 1.9f;
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}

	private void AttackB()
	{
		base.Body.PlayMovie("beatB", 700, 0);
		base.Body.CallFuction(RangeAttackingB, 3000);
	}

	private void RangeAttackingB()
	{
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			((PVEGame)base.Game).SendGameFocus(target, 0, 1000);
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.eleven.057", "out", 1, 1);
			base.Body.CallFuction(RangeAttacking, 1000);
			base.Body.CallFuction(GoOutB, 2000);
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
}

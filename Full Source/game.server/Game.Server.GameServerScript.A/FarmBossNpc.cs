using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FarmBossNpc : ABrain
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
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 919)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(765, base.Game.Map.Info.ForegroundWidth + 1000);
		}
		else if (m_attackTurn == 0)
		{
			PersonalAttack();
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
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void AttackC()
	{
		base.Body.PlayMovie("beatC", 1000, 0);
		base.Body.CallFuction(Health, 2500);
	}

	private void Health()
	{
		switch ((base.Body as SimpleBoss).NpcInfo.ID)
		{
		case 50101:
			base.Body.AddBlood(5000);
			break;
		case 50102:
			base.Body.AddBlood(15000);
			break;
		case 50103:
			base.Body.AddBlood(20000);
			break;
		case 50104:
			base.Body.AddBlood(25000);
			break;
		case 50105:
			base.Body.AddBlood(30000);
			break;
		}
	}

	private void AttackB()
	{
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.CallFuction(GoShootB, 2000);
	}

	private void GoShootB()
	{
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			((PVEGame)base.Game).SendGameFocus(target, 0, 1000);
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.fifteen.380b", "out", 1, 1);
			base.Body.CallFuction(GoOutB, 1500);
			base.Body.CallFuction(RangeAttacking, 1500);
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

	private void PersonalAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 0.8f;
			int num = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, 127, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}
}

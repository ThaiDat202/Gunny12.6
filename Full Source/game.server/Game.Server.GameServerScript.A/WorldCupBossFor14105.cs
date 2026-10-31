using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class WorldCupBossFor14105 : ABrain
{
	private int m_attackTurn = 0;

	public int currentCount = 0;

	public int Dander = 0;

	private int npcID = 14104;

	public List<SimpleNpc> Children = new List<SimpleNpc>();

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1070)
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
			KillAttack(0, base.Game.Map.Info.ForegroundWidth + 1);
			return;
		}
		if (m_attackTurn == 0)
		{
			CallChild();
			m_attackTurn++;
			return;
		}
		if (m_attackTurn == 1)
		{
			AttackA();
			m_attackTurn++;
			return;
		}
		if (m_attackTurn == 2)
		{
			AttackC();
			m_attackTurn++;
			return;
		}
		if (((SimpleBoss)base.Body).CurrentLivingNpcNum > 0)
		{
			HealChild();
		}
		else
		{
			CallChild();
		}
		m_attackTurn = 1;
	}

	public void HealChild()
	{
		base.Body.PlayMovie("beatD", 1000, 0);
		base.Body.CallFuction(Heal, 1500);
	}

	public void Heal()
	{
		SimpleNpc simpleNpc = ((SimpleBoss)base.Body).FindFrostChild();
		if (simpleNpc != null && simpleNpc.IsLiving)
		{
			if (simpleNpc != null && simpleNpc.IsFrost)
			{
				((PVEGame)base.Game).SendGameFocus(simpleNpc, 0, 2000);
				simpleNpc.IsFrost = false;
				simpleNpc.PlayMovie("stand", 1000, 0);
			}
			else
			{
				simpleNpc.AddBlood(10000);
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatD", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void CallChild()
	{
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.CallFuction(CreateChild, 1500);
	}

	private void AttackA()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		base.Body.CurrentDamagePlus = 1.3f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		foreach (Player item in allFightPlayers)
		{
			if (item != null && base.Body.ShootPoint(item.X, item.Y, 14005, 1000, 10000, 1, 3f, 1550))
			{
				base.Body.PlayMovie("beatA", 1050, 0);
			}
		}
	}

	private void AttackC()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.MoveTo(player.X + 220, player.Y, "walkB", 1000, "", 22, NextAttackC);
		}
	}

	private void NextAttackC()
	{
		base.Body.CurrentDamagePlus = 1.7f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("beatC", 500, 0);
		base.Body.RangeAttacking(base.Body.X - 270, base.Body.X, "cry", 1500, null);
		base.Body.CallFuction(Comback, 2000);
	}

	private void Comback()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.MoveTo(1816, base.Body.Y, "walkB", 1000, "", 22, ChangeDirection);
	}

	private void ChangeDirection()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 1580, 861, 60, 1, -1);
	}
}

using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NormalKingFirst : ABrain
{
	private int m_attackTurn = 1;

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
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 390 && allFightPlayer.X < 1110)
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
			KillAttack(390, 1110);
		}
		else if (m_attackTurn == 0)
		{
			AllAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			PersonalAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			PersonalAttack();
			m_attackTurn++;
		}
		else
		{
			Healing();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.Say("君临天下", 1, 1000);
		base.Body.PlayMovie("beat", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void AllAttack()
	{
		base.Body.Say("地震咯!", 1, 1000);
		base.Body.CurrentDamagePlus = 2.5f;
		base.Body.PlayMovie("beat", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void PersonalAttack()
	{
		int x = base.Game.Random.Next(850, 920);
		base.Body.Say("送你个球", 0, 1000);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", ((SimpleBoss)base.Body).NpcInfo.speed, NextAttack);
	}

	private void Healing()
	{
		base.Body.SyncAtTime = true;
		base.Body.Say("哈利路亚", 1, 1000);
		base.Body.AddBlood(150000);
		base.Body.PlayMovie("renew", 1000, 4500);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player == null)
		{
			return;
		}
		if (player.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 800);
		}
		else
		{
			base.Body.ChangeDirection(-1, 800);
		}
		base.Body.CurrentDamagePlus = 0.3f;
		if (player != null)
		{
			int x = base.Game.Random.Next(player.X - 50, player.X + 50);
			if (base.Body.ShootPoint(x, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 1f, 2300))
			{
				base.Body.PlayMovie("beat2", 1500, 0);
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

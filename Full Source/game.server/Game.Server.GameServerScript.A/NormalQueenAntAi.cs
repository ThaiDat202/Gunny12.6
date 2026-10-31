using System.Drawing;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NormalQueenAntAi : ABrain
{
	private int m_attackTurn = 0;

	private int npcID = 2104;

	private Point[] brithPoint = new Point[5]
	{
		new Point(979, 630),
		new Point(1013, 630),
		new Point(1052, 630),
		new Point(1088, 630),
		new Point(1142, 630)
	};

	private static string[] ShootChat = new string[2] { " 魔法箭", "你肯定想知道魔法水晶剑是什么？但你没机会了！" };

	private static string[] KillPlayerChat = new string[1] { "你肯定想知道魔法水晶剑是什么？但你没机会了！" };

	private static string[] KillAttackChat = new string[2] { "怎么敢的！", "我得赶紧通知gm 玩家敢骑到噩梦boss脸上了" };

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
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1169 && allFightPlayer.X < base.Game.Map.Info.ForegroundWidth + 1)
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
			KillAttack(1169, base.Game.Map.Info.ForegroundWidth + 1);
		}
		else if (m_attackTurn == 0 || m_attackTurn == 1)
		{
			if (base.Body.Blood <= base.Body.MaxBlood * 20 / 100 && m_attackTurn == 0)
			{
				PersonalAttack3();
				m_attackTurn = 1;
			}
			else if (base.Body.Blood <= base.Body.MaxBlood * 20 / 100 && m_attackTurn == 1)
			{
				PersonalAttack2();
				m_attackTurn = 2;
			}
			else
			{
				PersonalAttack();
				m_attackTurn = 0;
			}
		}
		else
		{
			PersonalAttack();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.CurrentDamagePlus = 50f;
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void PersonalAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 0.8f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 1000);
			int num2 = base.Game.Random.Next(670, 880);
			int num3 = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	private void PersonalAttack2()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 800f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			if (player.IsFrost)
			{
				base.Body.Say("算你聪明", 1, 1000);
			}
			else
			{
				base.Body.Say("还真接啊？？？？", 1, 1000);
			}
			int num2 = base.Game.Random.Next(670, 880);
			int num3 = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	private void PersonalAttack3()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 0.8f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say("下一箭无仙骨不能硬接", 1, 1000);
			int num2 = base.Game.Random.Next(670, 880);
			int num3 = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	private void Call()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, brithPoint, 9, 3, 9, -1);
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		base.Body.Say(KillPlayerChat[num], 1, 0, 2000);
	}
}

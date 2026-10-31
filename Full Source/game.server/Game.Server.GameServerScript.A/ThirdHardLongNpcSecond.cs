using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardLongNpcSecond : ABrain
{
	private int m_attackTurn = 0;

	private int isSay = 0;

	private static string[] KillPlayerChat = new string[1] { LanguageMgr.GetTranslation("Anh em tiến lên !") };

	private static string[] KillAttackChat = new string[2]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
	};

	private static string[] ShootedChat = new string[2]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")
	};

	private static string[] DiedChat = new string[1] { LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg17") };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		isSay = 0;
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
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 300, base.Body.X + 300);
		}
		else if (m_attackTurn == 0)
		{
			if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
			{
				PersonalAttack();
			}
			else
			{
				PersonalAttack();
			}
			m_attackTurn++;
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
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void PersonalAttack()
	{
		int x = base.Game.Random.Next(50, 200);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, NextAttack);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 1.8f;
			if (player.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 50);
			}
			else
			{
				base.Body.ChangeDirection(-1, 50);
			}
			int num = base.Game.Random.Next(player.X, player.X);
			if (base.Body.ShootPoint(1100, player.Y, 58, 1000, 10000, 3, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		base.Body.Say(KillPlayerChat[num], 1, 0, 2000);
	}

	public override void OnShootedSay()
	{
		int num = base.Game.Random.Next(0, ShootedChat.Length);
		if (isSay == 0 && base.Body.IsLiving)
		{
			base.Body.Say(ShootedChat[num], 1, 900, 0);
			isSay = 1;
		}
		if (!base.Body.IsLiving)
		{
			num = base.Game.Random.Next(0, DiedChat.Length);
			base.Body.Say(DiedChat[num], 1, 100, 2000);
		}
	}
}

using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdTerrorLongNpcFirst : ABrain
{
	private int m_attackTurn = 0;

	private int isSay = 0;

	protected Player m_targer;

	private static string[] KillPlayerChat = new string[1] { LanguageMgr.GetTranslation("Anh em tiến lên !") };

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
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1269 && allFightPlayer.X < base.Game.Map.Info.ForegroundWidth + 1)
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
			KillAttack();
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

	private void KillAttack()
	{
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		Beat(m_targer);
	}

	private void Beat(Player player)
	{
		int num = (int)player.Distance(base.Body.X, base.Body.Y);
		if (player.X > base.Body.X)
		{
			base.Body.MoveTo(base.Body.X + num + 50, base.Body.Y, "walk", 1200, "", 4, MoveBeat);
		}
		else
		{
			base.Body.MoveTo(base.Body.X - num - 50, base.Body.Y, "walk", 1200, "", 4, MoveBeat);
		}
	}

	private void MoveBeat()
	{
		base.Body.Beat(m_targer, "beatB", 100, 0, 500, 1, 1);
	}

	private void PersonalAttack()
	{
		int x = base.Game.Random.Next(1200, 1550);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, NextAttack);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 1.8f;
			if (base.Body.ShootPoint(player.X, player.Y, 58, 1000, 10000, 1, 2.5f, 2550))
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

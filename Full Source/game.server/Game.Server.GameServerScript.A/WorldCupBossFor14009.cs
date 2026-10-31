using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class WorldCupBossFor14009 : ABrain
{
	private int m_attackTurn = 0;

	private int isSay = 0;

	private PhysicalObj moive;

	private static string[] AllAttackChat = new string[3]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg1"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg2"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg3")
	};

	private static string[] ShootChat = new string[2]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg4"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg5")
	};

	private static string[] KillPlayerChat = new string[2]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg6"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg7")
	};

	private static string[] CallChat = new string[2]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg8"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg9")
	};

	private static string[] JumpChat = new string[3]
	{
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg10"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg11"),
		LanguageMgr.GetTranslation("Game.Server.GameServerScript.AI.NPC.SimpleQueenAntAi.msg12")
	};

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

	private int npcID = 14008;

	private SimpleNpc arbiNpc = null;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1155 && allFightPlayer.X < 1571)
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
		}
		else if (base.Body.Config.IsShield || base.Body.Blood <= 1)
		{
			arbiNpc = base.Game.FindHelper();
			if (m_attackTurn == 0)
			{
				AttackB(0, base.Game.Map.Info.ForegroundWidth + 1);
				m_attackTurn++;
			}
			else if (m_attackTurn == 1)
			{
				AttackA(0, base.Game.Map.Info.ForegroundWidth + 1);
				m_attackTurn++;
			}
			else if (m_attackTurn == 2)
			{
				AttackC(0, base.Game.Map.Info.ForegroundWidth + 1);
				m_attackTurn++;
			}
			else if (m_attackTurn == 3)
			{
				GoOutGoal();
				m_attackTurn++;
			}
			else
			{
				ComeBackGoal();
				m_attackTurn = 0;
			}
		}
	}

	private void GoOutGoal()
	{
		base.Body.MoveTo(1152, 861, "walk", 1000, "", 3, OnDown);
	}

	private void OnDown()
	{
		((PVEGame)base.Game).SendGameFocus(base.Body, 0, 2000);
		base.Body.SetRelateDemagemRect(-15, -35, 35, 35);
		base.Body.PlayMovie("cryB", 2000, 0);
		((SimpleBoss)base.Body).AddDelay(1000);
		base.Body.Config.IsDown = true;
	}

	private void ComeBackGoal()
	{
		base.Body.MoveTo(1223, 861, "walk", 1000, "", 3, OnUp);
	}

	private void OnUp()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("standB", 2000, 0);
		base.Body.AddBlood(((SimpleBoss)base.Body).NpcInfo.Blood);
		base.Body.Config.IsShield = true;
		((PVEGame)base.Game).CanBeginNextProtect = true;
		base.Body.Config.IsDown = false;
		base.Body.SetRelateDemagemRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
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
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void AttackA(int fx, int tx)
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 1.1f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
			base.Body.PlayMovie("beatA", 1700, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
		}
	}

	private void AttackB(int fx, int tx)
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 1.5f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
			base.Body.PlayMovie("beatB", 1900, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
			base.Body.CallFuction(GoMovie, 4000);
		}
	}

	private void GoMovie()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			moive = ((PVEGame)base.Game).Createlayer(item.X, item.Y, "moive", "asset.game.zero.294b", "out", 1, 0);
		}
	}

	private void AttackC(int fx, int tx)
	{
		if (arbiNpc != null)
		{
			base.Body.CurrentDamagePlus = 200f;
			base.Body.PlayMovie("beatE", 1300, 0);
			base.Body.CallFuction(RangeAttackingNPC, 2000);
		}
	}

	private void RangeAttackingNPC()
	{
		List<Living> list = base.Game.FindLivingByID(npcID);
		base.Body.RangeAttackingNPC("movie", 2000, list);
		((PVEGame)base.Game).SendGameFocus(arbiNpc, 0, 1000);
		if (((PVEGame)base.Game).TotalKillCount > list.Count)
		{
			base.Body.CallFuction(CallYellowCard, 3200);
		}
		else
		{
			base.Body.CallFuction(CallRedCard, 3200);
		}
	}

	private void CallYellowCard()
	{
		((PVEGame)base.Game).SendGameFocus(arbiNpc, 0, 1000);
		arbiNpc.PlayMovie("beatA", 2000, 0);
		arbiNpc.CallFuction(RemoveProtect, 2200);
	}

	private void CallRedCard()
	{
		((PVEGame)base.Game).SendGameFocus(base.Body, 0, 2000);
		base.Body.Say("Bye nhóc thua rồi nhé.", 1, 0, 2000);
		arbiNpc.CallFuction(EndGame, 2200);
	}

	private void EndGame()
	{
		((PVEGame)base.Game).SendGameFocus(arbiNpc, 0, 170);
		arbiNpc.PlayMovie("beatC", 1700, 0);
		arbiNpc.Say("Quá kém cỏi, thẻ đỏ rời sân ngay.", 1, 0, 2000);
		((PVEGame)base.Game).IsMissBall = true;
	}

	private void RemoveProtect()
	{
		((PVEGame)base.Game).SendGameFocus(base.Body, 0, 1700);
		base.Body.PlayMovie("beatD", 1700, 0);
		base.Body.Config.IsShield = false;
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

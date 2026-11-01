using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class TerrorKingZhen : ABrain
{
	private int attackingTurn = 1;

	private int npcID = 1311;

	private int isSay = 0;

	private int dander = 0;

	private int isBomb = 0;

	private Point[] brithPoint = new Point[2]
	{
		new Point(682, 673),
		new Point(1092, 673)
	};

	private static string[] AllAttackChat = new string[4]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg1"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg2"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg3"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg4")
	};

	private static string[] ShootChat = new string[5]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg5"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg7"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg8"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg9")
	};

	private static string[] CallChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg10") };

	private static string[] AngryChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg11") };

	private static string[] KillAttackChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg12") };

	private static string[] SealChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg13") };

	private static string[] KillPlayerChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg14"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg15")
	};

	private static string[] ShootedChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg16"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg17"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg18")
	};

	private static string[] DiedChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg19") };

	private static string[] GoOnAnggerChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.HardKingLast.msg17") };

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

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
		base.OnStartAttacking();
		bool flag = false;
		int num = 0;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 620 && allFightPlayer.X < 1194)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag && base.Body.State != 1)
		{
			KillAttack(620, 1194);
			return;
		}
		if (attackingTurn == 1)
		{
			Healing();
			HalfAttack();
		}
		else if (attackingTurn == 2)
		{
			Healing();
			Summon();
		}
		else if (attackingTurn == 3)
		{
			Healing();
			Seal();
		}
		else if (attackingTurn == 4)
		{
			Healing();
			Angger();
		}
		else
		{
			GoOnAngger();
			attackingTurn = 0;
		}
		attackingTurn++;
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void HalfAttack()
	{
		base.Body.CurrentDamagePlus = 1f;
		int num = base.Game.Random.Next(0, SealChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		if (base.Body.Direction == 1)
		{
			base.Body.RangeAttacking(base.Body.X - 1, base.Game.Map.Info.ForegroundWidth + 1, "cry", 3300, null);
		}
		else
		{
			base.Body.RangeAttacking(-1, base.Body.X + 1, "cry", 3300, null);
		}
	}

	public void Summon()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 0);
		base.Body.PlayMovie("beatA", 100, 0);
		base.Body.CallFuction(CreateChild, 2500);
	}

	public void Seal()
	{
		int num = base.Game.Random.Next(0, SealChat.Length);
		((SimpleBoss)base.Body).Say(SealChat[num], 1, 0);
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			(base.Body as SimpleBoss).TowardsToPlayer(player.X, 500);
			base.Body.PlayMovie("mantra", 2000, 2000);
			base.Body.Seal(player, 1, 3000);
		}
	}

	public void Angger()
	{
		int num = base.Game.Random.Next(0, AngryChat.Length);
		base.Body.Say(AngryChat[num], 1, 0);
		base.Body.State = 1;
		dander += 100;
		((SimpleBoss)base.Body).SetDander(dander);
		if (base.Body.Direction == -1)
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
		}
		else
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(-82, -252, 74, 50);
		}
	}

	public void GoOnAngger()
	{
		if (base.Body.State == 1)
		{
			((PVEGame)base.Game).CreateTip(0, 0, "com.mapobject.asset.wordtip75", "com.mapobject.asset.wordtip75", "Empty", 1, 0);
			base.Body.CurrentDamagePlus = 1000f;
			base.Game.AddAction(new FocusAction(base.Body, 2, 0, 1500));
			base.Body.Say(GoOnAnggerChat[0], 0, 3500);
			base.Body.PlayMovie("beatC", 9500, 0);
			base.Body.RangeAttacking(-1, base.Game.Map.Info.ForegroundWidth + 1, "cry", 12600, null);
			base.Game.AddAction(new LockFocusAction(isLock: false, 0, 0));
			base.Body.Die(11500);
			isBomb = 1;
			return;
		}
		base.Body.PlayMovie("cast", 0, 2000);
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ContinueReduceBloodEffect(2, 800, null), 0);
		}
	}

	public void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3300, null);
	}

	public void Healing()
	{
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(1000);
		base.Body.Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.TerrorKingZhen.msg20"), 1, 0);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, brithPoint, 8, 2, 0);
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		if (base.Body.IsLiving && base.Body.State == 0)
		{
			int num = base.Game.Random.Next(0, KillPlayerChat.Length);
			base.Body.Say(KillPlayerChat[num], 1, 0, 2000);
		}
	}

	public override void OnDiedSay()
	{
		int num = base.Game.Random.Next(0, DiedChat.Length);
		base.Body.Say(DiedChat[num], 1, 0, 1500);
	}

	public override void OnShootedSay()
	{
		int num = base.Game.Random.Next(0, ShootedChat.Length);
		if (isSay == 0 && base.Body.IsLiving)
		{
			base.Body.Say(ShootedChat[num], 1, 0, 0);
			isSay = 1;
		}
		if (!base.Body.IsLiving && isBomb == 0)
		{
			num = base.Game.Random.Next(0, DiedChat.Length);
			base.Body.Say(DiedChat[num], 1, 200, 2000);
		}
	}
}

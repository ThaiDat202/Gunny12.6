using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdTerorKingFour : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_Bloom = null;

	private int m_isSay = 0;

	private int isSay = 0;

	private int m_BloomID = 3318;

	private int m_attackCount = 3;

	private int m_bloomCreateDir = 0;

	private List<PhysicalObj> m_Physical = new List<PhysicalObj>();

	private Point[] bloomLeftPoint = new Point[1]
	{
		new Point(474, 545)
	};

	private Point[] bloomRightPoint = new Point[1]
	{
		new Point(1035, 547)
	};

	private static string[] AllBakeChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg1") };

	private static string[] BurningChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg2") };

	private static string[] ShootChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg7"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg8")
	};

	private static string[] KillAttackChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg21") };

	private static string[] AllAttackChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg3"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg4"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg5")
	};

	private static string[] AllCarryChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg9"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg10")
	};

	private static string[] KillBloomChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg11"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg12"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg13")
	};

	private static string[] KillPlayerChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg14"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg15"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg16")
	};

	private static string[] DieChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg17"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg18")
	};

	private static string[] ShooteChat = new string[4]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg20"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg201"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg202"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg203")
	};

	private static string[] BloomReliveChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg7"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg8")
	};

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		isSay = 0;
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
		if (m_attackTurn == 0)
		{
			int num = base.Game.Random.Next(0, AllBakeChat.Length);
			base.Body.Say(AllBakeChat[num], 0, 1500, 3000);
			base.Body.MoveTo(750, base.Body.Y, "walk", 4000, ((SimpleBoss)base.Body).NpcInfo.speed, AllBakeAttack);
			base.Body.PlayMovie("", 11000, 0);
			base.Body.CallFuction(CreateBloomS, 11000);
			List<Living> livedLivings = base.Game.GetLivedLivings();
			foreach (Living item in livedLivings)
			{
				if (!(item is Player) && !(item is SimpleBoss))
				{
					item.PlayMovie("die", 9000, 0);
					item.Die(10500);
				}
			}
			m_attackTurn++;
			int maxDelay = GetMaxDelay();
			((SimpleBoss)base.Body).Delay = maxDelay + 100;
		}
		else if (!IsKillAttack())
		{
			if (m_attackTurn == 1)
			{
				PersonalAttack();
				m_attackTurn++;
			}
			else if (m_attackTurn == 2)
			{
				AllCarry();
				m_attackTurn++;
			}
			else if (m_attackTurn == 3)
			{
				AllAttack();
				m_attackTurn++;
			}
			else if (m_attackTurn == 4)
			{
				ClearBloom();
				m_attackTurn++;
			}
			else if (m_attackTurn == 5)
			{
				AllAttack();
				m_attackTurn++;
			}
			else if (m_attackTurn == 6)
			{
				AllBakeAttack();
				m_attackTurn = 1;
			}
		}
	}

	public int GetMaxDelay()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		int num = 0;
		foreach (Player item in allFightPlayers)
		{
			if (item.Delay > num)
			{
				num = item.Delay;
			}
		}
		return num;
	}

	private bool IsKillAttack()
	{
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 150 && allFightPlayer.X < base.Body.X + 150)
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
			KillAttack(base.Body.X - 150, base.Body.X + 150);
			return true;
		}
		return false;
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 20f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1500);
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void PersonalAttack()
	{
		if (IsKillAttack())
		{
			return;
		}
		base.Body.CurrentDamagePlus = 0.4f;
		Player player = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (player != null)
		{
			int num = 4350;
			int num2 = 4200;
			int num3 = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num3], 1, 1500);
			base.Body.PlayMovie("aim", 2500, 0);
			if (player.X > base.Body.X)
			{
				base.Body.ChangeDirection(1, 0);
			}
			else
			{
				base.Body.ChangeDirection(-1, 0);
			}
			for (int i = 0; i < m_attackCount; i++)
			{
				AttackPlayer(player, num, num2, Range: true);
				num += 1600 - i * 80;
				num2 += 1500;
			}
		}
	}

	private void AttackPlayer(Living player, int ShootDelay, int MovieDelay, bool Range)
	{
		if (player.X > base.Body.X)
		{
			base.Body.ChangeDirection(1, 800 + MovieDelay - 1500);
		}
		else
		{
			base.Body.ChangeDirection(-1, 800 + MovieDelay - 1500);
		}
		int x = base.Game.Random.Next(player.X - 10, player.X + 10);
		if (!Range)
		{
			if (base.Body.ShootPoint(x, player.Y, 53, 1000, 10000, 1, 1f, ShootDelay))
			{
				base.Body.PlayMovie("beatA", MovieDelay, 1000);
			}
			return;
		}
		float time = 1f;
		if (base.Body.X - player.X > 271)
		{
			time = 1.8f;
		}
		if (base.Body.ShootPoint(x, player.Y, 53, 1000, 10000, 3, time, ShootDelay))
		{
			base.Body.PlayMovie("beatA", MovieDelay, 1000);
		}
	}

	private void AllBakeAttack()
	{
		int num = base.Game.Random.Next(0, BurningChat.Length);
		base.Body.Say(BurningChat[num], 0, 1000, 3000);
		base.Body.PlayMovie("beatC", 1500, 0);
		base.Body.PlayMovie("", 3500, 0);
		base.Body.CallFuction(BakeAttack, 3500);
	}

	private void BakeAttack()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item2 in allLivingPlayers)
		{
			if (item2.IsLiving)
			{
				item2.AddEffect(new ContinueReduceBloodEffect(3, 300, null), 0);
				PhysicalObj item = ((PVEGame)base.Body.Game).CreatePhysicalObj(item2.X - 20, 500, "Flame", "game.assetmap.Flame", "1", 1, 0);
				m_Physical.Add(item);
			}
		}
		base.Body.CallFuction(ClearFlame, 1800);
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 0.8f;
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 1500);
		base.Body.PlayMovie("beatB", 3500, 0);
		base.Body.RangeAttacking(-1, base.Game.Map.Info.ForegroundWidth + 1, "cry", 5500, null);
	}

	private void AllCarry()
	{
		int num = base.Game.Random.Next(0, AllCarryChat.Length);
		base.Body.Say(AllCarryChat[num], 1, 1500);
		base.Body.PlayMovie("beatC", 3500, 1000);
		base.Body.CallFuction(BeginCarry, 4800);
		base.Body.CallFuction(ClearFlame, 5800);
	}

	private void BeginCarry()
	{
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving)
			{
				int num = base.Game.Random.Next(50, 1400);
				PhysicalObj item = ((PVEGame)base.Body.Game).CreatePhysicalObj(num - 20, 500, "Flame", "game.assetmap.Flame", "", 1, 0);
				m_Physical.Add(item);
				((PVEGame)base.Body.Game).AddAction(new LivingBoltMoveAction(allFightPlayer, num, 500, 0));
			}
		}
		base.Body.CurrentDamagePlus = 0.1f;
		base.Body.RangeAttacking(-1, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}

	private void ClearFlame()
	{
		PhysicalObj[] array = base.Game.FindPhysicalObjByName("Flame");
		PhysicalObj[] array2 = array;
		foreach (PhysicalObj phy in array2)
		{
			base.Game.RemovePhysicalObj(phy, sendToClient: true);
		}
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			allFightPlayer.StartFalling(direct: false);
		}
	}

	private void ClearBloom()
	{
		if (m_Bloom.X > base.Body.X)
		{
			base.Body.ChangeDirection(1, 0);
		}
		else
		{
			base.Body.ChangeDirection(-1, 0);
		}
		int num = base.Game.Random.Next(0, KillBloomChat.Length);
		base.Body.Say(KillBloomChat[num], 1, 1500);
		base.Body.CallFuction(BeginClearBloom, 3500);
	}

	private void BeginClearBloom()
	{
		if (m_Bloom != null)
		{
			base.Body.CurrentDamagePlus = 0.1f;
			AttackPlayer(m_Bloom, 1650, 1500, Range: false);
			m_Bloom.PlayMovie("die", 1000, 2000);
			base.Body.CallFuction(DieBloomS, 2500);
			base.Body.CallFuction(CreateBloomS, 4000);
		}
		else
		{
			base.Body.CallFuction(PersonalAttack, 0);
		}
	}

	private void DieBloomS()
	{
		m_Bloom.Die();
	}

	private void CreateBloomS()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.IsHelper = true;
		livingConfig.CanTakeDamage = false;
		livingConfig.ReduceBloodStart = 100;
		if (m_bloomCreateDir == 0)
		{
			m_bloomCreateDir = 1;
			int num = base.Game.Random.Next(0, bloomLeftPoint.Length);
			base.Body.ChangeDirection(-1, 0);
			m_Bloom = ((PVEGame)base.Game).CreateBoss(m_BloomID, bloomLeftPoint[num].X, bloomLeftPoint[num].Y, 1, 3, "", livingConfig);
			if (m_isSay == 0)
			{
				m_isSay = 1;
				((PVEGame)base.Game).SendFreeFocus(m_Bloom.X, m_Bloom.Y, 0, 0, 1000);
				m_Bloom.Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingFour.msg19"), 0, 2000, 3000);
			}
			else
			{
				int num2 = base.Game.Random.Next(0, BloomReliveChat.Length);
				m_Bloom.Say(BloomReliveChat[num2], 0, 2000, 3000);
			}
		}
		else
		{
			m_bloomCreateDir = 0;
			int num = base.Game.Random.Next(0, bloomRightPoint.Length);
			base.Body.ChangeDirection(1, 0);
			m_Bloom = ((PVEGame)base.Game).CreateBoss(m_BloomID, bloomRightPoint[num].X, bloomRightPoint[num].Y, -1, 3, "", livingConfig);
			int num3 = base.Game.Random.Next(0, BloomReliveChat.Length);
			m_Bloom.Say(BloomReliveChat[num3], 0, 2000, 3000);
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnDiedSay()
	{
		int num = base.Game.Random.Next(1000, 2500);
		base.Body.Say(DieChat[base.Game.Random.Next(0, DieChat.Length)], 1, num - 800, 2000);
		((PVEGame)base.Game).SendFreeFocus(base.Body.X, base.Body.Y, 0, num - 800, 1000);
	}

	public override void OnShootedSay()
	{
		int delay = base.Game.Random.Next(1000, 2500);
		if (isSay == 0 && base.Body.IsLiving)
		{
			base.Body.Say(ShooteChat[base.Game.Random.Next(0, ShooteChat.Length)], 1, delay);
			isSay = 1;
		}
	}
}

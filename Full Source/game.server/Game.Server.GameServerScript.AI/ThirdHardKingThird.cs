using System;
using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardKingThird : ABrain
{
	private int _mAttackTurn;

	private readonly int _mAttackCount = 3;

	private int _totemType;

	private const int NpcId = 3203;

	private SimpleNpc _totemBlood;

	private SimpleNpc _totemBlow;

	private const int BlowTotemId = 3212;

	private const int BloodTotemId = 3213;

	private int _isSay;

	private static readonly string[] KillPlayerChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg19"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg20"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg21")
	};

	private static readonly string[] ShootChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg4"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg5")
	};

	private static readonly string[] SeriesShootChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg2"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg3")
	};

	private static readonly string[] KillAttackChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg22") };

	private static readonly string[] DieChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg18") };

	private static readonly string[] ShooteChat = new string[4]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg23"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg231"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg232"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg233")
	};

	private static readonly string[] AllAttackChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg15"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg16"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg17")
	};

	private static readonly string[] CallChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg12"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg13"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg14")
	};

	private static readonly string[] BlowTotemChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg7"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg8")
	};

	private static readonly string[] BloodTotemChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg9"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg10"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalKingThird.msg11")
	};

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		_isSay = 0;
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (IsKillAttack())
		{
			return;
		}
		switch (_mAttackTurn)
		{
		case 0:
			AllAttack();
			_mAttackTurn++;
			break;
		case 1:
			ClearTotem();
			_mAttackTurn++;
			break;
		case 2:
			TotemAttack();
			_mAttackTurn++;
			break;
		case 3:
		{
			int currentLivingNpcNum = ((SimpleBoss)base.Body).CurrentLivingNpcNum;
			if (currentLivingNpcNum < 8)
			{
				Summon();
			}
			else
			{
				BeginAttack();
			}
			_mAttackTurn = 0;
			break;
		}
		}
	}

	private bool IsKillAttack()
	{
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 100 && allFightPlayer.X < base.Body.X + 100)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (!flag)
		{
			return false;
		}
		KillAttack(base.Body.X - 100, base.Body.X + 100);
		return true;
	}

	private void ClearTotem()
	{
		if (_totemBlood != null && _totemBlood.IsLiving)
		{
			_totemBlood.PlayMovie("die", 0, 1000);
			_totemBlood.Die(1500);
			base.Body.CallFuction(BeginAttack, 2000);
		}
		else
		{
			base.Body.CallFuction(BeginAttack, 0);
		}
	}

	private void TotemAttack()
	{
		_totemType = base.Game.Random.Next(0, 2);
		base.Body.Say((_totemType == 0) ? BlowTotemChat[base.Game.Random.Next(0, BlowTotemChat.Length)] : BloodTotemChat[base.Game.Random.Next(0, BloodTotemChat.Length)], 1, 1500);
		base.Body.PlayMovie("call", 2500, 3000);
		base.Body.CallFuction(CreateTotem, 2500);
	}

	private void CreateTotem()
	{
		if (_totemType == 0)
		{
			int x = base.Game.FindBombPlayerX(100);
			_totemBlow = ((PVEGame)base.Game).CreateNpc(3212, x, 500, 1, 1);
			_totemBlood.SetRelateDemagemRect(-15, -120, 30, 120);
		}
		else
		{
			int x2 = ((base.Game.Random.Next(0, 2) == 0) ? base.Game.Random.Next(base.Body.X - 200, base.Body.X - 100) : base.Game.Random.Next(base.Body.X + 100, base.Body.X + 200));
			_totemBlood = ((PVEGame)base.Game).CreateNpc(3213, x2, 500, 1, 1);
			_totemBlood.SetRelateDemagemRect(-15, -120, 30, 120);
		}
	}

	private void BeginAttack()
	{
		int x = base.Game.Random.Next(645, 845);
		if (base.Game.Random.Next(0, 2) == 0)
		{
			base.Body.MoveTo(x, base.Body.Y, "walk", 2000, ((SimpleBoss)base.Body).NpcInfo.speed, PersonalAttack);
		}
		else
		{
			base.Body.MoveTo(x, base.Body.Y, "walk", 2000, ((SimpleBoss)base.Body).NpcInfo.speed, SeriesAttack);
		}
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 0.8f;
		base.Body.Say(AllAttackChat[base.Game.Random.Next(0, AllAttackChat.Length)], 1, 1500);
		base.Body.PlayMovie("beatC", 3500, 0);
		base.Body.RangeAttacking(-1, base.Game.Map.Info.ForegroundWidth + 1, "cry", 5500, null);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 20f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1500);
		base.Body.PlayMovie("beatC", 3500, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5500, null);
	}

	private void AttackPlayer(Player player, int shootDelay, int movieDelay, int value)
	{
		if (player.X > base.Body.X)
		{
			base.Body.ChangeDirection(1, movieDelay - 100);
		}
		else
		{
			base.Body.ChangeDirection(-1, movieDelay - 100);
		}
		int x = base.Game.Random.Next(player.X - 10, player.X + 10);
		float time = 1f;
		if (Math.Abs(base.Body.X - player.X) > 350)
		{
			time = 1.8f;
		}
		if (value == 0)
		{
			base.Body.CurrentDamagePlus = 1.3f;
			if (base.Body.ShootPoint(x, player.Y, 55, 1000, 10000, 1, time, shootDelay))
			{
				base.Body.PlayMovie("beatB", movieDelay, 0);
			}
		}
		else
		{
			base.Body.CurrentDamagePlus = 0.7f;
			if (base.Body.ShootPoint(x, player.Y, 54, 1000, 10000, 1, time, shootDelay))
			{
				base.Body.PlayMovie("beatA", movieDelay, 0);
			}
		}
	}

	private void PersonalAttack()
	{
		if (!IsKillAttack())
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				int num = base.Game.Random.Next(0, ShootChat.Length);
				base.Body.Say(ShootChat[num], 1, 1500);
				AttackPlayer(player, 4000, 3000, 0);
			}
		}
	}

	private void SeriesAttack()
	{
		if (IsKillAttack())
		{
			return;
		}
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		int num = 0;
		int num2 = 4000;
		int num3 = 3000;
		foreach (Player item in allFightPlayers)
		{
			if (item.IsLiving && num < _mAttackCount)
			{
				num++;
				int num4 = base.Game.Random.Next(0, SeriesShootChat.Length);
				base.Body.Say(SeriesShootChat[num4], 1, 1500);
				AttackPlayer(item, num2, num3, 1);
				num2 += 1800;
				num3 += 1800;
			}
		}
		if (num >= _mAttackCount)
		{
			return;
		}
		for (int i = num; i < _mAttackCount; i++)
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				AttackPlayer(player, num2, num3, 1);
				num2 += 1800;
				num3 += 1800;
			}
		}
	}

	private void Summon()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 1500);
		base.Body.PlayMovie("callB", 1500, 3000);
		base.Body.CallFuction(CreateChild, 1500);
	}

	private void CreateChild()
	{
		Point[] brithPoint = new Point[4]
		{
			new Point(base.Body.X + 200, 500),
			new Point(base.Body.X - 200, 500),
			new Point(base.Body.X + 100, 500),
			new Point(base.Body.X - 100, 500)
		};
		((SimpleBoss)base.Body).CreateChild(3203, brithPoint, 8, 4, 0, -1);
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		base.Body.Say(KillPlayerChat[num], 1, 1000);
	}

	public override void OnDiedSay()
	{
		int num = base.Game.Random.Next(1000, 2500);
		base.Body.Say(DieChat[base.Game.Random.Next(0, DieChat.Length)], 1, num - 800, 2000);
		((PVEGame)base.Game).SendFreeFocus(base.Body.X, base.Body.Y, 0, num - 800, 1000);
	}

	public override void OnDie()
	{
		base.OnDie();
		if (_totemBlood != null && _totemBlood.IsLiving)
		{
			_totemBlood.PlayMovie("die", 0, 0);
			_totemBlood.Die(1500);
		}
	}

	public override void OnShootedSay()
	{
		int delay = base.Game.Random.Next(1000, 2500);
		if (_isSay == 0 && base.Body.IsLiving)
		{
			base.Body.Say(ShooteChat[base.Game.Random.Next(0, ShooteChat.Length)], 1, delay);
			_isSay = 1;
		}
	}
}

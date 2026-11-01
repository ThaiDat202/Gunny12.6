using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourHardCattleBoss : ABrain
{
	private int _mAttackTurn;

	private PhysicalObj _mEffectAttack;

	private PhysicalObj _mPowerUpEffect;

	private readonly int m_totalNpc = 3;

	private readonly int npcId = 4207;

	private readonly float m_perPowerUp = 0.5f;

	private readonly int m_reduceStreng = 100;

	private bool _isFear;

	private Player _target;

	private float _lastpowDamage;

	private float _mCurrentPowDamage;

	private bool _isSay;

	private bool _canSacriflce;

	private int _sacriflceFlame;

	private readonly List<Point> _flamePoints = new List<Point>
	{
		new Point(-100, -100),
		new Point(100, -100),
		new Point(0, -200)
	};

	private readonly List<SimpleNpc> _flames = new List<SimpleNpc>();

	private readonly string[] _callNpcSay = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg2"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg3"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg4")
	};

	private readonly string[] _killAttackSay = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg29"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg30")
	};

	private readonly string[] _personAttack = new string[4]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg5"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg7"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg8")
	};

	private readonly string[] _allAttackPlayerSay = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg9"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg10"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg11")
	};

	private readonly string[] _tiredSay = new string[4]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg12"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg13"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg14"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg15")
	};

	private readonly string[] _fearSay = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg16"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg17")
	};

	private readonly string[] _onShootedChat = new string[9]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg20"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg21"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg22"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg23"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg24"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg25"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg26"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg27"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg28")
	};

	private readonly string[] _specialAttackSay = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg18"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg19")
	};

	private readonly string[] _diedChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg34") };

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		ClearEffect();
		base.Body.CurrentShootMinus = 1f;
		_isSay = false;
		if (_canSacriflce)
		{
			_canSacriflce = false;
		}
		if (_sacriflceFlame > 0)
		{
			_sacriflceFlame = 0;
		}
		base.Body.Config.IsShield = !_isFear;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		_isFear = false;
		base.Body.CurrentDamagePlus = 1f;
		_mCurrentPowDamage = 1f;
	}

	public override void OnStartAttacking()
	{
		if (!_isFear)
		{
			bool flag = false;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 200 && allFightPlayer.X < base.Body.X + 200)
				{
					flag = true;
				}
			}
			if (flag)
			{
				KillAttack(base.Body.X - 200, base.Body.X + 200);
				return;
			}
		}
		switch (_mAttackTurn)
		{
		case 0:
			if (!_isFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				if (((SimpleBoss)base.Body).CurrentLivingNpcNum <= 0)
				{
					int num = base.Game.Random.Next(0, _callNpcSay.Length);
					base.Body.Say(_callNpcSay[num], 1, 5000);
					base.Body.CallFuction(CallNpc, 7000);
				}
				else
				{
					base.Body.CallFuction(AttackAllPlayer, 4000);
				}
			}
			_mAttackTurn++;
			break;
		case 1:
			if (!_isFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				base.Body.CallFuction(AttackPerson, 4000);
			}
			_mAttackTurn++;
			break;
		case 2:
			if (!_isFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				base.Body.CallFuction(AttackAllPlayer, 4000);
				_mAttackTurn++;
				break;
			}
			goto IL_0389;
		case 3:
		{
			if (_isFear)
			{
				goto IL_0389;
			}
			base.Body.CallFuction(PowerUpEffect, 2000);
			int num = base.Game.Random.Next(0, _tiredSay.Length);
			base.Body.Say(_tiredSay[num], 1, 4000);
			if (((SimpleBoss)base.Body).CurrentLivingNpcNum <= 0)
			{
				_isFear = true;
				base.Body.CallFuction(ChangeAtoB, 4000);
				_mAttackTurn = 0;
			}
			else
			{
				_flames.AddRange((base.Body as SimpleBoss).FindChildLivings());
				base.Body.CallFuction(GetReadySacriflce, 6000);
				_mAttackTurn++;
			}
			break;
		}
		case 4:
			{
				_mCurrentPowDamage = 1f;
				int num = base.Game.Random.Next(0, _specialAttackSay.Length);
				base.Body.Say(_specialAttackSay[num], 1, 2000);
				base.Body.CallFuction(JumpAndAttack, 2000);
				_mAttackTurn = 0;
				break;
			}
			IL_0389:
			_isFear = false;
			_mCurrentPowDamage = 1f;
			base.Body.CallFuction(ChangeBtoA, 2000);
			_mAttackTurn = 0;
			break;
		}
	}

	private void GetReadySacriflce()
	{
		int num = 0;
		(base.Game as PVEGame)?.SendHideBlood(_flames, 1);
		foreach (SimpleNpc flame in _flames)
		{
			Point point = _flamePoints[num++];
			flame.MoveTo(base.Body.X + point.X, base.Body.Y + point.Y, "fly", 0, 15, FlameSacriflce);
		}
	}

	private void FlameSacriflce()
	{
		_sacriflceFlame++;
		if (_sacriflceFlame == _flames.Count)
		{
			_canSacriflce = true;
		}
		if (!_canSacriflce)
		{
			return;
		}
		foreach (SimpleNpc flame in _flames)
		{
			flame.PlayMovie("die", 2000, 1000);
			flame.Die(3000);
		}
		base.Body.PlayMovie("beatC", 1000, 0);
		base.Body.CallFuction(delegate
		{
			AddBlood(10000);
		}, 4500);
		_flames.Clear();
		base.Body.Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg31"), 1, 5000);
	}

	private void AddBlood(int blood)
	{
		base.Body.AddBlood(blood);
	}

	private void KillAttack(int fx, int tx)
	{
		_lastpowDamage = base.Body.CurrentDamagePlus;
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 100);
		int num = base.Game.Random.Next(0, _killAttackSay.Length);
		base.Body.Say(_killAttackSay[num], 1, 1000);
		base.Body.PlayMovie("beatC", 2000, 0);
		base.Body.PlayMovie("beatE", 5000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 7000, null);
		base.Body.CallFuction(SetState, 8000);
	}

	private void AttackPerson()
	{
		_target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (_target != null)
		{
			base.Body.ChangeDirection(_target, 100);
			int num = base.Game.Random.Next(0, _personAttack.Length);
			base.Body.Say(_personAttack[num], 1, 1000);
			base.Body.PlayMovie("beatA", 1200, 0);
			((PVEGame)base.Game).SendObjectFocus(_target, 1, 3200, 0);
			base.Body.CallFuction(CreateAttackEffect, 4000);
			if (base.Body.FindDirection(_target) == -1)
			{
				base.Body.RangeAttacking(_target.X - 50, base.Body.X, "cry", 4800, null);
			}
			else
			{
				base.Body.RangeAttacking(base.Body.X, _target.X + 50, "cry", 4800, null);
			}
			base.Body.CallFuction(SetState, 6000);
		}
	}

	private void AttackAllPlayer()
	{
		int num = base.Game.Random.Next(0, _allAttackPlayerSay.Length);
		base.Body.Say(_allAttackPlayerSay[num], 1, 1000);
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4100, null);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			allLivingPlayer.AddEffect(new ReduceStrengthEffect(1, m_reduceStreng), 4200);
		}
		base.Body.CallFuction(SetState, 5000);
	}

	private void ChangeAtoB()
	{
		base.Body.PlayMovie("beatD", 1000, 0);
		int num = base.Game.Random.Next(0, _fearSay.Length);
		base.Body.Say(_fearSay[num], 1, 2000);
		base.Body.PlayMovie("AtoB", 4000, 0);
		base.Body.CallFuction(SetState, 7000);
	}

	private void ChangeBtoA()
	{
		base.Body.PlayMovie("BtoA", 1000, 0);
		int num = base.Game.Random.Next(0, _specialAttackSay.Length);
		base.Body.Say(_specialAttackSay[num], 1, 2000);
		base.Body.CallFuction(JumpAndAttack, 3000);
	}

	private void JumpAndAttack()
	{
		_target = base.Game.FindRandomPlayer();
		if (_target != null)
		{
			base.Body.PlayMovie("jump", 500, 0);
			((PVEGame)base.Game).SendObjectFocus(_target, 1, 2000, 0);
			base.Body.BoltMove(_target.X, _target.Y, 2500);
			base.Body.PlayMovie("fall", 2600, 0);
			base.Body.RangeAttacking(_target.X - 100, _target.X + 100, "cry", 3000, null);
			base.Body.CallFuction(SetState, 4000);
		}
	}

	private void PowerUpEffect()
	{
		_mCurrentPowDamage += m_perPowerUp;
		base.Body.CurrentDamagePlus = _mCurrentPowDamage;
		_mPowerUpEffect = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y - 60, "", "game.crazytank.assetmap.Buff_powup", "", 1, 0);
	}

	private void CreateAttackEffect()
	{
		if (_target != null)
		{
			_mEffectAttack = ((PVEGame)base.Game).Createlayer(_target.X, _target.Y, "", "asset.game.4.blade", "", 1, 0);
		}
	}

	private void ClearEffect()
	{
		if (_mPowerUpEffect != null)
		{
			base.Game.RemovePhysicalObj(_mPowerUpEffect, sendToClient: true);
		}
		if (_mEffectAttack != null)
		{
			base.Game.RemovePhysicalObj(_mEffectAttack, sendToClient: true);
		}
	}

	private void CallNpc()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.IsFly = true;
		livingConfig.CanCountKill = false;
		for (int i = 0; i < m_totalNpc; i++)
		{
			int x = base.Game.Random.Next(350, 1300);
			int y = base.Game.Random.Next(100, 700);
			((SimpleBoss)base.Body).CreateChild(npcId, x, y, 0, -1, showBlood: true, livingConfig);
		}
	}

	private void SetState()
	{
		((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", _isFear ? "standB" : "standA");
	}

	public override void OnShootedSay()
	{
		base.OnShootedSay();
		int num = base.Game.Random.Next(0, _onShootedChat.Length);
		if (!_isSay && base.Body.IsLiving)
		{
			base.Body.Say(_onShootedChat[num], 1, 2000, 0);
			_isSay = true;
		}
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		((SimpleBoss)base.Body).Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg30"), 1, 1000);
	}

	public override void OnDiedSay()
	{
		base.OnDiedSay();
		int num = base.Game.Random.Next(0, _diedChat.Length);
		base.Body.Say(_diedChat[num], 1, 0, 1500);
	}
}

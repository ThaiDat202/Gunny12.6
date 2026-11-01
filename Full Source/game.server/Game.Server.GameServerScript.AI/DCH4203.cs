using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class DCH4203 : AMissionControl
{
	private SimpleBoss _mKing;

	private SimpleBoss _mKing2;

	private int _mStep = 1;

	private int bossID = 4208;

	private int bossID2 = 4209;

	private int npcId = 4207;

	private int _kill = 0;

	private PhysicalObj _mMoive;

	private PhysicalObj _mFront;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1750)
		{
			return 3;
		}
		if (score > 1675)
		{
			return 2;
		}
		if (score > 1600)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[3] { bossID, bossID2, npcId };
		int[] npcIds2 = new int[2] { bossID, bossID2 };
		base.Game.AddLoadingFile(2, "image/game/effect/4/power.swf", "game.crazytank.assetmap.Buff_powup");
		base.Game.AddLoadingFile(2, "image/game/effect/4/blade.swf", "asset.game.4.blade");
		base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.tingyuanlieshouAsset");
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1144);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsShield = true;
		_mMoive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		_mFront = base.Game.Createlayer(1019, 620, "front", "game.asset.living.emozhanshiAsset", "out", 1, 0);
		_mKing = base.Game.CreateBoss(bossID, 1255, 958, -1, 1, "born", livingConfig);
		base.Game.SendLivingActionMapping(_mKing, "die", "standA");
		_mKing.CallFuction(MovieCreateBoss, 1000);
	}

	private void MovieCreateBoss()
	{
		base.Game.SendObjectFocus(_mKing, 1, 500, 0);
		_mKing.PlayMovie("in", 2000, 0);
		base.Game.SendObjectFocus(_mKing, 2, 2000, 3000);
		_mKing.PlayMovie("standA", 9000, 0);
		_mKing.Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourNormalCattleBoss.msg1"), 0, 9200);
		_mMoive.PlayMovie("in", 15000, 0);
		_mFront.PlayMovie("in", 15200, 0);
		_mMoive.PlayMovie("out", 18200, 0);
		_mFront.PlayMovie("out", 18000, 0);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (_mStep < 2 || _mKing2 == null || _mKing2.IsLiving || _mKing == null || _mKing.IsLiving)
		{
			base.Game.TotalKillCount = 0;
		}
		if (base.Game.TurnIndex > 1)
		{
			if (_mMoive != null)
			{
				base.Game.RemovePhysicalObj(_mMoive, sendToClient: true);
				_mMoive = null;
			}
			if (_mFront != null)
			{
				base.Game.RemovePhysicalObj(_mFront, sendToClient: true);
				_mFront = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		if (_mKing2 != null && !_mKing2.IsLiving && _mStep >= 2)
		{
			return true;
		}
		if (base.Game.TurnIndex >= base.Game.MissionInfo.TotalTurn)
		{
			return true;
		}
		if (_mStep <= 1 && _mKing != null && !_mKing.IsLiving)
		{
			_mStep++;
			_mKing2 = base.Game.CreateBoss(bossID2, _mKing.X, _mKing.Y, _mKing.Direction, 1, "born");
			base.Game.RemoveLiving(_mKing.Id);
			_mKing2.Say(LanguageMgr.GetTranslation("GameServerScript.AI.NPC.FourHardFrantCattleBoss.msg1"), 0, 1200);
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (_mStep >= 2 && _mKing2 != null && !_mKing2.IsLiving && _mKing != null && !_mKing.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

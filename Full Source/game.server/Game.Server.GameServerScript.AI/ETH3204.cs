using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class ETH3204 : AMissionControl
{
	private PhysicalObj _mKingMoive;

	private PhysicalObj _mKingFront;

	private SimpleBoss _firstBoss;

	private SimpleBoss _secondBoss;

	private int _mState = 3216;

	private const int FirstBossId = 3216;

	private const int SecondBossId = 3217;

	private const int BloodNpcId = 3213;

	private const int NpcId = 3203;

	private const int BlowNpcId = 3212;

	private const int BloomSNpcId = 3218;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1870)
		{
			return 3;
		}
		if (score > 1825)
		{
			return 2;
		}
		if (score > 1780)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(1, "bombs/53.swf", "tank.resource.bombs.Bomb53");
		base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
		base.Game.AddLoadingFile(1, "bombs/55.swf", "tank.resource.bombs.Bomb55");
		base.Game.AddLoadingFile(2, "image/map/1126/object/1126object.swf", "game.assetmap.Flame");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanLeaderAsset");
		int[] npcIds = new int[6] { 3213, 3216, 3217, 3203, 3212, 3218 };
		int[] npcIds2 = new int[1] { 3216 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1126);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		_mKingMoive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		_mKingFront = base.Game.Createlayer(570, 300, "font", "game.asset.living.ClanLeaderAsset", "out", 1, 0);
		_firstBoss = base.Game.CreateBoss(3216, 745, 410, -1, 1, "");
		_firstBoss.FallFrom(745, 578, "fall", 0, 2, 1000);
		_firstBoss.Config.CanCountKill = false;
		_firstBoss.FallFrom(745, 578, "fall", 0, 2, 1000);
		_firstBoss.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.TNSM3106.msg1"), 0, 3000);
		_firstBoss.PlayMovie("", 3000, 5000);
		_mKingMoive.PlayMovie("in", 10000, 0);
		_mKingFront.PlayMovie("in", 10000, 0);
		_mKingMoive.PlayMovie("out", 15000, 0);
		_mKingFront.PlayMovie("out", 15400, 0);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (_mKingMoive != null)
		{
			base.Game.RemovePhysicalObj(_mKingMoive, sendToClient: true);
			_mKingMoive = null;
		}
		if (_mKingFront != null)
		{
			base.Game.RemovePhysicalObj(_mKingFront, sendToClient: true);
			_mKingFront = null;
		}
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (!_firstBoss.IsLiving && _mState == 3216)
		{
			_mState++;
		}
		if (_mState == 3217 && _secondBoss == null)
		{
			_secondBoss = base.Game.CreateBoss(_mState, _firstBoss.X, _firstBoss.Y, _firstBoss.Direction, 1, "");
			base.Game.RemoveLiving(_firstBoss.Id);
		}
		return (_secondBoss != null && !_secondBoss.IsLiving) || base.Game.TurnIndex >= base.Game.TotalTurn;
	}

	public override int UpdateUIData()
	{
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		base.Game.IsWin = _secondBoss != null && !_secondBoss.IsLiving;
	}
}

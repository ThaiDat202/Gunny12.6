using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class YS34001 : AMissionControl
{
	private SimpleBoss yellow = null;

	private SimpleBoss red = null;

	private SimpleBoss blue = null;

	private SimpleBoss boss = null;

	private int bossID = 34005;

	private int bossID1 = 34006;

	private int bossID2 = 34007;

	private int bossID3 = 34012;

	private int CountBossTurn = 0;

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
		int[] npcIds = new int[4] { bossID, bossID1, bossID2, bossID3 };
		int[] npcIds2 = new int[4] { bossID, bossID1, bossID2, bossID3 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
		base.Game.AddLoadingFile(1, "bombs/83.swf", "tank.resource.bombs.Bomb83");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.choudanbenbenAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/7/choud.swf", "asset.game.seven.choud");
		base.Game.AddLoadingFile(2, "image/game/effect/7/jinqucd.swf", "asset.game.seven.jinqucd");
		base.Game.AddLoadingFile(2, "image/game/effect/7/du.swf", "asset.game.seven.du");
		base.Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
		base.Game.AddLoadingFile(2, "image/game/effect/7/jinquhd.swf", "asset.game.seven.jinquhd");
		base.Game.SetMap(70034);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		red = base.Game.CreateBoss(bossID, 1602, 515, -1, 1, "");
		yellow = base.Game.CreateBoss(bossID1, 1662, 266, -1, 1, "");
		blue = base.Game.CreateBoss(bossID2, 1581, 805, -1, 1, "");
		boss = base.Game.CreateBoss(bossID3, 433, 313, -1, 1, "born");
		yellow.FallFrom(yellow.X, yellow.Y, "", 0, 0, 2000);
		blue.FallFrom(blue.X, blue.Y, "", 0, 0, 2000);
		red.FallFrom(red.X, red.Y, "", 0, 0, 2000);
		boss.FallFrom(boss.X, boss.Y, "", 0, 0, 2000);
		red.SetRelateDemagemRect(red.NpcInfo.X, red.NpcInfo.Y, red.NpcInfo.Width, red.NpcInfo.Height);
		blue.SetRelateDemagemRect(blue.NpcInfo.X, blue.NpcInfo.Y, blue.NpcInfo.Width, blue.NpcInfo.Height);
		yellow.SetRelateDemagemRect(yellow.NpcInfo.X, yellow.NpcInfo.Y, yellow.NpcInfo.Width, yellow.NpcInfo.Height);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		blue.Delay += 2;
		yellow.Delay++;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.CurrentTurnLiving == null || !(base.Game.CurrentTurnLiving is SimpleBoss))
		{
			return;
		}
		CountBossTurn++;
		switch (CountBossTurn)
		{
		case 1:
			if (blue.IsLiving)
			{
				blue.Properties1 = 1;
			}
			else if (yellow.IsLiving)
			{
				yellow.Properties1 = 1;
			}
			else if (red.IsLiving)
			{
				red.Properties1 = 1;
			}
			break;
		case 4:
			if (yellow.IsLiving)
			{
				yellow.Properties1 = 1;
			}
			else if (red.IsLiving)
			{
				red.Properties1 = 1;
			}
			else if (blue.IsLiving)
			{
				blue.Properties1 = 1;
			}
			break;
		case 7:
			if (red.IsLiving)
			{
				red.Properties1 = 1;
			}
			else if (blue.IsLiving)
			{
				blue.Properties1 = 1;
			}
			else if (yellow.IsLiving)
			{
				yellow.Properties1 = 1;
			}
			break;
		case 9:
			CountBossTurn = 0;
			break;
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (red != null && !red.IsLiving && yellow != null && !yellow.IsLiving && blue != null && !blue.IsLiving && boss != null && !boss.IsLiving)
		{
			return true;
		}
		if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
		{
			return true;
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
		if (red != null && !red.IsLiving && yellow != null && !yellow.IsLiving && blue != null && !blue.IsLiving && boss != null && !boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

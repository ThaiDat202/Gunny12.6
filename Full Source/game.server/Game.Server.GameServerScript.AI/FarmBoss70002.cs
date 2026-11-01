using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class FarmBoss70002 : AMissionControl
{
	private SimpleBoss boss = null;

	private int bossID = 50101;

	private int kill = 0;

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
		if (base.Game.loadBossID > bossID)
		{
			bossID = base.Game.loadBossID;
		}
		int[] npcIds = new int[1] { bossID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/127.swf", "tank.resource.bombs.Bomb127");
		base.Game.AddLoadingFile(2, "image/game/effect/15/380b.swf", "asset.game.fifteen.380b");
		base.Game.SetMap(70002);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(bossID, 969, 526, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		Console.WriteLine(boss.NpcInfo.Name);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (base.Game.GetAllFightPlayers().Count < 2)
		{
			return true;
		}
		if (boss != null && !boss.IsLiving)
		{
			kill++;
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (boss != null && !boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

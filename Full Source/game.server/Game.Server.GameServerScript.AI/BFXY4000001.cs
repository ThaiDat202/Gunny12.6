using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class BFXY4000001 : AMissionControl
{
	private SimpleBoss boss = null;

	private int npcID = 404012;

	private int npcID1 = 404013;

	private int npcID2 = 404014;

	private int bossID = 404011;

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
		int[] npcIds = new int[4] { bossID, npcID, npcID1, npcID2 };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut11424.swf", "bullet11424");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet11424.swf", "blastOutMovie11424");
		base.Game.SetMap(70063);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "4000001";
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsFly = true;
		boss = base.Game.CreateBoss(bossID, 2080, 848, -1, 1, "", livingConfig);
		boss.SetRelateDemagemRect(-189, -400, 387, 396);
	}

	public override void OnNewTurnStarted()
	{
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex <= 1)
		{
		}
	}

	public override bool CanGameOver()
	{
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
		if (!boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

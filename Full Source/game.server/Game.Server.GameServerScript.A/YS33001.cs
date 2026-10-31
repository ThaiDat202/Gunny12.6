using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class YS33001 : AMissionControl
{
	private SimpleBoss boss = null;

	private int npcID = 33007;

	private int npcID1 = 33006;

	private int npcID2 = 33005;

	private int bossID = 33104;

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
		base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut51.swf", "shootMovie51");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet51.swf", "bullet51");
		base.Game.SetMap(70032);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "33001";
		boss = base.Game.CreateBoss(bossID, 1616, 565, -1, 1, "");
		boss.SetRelateDemagemRect(-42, -200, 84, 194);
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

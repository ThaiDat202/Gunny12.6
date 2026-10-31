using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class AoAnhDoiCatS71152 : AMissionControl
{
	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	public LivingConfig config = null;

	private int dieRedCount = 0;

	private int npcIDs = 71158;

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
		int[] npcIds = new int[1] { npcIDs };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(12015);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "16401";
		config = base.Game.BaseConfig();
		config.IsFly = true;
		int x = base.Game.Random.Next(671, 1235);
		int y = base.Game.Random.Next(20, 360);
		someNpc.Add(base.Game.CreateNpc(npcIDs, x, y, 1, -1, config));
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || base.Game.GetLivedLivings().Count >= 20)
		{
			return;
		}
		for (int i = 0; i < 20 - base.Game.GetLivedLivings().Count; i++)
		{
			if (someNpc.Count == base.Game.MissionInfo.TotalCount)
			{
				break;
			}
			int x = base.Game.Random.Next(671, 1235);
			int y = base.Game.Random.Next(20, 360);
			if (GetNpcCountByID(npcIDs) < 20)
			{
				someNpc.Add(base.Game.CreateNpc(npcIDs, x, y, 1, 1, config));
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		bool flag = true;
		base.CanGameOver();
		dieRedCount = 0;
		foreach (SimpleNpc item in someNpc)
		{
			if (item.IsLiving)
			{
				flag = false;
			}
			else
			{
				dieRedCount++;
			}
		}
		if (flag && dieRedCount == base.Game.MissionInfo.TotalCount)
		{
			base.Game.IsWin = true;
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.IsWin = true;
			List<LoadingFileInfo> list = new List<LoadingFileInfo>();
			list.Add(new LoadingFileInfo(2, "image/map/20/show2", ""));
			base.Game.SendLoadResource(list);
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	protected int GetNpcCountByID(int Id)
	{
		int num = 0;
		foreach (SimpleNpc item in someNpc)
		{
			if (item.NpcInfo.ID == Id)
			{
				num++;
			}
		}
		return num;
	}
}

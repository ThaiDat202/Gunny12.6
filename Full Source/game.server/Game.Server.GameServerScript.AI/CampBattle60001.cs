using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class CampBattle60001 : AMissionControl
{
	private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

	private int npcId = 60001;

	private int totalNpcSpawn = 0;

	private int deadCount = 0;

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
		int[] npcIds = new int[1] { npcId };
		int[] npcIds2 = new int[1] { npcId };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(11018);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		for (int i = 0; i < 4; i++)
		{
			totalNpcSpawn++;
			if (i < 1)
			{
				SomeNpc.Add(base.Game.CreateNpc(npcId, 888, 568, 0, -1));
			}
			else if (i < 3)
			{
				SomeNpc.Add(base.Game.CreateNpc(npcId, 995, 518, 0, -1));
			}
			else
			{
				SomeNpc.Add(base.Game.CreateNpc(npcId, 1087, 572, 0, -1));
			}
		}
		totalNpcSpawn++;
		SomeNpc.Add(base.Game.CreateNpc(npcId, 1221, 587, 0, -1));
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		if (base.Game.TurnIndex <= 1 || (base.Game.CurrentPlayer != null && base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay))
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (totalNpcSpawn < 20)
			{
				totalNpcSpawn++;
				if (i < 1)
				{
					SomeNpc.Add(base.Game.CreateNpc(npcId, 888, 568, 0, -1));
				}
				else if (i < 3)
				{
					SomeNpc.Add(base.Game.CreateNpc(npcId, 995, 518, 0, -1));
				}
				else
				{
					SomeNpc.Add(base.Game.CreateNpc(npcId, 1087, 572, 0, -1));
				}
			}
		}
		if (totalNpcSpawn < 20)
		{
			totalNpcSpawn++;
			SomeNpc.Add(base.Game.CreateNpc(npcId, 1221, 587, 0, -1));
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		bool flag = true;
		deadCount = 0;
		foreach (SimpleNpc item in SomeNpc)
		{
			if (item.IsLiving)
			{
				flag = false;
			}
			else
			{
				deadCount++;
			}
		}
		if (flag && deadCount == 20)
		{
			base.Game.IsWin = true;
			return true;
		}
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
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
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdateCampBattleResult(base.Game.GameType, base.Game.BossCampId, allFightPlayers.Count, base.Game.IsWin);
		}
	}
}

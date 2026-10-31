using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class LauDaiPhepThuatS71055 : AMissionControl
{
	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	public LivingConfig config = null;

	private int dieRedCount = 0;

	private int[] npcIDs = new int[2] { 71056, 71055 };

	private int[] birthX = new int[4] { 635, 750, 900, 1115 };

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
		int[] npcIds = new int[2]
		{
			npcIDs[0],
			npcIDs[1]
		};
		int[] npcIds2 = new int[4]
		{
			npcIDs[1],
			npcIDs[0],
			npcIDs[0],
			npcIDs[0]
		};
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1464);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "16401";
		config = base.Game.BaseConfig();
		config.IsFly = true;
		int num = base.Game.Random.Next(0, npcIDs.Length);
		int x = base.Game.Random.Next(572, 1115);
		int y = base.Game.Random.Next(510, 650);
		if (npcIDs[num] == 71055)
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1, config));
		}
		else
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1));
		}
		num = base.Game.Random.Next(0, npcIDs.Length);
		x = base.Game.Random.Next(572, 1115);
		y = base.Game.Random.Next(510, 650);
		if (npcIDs[num] == 71055)
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1, config));
		}
		else
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1));
		}
		num = base.Game.Random.Next(0, npcIDs.Length);
		x = base.Game.Random.Next(572, 1115);
		y = base.Game.Random.Next(510, 650);
		if (npcIDs[num] == 71055)
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1, config));
		}
		else
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1));
		}
		num = base.Game.Random.Next(0, npcIDs.Length);
		x = base.Game.Random.Next(572, 1115);
		y = base.Game.Random.Next(510, 650);
		if (npcIDs[num] == 71055)
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1, config));
		}
		else
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1));
		}
		num = base.Game.Random.Next(0, npcIDs.Length);
		x = base.Game.Random.Next(572, 1115);
		y = base.Game.Random.Next(510, 650);
		if (npcIDs[num] == 71055)
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1, config));
		}
		else
		{
			someNpc.Add(base.Game.CreateNpc(npcIDs[num], x, y, 1, -1));
		}
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || base.Game.GetLivedLivings().Count >= 10)
		{
			return;
		}
		for (int i = 0; i < 10 - base.Game.GetLivedLivings().Count; i++)
		{
			if (someNpc.Count == base.Game.MissionInfo.TotalCount)
			{
				break;
			}
			int num = base.Game.Random.Next(0, birthX.Length);
			int num2 = birthX[num];
			int x = base.Game.Random.Next(572, 1115);
			int y = base.Game.Random.Next(510, 650);
			num = base.Game.Random.Next(0, npcIDs.Length);
			if (num == 1 && GetNpcCountByID(npcIDs[1]) < 10)
			{
				someNpc.Add(base.Game.CreateNpc(npcIDs[1], x, y, 1, 1, config));
				continue;
			}
			x = base.Game.Random.Next(1164, 1465);
			someNpc.Add(base.Game.CreateNpc(npcIDs[0], x, 911, 1, 1));
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

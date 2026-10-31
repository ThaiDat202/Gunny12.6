using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class WCH14202 : AMissionControl
{
	private SimpleNpc goal = null;

	private SimpleBoss boss = null;

	private int npcID = 14103;

	private int chillID = 14104;

	private int bossID = 14105;

	private int goalID = 14106;

	private int kill = 0;

	private int[] birthX = new int[3] { 1650, 1590, 1530 };

	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

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
		int[] array = new int[4] { bossID, npcID, goalID, chillID };
		int[] npcIds = array;
		base.Game.LoadResources(array);
		base.Game.AddLoadingFile(1, "bombs/109.swf", "tank.resource.bombs.Bomb109");
		base.Game.AddLoadingFile(1, "bombs/14005.swf", "tank.resource.bombs.Bomb14005");
		base.Game.AddLoadingFile(2, "image/game/living/living371.swf", "game.living.Living371");
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1406);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.AddAction(new CallFunctionAction(AddBoss, 2000));
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsHelper = true;
		livingConfig.HasTurn = false;
		goal = base.Game.CreateNpc(goalID, 117, 720, 1, 1, livingConfig);
		base.Game.SendGameFocus(goal, 0, 1000);
	}

	private void AddBoss()
	{
		boss = base.Game.CreateBoss(bossID, 1816, 861, -1, 1, "");
		base.Game.SendGameFocus(boss, 0, 1000);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		base.Game.AddAction(new CallFunctionAction(AddNpc, 3000));
	}

	private void AddNpc()
	{
		for (int i = 0; i < birthX.Length; i++)
		{
			someNpc.Add(base.Game.CreateNpc(npcID, birthX[i], 790, 1, -1));
		}
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		int num = 0;
		foreach (SimpleNpc item in someNpc)
		{
			if (!item.IsLiving)
			{
				num++;
			}
		}
		if (someNpc.Count - num < 3)
		{
			for (int i = 0; i < num; i++)
			{
				int num2 = base.Game.Random.Next(birthX.Length);
				someNpc.Add(base.Game.CreateNpc(npcID, birthX[num2], 790, 1, -1));
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (base.Game.IsMissBall)
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
		if (boss != null && !boss.IsLiving && !base.Game.IsMissBall)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

using System.Collections.Generic;
using Bussiness.Managers;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Server.GameServerScript.AI.Messions;

public class WCT14303 : AMissionControl
{
	private SimpleNpc npc = null;

	private SimpleNpc goal = null;

	private SimpleBoss boss = null;

	private SimpleNpc arbiNpc = null;

	private int npcID = 14208;

	private int bossID = 14209;

	private int goalID = 14206;

	private int arbiID = 14207;

	private int needGoal = 10;

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
		int[] array = new int[4] { bossID, npcID, goalID, arbiID };
		int[] npcIds = array;
		base.Game.LoadResources(array);
		base.Game.AddLoadingFile(1, "bombs/110.swf", "tank.resource.bombs.Bomb110");
		base.Game.AddLoadingFile(2, "image/game/effect/0/294b.swf", "asset.game.zero.294b");
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1407);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.AddAction(new CallFunctionAction(AddArbiNpc, 2000));
	}

	private void AddArbiNpc()
	{
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsHelper = true;
		livingConfig.HasTurn = false;
		arbiNpc = base.Game.CreateNpc(arbiID, 785, 261, 1, 1, livingConfig);
		base.Game.SendGameFocus(arbiNpc, 0, 100);
		arbiNpc.SetRelateDemagemRect(0, 0, 0, 0);
		LivingConfig livingConfig2 = base.Game.BaseConfig();
		livingConfig2.HasTurn = false;
		livingConfig2.IsGoal = true;
		goal = base.Game.CreateNpc(goalID, 1469, 860, 1, -1, livingConfig2);
		goal.SetRelateDemagemRect(-56, -110, 110, 110);
		base.Game.AddAction(new CallFunctionAction(AddBoss, 2000));
	}

	private void AddBoss()
	{
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsShield = true;
		livingConfig.KeepLife = true;
		boss = base.Game.CreateBoss(bossID, 1223, 861, -1, 1, "", livingConfig);
		base.Game.SendGameFocus(boss, 0, 1000);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		boss.PlayMovie("standB", 2000, 0);
		base.Game.AddAction(new CallFunctionAction(ProtectGoal, 3000));
	}

	private void ProtectGoal()
	{
		goal.PlayMovie("beatA", 1000, 0);
		base.Game.AddAction(new CallFunctionAction(AddNpc, 1000));
		boss.AddDelay(2500);
	}

	private void AddNpc()
	{
		base.Game.SendGameFocus(arbiNpc, 0, 1000);
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsFly = true;
		int x = base.Game.Random.Next(225, 1115);
		int y = base.Game.Random.Next(113, 354);
		for (int i = 0; i < 10; i++)
		{
			someNpc.Add(base.Game.CreateNpc(npcID, x, y, 0, -1, livingConfig));
			x = base.Game.Random.Next(225, 1115);
			y = base.Game.Random.Next(113, 354);
		}
	}

	public override void OnStartMovie()
	{
		base.OnStartMovie();
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item != null && item.IsLiving)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(10471);
				ItemInfo cloneItem = ItemInfo.CreateFromTemplate(goods, 1, 101);
				item.PlayerDetail.AddTemplate(cloneItem, eBageType.FightBag, 1, eGameView.OtherTypeGet);
			}
		}
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (boss.Config.IsDown)
		{
			goal.PlayMovie("beatB", 1000, 0);
		}
		if (base.Game.IsGoal && boss.Blood == 1)
		{
			base.Game.AddAction(new CallFunctionAction(Goal, 1000));
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.CanBeginNextProtect && boss.Blood == boss.NpcInfo.Blood)
		{
			base.Game.PveGameDelay = 0;
			ProtectGoal();
			base.Game.CanBeginNextProtect = false;
		}
	}

	public override void OnShooted()
	{
		base.OnShooted();
		if (base.Game.IsGoal && boss.Config.IsDown)
		{
			base.Game.TotalGoal++;
		}
	}

	private void Goal()
	{
		base.Game.SendGameFocus(arbiNpc, 0, 2000);
		arbiNpc.Say($"Vào, tỉ số là {base.Game.TotalGoal}-0", 1, 100);
		base.Game.IsGoal = false;
	}

	public override bool CanGameOver()
	{
		if (needGoal <= base.Game.TotalGoal || base.Game.IsMissBall)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return base.Game.TotalGoal;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (needGoal <= base.Game.TotalGoal && !base.Game.IsMissBall)
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
			item?.PlayerDetail.ClearFightBag();
		}
	}
}

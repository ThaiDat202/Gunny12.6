using System.Collections.Generic;
using Bussiness.Managers;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Server.GameServerScript.AI.Messions;

public class WCT14301 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleNpc ball = null;

	private int bossID = 14202;

	private int ballID = 14201;

	private int holdTurn = 40;

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
		int[] array = new int[2] { bossID, ballID };
		int[] npcIds = array;
		base.Game.LoadResources(array);
		base.Game.AddLoadingFile(1, "bombs/110.swf", "tank.resource.bombs.Bomb110");
		base.Game.AddLoadingFile(1, "bombs/117.swf", "tank.resource.bombs.Bomb117");
		base.Game.AddLoadingFile(2, "image/game/effect/9/biaoji.swf", "asset.game.nine.biaoji");
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1405);
	}

	public override void OnStartMovie()
	{
		base.OnStartMovie();
		int[] array = new int[3] { 10467, 10468, 10469 };
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item != null && item.IsLiving)
			{
				for (int i = 0; i < array.Length; i++)
				{
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(array[i]);
					ItemInfo cloneItem = ItemInfo.CreateFromTemplate(goods, 1, 101);
					item.PlayerDetail.AddTemplate(cloneItem, eBageType.FightBag, 1, eGameView.OtherTypeGet);
				}
			}
		}
		if (allFightPlayers.Count > 2)
		{
			holdTurn = 50;
		}
		base.Game.PassBallActive = true;
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(bossID, 1277, 812, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsHelper = true;
		livingConfig.HasTurn = false;
		ball = base.Game.CreateNpc(ballID, 638, 926, 1, -1, livingConfig);
		boss.AddDelay(2500);
	}

	public override void OnShooted()
	{
		base.OnShooted();
		Player currentPlayer = base.Game.CurrentPlayer;
		if (currentPlayer != null && currentPlayer.PassBallFail)
		{
			LivingConfig livingConfig = base.Game.BaseConfig();
			livingConfig.IsHelper = true;
			livingConfig.HasTurn = false;
			if (currentPlayer != null && !currentPlayer.LastPoint.IsEmpty)
			{
				ball = base.Game.CreateNpc(ballID, currentPlayer.LastPoint.X, currentPlayer.LastPoint.Y, 1, -1, livingConfig);
			}
			int num = ball.X + 150 * currentPlayer.Direction;
			if (num > base.Game.Map.Info.ForegroundWidth)
			{
				num = base.Game.Map.Info.ForegroundWidth;
			}
			if (num < 1)
			{
				num = 1;
			}
			ball.MoveTo(num, ball.Y, "walk", 0, 3);
		}
	}

	public override void OnMoving()
	{
		base.OnMoving();
		Player currentPlayer = base.Game.CurrentPlayer;
		if (currentPlayer != null && !currentPlayer.IsHoldBall && ball != null && currentPlayer.X < ball.X + 25 && currentPlayer.X > ball.X - 25)
		{
			base.Game.TakePassBall(currentPlayer.Id);
			base.Game.RemoveNPC(ball.Id);
		}
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
		if (holdTurn <= base.Game.TurnIndex || base.Game.IsMissBall)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return holdTurn;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (holdTurn <= base.Game.TurnIndex && !base.Game.IsMissBall)
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
		base.Game.PassBallActive = false;
	}
}

using System.Collections.Generic;
using System.Drawing;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class GOH6202 : AMissionControl
{
	private SimpleBoss m_boss;

	private SimpleBoss m_hero;

	private PhysicalObj m_wall;

	private int bossID = 6223;

	private int blueID = 6222;

	private int redID = 6221;

	private int homeID = 6224;

	private int heroID = 6214;

	private int m_maxPoint = 11;

	private SimpleNpc m_tempNpc;

	private int m_finishRedCount = 0;

	private int m_finishBlueCount = 0;

	private List<Point> m_point = new List<Point>
	{
		new Point(620, 1080)
	};

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 900)
		{
			return 3;
		}
		if (score > 825)
		{
			return 2;
		}
		if (score > 725)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(2, "image/game/effect/6/danti.swf", "asset.game.six.danti");
		base.Game.AddLoadingFile(2, "image/game/effect/6/qunti.swf", "asset.game.six.qunti");
		base.Game.AddLoadingFile(2, "image/game/effect/6/zhaozi.swf", "asset.game.six.zhaozi");
		base.Game.AddLoadingFile(2, "image/game/effect/6/danjia.swf", "asset.game.six.danjia");
		base.Game.AddLoadingFile(2, "image/game/effect/6/qunjia.swf", "asset.game.six.qunjia");
		int[] npcIds = new int[5] { bossID, redID, blueID, homeID, heroID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1166);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_boss = base.Game.CreateBoss(bossID, 1910, 1080, -1, 1, "");
		m_boss.Delay = 1;
		m_hero = base.Game.CreateBoss(heroID, 460, 1080, -1, 1, "");
		m_hero.Config.CanTakeDamage = false;
		m_hero.Config.HasTurn = false;
		m_hero.MoveTo(450, 1080, "walk", 500, 3);
		m_hero.PlayMovie("go", 1500, 0);
		m_hero.Say("Tấn công đội xanh, bảo vệ đội đỏ.", 0, 2000);
		m_wall = base.Game.CreateLayerBoss(1100, 1080, "font", "game.living.Living190", "stand", 1, 0);
		m_boss.CallFuction(CreateNpcBlue, 2500);
		base.Game.PveGameDelay = 0;
	}

	private void CreateNpcBlue()
	{
		m_maxPoint = 10;
		m_tempNpc = base.Game.CreateNpc(blueID, 300, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.MaxStepMove = 6;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(blueID, 250, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.MaxStepMove = 6;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(blueID, 200, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.MaxStepMove = 6;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(blueID, 150, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.MaxStepMove = 6;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(blueID, 100, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.MaxStepMove = 6;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_maxPoint = 9;
		m_tempNpc = base.Game.CreateNpc(redID, 50, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.IsHelper = true;
		m_tempNpc.Config.MaxStepMove = 4;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(redID, 0, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.IsHelper = true;
		m_tempNpc.Config.MaxStepMove = 4;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(redID, -50, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.IsHelper = true;
		m_tempNpc.Config.MaxStepMove = 4;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(redID, -100, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.IsHelper = true;
		m_tempNpc.Config.MaxStepMove = 4;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
		m_tempNpc = base.Game.CreateNpc(redID, -150, 1080, 1, 1);
		m_tempNpc.Config.IsFly = true;
		m_tempNpc.Config.IsHelper = true;
		m_tempNpc.Config.MaxStepMove = 4;
		m_tempNpc.Config.FirstStepMove = m_maxPoint;
		m_tempNpc.MoveTo(m_point[0].X, m_point[0].Y, "walk", 0, 8);
		m_maxPoint -= 2;
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		SimpleNpc[] array = base.Game.FindAllNpcLiving();
		m_finishRedCount = 0;
		m_finishBlueCount = 0;
		SimpleNpc[] array2 = array;
		foreach (SimpleNpc simpleNpc in array2)
		{
			if (simpleNpc.Config.CompleteStep)
			{
				if (simpleNpc.NpcInfo.ID == redID)
				{
					m_finishRedCount++;
				}
				else
				{
					m_finishBlueCount++;
				}
			}
		}
		if (m_finishRedCount >= 5 || m_finishBlueCount >= 5)
		{
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
		if (m_finishRedCount >= 5)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

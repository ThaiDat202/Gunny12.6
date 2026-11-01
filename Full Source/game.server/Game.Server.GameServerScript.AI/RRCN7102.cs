using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class RRCN7102 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss m_king = null;

	private SimpleBoss king = null;

	private int bossID = 7111;

	private int CountBossTurn = 0;

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
		int[] npcIds = new int[1] { bossID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
		base.Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
		base.Game.AddLoadingFile(2, "image/game/effect/7/jinquhd.swf", "asset.game.seven.jinquhd");
		base.Game.SetMap(1162);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_king = base.Game.CreateBoss(bossID, 1680, 315, -1, 1, "");
		boss = base.Game.CreateBoss(bossID, 1615, 565, -1, 1, "");
		king = base.Game.CreateBoss(bossID, 1600, 849, -1, 1, "");
		boss.FallFrom(boss.X, boss.Y, "", 0, 0, 2000);
		king.FallFrom(king.X, king.Y, "", 0, 0, 2000);
		m_king.FallFrom(m_king.X, m_king.Y, "", 0, 0, 2000);
		m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
		king.SetRelateDemagemRect(king.NpcInfo.X, king.NpcInfo.Y, king.NpcInfo.Width, king.NpcInfo.Height);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		king.Delay += 2;
		boss.Delay++;
		CreateEffectSaying();
	}

	private void CreateEffectSaying()
	{
		base.Game.SendObjectFocus(m_king, 1, 1000, 0);
		m_king.PlayMovie("speak", 1500, 0);
		m_king.Say("Loài người sao lại mò tới đây?", 0, 1500);
		base.Game.SendObjectFocus(boss, 1, 4000, 0);
		boss.PlayMovie("speak", 4500, 0);
		boss.Say("Không cần biết. Tiêu diệt bọn chúng!", 0, 4500);
		base.Game.SendObjectFocus(king, 1, 8000, 0);
		king.PlayMovie("speak", 8500, 0);
		king.Say("Bây giờ bọn ngươi bỏ chạy còn kịp đó.", 0, 8500, 10000);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.CurrentTurnLiving == null || !(base.Game.CurrentTurnLiving is SimpleBoss))
		{
			return;
		}
		CountBossTurn++;
		switch (CountBossTurn)
		{
		case 1:
			if (king.IsLiving)
			{
				king.Properties1 = 1;
			}
			else if (boss.IsLiving)
			{
				boss.Properties1 = 1;
			}
			else if (m_king.IsLiving)
			{
				m_king.Properties1 = 1;
			}
			break;
		case 4:
			if (boss.IsLiving)
			{
				boss.Properties1 = 1;
			}
			else if (m_king.IsLiving)
			{
				m_king.Properties1 = 1;
			}
			else if (king.IsLiving)
			{
				king.Properties1 = 1;
			}
			break;
		case 7:
			if (m_king.IsLiving)
			{
				m_king.Properties1 = 1;
			}
			else if (king.IsLiving)
			{
				king.Properties1 = 1;
			}
			else if (boss.IsLiving)
			{
				boss.Properties1 = 1;
			}
			break;
		case 9:
			CountBossTurn = 0;
			break;
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (m_king != null && !m_king.IsLiving && boss != null && !boss.IsLiving && king != null && !king.IsLiving)
		{
			return true;
		}
		if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
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
		if (m_king != null && !m_king.IsLiving && boss != null && !boss.IsLiving && king != null && !king.IsLiving)
		{
			base.Game.IsWin = true;
			base.Game.SessionId++;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

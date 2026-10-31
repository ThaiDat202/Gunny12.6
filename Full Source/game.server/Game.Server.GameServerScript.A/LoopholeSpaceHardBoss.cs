using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class LoopholeSpaceHardBoss : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss boss2 = null;

	private SimpleBoss m_secondboss = null;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int m_state = 71089;

	private int turn = 0;

	private int firstBossID = 71089;

	private int secondBossID = 71090;

	private int direction;

	private int m_kill = 0;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1150)
		{
			return 3;
		}
		if (score > 925)
		{
			return 2;
		}
		if (score > 700)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[2] { firstBossID, secondBossID };
		int[] npcIds2 = new int[2] { firstBossID, secondBossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut51.swf", "shootMovie51");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet51.swf", "bullet51");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.AntQueenAsset");
		base.Game.SetMap(1469);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(m_state, 910, 800, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		m_moive.PlayMovie("in", 4000, 0);
		m_front.PlayMovie("in", 4100, 0);
		m_moive.PlayMovie("out", 80000, 1000);
		m_front.PlayMovie("out", 9900, 0);
		turn = base.Game.TurnIndex;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > turn + 1)
		{
			if (m_moive != null)
			{
				base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
				m_moive = null;
			}
			if (m_front != null)
			{
				base.Game.RemovePhysicalObj(m_front, sendToClient: true);
				m_front = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		base.CanGameOver();
		if (!boss.IsLiving && m_state == firstBossID)
		{
			m_state++;
		}
		if (m_state == secondBossID && m_secondboss == null)
		{
			LivingConfig livingConfig = base.Game.BaseConfig();
			livingConfig.IsFly = true;
			m_secondboss = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 2, "", livingConfig);
			m_secondboss.SetRelateDemagemRect(m_secondboss.NpcInfo.X, m_secondboss.NpcInfo.Y, m_secondboss.NpcInfo.Width, m_secondboss.NpcInfo.Height);
			base.Game.RemoveLiving(boss.Id);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			Player player = base.Game.FindRandomPlayer();
			int num = 0;
			if (player != null)
			{
				num = player.Delay;
			}
			foreach (Player item in allFightPlayers)
			{
				if (item.Delay < num)
				{
					num = item.Delay;
				}
			}
			m_secondboss.AddDelay(num - 2000);
			turn = base.Game.TurnIndex;
		}
		if (m_secondboss != null && !m_secondboss.IsLiving)
		{
			direction = m_secondboss.Direction;
			m_kill++;
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return m_kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (m_state == secondBossID && !m_secondboss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

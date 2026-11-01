using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class AoAnhDoiCatS71153 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss boss2 = null;

	private SimpleBoss m_secondboss = null;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int m_state = 71160;

	private int turn = 0;

	private int npcID = 71159;

	private int firstBossID = 71160;

	private int secondBossID = 71161;

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
		int[] npcIds = new int[3] { firstBossID, secondBossID, npcID };
		int[] npcIds2 = new int[3] { firstBossID, secondBossID, npcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/11/099.swf", "asset.game.eleven.099");
		base.Game.SetMap(1483);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(m_state, 1240, 900, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
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
			PhysicalObj[] array = base.Game.FindPhysicalObjByName("dangerMoive");
			if (array != null)
			{
				PhysicalObj[] array2 = array;
				foreach (PhysicalObj phy in array2)
				{
					base.Game.RemovePhysicalObj(phy, sendToClient: true);
				}
			}
			m_state++;
		}
		if (m_state == secondBossID && m_secondboss == null)
		{
			LivingConfig livingConfig = base.Game.BaseConfig();
			livingConfig.IsFly = true;
			m_secondboss = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 1, "");
			m_secondboss.SetRelateDemagemRect(m_secondboss.NpcInfo.X, m_secondboss.NpcInfo.Y, m_secondboss.NpcInfo.Width, m_secondboss.NpcInfo.Height);
			base.Game.RemoveLiving(boss.Id);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			Player player = base.Game.FindRandomPlayer();
			base.Game.ClearAllNpc();
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

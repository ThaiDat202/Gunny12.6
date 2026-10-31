using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions
{
	public class QX70001 : AMissionControl
	{
		private SimpleBoss boss = null;

		private SimpleBoss lastBoss = null;

		private SimpleNpc npc = null;

		private LivingConfig config;

		private int bossID = 0;

		private int m_state = 0;

		private int kill = 0;

		private int[] resources = new int[10] { 70001, 70002, 70003, 70006, 70007, 70008, 70009, 70010, 70011, 70099 };

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
			return (score > 1600) ? 1 : 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			base.Game.LoadResources(resources);
			base.Game.LoadNpcGameOverResources(resources);
			base.Game.SetMap(1311);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			base.Game.AddAction(new CallFunctionAction(Addnpc, 2000));
			config = base.Game.BaseConfig();
			config.IsFly = true;
		}

		private void Addnpc()
		{
			config.HasTurn = false;
			npc = base.Game.CreateNpc(resources[9], 220, 530, 0, 1, config);
			base.Game.SendGameFocus(npc, 0, 1000);
			npc.SetRelateDemagemRect(0, 0, 0, 0);
			npc.CallFuction(AddBoss, 3000);
		}

		private void AddBoss()
		{
			m_state = resources[bossID];
			config.HasTurn = true;
			boss = base.Game.CreateBoss(m_state, 1120, 763, -1, 1, "", config);
			base.Game.SendGameFocus(boss, 0, 1000);
			boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			bossID++;
		}

		public override void OnNewTurnStarted()
		{
			base.OnNewTurnStarted();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		private int MinDelay()
		{
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
			return num;
		}

		private void AddNextBoss()
		{
			base.Game.RemoveLiving(boss.Id);
			m_state = resources[bossID];
			config.IsFly = bossID <= 2 || bossID >= 8;
			boss = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 1, "", config);
			if (boss.Direction == 1)
			{
				boss.SetRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			}
			boss.AddDelay(MinDelay() - 2000);
			bossID++;
		}

		private void AddLastBoss()
		{
			base.Game.RemoveLiving(boss.Id);
			m_state = resources[bossID];
			lastBoss = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 1, "");
			if (lastBoss.Direction == 1)
			{
				lastBoss.SetRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
				lastBoss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			}
			lastBoss.AddDelay(MinDelay() - 2000);
		}

		public override bool CanGameOver()
		{
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
			if (!boss.IsLiving && bossID < 8)
			{
				UpdateAward(boss.NpcInfo.DropId, boss.IsLiving);
				AddNextBoss();
			}
			if (m_state == resources[7] && !boss.IsLiving && lastBoss == null)
			{
				UpdateAward(boss.NpcInfo.DropId, boss.IsLiving);
				AddLastBoss();
			}
			if (lastBoss == null || lastBoss.IsLiving)
			{
				return false;
			}
			UpdateAward(lastBoss.NpcInfo.DropId, lastBoss.IsLiving);
			kill++;
			return true;
		}

		private void UpdateAward(int dropID, bool IsLiving)
		{
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.PlayerDetail.UpdatePveResult("qx", dropID, IsLiving);
			}
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return kill;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (lastBoss != null && !lastBoss.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}
	}
}
namespace Game.Server.GameServerScript.AI.Game
{
	public class QX70001 : APVEGameControl
	{
		public override void OnCreated()
		{
			base.Game.SetupMissions("70001");
			base.Game.TotalMissionCount = 1;
		}

		public override void OnPrepated()
		{
			base.Game.SessionId = 0;
		}

		public override int CalculateScoreGrade(int score)
		{
			if (score > 800)
			{
				return 3;
			}
			if (score > 725)
			{
				return 2;
			}
			if (score > 650)
			{
				return 1;
			}
			return 0;
		}

		public override void OnGameOverAllSession()
		{
		}
	}
}

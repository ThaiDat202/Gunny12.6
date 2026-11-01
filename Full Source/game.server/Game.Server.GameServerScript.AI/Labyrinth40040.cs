using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40040 : AMissionControl
	{
		private SimpleBoss boss = null;

		private SimpleBoss boss2 = null;

		private int kill = 0;

		private int m_state = 40065;

		private int turn = 0;

		private int bossID = 40065;

		private int bossID2 = 40066;

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			int result;
			if (score > 1870)
			{
				result = 3;
			}
			else if (score > 1825)
			{
				result = 2;
			}
			else if (score > 1780)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[]
			{
				this.bossID,
				this.bossID2
			};
			int[] npcIds2 = new int[]
			{
				this.bossID,
				this.bossID2
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1318);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.boss = base.Game.CreateBoss(this.bossID, 1169, 606, -1, 1, "");
			this.boss.SetRelateDemagemRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
			this.boss.AddDelay(10);
			this.turn = base.Game.TurnIndex;
		}

		public override void OnNewTurnStarted()
		{
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
		}

		public override bool CanGameOver()
		{
			bool result;
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				result = true;
			}
			else
			{
				if (!this.boss.IsLiving)
				{
					if (this.m_state == this.bossID)
					{
						this.m_state++;
					}
				}
				if (this.m_state == this.bossID2 && this.boss2 == null)
				{
					LivingConfig livingConfig = base.Game.BaseLivingConfig();
					livingConfig.IsFly = true;
					this.boss2 = base.Game.CreateBoss(this.m_state, this.boss.X, 606, this.boss.Direction, 2, "", livingConfig);
					base.Game.RemoveLiving(this.boss.Id);
					if (this.boss2.Direction == 1)
					{
						this.boss2.SetRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
					}
					this.boss2.SetRelateDemagemRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
					List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
					Player player = base.Game.FindRandomPlayer();
					int num = 0;
					if (player != null)
					{
						num = player.Delay;
					}
					foreach (Player current in allFightPlayers)
					{
						if (current.Delay < num)
						{
							num = current.Delay;
						}
					}
					this.boss2.AddDelay(num - 2000);
					this.turn = base.Game.TurnIndex;
				}
				if (this.boss2 != null && !this.boss2.IsLiving)
				{
					if (base.Game.CanEnterGate)
					{
						result = true;
						return result;
					}
					this.kill++;
					base.Game.CanShowBigBox = true;
				}
				result = false;
			}
			return result;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return this.kill;
		}

		public override void OnGameOverMovie()
		{
			base.OnGameOverMovie();
			if (this.boss2 != null && !this.boss2.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
		}
	}
}

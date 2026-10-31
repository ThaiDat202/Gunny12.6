using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40026 : AMissionControl
	{
		private int bossID = 40039;

		private int bossID2 = 40040;

		private int bossID3 = 40038;

		private int npcIDl = 40044;

		private int npcIDr = 40043;

		private int npcID2 = 40045;

		private int npcID3 = 40046;

		private SimpleBoss boss;

		private SimpleBoss boss2;

		private SimpleBoss boss3;

		private int kill = 0;

		private int m_state = 40041;

		private int turn = 0;

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
				this.bossID2,
				this.bossID3,
				this.npcIDl,
				this.npcIDr,
				this.npcID2,
				this.npcID3
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut51.swf", "bullet51");
			base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet51.swf", "bullet51");
			base.Game.SetMap(1280);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			this.boss = base.Game.CreateBoss(this.bossID, 1316, 444, -1, 1, "", livingConfig);
			this.boss.SetRelateDemagemRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
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
				if (!this.boss.IsLiving && this.boss2 == null)
				{
					LivingConfig livingConfig = base.Game.BaseLivingConfig();
					livingConfig.IsFly = true;
					this.boss2 = base.Game.CreateBoss(this.bossID2, this.boss.X, this.boss.Y, this.boss.Direction, 2, "", livingConfig);
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
				if (this.boss2 != null && !this.boss2.IsLiving && this.boss3 == null)
				{
					LivingConfig livingConfig = base.Game.BaseLivingConfig();
					livingConfig.IsFly = true;
					this.boss3 = base.Game.CreateBoss(this.bossID3, this.boss.X, this.boss.Y, this.boss.Direction, 2, "", livingConfig);
					base.Game.RemoveLiving(this.boss2.Id);
					if (this.boss3.Direction == 1)
					{
						this.boss3.SetRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
					}
					this.boss3.SetRelateDemagemRect(this.boss.NpcInfo.X, this.boss.NpcInfo.Y, this.boss.NpcInfo.Width, this.boss.NpcInfo.Height);
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
					this.boss3.AddDelay(num - 2000);
					this.turn = base.Game.TurnIndex;
				}
				if (this.boss3 != null && !this.boss3.IsLiving)
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
			if (this.boss3 != null && !this.boss3.IsLiving)
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

using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40032 : AMissionControl
	{
		private SimpleBoss boss = null;

		private int kill = 0;

		private int npcID = 2004;

		private int bossID = 40052;

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
				this.npcID
			};
			int[] npcIds2 = new int[]
			{
				this.bossID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut51.swf", "bullet51");
			base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet51.swf", "bullet51");
			base.Game.SetMap(12007);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			this.boss = base.Game.CreateBoss(this.bossID, 972, 444, -1, 1, "", livingConfig);
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
				if (this.boss != null && !this.boss.IsLiving)
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
			if (this.boss != null && !this.boss.IsLiving)
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

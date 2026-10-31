using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40012 : AMissionControl
	{
		private SimpleBoss boss1 = null;

		private SimpleBoss boss2 = null;

		private int kill = 0;

		private int bossID1 = 40015;

		private int bossID2 = 40016;

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
				this.bossID1,
				this.bossID2
			};
			int[] npcIds2 = new int[]
			{
				this.bossID1,
				this.bossID2
			};
			base.Game.AddLoadingFile(2, "image/game/effect/4/feather.swf", "asset.game.4.feather");
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1236);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			this.boss1 = base.Game.CreateBoss(this.bossID2, 1200, 900, -1, 1, "");
			this.boss1.FallFromTo(this.boss1.X, this.boss1.Y, null, 0, 0, 2000, null);
			this.boss1.SetRelateDemagemRect(this.boss1.NpcInfo.X, this.boss1.NpcInfo.Y, this.boss1.NpcInfo.Width, this.boss1.NpcInfo.Height);
			this.boss2 = base.Game.CreateBoss(this.bossID1, 1389, 620, -1, 0, "", livingConfig);
			this.boss2.SetRelateDemagemRect(this.boss2.NpcInfo.X, this.boss2.NpcInfo.Y, this.boss2.NpcInfo.Width, this.boss2.NpcInfo.Height);
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
				if (this.boss1 != null && !this.boss1.IsLiving && this.boss2 != null && !this.boss2.IsLiving)
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
			if (this.boss1 != null && !this.boss1.IsLiving && this.boss2 != null && !this.boss2.IsLiving)
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

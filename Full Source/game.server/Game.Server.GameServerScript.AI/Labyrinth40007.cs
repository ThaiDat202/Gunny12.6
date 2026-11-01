using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40007 : AMissionControl
	{
		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		private int kill = 0;

		private int npcIDs = 40008;

		private int[] birthX = new int[]
		{
			170,
			190,
			210,
			230,
			250,
			270,
			290,
			310,
			330,
			350
		};

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
				this.npcIDs
			};
			int[] npcIds2 = new int[]
			{
				this.npcIDs
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1241);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			for (int i = 0; i < this.birthX.Length; i++)
			{
				this.someNpc.Add(base.Game.CreateNpc(this.npcIDs, this.birthX[i], 800, 1, 1));
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
			bool flag = true;
			base.CanGameOver();
			bool result;
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				result = true;
			}
			else
			{
				this.kill = 0;
				foreach (SimpleNpc current in this.someNpc)
				{
					if (current.IsLiving)
					{
						flag = false;
					}
					else
					{
						this.kill++;
					}
				}
				if (flag && this.kill == base.Game.MissionInfo.TotalCount)
				{
					base.Game.CreateGate(true);
				}
				result = flag;
			}
			return result;
		}

		public override int UpdateUIData()
		{
			return base.Game.TotalKillCount;
		}

		public override void OnGameOverMovie()
		{
			base.OnGameOverMovie();
			if (base.Game.GetLivedLivings().Count == 0)
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

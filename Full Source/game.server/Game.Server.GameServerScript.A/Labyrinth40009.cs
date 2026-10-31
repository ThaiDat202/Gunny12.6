using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40009 : AMissionControl
	{
		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		private int kill = 0;

		private int npcID1 = 40010;

		private int npcID2 = 40011;

		private int[] birthX = new int[]
		{
			1110,
			1090,
			1060,
			1040,
			1020,
			1000,
			1050,
			1310
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
				this.npcID1,
				this.npcID2
			};
			int[] npcIds2 = new int[]
			{
				this.npcID1,
				this.npcID2
			};
			base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(1222);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			for (int i = 0; i < this.birthX.Length; i++)
			{
				if (i < 6)
				{
					this.someNpc.Add(base.Game.CreateNpc(this.npcID1, this.birthX[i], 430, 1, -1));
				}
				else
				{
					this.someNpc.Add(base.Game.CreateNpc(this.npcID2, this.birthX[i], 430, 1, -1));
					this.someNpc.Add(base.Game.CreateNpc(this.npcID2, this.birthX[i] + 10, 430, 1, -1));
				}
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

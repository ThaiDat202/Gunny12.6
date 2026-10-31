using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40018 : AMissionControl
	{
		private SimpleBoss boss = null;

		private SimpleBoss m_boss = null;

		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		private int bossID2 = 5302;

		private int bossID = 40024;

		private int Delay = 0;

		private int kill = 0;

		private PhysicalObj moive;

		private PhysicalObj m_moive;

		private PhysicalObj m_front;

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
				this.bossID
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(2, "image/game/effect/5/minigun.swf", "asset.game.4.minigun");
			base.Game.AddLoadingFile(2, "image/game/effect/5/jinqud.swf", "asset.game.4.jinqud");
			base.Game.AddLoadingFile(2, "image/game/effect/5/zap.swf", "asset.game.4.zap");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.gebulinzhihuiguanAsset");
			base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
			base.Game.AddLoadingFile(1, "bombs/72.swf", "tank.resource.bombs.Bomb72");
			base.Game.SetMap(1274);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
			this.m_front = base.Game.Createlayer(1131, 650, "font", "game.asset.living.gebulinzhihuiguanAsset", "out", 1, 1);
			this.moive = base.Game.Createlayer(1567, 810, "moive", "asset.game.4.jinqud", "out", 1, 0);
			this.m_boss = base.Game.CreateBoss(this.bossID2, 190, 365, 1, 1, "");
			this.boss = base.Game.CreateBoss(this.bossID, 1477, 768, -1, 0, "");
			this.boss.SetRelateDemagemRect(-110, -179, 220, 180);
			this.m_boss.SetRelateDemagemRect(-42, -200, 84, 104);
			this.boss.PlayMovie("in", 0, 2000);
			this.m_boss.PlayMovie("in", 4000, 2000);
			this.m_boss.PlayMovie("in", 0, 2000);
			this.m_moive.PlayMovie("in", 6000, 0);
			this.m_front.PlayMovie("in", 6100, 0);
			this.m_moive.PlayMovie("out", 10000, 1000);
			this.m_front.PlayMovie("out", 9900, 0);
		}

		public override void OnNewTurnStarted()
		{
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1)
			{
				if (this.m_moive != null)
				{
					base.Game.RemovePhysicalObj(this.m_moive, true);
					this.m_moive = null;
				}
				if (this.m_front != null)
				{
					base.Game.RemovePhysicalObj(this.m_front, true);
					this.m_front = null;
				}
			}
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

		public override void OnShooted()
		{
			if (this.boss.IsLiving)
			{
				if (this.boss.Y == 659)
				{
					this.boss.SetXY(1477, 659);
					this.boss.SetXY(1477, 759);
					this.boss.CallFuction(new LivingCallBack(this.kill1), 100);
				}
				else if (this.boss.Y == 559)
				{
					this.boss.SetXY(1477, 559);
					this.boss.SetXY(1477, 759);
					this.boss.CallFuction(new LivingCallBack(this.kill2), 100);
				}
				else if (this.boss.Y == 459)
				{
					this.boss.SetXY(1477, 459);
					this.boss.SetXY(1477, 759);
					this.boss.CallFuction(new LivingCallBack(this.kill3), 100);
				}
				else if (this.boss.Y == 359)
				{
					this.boss.SetXY(1477, 359);
					this.boss.SetXY(1477, 759);
					this.boss.CallFuction(new LivingCallBack(this.kill4), 100);
				}
				else if (this.boss.Y == 259)
				{
					this.boss.SetXY(1477, 259);
					this.boss.SetXY(1477, 759);
					this.boss.CallFuction(new LivingCallBack(this.kill5), 100);
				}
			}
		}

		private void kill1()
		{
			this.Delay += 155;
			this.boss.PlayMovie("cryA", 1000 + this.Delay, 0);
			this.boss.SetXY(1477, 659);
		}

		private void kill2()
		{
			this.Delay += 155;
			this.boss.PlayMovie("cryB", 1000 + this.Delay, 0);
			this.boss.SetXY(1477, 559);
		}

		private void kill3()
		{
			this.Delay += 155;
			this.boss.PlayMovie("cryC", 1000 + this.Delay, 0);
			this.boss.SetXY(1477, 459);
		}

		private void kill4()
		{
			this.Delay += 155;
			this.boss.PlayMovie("cryD", 1000 + this.Delay, 0);
			this.boss.SetXY(1477, 359);
		}

		private void kill5()
		{
			this.Delay += 155;
			this.boss.PlayMovie("cryE", 1000 + this.Delay, 0);
			this.boss.SetXY(1477, 259);
		}
	}
}

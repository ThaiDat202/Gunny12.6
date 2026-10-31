using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
	public class Labyrinth40020 : AMissionControl
	{
		private SimpleBoss m_boss;

		private int m_kill = 0;

		private SimpleBoss boss;

		private SimpleBoss king;

		private PhysicalObj m_moive;

		private PhysicalObj m_moive1;

		private PhysicalObj m_moive2;

		private PhysicalObj m_moive3;

		private PhysicalObj m_front;

		private PhysicalObj[] m_leftWall = null;

		private PhysicalObj[] m_rightWall = null;

		private PhysicalObj m_wallRight = null;

		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		private PhysicalObj m_NPC;

		private PhysicalObj n_NPC;

		private PhysicalObj npc = null;

		private PhysicalObj npc2 = null;

		private int IsEixt = 0;

		private int IsEixt2 = 0;

		private int bossID = 40028;

		private int bossID2 = 5313;

		private int bossID3 = 5312;

		private int npcID = 40026;

		private int npcID2 = 40027;

		private int npcID3 = 5311;

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
			base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
			base.Game.AddLoadingFile(2, "image/game/effect/5/mubiao.swf", "asset.game.4.mubiao");
			base.Game.AddLoadingFile(2, "image/game/effect/5/xiaopao.swf", "asset.game.4.xiaopao");
			base.Game.AddLoadingFile(2, "image/game/effect/5/zao.swf", "asset.game.4.zao");
			base.Game.AddLoadingFile(2, "image/game/living/living144.swf", "game.living.Living144");
			base.Game.AddLoadingFile(2, "image/game/living/living152.swf", "game.living.Living152");
			base.Game.AddLoadingFile(2, "image/game/living/living154.swf", "game.living.Living154");
			base.Game.AddLoadingFile(2, "image/game/living/living147.swf", "game.living.Living147");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.gebulinzhihuiguanAsset");
			int[] npcIds = new int[]
			{
				this.bossID,
				this.bossID2,
				this.bossID3,
				this.npcID,
				this.npcID2,
				this.npcID3
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1269);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			this.m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			this.m_front = base.Game.Createlayer(1100, 395, "font", "game.asset.living.gebulinzhihuiguanAsset", "out", 1, 0);
			this.m_wallRight = base.Game.CreatePhysicalObj(1460, 580, "wallLeft", "asset.game.4.zao", "1", 1, 0);
			this.m_wallRight.SetRect(-75, -159, 100, 130);
			this.m_boss = base.Game.CreateBoss(this.bossID, 1480, 610, -1, 1, "");
			this.king = base.Game.CreateBoss(this.bossID2, 1617, 544, -1, 1, "");
			this.boss = base.Game.CreateBoss(this.bossID3, 1300, 650, -1, 1, "");
			this.boss.FallFrom(1300, 650, "", 0, 0, 1000);
			this.m_NPC = base.Game.Createlayer(1550, 650, "NPC", "game.living.Living154", "stand", 1, 0);
			this.n_NPC = base.Game.Createlayer(1367, 845, "NPC", "game.living.Living147", "stand", 1, 0);
			this.king.SetRelateDemagemRect(-34, -35, 50, 40);
			this.boss.SetRelateDemagemRect(-34, -35, 50, 40);
			this.m_boss.SetRelateDemagemRect(-34, -35, 50, 40);
			this.m_moive.PlayMovie("in", 3000, 0);
			this.m_front.PlayMovie("in", 3000, 0);
			this.m_moive.PlayMovie("out", 4000, 0);
			this.m_front.PlayMovie("out", 4000, 0);
			this.m_moive1 = base.Game.Createlayer(1617, 530, "moive", "asset.game.4.mubiao", "out", 1, 0);
			this.m_moive2 = base.Game.Createlayer(1300, 635, "moive", "asset.game.4.mubiao", "out", 1, 0);
			this.m_moive3 = base.Game.Createlayer(1480, 595, "moive", "asset.game.4.mubiao", "out", 1, 0);
		}

		public override void OnNewTurnStarted()
		{
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1)
			{
				if (this.m_moive != null)
				{
					base.Game.RemovePhysicalObj(this.m_moive, true);
					this.m_moive = null;
				}
				if (this.m_moive1 != null)
				{
					base.Game.RemovePhysicalObj(this.m_moive1, true);
					this.m_moive1 = null;
				}
				if (this.m_moive2 != null)
				{
					base.Game.RemovePhysicalObj(this.m_moive2, true);
					this.m_moive2 = null;
				}
				if (this.m_moive3 != null)
				{
					base.Game.RemovePhysicalObj(this.m_moive3, true);
					this.m_moive3 = null;
				}
				if (this.m_front != null)
				{
					base.Game.RemovePhysicalObj(this.m_front, true);
					this.m_front = null;
				}
				if (this.m_NPC != null)
				{
					base.Game.RemovePhysicalObj(this.m_NPC, true);
					this.m_NPC = null;
				}
				if (this.n_NPC != null)
				{
					base.Game.RemovePhysicalObj(this.n_NPC, true);
					this.n_NPC = null;
				}
			}
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			if (this.boss != null && !this.boss.IsLiving && this.king != null && !this.king.IsLiving)
			{
				this.m_leftWall = base.Game.FindPhysicalObjByName("wallLeft", false);
				this.m_rightWall = base.Game.FindPhysicalObjByName("wallRight", false);
				this.m_wallRight.SetRect(0, 0, 0, 0);
				PhysicalObj[] array = this.m_leftWall;
				for (int i = 0; i < array.Length; i++)
				{
					PhysicalObj physicalObj = array[i];
					if (physicalObj != null)
					{
						base.Game.RemovePhysicalObj(physicalObj, true);
					}
				}
				array = this.m_rightWall;
				for (int i = 0; i < array.Length; i++)
				{
					PhysicalObj physicalObj = array[i];
					if (physicalObj != null)
					{
						base.Game.RemovePhysicalObj(physicalObj, true);
					}
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
				if (this.m_boss != null && !this.m_boss.IsLiving)
				{
					if (base.Game.CanEnterGate)
					{
						result = true;
						return result;
					}
					base.Game.CanShowBigBox = true;
				}
				result = false;
			}
			return result;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return 0;
		}

		public override void OnGameOverMovie()
		{
			base.OnGameOverMovie();
			if (this.m_boss != null && !this.m_boss.IsLiving)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
			this.m_leftWall = base.Game.FindPhysicalObjByName("wallLeft");
			this.m_rightWall = base.Game.FindPhysicalObjByName("wallRight");
			for (int i = 0; i < this.m_leftWall.Length; i++)
			{
				base.Game.RemovePhysicalObj(this.m_leftWall[i], true);
			}
			for (int i = 0; i < this.m_rightWall.Length; i++)
			{
				base.Game.RemovePhysicalObj(this.m_rightWall[i], true);
			}
		}

		private void OnDie()
		{
			if (!this.king.IsLiving && this.IsEixt == 0)
			{
				this.npc = base.Game.Createlayer(this.king.X, this.king.Y, "", "game.living.Living144", "standB", 1, 0);
				this.IsEixt = 1;
			}
			if (!this.boss.IsLiving && this.IsEixt2 == 0)
			{
				this.npc2 = base.Game.Createlayer(this.boss.X, this.boss.Y, "", "game.living.Living152", "standB", 1, 0);
				this.IsEixt2 = 1;
			}
		}

		public override void OnShooted()
		{
			if (!this.king.IsLiving)
			{
				this.king.CallFuction(new LivingCallBack(this.OnDie), 8000);
			}
			if (!this.boss.IsLiving)
			{
				this.boss.CallFuction(new LivingCallBack(this.OnDie), 8000);
			}
		}
	}
}

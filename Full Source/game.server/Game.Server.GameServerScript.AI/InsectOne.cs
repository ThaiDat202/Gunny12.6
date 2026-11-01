using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Server.GameServerScript.AI.NPC
{
	public class InsectOne : ABrain
	{
		protected Player m_targer;

		private int m_attackTurn = 0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			m_body.CurrentDamagePlus = 0.5f;
			m_body.CurrentShootMinus = 1f;
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.OnStartAttacking();
			if (m_attackTurn == 0)
			{
				MoveBeat();
				m_attackTurn++;
			}
			else
			{
				Move();
				m_attackTurn = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void Move()
		{
			base.Body.MoveTo(base.Game.Random.Next(125, 1215), base.Game.Random.Next(113, 554), "fly", 500, "", 12, null);
		}

		private void MoveBeat()
		{
			base.Body.MoveTo(base.Game.Random.Next(125, 1215), base.Game.Random.Next(113, 554), "fly", 500, "", 12, null);
		}
	}
}
namespace Game.Server.GameServerScript.AI.Messions
{
	public class InsectOne : AMissionControl
	{
		private SimpleBoss boss = null;

		private int bossID = 71009;

		private List<SimpleNpc> someNpc = new List<SimpleNpc>();

		private int npcID1 = 71006;

		private int npcID2 = 71007;

		private int npcID3 = 71008;

		private int kill = 0;

		private int limitTurn = 5;

		private Point[] brithPoint = new Point[30]
		{
			new Point(180, 147),
			new Point(375, 117),
			new Point(105, 242),
			new Point(277, 234),
			new Point(155, 338),
			new Point(313, 356),
			new Point(426, 295),
			new Point(517, 149),
			new Point(543, 268),
			new Point(291, 468),
			new Point(152, 500),
			new Point(419, 572),
			new Point(507, 441),
			new Point(508, 342),
			new Point(631, 514),
			new Point(809, 605),
			new Point(707, 392),
			new Point(684, 271),
			new Point(697, 111),
			new Point(833, 126),
			new Point(810, 263),
			new Point(875, 370),
			new Point(874, 485),
			new Point(1054, 483),
			new Point(944, 192),
			new Point(1000, 297),
			new Point(1088, 360),
			new Point(1246, 325),
			new Point(1127, 171),
			new Point(987, 564)
		};

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
			if (score > 1600)
			{
				return 1;
			}
			return 0;
		}

		public override void OnPrepareNewSession()
		{
			base.OnPrepareNewSession();
			int[] npcIds = new int[4] { bossID, npcID1, npcID2, npcID3 };
			int[] npcIds2 = new int[4] { bossID, npcID1, npcID2, npcID3 };
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.AddLoadingFile(1, "bombs/128.swf", "tank.resource.bombs.Bomb128");
			base.Game.AddLoadingFile(1, "bombs/129.swf", "tank.resource.bombs.Bomb129");
			base.Game.SetMap(70003);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			Player[] allPlayers = base.Game.GetAllPlayers();
			int[] array = new int[2] { 10615, 10616 };
			Player[] array2 = allPlayers;
			foreach (Player player in array2)
			{
				for (int j = 0; j < array.Length; j++)
				{
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(array[j]);
					ItemInfo cloneItem = ItemInfo.CreateFromTemplate(goods, 1, 101);
					player.PlayerDetail.AddTemplate(cloneItem, eBageType.FightBag, 1, eGameView.OtherTypeGet);
				}
				if (player.BatleConfig.CakeStatus)
				{
					LivingConfig livingConfig = base.Game.BaseConfig();
					livingConfig.IsInsectBoss = true;
					livingConfig.KeepLife = true;
					boss = base.Game.CreateBoss(bossID, 1184, 645, -1, 4, "", livingConfig);
					boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
					break;
				}
			}
			int num = 0;
			LivingConfig livingConfig2 = base.Game.BaseConfig();
			livingConfig2.IsInsectNpc = true;
			livingConfig2.IsFly = true;
			livingConfig2.isShowBlood = false;
			Point[] array3 = brithPoint;
			for (int k = 0; k < array3.Length; k++)
			{
				Point point = array3[k];
				if (num < 10)
				{
					someNpc.Add(base.Game.CreateNpc(npcID1, point.X, point.Y, 1, -1, livingConfig2));
				}
				else if (num < 20)
				{
					someNpc.Add(base.Game.CreateNpc(npcID2, point.X, point.Y, 1, -1, livingConfig2));
				}
				else
				{
					someNpc.Add(base.Game.CreateNpc(npcID3, point.X, point.Y, 1, -1, livingConfig2));
				}
				num++;
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
			Player[] allPlayers = base.Game.GetAllPlayers();
			Player[] array = allPlayers;
			foreach (Player player in array)
			{
				if (player.TurnNum >= limitTurn)
				{
					return true;
				}
			}
			kill = 0;
			foreach (SimpleNpc item in someNpc)
			{
				if (!item.IsLiving)
				{
					kill++;
				}
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return kill;
		}

		public override void OnTakeDamage()
		{
			base.OnTakeDamage();
			boss.PlayMovie("dieB", 2000, 3000);
			boss.CallFuction(Catching, 4500);
		}

		private void Catching()
		{
			if (boss.Blood < 10000)
			{
				boss.CallFuction(AddScore, 500);
				return;
			}
			boss.PlayMovie("dieC", 500, 1000);
			boss.CallFuction(Escape, 1500);
		}

		private void AddScore()
		{
			RemoveBoss();
			Player[] allPlayers = base.Game.GetAllPlayers();
			Player[] array = allPlayers;
			foreach (Player player in array)
			{
				player.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("CatchInsect.AddScore", 1200));
				player.PlayerDetail.AddSummerScore(1200);
			}
		}

		private void Escape()
		{
			boss.PlayMovie("dieD", 0, 2500);
			boss.CallFuction(RemoveBoss, 3000);
		}

		private void RemoveBoss()
		{
			base.Game.RemoveLiving(boss.Id);
			boss = null;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			base.Game.IsWin = true;
		}
	}
}
namespace Game.Server.GameServerScript.AI.Game
{
	public class InsectOne : APVEGameControl
	{
		public override void OnCreated()
		{
			base.Game.SetupMissions("7000701");
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

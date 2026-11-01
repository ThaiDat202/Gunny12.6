using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC
{
	public class CrosairBoss : ABrain
	{
		private int m_attackTurn = 0;

		public int currentCount = 0;

		public int Dander = 0;

		public override void OnBeginSelfTurn()
		{
			base.OnBeginSelfTurn();
		}

		public override void OnBeginNewTurn()
		{
			base.OnBeginNewTurn();
			base.Body.CurrentDamagePlus = 1f;
			base.Body.CurrentShootMinus = 1f;
			base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
			if (base.Body.Direction == -1)
			{
				base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
			}
			else
			{
				base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
			}
		}

		public override void OnCreated()
		{
			base.OnCreated();
		}

		public override void OnStartAttacking()
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			bool flag = false;
			int num = 0;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > 670)
				{
					int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
					if (num2 > num)
					{
						num = num2;
					}
					flag = true;
				}
			}
			if (flag)
			{
				KillAttack(0, base.Game.Map.Info.ForegroundWidth + 1);
			}
			else if (m_attackTurn == 0)
			{
				AttackA();
				m_attackTurn++;
			}
			else if (m_attackTurn == 1)
			{
				AttackB();
				m_attackTurn++;
			}
			else
			{
				AttackC();
				m_attackTurn = 0;
			}
		}

		public override void OnStopAttacking()
		{
			base.OnStopAttacking();
		}

		private void KillAttack(int fx, int tx)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatA", 1000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
		}

		private void AttackA()
		{
			base.Body.CurrentDamagePlus = 1.5f;
			base.Body.PlayMovie("beatA", 1000, 0);
			base.Body.CallFuction(RangeAttacking, 4000);
		}

		private void AttackB()
		{
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.CallFuction(RangeAttacking, 4000);
		}

		private void AttackC()
		{
			base.Body.CurrentDamagePlus = 3.1f;
			base.Body.PlayMovie("beatC", 1000, 0);
			base.Body.CallFuction(RangeAttacking, 3500);
		}

		private void RangeAttacking()
		{
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
		}
	}
}
namespace Game.Server.GameServerScript.AI.Messions
{
	public class CrosairBoss : AMissionControl
	{
		private SimpleBoss boss = null;

		private int bossID = 72000;

		private int kill = 0;

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
			int[] npcIds = new int[1] { bossID };
			int[] npcIds2 = new int[1] { bossID };
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds2);
			base.Game.SetMap(12016);
		}

		public override void OnStartGame()
		{
			base.OnStartGame();
			boss = base.Game.CreateBoss(bossID, 899, 190, -1, 4, "");
			boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
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
			if (boss != null && !boss.IsLiving)
			{
				kill++;
				return true;
			}
			return false;
		}

		public override int UpdateUIData()
		{
			base.UpdateUIData();
			return kill;
		}

		public override void OnGameOver()
		{
			base.OnGameOver();
			if (boss != null && !boss.IsLiving)
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
	public class CrosairBoss : APVEGameControl
	{
		public override void OnCreated()
		{
			base.Game.SetupMissions("12016");
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

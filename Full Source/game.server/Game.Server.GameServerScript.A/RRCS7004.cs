using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class RRCS7004 : AMissionControl
{
	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

	private PhysicalObj m_npc2;

	private SimpleBoss boss = null;

	private SimpleNpc npc = null;

	private int m_kill = 0;

	private int bossId = 7031;

	private int npcId = 7032;

	private int npcId2 = 7033;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1150)
		{
			return 3;
		}
		if (score > 925)
		{
			return 2;
		}
		if (score > 700)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(1, "bombs/83.swf", "tank.resource.bombs.Bomb83");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/7/choud.swf", "asset.game.seven.choud");
		base.Game.AddLoadingFile(2, "image/game/effect/7/jinqucd.swf", "asset.game.seven.jinqucd");
		base.Game.AddLoadingFile(2, "image/game/effect/7/du.swf", "asset.game.seven.du");
		int[] npcIds = new int[3] { bossId, npcId, npcId2 };
		base.Game.LoadResources(npcIds);
		int[] npcIds2 = new int[1] { bossId };
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1164);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_npc2 = base.Game.Createlayer(2170, 636, "", "game.living.Living178", "stand", 1, 1);
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.HasTurn = false;
		livingConfig.IsFly = true;
		npc = base.Game.CreateNpc(npcId, 1920, 900, 1, -1, livingConfig);
		npc.PlayMovie("stand", 1000, 0);
		npc.CallFuction(CreateBossEffect, 4000);
	}

	private void CreateBossEffect()
	{
		boss = base.Game.CreateBoss(bossId, 200, 200, 1, 1, "born");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		m_kingMoive.PlayMovie("in", 1000, 0);
		m_kingFront.PlayMovie("in", 2000, 0);
		m_kingMoive.PlayMovie("out", 5000, 0);
		m_kingFront.PlayMovie("out", 5400, 0);
		boss.Say("别想逃！", 0, 6000);
		boss.PlayMovie("skill", 8000, 0);
		base.Game.SendObjectFocus(npc, 1, 9000, 0);
		npc.PlayMovie("standB", 10000, 0);
		npc.Config.CanTakeDamage = false;
		base.Game.SendObjectFocus(boss, 1, 13000, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > 1)
		{
			if (m_kingMoive != null)
			{
				base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
				m_kingMoive = null;
			}
			if (m_kingFront != null)
			{
				base.Game.RemovePhysicalObj(m_kingFront, sendToClient: true);
				m_kingFront = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		if (boss != null && !boss.IsLiving && npc != null && !npc.IsLiving)
		{
			m_kill++;
			return true;
		}
		if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
		{
			return true;
		}
		return false;
	}

	public override void OnDied()
	{
		base.OnDied();
		if (boss != null && !boss.IsLiving && npc.IsLiving)
		{
			int waitTimerLeft = base.Game.GetWaitTimerLeft();
			base.Game.SendObjectFocus(npc, 1, waitTimerLeft + 500, 500);
			npc.PlayMovie("out", waitTimerLeft + 1000, 0);
			npc.Config.CanTakeDamage = true;
		}
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return m_kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (boss != null && !boss.IsLiving && npc != null && !npc.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

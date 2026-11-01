using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class M20 : AMissionControl
{
	private int bossId = 10000061;

	private int boss2Id = 10000062;

	private int npc1Id = 13012;

	private int npc2Id = 13013;

	private int npc3Id = 13014;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private SimpleBoss boss = null;

	private SimpleBoss boss2 = null;

	private SimpleBoss m_tempBoss = null;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 600)
		{
			return 3;
		}
		if (score > 520)
		{
			return 2;
		}
		if (score > 450)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[5] { bossId, boss2Id, npc1Id, npc2Id, npc3Id };
		base.Game.AddLoadingFile(2, "image/game/effect/10/danbao.swf", "asset.game.ten.danbao");
		base.Game.AddLoadingFile(2, "image/game/effect/10/qunbao.swf", "asset.game.ten.qunbao");
		base.Game.AddLoadingFile(2, "image/game/effect/10/tedabiaoji.swf", "asset.game.ten.tedabiaoji");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.shuangwangAsset");
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1217);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(810, 750, "front", "game.asset.living.shuangwangAsset", "out", 1, 0);
		boss = base.Game.CreateBoss(bossId, 83, 1020, 1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		base.Game.SendObjectFocus(boss, 0, 0, 0);
		boss.CallFuction(CreateBossHelp, 2000);
	}

	private void CreateBossHelp()
	{
		LivingConfig config = base.Game.BaseConfig();
		boss2 = base.Game.CreateBoss(boss2Id, 1912, 1020, -1, 1, "", config);
		boss2.SetRelateDemagemRect(boss2.NpcInfo.X, boss2.NpcInfo.Y, boss2.NpcInfo.Width, boss2.NpcInfo.Height);
		boss2.Delay = 1;
		base.Game.SendObjectFocus(boss2, 0, 0, 0);
		base.Game.SendFreeFocus(1000, 900, 1, 2000, 0);
		m_moive.PlayMovie("in", 3000, 0);
		m_front.PlayMovie("in", 3200, 0);
		m_moive.PlayMovie("out", 6000, 0);
		m_front.PlayMovie("out", 6200, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (m_moive != null && m_front != null)
		{
			base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
			base.Game.RemovePhysicalObj(m_front, sendToClient: true);
			m_moive = null;
			m_front = null;
		}
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (boss != null && !boss.IsLiving && boss2 != null && !boss2.IsLiving)
		{
			return true;
		}
		if (base.Game.TotalTurn > 200)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (!boss.IsLiving && !boss2.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

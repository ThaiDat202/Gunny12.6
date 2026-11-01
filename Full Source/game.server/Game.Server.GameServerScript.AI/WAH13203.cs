using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class WAH13203 : AMissionControl
{
	private int bossId = 13207;

	private int npcHelpId = 13206;

	private int npcId = 13210;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private SimpleBoss boss = null;

	private SimpleBoss bossHelp = null;

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
		int[] npcIds = new int[3] { bossId, npcHelpId, npcId };
		base.Game.AddLoadingFile(2, "image/game/effect/10/chengtuo.swf", "asset.game.ten.chengtuo");
		base.Game.AddLoadingFile(2, "image/game/effect/10/laotie.swf", "asset.game.ten.laotie");
		base.Game.AddLoadingFile(2, "image/game/effect/10/laotie.swf", "asset.game.ten.laotie2");
		base.Game.AddLoadingFile(2, "image/game/effect/5/lanhuo.swf", "asset.game.4.lanhuo");
		base.Game.AddLoadingFile(2, "image/game/effect/5/heip.swf", "asset.game.4.heip");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.dadangAsset");
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1216);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(1200, 700, "front", "game.asset.living.dadangAsset", "out", 1, 0);
		boss = base.Game.CreateBoss(bossId, 1683, 1012, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		base.Game.SendObjectFocus(boss, 0, 0, 0);
		boss.CallFuction(CreateBossHelp, 2000);
	}

	private void CreateBossHelp()
	{
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.CanTakeDamage = false;
		livingConfig.IsFly = true;
		bossHelp = base.Game.CreateBoss(npcHelpId, 1683, 762, -1, 1, "", livingConfig);
		bossHelp.Delay = 1;
		base.Game.SendObjectFocus(bossHelp, 0, 0, 0);
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
		if (boss != null && !boss.IsLiving)
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
		if (!boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

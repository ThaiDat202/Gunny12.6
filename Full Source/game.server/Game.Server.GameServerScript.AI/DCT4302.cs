using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class DCT4302 : AMissionControl
{
	private SimpleBoss m_king = null;

	private SimpleBoss m_boss = null;

	private int bossID = 4305;

	private int bossID2 = 4306;

	private int npcId = 4302;

	private int kill = 0;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

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
		int[] npcIds = new int[3] { bossID, bossID2, npcId };
		int[] npcIds2 = new int[2] { bossID, bossID2 };
		base.Game.AddLoadingFile(2, "image/game/effect/4/feather.swf", "asset.game.4.feather");
		base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/bossbornbgasset.swf", "game.asset.living.tingyuanlieshouAsset");
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1143);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(1098, 706, "front", "game.asset.living.tingyuanlieshouAsset", "out", 1, 0);
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsFly = true;
		m_king = base.Game.CreateBoss(bossID, 354, 344, -1, 1, "born", livingConfig);
		m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
		base.Game.SendObjectFocus(m_king, 1, 100, 0);
		base.Game.SendFreeFocus(1460, 962, 1, 3000, 0);
		m_king.CallFuction(CreateBoss, 4000);
	}

	private void CreateBoss()
	{
		m_boss = base.Game.CreateBoss(bossID2, 1460, 962, -1, 1, "born");
		m_boss.SetRelateDemagemRect(m_boss.NpcInfo.X, m_boss.NpcInfo.Y, m_boss.NpcInfo.Width, m_boss.NpcInfo.Height);
		m_moive.PlayMovie("in", 3000, 0);
		m_front.PlayMovie("in", 3200, 0);
		m_moive.PlayMovie("out", 6000, 0);
		m_front.PlayMovie("out", 6000, 0);
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
			if (m_moive != null)
			{
				base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
				m_moive = null;
			}
			if (m_front != null)
			{
				base.Game.RemovePhysicalObj(m_front, sendToClient: true);
				m_front = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		if (m_king != null && !m_king.IsLiving && m_boss != null && !m_boss.IsLiving)
		{
			kill++;
			return true;
		}
		if (base.Game.TotalTurn > base.Game.MissionInfo.TotalTurn)
		{
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
		if (m_king != null && !m_king.IsLiving && m_boss != null && !m_boss.IsLiving)
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
		base.OnShooted();
	}
}

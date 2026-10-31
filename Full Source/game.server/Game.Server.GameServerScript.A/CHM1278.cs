using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class CHM1278 : AMissionControl
{
	private SimpleBoss m_king = null;

	private int m_kill = 0;

	private int bossID = 1208;

	private int npcID = 1211;

	private PhysicalObj m_kingMoive;

	private PhysicalObj m_front;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 900)
		{
			return 3;
		}
		if (score > 825)
		{
			return 2;
		}
		if (score > 725)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[2] { npcID, bossID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BombKingAsset");
		base.Game.SetMap(1076);
		base.Game.IsBossWar = LanguageMgr.GetTranslation("GameServerScript.AI.Messions.CHM1278.msg1");
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(710, 380, "font", "game.asset.living.BombKingAsset", "out", 1, 0);
		m_king = base.Game.CreateBoss(bossID, 888, 590, -1, 0, "");
		m_king.FallFrom(888, 690, "fall", 0, 2, 1000);
		m_king.SetRelateDemagemRect(-41, -187, 83, 140);
		m_kingMoive.PlayMovie("in", 1000, 0);
		m_front.PlayMovie("in", 2000, 2000);
		m_king.AddDelay(16);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (m_king.State == 0)
		{
			m_king.SetRelateDemagemRect(-41, -187, 83, 140);
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (m_kingMoive != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
			m_kingMoive = null;
		}
		if (m_front != null)
		{
			base.Game.RemovePhysicalObj(m_front, sendToClient: true);
			m_front = null;
		}
	}

	public override bool CanGameOver()
	{
		if (!m_king.IsLiving)
		{
			m_kill++;
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return m_kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		bool flag = true;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving)
			{
				flag = false;
			}
		}
		if (!m_king.IsLiving && !flag)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

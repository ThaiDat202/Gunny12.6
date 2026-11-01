using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class CHM1277 : AMissionControl
{
	private SimpleBoss m_king = null;

	private int m_kill = 0;

	private int bossID = 1207;

	private int npcID = 1204;

	private PhysicalObj m_kingMoive;

	private PhysicalObj m_front;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1550)
		{
			return 3;
		}
		if (score > 1450)
		{
			return 2;
		}
		if (score > 1400)
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
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ZhenBombKingAsset");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.wordtip75");
		base.Game.SetMap(1076);
		base.Game.IsBossWar = "炸弹人王";
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_king = base.Game.CreateBoss(bossID, 890, 590, -1, 0, "");
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(710, 380, "font", "game.asset.living.ZhenBombKingAsset", "out", 1, 0);
		m_king.FallFrom(890, 590, "fall", 0, 2, 1000);
		m_king.SetRelateDemagemRect(-41, -187, 83, 140);
		m_kingMoive.PlayMovie("in", 1000, 0);
		m_front.PlayMovie("in", 2000, 2000);
		m_king.AddDelay(16);
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
		if (!m_king.IsLiving)
		{
			m_kill++;
			m_king.PlayMovie("die", 0, 200);
			return true;
		}
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
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

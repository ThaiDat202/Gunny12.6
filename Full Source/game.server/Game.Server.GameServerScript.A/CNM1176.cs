using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class CNM1176 : AMissionControl
{
	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

	private SimpleBoss m_king = null;

	private SimpleBoss m_secondKing = null;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private int kill = 0;

	private int m_state = 1105;

	private int turn = 0;

	private readonly int firstBossID = 1105;

	private readonly int secondBossID = 1106;

	private int npcID = 1202;

	private LivingConfig config;

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
		return (score > 725) ? 1 : 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
		base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.LoadResources(new int[2] { firstBossID, secondBossID });
		base.Game.LoadNpcGameOverResources(new int[1] { firstBossID });
		base.Game.SetMap(1076);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		config = base.Game.BaseConfig();
		if (secondBossID == 1006)
		{
			config.IsFly = true;
		}
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_king = base.Game.CreateBoss(m_state, 890, 590, -1, 1, "");
		config.IsFly = true;
		m_king.SetRelateDemagemRect(-21, -79, 72, 51);
		m_king.AddDelay(10);
		m_kingMoive.PlayMovie("in", 1000, 0);
		m_kingFront.PlayMovie("in", 1000, 0);
		m_kingMoive.PlayMovie("out", 1000, 0);
		m_kingFront.PlayMovie("out", 1000, 0);
		turn = base.Game.TurnIndex;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > turn + 1)
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
		base.CanGameOver();
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!m_king.IsLiving && m_state == firstBossID)
		{
			m_state++;
		}
		if (m_state == secondBossID && m_secondKing == null)
		{
			base.Game.RemoveLiving(m_king.Id);
			m_secondKing = base.Game.CreateBoss(m_state, m_king.X, m_king.Y - 420, m_king.Direction, 1, "", config);
			if (m_secondKing.Direction == -1)
			{
				m_secondKing.SetRectBomb(24, -159, 66, 38);
				m_secondKing.SetRelateDemagemRect(-21, -79, 72, 51);
			}
			else
			{
				m_secondKing.SetRectBomb(-90, -159, 66, 38);
				m_secondKing.SetRelateDemagemRect(-21, -79, 72, 51);
			}
		}
		if (m_secondKing != null && !m_secondKing.IsLiving)
		{
			m_leftWall = base.Game.FindPhysicalObjByName("com.mapobject.asset.WaveAsset_01_left");
			m_rightWall = base.Game.FindPhysicalObjByName("com.mapobject.asset.WaveAsset_01_right");
			for (int i = 0; i < m_leftWall.Length; i++)
			{
				base.Game.RemovePhysicalObj(m_leftWall[i], sendToClient: true);
			}
			for (int j = 0; j < m_rightWall.Length; j++)
			{
				base.Game.RemovePhysicalObj(m_rightWall[j], sendToClient: true);
			}
			base.Game.RemoveLiving(m_secondKing.Id);
			if (base.Game.CreatePhysicalObj(m_secondKing.X, m_secondKing.Y, "king", "game.living.Living005", "specialDie", 1, 0).CurrentAction == "specialDie")
			{
				kill++;
				return true;
			}
		}
		return false;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (m_state == secondBossID && !m_secondKing.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

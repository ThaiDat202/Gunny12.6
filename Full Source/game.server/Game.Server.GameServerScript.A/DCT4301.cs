using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class DCT4301 : AMissionControl
{
	private List<SimpleNpc> npcs = new List<SimpleNpc>();

	private SimpleBoss m_boss = null;

	private SimpleBoss m_helper = null;

	private int npcID2 = 4301;

	private int npcID = 4303;

	private int bossID = 4304;

	private int m_bloodReduce = 2100;

	private int m_totalNpc = 1;

	private int m_kill = 0;

	public int CountKill
	{
		get
		{
			return m_kill;
		}
		set
		{
			m_kill = value;
		}
	}

	public SimpleBoss Helper
	{
		get
		{
			return m_helper;
		}
		set
		{
			m_helper = value;
		}
	}

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
		int[] npcIds = new int[3] { bossID, npcID, npcID2 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.AddLoadingFile(2, "image/game/effect/4/gate.swf", "game.asset.Gate");
		base.Game.SetMap(1142);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.CanTakeDamage = false;
		m_boss = base.Game.CreateBoss(bossID, 1520, 350, -1, 1, "", livingConfig);
		base.Game.SendHideBlood(m_boss, 0);
		CreateHelper();
		base.Game.SendFreeFocus(1500, 250, 1, 2000, 3000);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (m_helper != null && m_helper.IsLiving && m_helper.X >= 600 && base.Game.FindAllNpcLiving().Length == 0 && base.Game.CurrentTurnLiving is Player)
		{
			CreateNpc();
		}
		if (m_helper != null && !m_helper.IsLiving)
		{
			base.Game.RemoveLiving(m_helper, sendToClient: true);
			CreateHelper();
		}
		if (base.Game.CurrentTurnLiving is Player && m_helper != null && m_helper.IsLiving && (int)m_boss.Properties1 == 1)
		{
			m_helper.PlayMovie("standB", 1200, 0);
		}
	}

	private void CreateHelper()
	{
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.IsHelper = true;
		m_helper = base.Game.CreateBoss(npcID2, 321, 746, 1, 0, "", livingConfig);
		base.Game.SendFreeFocus(m_helper.X, m_helper.Y, 1, 2000, 3000);
		m_helper.AddEffect(new ContinueReduceBloodEffect(2, m_bloodReduce, m_helper), 0);
	}

	private void CreateNpc()
	{
		int num = 0;
		npcs = new List<SimpleNpc>();
		for (int i = 0; i < m_totalNpc; i++)
		{
			if (num > 0)
			{
				int num2 = base.Game.Random.Next(5, 110);
				npcs.Add(base.Game.CreateNpc(npcID, m_helper.X - num2, m_helper.Y, 0));
			}
			else
			{
				npcs.Add(base.Game.CreateNpc(npcID, m_helper.X - 20, m_helper.Y, 0));
			}
			num++;
		}
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (m_kill >= base.Game.MissionInfo.TotalCount)
		{
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
		return m_kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (m_kill >= base.Game.MissionInfo.TotalCount)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

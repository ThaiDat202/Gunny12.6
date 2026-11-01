using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class ETH3203 : AMissionControl
{
	private SimpleBoss king = null;

	private SimpleBoss boss = null;

	private int m_kill = 0;

	private int bossMainId = 3208;

	private int bossTwoId = 3209;

	private int npcID = 3206;

	private int fearId = 3210;

	private int lockId = 3211;

	private PhysicalObj m_moive;

	private PhysicalObj m_front = null;

	private PhysicalObj moive;

	private PhysicalObj front;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1330)
		{
			return 3;
		}
		if (score > 1150)
		{
			return 2;
		}
		if (score > 970)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[5] { bossMainId, bossTwoId, npcID, fearId, lockId };
		int[] npcIds2 = new int[2] { bossMainId, bossTwoId };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
		base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
		base.Game.AddLoadingFile(2, "image/map/1124/object/1124object.swf", "game.crazytank.assetmap.Dici");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanBrotherAsset");
		base.Game.SetMap(1124);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.PveGameDelay = 0;
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(650, 400, "front", "game.asset.living.ClanBrotherAsset", "out", 1, 0);
		king = base.Game.CreateBoss(bossMainId, 234, 357, 1, 1, "");
		king.SetRelateDemagemRect(king.NpcInfo.X, king.NpcInfo.Y, king.NpcInfo.Width, king.NpcInfo.Height);
		boss = base.Game.CreateBoss(bossTwoId, 1368, 357, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		base.Game.SendObjectFocus(king, 1, 2000, 0);
		king.PlayMovie("call", 3000, 0);
		king.Say("Chạy à? Định chạy đi đâu?", 0, 3300);
		base.Game.SendObjectFocus(boss, 1, 6200, 0);
		boss.PlayMovie("castA", 7000, 0);
		boss.Say("Đại ca. Đây là vật tế cuối cùng rồi đấy!", 0, 7300);
		base.Game.SendFreeFocus(827, 534, 1, 10000, 0);
		m_moive.PlayMovie("in", 10000, 0);
		m_front.PlayMovie("in", 11000, 0);
		m_moive.PlayMovie("out", 14000, 0);
		m_front.PlayMovie("out", 15000, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
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

	public override bool CanGameOver()
	{
		base.CanGameOver();
		List<Living> list = base.Game.FindAllTurnBossLiving();
		if (list.Count <= 0)
		{
			return true;
		}
		if (base.Game.TurnIndex > 200)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		List<Living> list = base.Game.FindAllTurnBossLiving();
		if (list.Count <= 0)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	public override void OnDied()
	{
		base.OnDied();
	}
}

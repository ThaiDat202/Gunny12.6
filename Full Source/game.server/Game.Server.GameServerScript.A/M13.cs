using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class M13 : AMissionControl
{
	private SimpleBoss m_boss;

	private PhysicalObj m_front;

	private PhysicalObj m_moive;

	private PhysicalObj m_fan;

	private int IsSay = 0;

	private int bossID = 10000049;

	private int fanID = 6143;

	private static string[] KillChat = new string[2] { "欢迎来到开心弹弹堂", "告诉你们这个地图我就是这里的扛把子" };

	private static string[] ShootedChat = new string[2] { "一群沙雕 ...", "快点把我揍死哦！颜宇轩万岁！！！！..." };

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
		base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.bogucaipanAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/6/chang.swf", "asset.game.six.chang");
		base.Game.AddLoadingFile(2, "image/game/effect/6/bluecircle.swf", "asset.game.six.bluecircle");
		base.Game.AddLoadingFile(2, "image/game/effect/6/greencircle.swf", "asset.game.six.greencircle");
		int[] npcIds = new int[2] { bossID, fanID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1168);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_fan = base.Game.Createlayer(1250, 520, "moive", "game.living.Living189", "", 1, 0);
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(1550, 850, "front", "game.asset.living.bogucaipanAsset", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID, 1250, 1000, -1, 1, "");
		m_boss.FallFrom(m_boss.X, m_boss.Y, "", 0, 0, 1000);
		m_boss.SetRelateDemagemRect(m_boss.NpcInfo.X, m_boss.NpcInfo.Y, m_boss.NpcInfo.Width, m_boss.NpcInfo.Height);
		base.Game.SendGameFocus(m_boss, 0, 3000);
		m_boss.PlayMovie("shengqi", 1000, 2000);
		m_boss.PlayMovie("xialai", 2000, 3000);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6100, 0);
		m_moive.PlayMovie("out", 9000, 0);
		m_front.PlayMovie("out", 9100, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		IsSay = 0;
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!m_boss.IsLiving)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		if (m_boss == null)
		{
			return 0;
		}
		if (!m_boss.IsLiving)
		{
			return 1;
		}
		return base.UpdateUIData();
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (!m_boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}

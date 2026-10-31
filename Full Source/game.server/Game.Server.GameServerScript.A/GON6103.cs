using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class GON6103 : AMissionControl
{
	private SimpleBoss m_boss;

	private SimpleBoss m_arbitra;

	private SimpleNpc m_holegen;

	private PhysicalObj m_front;

	private PhysicalObj m_moive;

	private PhysicalObj m_fan;

	private int IsSay = 0;

	private int bossID = 6131;

	private int fanID = 6134;

	private int arbitraID = 6132;

	private int holegenID = 6135;

	private static string[] KillChat = new string[2] { "Gửi cho bạn trở về nhà!", "Một mình, bạn có ảo tưởng có thể đánh bại tôi?" };

	private static string[] ShootedChat = new string[2] { " Đau ah! Đau ...", "Quốc vương vạn tuế ..." };

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
		base.Game.AddLoadingFile(2, "image/game/effect/6/popcan.swf", "asset.game.six.popcan");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguquanwangAsset");
		int[] npcIds = new int[4] { bossID, fanID, arbitraID, holegenID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1167);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_fan = base.Game.Createlayer(1250, 520, "moive", "game.living.Living189", "", 1, 0);
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(1250, 750, "front", "game.asset.living.boguquanwangAsset", "out", 1, 0);
		LivingConfig livingConfig = base.Game.BaseConfig();
		livingConfig.HasTurn = false;
		livingConfig.CanTakeDamage = false;
		livingConfig.IsFly = true;
		m_holegen = base.Game.CreateNpc(holegenID, 1250, 700, 1, 1, livingConfig);
		base.Game.SendHideBlood(m_holegen, 0);
		m_holegen.OnSmallMap(state: false);
		m_boss = base.Game.CreateBoss(bossID, 1650, 1000, -1, 1, "");
		m_boss.SetRelateDemagemRect(m_boss.NpcInfo.X, m_boss.NpcInfo.Y, m_boss.NpcInfo.Width, m_boss.NpcInfo.Height);
		LivingConfig livingConfig2 = base.Game.BaseConfig();
		livingConfig2.IsFly = true;
		m_arbitra = base.Game.CreateBoss(arbitraID, 1250, 600, 1, 1, "", livingConfig2);
		m_arbitra.SetRelateDemagemRect(m_arbitra.NpcInfo.X, m_arbitra.NpcInfo.Y, m_arbitra.NpcInfo.Width, m_arbitra.NpcInfo.Height);
		base.Game.SendObjectFocus(m_arbitra, 1, 1000, 0);
		m_arbitra.Say("Chào mừng quý vị đến với trận đấu boxing gây cấn.", 0, 1500);
		m_arbitra.Say("Có sự hiện diện của Vua Boxing Oai Tử lừng lẫy giang hồ.", 0, 3000);
		base.Game.SendObjectFocus(m_boss, 1, 4000, 0);
		m_moive.PlayMovie("in", 5000, 0);
		m_front.PlayMovie("in", 5100, 0);
		m_moive.PlayMovie("out", 7000, 0);
		m_front.PlayMovie("out", 7100, 0);
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

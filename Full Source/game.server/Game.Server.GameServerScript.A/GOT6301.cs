using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.Messions;

public class GOT6301 : AMissionControl
{
	private SimpleBoss m_boss;

	private int bossID = 6321;

	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

	private PhysicalObj m_leftPoint;

	private PhysicalObj m_rightPoint;

	private PhysicalObj m_leftPoint1;

	private PhysicalObj m_leftPoint2;

	private PhysicalObj m_rightPoint1;

	private PhysicalObj m_rightPoint2;

	private PhysicalObj a1;

	private PhysicalObj a2;

	private PhysicalObj a3;

	private PhysicalObj a4;

	private PhysicalObj a5;

	private PhysicalObj a6;

	private PhysicalObj t1;

	private PhysicalObj t2;

	private PhysicalObj t3;

	private PhysicalObj t4;

	private PhysicalObj t5;

	private PhysicalObj t6;

	private int turn = 0;

	private int[] birthX = new int[18]
	{
		450, 550, 650, 750, 850, 950, 1050, 1150, 1250, 455,
		555, 655, 755, 855, 955, 1055, 1155, 1255
	};

	private int[] birthY = new int[5] { 184, 259, 335, 420, 504 };

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
		int[] npcIds = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.AddLoadingFile(2, "image/game/thing/bossborn6.swf", "game.asset.living.GuizeAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/6/ball.swf", "asset.game.six.ball");
		base.Game.AddLoadingFile(2, "image/game/effect/6/jifenpai.swf", "asset.game.six.fenshu");
		base.Game.AddLoadingFile(2, "image/game/effect/6/jifenpai.swf", "asset.game.six.shuzi");
		base.Game.SetMap(1165);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_leftPoint = base.Game.Createlayer(110, 669, "movie", "asset.game.six.fenshu", "Z", 1, 1);
		m_rightPoint = base.Game.Createlayer(1500, 669, "movie", "asset.game.six.fenshu", "Z", 1, 1);
		m_leftPoint1 = base.Game.Createlayer(185, 669, "movie", "asset.game.six.shuzi", "z0", 1, 1);
		m_leftPoint2 = base.Game.Createlayer(240, 669, "movie", "asset.game.six.shuzi", "z0", 1, 1);
		m_rightPoint1 = base.Game.Createlayer(1575, 669, "movie", "asset.game.six.shuzi", "z0", 1, 1);
		m_rightPoint2 = base.Game.Createlayer(1630, 669, "movie", "asset.game.six.shuzi", "z0", 1, 1);
		m_boss = base.Game.CreateBoss(bossID, 345, 860, -1, 10, "");
		m_boss.PlayMovie("standC", 0, 0);
		m_boss.PlayMovie("go", 1000, 0);
		m_boss.Say("Ô ! Ngưới mới à, chắc bạn không biết quy tắc ở đây.", 0, 0);
		m_boss.CallFuction(NextAttack2, 4000);
	}

	private void NextAttack2()
	{
		base.Game.SendGameFocus(m_boss, 0, 1000);
		m_boss.CallFuction(NextAttack, 3000);
	}

	private void NextAttack()
	{
		m_kingFront = base.Game.Createlayer(900, 400, "font", "game.asset.living.GuizeAsset", "out", 1, 1);
		base.Game.SendGameObjectFocus(1, "font", 0, 1000);
		base.Game.AddAction(new LivingCallFunctionAction(null, CreatBall, 5000));
	}

	private void CreatBall()
	{
		int num = base.Game.Random.Next(-2, 6);
		int num2 = base.Game.Random.Next(-2, 6);
		int num3 = base.Game.Random.Next(-2, 6);
		int num4 = base.Game.Random.Next(-3, 6);
		int num5 = base.Game.Random.Next(-1, 6);
		int num6 = base.Game.Random.Next(-1, 6);
		int num7 = base.Game.Random.Next(-1, 6);
		int num8 = base.Game.Random.Next(-4, 6);
		int num9 = base.Game.Random.Next(1, 6);
		int num10 = base.Game.Random.Next(1, 6);
		int num11 = base.Game.Random.Next(1, 6);
		int num12 = base.Game.Random.Next(-6, 6);
		a1 = base.Game.CreateBall(850, 300, "shield" + num, "s-" + num, 1, 0);
		a2 = base.Game.CreateBall(750, 400, "shield" + num2, "s-" + num2, 1, 0);
		a3 = base.Game.CreateBall(650, 300, "shield" + num3, "s-" + num3, 1, 0);
		a4 = base.Game.CreateBall(950, 400, "shield" + num4, "s-" + num4, 1, 0);
		a5 = base.Game.CreateBall(1050, 300, "shield" + num5, "s-" + num5, 1, 0);
		a6 = base.Game.CreateBall(1150, 400, "shield" + num6, "s-" + num6, 1, 0);
		t1 = base.Game.CreateBall(850, 500, "shield" + num7, "s" + num7, 1, 0);
		t2 = base.Game.CreateBall(750, 600, "shield" + num8, "s" + num8, 1, 0);
		t3 = base.Game.CreateBall(650, 500, "shield" + num9, "s" + num9, 1, 0);
		t4 = base.Game.CreateBall(950, 600, "shield" + num10, "s" + num10, 1, 0);
		t5 = base.Game.CreateBall(1050, 500, "shield" + num11, "s" + num11, 1, 0);
		t6 = base.Game.CreateBall(1150, 600, "shield" + num12, "s" + num12, 1, 0);
		if (m_boss != null)
		{
			m_boss.Die();
			base.Game.RemoveLiving(m_boss.Id);
			m_boss = null;
		}
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.TurnIndex > turn + 2)
		{
			base.Game.RemovePhysicalObj(a1, sendToClient: true);
			base.Game.RemovePhysicalObj(a2, sendToClient: true);
			base.Game.RemovePhysicalObj(a3, sendToClient: true);
			base.Game.RemovePhysicalObj(a4, sendToClient: true);
			base.Game.RemovePhysicalObj(a5, sendToClient: true);
			base.Game.RemovePhysicalObj(a6, sendToClient: true);
			base.Game.RemovePhysicalObj(t1, sendToClient: true);
			base.Game.RemovePhysicalObj(t2, sendToClient: true);
			base.Game.RemovePhysicalObj(t3, sendToClient: true);
			base.Game.RemovePhysicalObj(t4, sendToClient: true);
			base.Game.RemovePhysicalObj(t5, sendToClient: true);
			base.Game.RemovePhysicalObj(t6, sendToClient: true);
			base.Game.AddAction(new LivingCallFunctionAction(null, CreatBall, 500));
		}
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
		if (base.Game.TotalKillCount > 99)
		{
			base.Game.TotalKillCount = 99;
		}
		PlayPoint();
	}

	private void PlayPoint()
	{
		string text = base.Game.TotalKillCount.ToString();
		if (text.Length == 1)
		{
			text = "0" + text;
		}
		m_leftPoint1.PlayMovie("z" + text[0], 100, 500);
		m_leftPoint2.PlayMovie("z" + text[1], 100, 500);
		m_rightPoint1.PlayMovie("z" + text[0], 100, 500);
		m_rightPoint2.PlayMovie("z" + text[1], 100, 500);
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (base.Game.TotalKillCount >= 99)
		{
			base.Game.TotalKillCount = 99;
			PlayPoint();
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
		if (base.Game.TotalKillCount >= 99)
		{
			base.Game.IsWin = true;
		}
		else if (base.Game.TotalTurn > base.Game.PlayerCount * 10)
		{
			base.Game.IsWin = false;
		}
	}
}

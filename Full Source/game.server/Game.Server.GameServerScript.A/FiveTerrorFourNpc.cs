using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveTerrorFourNpc : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private int m_bossId = 0;

	private int m_npcId = 0;

	private static string[] AttackChat = new string[1] { "" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.MaxBeatDis = 200;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (m_targer != null)
		{
			base.Body.ChangeDirection(m_targer, 500);
			base.Body.MoveTo(m_targer.X, m_targer.Y - 20, "fly", 5, 1000, BeatTarget);
		}
	}

	private void BeatTarget()
	{
		base.Body.Beat(m_targer, "beatA", 100, 0, 500);
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

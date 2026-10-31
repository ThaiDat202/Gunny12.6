using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCCau3Mau : ABrain
{
	protected Player m_targer;

	private int m_attackTurn = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 30f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		if (m_attackTurn == 0)
		{
			Move();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			Move();
			m_attackTurn++;
		}
		else
		{
			MoveBeat();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void Move()
	{
		base.Body.MoveTo(base.Game.Random.Next(600, 1350), base.Game.Random.Next(327, 614), "fly", 500, "", 12, null);
	}

	private void MoveBeat()
	{
		base.Body.MoveTo(base.Game.Random.Next(600, 1350), base.Game.Random.Next(327, 614), "fly", 500, "", 12, Beating);
	}

	public void Beating()
	{
		base.Body.PlayMovie("beatA", 2000, 0);
		base.Body.CallFuction(RangeAttacking, 3000);
		base.Body.CallFuction(BodyDie, 4000);
	}

	private void BodyDie()
	{
		((PVEGame)base.Game).ClearAllNpc();
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Body.Game.Map.Info.ForegroundWidth + 1, "cry", 1000, null);
	}
}

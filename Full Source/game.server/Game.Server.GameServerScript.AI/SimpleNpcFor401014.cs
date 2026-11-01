using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SimpleNpcFor401014 : ABrain
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
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (m_targer == null)
		{
			m_targer = ((PVEGame)base.Game).FindPlayer((int)base.Body.Properties1);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
		if (m_attackTurn == 2)
		{
			KillAttack();
			m_attackTurn = 0;
		}
	}

	private void KillAttack()
	{
		base.Body.CurrentDamagePlus = 100000f;
		base.Body.Beat(m_targer, "beatA", 100, 0, 1000);
		base.Body.CallFuction(CheckState, 500);
	}

	private void CheckState()
	{
		if (!m_targer.IsLiving)
		{
			DieState();
		}
	}

	private void DieState()
	{
		if (base.Body.IsLiving)
		{
			base.Body.PlayMovie("die", 500, 500);
			base.Body.Die(500);
			base.Body.SetVisible(state: false);
		}
	}

	public override void OnDie()
	{
		m_targer.SetVisible(state: true);
		m_targer.BlockTurn = false;
		base.Body.Die(500);
		base.Body.SetVisible(state: false);
		((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 1000, 0);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		base.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
	}
}

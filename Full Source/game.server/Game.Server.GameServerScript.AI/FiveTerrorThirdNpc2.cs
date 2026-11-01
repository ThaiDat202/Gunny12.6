using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveTerrorThirdNpc2 : ABrain
{
	protected Player m_targer;

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
		KillAttack();
	}

	private void KillAttack()
	{
		((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 1000, 0);
		base.Body.PlayMovie("beatA", 1500, 0);
		m_targer.Die(3000);
		base.Body.CallFuction(CheckState, 4000);
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
			base.Body.Die(1000);
		}
	}

	public override void OnDie()
	{
		if (m_targer.IsLiving)
		{
			m_targer.BoltMove(((Point)base.Body.Properties2).X, ((Point)base.Body.Properties2).Y, 0);
		}
		m_targer.SetVisible(state: true);
		m_targer.BlockTurn = false;
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

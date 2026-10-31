using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveHardThirdNpc1 : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private int m_bossId = 0;

	private int m_npcId = 0;

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
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.Beat(m_targer, "beatA", 100, 0, 1000);
		base.Body.CallFuction(CheckState, 3000);
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
			base.Body.PlayMovie("die", 500, 2000);
			base.Body.Die(2000);
		}
	}

	public override void OnDie()
	{
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

using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenSimpleNpc : ABrain
{
	private int m_attackTurn = 0;

	private Player m_target;

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
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		switch (m_attackTurn)
		{
		case 0:
			TakeDamage();
			break;
		case 1:
			MakeDead();
			break;
		}
		m_attackTurn++;
	}

	private void TakeDamage()
	{
		if (base.Body.Properties1 != null)
		{
			m_target = base.Game.FindPlayerWithId((int)base.Body.Properties1);
			if (m_target != null && m_target.IsLiving)
			{
				base.Body.PlayMovie("beatA", 500, 0);
				base.Body.BeatDirect(m_target, "", 2000, 1, 1);
			}
		}
	}

	private void MakeDead()
	{
		base.Body.PlayMovie("die", 1000, 0);
		if (m_target != null)
		{
			base.Body.CallFuction(ShowPlayer, 2500);
			m_target.Die(3000);
		}
		base.Body.Die(4000);
	}

	private void ShowPlayer()
	{
		m_target.SetVisible(state: true);
		m_target.BlockTurn = false;
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

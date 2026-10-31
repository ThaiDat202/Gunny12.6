using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveHardFourNpc2 : ABrain
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
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnAfterTakedFrozen()
	{
		base.Body.Properties1 = (int)base.Body.Properties1 + 1;
		SetStand();
		switch ((int)base.Body.Properties1)
		{
		case 1:
			base.Body.PlayMovie("standB", 100, 2000);
			break;
		case 2:
			base.Body.PlayMovie("standC", 100, 2000);
			break;
		}
	}

	private void SetStand()
	{
		switch ((int)base.Body.Properties1)
		{
		case 0:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
			break;
		case 1:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			break;
		case 2:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standC");
			break;
		}
	}
}

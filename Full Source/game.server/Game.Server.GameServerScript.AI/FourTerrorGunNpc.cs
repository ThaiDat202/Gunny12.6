using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourTerrorGunNpc : ABrain
{
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
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		bool flag = false;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
			{
				int num = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 300, base.Body.X + 300);
			return;
		}
		if (m_attackTurn == 0)
		{
			base.Body.PlayMovie("beatA", 1000, 0);
			((PVEGame)base.Game).SendFreeFocus(900, 698, 1, 2600, 0);
			base.Body.CallFuction(SetStand, 3000);
			base.Body.Properties1 = 1;
			m_attackTurn++;
		}
		if (m_attackTurn == 1)
		{
			base.Body.PlayMovie("beatB", 1000, 0);
			((PVEGame)base.Game).SendFreeFocus(900, 698, 2, 2500, 1000);
			base.Body.RangeAttacking(409, 1900, "cry", 4000, removeFrost: false, null);
			base.Body.CallFuction(SetStand, 4000);
			base.Body.Properties1 = 0;
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatC", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 2000, null);
		base.Body.CallFuction(SetStand, 2500);
	}

	private void SetStand()
	{
		int num = (int)base.Body.Properties1;
		int num2 = num;
		if (num2 == 1)
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
		}
		else
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

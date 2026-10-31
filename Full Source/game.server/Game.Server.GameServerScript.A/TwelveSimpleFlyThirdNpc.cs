using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class TwelveSimpleFlyThirdNpc : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive = null;

	private PhysicalObj moive = null;

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
		if (m_attackTurn == 0)
		{
			WalkBeat();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			base.Body.PlayMovie("beatA", 2000, 1000);
			base.Body.CurrentDamagePlus = 10f;
			base.Body.RangeAttacking(base.Body.X - 50, base.Body.X + 50, "cry", 3000, null);
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			WalkBeat();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			m_attackTurn++;
		}
		else
		{
			base.Body.PlayMovie("beatA", 2000, 1000);
			base.Body.CurrentDamagePlus = 10f;
			base.Body.RangeAttacking(base.Body.X - 50, base.Body.X + 50, "cry", 3000, null);
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			m_attackTurn = 0;
		}
	}

	private void WalkBeat()
	{
		int x = base.Game.Random.Next(100, 1800);
		int y = base.Game.Random.Next(400, 600);
		base.Body.MoveTo(x, y, "fly", 1000, "", 10, PlayMovie);
	}

	private void PlayMovie()
	{
		base.Body.PlayMovie("beat", 4000, 1000);
		base.Body.CallFuction(CallPhysicalObj, 4000);
	}

	private void CallPhysicalObj()
	{
		moive = ((PVEGame)base.Game).Createlayer(base.Body.X, 910, "moive", "asset.game.nine.biaoji", "", 1, 0);
	}
}

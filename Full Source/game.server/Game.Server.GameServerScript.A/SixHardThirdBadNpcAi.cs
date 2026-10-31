using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SixHardThirdBadNpcAi : ABrain
{
	private int m_attackTurn = 0;

	private List<PhysicalObj> m_objects = new List<PhysicalObj>();

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		foreach (PhysicalObj @object in m_objects)
		{
			base.Game.RemovePhysicalObj(@object, sendToClient: true);
		}
		m_objects = new List<PhysicalObj>();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.MaxBeatDis = 200;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnAfterTakedBomb()
	{
		base.OnAfterTakedBomb();
	}
}

using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SixHardSecondBoss : ABrain
{
	private int m_redNpc = 6221;

	private int m_blueNpc = 6222;

	private SimpleNpc[] m_blue;

	private SimpleNpc[] m_red;

	private int m_bloodBuff = 2000;

	private List<PhysicalObj> m_phy = new List<PhysicalObj>();

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
		if (m_blue == null || m_red == null)
		{
			m_blue = base.Game.GetNPCLivingWithID(m_blueNpc);
			m_red = base.Game.GetNPCLivingWithID(m_redNpc);
		}
		SimpleNpc[] blue = m_blue;
		foreach (SimpleNpc simpleNpc in blue)
		{
			if (simpleNpc.Blood <= 1)
			{
				base.Body.PlayMovie("beat", 1000, 0);
				(base.Game as PVEGame).SendFreeFocus(1100, 650, 1, 2500, 3000);
				base.Body.CallFuction(RestoreBloodNpcEffect, 3500);
				return;
			}
		}
		base.Body.PlayMovie("beatA", 1000, 0);
		(base.Game as PVEGame).SendFreeFocus(1100, 650, 1, 2500, 3000);
		base.Body.CallFuction(DamageNpcEffect, 3500);
	}

	private void DamageNpcEffect()
	{
		SimpleNpc[] red = m_red;
		foreach (SimpleNpc simpleNpc in red)
		{
			if (!simpleNpc.Config.CompleteStep)
			{
				m_phy.Add((base.Game as PVEGame).Createlayer(simpleNpc.X, simpleNpc.Y, "front", "asset.game.six.qunti", "", 1, 0));
			}
		}
		base.Body.CallFuction(DamageNpcBuff, 500);
	}

	private void DamageNpcBuff()
	{
		SimpleNpc[] red = m_red;
		foreach (SimpleNpc simpleNpc in red)
		{
			if (!simpleNpc.Config.CompleteStep)
			{
				base.Body.BeatDirect(simpleNpc, "", 100, 1, 1);
			}
		}
	}

	private void RestoreBloodNpcEffect()
	{
		SimpleNpc[] blue = m_blue;
		foreach (SimpleNpc simpleNpc in blue)
		{
			if (!simpleNpc.Config.CompleteStep)
			{
				m_phy.Add((base.Game as PVEGame).Createlayer(simpleNpc.X, simpleNpc.Y, "front", "asset.game.six.qunjia", "", 1, 0));
			}
		}
		base.Body.CallFuction(RestoreBloodNpcBuff, 500);
	}

	private void RestoreBloodNpcBuff()
	{
		SimpleNpc[] blue = m_blue;
		foreach (SimpleNpc simpleNpc in blue)
		{
			if (!simpleNpc.Config.CompleteStep)
			{
				simpleNpc.AddBlood(m_bloodBuff);
			}
		}
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

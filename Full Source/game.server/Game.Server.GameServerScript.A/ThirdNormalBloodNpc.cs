using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdNormalBloodNpc : ABrain
{
	private int m_blood = 1000;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		AddBlood();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void AddBlood()
	{
		List<Living> list = base.Game.FindAllTurnBossLiving();
		foreach (Living item in list)
		{
			if (item.IsLiving)
			{
				item.AddBlood(m_blood);
			}
		}
	}
}

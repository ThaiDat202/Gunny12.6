using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenTerrorBrotherNpc : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_friendNpc;

	protected Player m_targer;

	private PhysicalObj phyObjects;

	private int m_friendBoss = 13305;

	private int m_damageBuff = 400;

	private int m_guardBuff = 150;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (m_friendNpc == null)
		{
			SimpleBoss[] array = ((PVEGame)base.Game).FindLivingTurnBossWithID(m_friendBoss);
			if (array.Length != 0)
			{
				m_friendNpc = array[0];
			}
		}
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
		if ((int)base.Body.Properties1 == 0)
		{
			CreateJitan();
		}
		else
		{
			FinishJitan();
		}
	}

	private void FinishJitan()
	{
		base.Body.Properties1 = 0;
		base.Body.Say("Tà thần vĩ đại, hãy ban sức mạnh của ngài cho chúng tôi!", 1, 1000);
		base.Body.PlayMovie("walk", 2000, 0);
		base.Body.BoltMove(1660, 485, 2500);
		List<Player> list = new List<Player>();
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving && allLivingPlayer.X >= 1003 && allLivingPlayer.X <= 1300 && allLivingPlayer.Y <= 620)
			{
				list.Add(allLivingPlayer);
			}
		}
		if (list.Count > 0)
		{
			base.Body.CurrentDamagePlus = 2f;
			base.Body.PlayMovie("castA", 4000, 0);
			phyObjects.PlayMovie("beatA", 5000, 0);
			foreach (Player allLivingPlayer2 in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer2.IsLiving && list.Contains(allLivingPlayer2))
				{
					base.Body.BeatDirect(allLivingPlayer2, "", 7500, 1, 1);
				}
				allLivingPlayer2.AddEffect(new AddDamageTurnEffect(m_damageBuff, 2), 7000);
				allLivingPlayer2.AddEffect(new AddGuardTurnEffect(m_guardBuff, 2), 7000);
			}
			(base.Game as PVEGame).SendObjectFocus(m_friendNpc, 0, 9000, 0);
			m_friendNpc.Say("Có chuyện gì vậy? Cúng tế bị sai cmnr.", 0, 10500, 2000);
			base.Body.PlayMovie("walk", 8000, 0);
			base.Body.BoltMove(1604, 594, 8500);
			base.Body.CallFuction(RemovePhy, 13000);
		}
		else
		{
			base.Body.PlayMovie("beatB", 4000, 0);
			base.Body.CallFuction(RemovePhy, 11000);
			m_friendNpc.Properties1 = 1;
			base.Body.PlayMovie("walk", 12000, 0);
			base.Body.BoltMove(1604, 594, 12500);
		}
		m_friendNpc.Delay = base.Game.GetLowDelayTurn() - 1;
		(base.Body as SimpleBoss).Delay = base.Game.GetHighDelayTurn() + 1;
	}

	private void RemovePhy()
	{
		base.Game.RemovePhysicalObj(phyObjects, sendToClient: true);
	}

	private void CreateJitan()
	{
		base.Body.Properties1 = 1;
		base.Body.Say("Lễ cúng tế sắp bắt đầu! Chúng ta sẽ có sức mạnh của tà thần! Hãy cố lên….", 1, 1000);
		base.Body.PlayMovie("call", 1100, 4000);
		base.Body.CallFuction(CreateJitanObject, 3000);
	}

	private void CreateJitanObject()
	{
		phyObjects = (base.Game as PVEGame).Createlayer(1150, 572, "front", "asset.game.ten.jitan", "1", 1, 0);
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

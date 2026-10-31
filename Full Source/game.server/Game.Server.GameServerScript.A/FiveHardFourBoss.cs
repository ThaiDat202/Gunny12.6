using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveHardFourBoss : ABrain
{
	private int m_attackTurn = 0;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private int NextBloodChange = 0;

	protected Player m_targer;

	private int m_enemy2Id = 5234;

	private int m_npcId = 5232;

	private int m_plusBloodChange = 20;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		foreach (PhysicalObj phyObject in phyObjects)
		{
			base.Game.RemovePhysicalObj(phyObject, sendToClient: true);
		}
		phyObjects = new List<PhysicalObj>();
		if ((int)base.Body.Properties2 == 3)
		{
			NextBloodChange = base.Body.Blood - base.Body.MaxBlood / 100 * m_plusBloodChange;
			if (NextBloodChange < 0)
			{
				NextBloodChange = 0;
			}
			base.Body.Properties2 = 0;
		}
		SetState();
	}

	private void SetState()
	{
		if ((int)base.Body.Properties2 == 1)
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryB");
			base.Body.SetRect(-120, -190, 260, 200);
			base.Body.SetRelateDemagemRect(-120, -190, 260, 200);
		}
		else
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryA");
			base.Body.SetRect(-180, -90, 300, 100);
			base.Body.SetRelateDemagemRect(-60, -200, 116, 100);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
		NextBloodChange = base.Body.MaxBlood - base.Body.MaxBlood / 100 * m_plusBloodChange;
		base.Body.Properties1 = 0;
		base.Body.Properties2 = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		bool flag = false;
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if ((allLivingPlayer.IsLiving && allLivingPlayer.X >= 1580) || allLivingPlayer.X <= 470)
			{
				flag = true;
				return;
			}
		}
		if (flag)
		{
			KillAttack();
			return;
		}
		if ((int)base.Body.Properties1 == 1)
		{
			AttackBeatA();
			base.Body.Properties1 = 0;
			return;
		}
		m_attackTurn++;
		switch (m_attackTurn)
		{
		case 1:
			AttackBeatE();
			break;
		case 2:
			StartBeatA();
			base.Body.Properties1 = 1;
			break;
		case 3:
			AttackBeatC();
			break;
		case 4:
			AttackBeatD();
			m_attackTurn = 0;
			break;
		}
	}

	private void KillAttack()
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatC", 1000, 0);
		base.Body.RangeAttacking(1580, 2000, "cry", 3000, directDamage: true);
	}

	private void StartBeatA()
	{
		base.Body.PlayMovie("beatA", 1000, 4000);
		base.Body.Properties1 = 1;
	}

	private void AttackBeatA()
	{
		bool flag = true;
		base.Body.CurrentDamagePlus = 10f;
		SimpleNpc[] nPCLivingWithID = ((PVEGame)base.Game).GetNPCLivingWithID(m_enemy2Id);
		if (nPCLivingWithID.Length != 0 && (int)nPCLivingWithID[0].Properties1 == 2)
		{
			flag = false;
		}
		if (flag)
		{
			base.Body.PlayMovie("beatF", 1000, 5000);
			base.Body.RangeAttacking(480, 1580, "cry", 4000, directDamage: true);
		}
		else
		{
			base.Body.PlayMovie("beatB", 1000, 0);
			base.Body.RangeAttacking(1330, 2000, "cry", 4000, directDamage: true);
		}
		SimpleNpc[] array = nPCLivingWithID;
		foreach (SimpleNpc simpleNpc in array)
		{
			switch ((int)simpleNpc.Properties1)
			{
			case 0:
				simpleNpc.PlayMovie("die", 3500, 0);
				break;
			case 1:
				simpleNpc.PlayMovie("dieB", 3500, 0);
				break;
			case 2:
				simpleNpc.PlayMovie("dieC", 3500, 0);
				break;
			}
			simpleNpc.Die(5000);
		}
	}

	private void AttackBeatC()
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			base.Body.PlayMovie("beatC", 1000, 0);
			((PVEGame)base.Game).SendObjectFocus(m_targer, 1, 2800, 0);
			base.Body.CallFuction(CreateBeatCEffect, 3500);
			base.Body.RangeAttacking(m_targer.X - 50, m_targer.X + 50, "cry", 4000, directDamage: true);
		}
	}

	private void AttackBeatD()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		base.Body.PlayMovie("beatD", 1000, 0);
		base.Body.CallFuction(CreateBeatDEffect, 1500);
		base.Body.CallFuction(SetDefaultSpeedMult, 5000);
	}

	private void AttackBeatE()
	{
		base.Body.CurrentDamagePlus = 2f;
		base.Body.PlayMovie("beatE", 1000, 5000);
		base.Body.RangeAttacking(480, 1580, "cry", 4000, directDamage: true);
	}

	private void CreateBeatDEffect()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			phyObjects.Add(((PVEGame)base.Game).CreatePhysicalObj(0, 0, "top", "asset.game.4.cuipao", "", 1, 1, allLivingPlayer.Id + 1));
			allLivingPlayer.SpeedMultX(18);
			allLivingPlayer.StartSpeedMult(allLivingPlayer.X - 400, allLivingPlayer.Y, 0);
			base.Body.BeatDirect(allLivingPlayer, "", 100, 3, 1);
		}
	}

	private void SetDefaultSpeedMult()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			allLivingPlayer.SpeedMultX(3);
		}
	}

	private void CreateBeatCEffect()
	{
		phyObjects.Add(((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.guang", "", 1, 1));
	}

	private void CallNpc()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.IsFly = true;
		int x = base.Game.Random.Next(570, 647);
		((SimpleBoss)base.Body).CreateChild(m_npcId, x, 550, showBlood: true, livingConfig);
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
		if (base.Body.Blood <= NextBloodChange && (int)base.Body.Properties2 == 0 && base.Body.IsLiving)
		{
			base.Body.Properties2 = 1;
			base.Body.BlockTurn = true;
			base.Body.PlayMovie("AtoB", 100, 1100);
			SetState();
			base.Body.CallFuction(CallNpc, 1200);
		}
	}
}

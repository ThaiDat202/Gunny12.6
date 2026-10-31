using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenHardDevilBoss : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_friendNpc;

	protected Player m_targer;

	protected Player m_targerBig1000;

	private SimpleNpc m_npc;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private PhysicalObj phyBig1000;

	private List<PhysicalObj> phyIronFire = new List<PhysicalObj>();

	private int m_friendBoss = 13207;

	private int m_npcId = 13210;

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
		if (phyObjects == null || phyObjects.Count <= 0)
		{
			return;
		}
		foreach (PhysicalObj phyObject in phyObjects)
		{
			base.Game.RemovePhysicalObj(phyObject, sendToClient: true);
		}
		phyObjects.Clear();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		switch (m_attackTurn)
		{
		case 0:
			CreateBig1000();
			break;
		case 1:
			DefaultAttack();
			break;
		case 2:
			CreateDamageBox();
			break;
		case 3:
			CreateDamageFire();
			break;
		case 4:
			CreateGlobalAttack();
			m_attackTurn = -1;
			break;
		}
		m_attackTurn++;
	}

	private void CreateGlobalAttack()
	{
		base.Body.ChangeDirection(-1, 500);
		base.Body.MoveTo(900, 400, "fly", 1000, 10, RunGlobalAttack);
	}

	private void RunGlobalAttack()
	{
		base.Body.CurrentDamagePlus = 2.5f;
		base.Body.PlayMovie("beatA", 1200, 0);
		base.Body.CallFuction(CreateGlobalAttackEffect, 2500);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4500, directDamage: true);
		base.Body.PlayMovie("beatC", 4000, 2000);
		base.Body.CallFuction(FlyRandom, 5000);
	}

	private void CreateGlobalAttackEffect()
	{
		phyObjects.Add(((PVEGame)base.Game).CreateLayerTop(500, 300, "", "asset.game.4.heip", "", 1, 1));
	}

	private void CreateDamageFire()
	{
		base.Body.Say(base.Game.ListPlayersName() + ", ta muốn ăn thịt xông khói đây...", 0, 1000);
		base.Body.PlayMovie("beatD", 2000, 4000);
		base.Body.CallFuction(CreateFireEffect, 4000);
	}

	private void CreateFireEffect()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			phyIronFire.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 50, "", "asset.game.ten.laotie", "born", 1, 1));
		}
	}

	private void CreateDamageBox()
	{
		m_targer = base.Game.FindRandomPlayer();
		base.Body.ChangeDirection(base.Game.FindRandomPlayer(), 1000);
		base.Body.Say("Bắt chúng nhốt lại cho ta.", 0, 2000);
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.CallFuction(CreateNpcBoxToTarget, 4000);
		(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 6000, 0);
		base.Body.CallFuction(CreateEffectMovePlayer, 7000);
		base.Body.CallFuction(BlockAndHidePlayer, 8000);
		base.Body.CallFuction(CallMoveAndCloseBox, 8000);
	}

	private void CallMoveAndCloseBox()
	{
		m_targer.BoltMove(m_npc.X, m_npc.Y, 100);
		((PVEGame)base.Game).SendObjectFocus(m_npc, 1, 800, 0);
		m_npc.PlayMovie("AtoB", 1500, 2000);
	}

	private void CreateNpcBoxToTarget()
	{
		int x = base.Game.Random.Next(200, 1700);
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.CanTakeDamage = false;
		m_npc = ((SimpleBoss)base.Body).CreateChild(m_npcId, x, 1000, showBlood: false, livingConfig);
		m_npc.Properties1 = m_targer.Id;
		(base.Game as PVEGame).SendObjectFocus(m_npc, 1, 0, 0);
	}

	private void CreateEffectMovePlayer()
	{
		phyObjects.Add(((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.lanhuo", "", 1, 1));
	}

	private void BlockAndHidePlayer()
	{
		m_targer.SetVisible(state: false);
		m_targer.BlockTurn = true;
	}

	private void DefaultAttack()
	{
		m_targer = base.Game.FindRandomPlayer();
		base.Body.Say("Coi ta cắt chym tụi mi đây.", 0, 1000);
		base.Body.MoveTo(m_targer.X, m_targer.Y, "fly", 1500, 10, AttackTarget);
	}

	private void AttackTarget()
	{
		base.Body.BeatDirect(m_targer, "beatE", 0, 1, 1);
		base.Body.CallFuction(FlyRandom, 2000);
	}

	private void FlyRandom()
	{
		int x = base.Game.Random.Next(100, 1800);
		int y = base.Game.Random.Next(100, 850);
		base.Body.MoveTo(x, y, "fly", 1000, 10, CallAttackFriendlyBoss);
	}

	private void UpdateStatusDamage()
	{
		m_friendNpc.Properties1 = 1;
		m_friendNpc.Config.CancelGuard = true;
		(base.Game as PVEGame).SendLivingActionMapping(m_friendNpc, "stand", "standC");
		(base.Game as PVEGame).SendLivingActionMapping(m_friendNpc, "cry", "standC");
	}

	private void CallAttackFriendlyBoss()
	{
		if (phyBig1000 != null)
		{
			(base.Game as PVEGame).SendObjectFocus(phyBig1000, 1, 1000, 0);
			if (m_friendNpc.X >= phyBig1000.X - 160 && m_friendNpc.X <= phyBig1000.X + 160)
			{
				phyBig1000.PlayMovie("beatB", 2000, 0);
				m_friendNpc.PlayMovie("ready", 3000, 0);
				m_friendNpc.PlayMovie("standC", 3700, 2000);
				base.Body.CallFuction(UpdateStatusDamage, 5700);
			}
			else
			{
				base.Body.CurrentDamagePlus = 10f;
				phyBig1000.PlayMovie("beatA", 2000, 3000);
				foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
				{
					if (allLivingPlayer.X >= phyBig1000.X - 160 && allLivingPlayer.X <= phyBig1000.X + 160)
					{
						base.Body.BeatDirect(allLivingPlayer, "", 3000, 1, 1);
					}
				}
			}
			base.Body.CallFuction(RemoveBig1000Object, 5000);
		}
		else
		{
			if (phyIronFire.Count <= 0)
			{
				return;
			}
			List<Player> list = base.Game.FindRangePlayers(m_friendNpc.X - 100, m_friendNpc.X + 100);
			if (list.Count < 4)
			{
				base.Body.CurrentDamagePlus = 5f;
				{
					foreach (PhysicalObj item in phyIronFire)
					{
						item.PlayMovie("beatA", 1000, 4000);
						list = base.Game.FindRangePlayers(item.X - 25, item.X + 25);
						if (list.Count > 0)
						{
							foreach (Player item2 in list)
							{
								base.Body.BeatDirect(item2, "", 2000, 1, 1);
							}
						}
						base.Body.CallFuction(ClearAllFireDamageEffect, 4600);
					}
					return;
				}
			}
			(base.Game as PVEGame).SendObjectFocus(m_friendNpc, 1, 1000, 0);
			base.Body.CallFuction(ClearAllFireDamageEffect, 2500);
			base.Body.CallFuction(CreateFireIronFriendBoss, 3500);
			foreach (PhysicalObj item3 in phyIronFire)
			{
				item3.PlayMovie("beatA", 5000, 0);
			}
			m_friendNpc.PlayMovie("ready", 6500, 0);
			m_friendNpc.PlayMovie("standC", 7200, 2000);
			base.Body.CallFuction(UpdateStatusDamage, 9200);
		}
	}

	private void CreateFireIronFriendBoss()
	{
		phyIronFire.Add((base.Game as PVEGame).Createlayer(m_friendNpc.X - 100, m_friendNpc.Y - 170, "", "asset.game.ten.laotie2", "", 1, 0));
		phyIronFire.Add((base.Game as PVEGame).Createlayer(m_friendNpc.X - 80, m_friendNpc.Y - 200, "", "asset.game.ten.laotie2", "", 1, 0));
		phyIronFire.Add((base.Game as PVEGame).Createlayer(m_friendNpc.X + 100, m_friendNpc.Y - 170, "", "asset.game.ten.laotie", "", 1, 0));
		phyIronFire.Add((base.Game as PVEGame).Createlayer(m_friendNpc.X + 80, m_friendNpc.Y - 200, "", "asset.game.ten.laotie", "", 1, 0));
	}

	private void ClearAllFireDamageEffect()
	{
		foreach (PhysicalObj item in phyIronFire)
		{
			base.Game.RemovePhysicalObj(item, sendToClient: true);
		}
		phyIronFire = new List<PhysicalObj>();
	}

	private void CreateBig1000()
	{
		m_targerBig1000 = base.Game.FindRandomPlayer();
		base.Body.Say("Sao to và nặng value thế nhỉ?", 0, 1000);
		base.Body.PlayMovie("beatD", 2000, 0);
		(base.Game as PVEGame).SendFreeFocus(m_targerBig1000.X, m_targerBig1000.Y - 150, 1, 4000, 3000);
		base.Body.CallFuction(CreateBig1000Object, 5000);
	}

	private void CreateBig1000Object()
	{
		phyBig1000 = (base.Game as PVEGame).Createlayer(m_targerBig1000.X, m_targerBig1000.Y, "", "asset.game.ten.chengtuo", "", 1, 0);
	}

	private void RemoveBig1000Object()
	{
		base.Game.RemovePhysicalObj(phyBig1000, sendToClient: true);
		phyBig1000 = null;
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

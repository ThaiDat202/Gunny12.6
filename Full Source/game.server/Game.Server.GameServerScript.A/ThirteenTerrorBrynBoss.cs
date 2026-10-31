using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenTerrorBrynBoss : ABrain
{
	private int m_attackTurn = 0;

	private int m_countNormalAttack = 0;

	private SimpleBoss m_friendNpc;

	protected Player m_targer;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private int m_friendBoss = 13304;

	private int m_fearLength = 80;

	private int m_healBlood = 12000;

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
		m_countNormalAttack = 0;
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (phyObjects != null && phyObjects.Count > 0)
		{
			foreach (PhysicalObj phyObject in phyObjects)
			{
				base.Game.RemovePhysicalObj(phyObject, sendToClient: true);
			}
			phyObjects.Clear();
		}
		if ((int)base.Body.Properties1 == 2)
		{
			(base.Game as PVEGame).SendPlayersPicture(base.Body, 29, state: false);
			(base.Game as PVEGame).SendPlayersPicture(base.Body, 30, state: false);
			base.Body.Properties1 = 0;
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		List<Player> list = new List<Player>();
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving && base.Body.X - 150 <= allLivingPlayer.X && base.Body.X + 150 >= allLivingPlayer.X && allLivingPlayer.Y >= 780)
			{
				list.Add(allLivingPlayer);
			}
		}
		if (list.Count > 0)
		{
			DirectKill(list);
			return;
		}
		switch (m_attackTurn)
		{
		case 0:
			FirstAttack();
			break;
		case 1:
			NextAttack();
			break;
		case 2:
			CallBuff();
			break;
		case 3:
			UseBuff();
			break;
		case 4:
			NormalAttack();
			m_attackTurn = -1;
			break;
		}
		m_attackTurn++;
	}

	private void NormalAttack()
	{
		m_friendNpc.Delay = base.Game.GetHighDelayTurn() + 1;
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null && m_targer.IsLiving)
		{
			base.Body.ChangeDirection(m_targer, 1000);
			if (m_targer.Y > 620)
			{
				if (base.Body.ShootPoint(m_targer.X, m_targer.Y, 55, 1000, 10000, 1, 1.5f, 2900))
				{
					base.Body.PlayMovie("beatB", 2000, 3000);
				}
			}
			else
			{
				base.Body.CurrentDamagePlus = 15f;
				base.Body.Say("Ngươi dám leo lên trên đó à? Chết đi!!!", 0, 2000);
				base.Body.PlayMovie("beatC", 3000, 0);
				base.Body.BeatDirect(m_targer, "", 4000, 3, 1);
			}
		}
		if (m_countNormalAttack == 0)
		{
			base.Body.CallFuction(NormalAttack, 4000);
		}
		m_countNormalAttack++;
	}

	private void UseBuff()
	{
		m_friendNpc.Delay = base.Game.GetHighDelayTurn() + 1;
		if ((int)base.Body.Properties1 == 1)
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say("Hãy xem sức mạnh của tà thần đây. Đây mới chính là sức mạnh thực sự của ta.", 1, 1000);
			(base.Game as PVEGame).SendPlayersPicture(base.Body, 29, state: true);
			(base.Game as PVEGame).SendPlayersPicture(base.Body, 30, state: true);
			base.Body.CallFuction(CallCompleteBuffEffect, 2000);
			base.Body.Properties1 = 2;
		}
		else
		{
			base.Body.CurrentDamagePlus = 2.3f;
			base.Body.Say("Đậu xanh rau muống dám phá nghi lễ của ta.", 1, 2000);
			base.Body.PlayMovie("beatC", 3000, 0);
			base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "", 4000, directDamage: false);
		}
	}

	private void CallCompleteBuffEffect()
	{
		phyObjects.Add((base.Game as PVEGame).Createlayer(base.Body.X, base.Body.Y, "", "asset.game.ten.up", "", 1, 0));
		base.Body.PlayMovie("beatC", 2000, 5000);
		base.Body.CallFuction(CallHealBlood, 3000);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "", 4000, directDamage: false);
	}

	private void CallHealBlood()
	{
		base.Body.AddBlood(m_healBlood);
	}

	private void CallBuff()
	{
		base.Body.Say("Hãy mau thực hiện nghi thức cúng tế, sức mạnh tà thần sẽ thuộc về ta!", 1000, 3000);
		base.Body.Properties1 = 0;
		m_friendNpc.Delay = base.Game.GetLowDelayTurn() - 1;
	}

	private void NextAttack()
	{
		m_friendNpc.Delay = base.Game.GetHighDelayTurn() + 1;
		base.Body.PlayMovie("callA", 1000, 0);
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 2000, 0);
		}
		base.Body.CallFuction(CallObjectNextAttack, 2800);
	}

	private void CallObjectNextAttack()
	{
		base.Body.CurrentDamagePlus = 1.8f;
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			switch (base.Game.Random.Next(0, 3))
			{
			case 0:
				phyObjects.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.jiaodu", "", 1, 0));
				allLivingPlayer.AddEffect(new LockDirectionEffect(2), 1500);
				break;
			case 1:
				phyObjects.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.pilao", "", 1, 0));
				allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, m_fearLength), 1500);
				break;
			default:
				phyObjects.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.pilao", "", 1, 0));
				base.Body.BeatDirect(allLivingPlayer, "", 1500, 1, 1);
				break;
			}
			base.Body.BeatDirect(allLivingPlayer, "", 1500, 1, 1);
		}
	}

	private void FirstAttack()
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			(base.Game as PVEGame).SendObjectFocus(m_targer, 1, 2000, 0);
		}
		base.Body.PlayMovie("callA", 1000, 0);
		base.Body.CallFuction(CreateFirstDamageEffect, 3000);
	}

	private void CreateFirstDamageEffect()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			phyObjects.Add((base.Game as PVEGame).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.baozha", "", 1, 0));
		}
		base.Body.CallFuction(MoveAllPlayerEffect, 1200);
	}

	private void MoveAllPlayerEffect()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving)
			{
				allLivingPlayer.SpeedMultX(20);
				if (base.Body.FindDirection(allLivingPlayer) == 1)
				{
					allLivingPlayer.StartSpeedMult(allLivingPlayer.X - base.Game.Random.Next(400, 600), allLivingPlayer.Y, 0);
				}
				else
				{
					allLivingPlayer.StartSpeedMult(allLivingPlayer.X + base.Game.Random.Next(400, 600), allLivingPlayer.Y, 0);
				}
			}
			base.Body.BeatDirect(allLivingPlayer, "", 1500, 1, 1);
		}
		base.Body.CallFuction(SetDefaultSpeedMult, 5000);
	}

	private void SetDefaultSpeedMult()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			allLivingPlayer.SpeedMultX(3);
		}
	}

	private void DirectKill(List<Player> players)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.Say("Chán sống hay sao mà đứng gần ta?", 1, 1000);
		base.Body.PlayMovie("beatC", 2000, 0);
		foreach (Player player in players)
		{
			base.Body.BeatDirect(player, "", 3500, 1, 1);
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

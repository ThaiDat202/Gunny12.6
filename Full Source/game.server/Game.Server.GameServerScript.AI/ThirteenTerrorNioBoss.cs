using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenTerrorNioBoss : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_friendNpc;

	protected Player m_targer;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private int m_friendBoss = 13301;

	private int m_healBloodSelf = 30000;

	private int m_healBloodFriend = 130000;

	private string[] HealChat = new string[4] { "Phê phê phê. Coi đánh ta kiểu gì?", "Ôi thật thần kỳ. Buff máu phê vãi", "Buff cho phê, phê như con tê tê..", "Đánh bại ta đâu có dễ. Coi ta này." };

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
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		List<Player> list = new List<Player>();
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving && allLivingPlayer.X > 1066)
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
			BeatA();
			break;
		case 1:
			HealBlood();
			break;
		case 2:
			BeatFrozen();
			m_attackTurn = -1;
			break;
		}
		m_attackTurn++;
	}

	private void BeatFrozen()
	{
		List<Player> list = new List<Player>();
		if (m_friendNpc.IsLiving)
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.IsFrost)
				{
					list.Add(allLivingPlayer);
				}
			}
		}
		base.Body.PlayMovie("beat", 1000, 0);
		if (list.Count > 0)
		{
			base.Body.CurrentDamagePlus = 1000f;
			{
				foreach (Player item in list)
				{
					base.Body.BeatDirect(item, "", 3200, 1, 1);
				}
				return;
			}
		}
		base.Body.CurrentDamagePlus = 1.5f;
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 3200, directDamage: false);
	}

	private void BeatA()
	{
		base.Body.CurrentDamagePlus = 1.8f;
		m_targer = base.Game.FindRandomPlayer();
		base.Body.ChangeDirection(m_targer, 500);
		if (base.Body.ShootPoint(m_targer.X, m_targer.Y, 61, 1000, 10000, 1, 1.7f, 1900))
		{
			base.Body.PlayMovie("beat2", 1000, 4000);
		}
	}

	private void HealBlood()
	{
		int num = base.Game.Random.Next(0, HealChat.Length);
		base.Body.Say(HealChat[num], 0, 1000);
		base.Body.PlayMovie("renew", 1500, 3500);
		base.Body.CallFuction(HealBloodCallBack, 3000);
	}

	private void HealBloodCallBack()
	{
		base.Body.AddBlood(m_healBloodSelf);
		if (m_friendNpc.IsLiving)
		{
			(base.Game as PVEGame).SendObjectFocus(m_friendNpc, 0, 1500, 0);
			base.Body.CallFuction(HealFriendBlood, 3000);
		}
	}

	private void HealFriendBlood()
	{
		m_friendNpc.AddBlood(m_healBloodFriend);
	}

	private void DirectKill(List<Player> players)
	{
		base.Body.CurrentDamagePlus = 1000f;
		foreach (Player player in players)
		{
			base.Body.BeatDirect(player, "", 2000, 1, 1);
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

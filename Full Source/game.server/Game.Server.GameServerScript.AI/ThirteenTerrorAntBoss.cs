using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenTerrorAntBoss : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_friendNpc;

	protected Player m_targer;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private int m_friendBoss = 13302;

	private string[] GlobalAttackChat = new string[4] { "Hãy liếm thử mũi tên này của ta", "Xem có thốn không nhé", "Ta sẽ hạ sát hết lũ các ngươi", "Các ngươi nghĩ đủ trình hạ ta?." };

	private string[] FrozenAttackChat = new string[3] { "Băng trâm tiễn.", "Liếm thử băng trâm tiễn của ta", "Để ta giúp ngươi về trời nhanh." };

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
		if (phyObjects.Count <= 0)
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
			GlobalAttack();
			break;
		case 1:
			FrozenAttack();
			break;
		case 2:
			PersonAttack();
			m_attackTurn = -1;
			break;
		}
		m_attackTurn++;
	}

	private void PersonAttack()
	{
		base.Body.CurrentDamagePlus = 1.3f;
		m_targer = base.Game.FindRandomPlayer();
		if (base.Body.ShootPoint(m_targer.X, m_targer.Y, 51, 1000, 10000, 1, 3f, 2800))
		{
			base.Body.PlayMovie("beatD", 1000, 3000);
		}
	}

	private void FrozenAttack()
	{
		int num = base.Game.Random.Next(0, FrozenAttackChat.Length);
		base.Body.Say(FrozenAttackChat[num], 0, 1000, 0);
		Player[] array = base.Game.FindRandomPlayer(2);
		if (array.Length != 0)
		{
			if (base.Body.ShootPoint(array[0].X - 20, array[0].Y, 99, 1000, 10000, 1, 3f, 3000))
			{
				base.Body.PlayMovie("beatD", 1500, 3000);
			}
			if (array.Length == 2 && base.Body.ShootPoint(array[1].X - 20, array[1].Y, 99, 1000, 10000, 1, 3f, 5000))
			{
				base.Body.PlayMovie("beatD", 3500, 3000);
			}
		}
	}

	private void GlobalAttack()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		int num = base.Game.Random.Next(0, GlobalAttackChat.Length);
		base.Body.Say(GlobalAttackChat[num], 0, 1000, 6000);
		base.Body.PlayMovie("beatC", 1500, 0);
		m_targer = base.Game.FindRandomPlayer();
		(base.Game as PVEGame).SendObjectFocus(m_targer, 0, 3000, 0);
		base.Body.CallFuction(CreateEffectGlobalAttack, 4000);
	}

	private void CreateEffectGlobalAttack()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			phyObjects.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.ten.jianyu", "", 1, 1));
			base.Body.BeatDirect(allLivingPlayer, "", 2000, 3, 1);
		}
	}

	private void DirectKill(List<Player> players)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatB", 1000, 0);
		foreach (Player player in players)
		{
			base.Body.BeatDirect(player, "", 2500, 1, 1);
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

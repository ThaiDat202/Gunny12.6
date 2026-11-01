using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourHardHawkNpc : ABrain
{
	private int m_attackTurn = 0;

	private List<PhysicalObj> m_featherEffect = new List<PhysicalObj>();

	private int m_totalNpc = 4;

	private int npcId = 4202;

	private int friendlyNpcId = 4206;

	private SimpleBoss friendBoss = null;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		ClearFeatherEffect();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		if ((int)base.Body.Properties1 == 0)
		{
			base.Body.Config.IsShield = true;
		}
		else
		{
			base.Body.Config.IsShield = false;
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
	}

	public override void OnStartAttacking()
	{
		if (m_attackTurn == 0)
		{
			if ((int)base.Body.Properties1 == 0)
			{
				FlyRandom(AllAttackFeather);
			}
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			if ((int)base.Body.Properties1 == 0)
			{
				FlyRandom(AllAttackFeather);
			}
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			if ((int)base.Body.Properties1 == 0)
			{
				FlyRandom(ChangeAToB);
				base.Body.Properties1 = 1;
			}
			else
			{
				ChangeBToA();
				base.Body.Properties1 = 0;
			}
			m_attackTurn = 0;
		}
	}

	public override void OnBeforeTakedDamage(Living source, ref int damage, ref int crit)
	{
		if ((int)base.Body.Properties1 == 1)
		{
			if (friendBoss == null)
			{
				GetFriendBoss();
			}
			if (damage > 0 || crit > 0)
			{
				friendBoss.TakeDamage(source, ref damage, ref crit, "");
			}
		}
		base.OnBeforeTakedDamage(source, ref damage, ref crit);
	}

	private void GetFriendBoss()
	{
		SimpleBoss[] array = base.Game.FindLivingTurnBossWithID(friendlyNpcId);
		int num = 0;
		if (num < array.Length)
		{
			SimpleBoss simpleBoss = (friendBoss = array[num]);
		}
	}

	private void FlyRandom(LivingCallBack callBack)
	{
		int x = base.Game.Random.Next(400, 1283);
		int y = base.Game.Random.Next(400, 654);
		base.Body.MoveTo(x, y, "fly", 1000, 7, callBack);
	}

	private void CreateNpc()
	{
		for (int i = 0; i < m_totalNpc; i++)
		{
			int num = base.Body.X + base.Game.Random.Next(-300, 300);
			int num2 = base.Body.Y + base.Game.Random.Next(0, 300);
			num = ((num >= 0) ? num : 0);
			num = ((num > base.Game.Map.Info.DeadWidth) ? base.Game.Map.Info.DeadWidth : num);
			num2 = ((num2 > 778) ? 778 : num2);
			LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
			livingConfig.IsFly = true;
			((SimpleBoss)base.Body).CreateChild(npcId, num, num2, showBlood: false, livingConfig);
		}
		base.Body.CallFuction(SetState, 2000);
	}

	private void RemoveAllNpc()
	{
		((SimpleBoss)base.Body).RemoveAllChild();
	}

	private void AllAttackFeather()
	{
		base.Body.PlayMovie("beatA", 500, 0);
		Player obj = base.Game.FindRandomPlayer();
		((PVEGame)base.Game).SendObjectFocus(obj, 1, 1500, 0);
		base.Body.CallFuction(CreateFeatherEffect, 2000);
		base.Body.RangeAttacking(0, base.Game.Map.Info.DeadWidth, "cry", 2100, null);
	}

	private void CreateFeatherEffect()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			m_featherEffect.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.4.feather", "", 1, 0));
		}
	}

	private void ClearFeatherEffect()
	{
		foreach (PhysicalObj item in m_featherEffect)
		{
			base.Game.RemovePhysicalObj(item, sendToClient: true);
		}
		m_featherEffect = new List<PhysicalObj>();
	}

	private void ChangeAToB()
	{
		base.Body.PlayMovie("AtoB", 500, 0);
		base.Body.CallFuction(CreateNpc, 3500);
	}

	private void ChangeBToA()
	{
		RemoveAllNpc();
		base.Body.PlayMovie("BtoA", 1000, 0);
		base.Body.CallFuction(SetState, 4000);
	}

	private void SetState()
	{
		if ((int)base.Body.Properties1 == 1)
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
		}
		else
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

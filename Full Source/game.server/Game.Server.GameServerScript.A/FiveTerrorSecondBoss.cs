using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveTerrorSecondBoss : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private SimpleBoss m_leftNpc;

	private SimpleBoss m_rightNpc;

	private SimpleNpc m_bottomNpc;

	private SimpleNpc m_centerNpc;

	private List<PhysicalObj> effectPhys = new List<PhysicalObj>();

	private bool StepCenter = true;

	private int npcLeftId = 5312;

	private int npcRightId = 5313;

	private int npcBottomId = 5316;

	private int npcCenterId = 5317;

	private int m_bloodBuff = 15000;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (m_leftNpc == null || m_rightNpc == null)
		{
			List<Living> list = base.Game.FindAllTurnBossLiving();
			foreach (Living item in list)
			{
				if (((SimpleBoss)item).NpcInfo.ID == npcLeftId)
				{
					m_leftNpc = item as SimpleBoss;
				}
				else if (((SimpleBoss)item).NpcInfo.ID == npcRightId)
				{
					m_rightNpc = item as SimpleBoss;
				}
			}
		}
		if (m_bottomNpc == null || m_centerNpc == null)
		{
			SimpleNpc[] array = base.Game.FindAllNpc();
			foreach (SimpleNpc simpleNpc in array)
			{
				if (simpleNpc.NpcInfo.ID == npcBottomId)
				{
					m_bottomNpc = simpleNpc;
				}
				else if (simpleNpc.NpcInfo.ID == npcCenterId)
				{
					m_centerNpc = simpleNpc;
				}
			}
			m_bottomNpc.Properties1 = 0;
			m_centerNpc.Properties1 = 0;
		}
		if (effectPhys.Count <= 0)
		{
			return;
		}
		foreach (PhysicalObj effectPhy in effectPhys)
		{
			base.Game.RemovePhysicalObj(effectPhy, sendToClient: true);
		}
		effectPhys = new List<PhysicalObj>();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		StepCenter = true;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		bool flag = false;
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.X > 1138)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			KillAttack();
			return;
		}
		m_attackTurn++;
		switch (m_attackTurn)
		{
		case 1:
			if (m_leftNpc.IsLiving)
			{
				BeatFromLeft();
			}
			else
			{
				PersonAttack(null);
			}
			break;
		case 2:
		{
			LivingCallBack livingCallBack = null;
			livingCallBack = ((!StepCenter) ? new LivingCallBack(BottomReadyShoot) : new LivingCallBack(CenterReadyShoot));
			if (m_rightNpc.IsLiving)
			{
				BeatFromRight(livingCallBack);
			}
			else
			{
				PersonAttack(livingCallBack);
			}
			break;
		}
		case 3:
			if (StepCenter)
			{
				StepCenter = false;
				CenterShoot();
			}
			else
			{
				StepCenter = true;
				BottomShoot();
			}
			m_attackTurn = 0;
			break;
		}
	}

	private void KillAttack()
	{
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.RangeAttacking(1138, 2000, "cry", 4000, directDamage: true);
	}

	private void PersonAttack(LivingCallBack callBack)
	{
		m_targer = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.CallFuction(CreateEffectPersonTarget, 3600);
		base.Body.BeatDirect(m_targer, "", 4000, 1, 1);
		if (callBack != null)
		{
			base.Body.CallFuction(callBack, 5000);
		}
	}

	private void BeatFromLeft()
	{
		base.Body.PlayMovie("beatD", 1000, 0);
		m_leftNpc.PlayMovie("failB", 1900, 0);
		m_leftNpc.PlayMovie("beatA", 3000, 4500);
		((PVEGame)base.Game).SendFreeFocus(618, 585, 1, 5000, 0);
		m_leftNpc.CallFuction(CreateEffectBeatLeft, 5800);
		m_leftNpc.RangeAttacking(m_leftNpc.X - 10000, m_leftNpc.X + 10000, "cry", 6300, null);
	}

	private void BeatFromRight(LivingCallBack callBack)
	{
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer != null)
		{
			base.Body.PlayMovie("beatC", 1000, 0);
			base.Body.CallFuction(AddBloodToAll, 4000);
			if (m_rightNpc.ShootPoint(m_targer.X - 15, m_targer.Y - 15, 56, 1000, 10000, 1, 2.5f, 6200))
			{
				m_rightNpc.PlayMovie("beatA", 3500, 0);
			}
			if (callBack != null)
			{
				base.Body.CallFuction(callBack, 10000);
			}
		}
	}

	private void BottomReadyShoot()
	{
		m_bottomNpc.Properties1 = 1;
		((PVEGame)base.Game).SendObjectFocus(m_bottomNpc, 1, 1000, 0);
		m_bottomNpc.PlayMovie("toA", 2000, 3000);
		ChangeStateBottom();
	}

	private void BottomShoot()
	{
		m_bottomNpc.Properties1 = 0;
		m_bottomNpc.CurrentDamagePlus = 5f;
		((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 1000, 0);
		base.Body.PlayMovie("beatC", 2000, 0);
		((PVEGame)base.Game).SendObjectFocus(m_bottomNpc, 1, 5000, 0);
		m_bottomNpc.PlayMovie("beatA", 6000, 5000);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.Y > 760)
			{
				m_bottomNpc.BeatDirect(allLivingPlayer, "", 8000, 1, 1);
			}
		}
		ChangeStateBottom();
	}

	private void CenterReadyShoot()
	{
		m_centerNpc.Properties1 = 1;
		m_bottomNpc.CurrentDamagePlus = 2f;
		((PVEGame)base.Game).SendObjectFocus(m_centerNpc, 1, 1000, 0);
		m_centerNpc.PlayMovie("toA", 2000, 3000);
	}

	private void CenterShoot()
	{
		m_centerNpc.Properties1 = 0;
		((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 1000, 0);
		base.Body.PlayMovie("beatB", 2000, 0);
		m_centerNpc.PlayMovie("beatA", 5000, 6000);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if ((allLivingPlayer.X <= 166 && allLivingPlayer.Y <= 760) || (allLivingPlayer.X >= 342 && allLivingPlayer.X <= 637 && allLivingPlayer.Y <= 760) || (allLivingPlayer.X >= 820 && allLivingPlayer.X <= 1116 && allLivingPlayer.Y <= 760) || (allLivingPlayer.X > 166 && allLivingPlayer.X < 342 && allLivingPlayer.Y > 760) || (allLivingPlayer.X > 637 && allLivingPlayer.X < 820 && allLivingPlayer.Y > 760))
			{
				m_centerNpc.BeatDirect(allLivingPlayer, "", 11000, 1, 1);
			}
		}
	}

	private void ChangeStateBottom()
	{
		if ((int)m_bottomNpc.Properties1 == 1)
		{
			((PVEGame)base.Game).SendLivingActionMapping(m_bottomNpc, "stand", "standA");
		}
		else
		{
			((PVEGame)base.Game).SendLivingActionMapping(m_bottomNpc, "stand", "stand");
		}
	}

	private void CreateEffectBeatLeft()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			effectPhys.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.4.xiaopao", "", 1, 1));
		}
	}

	private void CreateEffectPersonTarget()
	{
		if (m_targer != null)
		{
			effectPhys.Add(((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.jinqudan", "", 1, 1));
		}
	}

	private void AddBloodToAll()
	{
		base.Body.AddBlood(m_bloodBuff);
		m_leftNpc.AddBlood(m_bloodBuff);
		m_rightNpc.AddBlood(m_bloodBuff);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SimpleBossFor34012 : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private PhysicalObj moive;

	private PhysicalObj effectPhy;

	private int m_bloodReduce = 3000;

	private List<Player> players = null;

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
		base.Body.MaxBeatDis = 300;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 300, base.Body.X + 300);
		}
		else if (m_attackTurn == 0)
		{
			Attack1();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			m_attackTurn++;
		}
		else
		{
			Attack2();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10000f;
		base.Body.RangeAttacking(fx, tx, "cry", 2000, null);
	}

	private void Attack1()
	{
		base.Body.MoveTo(333, 313, "run", 1000, "", 100, EffectAttack);
	}

	private void Attack2()
	{
		base.Body.MoveTo(533, 313, "run", 1000, "", 100, EffectAttack);
	}

	private void EffectAttack()
	{
		base.Body.ChangeDirection(-1, 0);
		base.Body.PlayMovie("beatD", 500, 0);
		base.Body.Say("憨憨们，我要毒死你们!", 4500, 0);
		base.Body.CallFuction(CallEffectRandomPlace, 5000);
	}

	private void CallEffectRandomPlace()
	{
		ClearEffect();
		m_targer = base.Game.FindRandomPlayer();
		if (m_targer == null)
		{
			return;
		}
		((PVEGame)base.Game).SendObjectFocus(m_targer, 0, 0, 0);
		base.Body.CallFuction(CreateEffectMovie, 1000);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.X > m_targer.X - 180 && allLivingPlayer.X < m_targer.X + 180)
			{
				allLivingPlayer.AddEffect(new ContinueReduceGreenBloodEffect(5, m_bloodReduce, base.Body), 1500);
			}
		}
		base.Body.CallFuction(RunBackDefaultPoint, 3000);
	}

	private void CreateEffectMovie()
	{
		if (m_targer != null)
		{
			moive = ((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.seven.choud", "", 1, 1);
			effectPhy = ((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.seven.du", "", 1, 1);
			base.Body.CallFuction(ClearMovie, 3000);
		}
	}

	private void ClearEffect()
	{
		if (effectPhy != null)
		{
			base.Game.RemovePhysicalObj(effectPhy, sendToClient: true);
		}
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.EffectList.GetOfType(eEffectType.ContinueReduceGreenBloodEffect) is ContinueReduceGreenBloodEffect continueReduceGreenBloodEffect)
			{
				continueReduceGreenBloodEffect.Stop();
			}
		}
	}

	private void ClearMovie()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
		}
	}

	private void RunBackDefaultPoint()
	{
		base.Body.MoveTo(433, 590, "run", 1000, "", 100, ChangeDir);
	}

	private void ChangeDir()
	{
		if (base.Body.X > 1000)
		{
			base.Body.ChangeDirection(-1, 100);
		}
		else
		{
			base.Body.ChangeDirection(1, 100);
		}
	}

	public override void OnDie()
	{
		base.OnDie();
		ClearEffect();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

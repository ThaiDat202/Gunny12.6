using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveTerrorThirdNpc : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private PhysicalObj deadEffect;

	private int m_bloodReduce = 800;

	private static string[] AttackChat = new string[1] { "" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (deadEffect != null)
		{
			base.Game.RemovePhysicalObj(deadEffect, sendToClient: true);
		}
		deadEffect = null;
		if (m_targer != null)
		{
			m_targer.SpeedMultX(3);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		FindTarget();
	}

	private void FindTarget()
	{
		int num = int.MaxValue;
		int num2 = int.MinValue;
		Player player = null;
		Player player2 = null;
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.X > 1000 && allLivingPlayer.X > num2)
			{
				num2 = allLivingPlayer.X;
				player2 = allLivingPlayer;
			}
			else if (allLivingPlayer.X <= 1000 && allLivingPlayer.X < num)
			{
				num = allLivingPlayer.X;
				player = allLivingPlayer;
			}
		}
		if (player == null && player2 != null)
		{
			m_targer = player2;
		}
		else if (player2 == null && player != null)
		{
			m_targer = player;
		}
		else if (player2 != null && player != null)
		{
			double num3 = base.Body.Distance(player.X, player.Y);
			double num4 = base.Body.Distance(player2.X, player2.Y);
			if (num3 < num4)
			{
				m_targer = player;
			}
			else
			{
				m_targer = player2;
			}
		}
		else
		{
			m_targer = base.Game.FindRandomPlayer();
		}
		MoveAndAttack();
	}

	private void MoveAndAttack()
	{
		if (m_targer != null)
		{
			base.Body.MoveTo(m_targer.X, m_targer.Y, "fly", 1000, 6, AttackTarget);
		}
	}

	private void AttackTarget()
	{
		base.Body.MaxBeatDis = 500;
		m_targer.SpeedMultX(18);
		base.Body.PlayMovie("beatA", 500, 0);
		base.Body.CallFuction(CreateDeadEffect, 1500);
		base.Body.BeatDirect(m_targer, "", 1600, 1, 1);
		base.Body.CallFuction(StartMoveTarget, 1700);
		m_targer.AddEffect(new ContinueReduceBloodEffect(1, m_bloodReduce, base.Body), 3200);
		base.Body.CallFuction(MoveToUp, 2500);
	}

	private void MoveToUp()
	{
		base.Body.MoveTo(base.Body.X, base.Body.Y - 50, "fly", 0, 6);
	}

	private void StartMoveTarget()
	{
		if (m_targer.X > 1000)
		{
			m_targer.StartSpeedMult(m_targer.X + 150, m_targer.Y, 0);
		}
		else
		{
			m_targer.StartSpeedMult(m_targer.X - 150, m_targer.Y, 0);
		}
	}

	private void CreateDeadEffect()
	{
		deadEffect = ((PVEGame)base.Game).Createlayer(m_targer.X, m_targer.Y, "", "asset.game.4.tang", "", 1, 1);
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		base.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
	}
}

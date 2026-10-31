using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCKhiChuaGianDu : ABrain
{
	protected Player m_targer;

	private int m_turn = 0;

	private PhysicalObj moive;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 500f;
		base.Body.CurrentShootMinus = 500f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1150)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 150, base.Body.X + 150);
			return;
		}
		m_targer = base.Game.FindRandomPlayer();
		if (m_turn == 0)
		{
			TankA();
			m_turn++;
		}
		else
		{
			TankB();
			m_turn = 0;
		}
	}

	private void TankA()
	{
		base.Body.CurrentDamagePlus = base.Game.Random.Next(150, 200);
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 100, m_targer.X + 100, "cry", 4000, null);
	}

	private void TankB()
	{
		base.Body.CurrentDamagePlus = base.Game.Random.Next(150, 200);
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.RangeAttacking(m_targer.X - 100, m_targer.X + 100, "cry", 4000, null);
		base.Body.CallFuction(SkillBong, 3000);
	}

	private void SkillBong()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ReduceStrengthEffect(2, 1000), 0);
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10000f;
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void GoOut()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}
}

using System.Collections.Generic;
using Game.Logic;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCLinhHon : ABrain
{
	protected Player m_targer;

	private PhysicalObj moive;

	private List<Player> m_denyPlayer;

	private int m_attackTurn = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 30f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		m_denyPlayer = new List<Player>();
	}

	public override void OnStartAttacking()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (moive != null)
		{
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			m_denyPlayer.Clear();
			foreach (Player item in allFightPlayers)
			{
				bool flag = false;
				if (!item.IsLiving)
				{
					continue;
				}
				if (item.X < moive.X)
				{
					if (moive.X - item.X < 100)
					{
						flag = true;
					}
				}
				else if (item.X > moive.X)
				{
					if (item.X - moive.X < 100)
					{
						flag = true;
					}
				}
				else if (item.X == moive.X)
				{
					flag = true;
				}
				if (flag)
				{
					m_denyPlayer.Add(item);
				}
			}
		}
		if (m_denyPlayer.Count > 0)
		{
			TankC();
		}
		else if (m_attackTurn == 0)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			TankA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			TankB();
			m_attackTurn = 2;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void AddBloodA()
	{
		for (int i = 0; i < m_denyPlayer.Count; i++)
		{
			m_denyPlayer[i].AddBlood(-100000, 1);
			if (m_denyPlayer[i].Blood <= 0)
			{
				m_denyPlayer[i].Die();
			}
		}
	}

	private void TankA()
	{
		base.Body.PlayMovie("beatA", 2400, 0);
		base.Body.CallFuction(HieuUngA, 3000);
	}

	private void TankB()
	{
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.CallFuction(RangeAttacking, 3000);
		base.Body.CallFuction(GoOut, 4000);
	}

	private void TankC()
	{
		base.Body.PlayMovie("beatB", 2400, 0);
		base.Body.CallFuction(KillAttack, 3000);
		base.Body.CallFuction(GoOut, 4000);
	}

	private void KillAttack()
	{
		((PVEGame)base.Game).AddAction(new FocusAction(m_denyPlayer[0], 0, 2000, 1000));
		base.Body.CallFuction(AddBloodA, 3000);
	}

	private void HieuUngA()
	{
		Player player = base.Game.FindRandomPlayer();
		moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "dangerMoive", "asset.game.eleven.099", "out", 1, 1);
	}

	private void GoOut()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	private void BodyDie()
	{
		((PVEGame)base.Game).ClearAllNpc();
		base.Body.Die();
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Body.Game.Map.Info.ForegroundWidth + 1, "cry", 1000, null);
	}
}

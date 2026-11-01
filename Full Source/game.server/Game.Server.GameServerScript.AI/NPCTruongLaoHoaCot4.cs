using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCTruongLaoHoaCot4 : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj moive;

	private int npcID = 71144;

	private int isSay = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 100f;
		base.Body.CurrentShootMinus = 1f;
		isSay = 0;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving)
			{
				if (allFightPlayer.X > base.Body.X && allFightPlayer.X - base.Body.X < 150)
				{
					flag = true;
				}
				if (allFightPlayer.X < base.Body.X && base.Body.X - allFightPlayer.X < 150)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			KillAttack();
		}
		else if (m_attackTurn == 0)
		{
			TankB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			TankC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			TankA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			TankD();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 50000f;
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(player.X - 150, player.X + 150, "cry", 4000, null);
	}

	private void TankA()
	{
		base.Body.PlayMovie("beatA", 1700, 0);
		base.Body.CallFuction(Call, 3000);
	}

	private void TankB()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("beatB", 1700, 0);
		base.Body.RangeAttacking(player.X - 100, player.X + 100, "cry", 4000, null);
		base.Body.CallFuction(SkillBong, 3000);
	}

	private void TankC()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("beatC", 1700, 0);
		base.Body.RangeAttacking(player.X - 100, player.X + 100, "cry", 4000, null);
	}

	private void TankD()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.ShootPoint(player.X, player.Y, 53, 1, 10, 1, 2f, 2550);
		base.Body.PlayMovie("beatA", 1700, 0);
		base.Body.CallFuction(CallNpcKill, 5000);
	}

	public void CallNpcKill()
	{
	}

	private void Call()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player.X < 85)
		{
			((PVEGame)base.Game).CreateNpc(npcID, 70, 496, 1, -1);
			((PVEGame)base.Game).CreateNpc(npcID, player.X + 80, 496, 1, -1);
		}
		else if (player.X > 1160)
		{
			((PVEGame)base.Game).CreateNpc(npcID, 1177, 496, 1, -1);
			((PVEGame)base.Game).CreateNpc(npcID, player.X - 80, 496, 1, -1);
		}
		else
		{
			((PVEGame)base.Game).CreateNpc(npcID, player.X - 80, 496, 1, -1);
			((PVEGame)base.Game).CreateNpc(npcID, player.X + 80, 496, 1, -1);
		}
	}

	private void SkillBong()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		int blood = base.Game.Random.Next(5000, 10000);
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ContinueReduceBloodEffect(2, blood, item), 0);
		}
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
	}

	private void GoOut()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	public override void OnShootedSay()
	{
		if (isSay == 0 && base.Body.IsLiving)
		{
			isSay = 1;
		}
	}
}

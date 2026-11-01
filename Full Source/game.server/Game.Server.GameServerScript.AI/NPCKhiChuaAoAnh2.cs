using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCKhiChuaAoAnh2 : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj moive;

	private int npcID = 71147;

	private int isSay = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentShootMinus = 5f;
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
		PhysicalObj[] array = base.Game.FindPhysicalObjByName("dangerMoive");
		if (array != null)
		{
			PhysicalObj[] array2 = array;
			foreach (PhysicalObj phy in array2)
			{
				base.Game.RemovePhysicalObj(phy, sendToClient: true);
			}
		}
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X == base.Body.X)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack();
		}
		else if (m_attackTurn == 0)
		{
			TankA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			TankB();
			m_attackTurn++;
		}
		else
		{
			TankC();
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
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(player.X - 150, player.X + 150, "cry", 4000, null);
	}

	private void TankA()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatA", 1700, 0);
		base.Body.RangeAttacking(player.X - 100, player.X + 100, "cry", 4000, null);
	}

	private void TankB()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 5f;
		base.Body.PlayMovie("beatB", 1700, 0);
		int num = base.Game.Random.Next(200, 300);
		base.Body.RangeAttacking(player.X - num, player.X + num, "cry", 4000, null);
		base.Body.CallFuction(PlayersMove, 3000);
	}

	private void TankC()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("beatC", 1700, 0);
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(500000);
		base.Body.SyncAtTime = false;
		base.Body.MoveTo(player.X, player.Y, "walk", 1000, 10);
	}

	private void PlayersMove()
	{
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			allFightPlayer.JumpTo(allFightPlayer.X - 100, allFightPlayer.Y, "born", 0, 12);
			base.Body.CallFuction(Roi, 1000);
		}
	}

	private void Roi()
	{
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			allFightPlayer.MoveTo(allFightPlayer.X - 400, allFightPlayer.Y, "run", 2000, 12);
			allFightPlayer.AddEffect(new ContinueReduceBloodEffect(2, 15000, allFightPlayer), 0);
		}
		base.Body.JumpTo(base.Body.X + 100, 300, "walk", 1000, 12, Call);
	}

	private void Call()
	{
		((SimpleBoss)base.Body).CreateChild(70014, 540, 900, 1, 1, -1);
		base.Body.Say("祝我一臂之力", 1, 1000);
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

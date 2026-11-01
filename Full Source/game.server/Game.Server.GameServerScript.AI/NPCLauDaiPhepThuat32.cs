using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NPCLauDaiPhepThuat32 : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj moive;

	private int npcID = 71075;

	private int npcID2 = 71076;

	private int npcID3 = 71077;

	private int isSay = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1150)
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
		else
		{
			TankB();
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
		base.Body.CurrentDamagePlus = 5000f;
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(player.X - 300, player.X + 300, "cry", 4000, null);
		base.Body.CallFuction(HieuUngA, 3000);
	}

	private void TankA()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatA", 1700, 0);
		int num = base.Game.Random.Next(200, 300);
		base.Body.RangeAttacking(player.X - num, player.X + num, "cry", 4000, null);
		base.Body.CallFuction(HieuUngA, 3000);
	}

	private void TankB()
	{
		base.Body.PlayMovie("beatB", 1700, 0);
		base.Body.CallFuction(Call, 4000);
	}

	private void Call()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.IsFly = true;
		((PVEGame)base.Game).CreateNpc(npcID, 1000, 636, 1, -1, livingConfig);
		((PVEGame)base.Game).CreateNpc(npcID2, 830, 580, 1, -1, livingConfig);
		((PVEGame)base.Game).CreateNpc(npcID3, 744, 537, 1, -1, livingConfig);
	}

	private void HieuUngA()
	{
		Player player = base.Game.FindRandomPlayer();
		moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.fifteen.424a", "out", 1, 1);
		base.Body.CallFuction(GoOut, 4000);
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
	}

	public override void OnDiedSay()
	{
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

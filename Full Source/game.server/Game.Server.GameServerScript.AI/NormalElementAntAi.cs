using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class NormalElementAntAi : ABrain
{
	private int m_attackTurn = 0;

	private int npcID = 33006;

	private int npcID1 = 33006;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1566 && allFightPlayer.X < 1666)
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
			KillAttack(1566, 1666);
		}
		else if (base.Body.Blood <= base.Body.MaxBlood * 20 / 100)
		{
			Player player = base.Game.FindRandomPlayer();
			if (player != null)
			{
				base.Body.CurrentDamagePlus = 1000f;
				int num3 = base.Game.Random.Next(670, 880);
				int num4 = base.Game.Random.Next(player.X - 10, player.X + 10);
				if (base.Body.ShootPoint(player.X, player.Y, 51, 1000, 10000, 1, 3f, 2550))
				{
					base.Body.PlayMovie("beatA", 2000, 0);
				}
			}
		}
		else if (m_attackTurn == 0)
		{
			if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
			{
				PersonalAttack();
			}
			else
			{
				Summon();
			}
			m_attackTurn++;
		}
		else
		{
			PersonalAttack();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10000f;
		base.Body.PlayMovie("beatC", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, directDamage: false);
	}

	private void PersonalAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 1.5f;
		base.Body.PlayMovie("beatB", 1500, 0);
		base.Body.RangeAttacking(player.X - 10000, player.X + 10000, "cry", 5000, directDamage: false);
	}

	private void Summon()
	{
		base.Body.PlayMovie("call", 1700, 2000);
		base.Body.CallFuction(Call, 4000);
	}

	private void Call()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 86, 623, -1, 89, -1);
		((SimpleBoss)base.Body).CreateChild(npcID1, 1216, 626, -1, 89, -1);
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
	}
}

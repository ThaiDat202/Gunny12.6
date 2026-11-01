using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class TwelveTerrorCaptainBoss : ABrain
{
	private int turnToFirstAttack = 0;

	private Player player_0;

	private int int_1;

	private void method_1()
	{
		player_0 = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		base.Body.ChangeDirection(player_0, 500);
		if (player_0 != null && player_0.IsLiving)
		{
			int_1 = (int)player_0.Distance(base.Body.X, base.Body.Y);
			if (int_1 > base.Body.MaxBeatDis)
			{
				base.Body.CallFuction(method_2, 1000);
			}
			else
			{
				base.Body.CallFuction(method_3, 1000);
			}
		}
	}

	private void method_2()
	{
		int num = base.Game.Random.Next(((SimpleBoss)base.Body).NpcInfo.MoveMax, ((SimpleBoss)base.Body).NpcInfo.MoveMax);
		if (int_1 < num)
		{
			num = int_1;
		}
		int x = ((base.Body.Direction == -1) ? (base.Body.X - num) : (base.Body.X + num));
		base.Body.MoveTo(x, base.Body.Y, "walk", 1200, "", ((SimpleBoss)base.Body).NpcInfo.speed, method_3);
	}

	private void method_3()
	{
		base.Body.Beat(player_0, "beatA", 100, 0, 500, 1, 1);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.MaxBeatDis = 190;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		if (turnToFirstAttack == 1)
		{
			method_1();
		}
		else if (turnToFirstAttack < 1)
		{
			turnToFirstAttack++;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}

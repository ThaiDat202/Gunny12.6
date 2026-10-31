using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardBlowNpc : ABrain
{
	private int attackingTurn = 1;

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
		base.OnStartAttacking();
		if (attackingTurn == 1)
		{
			NoDame();
		}
		else if (attackingTurn == 2)
		{
			Beat();
		}
		else
		{
			NoDame();
			attackingTurn = 1;
		}
		attackingTurn++;
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void NoDame()
	{
		base.Body.PlayMovie("stand", 100, 0);
	}

	public void Beat()
	{
		base.Body.PlayMovie("beatA", 500, 0);
		base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1500, null);
		base.Body.Die(1000);
	}
}

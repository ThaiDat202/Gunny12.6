using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveEpicFirstNpc : ABrain
{
	private int m_attackTurn = 0;

	protected Player m_targer;

	private int m_bossId = 0;

	private int m_npcId = 0;

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
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
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

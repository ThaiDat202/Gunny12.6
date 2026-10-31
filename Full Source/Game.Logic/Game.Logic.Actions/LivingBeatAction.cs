using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingBeatAction : BaseAction
{
	private Living m_living;

	private Living m_target;

	private int m_demageAmount;

	private int m_criticalAmount;

	private string m_action;

	private int m_livingCount;

	private int m_attackEffect;

	private string m_deadEffect;

	private int m_type;

	public LivingBeatAction(Living living, Living target, int demageAmount, int criticalAmount, string action, int delay, int livingCount, int attackEffect, string deadEffect, int type)
		: base(delay)
	{
		m_living = living;
		m_target = target;
		m_demageAmount = demageAmount;
		m_criticalAmount = criticalAmount;
		m_action = action;
		m_livingCount = livingCount;
		m_attackEffect = attackEffect;
		m_deadEffect = deadEffect;
		m_type = type;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		m_target.SyncAtTime = false;
		try
		{
			m_target.SyncAtTime = true;
			if (m_target is Player)
			{
				m_target.OnTakedDamage(m_target, ref m_demageAmount, ref m_criticalAmount);
			}
			if (m_target.TakeDamage(m_living, ref m_demageAmount, ref m_criticalAmount, "LivingFire"))
			{
				int totalDemageAmount = m_demageAmount + m_criticalAmount;
				game.SendLivingBeat(m_living, m_target, totalDemageAmount, m_action, m_livingCount, m_attackEffect, m_deadEffect, m_type);
			}
			m_target.IsFrost = false;
			Finish(tick);
		}
		finally
		{
		}
	}
}

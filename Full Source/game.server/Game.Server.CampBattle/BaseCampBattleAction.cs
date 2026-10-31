using Game.Logic;

namespace Game.Server.CampBattle;

public class BaseCampBattleAction : ICampBattleAction
{
	private long m_tick;

	private long m_finishDelay;

	private long m_finishTick;

	public BaseCampBattleAction(int delay)
		: this(delay, 0)
	{
	}

	public BaseCampBattleAction(int delay, int finishDelay)
	{
		m_tick = TickHelper.GetTickCount() + delay;
		m_finishDelay = finishDelay;
		m_finishTick = long.MaxValue;
	}

	public void Execute(CampBattleControl battle, long tick)
	{
		if (m_tick <= tick && m_finishTick == long.MaxValue)
		{
			ExecuteImp(battle, tick);
		}
	}

	protected virtual void ExecuteImp(CampBattleControl battle, long tick)
	{
		Finish(tick);
	}

	public void Finish(long tick)
	{
		m_finishTick = tick + m_finishDelay;
	}

	public bool IsFinished(long tick)
	{
		return m_finishTick <= tick;
	}
}

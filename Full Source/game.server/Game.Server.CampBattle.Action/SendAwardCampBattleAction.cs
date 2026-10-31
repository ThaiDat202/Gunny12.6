using System.Reflection;
using Game.Logic;
using log4net;

namespace Game.Server.CampBattle.Action;

public class SendAwardCampBattleAction : ICampBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	public SendAwardCampBattleAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(CampBattleControl game, long tick)
	{
		if (m_tick <= tick)
		{
			m_isFinished = true;
		}
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

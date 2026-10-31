using System.Linq;
using System.Reflection;
using Game.Logic;
using log4net;

namespace Game.Server.GuildBattle.Action;

public class ReloadRankAction : IGuildBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	public ReloadRankAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(GuildBattleMgr game, long tick)
	{
		int num = 1;
		GuildBattleConsortiaInfo[] array = (from a in game.GetAllConsortia()
			orderby a.Score descending
			select a).ToArray();
		GuildBattleConsortiaInfo[] array2 = array;
		foreach (GuildBattleConsortiaInfo guildBattleConsortiaInfo in array2)
		{
			guildBattleConsortiaInfo.Rank = num;
			num++;
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

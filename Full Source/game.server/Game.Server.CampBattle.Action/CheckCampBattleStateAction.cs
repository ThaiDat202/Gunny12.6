using System;
using System.Reflection;
using Game.Logic;
using log4net;

namespace Game.Server.CampBattle.Action;

public class CheckCampBattleStateAction : ICampBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	public CheckCampBattleStateAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(CampBattleControl game, long tick)
	{
		if (m_tick > tick)
		{
			return;
		}
		DateTime now = DateTime.Now;
		if (!game.IsOpenCampBattle && now.Hour >= 14 && now.Hour < 15)
		{
			game.IsOpenCampBattle = true;
			game.StartTime = now;
			game.EndTime = now.AddHours(1.0);
			game.ClearAll();
			game.SendAllOpenClose();
			Console.WriteLine("Camp Battle is opened!");
		}
		else if (game.IsOpenCampBattle)
		{
			if (now >= game.EndTime)
			{
				game.IsOpenCampBattle = false;
				game.AddAction(new CloseCampBattleAction(0));
				game.AddAction(new SendAwardCampBattleAction(10000));
				Console.WriteLine("Camp Battle is closed!");
				return;
			}
			game.AddAction(new CheckMonsterAction(0));
			game.UpdatePointAllTeam();
		}
		game.WaitTime(1000);
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

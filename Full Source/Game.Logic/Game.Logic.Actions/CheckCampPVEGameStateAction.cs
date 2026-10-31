using System;
using System.Reflection;
using log4net;

namespace Game.Logic.Actions;

public class CheckCampPVEGameStateAction : IAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_time;

	private bool m_isFinished;

	public CheckCampPVEGameStateAction(int delay)
	{
		m_time = TickHelper.GetTickCount() + delay;
		m_isFinished = false;
	}

	public void Execute(BaseGame game, long tick)
	{
		if (m_time > tick || game.GetWaitTimer() >= tick)
		{
			return;
		}
		if (game is CampBattlePVEGame campBattlePVEGame)
		{
			switch (campBattlePVEGame.GameState)
			{
			case eGameState.Inited:
				campBattlePVEGame.Prepare();
				break;
			case eGameState.Prepared:
				if (campBattlePVEGame.PlayerCount > 0)
				{
					campBattlePVEGame.InitCreateGame();
				}
				else if (campBattlePVEGame.beginTime.AddMinutes(1.0) <= DateTime.Now)
				{
					campBattlePVEGame.StopGame();
				}
				else
				{
					game.WaitTime(1000);
				}
				break;
			case eGameState.SessionPrepared:
				campBattlePVEGame.StartLoading();
				break;
			case eGameState.Loading:
				if (campBattlePVEGame.IsAllComplete())
				{
					campBattlePVEGame.StartGame();
					campBattlePVEGame.PreSessionId++;
				}
				else
				{
					game.WaitTime(1000);
				}
				break;
			case eGameState.GameStartMovie:
				if (game.CurrentActionCount <= 1)
				{
					campBattlePVEGame.StartGame();
				}
				else
				{
					campBattlePVEGame.StartGameMovie();
				}
				break;
			case eGameState.GameStart:
				campBattlePVEGame.PrepareNewGame();
				break;
			case eGameState.Playing:
				if ((campBattlePVEGame.CurrentLiving == null || !campBattlePVEGame.CurrentLiving.IsAttacking) && game.CurrentActionCount <= 1)
				{
					if (campBattlePVEGame.CanGameOver())
					{
						campBattlePVEGame.GameOver();
					}
					else
					{
						campBattlePVEGame.NextTurn();
					}
				}
				else if (campBattlePVEGame.CanGameOver())
				{
					campBattlePVEGame.GameOver();
				}
				break;
			case eGameState.GameOver:
				campBattlePVEGame.GameOverAllSession();
				break;
			case eGameState.ALLSessionStopped:
				campBattlePVEGame.StopGame();
				break;
			}
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

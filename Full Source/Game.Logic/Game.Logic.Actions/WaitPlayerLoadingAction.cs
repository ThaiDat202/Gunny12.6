using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class WaitPlayerLoadingAction : IAction
{
	private long m_time;

	private bool m_isFinished;

	public WaitPlayerLoadingAction(BaseGame game, int maxTime)
	{
		m_time = TickHelper.GetTickCount() + maxTime;
		game.GameStarted += game_GameStarted;
	}

	private void game_GameStarted(AbstractGame game)
	{
		game.GameStarted -= game_GameStarted;
		m_isFinished = true;
	}

	public void Execute(BaseGame game, long tick)
	{
		if (!m_isFinished && tick > m_time && game.GameState == eGameState.Loading)
		{
			if (game.GameState == eGameState.Loading)
			{
				List<Player> allFightPlayers = game.GetAllFightPlayers();
				foreach (Player item in allFightPlayers)
				{
					if (item.LoadingProcess < 100)
					{
						game.SendPlayerRemove(item);
						game.RemovePlayer(item.PlayerDetail, IsKick: false);
					}
				}
				game.CheckState(0);
			}
			m_isFinished = true;
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(16);
		List<Player> allFightPlayers2 = game.GetAllFightPlayers();
		foreach (Player item2 in allFightPlayers2)
		{
			if (item2.PlayerDetail.PlayerCharacter.IsAutoBot && item2.LoadingProcess < 100)
			{
				item2.LoadingProcess = 1000;
				game.CheckState(0);
				gSPacketIn.WriteInt(item2.LoadingProcess);
				gSPacketIn.WriteInt(item2.PlayerDetail.ZoneId);
				gSPacketIn.WriteInt(item2.PlayerDetail.PlayerCharacter.ID);
				game.SendToAll(gSPacketIn);
			}
		}
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

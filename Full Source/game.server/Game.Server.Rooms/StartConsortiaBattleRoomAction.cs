using System.Collections.Generic;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms;

public class StartConsortiaBattleRoomAction : IAction
{
	private BaseRoom m_redRoom;

	private BaseRoom m_blueRoom;

	public StartConsortiaBattleRoomAction(BaseRoom red, BaseRoom blue)
	{
		m_redRoom = red;
		m_blueRoom = blue;
	}

	public void Execute()
	{
		List<GamePlayer> players = m_redRoom.GetPlayers();
		List<GamePlayer> players2 = m_blueRoom.GetPlayers();
		List<IGamePlayer> list = new List<IGamePlayer>();
		List<IGamePlayer> list2 = new List<IGamePlayer>();
		foreach (GamePlayer item in players)
		{
			if (item != null)
			{
				item.GuildBattleEnemyId = players2[0].PlayerId;
				list.Add(item);
			}
		}
		foreach (GamePlayer item2 in players2)
		{
			if (item2 != null)
			{
				item2.GuildBattleEnemyId = players[0].PlayerId;
				list2.Add(item2);
			}
		}
		BaseGame baseGame = GameMgr.StartChallengePVPGame(list, list2, m_redRoom, m_blueRoom, 0, eRoomType.ConsortiaBattle, eGameType.ConsortiaBattle, 5);
		if (baseGame != null)
		{
			m_blueRoom.StartGame(baseGame);
			m_redRoom.StartGame(baseGame);
			return;
		}
		m_blueRoom.IsPlaying = false;
		m_blueRoom.SendPlayerState();
		m_redRoom.IsPlaying = false;
		m_redRoom.SendPlayerState();
	}
}

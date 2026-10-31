using Game.Server.GameObjects;

namespace Game.Server.Rooms;

public class CreateHorseRaceRoomAction : IAction
{
	private GamePlayer m_player;

	public CreateHorseRaceRoomAction(GamePlayer player)
	{
		m_player = player;
	}

	public void Execute()
	{
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		if (m_player.CurrentHorseRaceRoom != null)
		{
			m_player.CurrentHorseRaceRoom.RemovePlayerUnsafe(m_player);
		}
		if (m_player.PlayerCharacter.horseRaceCanRaceTime <= 0)
		{
			return;
		}
		BaseHorseRaceRoom[] horseRaceRoom = RoomMgr.HorseRaceRoom;
		BaseHorseRaceRoom baseHorseRaceRoom = FindRandomRoom(horseRaceRoom);
		if (baseHorseRaceRoom == null)
		{
			for (int i = 0; i < horseRaceRoom.Length; i++)
			{
				if (!horseRaceRoom[i].IsPlaying)
				{
					baseHorseRaceRoom = horseRaceRoom[i];
					break;
				}
			}
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			baseHorseRaceRoom.AddPlayerUnsafe(m_player);
			baseHorseRaceRoom.IsWaiting = true;
		}
		else
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			baseHorseRaceRoom.AddPlayerUnsafe(m_player);
		}
		if (baseHorseRaceRoom.CanStartGame())
		{
			baseHorseRaceRoom.StartGameTimer();
		}
	}

	private BaseHorseRaceRoom FindRandomRoom(BaseHorseRaceRoom[] rooms)
	{
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i] != null && rooms[i].IsWaiting && !rooms[i].IsPlaying && rooms[i].CanAddPlayer())
			{
				return rooms[i];
			}
		}
		return null;
	}
}

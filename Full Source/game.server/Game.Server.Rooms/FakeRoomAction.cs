namespace Game.Server.Rooms;

public class FakeRoomAction : IAction
{
	private string roomName;

	private int playerCount;

	private int maxPlayerCount;

	private int roomType;

	public FakeRoomAction(string roomName, int playerCount, int maxPlayerCount, int roomType)
	{
		this.roomName = roomName;
		this.playerCount = playerCount;
		this.maxPlayerCount = maxPlayerCount;
		this.roomType = roomType;
	}

	public void Execute()
	{
		BaseRoom[] rooms = RoomMgr.Rooms;
		FakeRoom fakeRoom = null;
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i].IsUsing)
			{
				fakeRoom = (FakeRoom)(rooms[i] = new FakeRoom(rooms[i].RoomId, roomName, playerCount, maxPlayerCount, roomType));
				break;
			}
		}
		if (fakeRoom != null)
		{
			fakeRoom.Start();
			RoomMgr.WaitingRoom.SendUpdateRoom(fakeRoom);
		}
	}
}

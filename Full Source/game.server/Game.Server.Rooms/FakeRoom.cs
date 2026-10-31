using System;
using Game.Logic;
using SqlDataProvider.Data;

namespace Game.Server.Rooms;

public class FakeRoom : BaseRoom
{
	private static Random random = new Random(Environment.TickCount);

	private bool isPlaying = false;

	private PveInfo pveinfo;

	public override bool IsPlaying
	{
		get
		{
			return isPlaying;
		}
		set
		{
		}
	}

	public FakeRoom(int roomId, string roomName, int playerCount, int maxPlayerCount, int roomType)
		: base(roomId)
	{
		m_playerCount = playerCount;
		m_placesCount = maxPlayerCount;
		Name = roomName;
		switch (roomType)
		{
		case 1:
			RoomType = eRoomType.Match;
			Password = Guid.NewGuid().ToString();
			break;
		case 2:
			RoomType = eRoomType.Match;
			isPlaying = true;
			break;
		case 3:
			RoomType = eRoomType.Dungeon;
			Password = Guid.NewGuid().ToString();
			pveinfo = PveInfoMgr.GetRandomPve();
			MapId = pveinfo.ID;
			HardLevel = GetRandomHardLevel();
			break;
		case 4:
			RoomType = eRoomType.Dungeon;
			pveinfo = PveInfoMgr.GetRandomPve();
			MapId = pveinfo.ID;
			HardLevel = GetRandomHardLevel();
			isPlaying = true;
			break;
		}
	}

	private eHardLevel GetRandomHardLevel()
	{
		int num;
		do
		{
			num = random.Next(0, 3);
			if (num != 0 || string.IsNullOrEmpty(pveinfo.NormalGameScript))
			{
				if (num != 1 || string.IsNullOrEmpty(pveinfo.NormalGameScript))
				{
					if (num == 2 && !string.IsNullOrEmpty(pveinfo.NormalGameScript))
					{
						return eHardLevel.Hard;
					}
					continue;
				}
				return eHardLevel.Normal;
			}
			return eHardLevel.Easy;
		}
		while (num != 3 || string.IsNullOrEmpty(pveinfo.NormalGameScript));
		return eHardLevel.Terror;
	}

	protected override void Reset()
	{
	}

	public override void Stop()
	{
	}
}

using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms;

public class CreateCryptBossAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	private int m_section;

	private int m_mapId;

	private eHardLevel m_hardLevel;

	public CreateCryptBossAction(GamePlayer player, int id, int level)
	{
		m_player = player;
		m_name = "CryptBoss";
		m_password = "12dasS12dasda44C@tchBe@st";
		m_roomType = eRoomType.Cryptboss;
		m_section = 0;
		m_timeType = 3;
		switch (id)
		{
		case 1:
			m_mapId = 50201;
			break;
		case 2:
			m_mapId = 50202;
			break;
		case 3:
			m_mapId = 50203;
			break;
		case 4:
			m_mapId = 50204;
			break;
		case 5:
			m_mapId = 50205;
			break;
		case 6:
			m_mapId = 50206;
			break;
		default:
			m_mapId = 50201;
			break;
		}
		switch (level)
		{
		case 1:
			m_hardLevel = eHardLevel.Easy;
			break;
		case 2:
			m_hardLevel = eHardLevel.Normal;
			break;
		case 3:
			m_hardLevel = eHardLevel.Hard;
			break;
		case 4:
			m_hardLevel = eHardLevel.Terror;
			m_timeType = 2;
			break;
		case 5:
			m_hardLevel = eHardLevel.Epic;
			m_timeType = 1;
			break;
		default:
			m_hardLevel = eHardLevel.Easy;
			break;
		}
	}

	public void Execute()
	{
		if (m_player.MainWeapon == null)
		{
			m_player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
			return;
		}
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		if (!m_player.IsActive)
		{
			return;
		}
		BaseRoom[] rooms = RoomMgr.Rooms;
		BaseRoom baseRoom = null;
		for (int i = 0; i < rooms.Length; i++)
		{
			if (!rooms[i].IsUsing)
			{
				baseRoom = rooms[i];
				break;
			}
		}
		if (baseRoom == null)
		{
			return;
		}
		RoomMgr.WaitingRoom.RemovePlayer(m_player);
		baseRoom.Start();
		baseRoom.HardLevel = m_hardLevel;
		baseRoom.LevelLimits = (int)baseRoom.GetLevelLimit(m_player);
		baseRoom.isCrosszone = false;
		baseRoom.isOpenBoss = false;
		baseRoom.MapId = m_mapId;
		baseRoom.currentFloor = m_section;
		baseRoom.TimeMode = m_timeType;
		baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, baseRoom.MapId);
		GSPacketIn gSPacketIn = m_player.Out.SendRoomCreate(baseRoom);
		if (!baseRoom.AddPlayerUnsafe(m_player))
		{
			return;
		}
		List<GamePlayer> players = baseRoom.GetPlayers();
		List<IGamePlayer> list = new List<IGamePlayer>();
		foreach (GamePlayer item in players)
		{
			if (item != null)
			{
				list.Add(item);
			}
		}
		BaseGame baseGame = GameMgr.StartPVEGame(baseRoom.RoomId, list, baseRoom.MapId, baseRoom.RoomType, baseRoom.GameType, baseRoom.TimeMode, baseRoom.HardLevel, baseRoom.LevelLimits, baseRoom.currentFloor);
		if (baseGame != null)
		{
			baseRoom.IsPlaying = true;
			baseRoom.StartGame(baseGame);
		}
		else
		{
			baseRoom.IsPlaying = false;
			baseRoom.SendPlayerState();
		}
	}
}

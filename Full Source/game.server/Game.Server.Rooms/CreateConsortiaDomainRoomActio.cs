using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms;

public class CreateConsortiaDomainRoomAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	public CreateConsortiaDomainRoomAction(GamePlayer player)
	{
		m_player = player;
		m_name = "Consortia Domain";
		m_password = "12dasSda44";
		m_roomType = eRoomType.ConsortiaDomain;
		m_timeType = 2;
	}

	public void Execute()
	{
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
		if (baseRoom != null && GameMgr.ConsortiaDomain.GetPlayer(m_player.PlayerCharacter.ID) == null && GameMgr.ConsortiaDomain.GetConsortiaDomain(m_player.PlayerCharacter.ConsortiaID) != null)
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			baseRoom.Start();
			baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
			baseRoom.ZoneId = m_player.ZoneId;
			baseRoom.isCrosszone = false;
			GSPacketIn gSPacketIn = m_player.Out.SendSingleRoomCreate(baseRoom);
			baseRoom.AddPlayerUnsafe(m_player);
		}
		else
		{
			m_player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaDomain.EnterError"));
		}
	}
}

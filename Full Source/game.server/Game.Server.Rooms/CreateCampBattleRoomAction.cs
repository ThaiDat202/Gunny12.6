using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms;

public class CreateCampBattleRoomAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	public CreateCampBattleRoomAction(GamePlayer player)
	{
		m_player = player;
		m_name = "Camp Battle";
		m_password = "54nfr4t";
		m_roomType = eRoomType.CampBattleBattle;
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
		if (baseRoom != null && GameMgr.CampBattle.IsOpenCampBattle)
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
			m_player.SendMessage(LanguageMgr.GetTranslation("GameServer.CampBattle.EnterGame.Error"));
		}
	}
}

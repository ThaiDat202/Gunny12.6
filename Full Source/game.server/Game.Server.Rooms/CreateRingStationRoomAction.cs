using System;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.RingStation;
using SqlDataProvider.Data;

namespace Game.Server.Rooms;

public class CreateRingStationRoomAction : IAction
{
	private GamePlayer m_player;

	private UserRingStationInfo m_challenge;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	private Random rand;

	public CreateRingStationRoomAction(GamePlayer player, UserRingStationInfo challengePlayer)
	{
		m_player = player;
		m_challenge = challengePlayer;
		m_name = "RingStation Battle";
		m_password = "12dasSda44";
		m_roomType = eRoomType.RingStation;
		m_timeType = 2;
		rand = new Random();
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
		int num = rand.Next(rooms.Length);
		for (int i = 0; i < rooms.Length; i++)
		{
			if (!rooms[num].IsUsing)
			{
				baseRoom = rooms[num];
				break;
			}
			num = rand.Next(rooms.Length);
		}
		if (baseRoom != null)
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			baseRoom.Start();
			baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
			m_player.Out.SendRoomCreate(baseRoom);
			if (baseRoom.AddPlayerUnsafe(m_player))
			{
				baseRoom.StartWithNpc = true;
				baseRoom.PickUpNpcId = RingStationMgr.CreateRingStationChallenge(m_challenge, (int)baseRoom.RoomType, (int)baseRoom.GameType);
			}
			BattleServer battleServer = BattleMgr.AddRoom(baseRoom);
			if (battleServer != null)
			{
				baseRoom.BattleServer = battleServer;
				baseRoom.IsPlaying = true;
				RingStationMgr.SetChallenge(baseRoom.PickUpNpcId, onFight: true);
			}
			else if (m_player.CurrentRoom != null)
			{
				m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
			}
		}
	}
}

using System.Collections.Generic;
using Game.Base.Packets;

namespace Game.Server.RingStation.Battle;

public class RingStationBattleServer
{
	private int m_serverId;

	private RingStationFightConnector m_server;

	private Dictionary<int, BaseRoomRingStation> m_rooms;

	private string m_ip;

	private int m_port;

	public RingStationFightConnector Server => m_server;

	public bool IsActive => m_server.IsConnected;

	public string Ip => m_ip;

	public int Port => m_port;

	public RingStationBattleServer(int serverId, string ip, int port, string loginKey)
	{
		m_serverId = serverId;
		m_ip = ip;
		m_port = port;
		m_server = new RingStationFightConnector(this, ip, port, loginKey);
		m_rooms = new Dictionary<int, BaseRoomRingStation>();
	}

	public bool Start()
	{
		return m_server.Connect();
	}

	public void Stop()
	{
		m_server.Disconnect();
	}

	public bool AddRoom(BaseRoomRingStation room)
	{
		bool flag = false;
		BaseRoomRingStation baseRoomRingStation = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(room.RoomId))
			{
				baseRoomRingStation = m_rooms[room.RoomId];
				m_rooms.Remove(room.RoomId);
			}
		}
		if (baseRoomRingStation != null && baseRoomRingStation.Game != null)
		{
			baseRoomRingStation.Game.Stop();
		}
		lock (m_rooms)
		{
			if (!m_rooms.ContainsKey(room.RoomId))
			{
				m_rooms.Add(room.RoomId, room);
				flag = true;
			}
		}
		if (flag)
		{
			m_server.SendAddRoom(room);
		}
		room.BattleServer = this;
		room.IsPlaying = true;
		return flag;
	}

	public bool RemoveRoom(BaseRoomRingStation room)
	{
		bool flag = false;
		lock (m_rooms)
		{
			flag = m_rooms.ContainsKey(room.RoomId);
			if (flag)
			{
				m_server.SendRemoveRoom(room);
			}
		}
		return flag;
	}

	public void RemoveRoomImp(int roomId)
	{
		BaseRoomRingStation baseRoomRingStation = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(roomId))
			{
				baseRoomRingStation = m_rooms[roomId];
				m_rooms.Remove(roomId);
			}
		}
	}

	public void StartGame(int roomId, ProxyRingStationGame game)
	{
		FindRoom(roomId)?.StartGame(game);
	}

	public void StopGame(int roomId, int gameId)
	{
		BaseRoomRingStation baseRoomRingStation = FindRoom(roomId);
		if (baseRoomRingStation != null)
		{
			lock (m_rooms)
			{
				m_rooms.Remove(roomId);
			}
		}
	}

	public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
	{
		BaseRoomRingStation baseRoomRingStation = FindRoom(roomId);
		if (baseRoomRingStation == null)
		{
			return;
		}
		if (exceptId != 0)
		{
			VirtualGamePlayer playerById = RingStationMgr.GetPlayerById(exceptId);
			if (playerById != null)
			{
				if (playerById.GamePlayerId == exceptGameId)
				{
					baseRoomRingStation.SendToAll(pkg, playerById);
				}
				else
				{
					baseRoomRingStation.SendToAll(pkg);
				}
			}
		}
		else
		{
			baseRoomRingStation.SendToAll(pkg);
		}
	}

	private BaseRoomRingStation FindRoom(int roomId)
	{
		BaseRoomRingStation result = null;
		lock (m_rooms)
		{
			try
			{
				if (m_rooms.ContainsKey(roomId))
				{
					result = m_rooms[roomId];
				}
			}
			finally
			{
			}
		}
		return result;
	}

	public void UpdatePlayerGameId(int playerid, int gamePlayerId)
	{
		VirtualGamePlayer playerById = RingStationMgr.GetPlayerById(playerid);
		if (playerById != null)
		{
			playerById.GamePlayerId = gamePlayerId;
		}
	}

	public override string ToString()
	{
		return $"ServerID:{m_serverId},Ip:{m_server.RemoteEP.Address},Port:{m_server.RemoteEP.Port},IsConnected:{m_server.IsConnected},RoomCount:";
	}
}

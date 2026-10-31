using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Rooms;

public class BaseWaitingRoom
{
	private Dictionary<int, GamePlayer> m_list;

	public BaseWaitingRoom()
	{
		m_list = new Dictionary<int, GamePlayer>();
	}

	public bool AddPlayer(GamePlayer player)
	{
		bool flag = false;
		lock (m_list)
		{
			if (!m_list.ContainsKey(player.PlayerId))
			{
				m_list.Add(player.PlayerId, player);
				flag = true;
			}
		}
		if (flag)
		{
			GSPacketIn packet = player.Out.SendSceneAddPlayer(player);
			SendToALL(packet, player);
		}
		return flag;
	}

	public bool RemovePlayer(GamePlayer player)
	{
		bool flag = false;
		lock (m_list)
		{
			flag = m_list.Remove(player.PlayerId);
		}
		if (flag)
		{
			GSPacketIn packet = player.Out.SendSceneRemovePlayer(player);
			SendToALL(packet, player);
		}
		return true;
	}

	public void SendSceneUpdate(GamePlayer player)
	{
		GSPacketIn packet = player.Out.SendSceneAddPlayer(player);
		SendToALL(packet, player);
		GamePlayer[] playersSafe = GetPlayersSafe();
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != player)
			{
				player.Out.SendSceneAddPlayer(gamePlayer);
			}
		}
	}

	public void SendUpdateRoom(BaseRoom room)
	{
		GamePlayer[] array = null;
		switch (room.RoomType)
		{
		case eRoomType.Match:
		case eRoomType.Freedom:
			array = GetPlayersSafe(ePlayerState.Online);
			break;
		case eRoomType.Dungeon:
			array = GetPlayersSafe(ePlayerState.Away);
			break;
		}
		if (array != null)
		{
			GamePlayer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Out.SendUpdateRoomList(new List<BaseRoom> { room });
			}
		}
	}

	public GamePlayer[] GetPlayersSafe(ePlayerState typeState)
	{
		List<GamePlayer> list = new List<GamePlayer>();
		lock (m_list)
		{
			foreach (GamePlayer value in m_list.Values)
			{
				if (value.PlayerState == typeState)
				{
					list.Add(value);
				}
			}
		}
		return list.ToArray();
	}

	public void SendUpdateRoom(GamePlayer player)
	{
		List<BaseRoom> list = new List<BaseRoom>();
		if (player.PlayerState == ePlayerState.Away)
		{
			list.AddRange(RoomMgr.GetAllPveRooms());
		}
		else
		{
			list.AddRange(RoomMgr.GetAllMatchRooms());
		}
		player.Out.SendUpdateRoomList(list);
	}

	public void SendUpdateCurrentRoom(BaseRoom room)
	{
		if (room == null)
		{
			return;
		}
		List<BaseRoom> allRooms = RoomMgr.GetAllRooms(room);
		GSPacketIn gSPacketIn = null;
		foreach (GamePlayer player in room.GetPlayers())
		{
			if (gSPacketIn == null)
			{
				gSPacketIn = player.Out.SendUpdateRoomList(allRooms);
			}
			else
			{
				player.Out.SendTCP(gSPacketIn);
			}
		}
	}

	public void SendToALL(GSPacketIn packet)
	{
		SendToALL(packet, null);
	}

	public void SendToALL(GSPacketIn packet, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_list)
		{
			array = new GamePlayer[m_list.Count];
			m_list.Values.CopyTo(array, 0);
		}
		GamePlayer[] array2 = array;
		foreach (GamePlayer gamePlayer in array2)
		{
			if (gamePlayer != null && gamePlayer != except)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		lock (m_list)
		{
			array = new GamePlayer[m_list.Count];
			m_list.Values.CopyTo(array, 0);
		}
		return array;
	}
}

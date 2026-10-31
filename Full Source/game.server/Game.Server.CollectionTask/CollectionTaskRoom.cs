using System.Collections.Generic;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;

namespace Game.Server.CollectionTask;

public class CollectionTaskRoom
{
	private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Dictionary<int, GamePlayer> _playerList;

	private object _syncStop = new object();

	public CollectionTaskRoom()
	{
		_playerList = new Dictionary<int, GamePlayer>();
	}

	public void SendPlayerInfos(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(261);
		gSPacketIn.WriteByte(5);
		GamePlayer[] playersSafe = GetPlayersSafe();
		int num = ((playersSafe.Length > 20) ? 20 : playersSafe.Length);
		gSPacketIn.WriteInt(playersSafe.Length);
		for (int i = 0; i < num; i++)
		{
			GamePlayer gamePlayer = playersSafe[i];
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.typeVIP);
			gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.MountsType);
			gSPacketIn.WriteInt(gamePlayer.CollectionTaskPosX);
			gSPacketIn.WriteInt(gamePlayer.CollectionTaskPosY);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ConsortiaID);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.badgeID);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.ConsortiaName);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Honor);
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendAddPlayer(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(261);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.ID);
		gSPacketIn.WriteString(player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
		gSPacketIn.WriteInt(player.PlayerCharacter.typeVIP);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(player.PlayerCharacter.Style);
		gSPacketIn.WriteString(player.PlayerCharacter.Colors);
		gSPacketIn.WriteInt(player.PlayerCharacter.MountsType);
		gSPacketIn.WriteInt(player.CollectionTaskPosX);
		gSPacketIn.WriteInt(player.CollectionTaskPosY);
		gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
		gSPacketIn.WriteInt(player.PlayerCharacter.badgeID);
		gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
		gSPacketIn.WriteString(player.PlayerCharacter.Honor);
		SendToALL(gSPacketIn, player);
	}

	public void PlayerMove(int ID, int X, int Y)
	{
		GamePlayer playerByID = GetPlayerByID(ID);
		playerByID.CollectionTaskPosX = X;
		playerByID.CollectionTaskPosY = Y;
		GSPacketIn gSPacketIn = new GSPacketIn(261);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(playerByID.PlayerId);
		gSPacketIn.WriteInt(playerByID.CollectionTaskPosX);
		gSPacketIn.WriteInt(playerByID.CollectionTaskPosY);
		SendToALL(gSPacketIn, playerByID);
	}

	public bool AddPlayerInfo(int playerId)
	{
		if (_playerList.ContainsKey(playerId))
		{
			SendAddPlayer(_playerList[playerId]);
		}
		return true;
	}

	public bool AddPlayer(GamePlayer player)
	{
		lock (_syncStop)
		{
			if (player.CurrentRoom != null)
			{
				player.CurrentRoom.RemovePlayerUnsafe(player);
			}
			if (!_playerList.ContainsKey(player.PlayerCharacter.ID))
			{
				_playerList.Add(player.PlayerCharacter.ID, player);
			}
			SendPlayerInfos(player);
		}
		return true;
	}

	public bool RemovePlayer(int Id)
	{
		lock (_syncStop)
		{
			if (_playerList.Remove(Id))
			{
				GSPacketIn gSPacketIn = new GSPacketIn(261);
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteInt(Id);
				SendToALL(gSPacketIn);
				return true;
			}
		}
		return false;
	}

	public GamePlayer GetPlayerByID(int ID)
	{
		lock (_syncStop)
		{
			if (_playerList.ContainsKey(ID))
			{
				return _playerList[ID];
			}
		}
		return null;
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		lock (_playerList)
		{
			array = new GamePlayer[_playerList.Count];
			_playerList.Values.CopyTo(array, 0);
		}
		return (array == null) ? new GamePlayer[0] : array;
	}

	public GamePlayer[] GetPlayersSafe(GamePlayer except)
	{
		List<GamePlayer> list = new List<GamePlayer>();
		GamePlayer[] playersSafe = GetPlayersSafe();
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != null && gamePlayer != except)
			{
				list.Add(gamePlayer);
			}
		}
		return list.ToArray();
	}

	public void SendToALL(GSPacketIn packet)
	{
		SendToALL(packet, null);
	}

	public void SendToALL(GSPacketIn packet, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (_playerList)
		{
			array = new GamePlayer[_playerList.Count];
			_playerList.Values.CopyTo(array, 0);
		}
		if (array == null)
		{
			return;
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
}

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Helpers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;

namespace Game.Server.NewHall;

public class NewHallRoom
{
	private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Dictionary<int, GamePlayer> _playerList;

	private object _syncStop = new object();

	public NewHallRoom()
	{
		_playerList = new Dictionary<int, GamePlayer>();
	}

	public void HidePlayerInfo(GamePlayer p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(262);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteBoolean(p.HideAllFriend);
		p.SendTCP(gSPacketIn);
		SendPlayerInfo(p);
	}

	public void PlayerOnline(GamePlayer p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(262);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteString(p.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(p.PlayerCharacter.honorId);
		SendToALL(gSPacketIn);
	}

	public void SendPlayerInfo(GamePlayer player)
	{
		GamePlayer[] playersSafe = GetPlayersSafe(player);
		if (playersSafe.Length > 20)
		{
			IEnumerable<IEnumerable<GamePlayer>> enumerable = playersSafe.Split(20);
			{
				foreach (IEnumerable<GamePlayer> item in enumerable)
				{
					GamePlayer[] array = item.ToArray();
					GSPacketIn gSPacketIn = new GSPacketIn(262);
					gSPacketIn.WriteByte(0);
					gSPacketIn.WriteInt(player.PosX);
					gSPacketIn.WriteInt(player.PosY);
					gSPacketIn.WriteInt(array.Length);
					GamePlayer[] array2 = array;
					foreach (GamePlayer gamePlayer in array2)
					{
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
						gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.typeVIP);
						gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
						gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
						gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.MountsType);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.PetsID);
						gSPacketIn.WriteInt(gamePlayer.PosX);
						gSPacketIn.WriteInt(gamePlayer.PosY);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ConsortiaID);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.badgeID);
						gSPacketIn.WriteString(gamePlayer.PlayerCharacter.ConsortiaName);
						gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Honor);
						gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.honorId);
						gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.isAttest);
						gSPacketIn.WriteString(gamePlayer.PlayerCharacter.ImagePath);
						gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.IsShow);
						if (GameProperties.VERSION >= 10400)
						{
							gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.magicWardrobeID);
						}
					}
					player.SendTCP(gSPacketIn);
				}
				return;
			}
		}
		GSPacketIn gSPacketIn2 = new GSPacketIn(262);
		gSPacketIn2.WriteByte(0);
		gSPacketIn2.WriteInt(player.PosX);
		gSPacketIn2.WriteInt(player.PosY);
		gSPacketIn2.WriteInt(playersSafe.Length);
		GamePlayer[] array3 = playersSafe;
		foreach (GamePlayer gamePlayer2 in array3)
		{
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.ID);
			gSPacketIn2.WriteString(gamePlayer2.PlayerCharacter.NickName);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.VIPLevel);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.typeVIP);
			gSPacketIn2.WriteBoolean(gamePlayer2.PlayerCharacter.Sex);
			gSPacketIn2.WriteString(gamePlayer2.PlayerCharacter.Style);
			gSPacketIn2.WriteString(gamePlayer2.PlayerCharacter.Colors);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.MountsType);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.PetsID);
			gSPacketIn2.WriteInt(gamePlayer2.PosX);
			gSPacketIn2.WriteInt(gamePlayer2.PosY);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.ConsortiaID);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.badgeID);
			gSPacketIn2.WriteString(gamePlayer2.PlayerCharacter.ConsortiaName);
			gSPacketIn2.WriteString(gamePlayer2.PlayerCharacter.Honor);
			gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.honorId);
			gSPacketIn2.WriteBoolean(gamePlayer2.PlayerCharacter.isAttest);
			gSPacketIn2.WriteString(gamePlayer2.PlayerCharacter.ImagePath);
			gSPacketIn2.WriteBoolean(gamePlayer2.PlayerCharacter.IsShow);
			if (GameProperties.VERSION >= 10400)
			{
				gSPacketIn2.WriteInt(gamePlayer2.PlayerCharacter.magicWardrobeID);
			}
		}
		player.SendTCP(gSPacketIn2);
	}

	public void SendOtherFriendPlayerInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(262);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.ID);
		gSPacketIn.WriteString(player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
		gSPacketIn.WriteInt(player.PlayerCharacter.typeVIP);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(player.PlayerCharacter.Style);
		gSPacketIn.WriteString(player.PlayerCharacter.Colors);
		gSPacketIn.WriteInt(player.PlayerCharacter.MountsType);
		gSPacketIn.WriteInt(player.PlayerCharacter.PetsID);
		gSPacketIn.WriteInt(player.PosX);
		gSPacketIn.WriteInt(player.PosY);
		gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
		gSPacketIn.WriteInt(player.PlayerCharacter.badgeID);
		gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
		gSPacketIn.WriteString(player.PlayerCharacter.Honor);
		gSPacketIn.WriteInt(player.PlayerCharacter.honorId);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.isAttest);
		gSPacketIn.WriteString(player.PlayerCharacter.ImagePath);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.IsShow);
		if (GameProperties.VERSION >= 10400)
		{
			gSPacketIn.WriteInt(player.PlayerCharacter.magicWardrobeID);
		}
		SendToALL(gSPacketIn, player);
	}

	public void SendAddPlayer(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(262);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(player.PlayerCharacter.ID);
		gSPacketIn.WriteString(player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
		gSPacketIn.WriteInt(player.PlayerCharacter.typeVIP);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(player.PlayerCharacter.Style);
		gSPacketIn.WriteString(player.PlayerCharacter.Colors);
		gSPacketIn.WriteInt(player.PlayerCharacter.MountsType);
		gSPacketIn.WriteInt(player.PlayerCharacter.PetsID);
		gSPacketIn.WriteInt(player.PosX);
		gSPacketIn.WriteInt(player.PosY);
		gSPacketIn.WriteInt(player.PlayerCharacter.ConsortiaID);
		gSPacketIn.WriteInt(player.PlayerCharacter.badgeID);
		gSPacketIn.WriteString(player.PlayerCharacter.ConsortiaName);
		gSPacketIn.WriteString(player.PlayerCharacter.Honor);
		gSPacketIn.WriteInt(player.PlayerCharacter.honorId);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.isAttest);
		gSPacketIn.WriteString(player.PlayerCharacter.ImagePath);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.IsShow);
		if (GameProperties.VERSION >= 10400)
		{
			gSPacketIn.WriteInt(player.PlayerCharacter.magicWardrobeID);
		}
		SendToALL(gSPacketIn, player);
	}

	public void PlayerMove(int ID, int X, int Y)
	{
		GamePlayer playerByID = GetPlayerByID(ID);
		playerByID.PosX = X;
		playerByID.PosY = Y;
		GSPacketIn gSPacketIn = new GSPacketIn(262);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(playerByID.PlayerId);
		gSPacketIn.WriteInt(playerByID.PosX);
		gSPacketIn.WriteInt(playerByID.PosY);
		SendToALL(gSPacketIn, playerByID);
	}

	public bool AddPlayerInfo(int playerId)
	{
		if (_playerList.ContainsKey(playerId))
		{
			GamePlayer player = _playerList[playerId];
			SendAddPlayer(player);
		}
		return true;
	}

	public bool PlayerInfo(GamePlayer player)
	{
		if (player.CurrentRoom != null)
		{
			player.CurrentRoom.RemovePlayerUnsafe(player);
		}
		lock (_syncStop)
		{
			if (!_playerList.ContainsKey(player.PlayerCharacter.ID))
			{
				_playerList.Add(player.PlayerCharacter.ID, player);
			}
		}
		HidePlayerInfo(player);
		PlayerOnline(player);
		if (!player.HideAllFriend)
		{
			SendOtherFriendPlayerInfo(player);
		}
		return true;
	}

	public bool UpdatePets(int playerId, int petsId)
	{
		GamePlayer gamePlayer = null;
		lock (_syncStop)
		{
			if (_playerList.ContainsKey(playerId))
			{
				_playerList[playerId].PlayerCharacter.PetsID = petsId;
				gamePlayer = _playerList[playerId];
			}
		}
		if (gamePlayer != null)
		{
			HidePlayerInfo(gamePlayer);
			if (!gamePlayer.HideAllFriend)
			{
				SendOtherFriendPlayerInfo(gamePlayer);
			}
		}
		return true;
	}

	public bool RemovePlayer(int Id)
	{
		lock (_syncStop)
		{
			if (_playerList.Remove(Id))
			{
				GSPacketIn gSPacketIn = new GSPacketIn(262);
				gSPacketIn.WriteByte(2);
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
		if (!except.HideAllFriend)
		{
			GamePlayer[] playersSafe = GetPlayersSafe();
			GamePlayer[] array = playersSafe;
			foreach (GamePlayer gamePlayer in array)
			{
				if (gamePlayer != null && gamePlayer != except)
				{
					list.Add(gamePlayer);
				}
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
			if (gamePlayer != null && gamePlayer != except && !gamePlayer.HideAllFriend)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.HotSpringRooms;

public class HotSpringRoom
{
	private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Dictionary<int, GamePlayer> m_players;

	private object _syncStop = new object();

	public int RoomId;

	public int MaxCount = 10;

	public string RoomName { get; set; }

	public int RoomNumber { get; set; }

	public string RoomPassword { get; set; }

	public int EffectiveTime { get; set; }

	public int CurrCount => m_players.Count;

	public DateTime StartTime { get; set; }

	public string RoomIntroduction { get; set; }

	public int RoomType { get; set; }

	public HotSpringRoom(int roomId)
	{
		RoomId = roomId;
		RoomNumber = roomId;
		RoomName = LanguageMgr.GetTranslation("HotSpringRoom.RoomName");
		RoomPassword = "";
		StartTime = DateTime.Now;
		RoomIntroduction = LanguageMgr.GetTranslation("HotSpringRoom.RoomIntroduction");
		RoomType = 1;
		EffectiveTime = 60;
		m_players = new Dictionary<int, GamePlayer>();
	}

	public bool HasFreeLot()
	{
		return CurrCount < MaxCount;
	}

	public void PlayerMove(int id, int X, int Y, string pathStr, int direction)
	{
		GamePlayer playerByID = GetPlayerByID(id);
		if (playerByID == null)
		{
			playerByID.SendMessage(LanguageMgr.GetTranslation("ClickFlowerEgg.PlayerNotExit"));
			return;
		}
		playerByID.X = X;
		playerByID.Y = Y;
		playerByID.Direction = direction;
		GSPacketIn gSPacketIn = new GSPacketIn(191);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteString(pathStr);
		gSPacketIn.WriteInt(id);
		gSPacketIn.WriteInt(playerByID.X);
		gSPacketIn.WriteInt(playerByID.Y);
		SendToALL(gSPacketIn);
	}

	public void RoomTimeUpdate()
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		string[] array = GameProperties.HotSpringExp.Split(',');
		int value = 1;
		GamePlayer[] array2 = playersSafe;
		foreach (GamePlayer gamePlayer in array2)
		{
			if (gamePlayer == null)
			{
				continue;
			}
			int num = ((gamePlayer.Level > array.Length) ? (array.Length - 1) : gamePlayer.Level);
			gamePlayer.EffectiveTime = CalEffectiveTime(gamePlayer.EnterRoomTime);
			if (gamePlayer.EffectiveTime > 0)
			{
				int num2 = int.Parse(array[(num - 1 >= 0) ? (num - 1) : 0]);
				if (GameProperties.VERSION < 7600)
				{
					gamePlayer.AddGP(num2, canDupeExp: false, canAddTop: false);
					if (gamePlayer.Level >= 50)
					{
						gamePlayer.SendHideMessage(LanguageMgr.GetTranslation("HotSpringRoom.GetExp", num2));
					}
				}
				if (GameProperties.VERSION >= 8100 && gamePlayer.AddEnergyTotal(value) != 0)
				{
					gamePlayer.AddMissionEnergy(value);
				}
				gamePlayer.SendTCP(SendUpdateTimeToClient(gamePlayer.EffectiveTime, num2, gamePlayer.Extra.Info.EnergyTotal));
			}
			else
			{
				gamePlayer.SendHideMessage(LanguageMgr.GetTranslation("HotSpringRoom.EffectiveTimeEnd"));
			}
		}
	}

	private GSPacketIn SendUpdateTimeToClient(int effectiveTime, int miniExp, int energy)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(191);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(effectiveTime);
		gSPacketIn.WriteInt(miniExp);
		if (GameProperties.VERSION >= 8100)
		{
			gSPacketIn.WriteInt(energy);
		}
		return gSPacketIn;
	}

	public int CalEffectiveTime(DateTime enterRoomTime)
	{
		int num = Math.Abs((int)(DateTime.Now - enterRoomTime).TotalMinutes);
		return (num >= 0) ? num : 0;
	}

	public bool AddPlayer(GamePlayer player)
	{
		if (CurrCount >= MaxCount)
		{
			return false;
		}
		lock (_syncStop)
		{
			if (player.CurrentRoom != null)
			{
				player.CurrentRoom.RemovePlayerUnsafe(player);
			}
			if (!m_players.ContainsKey(player.PlayerCharacter.ID))
			{
				if (CalEffectiveTime(player.EnterRoomTime) <= 0)
				{
					player.EnterRoomTime = DateTime.Now;
				}
				player.EnterRoomTime = DateTime.Now.AddMinutes(EffectiveTime);
				player.EffectiveTime = EffectiveTime;
				m_players.Add(player.PlayerCharacter.ID, player);
				player.CurrentHotSpringRoom = this;
				return true;
			}
		}
		return false;
	}

	public void UpdateAllPlayersInCurrRoom(GamePlayer player)
	{
		GamePlayer[] playersSafe = GetPlayersSafe(player);
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(198);
			PlayerInfo playerCharacter = gamePlayer.PlayerCharacter;
			gSPacketIn.WriteInt(playerCharacter.ID);
			gSPacketIn.WriteInt(playerCharacter.Grade);
			gSPacketIn.WriteInt(playerCharacter.Hide);
			gSPacketIn.WriteInt(playerCharacter.Repute);
			gSPacketIn.WriteString(playerCharacter.NickName);
			gSPacketIn.WriteByte(playerCharacter.typeVIP);
			gSPacketIn.WriteInt(playerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(playerCharacter.Sex);
			gSPacketIn.WriteString(playerCharacter.Style);
			gSPacketIn.WriteString(playerCharacter.Colors);
			gSPacketIn.WriteString(playerCharacter.Skin);
			gSPacketIn.WriteInt(player.X);
			gSPacketIn.WriteInt(player.Y);
			gSPacketIn.WriteInt(playerCharacter.FightPower);
			gSPacketIn.WriteInt(playerCharacter.Win);
			gSPacketIn.WriteInt(playerCharacter.Total);
			gSPacketIn.WriteInt(player.Direction);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(playerCharacter.eggNum);
			gSPacketIn.WriteInt(playerCharacter.flowerNum);
			if (playerCharacter.ID == player.PlayerId)
			{
				gSPacketIn.WriteBoolean(playerCharacter.isClicked);
			}
			player.SendTCP(gSPacketIn);
		}
	}

	public bool RemovePlayer(GamePlayer player)
	{
		lock (_syncStop)
		{
			if (m_players.Remove(player.PlayerId))
			{
				GSPacketIn gSPacketIn = new GSPacketIn(169);
				gSPacketIn.WriteString(LanguageMgr.GetTranslation("HotSpringRoomPlayerRemoveHandler.Msg2", player.PlayerCharacter.NickName));
				SendToALL(gSPacketIn, player);
				SendRemovePlayer(player.PlayerId);
				player.CurrentHotSpringRoom = null;
				return true;
			}
		}
		return false;
	}

	public void SendRemovePlayer(int id)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(199);
		gSPacketIn.WriteInt(id);
		SendToALL(gSPacketIn);
	}

	public void HotSpringFlower_Egg(int id, int type)
	{
		GamePlayer playerByID = GetPlayerByID(id);
		if (playerByID == null)
		{
			playerByID.SendMessage(LanguageMgr.GetTranslation("ClickFlowerEgg.PlayerNotExit"));
			return;
		}
		if (playerByID.PlayerCharacter.isClicked)
		{
			playerByID.SendMessage(LanguageMgr.GetTranslation("ClickFlowerEgg.isClicked"));
			return;
		}
		switch (type)
		{
		case 1:
			playerByID.PlayerCharacter.flowerNum++;
			break;
		case 2:
			playerByID.PlayerCharacter.eggNum++;
			break;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(191);
		gSPacketIn.WriteByte(13);
		gSPacketIn.WriteInt(id);
		gSPacketIn.WriteInt(playerByID.PlayerCharacter.eggNum);
		gSPacketIn.WriteInt(playerByID.PlayerCharacter.flowerNum);
		SendToALL(gSPacketIn);
		playerByID.PlayerCharacter.isClicked = true;
	}

	public GamePlayer GetPlayerByID(int ID)
	{
		lock (_syncStop)
		{
			if (m_players.ContainsKey(ID))
			{
				return m_players[ID];
			}
		}
		return null;
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		lock (m_players)
		{
			array = new GamePlayer[m_players.Count];
			m_players.Values.CopyTo(array, 0);
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
		lock (m_players)
		{
			array = new GamePlayer[m_players.Count];
			m_players.Values.CopyTo(array, 0);
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

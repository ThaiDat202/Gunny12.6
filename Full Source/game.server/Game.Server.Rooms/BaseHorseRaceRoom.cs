using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.BuffHorseRace;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Rooms;

public class BaseHorseRaceRoom
{
	public static Random random = new Random();

	public int RoomId;

	public eStepHorseRace StepProcess;

	private GamePlayer[] m_places;

	private int[] m_placesState;

	private byte[] m_playerState;

	public bool IsPlaying;

	public bool IsWaiting;

	private int m_playerCount = 0;

	private int m_placesCount = 0;

	public DateTime TimeStartGame;

	protected Timer _startGameTimer;

	public DateTime CountDownEnd;

	private Dictionary<int, GamePlayer> listPlayersComplete;

	private int HorseGamePlayerCount = GameProperties.HorseGamePlayerCount;

	public BaseHorseRaceRoom(int roomId)
	{
		RoomId = roomId;
		m_places = new GamePlayer[HorseGamePlayerCount];
		m_placesState = new int[HorseGamePlayerCount];
		m_playerState = new byte[HorseGamePlayerCount];
		m_placesCount = HorseGamePlayerCount;
		Reset();
	}

	public void StartGameTimer()
	{
		if (!IsPlaying)
		{
			IsPlaying = true;
			IsWaiting = false;
			CountdownTimes();
			int num = 1000;
			if (_startGameTimer == null)
			{
				_startGameTimer = new Timer(HorseRaceCheck, null, num, num);
			}
			else
			{
				_startGameTimer.Change(num, num);
			}
		}
	}

	public void StopGameTimer()
	{
		if (_startGameTimer != null)
		{
			_startGameTimer.Dispose();
			_startGameTimer = null;
		}
	}

	private void HorseRaceCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			switch (StepProcess)
			{
			case eStepHorseRace.WAITING:
				SendBeginStartGame();
				StepProcess = eStepHorseRace.COUNTDOWN;
				break;
			case eStepHorseRace.COUNTDOWN:
				TimeStartGame = DateTime.Now.AddSeconds(10.0);
				SendCountDown((DateTime.Now - TimeStartGame).Milliseconds);
				StepProcess = eStepHorseRace.WAIT_COUNTDOWN;
				break;
			case eStepHorseRace.WAIT_COUNTDOWN:
				if (!(DateTime.Now >= TimeStartGame))
				{
					break;
				}
				SendStartGame();
				lock (m_places)
				{
					GamePlayer[] places = m_places;
					foreach (GamePlayer gamePlayer2 in places)
					{
						if (gamePlayer2 != null && gamePlayer2.CurrentHorseRaceInfo != null)
						{
							gamePlayer2.CurrentHorseRaceInfo.Reset();
							SendUpdateBuffItem(gamePlayer2, successpingzhang: false);
						}
					}
				}
				StepProcess = eStepHorseRace.STARTING_RACE;
				break;
			case eStepHorseRace.STARTING_RACE:
				if (CanStopGame())
				{
					SendAllFinish();
					StepProcess = eStepHorseRace.END_RACE;
				}
				else
				{
					StartGameChange();
					SendSynGame();
				}
				break;
			case eStepHorseRace.END_RACE:
			{
				StopGameTimer();
				List<GamePlayer> playersWithRank = GetPlayersWithRank();
				foreach (GamePlayer item in playersWithRank)
				{
					SendAward(item.CurrentHorseRaceInfo.Rank, item.PlayerCharacter.NickName, item.PlayerCharacter.ID, item.ZoneId);
				}
				GamePlayer[] playersUnSafe = GetPlayersUnSafe();
				GamePlayer[] array = playersUnSafe;
				foreach (GamePlayer gamePlayer in array)
				{
					if (gamePlayer != null && gamePlayer.CurrentHorseRaceRoom != null)
					{
						gamePlayer.CurrentHorseRaceRoom.RemovePlayerUnsafe(gamePlayer);
					}
				}
				Reset();
				break;
			}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("HorseRace Timer Error: " + ex.ToString());
		}
	}

	private void SendAward(int rank, string nickName, int playerID, int zoneID)
	{
		int[] array = new int[5] { 201574, 201575, 201576, 201577, 201578 };
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(array[rank - 1]);
		if (itemTemplateInfo != null)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 101);
			itemInfo.IsBinds = true;
			itemInfo.ValidDate = 0;
			WorldEventMgr.SendItemToMail(itemInfo, playerID, nickName, zoneID, LanguageMgr.GetTranslation("GameServer.HorseGame.MailAwardTitle", rank));
		}
	}

	private void StartGameChange()
	{
		lock (m_places)
		{
			List<GamePlayer> list = new List<GamePlayer>();
			GamePlayer[] places = m_places;
			foreach (GamePlayer gamePlayer in places)
			{
				if (gamePlayer != null && gamePlayer.CurrentHorseRaceInfo != null)
				{
					if (gamePlayer.CurrentHorseRaceInfo.CheckBuff())
					{
						SendUpdateBuffItem(gamePlayer, successpingzhang: false);
					}
					if (!gamePlayer.CurrentHorseRaceInfo.IsSendSpeed)
					{
						list.Add(gamePlayer);
						gamePlayer.CurrentHorseRaceInfo.IsSendSpeed = true;
					}
					if (gamePlayer.CurrentHorseRaceInfo.RaceLength < RoomMgr.MaxRaceLength)
					{
						gamePlayer.CurrentHorseRaceInfo.ChangePos();
					}
					if (gamePlayer.CurrentHorseRaceInfo.RaceLength >= RoomMgr.MaxRaceLength && !listPlayersComplete.ContainsKey(gamePlayer.PlayerCharacter.ID))
					{
						gamePlayer.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.CompleteNotice"));
						gamePlayer.CurrentHorseRaceInfo.RaceLength = RoomMgr.MaxRaceLength;
						listPlayersComplete.Add(gamePlayer.PlayerCharacter.ID, gamePlayer);
						SendPlayerFinish(listPlayersComplete.Values.ToList());
					}
				}
			}
			if (list.Count > 0)
			{
				SendUpdateSpeed(list);
			}
		}
	}

	private void Reset()
	{
		for (int i = 0; i < HorseGamePlayerCount; i++)
		{
			m_places[i] = null;
			m_placesState[i] = -1;
			m_playerState[i] = 0;
		}
		IsPlaying = false;
		IsWaiting = false;
		m_placesCount = HorseGamePlayerCount;
		m_playerCount = 0;
		StepProcess = eStepHorseRace.WAITING;
		TimeStartGame = DateTime.Now;
		CountDownEnd = DateTime.Now;
		listPlayersComplete = new Dictionary<int, GamePlayer>();
	}

	public bool CanAddPlayer()
	{
		return m_playerCount < m_placesCount;
	}

	public bool CanStartGame()
	{
		if (!IsPlaying && m_playerCount >= m_placesCount)
		{
			return true;
		}
		return false;
	}

	public bool CanStopGame()
	{
		if (IsPlaying && StepProcess == eStepHorseRace.STARTING_RACE)
		{
			if (m_playerCount == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = 0;
			GamePlayer[] places = m_places;
			foreach (GamePlayer gamePlayer in places)
			{
				if (gamePlayer != null)
				{
					num2++;
					if (gamePlayer.CurrentHorseRaceInfo != null && gamePlayer.CurrentHorseRaceInfo.RaceLength >= RoomMgr.MaxRaceLength)
					{
						num++;
					}
				}
			}
			if (num >= num2)
			{
				return true;
			}
		}
		return false;
	}

	public bool AddPlayerUnsafe(GamePlayer player)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < HorseGamePlayerCount; i++)
			{
				if (m_places[i] == null && m_placesState[i] == -1)
				{
					m_places[i] = player;
					m_placesState[i] = player.PlayerId;
					m_playerCount++;
					num = i;
					break;
				}
			}
		}
		if (num != -1)
		{
			player.CurrentHorseRaceRoom = this;
			player.CurrentHorseRaceInfo = new UserHorseRaceInfo(player.PlayerCharacter.ID, player.PlayerCharacter.NickName, player.PlayerCharacter.MountLv, num, 10);
		}
		return num != -1;
	}

	public bool RemovePlayerUnsafe(GamePlayer player)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < HorseGamePlayerCount; i++)
			{
				if (m_places[i] == player)
				{
					m_places[i] = null;
					m_playerState[i] = 0;
					m_placesState[i] = -1;
					m_playerCount--;
					num = i;
					break;
				}
			}
		}
		if (num != -1)
		{
			if (player.CurrentHorseRaceInfo != null)
			{
				player.CurrentHorseRaceInfo.Stop();
			}
			player.CurrentHorseRaceRoom = null;
			player.CurrentHorseRaceInfo = null;
			player.TempBag.ClearBag();
		}
		if (m_playerCount <= 0 && _startGameTimer == null)
		{
			Reset();
		}
		return num != -1;
	}

	public void SendBeginStartGame()
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(RoomId);
		gSPacketIn.WriteInt(playersSafe.Length);
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
			gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Hide);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
			gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(gamePlayer.CurrentHorseRaceInfo.Index);
			gSPacketIn.WriteInt(gamePlayer.CurrentHorseRaceInfo.Speed);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.MountsType);
		}
		SendToALL(gSPacketIn);
	}

	public void SendCountDown(int timerLeft)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(timerLeft);
		SendToALL(gSPacketIn);
	}

	public void SendStartGame()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(4);
		SendToALL(gSPacketIn);
	}

	public void SendPlayerFinish(List<GamePlayer> player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteInt(player.Count);
		foreach (GamePlayer item in player)
		{
			gSPacketIn.WriteInt(item.PlayerCharacter.ID);
		}
		SendToALL(gSPacketIn);
	}

	public void SendAllFinish()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(listPlayersComplete.Count);
		foreach (int key in listPlayersComplete.Keys)
		{
			gSPacketIn.WriteInt(key);
		}
		SendToALL(gSPacketIn);
	}

	public void SendUpdateBuffItem(GamePlayer p, bool successpingzhang)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(p.CurrentHorseRaceInfo.BuffItem1);
		gSPacketIn.WriteInt(p.CurrentHorseRaceInfo.BuffItem2);
		gSPacketIn.WriteBoolean(successpingzhang);
		p.SendTCP(gSPacketIn);
	}

	public void SendShowMsg(string attacker, string buffName, string defender)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteString(attacker);
		gSPacketIn.WriteString(buffName);
		gSPacketIn.WriteString(defender);
		SendToALL(gSPacketIn);
	}

	public void SendUpdateSpeed(List<GamePlayer> lists)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(lists.Count);
		foreach (GamePlayer list in lists)
		{
			gSPacketIn.WriteInt(list.PlayerCharacter.ID);
			gSPacketIn.WriteInt(list.CurrentHorseRaceInfo.Speed);
		}
		SendToALL(gSPacketIn);
	}

	public void SendSynGame()
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		GSPacketIn gSPacketIn = new GSPacketIn(282);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(playersSafe.Length);
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			bool val = (listPlayersComplete.ContainsKey(gamePlayer.PlayerCharacter.ID) ? true : false);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(gamePlayer.CurrentHorseRaceInfo.RaceLength);
			gSPacketIn.WriteBoolean(val);
			List<AbstractHorseRaceBuffer> allBuffers = gamePlayer.CurrentHorseRaceInfo.BuffList.GetAllBuffers();
			gSPacketIn.WriteInt(allBuffers.Count);
			foreach (AbstractHorseRaceBuffer item in allBuffers)
			{
				gSPacketIn.WriteByte((byte)item.Info.Type);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
		}
		SendToALL(gSPacketIn);
	}

	private void CountdownTimes()
	{
		GamePlayer[] playersUnSafe = GetPlayersUnSafe();
		GamePlayer[] array = playersUnSafe;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.RemoveHorseRaceTimes(1);
		}
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		List<GamePlayer> list = new List<GamePlayer>();
		lock (m_places)
		{
			GamePlayer[] places = m_places;
			foreach (GamePlayer gamePlayer in places)
			{
				if (gamePlayer != null)
				{
					list.Add(gamePlayer);
				}
			}
			array = new GamePlayer[list.Count];
			list.CopyTo(array, 0);
		}
		return (array == null) ? new GamePlayer[0] : array;
	}

	public GamePlayer[] GetPlayersUnSafe()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		lock (m_places)
		{
			GamePlayer[] places = m_places;
			foreach (GamePlayer gamePlayer in places)
			{
				if (gamePlayer != null)
				{
					list.Add(gamePlayer);
				}
			}
		}
		return list.ToArray();
	}

	public GamePlayer GetPlayerByID(int userId)
	{
		lock (m_places)
		{
			GamePlayer[] places = m_places;
			foreach (GamePlayer gamePlayer in places)
			{
				if (gamePlayer != null && gamePlayer.PlayerCharacter.ID == userId)
				{
					return gamePlayer;
				}
			}
			return null;
		}
	}

	public List<GamePlayer> GetPlayersWithRank()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		GamePlayer[] playersSafe = GetPlayersSafe();
		IOrderedEnumerable<GamePlayer> orderedEnumerable = from pair in playersSafe
			where pair.CurrentHorseRaceInfo.RaceLength > 0
			orderby pair.CurrentHorseRaceInfo.RaceLength, pair.CurrentHorseRaceInfo.FinishTime descending
			select pair;
		int num = 1;
		foreach (GamePlayer item in orderedEnumerable)
		{
			item.CurrentHorseRaceInfo.Rank = num;
			list.Add(item);
			num++;
		}
		return list;
	}

	public void SendToALL(GSPacketIn packet)
	{
		SendToALL(packet, null);
	}

	public void SendToALL(GSPacketIn packet, GamePlayer except)
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		if (playersSafe == null)
		{
			return;
		}
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != null && gamePlayer != except)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Rooms;

public class BaseRoom
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private GamePlayer[] m_places;

	private int[] m_placesState;

	private byte[] m_playerState;

	protected int m_playerCount = 0;

	protected int m_placesCount = 10;

	private bool m_isUsing = false;

	private GamePlayer m_host;

	private bool m_IsPlaying;

	public bool IsShowLoading;

	public int RoomId;

	public int PickUpNpcId;

	public int maxViewerCnt = 2;

	public DateTime horaInicio;

	private int m_viewerCnt = 0;

	public int GameStyle = 0;

	public int barrierNum = 0;

	public string Name;

	public string Pic;

	public string Password;

	public bool isCrosszone;

	public bool isWithinLeageTime;

	public bool isOpenBoss;

	public eRoomType RoomType;

	public eGameType GameType;

	public eHardLevel HardLevel;

	public int LevelLimits;

	public int currentFloor;

	public byte TimeMode;

	public int MapId;

	public string m_roundName;

	public int ZoneId;

	public int groupType;

	private bool m_startWithNpc;

	private int m_avgLevel = 0;

	private AbstractGame m_game;

	public BattleServer BattleServer;

	public virtual bool IsPlaying
	{
		get
		{
			return m_IsPlaying;
		}
		set
		{
			m_IsPlaying = value;
		}
	}

	public int viewerCnt => m_viewerCnt;

	public GamePlayer Host => m_host;

	public byte[] PlayerState
	{
		get
		{
			return m_playerState;
		}
		set
		{
			m_playerState = value;
		}
	}

	public int PlayerCount
	{
		get
		{
			return m_playerCount;
		}
		set
		{
			m_playerCount = value;
		}
	}

	public int PlacesCount => m_placesCount;

	public int GuildId
	{
		get
		{
			if (m_host == null)
			{
				return 0;
			}
			return m_host.PlayerCharacter.ConsortiaID;
		}
	}

	public bool IsUsing => m_isUsing;

	public string RoundName
	{
		get
		{
			return m_roundName;
		}
		set
		{
			m_roundName = value;
		}
	}

	public bool StartWithNpc
	{
		get
		{
			return m_startWithNpc;
		}
		set
		{
			m_startWithNpc = value;
		}
	}

	public bool NeedPassword => !string.IsNullOrEmpty(Password);

	public bool IsEmpty => m_playerCount == 0;

	public int AvgLevel => m_avgLevel;

	public AbstractGame Game => m_game;

	public bool IsSingle()
	{
		switch (RoomType)
		{
		case eRoomType.Encounter:
		case eRoomType.FightGround:
		case eRoomType.SingleBattle:
		case eRoomType.FightFootballTime:
		case eRoomType.SurvivalMode:
			return true;
		default:
			return false;
		}
	}

	public BaseRoom(int roomId)
	{
		RoomId = roomId;
		m_places = new GamePlayer[10];
		m_placesState = new int[10];
		m_playerState = new byte[10];
		IsShowLoading = false;
		PickUpNpcId = -1;
		Reset();
	}

	public void Start()
	{
		if (!m_isUsing)
		{
			m_isUsing = true;
			Reset();
		}
	}

	public virtual void Stop()
	{
		if (m_isUsing)
		{
			m_isUsing = false;
			if (m_game != null)
			{
				m_game.GameStopped -= m_game_GameStopped;
				m_game = null;
				IsPlaying = false;
			}
			RoomMgr.WaitingRoom.SendUpdateCurrentRoom(this);
		}
	}

	protected virtual void Reset()
	{
		for (int i = 0; i < 10; i++)
		{
			m_places[i] = null;
			m_placesState[i] = -1;
			m_playerState[i] = 0;
		}
		m_host = null;
		IsPlaying = false;
		m_placesCount = 10;
		m_playerCount = 0;
		isCrosszone = false;
		groupType = 0;
		HardLevel = eHardLevel.Easy;
		PickUpNpcId = -1;
		StartWithNpc = false;
		Pic = "";
		MapId = 10000;
		currentFloor = 0;
		isOpenBoss = false;
	}

	public bool CanStart()
	{
		if (RoomType == eRoomType.Freedom)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 10; i++)
			{
				if (i % 2 == 0)
				{
					if (m_playerState[i] > 0)
					{
						num++;
					}
				}
				else if (m_playerState[i] > 0)
				{
					num2++;
				}
			}
			return num > 0 && num2 > 0;
		}
		int num3 = 0;
		for (int j = 0; j < 10; j++)
		{
			if (m_playerState[j] > 0)
			{
				num3++;
			}
		}
		return num3 == m_playerCount;
	}

	public bool CanAddPlayer()
	{
		return m_playerCount < m_placesCount;
	}

	public bool CanAddViewPlayer()
	{
		int num = 0;
		for (int i = 8; i < 10; i++)
		{
			if (m_placesState[i] < 0)
			{
				num++;
			}
		}
		return num > 0;
	}

	public List<GamePlayer> GetPlayers()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		lock (m_places)
		{
			for (int i = 0; i < 10; i++)
			{
				if (m_places[i] != null)
				{
					list.Add(m_places[i]);
				}
			}
		}
		return list;
	}

	public void SetHost(GamePlayer player)
	{
		if (m_host != player)
		{
			if (m_host != null)
			{
				UpdatePlayerState(player, 0, sendToClient: false);
			}
			m_host = player;
			UpdatePlayerState(player, 2, sendToClient: true);
		}
	}

	public void UpdateRoom(string name, string pwd, eRoomType roomType, byte timeMode, int mapId)
	{
		Name = name;
		Password = pwd;
		RoomType = roomType;
		TimeMode = timeMode;
		MapId = mapId;
		UpdateRoomGameType();
		if (roomType == eRoomType.Freedom)
		{
			m_placesCount = 8;
		}
		else
		{
			m_placesCount = 2;
		}
		for (int i = m_placesCount; i < 10; i++)
		{
			m_placesState[i] = 0;
		}
	}

	public void UpdateRoomGameType()
	{
		switch (RoomType)
		{
		case eRoomType.FightLib:
			GameType = eGameType.FightLib;
			break;
		case eRoomType.RingStation:
			GameType = eGameType.RingStation;
			break;
		case eRoomType.ConsortiaBoss:
			GameType = eGameType.ConsortiaBoss;
			break;
		case eRoomType.Dungeon:
		case eRoomType.AcademyDungeon:
		case eRoomType.Lanbyrinth:
		case eRoomType.ActivityDungeon:
		case eRoomType.SpecialActivityDungeon:
		case eRoomType.FarmBoss:
		case eRoomType.Christmas:
		case eRoomType.Cryptboss:
		case eRoomType.Element_PVE:
			GameType = eGameType.Dungeon;
			break;
		case eRoomType.CampBattleBattle:
			GameType = eGameType.CampBattleModelPve;
			break;
		case eRoomType.Freshman:
			GameType = eGameType.Freshman;
			break;
		case eRoomType.Entertainment:
			GameType = eGameType.EntertainmentMode;
			break;
		case eRoomType.EntertainmentPK:
			GameType = eGameType.EntertainmentModePK;
			break;
		case eRoomType.Match:
		case eRoomType.Freedom:
		case eRoomType.PolarRegion:
			GameType = eGameType.Free;
			break;
		case eRoomType.FightGround:
			GameType = eGameType.BattleGame;
			break;
		case eRoomType.Encounter:
			GameType = eGameType.Encounter;
			break;
		case eRoomType.ConsortiaBattle:
			GameType = eGameType.ConsortiaBattle;
			break;
		case eRoomType.FightFootballTime:
			GameType = eGameType.FightFootballTime;
			break;
		case eRoomType.SurvivalMode:
			GameType = eGameType.Survival;
			break;
		case eRoomType.BombKing:
			GameType = eGameType.BombKing;
			break;
		default:
			GameType = eGameType.ALL;
			break;
		}
	}

	public void UpdatePlayerState(GamePlayer player, byte state, bool sendToClient)
	{
		m_playerState[player.CurrentRoomIndex] = state;
		if (sendToClient)
		{
			SendPlayerState();
		}
	}

	public void UpdateAvgLevel()
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (m_places[i] != null)
			{
				num += m_places[i].PlayerCharacter.Grade;
			}
		}
		if (m_placesCount > 0 && num > 0)
		{
			m_avgLevel = num / m_playerCount;
		}
	}

	public void SendToAll(GSPacketIn pkg, IGamePlayer except)
	{
	}

	public void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public void SendToAll(GSPacketIn pkg, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_places)
		{
			array = (GamePlayer[])m_places.Clone();
		}
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i] != except)
			{
				array[i].Out.SendTCP(pkg);
			}
		}
	}

	public void SendToTeam(GSPacketIn pkg, int team)
	{
		SendToTeam(pkg, team, null);
	}

	public void SendToTeam(GSPacketIn pkg, int team, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_places)
		{
			array = (GamePlayer[])m_places.Clone();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].CurrentRoomTeam == team && array[i] != except)
			{
				array[i].Out.SendTCP(pkg);
			}
		}
	}

	public void SendToHost(GSPacketIn pkg)
	{
		GamePlayer[] array = null;
		lock (m_places)
		{
			array = (GamePlayer[])m_places.Clone();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i] == Host)
			{
				array[i].Out.SendTCP(pkg);
			}
		}
	}

	public void SendPlayerState()
	{
		GSPacketIn pkg = m_host.Out.SendRoomUpdatePlayerStates(m_playerState);
		SendToAll(pkg, m_host);
	}

	public void SendPlaceState()
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendRoomUpdatePlacesStates(m_placesState);
			SendToAll(pkg, m_host);
		}
	}

	public void SendCancelPickUp()
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendRoomPairUpCancel(this);
			SendToAll(pkg, m_host);
		}
	}

	public void SendStartPickUp()
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendRoomPairUpStart(this);
			SendToAll(pkg, m_host);
		}
	}

	public void SendMessage(eMessageType type, string msg)
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendMessage(type, msg);
			SendToAll(pkg, m_host);
		}
	}

	public void SendRoomSetupChange(BaseRoom room)
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendGameRoomSetupChange(room);
			SendToAll(pkg, m_host);
		}
	}

	public bool UpdatePosUnsafe(int pos, bool isOpened, int place, int placeView)
	{
		if (pos < 0 || pos > 9)
		{
			return false;
		}
		if (m_placesState[pos] != place)
		{
			if (m_places[pos] != null)
			{
				RemovePlayerUnsafe(m_places[pos]);
			}
			m_placesState[pos] = place;
			SendPlaceState();
			if (place == -1)
			{
				if (pos < 8)
				{
					m_placesCount++;
				}
			}
			else if (pos < 8)
			{
				m_placesCount--;
			}
			return true;
		}
		return false;
	}

	public bool IsAllSameGuild()
	{
		int guildId = GuildId;
		if (guildId != 0)
		{
			List<GamePlayer> players = GetPlayers();
			if (players.Count >= 2)
			{
				foreach (GamePlayer item in players)
				{
					if (item.PlayerCharacter.ConsortiaID != guildId)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public void UpdateGameStyle()
	{
		if (m_host != null && RoomType == eRoomType.Match)
		{
			if (IsAllSameGuild())
			{
				GameStyle = 1;
				GameType = eGameType.Guild;
			}
			else
			{
				GameStyle = 0;
				GameType = eGameType.Free;
			}
			GSPacketIn pkg = m_host.Out.SendRoomType(m_host, this);
			SendToAll(pkg);
		}
	}

	public bool AddPlayerCampBattle(GamePlayer player)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < 8; i++)
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
			player.CurrentRoom = this;
			player.CurrentRoomIndex = num;
			player.CurrentRoomTeam = 1;
			if (m_host == null)
			{
				m_host = player;
				UpdatePlayerState(player, 2, sendToClient: false);
			}
			else
			{
				UpdatePlayerState(player, 0, sendToClient: false);
			}
		}
		return num != -1;
	}

	public bool AddPlayerUnsafe(GamePlayer player)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < 10; i++)
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
			player.CurrentRoom = this;
			player.CurrentRoomIndex = num;
			if (RoomType == eRoomType.Freedom)
			{
				player.CurrentRoomTeam = num % 2 + 1;
			}
			else
			{
				player.CurrentRoomTeam = 1;
			}
			if (num >= 8)
			{
				player.PlayerCharacter.isViewer = true;
			}
			if (player.Extra.Info.curcentRoomBordenTemplateId > 0)
			{
				ItemInfo itemByTemplateID = player.RoomBordenBag.GetItemByTemplateID(player.Extra.Info.curcentRoomBordenTemplateId);
				if (itemByTemplateID == null || !itemByTemplateID.IsValidItem())
				{
					if (itemByTemplateID != null)
					{
						player.RoomBordenBag.ClearVaildItem();
					}
					player.Extra.Info.curcentRoomBordenTemplateId = 0;
				}
			}
			GSPacketIn pkg = player.Out.SendRoomPlayerAdd(player);
			SendToAll(pkg, player);
			GSPacketIn pkg2 = player.Out.SendBufferList(player, player.BufferList.GetAllBufferByTemplate());
			SendToAll(pkg2, player);
			List<GamePlayer> players = GetPlayers();
			foreach (GamePlayer item in players)
			{
				if (item != player)
				{
					player.Out.SendRoomPlayerAdd(item);
					player.Out.SendBufferList(item, item.BufferList.GetAllBufferByTemplate());
				}
			}
			if (m_host == null)
			{
				m_host = player;
				UpdatePlayerState(player, 2, sendToClient: true);
			}
			else
			{
				UpdatePlayerState(player, 0, sendToClient: true);
			}
			SendPlaceState();
			UpdateGameStyle();
			if (RoomType == eRoomType.ConsortiaBattle)
			{
				GameMgr.GuildBattle.AddPlayer(player);
			}
			if (RoomType == eRoomType.CampBattleBattle)
			{
				GameMgr.CampBattle.AddPlayer(player);
			}
			if (RoomType == eRoomType.ConsortiaDomain)
			{
				GameMgr.ConsortiaDomain.AddPlayer(player);
			}
		}
		return num != -1;
	}

	public bool RemovePlayerUnsafe(GamePlayer player)
	{
		return RemovePlayerUnsafe(player, isKick: false);
	}

	public bool RemovePlayerUnsafe(GamePlayer player, bool isKick)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < 10; i++)
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
			UpdatePosUnsafe(num, isOpened: false, -1, -100);
			player.CurrentRoom = null;
			player.TempBag.ClearBag();
			GSPacketIn pkg = player.Out.SendRoomPlayerRemove(player);
			SendToAll(pkg);
			if (isKick)
			{
				player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));
			}
			bool flag = false;
			if (m_host == player)
			{
				if (m_playerCount > 0)
				{
					for (int j = 0; j < 10; j++)
					{
						if (m_places[j] != null)
						{
							SetHost(m_places[j]);
							flag = true;
							break;
						}
					}
				}
				else
				{
					m_host = null;
				}
			}
			if (IsPlaying)
			{
				if (!string.IsNullOrEmpty(player.PlayerCharacter.tempStyle))
				{
					player.UpdatePublicPlayer();
				}
				if (m_game != null)
				{
					if (flag && m_game is PVEGame)
					{
						PVEGame pVEGame = m_game as PVEGame;
						foreach (Player value in pVEGame.Players.Values)
						{
							if (value.PlayerDetail == m_host)
							{
								value.Ready = false;
							}
						}
					}
					m_game.RemovePlayer(player, isKick);
				}
				if (BattleServer != null)
				{
					if (m_game != null)
					{
						BattleServer.Server.SendPlayerDisconnet(Game.Id, player.TempGameId, RoomId);
						if (PlayerCount == 0)
						{
							BattleServer.RemoveRoom(this);
						}
					}
					else
					{
						SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed"));
						RoomMgr.AddAction(new CancelPickupAction(BattleServer, this));
						BattleServer.RemoveRoom(this);
						IsPlaying = false;
					}
				}
			}
			else
			{
				UpdateGameStyle();
				if (flag)
				{
					if (RoomType == eRoomType.Dungeon || RoomType == eRoomType.Element_PVE)
					{
						HardLevel = eHardLevel.Normal;
					}
					else
					{
						HardLevel = eHardLevel.Easy;
					}
					foreach (GamePlayer player2 in GetPlayers())
					{
						player2.Out.SendGameRoomSetupChange(this);
					}
				}
			}
			if (RoomType == eRoomType.ConsortiaBattle)
			{
				GameMgr.GuildBattle.RemovePlayer(player);
			}
			if (RoomType == eRoomType.CampBattleBattle)
			{
				GameMgr.CampBattle.RemovePlayer(player);
			}
			if (RoomType == eRoomType.ConsortiaDomain)
			{
				GameMgr.ConsortiaDomain.RemovePlayer(player);
			}
		}
		return num != -1;
	}

	public void RemovePlayerAtUnsafe(int pos)
	{
		if (pos >= 0 && pos <= 9 && m_places[pos] != null)
		{
			if (m_places[pos].KickProtect)
			{
				string translation = LanguageMgr.GetTranslation("Game.Server.SceneGames.Protect", m_places[pos].PlayerCharacter.NickName);
				GSPacketIn gSPacketIn = new GSPacketIn(3);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteString(translation);
				SendToHost(gSPacketIn);
			}
			else
			{
				RemovePlayerUnsafe(m_places[pos], isKick: true);
			}
		}
	}

	public bool SwitchTeamUnsafe(GamePlayer m_player)
	{
		if (RoomType == eRoomType.Match)
		{
			return false;
		}
		int num = -1;
		lock (m_places)
		{
			for (int i = (m_player.CurrentRoomIndex + 1) % 2; i < 8; i += 2)
			{
				if (m_places[i] == null && m_placesState[i] == -1)
				{
					num = i;
					m_places[m_player.CurrentRoomIndex] = null;
					m_places[i] = m_player;
					m_placesState[m_player.CurrentRoomIndex] = -1;
					m_placesState[i] = m_player.PlayerId;
					m_playerState[i] = m_playerState[m_player.CurrentRoomIndex];
					m_playerState[m_player.CurrentRoomIndex] = 0;
					break;
				}
			}
		}
		if (num != -1)
		{
			m_player.CurrentRoomIndex = num;
			m_player.CurrentRoomTeam = num % 2 + 1;
			GSPacketIn pkg = m_player.Out.SendRoomPlayerChangedTeam(m_player);
			SendToAll(pkg, m_player);
			SendPlaceState();
			return true;
		}
		return false;
	}

	public eLevelLimits GetLevelLimit(GamePlayer player)
	{
		if (player.PlayerCharacter.Grade <= 10)
		{
			return eLevelLimits.ZeroToTen;
		}
		if (player.PlayerCharacter.Grade <= 20)
		{
			return eLevelLimits.ElevenToTwenty;
		}
		return eLevelLimits.TwentyOneToThirty;
	}

	public void StartGame(AbstractGame game)
	{
		if (m_game != null)
		{
			List<GamePlayer> players = GetPlayers();
			foreach (GamePlayer item in players)
			{
				m_game.RemovePlayer(item, IsKick: false);
			}
			m_game_GameStopped(m_game);
		}
		horaInicio = DateTime.Now;
		m_game = game;
		IsPlaying = true;
		m_game.GameStopped += m_game_GameStopped;
	}

	private void m_game_GameStopped(AbstractGame game)
	{
		if (game == null)
		{
			return;
		}
		List<GamePlayer> players = GetPlayers();
		foreach (GamePlayer item in players)
		{
			if (!string.IsNullOrEmpty(item.PlayerCharacter.tempStyle))
			{
				item.UpdatePublicPlayer();
			}
		}
		m_game.GameStopped -= m_game_GameStopped;
		m_game = null;
		IsPlaying = false;
		RoomMgr.WaitingRoom.SendUpdateCurrentRoom(this);
		horaInicio = DateTime.MinValue;
	}

	public void ResetPlayerState()
	{
		for (int i = 0; i < m_playerState.Length; i++)
		{
			if (m_playerState[i] != 2)
			{
				m_playerState[i] = 0;
			}
		}
	}

	public string GetNameByMapId()
	{
		string text = LanguageMgr.GetTranslation("BaseRoom.Msg1");
		string translation = LanguageMgr.GetTranslation("BaseRoom.Msg2");
		MapInfo mapInfo = MapMgr.FindMapInfo(MapId);
		if (mapInfo != null)
		{
			text = mapInfo.Name;
		}
		PveInfo pveInfoById = PveInfoMgr.GetPveInfoById(MapId);
		if (pveInfoById != null)
		{
			text = pveInfoById.Name;
			text = (((pveInfoById.ID == 2 || pveInfoById.ID == 7) && HardLevel >= eHardLevel.Epic) ? (text + LanguageMgr.GetTranslation("BaseRoom.Msg9")) : (((pveInfoById.ID != 5 && pveInfoById.ID != 13) || HardLevel != eHardLevel.Nightmare) ? (text + GetNameHardLv()) : (text + LanguageMgr.GetTranslation("BaseRoom.Msg8"))));
		}
		else
		{
			translation = LanguageMgr.GetTranslation("BaseRoom.Msg3");
		}
		return translation + text;
	}

	public string GetNameHardLv()
	{
		string translation = LanguageMgr.GetTranslation("BaseRoom.Msg4");
		switch (HardLevel)
		{
		case eHardLevel.Normal:
			translation = LanguageMgr.GetTranslation("BaseRoom.Msg5");
			break;
		case eHardLevel.Hard:
			translation = LanguageMgr.GetTranslation("BaseRoom.Msg6");
			break;
		case eHardLevel.Terror:
			translation = LanguageMgr.GetTranslation("BaseRoom.Msg7");
			break;
		case eHardLevel.Epic:
			translation = LanguageMgr.GetTranslation("BaseRoom.Msg8");
			break;
		case eHardLevel.Nightmare:
			translation = LanguageMgr.GetTranslation("BaseRoom.Msg9");
			break;
		}
		return translation;
	}

	public int GetDungeonTicketId(int mapId)
	{
		int result = 0;
		switch (mapId)
		{
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
			result = 100000019;
			break;
		case 12016:
			result = 11742;
			break;
		}
		return result;
	}

	public void ProcessData(GSPacketIn packet)
	{
		if (m_game != null)
		{
			m_game.ProcessData(packet);
		}
	}

	public void RemoveAllPlayer()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_places[i] != null)
			{
				RoomMgr.AddAction(new ExitRoomAction(this, m_places[i]));
				RoomMgr.AddAction(new EnterWaitingRoomAction(m_places[i]));
			}
		}
	}

	public override string ToString()
	{
		return $"Id:{RoomId},player:{PlayerCount},game:{Game},isPlaying:{IsPlaying}";
	}

	public int GetDungeonTicketId(int mapId, eHardLevel level)
	{
		int result = 0;
		switch (mapId)
		{
		case 13:
			switch (level)
			{
			case eHardLevel.Easy:
				result = 200619;
				break;
			case eHardLevel.Normal:
				result = 200620;
				break;
			case eHardLevel.Hard:
				result = 200621;
				break;
			case eHardLevel.Terror:
				result = 200622;
				break;
			case eHardLevel.Epic:
				result = 201105;
				break;
			}
			break;
		case 12016:
			result = 11742;
			break;
		case 15001:
			result = 201628;
			break;
		case 16001:
			result = 201629;
			break;
		}
		return result;
	}

	public bool IsElementPVE(int MapID)
	{
		if ((uint)(MapID - 33) <= 6u)
		{
			return true;
		}
		return false;
	}

	public int GetTicketID(int MapID)
	{
		int result = 0;
		switch (MapID)
		{
		case 33:
			result = 100000010;
			break;
		case 34:
			result = 100000011;
			break;
		case 35:
			result = 100000012;
			break;
		case 36:
			result = 100000013;
			break;
		case 37:
			result = 100000014;
			break;
		case 38:
			result = 100000015;
			break;
		case 39:
			result = 100000016;
			break;
		}
		return result;
	}
}

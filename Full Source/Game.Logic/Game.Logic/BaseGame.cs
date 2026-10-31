using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class BaseGame : AbstractGame
{
	public delegate void GameOverLogEventHandle(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar);

	public delegate void GameNpcDieEventHandle(int NpcId);

	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public readonly int[] EquipPlace = new int[15]
	{
		1, 2, 3, 4, 5, 6, 11, 13, 14, 15,
		16, 17, 18, 19, 20
	};

	protected int turnIndex;

	protected int m_nextPlayerId;

	protected int m_nextWind;

	protected int m_nextWindRate;

	protected eGameState m_gameState;

	protected eGameState m_gameSecondState;

	protected Map m_map;

	protected Dictionary<int, Player> m_players;

	protected Dictionary<int, SimpleBoss> m_boss;

	protected List<Living> m_livings;

	protected Random m_random;

	protected TurnedLiving m_currentLiving;

	public TurnedLiving LastTurnLiving;

	public int PhysicalId;

	public int CurrentTurnTotalDamage;

	public int TotalHurt;

	public int redScore;

	public int blueScore;

	public int ConsortiaAlly;

	public int RichesRate;

	public string BossWarField;

	private ArrayList m_actions;

	private List<TurnedLiving> m_turnQueue;

	protected Dictionary<int, string> m_logStartIps;

	private int m_roomId;

	public int[] Cards;

	private int m_lifeTime = 0;

	private long m_waitTimer = 0L;

	private long m_lastWaitTimer = 0L;

	private long m_passTick = 0L;

	public int CurrentActionCount = 0;

	public int loadBossID = 0;

	private List<Ball> m_tempBall;

	private List<SimpleBox> m_tempBox;

	private List<Point> m_tempPoints;

	private List<Point> m_tempGhostPoints;

	private List<LoadingFileInfo> m_loadingFiles = new List<LoadingFileInfo>();

	public int TotalCostMoney;

	public int TotalCostGold;

	protected int m_turnIndex
	{
		get
		{
			return turnIndex;
		}
		set
		{
			turnIndex = value;
		}
	}

	public int RoomId => m_roomId;

	public Dictionary<int, Player> Players => m_players;

	public int PlayerCount
	{
		get
		{
			lock (m_players)
			{
				return m_players.Count;
			}
		}
	}

	public int TurnIndex
	{
		get
		{
			return m_turnIndex;
		}
		set
		{
			m_turnIndex = value;
		}
	}

	public int nextPlayerId
	{
		get
		{
			return m_nextPlayerId;
		}
		set
		{
			m_nextPlayerId = value;
		}
	}

	public eGameState GameState => m_gameState;

	public eGameState GameSecondState => m_gameSecondState;

	public float Wind => m_map.wind;

	public float WindRate => m_map.windRate;

	public Map Map => m_map;

	public List<TurnedLiving> TurnQueue => m_turnQueue;

	public bool HasPlayer => m_players.Count > 0;

	public Random Random => m_random;

	public TurnedLiving CurrentLiving => m_currentLiving;

	public int LifeTime => m_lifeTime;

	public event GameEventHandle GameOverred;

	public event GameEventHandle BeginNewTurn;

	public event GameOverLogEventHandle GameOverLog;

	public event GameNpcDieEventHandle GameNpcDie;

	public BaseGame(int id, int roomId, Map map, eRoomType roomType, eGameType gameType, int timeType)
		: base(id, roomType, gameType, timeType)
	{
		m_roomId = roomId;
		m_players = new Dictionary<int, Player>();
		m_turnQueue = new List<TurnedLiving>();
		m_livings = new List<Living>();
		m_random = new Random();
		m_logStartIps = new Dictionary<int, string>();
		m_map = map;
		m_actions = new ArrayList();
		PhysicalId = 0;
		BossWarField = "";
		m_tempBall = new List<Ball>();
		m_tempBox = new List<SimpleBox>();
		m_tempPoints = new List<Point>();
		m_tempGhostPoints = new List<Point>();
		if (roomType == eRoomType.Dungeon || roomType == eRoomType.SpecialActivityDungeon || roomType == eRoomType.SurvivalMode || roomType == eRoomType.Element_PVE)
		{
			Cards = new int[21];
		}
		else
		{
			Cards = new int[9];
		}
		m_gameState = eGameState.Inited;
	}

	public void SetWind(int wind)
	{
		m_map.wind = wind;
	}

	public bool SetMap(int mapId)
	{
		if (GameState == eGameState.Playing)
		{
			return false;
		}
		Map map = MapMgr.CloneMap(mapId);
		if (map != null)
		{
			m_map = map;
			return true;
		}
		return false;
	}

	public int GetTurnWaitTime()
	{
		return m_timeType;
	}

	protected void AddLogIp(int id, string ip)
	{
		lock (m_logStartIps)
		{
			if (!m_logStartIps.ContainsKey(id))
			{
				m_logStartIps.Add(id, ip);
			}
		}
	}

	protected void AddPlayer(Player fp)
	{
		m_players.Add(fp.Id, fp);
		if (fp.Weapon != null)
		{
			m_turnQueue.Add(fp);
		}
	}

	public bool UpdateTarget(int Id, int targetId)
	{
		lock (m_players)
		{
			if (m_players.ContainsKey(Id))
			{
				m_players[Id].targetId = targetId;
				return true;
			}
		}
		return false;
	}

	public bool IsTargetLife(int targetId)
	{
		if (m_players.ContainsKey(targetId))
		{
			return m_players[targetId].IsLiving;
		}
		return false;
	}

	public bool IsUseAmulet()
	{
		eRoomType roomType = base.RoomType;
		eRoomType eRoomType2 = roomType;
		if ((uint)eRoomType2 <= 1u || eRoomType2 == eRoomType.Dungeon)
		{
			return true;
		}
		return false;
	}

	public bool IsPVE()
	{
		switch (base.RoomType)
		{
		case eRoomType.Dungeon:
		case eRoomType.Lanbyrinth:
		case eRoomType.CampBattleBattle:
		case eRoomType.ActivityDungeon:
		case eRoomType.Element_PVE:
			return true;
		default:
			return false;
		}
	}

	public virtual void AddLiving(Living living)
	{
		m_map.AddPhysical(living);
		if (living is Player)
		{
			Player player = living as Player;
			if (player.Weapon == null)
			{
				return;
			}
		}
		if (living is TurnedLiving)
		{
			m_turnQueue.Add(living as TurnedLiving);
		}
		else
		{
			m_livings.Add(living);
		}
		if (!(living is Player))
		{
			SendAddLiving(living);
		}
	}

	public virtual void AddGhostBoxObj(PhysicalObj phy)
	{
		m_map.AddPhysical(phy);
		phy.SetGame(this);
	}

	public virtual void AddPhysicalObj(PhysicalObj phy, bool sendToClient)
	{
		m_map.AddPhysical(phy);
		phy.SetGame(this);
		if (sendToClient)
		{
			SendAddPhysicalObj(phy);
		}
	}

	public virtual void AddPhysicalTip(PhysicalObj phy, bool sendToClient)
	{
		m_map.AddPhysical(phy);
		phy.SetGame(this);
		if (sendToClient)
		{
			SendAddPhysicalTip(phy);
		}
	}

	public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
	{
		Player player = null;
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.PlayerDetail == gp)
				{
					player = value;
					m_players.Remove(value.Id);
					break;
				}
			}
		}
		if (player != null)
		{
			AddAction(new RemovePlayerAction(player));
		}
		return player;
	}

	public void RemovePhysicalObj(PhysicalObj phy, bool sendToClient)
	{
		m_map.RemovePhysical(phy);
		phy.SetGame(null);
		if (sendToClient)
		{
			SendRemovePhysicalObj(phy);
		}
	}

	public void RemoveLiving(int id)
	{
		foreach (Living living in m_livings)
		{
			if (living.Id == id)
			{
				m_map.RemovePhysical(living);
				if (living is TurnedLiving)
				{
					m_turnQueue.Remove(living as TurnedLiving);
				}
				else
				{
					m_livings.Remove(living);
				}
			}
		}
		SendRemoveLiving(id);
	}

	public List<Living> GetLivedLivings()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public List<Living> GetLivedLivingsHadTurn()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living is SimpleNpc && living.Config.HasTurn)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public List<Living> GetBossLivings()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living is SimpleBoss)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public void ClearAllChild()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living is SimpleNpc)
			{
				list.Add(living);
			}
		}
		foreach (Living item in list)
		{
			m_livings.Remove(item);
			item.Dispose();
			RemoveLiving(item.Id);
		}
	}

	public void RemoveNPC(int Id)
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.Id == Id)
			{
				list.Add(living);
			}
		}
		foreach (Living item in list)
		{
			m_livings.Remove(item);
			item.Dispose();
			RemoveLiving(item.Id);
		}
	}

	public List<Living> GetFightFootballLivings()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public void ClearAllNpc()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc)
			{
				list.Add(living);
			}
		}
		foreach (Living item in list)
		{
			m_livings.Remove(item);
			item.Dispose();
			SendRemoveLiving(item.Id);
		}
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item2 in allPhysicalSafe)
		{
			if (item2 is SimpleNpc)
			{
				m_map.RemovePhysical(item2);
			}
		}
	}

	public void ClearDiedPhysicals()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (!living.IsLiving)
			{
				list.Add(living);
			}
		}
		foreach (Living item2 in list)
		{
			m_livings.Remove(item2);
			item2.Dispose();
		}
		List<Living> list2 = new List<Living>();
		foreach (TurnedLiving item3 in m_turnQueue)
		{
			if (!item3.IsLiving)
			{
				list2.Add(item3);
			}
		}
		foreach (TurnedLiving item4 in list2)
		{
			m_turnQueue.Remove(item4);
		}
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item5 in allPhysicalSafe)
		{
			if (!item5.IsLiving && !(item5 is Player))
			{
				m_map.RemovePhysical(item5);
			}
		}
	}

	public bool IsAllComplete()
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (GameProperties.VERSION >= 7600)
			{
				item.InitGuardCoreEffect();
			}
			if (item.LoadingProcess < 100)
			{
				return false;
			}
		}
		return true;
	}

	public Player FindPlayer(int id)
	{
		lock (m_players)
		{
			if (m_players.ContainsKey(id))
			{
				return m_players[id];
			}
		}
		return null;
	}

	public Player FindFarPlayer(int x, int y)
	{
		lock (m_players)
		{
			double num = double.MinValue;
			Player result = null;
			foreach (Player value in m_players.Values)
			{
				if (value.IsLiving)
				{
					double num2 = value.Distance(x, y);
					if (num2 > num)
					{
						num = num2;
						result = value;
					}
				}
			}
			return result;
		}
	}

	public TurnedLiving FindNextTurnedFightFootball()
	{
		if (m_turnQueue.Count == 0)
		{
			return null;
		}
		int index = m_random.Next(m_turnQueue.Count - 1);
		TurnedLiving turnedLiving = m_turnQueue[index];
		if (TurnIndex > 0)
		{
			for (int i = 0; i < m_turnQueue.Count; i++)
			{
				if ((m_turnQueue[i] as Player).PlayerDetail.PlayerCharacter.ID == m_nextPlayerId)
				{
					turnedLiving = m_turnQueue[i];
					break;
				}
			}
		}
		turnedLiving.TurnNum++;
		for (int j = 0; j < m_turnQueue.Count; j++)
		{
			if (m_turnQueue[j].Team != turnedLiving.Team && m_turnQueue[j].TurnNum < turnedLiving.TurnNum)
			{
				m_nextPlayerId = (m_turnQueue[j] as Player).PlayerDetail.PlayerCharacter.ID;
				break;
			}
			index = m_random.Next(m_turnQueue.Count - 1);
			m_nextPlayerId = (m_turnQueue[index] as Player).PlayerDetail.PlayerCharacter.ID;
		}
		return turnedLiving;
	}

	public TurnedLiving FindNextTurnedLiving(bool isPvP)
	{
		if (m_turnQueue.Count == 0)
		{
			return null;
		}
		TurnedLiving turnedLiving = null;
		if (TurnIndex == 1 && isPvP)
		{
			double num = -1.0;
			for (int i = 0; i < m_turnQueue.Count; i++)
			{
				if ((!(m_turnQueue[i] is Player) || !(m_turnQueue[i] as Player).PlayerDetail.PlayerCharacter.isViewer) && m_turnQueue[i].IsLiving && m_turnQueue[i] is Player)
				{
					double num2 = (m_turnQueue[i] as Player).Agility + (m_turnQueue[i] as Player).Agility * ((m_turnQueue[i] as Player).Speed * 0.053) / 100.0;
					if (num2 > num)
					{
						turnedLiving = m_turnQueue[i];
						num = (m_turnQueue[i] as Player).Agility + (m_turnQueue[i] as Player).Agility * ((m_turnQueue[i] as Player).Speed * 0.053) / 100.0;
					}
				}
			}
		}
		if (turnedLiving == null)
		{
			int index = m_random.Next(m_turnQueue.Count - 1);
			turnedLiving = m_turnQueue[index];
			double agility = turnedLiving.Agility;
			double num3 = 0.0;
			num3 = ((!(1600.0 - 1200.0 * agility / (agility + 1200.0) - turnedLiving.Speed / 3.0 < 0.0)) ? (10000.0 * ((1600.0 - 1200.0 * agility / (agility + 1200.0) - (1600.0 - 1200.0 * agility / (agility + 1200.0) - turnedLiving.Speed / 3.0)) / (1600.0 - 1200.0 * agility / (agility + 1200.0))) / 100.0) : 0.0);
			double num4 = (double)turnedLiving.Delay - (double)turnedLiving.Delay * num3 / 100.0;
			for (int j = 0; j < m_turnQueue.Count; j++)
			{
				if ((!(m_turnQueue[j] is Player) || !(m_turnQueue[j] as Player).PlayerDetail.PlayerCharacter.isViewer) && (m_turnQueue[j].Config.HasTurn || m_turnQueue[j] is Player) && m_turnQueue[j].IsLiving)
				{
					double agility2 = m_turnQueue[j].Agility;
					double num5 = 0.0;
					num5 = ((!(1600.0 - 1200.0 * agility2 / (agility2 + 1200.0) - turnedLiving.Speed / 3.0 < 0.0)) ? (10000.0 * ((1600.0 - 1200.0 * agility2 / (agility2 + 1200.0) - (1600.0 - 1200.0 * agility2 / (agility2 + 1200.0) - m_turnQueue[j].Speed / 3.0)) / (1600.0 - 1200.0 * agility2 / (agility2 + 1200.0))) / 100.0) : 0.0);
					double num6 = (double)m_turnQueue[j].Delay - (double)m_turnQueue[j].Delay * num5 / 100.0;
					if (num6 < num4)
					{
						num4 = num6;
						turnedLiving = m_turnQueue[j];
					}
				}
			}
		}
		turnedLiving.TurnNum++;
		return turnedLiving;
	}

	public TurnedLiving[] GetNextAllTurnedLiving()
	{
		if (m_turnQueue.Count == 0)
		{
			return null;
		}
		List<TurnedLiving> list = new List<TurnedLiving>();
		for (int i = 0; i < m_turnQueue.Count; i++)
		{
			if (m_turnQueue[i].IsLiving && !m_turnQueue[i].IsFrost && !m_turnQueue[i].IsAttacking && m_turnQueue[i] is Player)
			{
				m_turnQueue[i].TurnNum++;
				list.Add(m_turnQueue[i]);
			}
		}
		return list.ToArray();
	}

	public virtual void MinusDelays(int lowestDelay)
	{
		foreach (TurnedLiving item in m_turnQueue)
		{
			item.Delay -= lowestDelay;
		}
	}

	public SimpleBoss[] FindAllBoss()
	{
		List<SimpleBoss> list = new List<SimpleBoss>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleBoss)
			{
				list.Add(living as SimpleBoss);
			}
		}
		return list.ToArray();
	}

	public SimpleNpc[] FindAllNpc()
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc)
			{
				list.Add(living as SimpleNpc);
			}
		}
		return list.ToArray();
	}

	public SimpleNpc[] FindAllNpcLiving()
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc && living.IsLiving)
			{
				list.Add(living as SimpleNpc);
			}
		}
		return list.ToArray();
	}

	public SimpleNpc[] FindNpcLiving(Point p)
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc && living.IsLiving && living.X == p.X && living.Y == p.Y)
			{
				list.Add(living as SimpleNpc);
			}
		}
		return list.ToArray();
	}

	public SimpleNpc[] GetNPCLivingWithID(int id)
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc && living.IsLiving && (living as SimpleNpc).NpcInfo.ID == id)
			{
				list.Add(living as SimpleNpc);
			}
		}
		return list.ToArray();
	}

	public SimpleBoss[] FindLivingTurnBossWithID(int id)
	{
		List<SimpleBoss> list = new List<SimpleBoss>();
		foreach (TurnedLiving item in m_turnQueue)
		{
			if (item is SimpleBoss && item.IsLiving && (item as SimpleBoss).NpcInfo.ID == id)
			{
				list.Add(item as SimpleBoss);
			}
		}
		return list.ToArray();
	}

	public List<TurnedLiving> FindAllTurnLiving()
	{
		List<TurnedLiving> list = new List<TurnedLiving>();
		foreach (TurnedLiving item in m_turnQueue)
		{
			if ((item is Player && item.IsLiving) || (item is SimpleBoss && item.Config.HasTurn && item.IsLiving))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void RemoveLiving(Living living, bool sendToClient)
	{
		m_map.RemovePhysical(living);
		if (sendToClient)
		{
			SendRemoveLiving(living.Id);
		}
	}

	public List<Living> FindAllTurnBossLiving()
	{
		List<Living> list = new List<Living>();
		foreach (TurnedLiving item in m_turnQueue)
		{
			if (item is SimpleBoss && item.IsLiving)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<SimpleBoss> FindAllTurnBoss()
	{
		List<SimpleBoss> list = new List<SimpleBoss>();
		foreach (TurnedLiving item in m_turnQueue)
		{
			if (item is SimpleBoss)
			{
				list.Add(item as SimpleBoss);
			}
		}
		return list;
	}

	public int GetHighDelayTurn()
	{
		List<Living> list = new List<Living>();
		int num = int.MinValue;
		foreach (TurnedLiving item in m_turnQueue)
		{
			if (item != null && item.Delay > num)
			{
				num = item.Delay;
			}
		}
		return num;
	}

	public int GetLowDelayTurn()
	{
		List<Living> list = new List<Living>();
		int num = int.MaxValue;
		foreach (TurnedLiving item in m_turnQueue)
		{
			if (item != null && item.Delay < num)
			{
				num = item.Delay;
			}
		}
		return num;
	}

	public bool AreaPK()
	{
		eRoomType roomType = base.RoomType;
		eRoomType eRoomType2 = roomType;
		if (eRoomType2 == eRoomType.RingStation || (uint)(eRoomType2 - 41) <= 1u)
		{
			return true;
		}
		return false;
	}

	public bool ArenaBoss()
	{
		eRoomType roomType = base.RoomType;
		eRoomType eRoomType2 = roomType;
		if (eRoomType2 == eRoomType.FarmBoss || eRoomType2 == eRoomType.Cryptboss)
		{
			return true;
		}
		return false;
	}

	public float GetNextWind()
	{
		int num = (int)(Wind * 10f);
		int num2 = 0;
		if (num > m_nextWind)
		{
			num2 = num - m_random.Next(11);
			if (num <= m_nextWind)
			{
				m_nextWind = m_random.Next(-40, 40);
			}
		}
		else
		{
			num2 = num + m_random.Next(11);
			if (num >= m_nextWind)
			{
				m_nextWind = m_random.Next(-40, 40);
			}
		}
		return (float)num2 / 10f;
	}

	public float GetNextWindRate()
	{
		int num = (int)(WindRate * 10f);
		int num2 = 0;
		if (num > m_nextWindRate)
		{
			num2 = num - m_random.Next(11);
			if (num <= m_nextWindRate)
			{
				m_nextWindRate = m_random.Next(-40, 40);
			}
		}
		else
		{
			num2 = num + m_random.Next(11);
			if (num >= m_nextWindRate)
			{
				m_nextWindRate = m_random.Next(-40, 40);
			}
		}
		return ((float)num2 / 10f == 0f) ? 1f : ((float)num2 / 10f);
	}

	public void UpdateWind(float wind, bool sendToClient)
	{
		if (m_map.wind != wind)
		{
			m_map.wind = wind;
			if (sendToClient)
			{
				SendGameUpdateWind(wind);
			}
		}
	}

	public void UpdateWind(float wind, float windRate, bool sendToClient)
	{
		if (m_map.wind != wind)
		{
			m_map.wind = wind;
			if (base.RoomType == eRoomType.PolarRegion)
			{
				m_map.windRate = windRate;
			}
			if (sendToClient)
			{
				SendGameUpdateWind(wind);
			}
		}
	}

	public void TakePassBall(int livvingId)
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item != null)
			{
				item.SyncAtTime = true;
				if (item.Id == livvingId)
				{
					item.HoldingBall(state: true);
				}
				else
				{
					item.HoldingBall(state: false);
				}
			}
		}
	}

	public int GetDiedPlayerCount()
	{
		int num = 0;
		foreach (Player value in m_players.Values)
		{
			if (!value.IsLiving)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDiedNPCCount()
	{
		int num = 0;
		SimpleNpc[] array = FindAllNpc();
		foreach (SimpleNpc simpleNpc in array)
		{
			if (!simpleNpc.IsLiving)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDiedCount()
	{
		return GetDiedNPCCount() + GetDiedBossCount();
	}

	public int GetDiedBossCount()
	{
		int num = 0;
		SimpleBoss[] array = FindAllBoss();
		foreach (SimpleBoss simpleBoss in array)
		{
			if (!simpleBoss.IsLiving)
			{
				num++;
			}
		}
		return num;
	}

	public Point GetPlayerPoint(int team)
	{
		MapPoint pVEMapRandomPos = MapMgr.GetPVEMapRandomPos(m_map.Info.ID);
		return GetPlayerPoint(pVEMapRandomPos, team);
	}

	protected Point GetPlayerPoint(MapPoint mapPos, int team)
	{
		List<Point> list = mapPos.PosX;
		switch (team)
		{
		case 2:
		case 4:
		case 6:
			list = mapPos.PosX1;
			break;
		}
		int index = m_random.Next(list.Count);
		Point point = list[index];
		list.Remove(point);
		return point;
	}

	public virtual void CheckState(int delay)
	{
	}

	public override void ProcessData(GSPacketIn packet)
	{
		if (m_players.ContainsKey(packet.Parameter1))
		{
			Player player = m_players[packet.Parameter1];
			AddAction(new ProcessPacketAction(player, packet));
		}
	}

	public Player GetPlayerByIndex(int index)
	{
		return m_players.ElementAt(index).Value;
	}

	public Player FindNearestPlayer(int x, int y)
	{
		double num = double.MaxValue;
		Player result = null;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				double num2 = value.Distance(x, y);
				if (num2 < num)
				{
					num = num2;
					result = value;
				}
			}
		}
		return result;
	}

	public List<Living> FindHoldBall()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.Config.IsHelper)
			{
				list.Add(living);
			}
		}
		foreach (Player value in m_players.Values)
		{
			if (value.Config.IsHelper || value.IsHoldBall)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public Player FindRandomPlayer()
	{
		List<Player> list = new List<Player>();
		Player result = null;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				list.Add(value);
			}
		}
		if (list.Count > 0)
		{
			int index = Random.Next(0, list.Count);
			result = list[index];
		}
		return result;
	}

	public Player FindPlayerByBlood()
	{
		List<Player> list = new List<Player>();
		List<int> list2 = new List<int>();
		Player result = null;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				list.Add(value);
				list2.Add(value.Blood);
			}
		}
		if (list.Count > 0)
		{
			foreach (Player item in list)
			{
				if (item.Blood == list2.Max())
				{
					result = item;
				}
			}
		}
		return result;
	}

	public SimpleNpc FindHelper()
	{
		SimpleNpc result = null;
		foreach (SimpleNpc living in m_livings)
		{
			if (living.Config.IsHelper)
			{
				result = living;
			}
		}
		return result;
	}

	public List<Living> FindLivingByID(int ID)
	{
		List<Living> list = new List<Living>();
		foreach (SimpleNpc living in m_livings)
		{
			if (living.NpcInfo.ID == ID)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public Living FindRandomLiving()
	{
		List<Living> list = new List<Living>();
		Living result = null;
		foreach (Living living in m_livings)
		{
			if (living.IsLiving)
			{
				list.Add(living);
			}
		}
		if (list.Count > 0)
		{
			int index = Random.Next(0, list.Count);
			result = list[index];
		}
		return result;
	}

	public int FindlivingbyDir(Living npc)
	{
		int num = 0;
		int num2 = 0;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				if (value.X > npc.X)
				{
					num2++;
				}
				else
				{
					num++;
				}
			}
		}
		if (num2 > num)
		{
			return 1;
		}
		if (num2 < num)
		{
			return -1;
		}
		return -npc.Direction;
	}

	public Living FindNearestHelper(int x, int y)
	{
		double num = double.MaxValue;
		Living result = null;
		foreach (TurnedLiving item in m_turnQueue)
		{
			if (item.IsLiving && (item is Player || item.Config.IsHelper))
			{
				double num2 = item.Distance(x, y);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	public PhysicalObj[] FindPhysicalObjByName(string name)
	{
		List<PhysicalObj> list = new List<PhysicalObj>();
		foreach (PhysicalObj item in m_map.GetAllPhysicalObjSafe())
		{
			if (item.Name == name)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public PhysicalObj[] FindPhysicalObjByName(string name, bool CanPenetrate)
	{
		List<PhysicalObj> list = new List<PhysicalObj>();
		foreach (PhysicalObj item in m_map.GetAllPhysicalObjSafe())
		{
			if (item.Name == name && item.CanPenetrate == CanPenetrate)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public Player GetFrostPlayerRadom()
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		List<Player> list = new List<Player>();
		foreach (Player item in allFightPlayers)
		{
			if (item.IsFrost)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			int index = Random.Next(0, list.Count);
			return list.ElementAt(index);
		}
		return null;
	}

	public virtual bool TakeCard(Player player)
	{
		return false;
	}

	public virtual bool TakeCard(Player player, int index, bool isAuto)
	{
		return false;
	}

	public override void Pause(int time)
	{
		m_passTick = Math.Max(m_passTick, TickHelper.GetTickCount() + time);
	}

	public override void Resume()
	{
		m_passTick = 0L;
	}

	public void AddAction(IAction action)
	{
		lock (m_actions)
		{
			m_actions.Add(action);
		}
	}

	public void AddAction(ArrayList actions)
	{
		lock (m_actions)
		{
			m_actions.AddRange(actions);
		}
	}

	public void ClearWaitTimer()
	{
		m_waitTimer = 0L;
	}

	public void WaitTime(int delay)
	{
		m_waitTimer = Math.Max(m_waitTimer, TickHelper.GetTickCount() + delay);
		m_lastWaitTimer = m_waitTimer;
	}

	public long GetWaitTimer()
	{
		return m_waitTimer;
	}

	public int GetWaitTimerLeft()
	{
		if (m_lastWaitTimer <= 0)
		{
			return 0;
		}
		long num = ((TickHelper.GetTickCount() > m_lastWaitTimer) ? (TickHelper.GetTickCount() - m_lastWaitTimer) : (m_lastWaitTimer - TickHelper.GetTickCount()));
		if (num > 10000)
		{
			return 1000;
		}
		return (int)num;
	}

	public void Update(long tick)
	{
		if (m_passTick >= tick)
		{
			return;
		}
		m_lifeTime++;
		if (GameState == eGameState.Stopped)
		{
			return;
		}
		ArrayList arrayList;
		lock (m_actions)
		{
			arrayList = (ArrayList)m_actions.Clone();
			m_actions.Clear();
		}
		CurrentActionCount = arrayList.Count;
		if (arrayList.Count > 0)
		{
			ArrayList arrayList2 = new ArrayList();
			foreach (IAction item in arrayList)
			{
				try
				{
					item.Execute(this, tick);
					if (!item.IsFinished(tick))
					{
						arrayList2.Add(item);
					}
				}
				catch (Exception exception)
				{
					log.Error("Map update error:", exception);
				}
			}
			AddAction(arrayList2);
		}
		else if (m_waitTimer < tick)
		{
			CheckState(0);
		}
	}

	public string ListPlayersName()
	{
		List<string> list = new List<string>();
		List<Player> allLivingPlayers = GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			list.Add(item.PlayerDetail.PlayerCharacter.NickName);
		}
		return string.Join(",", list);
	}

	public List<Player> FindRangePlayers(int minX, int maxX)
	{
		lock (m_players)
		{
			List<Player> list = new List<Player>();
			foreach (Player value in m_players.Values)
			{
				if (value.IsLiving && value.X >= minX && value.X <= maxX)
				{
					list.Add(value);
				}
			}
			return list;
		}
	}

	public Player FindPlayerWithId(int id)
	{
		lock (m_players)
		{
			if (m_players.Count > 0)
			{
				foreach (Player value in m_players.Values)
				{
					if (value.IsLiving && value.Id == id)
					{
						return value;
					}
				}
			}
			return null;
		}
	}

	public Player[] FindRandomPlayer(int max)
	{
		List<Player> list = new List<Player>();
		if (m_players.Count > 0)
		{
			List<Player> list2 = new List<Player>();
			foreach (Player value in m_players.Values)
			{
				if (value.IsLiving)
				{
					list2.Add(value);
				}
			}
			for (int i = 0; i < max; i++)
			{
				int index = Random.Next(0, list2.Count);
				list.Add(list2[index]);
				list2.RemoveAt(index);
				if (list2.Count <= 0)
				{
					break;
				}
			}
		}
		return list.ToArray();
	}

	public List<Player> GetAllFightPlayers()
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			list.AddRange(m_players.Values);
		}
		return list;
	}

	public List<Player> GetAllFightingPlayers()
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (!value.PlayerDetail.PlayerCharacter.isViewer)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllTeamPlayers(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.Team == living.Team)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllPlayersSameTeam(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.Team == living.Team && value != living)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllTeamPlayerDies(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.Team == living.Team && !value.IsLiving)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllEnemyPlayers(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.Team != living.Team)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllEnemys(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_turnQueue)
		{
			foreach (Player item in m_turnQueue)
			{
				if (item.Team != living.Team)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllBoss(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (living is TurnedLiving)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllTarget(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.Team != living.Team)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllLivingPlayers()
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.IsLiving)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public bool GetSameTeam()
	{
		bool result = false;
		Player[] allPlayers = GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player.Team == allPlayers[0].Team)
			{
				result = true;
				continue;
			}
			result = false;
			break;
		}
		return result;
	}

	public Player[] GetAllPlayers()
	{
		return GetAllFightPlayers().ToArray();
	}

	public Player GetPlayer(IGamePlayer gp)
	{
		Player result = null;
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.PlayerDetail == gp)
				{
					result = value;
					break;
				}
			}
		}
		return result;
	}

	public int GetPlayerCount()
	{
		return GetAllFightPlayers().Count;
	}

	public virtual void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
	{
		if (pkg.Parameter2 == 0)
		{
			pkg.Parameter2 = LifeTime;
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.PlayerDetail != except)
			{
				item.PlayerDetail.SendTCP(pkg);
			}
		}
	}

	public virtual void SendToTeam(GSPacketIn pkg, int team)
	{
		SendToTeam(pkg, team, null);
	}

	public virtual void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.PlayerDetail != except && item.Team == team)
			{
				item.PlayerDetail.SendTCP(pkg);
			}
		}
	}

	public Ball AddBall(Point pos, bool sendToClient)
	{
		Ball ball = new Ball(PhysicalId++, "", "1", 1, 1);
		ball.SetXY(pos);
		AddPhysicalObj(ball, sendToClient);
		return AddBall(ball, sendToClient);
	}

	public Ball AddBall(Ball ball, bool sendToClient)
	{
		m_tempBall.Add(ball);
		AddPhysicalObj(ball, sendToClient);
		return ball;
	}

	public void ClearBall()
	{
		List<Ball> list = new List<Ball>();
		foreach (Ball item in m_tempBall)
		{
			list.Add(item);
		}
		foreach (Ball item2 in list)
		{
			m_tempBall.Remove(item2);
			RemovePhysicalObj(item2, sendToClient: true);
		}
	}

	public void AddTempPoint(int x, int y)
	{
		m_tempPoints.Add(new Point(x, y));
	}

	public SimpleBox AddBox(ItemInfo item, Point pos, bool sendToClient)
	{
		SimpleBox simpleBox = new SimpleBox(PhysicalId++, "1", item, 1);
		simpleBox.SetXY(pos);
		return AddBox(simpleBox, sendToClient);
	}

	public SimpleBox AddBox(SimpleBox box, bool sendToClient)
	{
		m_tempBox.Add(box);
		AddPhysicalObj(box, sendToClient);
		return box;
	}

	public void AddTempGhostPoint(int x, int y)
	{
		m_tempGhostPoints.Add(new Point(x, y));
	}

	public SimpleBox AddGhostBox(Point pos, int type)
	{
		SimpleBox simpleBox = new SimpleBox(PhysicalId++, "1", null, type);
		simpleBox.SetXY(pos);
		return AddGhostBox(simpleBox);
	}

	public SimpleBox AddGhostBox(SimpleBox box)
	{
		m_tempBox.Add(box);
		AddGhostBoxObj(box);
		return box;
	}

	public void CheckBox()
	{
		List<SimpleBox> list = new List<SimpleBox>();
		foreach (SimpleBox item in m_tempBox)
		{
			if (!item.IsLiving)
			{
				list.Add(item);
			}
		}
		foreach (SimpleBox item2 in list)
		{
			m_tempBox.Remove(item2);
			RemovePhysicalObj(item2, sendToClient: true);
		}
	}

	public int CheckGhostBox()
	{
		List<SimpleBox> list = new List<SimpleBox>();
		foreach (SimpleBox item in m_tempBox)
		{
			if (item.Type > 1)
			{
				list.Add(item);
			}
		}
		return list.Count();
	}

	public List<Point> DrawCirclePoints(int points, int dis, double radius, Point center)
	{
		List<Point> list = new List<Point>();
		double num = Math.PI * 2.0 / (double)points;
		for (double num2 = radius; num2 > (double)dis; num2 -= (double)dis)
		{
			for (int i = 0; i < points; i++)
			{
				double num3 = num * (double)i;
				int x = (int)((double)center.X + num2 * Math.Cos(num3));
				int y = (int)((double)center.Y + num2 * Math.Sin(num3));
				Point item = new Point(x, y);
				list.Add(item);
			}
		}
		return list;
	}

	public void CreateGhostPoints()
	{
		int backroundHeight = m_map.Info.BackroundHeight;
		int backroundWidht = m_map.Info.BackroundWidht;
		Point center = new Point(backroundWidht / 2, backroundHeight / 2);
		int num = 180;
		m_tempGhostPoints = DrawCirclePoints(backroundHeight, 30, backroundHeight - num, center);
	}

	public List<SimpleBox> CreateBox()
	{
		int num = m_players.Count + 2;
		int num2 = 0;
		List<ItemInfo> info = null;
		if (CurrentTurnTotalDamage > 0)
		{
			num2 = m_random.Next(1, 3);
			if (m_tempBox.Count + num2 > num)
			{
				num2 = num - m_tempBox.Count;
			}
			if (num2 > 0)
			{
				DropInventory.BoxDrop(m_roomType, ref info);
			}
		}
		int diedPlayerCount = GetDiedPlayerCount();
		int num3 = 0;
		List<SimpleBox> list = new List<SimpleBox>();
		if (diedPlayerCount > 0 && base.RoomType != eRoomType.SurvivalMode)
		{
			num3 = m_random.Next(diedPlayerCount);
			if (m_tempGhostPoints.Count < num)
			{
				CreateGhostPoints();
			}
			for (int i = 0; i < m_tempGhostPoints.Count; i++)
			{
				int index = m_random.Next(m_tempGhostPoints.Count);
				Point value = m_tempGhostPoints[index];
				m_tempGhostPoints[index] = m_tempGhostPoints[i];
				m_tempGhostPoints[i] = value;
			}
			int num4 = diedPlayerCount + num - CheckGhostBox();
			if (m_tempGhostPoints.Count > num4)
			{
				int[] array = new int[2] { 2, 3 };
				for (int j = 0; j < num4; j++)
				{
					int num5 = m_random.Next(array.Length);
					int index2 = m_random.Next(m_tempGhostPoints.Count);
					list.Add(AddGhostBox(m_tempGhostPoints[index2], array[num5]));
				}
			}
		}
		if (m_tempBox.Count + num2 + num3 > num)
		{
			num3 = num - m_tempBox.Count - num2;
		}
		if (info != null)
		{
			for (int k = 0; k < m_tempPoints.Count; k++)
			{
				int index3 = m_random.Next(m_tempPoints.Count);
				Point value2 = m_tempPoints[index3];
				m_tempPoints[index3] = m_tempPoints[k];
				m_tempPoints[k] = value2;
			}
			int num6 = Math.Min(info.Count, m_tempPoints.Count);
			for (int l = 0; l < num6; l++)
			{
				list.Add(AddBox(info[l], m_tempPoints[l], sendToClient: false));
			}
		}
		m_tempPoints.Clear();
		m_tempGhostPoints.Clear();
		return list;
	}

	public void AddLoadingFile(int type, string file, string className)
	{
		if (file != null && className != null)
		{
			m_loadingFiles.Add(new LoadingFileInfo(type, file, className));
		}
	}

	public void ClearLoadingFiles()
	{
		m_loadingFiles.Clear();
	}

	public void AfterUseItem(ItemInfo item)
	{
	}

	internal void SendShadownPlayer(Player p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(157);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
		gSPacketIn.WriteInt(p.X);
		gSPacketIn.WriteInt(p.Y);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteByte(0);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
		SendToAll(gSPacketIn);
	}

    //__createGame createGameInfo
    internal void SendCreateGame()
	{
		bool flag = m_roomType == eRoomType.FightFootballTime;
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		if (base.isSingleBatle)
		{
			gSPacketIn.WriteByte(74);
		}
		else
		{
			gSPacketIn.WriteByte(101);
		}
		gSPacketIn.WriteInt((int)m_roomType);
		gSPacketIn.WriteInt((int)m_gameType);
		gSPacketIn.WriteInt(m_timeType);
		List<Player> allFightPlayers = GetAllFightPlayers();
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			IGamePlayer playerDetail = item.PlayerDetail;
			gSPacketIn.WriteInt(playerDetail.ZoneId); //_loc11_ = param1.readInt();
            gSPacketIn.WriteString(playerDetail.ZoneName); //_loc33_ = param1.readUTF();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ID); // _loc13_ = param1.readInt();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.NickName); //_loc15_ = param1.readUTF();
            gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.isViewer);
			gSPacketIn.WriteByte(playerDetail.PlayerCharacter.typeVIP); //_loc24_.typeVIP = param1.readByte();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.VIPLevel); // _loc24_.VIPLevel = param1.readInt();
            gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.Sex); //_loc24_.Sex = param1.readBoolean();
            int num = playerDetail.PlayerCharacter.TempHideCategory(4, hide: true);
			if (num == 0)
			{
				num = playerDetail.PlayerCharacter.Hide;
			}
			gSPacketIn.WriteInt(num); //_loc24_.Hide = param1.readInt();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.Style); //_loc24_.Style = param1.readUTF();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.Colors); //_loc24_.Colors = param1.readUTF();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.Skin); //_loc24_.Skin = param1.readUTF();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Grade); //_loc24_.Grade = param1.readInt();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Repute); //_loc24_.Repute = param1.readInt();
            if (flag)
			{
				gSPacketIn.WriteInt(70396);
			}
			else if (playerDetail.MainWeapon.GoldValidDate())
			{
				gSPacketIn.WriteInt(playerDetail.MainWeapon.GoldEquip.TemplateID); //_loc24_.WeaponID = param1.readInt();
            }
			else
			{
				gSPacketIn.WriteInt(playerDetail.MainWeapon.TemplateID); //_loc24_.WeaponID = param1.readInt();
            }
			gSPacketIn.WriteInt(playerDetail.MainWeapon.RefineryLevel); //_loc39_ = param1.readInt();
            gSPacketIn.WriteString(playerDetail.MainWeapon.Template.Pic); // _loc28_ = param1.readUTF();
            gSPacketIn.WriteDateTime(DateTime.Now); //_loc9_ = param1.readDateString();
            gSPacketIn.WriteInt((playerDetail.SecondWeapon != null) ? playerDetail.SecondWeapon.TemplateID : 0); //_loc24_.DeputyWeaponID = param1.readInt();
            gSPacketIn.WriteInt((playerDetail.PlayerCharacter.badgePvpid <= 50505) ? playerDetail.PlayerCharacter.badgePvpid : 0); //_loc24_.pvpBadgeId = param1.readInt();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Nimbus); //_loc24_.Nimbus = param1.readInt();
            gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.IsShowConsortia); //_loc8_ = param1.readBoolean();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ConsortiaID); //_loc24_.ConsortiaID = param1.readInt();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.ConsortiaName); //_loc24_.ConsortiaName = param1.readUTF();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.badgeID); //_loc24_.badgeID = param1.readInt();
            gSPacketIn.WriteInt(0); //_loc18_ = param1.readInt();
			gSPacketIn.WriteInt(0); //_loc14_ = param1.readInt();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Win); // _loc24_.WinCount = param1.readInt();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Total); //_loc24_.TotalCount = param1.readInt();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.FightPower); //_loc24_.FightPower = param1.readInt();
            if (GameProperties.VERSION >= 12100)
            {
                gSPacketIn.WriteLong(0L); //_loc24_.FightPower_Show = param1.readLong();
            }
			else
			{
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteInt(0);
                gSPacketIn.WriteString("");
            }	
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.AcademyInfo.ApprenticeshipState); //_loc24_.apprenticeshipState = param1.readInt();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.AcademyInfo.MasterID); //loc24_.masterID = param1.readInt();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.AcademyInfo.MasterOrApprentices); //_loc24_.setMasterOrApprentices(param1.readUTF());
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.AchievementPoint);  //_loc24_.AchievementPoint = param1.readInt();
            gSPacketIn.WriteString(playerDetail.PlayerCharacter.Honor); //_loc24_.honor = param1.readUTF();
            gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Offer); //_loc24_.Offer = param1.readInt();
            gSPacketIn.WriteBoolean(val: false); //_loc24_.DailyLeagueFirst = param1.readBoolean();
            gSPacketIn.WriteInt(0); //_loc24_.DailyLeagueLastScore = param1.readInt();
            if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt(GuardCoreEnable() ? playerDetail.PlayerCharacter.guardCoreID : 0); // _loc24_.guardCoreID = param1.readInt();
            }
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.IsMarried); //_loc21_.playerInfo.IsMarried = param1.readBoolean();
            if (playerDetail.PlayerCharacter.IsMarried)
			{
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.SpouseID); // _loc21_.playerInfo.SpouseID = param1.readInt();
                gSPacketIn.WriteString(playerDetail.PlayerCharacter.SpouseName); //_loc21_.playerInfo.SpouseName = param1.readUTF();
            }
			gSPacketIn.WriteInt(0); //param1.readInt();
			gSPacketIn.WriteInt(0); //param1.readInt();
			gSPacketIn.WriteInt(0); //param1.readInt();
			gSPacketIn.WriteInt(0); //param1.readInt();
			gSPacketIn.WriteInt(0); //param1.readInt();
			gSPacketIn.WriteInt(0); //param1.readInt();
            if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt((playerDetail.Weapon3D != null) ? playerDetail.Weapon3D.TemplateID : 0); //_loc3_ = param1.readInt();
            }
			gSPacketIn.WriteInt(item.Team); //_loc21_.team = param1.readInt();
            gSPacketIn.WriteInt(item.Id); //_loc19_ = int(param1.readInt());
            gSPacketIn.WriteInt(item.MaxBlood); //_loc29_ = param1.readInt()
            if (item.Pet == null || flag)
			{
				gSPacketIn.WriteInt(0); //_loc5_ = param1.readInt();
            }
			else
			{
				gSPacketIn.WriteInt(1); //_loc5_ = param1.readInt();
                gSPacketIn.WriteInt(item.Pet.Place); //_loc17_ = param1.readInt();
                gSPacketIn.WriteInt(item.Pet.TemplateID); //_loc36_ = param1.readInt();
                gSPacketIn.WriteInt(item.Pet.ID); //_loc42_.ID = param1.readInt();
                gSPacketIn.WriteString(item.Pet.Name); //_loc42_.Name = param1.readUTF();
                gSPacketIn.WriteInt(item.Pet.UserID); //_loc42_.UserID = param1.readInt();
                gSPacketIn.WriteInt(item.Pet.Level); // _loc42_.Level = param1.readInt();
                string[] array = item.Pet.SkillEquip.Split('|');
				gSPacketIn.WriteInt(array.Length); //_loc12_ = param1.readInt();
                string[] array2 = array;
				foreach (string text in array2)
				{
					gSPacketIn.WriteInt(int.Parse(text.Split(',')[1])); //_loc38_ = param1.readInt();
                    gSPacketIn.WriteInt(int.Parse(text.Split(',')[0])); //_loc22_ = param1.readInt();
                }
			}
			gSPacketIn.WriteInt(item.HorseSkillEquip.Length); //_loc10_ = param1.readInt();
            int[] horseSkillEquip = item.HorseSkillEquip;
			foreach (int val in horseSkillEquip)
			{
				gSPacketIn.WriteInt(val); //_loc21_.horseSkillEquipList.push(param1.readInt());
            }
			gSPacketIn.WriteInt(0); //_loc41_ = param1.readInt();
            gSPacketIn.WriteBoolean(item.PlayerDetail.PlayerCharacter.IsAutoBot); //_loc4_.IsRobot = param1.readBoolean();
            if (GameProperties.VERSION >= 11000)
			{
				if (item.PlayerDetail.PlayerCharacter.IsHideElf)
				{
					gSPacketIn.WriteInt(0); //_loc24_.peakPetId = param1.readInt();
                }
				else
				{
					gSPacketIn.WriteInt((item.PlayerDetail.PlayerCharacter?.ElfInfo?.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.TempID).GetValueOrDefault()); //_loc24_.peakPetId = param1.readInt();
                }
				gSPacketIn.WriteString(item.PlayerDetail.PlayerCharacter?.ElfInfo?.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.SkillList ?? ""); //_loc24_.peakPetSkill = param1.readUTF();
                gSPacketIn.WriteString(item.Pet?.SkillEquip.Replace('|', ',') ?? ""); //_loc16_ = param1.readUTF();
            }
			if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteBoolean(item.PlayerDetail.PlayerCharacter.ringFlag); //_loc4_.ringFlag = param1.readBoolean();
                gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.loveBuffLevel); //_loc4_.loveBuffLevel = param1.readInt();
                gSPacketIn.WriteInt(item.TurnNum); //_loc4_.turnCount = param1.readInt();
            }
			if (GameProperties.VERSION >= 8800)
			{
				gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Team.teamID); //_loc24_.teamID = param1.readInt();
                gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Team.teamDivision); //_loc24_.teamDivision = param1.readInt();
                if (GameProperties.VERSION >= 12100)
				{
                    gSPacketIn.WriteString(item.PlayerDetail.PlayerCharacter.Team.teamName); //_loc24_.teamName = param1.readUTF();
                    gSPacketIn.WriteString(item.PlayerDetail.PlayerCharacter.Team.teamTag); //_loc24_.teamTag = param1.readUTF();
                }
				else
				{
                    gSPacketIn.WriteString(item.PlayerDetail.PlayerCharacter.Team.teamTag);
                    gSPacketIn.WriteString(item.PlayerDetail.PlayerCharacter.Team.teamName);
                }
                gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Team.teamGrade); //_loc24_.teamGrade = param1.readInt();
                gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Team.teamWinTime); //_loc24_.teamWinTime = param1.readInt();
                gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Team.teamTotalTime); //_loc24_.teamTotalTime = param1.readInt();
                gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Team.teamPersonalScore); //_loc24_.teamPersonalScore = param1.readInt();
            }
			if (GameProperties.VERSION < 11000)
			{
				continue;
			}
			if (item.PlayerDetail.PlayerCharacter.IsHideElf)
			{
				gSPacketIn.WriteInt(0); //_loc24_.elfTempId = param1.readInt();
                continue;
			}
			gSPacketIn.WriteInt((item.PlayerDetail.PlayerCharacter?.ElfInfo?.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.TempID).GetValueOrDefault()); //_loc24_.elfTempId = param1.readInt();
        }
		if (GameProperties.VERSION >= 7600 && !base.isSingleBatle)
		{
			gSPacketIn.WriteBoolean(GuardCoreEnable());
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString("0,SMALLMAP_BORDER_ENABLE,SMALLMAP_ALPHA,MALLMAP_POINT_ENABLE,SMALLMAP_GRID_ENABLE,SMALLMAP_SHAPE_ENABLE");
			gSPacketIn.WriteInt(0);
		}
		SendToAll(gSPacketIn);
	}

	public bool GuardCoreEnable()
	{
		return base.RoomType == eRoomType.Match || base.RoomType == eRoomType.Freedom;
	}

	internal void SendLockFocus(bool IsLock)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(69);
		gSPacketIn.WriteBoolean(IsLock);
		SendToAll(gSPacketIn);
	}

	internal void SendOpenSelectLeaderWindow(int maxTime)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(102);
		gSPacketIn.WriteInt(maxTime);
		SendToAll(gSPacketIn);
	}

	internal void SendSelectObject(int playerId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, playerId);
		gSPacketIn.Parameter1 = playerId;
		gSPacketIn.WriteByte(138);
		int count = Players.Count;
		gSPacketIn.WriteInt(count);
		for (int i = 0; i < count; i++)
		{
			gSPacketIn.WriteInt(Players[i].PlayerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteInt(Players[i].PlayerDetail.ZoneId);
			gSPacketIn.WriteInt(Players[i].Team);
			gSPacketIn.WriteInt(Players[i].Id);
			gSPacketIn.WriteInt(Players[i].PlayerDetail.ZoneId);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendMarkMeHideInfo(Player player, int targetId, bool isMark)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(153);
		gSPacketIn.WriteInt(targetId);
		gSPacketIn.WriteBoolean(isMark);
		if (player.Id == targetId)
		{
			SendToAll(gSPacketIn);
		}
		else
		{
			player.PlayerDetail.SendTCP(gSPacketIn);
		}
	}

	internal void SendSkipNext(Player player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(12);
		player.PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendPetSkillCd(Living player, int skillInfoId, int coldDown)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(147);
		gSPacketIn.WriteInt(skillInfoId);
		gSPacketIn.WriteInt(coldDown);
		((Player)player).PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendFightStatus(Living player, int status)
	{
		if (base.RoomType == eRoomType.ActivityDungeon)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(76);
			gSPacketIn.WriteInt(status);
			SendToAll(gSPacketIn);
		}
	}

	internal void SendIsLastMission(bool isLast)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(160);
		gSPacketIn.WriteBoolean(isLast);
		SendToAll(gSPacketIn);
	}

	internal void SendMissionTryAgain(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(119);
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal bool isTrainer()
	{
		return base.RoomType == eRoomType.Freshman;
	}

	internal void SendStartLoading(int maxTime)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(103);
		gSPacketIn.WriteInt(maxTime);
		gSPacketIn.WriteInt(m_map.Info.ID);
		gSPacketIn.WriteInt(m_loadingFiles.Count);
		foreach (LoadingFileInfo loadingFile in m_loadingFiles)
		{
			gSPacketIn.WriteInt(loadingFile.Type);
			gSPacketIn.WriteString(loadingFile.Path);
			gSPacketIn.WriteString(loadingFile.ClassName);
		}
		if (isTrainer() || base.RoomType == eRoomType.FightFootballTime)
		{
			gSPacketIn.WriteInt(0);
		}
		else
		{
			GameNeedPetSkillInfo[] gameNeedPetSkill = PetMgr.GetGameNeedPetSkill();
			gSPacketIn.WriteInt(gameNeedPetSkill.Length);
			GameNeedPetSkillInfo[] array = gameNeedPetSkill;
			foreach (GameNeedPetSkillInfo gameNeedPetSkillInfo in array)
			{
				gSPacketIn.WriteString(gameNeedPetSkillInfo.Pic.ToString());
				gSPacketIn.WriteString(gameNeedPetSkillInfo.EffectPic);
			}
		}
		SendToAll(gSPacketIn);
	}

	internal void SendSkillInfoInit(Player player, Dictionary<int, PetSkillInfo> petSkillList, Dictionary<int, MountSkillTemplateInfo> horseSkillList)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(150);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(horseSkillList.Count);
		foreach (MountSkillTemplateInfo value in horseSkillList.Values)
		{
			gSPacketIn.WriteInt(value.ID);
			gSPacketIn.WriteInt(value.Turn);
			gSPacketIn.WriteInt(value.Count);
		}
		player.PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendRoundOneEnd(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(4);
		((Player)player).PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendAddPhysicalObj(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(48);
		gSPacketIn.WriteInt(obj.Id);
		gSPacketIn.WriteInt(obj.Type);
		gSPacketIn.WriteInt(obj.X);
		gSPacketIn.WriteInt(obj.Y);
		if (GameProperties.VERSION >= 11000)
		{
			gSPacketIn.WriteString(obj.Name);
		}
		gSPacketIn.WriteString(obj.Model);
		gSPacketIn.WriteString(obj.CurrentAction);
		gSPacketIn.WriteInt(obj.Scale);
		gSPacketIn.WriteInt(obj.Scale);
		gSPacketIn.WriteInt(obj.Rotation);
		gSPacketIn.WriteInt(obj.phyBringToFront);
		gSPacketIn.WriteInt(obj.typeEffect);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(obj.ActionMapping.Count);
		foreach (string key in obj.ActionMapping.Keys)
		{
			gSPacketIn.WriteString(key);
			gSPacketIn.WriteString(obj.ActionMapping[key]);
		}
		gSPacketIn.WriteInt(obj.IsCollide);
		SendToAll(gSPacketIn);
	}

	internal void SendAddPhysicalTip(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(68);
		gSPacketIn.WriteInt(obj.Id);
		gSPacketIn.WriteInt(obj.Type);
		gSPacketIn.WriteInt(obj.X);
		gSPacketIn.WriteInt(obj.Y);
		gSPacketIn.WriteString(obj.Model);
		gSPacketIn.WriteString(obj.CurrentAction);
		gSPacketIn.WriteInt(obj.Scale);
		gSPacketIn.WriteInt(obj.Rotation);
		SendToAll(gSPacketIn);
	}

	internal void SendPhysicalObjFocus(Physics obj, int type)
	{
		SendPhysicalObjFocus(obj.X, obj.Y, type);
	}

	internal void SendPhysicalObjFocus(int x, int y, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(62);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		SendToAll(gSPacketIn);
	}

	internal void SendPhysicalObjPlayAction(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(66);
		gSPacketIn.WriteInt(obj.Id);
		gSPacketIn.WriteString(obj.CurrentAction);
		SendToAll(gSPacketIn);
	}

	internal void SendRemovePhysicalObj(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(53);
		gSPacketIn.WriteInt(obj.Id);
		SendToAll(gSPacketIn);
	}

	internal void SendRemoveLiving(int id)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(53);
		gSPacketIn.WriteInt(id);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingBoltMove(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(72);
		gSPacketIn.WriteInt(living.X);
		gSPacketIn.WriteInt(living.Y);
		SendToAll(gSPacketIn);
	}

	internal GSPacketIn SendAddLivingPkg(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(64);
		gSPacketIn.WriteByte((byte)living.Type);
		gSPacketIn.WriteInt(living.Id);
		gSPacketIn.WriteString(living.Name);
		gSPacketIn.WriteString(living.ModelId);
		gSPacketIn.WriteString(living.ActionStr);
		gSPacketIn.WriteInt(living.X);
		gSPacketIn.WriteInt(living.Y);
		gSPacketIn.WriteInt(living.Blood);
		gSPacketIn.WriteInt(living.MaxBlood);
		gSPacketIn.WriteInt(living.Team);
		gSPacketIn.WriteByte((byte)living.Direction);
		gSPacketIn.WriteByte(living.Config.isBotom);
		gSPacketIn.WriteBoolean(living.Config.isShowBlood);
		gSPacketIn.WriteBoolean(living.Config.isShowSmallMapPoint);
		if (GameProperties.VERSION >= 11000)
		{
			gSPacketIn.WriteBoolean(val: false);
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteBoolean(living.IsFrost);
		gSPacketIn.WriteBoolean(living.IsHide);
		gSPacketIn.WriteBoolean(living.IsNoHole);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteInt(0);
		if (base.RoomType == eRoomType.ActivityDungeon && living is SimpleBoss)
		{
			gSPacketIn.WriteInt((living as SimpleBoss).NpcInfo.ID);
		}
		return gSPacketIn;
	}

	internal void SendAddLiving(Living living)
	{
		GSPacketIn pkg = SendAddLivingPkg(living);
		SendToAll(pkg);
	}

	internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		gSPacketIn.WriteByte(dir);
		gSPacketIn.WriteBoolean(isLiving);
		if (type == 2)
		{
			gSPacketIn.WriteInt(m_tempBox.Count);
			foreach (SimpleBox item in m_tempBox)
			{
				gSPacketIn.WriteInt(item.X);
				gSPacketIn.WriteInt(item.Y);
			}
		}
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerMove(Player player, int type, int x, int y, byte dir)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.PlayerDetail.PlayerCharacter.ID);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		gSPacketIn.WriteByte(dir);
		gSPacketIn.WriteBoolean(player.IsLiving);
		if (type == 2)
		{
			gSPacketIn.WriteInt(m_tempBox.Count);
			foreach (SimpleBox item in m_tempBox)
			{
				gSPacketIn.WriteInt(item.X);
				gSPacketIn.WriteInt(item.Y);
			}
		}
		SendToAll(gSPacketIn, player.PlayerDetail);
	}

	internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed, string sAction)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(55);
		gSPacketIn.WriteInt(fromX);
		gSPacketIn.WriteInt(fromY);
		gSPacketIn.WriteInt(toX);
		gSPacketIn.WriteInt(toY);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteString(sAction);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingSay(Living living, string msg, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(59);
		gSPacketIn.WriteString(msg);
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingFall(Living living, int toX, int toY, int speed, string action, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(56);
		gSPacketIn.WriteInt(toX);
		gSPacketIn.WriteInt(toY);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingJump(Living living, int toX, int toY, int speed, string action, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(57);
		gSPacketIn.WriteInt(toX);
		gSPacketIn.WriteInt(toY);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingBeat(Living living, Living target, int totalDemageAmount, string action, int livingCount, int attackEffect, string deadEffect, int type)
	{
		int val = 0;
		if (target is Player)
		{
			Player player = target as Player;
			val = player.Dander;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(58);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		if (GameProperties.VERSION >= 7700)
		{
			gSPacketIn.WriteString(deadEffect);
		}
		gSPacketIn.WriteInt(livingCount);
		for (int i = 1; i <= livingCount; i++)
		{
			gSPacketIn.WriteInt(target.Id);
			gSPacketIn.WriteInt(totalDemageAmount);
			gSPacketIn.WriteInt(target.Blood);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(attackEffect);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(type);
			}
			if (GameProperties.VERSION >= 11000)
			{
				gSPacketIn.WriteInt(0);
			}
		}
		SendToAll(gSPacketIn);
	}

	internal void SendLivingPlayMovie(Living living, string action)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(60);
		gSPacketIn.WriteString(action);
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerInfoInGame(Player living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter2 = LifeTime;
		gSPacketIn.WriteByte(120);
		gSPacketIn.WriteInt(living.PlayerDetail.ZoneId);
		gSPacketIn.WriteInt(living.PlayerDetail.PlayerCharacter.ID);
		gSPacketIn.WriteInt(living.Team);
		gSPacketIn.WriteInt(living.Id);
		gSPacketIn.WriteInt(living.MaxBlood);
		gSPacketIn.WriteBoolean(living.Ready);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateDander(TurnedLiving player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(14);
		gSPacketIn.WriteInt(player.Dander);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateMaxBlood(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(177);
		gSPacketIn.WriteInt(player.MaxBlood);
		gSPacketIn.WriteInt(player.Blood);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateFrozenState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(33);
		gSPacketIn.WriteBoolean(player.IsFrost);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateNoHoleState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(82);
		gSPacketIn.WriteBoolean(player.IsNoHole);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdatefireState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(37);
		gSPacketIn.WriteBoolean(player.Isfire);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateHideState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(35);
		gSPacketIn.WriteBoolean(player.IsHide);
		SendToAll(gSPacketIn);
	}

	internal void SendGamePlayerProperty(Living living, string type, string state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(41);
		gSPacketIn.WriteString(type);
		gSPacketIn.WriteString(state);
		SendToAll(gSPacketIn);
	}

	internal void SendCurrentPlayerProperty(Living living, string type, string state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(41);
		gSPacketIn.WriteString(type);
		gSPacketIn.WriteString(state);
		if (living is Player)
		{
			((Player)living).PlayerDetail.SendTCP(gSPacketIn);
		}
	}

	internal void SendLivingShowBlood(Player player, long blood, int x, int y)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(39);
		gSPacketIn.WriteInt(player.Id);
		gSPacketIn.WriteLong(blood);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		SendToAll(gSPacketIn);
	}

	public void SendLivingShowBlood(Living player, int isShow)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(80);
		gSPacketIn.WriteInt(player.Id);
		gSPacketIn.WriteInt(isShow);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingTurnRotation(Living player, int rotation, int speed, string endPlay)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(85);
		gSPacketIn.WriteInt(rotation);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString(endPlay);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateShootCount(Player player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(46);
		gSPacketIn.WriteByte((byte)player.ShootCount);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingActionMapping(int id, string source, string value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, id);
		gSPacketIn.Parameter1 = id;
		gSPacketIn.WriteByte(223);
		gSPacketIn.WriteInt(id);
		gSPacketIn.WriteString(source);
		gSPacketIn.WriteString(value);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateBall(Player player, bool Special)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(20);
		gSPacketIn.WriteBoolean(Special);
		gSPacketIn.WriteInt(player.CurrentBall.ID);
		SendToAll(gSPacketIn);
	}

	internal void SendGamePickBox(Living player, int index, int arkType, string goods)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(49);
		gSPacketIn.WriteByte((byte)index);
		gSPacketIn.WriteByte((byte)arkType);
		gSPacketIn.WriteString(goods);
		SendToAll(gSPacketIn);
	}

	internal void SendGameActionMapping(Living player, Ball ball)
	{
		string currentAction = ball.CurrentAction;
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(223);
		gSPacketIn.WriteInt(ball.Id);
		gSPacketIn.WriteString(currentAction);
		gSPacketIn.WriteString(ball.ActionMapping[currentAction]);
		SendToAll(gSPacketIn);
	}

	internal void SendGameBigBox(Living player, List<int> listTemplate)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(136);
		gSPacketIn.WriteInt(listTemplate.Count);
		foreach (int item in listTemplate)
		{
			gSPacketIn.WriteInt(item);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateWind(float wind)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(38);
		int num = (int)(wind * 10f);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(num > 0);
		gSPacketIn.WriteByte(GetVane(num, 1));
		gSPacketIn.WriteByte(GetVane(num, 2));
		gSPacketIn.WriteByte(GetVane(num, 3));
		gSPacketIn.WriteInt((int)Map.windRate);
		gSPacketIn.WriteInt((int)Map.windRate * 100);
		SendToAll(gSPacketIn);
	}

	public byte GetVane(int Wind, int param)
	{
		int wind = Math.Abs(Wind);
		return param switch
		{
			1 => WindMgr.GetWindID(wind, 1), 
			3 => WindMgr.GetWindID(wind, 3), 
			_ => 0, 
		};
	}

	public void VaneLoading(Player p)
	{
		List<WindInfo> wind = WindMgr.GetWind();
		foreach (WindInfo item in wind)
		{
			SendGameWindPic(p, (byte)item.WindID, item.WindPic);
		}
	}

	internal void SendGameWindPic(Player p, byte windId, byte[] windpic)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(241);
		gSPacketIn.WriteByte(windId);
		gSPacketIn.Write(windpic);
		if (p != null)
		{
			p.PlayerDetail.SendTCP(gSPacketIn);
		}
		else
		{
			SendToAll(gSPacketIn);
		}
	}

	internal void SendLivingRevive(Living living, int blood, int x, int y)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.WriteByte(39);
		gSPacketIn.WriteInt(living.Id);
		gSPacketIn.WriteLong(blood);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		SendToAll(gSPacketIn);
	}

	internal void SendPetUseKill(Player player, int type, int count)
	{
		SendPetUseKill(player, player.PetEffects.CurrentUseSkill, player.PetEffects.CurrentUseSkill != 0, type, count);
	}

	internal void SendPetUseKill(Player player, int skillId, bool isUse, int type, int count)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(144);
		gSPacketIn.WriteInt(skillId);
		gSPacketIn.WriteBoolean(isUse);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteInt(count);
		gSPacketIn.WriteByte(0);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingBoltmove(Player player, int x, int y)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(72);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		SendToAll(gSPacketIn);
	}

	internal void SendAddAnimation(Player player, int type, bool states)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(79);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(states);
		gSPacketIn.WriteInt(player.Id);
		if (type == 1)
		{
			gSPacketIn.WriteDateTime(DateTime.Now.AddMilliseconds(9000.0));
		}
		SendToAll(gSPacketIn);
	}

	internal void SendUseDeputyWeapon(Player player, int ResCount)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(84);
		gSPacketIn.WriteInt(ResCount);
		player.PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendPlayerUseProp(Player player, int type, int place, int templateID)
	{
		SendPlayerUseProp(player, type, place, templateID, player);
	}

	internal void SendPlayerUseProp(Living player, int type, int place, int templateID, Player p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(32);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(place);
		gSPacketIn.WriteInt(templateID);
		gSPacketIn.WriteInt(p.Id);
		gSPacketIn.WriteBoolean(templateID == 10017);
		SendToAll(gSPacketIn);
	}

	internal void SendGamePlayerTakeCard(Player player, bool isAuto, int index, int templateID, int count)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(98);
		gSPacketIn.WriteBoolean(isAuto);
		gSPacketIn.WriteByte((byte)index);
		gSPacketIn.WriteInt(templateID);
		gSPacketIn.WriteInt(count);
		gSPacketIn.WriteBoolean(val: false);
		SendToAll(gSPacketIn);
	}

	public int getTurnTime()
	{
		return m_timeType switch
		{
			1 => 8, 
			2 => 10, 
			3 => 12, 
			4 => 16, 
			5 => 21, 
			6 => 31, 
			_ => -1, 
		};
	}
    //syncMap()
    internal void SendGameNextTurn(Living living, BaseGame game, List<SimpleBox> newBoxes)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(6);
		if (GameProperties.VERSION >= 8100)
		{
			gSPacketIn.WriteInt(0);
			if (base.RoomType == eRoomType.HerroAuto)
			{
				gSPacketIn.WriteInt((int)m_map.wind);
			}
		}
		int num = (int)(m_map.wind * 10f);
		gSPacketIn.WriteBoolean(num > 0); //_loc11_:Boolean = _pkg.readBoolean();
        gSPacketIn.WriteByte(GetVane(num, 1)); //_loc5_:int = _pkg.readByte();
        gSPacketIn.WriteByte(GetVane(num, 2)); //_loc7_:int = _pkg.readByte();
        gSPacketIn.WriteByte(GetVane(num, 3)); //_loc6_:int = _pkg.readByte();
        gSPacketIn.WriteInt((int)Map.windRate); //_loc22_:int = _pkg.readInt();
        gSPacketIn.WriteInt((int)Map.windRate * 100); //_loc36_:Number = _pkg.readInt() / 100;
        gSPacketIn.WriteInt((int)Map.windRate); //_loc40_:int = _pkg.readInt();
        gSPacketIn.WriteBoolean(living.IsHide); //_info.isHidden = _pkg.readBoolean();
        gSPacketIn.WriteInt(getTurnTime()); //_loc17_:int = _pkg.readInt();
        gSPacketIn.WriteInt(newBoxes.Count); //_loc4_:int = _pkg.readInt();
        foreach (SimpleBox newBox in newBoxes)
		{
			gSPacketIn.WriteInt(newBox.Id);   //_loc14_ = _pkg.readInt();
            gSPacketIn.WriteInt(newBox.X);	  //_loc9_ = _pkg.readInt();
			gSPacketIn.WriteInt(newBox.Y);	  //_loc8_ = _pkg.readInt();
			gSPacketIn.WriteInt(newBox.Type); //_loc34_ = _pkg.readInt();
		}
		if (living is TurnedLiving)
		{
			List<Player> allFightPlayers = game.GetAllFightPlayers();
			gSPacketIn.WriteInt(allFightPlayers.Count); //_loc25_:int = _pkg.readInt();
            foreach (Player item in allFightPlayers)
			{
				gSPacketIn.WriteInt(item.Id); //_loc15_ = _pkg.readInt();
                gSPacketIn.WriteBoolean(item.IsLiving); //_loc10_ = _pkg.readBoolean();
                gSPacketIn.WriteInt(item.X); //_loc32_ = _pkg.readInt();
                gSPacketIn.WriteInt(item.Y); //_loc31_ = _pkg.readInt();
                gSPacketIn.WriteInt(item.Blood); //_loc24_ = _pkg.readInt();
                gSPacketIn.WriteBoolean(item.IsNoHole); //_loc18_ = _pkg.readBoolean();
                gSPacketIn.WriteInt(item.Energy); //_loc19_ = _pkg.readInt();
                gSPacketIn.WriteInt(item.psychic); //_loc13_ = _pkg.readInt();
                gSPacketIn.WriteInt(item.Dander); //_loc21_ = _pkg.readInt();
                if (item.Pet == null)
				{
					gSPacketIn.WriteInt(0); //_loc23_ = _pkg.readInt();
                    gSPacketIn.WriteInt(0); //_loc2_ = _pkg.readInt();
					if (GameProperties.VERSION >= 12100)
					{
                        gSPacketIn.WriteInt(0);
                    }
                }
				else
				{
					gSPacketIn.WriteInt(item.PetMaxMP); //_loc23_ = _pkg.readInt();
                    gSPacketIn.WriteInt(item.PetMP); //_loc2_ = _pkg.readInt();
                    if (GameProperties.VERSION >= 12100)
                    {
						//Can sua
                        gSPacketIn.WriteInt(0); //_loc20_ = _pkg.readInt(); ?? _loc1_.currentPet.petFlag
                    }
                }
				gSPacketIn.WriteInt(item.ShootCount); //_loc33_ = _pkg.readInt();
                gSPacketIn.WriteInt(item.flyCount); // _loc16_ = _pkg.readInt();
                if (GameProperties.VERSION >= 7600)
				{
					gSPacketIn.WriteInt(item.TurnNum); //_loc39_ = _pkg.readInt();
                }
			}
			gSPacketIn.WriteInt(game.TurnIndex); //_map.currentTurn = _pkg.readInt();
        }
		gSPacketIn.WriteBoolean(val: false); //_pkg.readBoolean()
        if (base.RoomType == eRoomType.FightFootballTime)
		{
			gSPacketIn.WriteInt(game.nextPlayerId);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendSigleNextTurn(Living p, BaseGame game, List<SimpleBox> newBoxes)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, p.Id);
		gSPacketIn.Parameter1 = p.Id;
		gSPacketIn.WriteByte(6);
		if (GameProperties.VERSION >= 8100)
		{
			gSPacketIn.WriteInt(0);
			if (base.RoomType == eRoomType.HerroAuto)
			{
				gSPacketIn.WriteInt((int)m_map.wind);
			}
		}
		int num = (int)(m_map.wind * 10f);
		gSPacketIn.WriteBoolean(num > 0);
		gSPacketIn.WriteByte(GetVane(num, 1));
		gSPacketIn.WriteByte(GetVane(num, 2));
		gSPacketIn.WriteByte(GetVane(num, 3));
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(100);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteBoolean(p.IsHide);
		gSPacketIn.WriteInt(getTurnTime());
		gSPacketIn.WriteInt(newBoxes.Count);
		foreach (SimpleBox newBox in newBoxes)
		{
			gSPacketIn.WriteInt(newBox.Id);
			gSPacketIn.WriteInt(newBox.X);
			gSPacketIn.WriteInt(newBox.Y);
			gSPacketIn.WriteInt(newBox.Type);
		}
		if (p is TurnedLiving)
		{
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(p.Id);
			gSPacketIn.WriteBoolean(p.IsLiving);
			gSPacketIn.WriteInt(p.X);
			gSPacketIn.WriteInt(p.Y);
			gSPacketIn.WriteInt(p.Blood);
			gSPacketIn.WriteBoolean(p.IsNoHole);
			gSPacketIn.WriteInt(((Player)p).Energy);
			gSPacketIn.WriteInt(((Player)p).psychic);
			gSPacketIn.WriteInt(((Player)p).Dander);
			if (((Player)p).Pet == null)
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(((Player)p).PetMaxMP);
				gSPacketIn.WriteInt(((Player)p).PetMP);
			}
			gSPacketIn.WriteInt(((Player)p).ShootCount);
			gSPacketIn.WriteInt(((Player)p).flyCount);
			if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt(p.TurnNum);
			}
			gSPacketIn.WriteInt(game.TurnIndex);
		}
		gSPacketIn.WriteBoolean(val: false);
		((Player)p).PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendLivingUpdateDirection(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(living.Direction);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingUpdateAngryState(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(118);
		gSPacketIn.WriteInt(living.State);
		SendToAll(gSPacketIn);
	}

	internal void SendEquipEffect(Living player, string buffer)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(buffer);
		SendToAll(gSPacketIn);
	}

	internal void SendMessage(IGamePlayer player, string msg, string msg1, int type)
	{
		if (msg != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteString(msg);
			player.SendTCP(gSPacketIn);
		}
		if (msg1 != null)
		{
			GSPacketIn gSPacketIn2 = new GSPacketIn(3);
			gSPacketIn2.WriteInt(type);
			gSPacketIn2.WriteString(msg1);
			SendToAll(gSPacketIn2, player);
		}
	}

	internal void SendFightAchievement(Living living, int achievID, int num, int delay)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(238);
		gSPacketIn.WriteInt(achievID);
		gSPacketIn.WriteInt(10);
		gSPacketIn.WriteInt(delay);
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerPicture(Living living, int type, bool state, int count = 1)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(128);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(state);
		gSPacketIn.WriteInt(count);
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerRemove(Player player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerDetail.PlayerCharacter.ID);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(player.PlayerDetail.ZoneId);
		SendToAll(gSPacketIn);
	}

	internal void SendAttackEffect(Living player, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(129);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendSyncLifeTime()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(131);
		gSPacketIn.WriteInt(m_lifeTime);
		SendToAll(gSPacketIn);
	}

	protected void OnGameOverred()
	{
		if (this.GameOverred != null)
		{
			this.GameOverred(this);
		}
	}

	protected void OnBeginNewTurn()
	{
		if (this.BeginNewTurn != null)
		{
			this.BeginNewTurn(this);
		}
	}

	public void OnGameOverLog(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
	{
		if (this.GameOverLog != null)
		{
			this.GameOverLog(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, BossWarField);
		}
	}

	public void OnGameNpcDie(int Id)
	{
		if (this.GameNpcDie != null)
		{
			this.GameNpcDie(Id);
		}
	}

	public override string ToString()
	{
		return $"Id:{base.Id},player:{PlayerCount},state:{GameState},current:{CurrentLiving},turnIndex:{m_turnIndex},actions:{m_actions.Count}";
	}

	internal void sendShowPicSkil(Living player, PetSkillElementInfo info, bool isActive)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(145);
		gSPacketIn.WriteInt(info.ID);
		gSPacketIn.WriteString(info.Name);
		gSPacketIn.WriteString(info.Description);
		gSPacketIn.WriteString(info.Pic.ToString());
		gSPacketIn.WriteString(info.EffectPic);
		gSPacketIn.WriteBoolean(isActive);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateHealth(Living living, int type, int value, int embleType = 0)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteLong(living.Blood);
		gSPacketIn.WriteInt(value);
		gSPacketIn.WriteInt(embleType);
		SendToAll(gSPacketIn);
		UpdateMaxBlood(living);
	}

	internal void UpdateBlood(Living living, int type, int value, int embleType = 0)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteLong(living.Blood);
		gSPacketIn.WriteInt(value);
		gSPacketIn.WriteInt(embleType);
		SendToAll(gSPacketIn);
	}

	internal void UpdateMaxBlood(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(164);
		gSPacketIn.WriteInt(player.Id);
		gSPacketIn.WriteInt(player.MaxBlood);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingShowBlood(Living player, long blood, int x, int y)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(39);
		gSPacketIn.WriteInt(player.Id);
		gSPacketIn.WriteLong(blood);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		SendToAll(gSPacketIn);
	}

	public void method_53(Living living_0, string string_0)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(string_0);
		SendToAll(gSPacketIn);
	}

	public void method_54(Living living_0, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteInt(value);
		SendToAll(gSPacketIn);
	}

	internal void updatePlayerBuff(Living living, int type, bool state, int count = 1)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(128);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(state);
		gSPacketIn.WriteInt(count);
		SendToAll(gSPacketIn);
	}

	public Living FindRandomLivingNPC()
	{
		List<Living> list = new List<Living>();
		Living result = null;
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living.IsNPC)
			{
				list.Add(living);
			}
		}
		int index = Random.Next(0, list.Count);
		if (list.Count > 0)
		{
			result = list[index];
		}
		return result;
	}

	internal void SendTempStyle(Player living, string style, bool isTemp)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(134);
		gSPacketIn.WriteInt(1);
		if (!isTemp)
		{
			gSPacketIn.WriteString(living.PlayerDetail.PlayerCharacter.Style);
			gSPacketIn.WriteInt(living.PlayerDetail.PlayerCharacter.Hide);
		}
		else
		{
			gSPacketIn.WriteString(style);
			gSPacketIn.WriteInt(1111111111);
		}
		gSPacketIn.WriteBoolean(living.PlayerDetail.PlayerCharacter.Sex);
		gSPacketIn.WriteString(living.PlayerDetail.PlayerCharacter.Skin);
		gSPacketIn.WriteString(living.PlayerDetail.PlayerCharacter.Colors);
		gSPacketIn.WriteInt(living.PlayerDetail.PlayerCharacter.ID);
		SendToAll(gSPacketIn);
	}

	internal void SkillLock(Player living, bool isLock)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(151);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteBoolean(isLock);
		living.PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void LockSreen(Player living, bool vaule)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(69);
		gSPacketIn.WriteBoolean(vaule);
		living.PlayerDetail.SendTCP(gSPacketIn);
	}

	public Player FindRandomEnemy(Living living)
	{
		List<Player> list = new List<Player>();
		Player result = null;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving && value.Team != living.Team)
			{
				list.Add(value);
			}
		}
		int index = Random.Next(0, list.Count);
		if (list.Count > 0)
		{
			result = list[index];
		}
		return result;
	}

	public List<Player> FindAllEnemyByPixel(Living living, int pixel)
	{
		List<Player> list = new List<Player>();
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving && value.Team != living.Team && Math.Abs(living.X - value.X) <= pixel)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public List<Player> FindAllEnemyByY(Living living)
	{
		List<Player> list = new List<Player>();
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving && value.Team != living.Team && living.Y < value.Y)
			{
				list.Add(value);
			}
		}
		return list;
	}

	internal void SendSelectLabyrinthBuff(string buff)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(165);
		gSPacketIn.WriteString(buff);
		SendToAll(gSPacketIn);
	}

	public int FindBombPlayerX(int blowArea)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		List<int> list = new List<int>();
		List<Player> allFightPlayers = GetAllFightPlayers();
		int num = 0;
		foreach (Player item in allFightPlayers)
		{
			if (!item.IsLiving)
			{
				continue;
			}
			for (int i = 0; i < 10; i++)
			{
				int key;
				do
				{
					key = Random.Next(item.X - blowArea, item.X + blowArea);
				}
				while (dictionary.ContainsKey(key));
				dictionary.Add(key, 0);
			}
		}
		foreach (int key3 in dictionary.Keys)
		{
			foreach (Player item2 in allFightPlayers)
			{
				if (item2.X > key3 - blowArea && item2.X < key3 + blowArea)
				{
					if (!dictionary2.ContainsKey(key3))
					{
						dictionary2.Add(key3, 1);
						continue;
					}
					Dictionary<int, int> dictionary3 = dictionary2;
					Dictionary<int, int> dictionary4 = dictionary3;
					int key2 = key3;
					dictionary3[key2] = dictionary4[key2] + 1;
				}
			}
		}
		foreach (int value in dictionary2.Values)
		{
			if (value > num)
			{
				num = value;
			}
		}
		foreach (int key4 in dictionary2.Keys)
		{
			if (dictionary2[key4] == num)
			{
				list.Add(key4);
			}
		}
		int index = Random.Next(0, list.Count);
		return list[index];
	}

	public List<SimpleNpc> GetNPCLivingWithProperties(int properties)
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc && living.IsLiving && (living as SimpleNpc).State == properties)
			{
				list.Add(living as SimpleNpc);
			}
		}
		return list;
	}

	public void SendLivingChangeAngle(Living living, int Speed, int Angle, string endPlay)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(85);
		gSPacketIn.WriteInt(Angle);
		gSPacketIn.WriteInt(Speed);
		gSPacketIn.WriteString(endPlay);
		SendToAll(gSPacketIn);
	}

	public void ClearAllChildByID(int ID)
	{
		List<Living> list = new List<Living>();
		foreach (SimpleNpc living in m_livings)
		{
			if (living.IsLiving && living != null && living.NpcInfo.ID == ID)
			{
				list.Add(living);
			}
		}
		foreach (Living item in list)
		{
			m_livings.Remove(item);
			item.Dispose();
			RemoveLiving(item.Id);
		}
	}
}

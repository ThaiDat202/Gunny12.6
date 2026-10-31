using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Game;
using Game.Logic.AI.Mission;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class PVEGame : BaseGame
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected APVEGameControl m_gameAI = null;

	protected AMissionControl m_missionAI = null;

	public int SessionId;

	public int PreSessionId;

	public bool IsWin;

	public bool IsKillWorldBoss;

	public bool CanEnterGate;

	public bool CanShowBigBox;

	public int TotalMissionCount;

	public int TotalCount;

	public int TotalTurn;

	public int Param1;

	public int Param2;

	public int Param3;

	public int Param4;

	public string Pic;

	public int TotalKillCount;

	public bool IsMissBall;

	public int TotalGoal;

	public bool IsGoal;

	public bool PassBallActive;

	public bool defendFail;

	public bool CanBeginNextProtect;

	public double TotalNpcExperience;

	public double TotalNpcGrade;

	private int BeginPlayersCount;

	private PveInfo m_info;

	protected List<string> m_gameOverResources;

	public Dictionary<int, MissionInfo> Misssions;

	private MapPoint mapPos;

	public int WantTryAgain;

	public long WorldbossBood;

	public long AllWorldDameBoss;

	private eHardLevel m_hardLevel;

	private string m_IsBossType;

	public int BossCampId;

	protected MissionInfo m_missionInfo;

	private static Random random = new Random();

	private List<int> m_mapHistoryIds;

	public int[] BossCards;

	private int m_bossCardCount;

	protected int m_pveGameDelay;

	private int m_countAward;

	public AMissionControl MissionAI => m_missionAI;

	public DateTime beginTime { get; set; }

	public MissionInfo MissionInfo
	{
		get
		{
			return m_missionInfo;
		}
		set
		{
			m_missionInfo = value;
		}
	}

	public Player CurrentPlayer => m_currentLiving as Player;

	public TurnedLiving CurrentTurnLiving => m_currentLiving;

	public List<int> MapHistoryIds
	{
		get
		{
			return m_mapHistoryIds;
		}
		set
		{
			m_mapHistoryIds = value;
		}
	}

	public eHardLevel HandLevel => m_hardLevel;

	public MapPoint MapPos => mapPos;

	public string IsBossWar
	{
		get
		{
			return m_IsBossType;
		}
		set
		{
			m_IsBossType = value;
		}
	}

	public List<string> GameOverResources => m_gameOverResources;

	public int BossCardCount
	{
		get
		{
			return m_bossCardCount;
		}
		set
		{
			if (value > 0)
			{
				BossCards = new int[9];
				m_bossCardCount = value;
			}
		}
	}

	public int PveGameDelay
	{
		get
		{
			return m_pveGameDelay;
		}
		set
		{
			m_pveGameDelay = value;
		}
	}

	public int CountMosterPlace
	{
		get
		{
			return m_countAward;
		}
		set
		{
			m_countAward = value;
		}
	}

	public PVEGame(int id, int roomId, PveInfo info, List<IGamePlayer> players, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int currentFloor)
		: base(id, roomId, map, roomType, gameType, timeType)
	{
		foreach (IGamePlayer player in players)
		{
			AddPlayer(new Player(player, PhysicalId++, this, 1, player.PlayerCharacter.hp)
			{
				Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1))
			});
			WorldbossBood = player.WorldbossBood;
			AllWorldDameBoss = player.AllWorldDameBoss;
		}
		m_info = info;
		BeginPlayersCount = players.Count;
		TotalKillCount = 0;
		TotalNpcGrade = 0.0;
		TotalNpcExperience = 0.0;
		TotalHurt = 0;
		m_IsBossType = "";
		WantTryAgain = 0;
		loadBossID = currentFloor;
		if (currentFloor > 0)
		{
			SessionId = currentFloor - 1;
		}
		else
		{
			SessionId = 0;
		}
		PreSessionId = 0;
		m_gameOverResources = new List<string>();
		Misssions = new Dictionary<int, MissionInfo>();
		m_mapHistoryIds = new List<int>();
		m_hardLevel = hardLevel;
		string script = GetScript(info, hardLevel);
		m_gameAI = ScriptMgr.CreateInstance(script) as APVEGameControl;
		if (m_gameAI == null)
		{
			log.ErrorFormat("Can't create game ai :{0}, PveId {1}, Pve Name {2}", script, info.ID, info.Name);
			m_gameAI = SimplePVEGameControl.Simple;
			foreach (IGamePlayer player2 in players)
			{
				player2.ResetRoom(isWin: false, "CreateInstanceFail");
				RemovePlayer(player2, isKick: false);
			}
		}
		m_gameAI.Game = this;
		m_gameAI.OnCreated();
		m_missionAI = SimpleMissionControl.Simple;
		beginTime = DateTime.Now;
		m_bossCardCount = 0;
		IsMissBall = false;
	}

	public void LivingChangeAngle(SimpleNpc bomb, int v1, int v2, string v3)
	{
		throw new NotImplementedException();
	}

	private string GetScript(PveInfo pveInfo, eHardLevel hardLevel)
	{
		return hardLevel switch
		{
			eHardLevel.Easy => pveInfo.SimpleGameScript, 
			eHardLevel.Normal => pveInfo.NormalGameScript, 
			eHardLevel.Hard => pveInfo.HardGameScript, 
			eHardLevel.Terror => pveInfo.TerrorGameScript, 
			eHardLevel.Epic => pveInfo.EpicGameScript, 
			eHardLevel.Nightmare => pveInfo.NightmareGameScript, 
			_ => pveInfo.SimpleGameScript, 
		};
	}

	public string GetMissionIdStr(string missionIds, int randomCount)
	{
		if (string.IsNullOrEmpty(missionIds))
		{
			return "";
		}
		string[] array = missionIds.Split(',');
		if (array.Length < randomCount)
		{
			return "";
		}
		List<string> list = new List<string>();
		int maxValue = array.Length;
		int num = 0;
		while (num < randomCount)
		{
			int num2 = base.Random.Next(maxValue);
			string item = array[num2];
			if (!list.Contains(item))
			{
				list.Add(item);
				num++;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item2 in list)
		{
			stringBuilder.Append(item2).Append(",");
		}
		return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
	}

	public void SetupMissions(string missionIds)
	{
		if (string.IsNullOrEmpty(missionIds))
		{
			log.Error("SetupMissions fail, missionIds is empty or null " + missionIds);
			return;
		}
		if ((base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.Element_PVE) && SessionId != 0)
		{
			string[] array = missionIds.Split(',');
			MissionInfo missionInfo = MissionInfoMgr.GetMissionInfo(int.Parse(array[SessionId]));
			Misssions.Add(1, missionInfo);
			SessionId = 0;
			if (missionInfo == null)
			{
				log.Error("SetupMissions fail, not found mission Ids: " + missionIds);
			}
			return;
		}
		int num = 0;
		string[] array2 = missionIds.Split(',');
		string[] array3 = array2;
		foreach (string s in array3)
		{
			num++;
			MissionInfo missionInfo2 = MissionInfoMgr.GetMissionInfo(int.Parse(s));
			Misssions.Add(num, missionInfo2);
		}
	}

	public LivingConfig BaseConfig()
	{
		return new LivingConfig
		{
			isBotom = 1,
			HasTurn = true,
			isShowBlood = true,
			isShowSmallMapPoint = true,
			ReduceBloodStart = 1,
			CanTakeDamage = true,
			CanFrost = true,
			CanCountKill = true
		};
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type)
	{
		return CreateNpc(npcId, x, y, type, -1, "", BaseConfig());
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction)
	{
		return CreateNpc(npcId, x, y, type, direction, "", BaseConfig());
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, string action)
	{
		return CreateNpc(npcId, x, y, type, direction, action, BaseConfig());
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, LivingConfig config)
	{
		return CreateNpc(npcId, x, y, type, direction, "", config);
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, string action, LivingConfig config)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleNpc simpleNpc = new SimpleNpc(PhysicalId++, this, npcInfoById, type, direction, action);
		if (config != null)
		{
			simpleNpc.Config = config;
		}
		if (simpleNpc.Config.ReduceBloodStart > 1)
		{
			simpleNpc.Blood = npcInfoById.Blood / simpleNpc.Config.ReduceBloodStart;
		}
		else
		{
			simpleNpc.Reset();
		}
		simpleNpc.SetXY(x, y);
		AddLiving(simpleNpc);
		simpleNpc.StartMoving();
		return simpleNpc;
	}

	public SimpleNpc CreateNpc(int npcId, int type, int direction)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleNpc simpleNpc = new SimpleNpc(PhysicalId++, this, npcInfoById, type, direction, "");
		Point playerPoint = GetPlayerPoint(mapPos, npcInfoById.Camp);
		simpleNpc.Reset();
		simpleNpc.SetXY(playerPoint);
		AddLiving(simpleNpc);
		simpleNpc.StartMoving();
		return simpleNpc;
	}

	public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action)
	{
		return CreateBoss(npcId, x, y, direction, type, action, BaseConfig());
	}

	public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action, LivingConfig config)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleBoss simpleBoss = new SimpleBoss(PhysicalId++, this, npcInfoById, direction, type, action);
		if (config != null)
		{
			simpleBoss.Config = config;
		}
		if (simpleBoss.Config.ReduceBloodStart > 1)
		{
			simpleBoss.Blood = npcInfoById.Blood / simpleBoss.Config.ReduceBloodStart;
		}
		else
		{
			simpleBoss.Reset();
			if (simpleBoss.Config.IsWorldBoss && WorldbossBood < int.MaxValue)
			{
				simpleBoss.Blood = (int)WorldbossBood;
			}
			if (simpleBoss.Config.isConsortiaBoss)
			{
				simpleBoss.Blood -= (int)AllWorldDameBoss;
			}
		}
		simpleBoss.SetXY(x, y);
		AddLiving(simpleBoss);
		simpleBoss.StartMoving();
		return simpleBoss;
	}

	public SimpleBox CreateBox(int x, int y, string model, ItemInfo item)
	{
		SimpleBox simpleBox = new SimpleBox(PhysicalId++, model, item, 1);
		simpleBox.SetXY(x, y);
		m_map.AddPhysical(simpleBox);
		AddBox(simpleBox, sendToClient: true);
		return simpleBox;
	}

	public Ball CreateBall(int x, int y, string action)
	{
		Ball ball = new Ball(PhysicalId++, "", action, 1, 1);
		ball.SetXY(x, y);
		m_map.AddPhysical(ball);
		AddBall(ball, sendToClient: true);
		return ball;
	}

	public void SendGameFocus(Physics p, int delay, int finishTime)
	{
		AddAction(new FocusAction(p, 1, delay, finishTime));
	}

	public void SendGameFocus(int x, int y, int delay, int finishTime)
	{
		AddAction(new FocusLocalAction(x, y, 1, delay, finishTime));
	}

	public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		return CreatePhysicalObj(x, y, name, model, defaultAction, scale, rotation, 0);
	}

	public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation, int typeEffect)
	{
		PhysicalObj physicalObj = new PhysicalObj(PhysicalId++, name, model, defaultAction, scale, rotation, typeEffect, canPenetrate: false);
		physicalObj.SetXY(x, y);
		AddPhysicalObj(physicalObj, sendToClient: true);
		return physicalObj;
	}

	public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scale, int rotation, bool CanPenetrate)
	{
		int id = PhysicalId++;
		if (defaultAction == "")
		{
			defaultAction = "out";
		}
		Layer layer = new Layer(id, name, model, defaultAction, scale, rotation, CanPenetrate);
		layer.SetXY(x, y);
		AddPhysicalObj(layer, sendToClient: true);
		return layer;
	}

	public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		Layer layer = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
		layer.SetXY(x, y);
		AddPhysicalObj(layer, sendToClient: true);
		return layer;
	}

	public LayerTop CreateLayerTop(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		LayerTop layerTop = new LayerTop(PhysicalId++, name, model, defaultAction, scale, rotation);
		layerTop.SetXY(x, y);
		AddPhysicalObj(layerTop, sendToClient: true);
		return layerTop;
	}

	public Layer CreateLayerBoss(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		Layer layer = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
		layer.SetXY(x, y);
		AddPhysicalObj(layer, sendToClient: true);
		return layer;
	}

	public Layer CreateTip(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		Layer layer = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
		layer.SetXY(x, y);
		AddPhysicalTip(layer, sendToClient: true);
		return layer;
	}

	public Ball CreateBall(int x, int y, string name, string defaultAction, int scale, int rotation)
	{
		Ball ball = new Ball(PhysicalId++, name, defaultAction, scale, rotation);
		ball.SetXY(x, y);
		AddPhysicalObj(ball, sendToClient: true);
		return ball;
	}

	public void CreateGate(bool isEnter)
	{
		CanEnterGate = isEnter;
	}

	public void ClearMissionData()
	{
		foreach (Living living in m_livings)
		{
			living.Dispose();
		}
		m_livings.Clear();
		List<TurnedLiving> list = new List<TurnedLiving>();
		foreach (TurnedLiving item in base.TurnQueue)
		{
			if (item is Player)
			{
				if (item.IsLiving)
				{
					list.Add(item);
				}
			}
			else
			{
				item.Dispose();
			}
		}
		base.TurnQueue.Clear();
		foreach (TurnedLiving item2 in list)
		{
			base.TurnQueue.Add(item2);
		}
		if (m_map == null)
		{
			return;
		}
		foreach (PhysicalObj item3 in m_map.GetAllPhysicalObjSafe())
		{
			item3.Dispose();
		}
	}

	public void AddAllPlayerToTurn()
	{
		foreach (Player value in base.Players.Values)
		{
			base.TurnQueue.Add(value);
		}
	}

	public override void AddLiving(Living living)
	{
		base.AddLiving(living);
		living.Died += living_Died;
	}

	private void living_Died(Living living)
	{
		if (base.CurrentLiving != null && base.CurrentLiving is Player && !(living is Player) && living != base.CurrentLiving && living.Config.CanCountKill)
		{
			TotalKillCount++;
			TotalNpcExperience += living.Experience;
			TotalNpcGrade += living.Grade;
		}
		if (living is SimpleNpc)
		{
			((SimpleNpc)living).OnDie();
		}
		m_missionAI.OnDied();
	}

	public override void MissionSingleStart(Player p)
	{
		SendCreateGame();
		StartLoading(p);
		m_gameSecondState = eGameState.Loading;
	}

	public override void MissionStart(IGamePlayer host)
	{
		if (base.GameState != eGameState.SessionPrepared && base.GameState != eGameState.GameOver)
		{
			return;
		}
		foreach (Player value in base.Players.Values)
		{
			value.Ready = true;
		}
		CheckState(0);
	}

	public override bool CanAddPlayer()
	{
		lock (m_players)
		{
			return base.GameState == eGameState.SessionPrepared && m_players.Count < 4;
		}
	}

	public override Player AddPlayer(IGamePlayer gp)
	{
		if (CanAddPlayer())
		{
			Player player = new Player(gp, PhysicalId++, this, 1, gp.PlayerCharacter.hp);
			player.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
			player.Ready = true;
			AddPlayer(player);
			SendCreateGameToSingle(this, gp);
			SendPlayerInfoInGame(player);
			log.Error("//send add single players");
			return player;
		}
		return null;
	}

	public override Player RemovePlayer(IGamePlayer gp, bool isKick)
	{
		Player player = GetPlayer(gp);
		if (player != null)
		{
			player.PlayerDetail.ClearFightBuffOneMatch();
			player.PlayerDetail.ClearPropItem();
			player.PlayerDetail.ResetRoom(isWin: false, "RemovePlayer");
			base.RemovePlayer(gp, isKick);
		}
		return player;
	}

	public void LoadResources(int[] npcIds)
	{
		if (npcIds == null || npcIds.Length == 0)
		{
			return;
		}
		foreach (int num in npcIds)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(num);
			if (npcInfoById == null)
			{
				log.ErrorFormat("LoadResources npcInfo {0} resoure is not exits", num);
			}
			else
			{
				AddLoadingFile(2, npcInfoById.ResourcesPath, npcInfoById.ModelID);
			}
		}
	}

	public void LoadNpcGameOverResources(int[] npcIds)
	{
		if (npcIds == null || npcIds.Length == 0)
		{
			return;
		}
		foreach (int id in npcIds)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(id);
			if (npcInfoById == null)
			{
				log.Error("LoadGameOverResources npcInfo resoure is not exits");
			}
			else
			{
				m_gameOverResources.Add(npcInfoById.ModelID);
			}
		}
	}

	public virtual void Prepare()
	{
		if (GetAllPlayers().Length == 0)
		{
			m_gameState = eGameState.Stopped;
			log.Error("Prepare GetAllPlayers().Length == 0");
		}
		else if (base.GameState == eGameState.Inited)
		{
			m_gameState = eGameState.Prepared;
			SendCreateGame();
			CheckState(0);
			try
			{
				m_gameAI.OnPrepated();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
		}
	}

	public virtual void PrepareNewSession()
	{
		if (base.GameState != eGameState.Prepared && base.GameState != eGameState.GameOver && base.GameState != eGameState.ALLSessionStopped)
		{
			return;
		}
		try
		{
			m_gameState = eGameState.SessionPrepared;
			SessionId++;
			ClearLoadingFiles();
			ClearMissionData();
			m_gameOverResources.Clear();
			WantTryAgain = 0;
			m_missionInfo = Misssions[SessionId];
			m_pveGameDelay = m_missionInfo.Delay;
			TotalCount = m_missionInfo.TotalCount;
			TotalTurn = m_missionInfo.TotalTurn;
			Param1 = m_missionInfo.Param1;
			Param2 = m_missionInfo.Param2;
			Param3 = -1;
			Param4 = -1;
			m_missionAI = ScriptMgr.CreateInstance(m_missionInfo.Script) as AMissionControl;
			if (m_missionAI == null)
			{
				log.ErrorFormat("Can't create game mission ai :{0}", m_missionInfo.Script);
				m_missionAI = SimpleMissionControl.Simple;
				List<Player> allFightPlayers = GetAllFightPlayers();
				foreach (Player item in allFightPlayers)
				{
					item.PlayerDetail.ResetRoom(isWin: false, "CreateInstanceFail");
					RemovePlayer(item.PlayerDetail, isKick: false);
				}
				m_gameState = eGameState.Stopped;
			}
			if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.Element_PVE)
			{
				Pic = $"show{SessionId}.jpg";
				List<Player> allFightPlayers2 = GetAllFightPlayers();
				foreach (Player item2 in allFightPlayers2)
				{
					item2.PlayerDetail.UpdateBarrier(SessionId, Pic);
				}
			}
			m_missionAI.Game = this;
			try
			{
				m_missionAI.OnPrepareNewSession();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
		}
		catch (Exception arg2)
		{
			m_gameState = eGameState.Stopped;
			log.ErrorFormat("Preparen new session, m_missionInfo.Script {0}, m_missionInfo.Id {1}, exception: {2}", m_missionInfo.Script, m_missionInfo.Id, arg2);
		}
	}

	public bool CanStartNewSession()
	{
		return base.m_turnIndex == 0 || IsAllReady();
	}

	public bool IsAllReady()
	{
		foreach (Player value in base.Players.Values)
		{
			if (!value.Ready)
			{
				return false;
			}
		}
		return true;
	}

	public void StartLoading()
	{
		StartLoading(null);
	}

	public void StartLoading(Player np)
	{
		if (base.RoomType != eRoomType.CampBattleBattle && base.GameState != eGameState.SessionPrepared)
		{
			return;
		}
		try
		{
			if (np == null)
			{
				m_gameState = eGameState.Loading;
				base.m_turnIndex = 0;
			}
			VaneLoading(np);
			SendMissionInfo(np);
			SetupStyle();
			SendStartLoading(60);
			if (np == null)
			{
				AddAction(new WaitPlayerLoadingAction(this, 61000));
			}
		}
		catch (Exception message)
		{
			m_gameState = eGameState.Stopped;
			log.Error(message);
		}
	}

	public void StartGameMovie()
	{
		if (base.GameState == eGameState.Loading)
		{
			try
			{
				m_missionAI.OnStartMovie();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
		}
	}

	public void SetupStyle()
	{
		SendTempStyle((m_info != null) ? m_info.ID : 0);
	}

	public bool HasChangeStyle()
	{
		if (m_info == null)
		{
			return false;
		}
		int iD = m_info.ID;
		int num = iD;
		if (num == 6)
		{
			return true;
		}
		return false;
	}

	public string GuluOlympics(string style, int place)
	{
		string text = style;
		string[] array = style.Split(',');
		string[] array2 = new string[4] { "13300|suits100", "13301|suits101", "13302|suits102", "13303|suits103" };
		text = array[0];
		for (int i = 1; i < EquipPlace.Length; i++)
		{
			text += ",";
			if (EquipPlace[i] == 11)
			{
				text = text + array[i] + ",";
				text += array2[place];
			}
			else
			{
				text += array[i];
			}
		}
		return text;
	}

	internal void SendTempStyle(int skin)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(134);
		List<Player> allFightPlayers = GetAllFightPlayers();
		int num = 0;
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			IGamePlayer playerDetail = item.PlayerDetail;
			string style = playerDetail.PlayerCharacter.Style;
			if (skin == 6)
			{
				gSPacketIn.WriteString(GuluOlympics(style, num));
			}
			else
			{
				gSPacketIn.WriteString(style);
			}
			num++;
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Hide);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.Sex);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Skin);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Colors);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ID);
		}
		if (HasChangeStyle())
		{
			SendToAll(gSPacketIn);
		}
	}

	public void StartGameNewPlayer(Player player)
	{
		if (base.GameSecondState != eGameState.Loading)
		{
			return;
		}
		m_gameSecondState = eGameState.GameStart;
		SendSyncLifeTime();
		mapPos = MapMgr.GetPVEMapRandomPos(m_map.Info.ID);
		List<Player> allFightingPlayers = GetAllFightingPlayers();
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(99);
		gSPacketIn.WriteInt(allFightingPlayers.Count);
		foreach (Player item in allFightingPlayers)
		{
			if (player == item)
			{
				item.Reset();
				Point playerPoint = GetPlayerPoint(mapPos, item.Team);
				item.SetXY(playerPoint);
				m_map.AddPhysical(item);
				AddLiving(item);
				item.StartMoving();
				item.StartGame();
				if (playerPoint.X < 600)
				{
					item.Direction = 1;
				}
				else
				{
					item.Direction = -1;
				}
				SendAddNewPlayer(item);
			}
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.X);
			gSPacketIn.WriteInt(item.Y);
			gSPacketIn.WriteInt(item.Direction);
			gSPacketIn.WriteInt(item.Blood);
			gSPacketIn.WriteInt(item.MaxBlood);
			gSPacketIn.WriteInt(item.Team);
			gSPacketIn.WriteInt(item.Weapon.RefineryLevel);
			gSPacketIn.WriteInt(item.deputyWeaponCount);
			gSPacketIn.WriteInt(item.PowerRatio);
			gSPacketIn.WriteInt(item.Dander);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.PlayerDetail.FightBuffs.Count);
			foreach (BufferInfo fightBuff in item.PlayerDetail.FightBuffs)
			{
				gSPacketIn.WriteInt(fightBuff.Type);
				gSPacketIn.WriteInt(fightBuff.Value);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(item.IsFrost);
			gSPacketIn.WriteBoolean(item.IsHide);
			gSPacketIn.WriteBoolean(item.IsNoHole);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			if (GameProperties.VERSION >= 9500)
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteDateTime(DateTime.Now);
		if (GameProperties.VERSION >= 8100)
		{
			gSPacketIn.WriteInt(0);
		}
		player.PlayerDetail.SendTCP(gSPacketIn);
		SendUpdateUiData(player);
	}

    //__gameStart PVE
    public void StartGame()
	{
		if (base.GameState != eGameState.Loading)
		{
			return;
		}
		try
		{
			StartGameMovie();
			m_gameState = eGameState.GameStart;
			SendSyncLifeTime();
			TotalKillCount = 0;
			TotalNpcGrade = 0.0;
			TotalNpcExperience = 0.0;
			TotalHurt = 0;
			m_bossCardCount = 0;
			BossCards = null;
			List<Player> allFightingPlayers = GetAllFightingPlayers();
			mapPos = MapMgr.GetPVEMapRandomPos(m_map.Info.ID);
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(99);
			gSPacketIn.WriteInt(allFightingPlayers.Count); //_loc26_ = (_loc21_ = param1.pkg).readInt();
            foreach (Player item in allFightingPlayers)
			{
				if (!item.IsLiving)
				{
					AddLiving(item);
				}
				item.Reset();
				Point playerPoint = GetPlayerPoint(mapPos, item.Team);
				item.SetXY(playerPoint);
				m_map.AddPhysical(item);
				item.StartMoving();
				item.StartGame();
				gSPacketIn.WriteInt(item.Id);
				gSPacketIn.WriteInt(item.X);
				gSPacketIn.WriteInt(item.Y);
				if (playerPoint.X < 600)
				{
					item.Direction = 1;
				}
				else
				{
					item.Direction = -1;
				}
				gSPacketIn.WriteInt(item.Direction); //_loc9_.direction = _loc21_.readInt();
                gSPacketIn.WriteInt(item.Blood);
                gSPacketIn.WriteInt(item.MaxBlood);
                gSPacketIn.WriteInt(item.Team);
                gSPacketIn.WriteInt(item.Weapon.RefineryLevel);
                gSPacketIn.WriteInt(item.deputyWeaponCount);
                gSPacketIn.WriteInt(item.PowerRatio);
                gSPacketIn.WriteInt(item.Dander);
                gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(item.PlayerDetail.FightBuffs.Count);
				foreach (BufferInfo fightBuff in item.PlayerDetail.FightBuffs)
				{
					gSPacketIn.WriteInt(fightBuff.Type);
					gSPacketIn.WriteInt(fightBuff.Value);
				}
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(item.IsFrost);
				gSPacketIn.WriteBoolean(item.IsHide);
				gSPacketIn.WriteBoolean(item.IsNoHole);
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteInt(0);
				if (GameProperties.VERSION >= 8100)
				{
					gSPacketIn.WriteInt(0);
				}
				if (GameProperties.VERSION >= 9500)
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
				}
                if (GameProperties.VERSION >= 12100)
                {
                    /*_loc9_.lingStoneSkillId = _loc21_.readInt();
                      _loc9_.lingStoneSkillCount = _loc21_.readInt();*/
                    gSPacketIn.WriteInt(0);
                    gSPacketIn.WriteInt(0);
                }
                item.OnGameStarted();
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteDateTime(DateTime.Now);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			SendToAll(gSPacketIn);
			try
			{
				m_missionAI.OnPrepareStartGame();
				foreach (Player item2 in allFightingPlayers)
				{
					item2.StartMoving();
					item2.BoltMove(item2.X, item2.Y, 1000);
				}
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
			SendUpdateUiData(null);
			WaitTime(base.PlayerCount * 2500 + 1000);
			OnGameStarted();
		}
		catch (Exception message)
		{
			m_gameState = eGameState.Stopped;
			log.Error(message);
		}
	}

	public void PrepareNewGame()
	{
		if (base.GameState == eGameState.GameStart)
		{
			m_gameState = eGameState.Playing;
			BossCardCount = 0;
			SendSyncLifeTime();
			WaitTime(base.PlayerCount * 1000);
			try
			{
				m_missionAI.OnStartGame();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
		}
	}

	public void NextTurn()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		ClearWaitTimer();
		ClearDiedPhysicals();
		CheckBox();
		LivingRandSay();
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item in allPhysicalSafe)
		{
			item.PrepareNewTurn();
		}
		List<SimpleBox> newBoxes = CreateBox();
		try
		{
			m_missionAI.OnNewTurnStarted();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("NextTurn game ai script {0} error:{1}", base.GameState, arg);
		}
		LastTurnLiving = m_currentLiving;
		m_currentLiving = FindNextTurnedLiving(isPvP: false);
		if (m_currentLiving != null)
		{
			if (m_currentLiving != LastTurnLiving && LastTurnLiving is Player && (LastTurnLiving as Player).IsAddTurn)
			{
				m_currentLiving = LastTurnLiving;
				(LastTurnLiving as Player).IsAddTurn = false;
			}
			base.m_turnIndex++;
			SendUpdateUiData(null);
			List<Living> livedLivingsHadTurn = GetLivedLivingsHadTurn();
			Living living = null;
			if (livedLivingsHadTurn.Count > 0 && m_currentLiving.Delay >= m_pveGameDelay)
			{
				MinusDelays(m_pveGameDelay);
				foreach (Living living2 in m_livings)
				{
					living2.PrepareSelfTurn();
					if (!living2.IsFrost)
					{
						living2.StartAttacking();
						if (living == null)
						{
							living = living2;
						}
					}
				}
				if (living != null)
				{
					SendGameNextTurn(living, this, newBoxes);
				}
				foreach (Living living3 in m_livings)
				{
					if (living3.IsAttacking)
					{
						living3.StopAttacking();
					}
				}
				m_pveGameDelay += MissionInfo.IncrementDelay;
				m_currentLiving = null;
				CheckState(0);
			}
			else if (base.RoomType == eRoomType.ActivityDungeon && m_currentLiving is Player)
			{
				TurnedLiving[] nextAllTurnedLiving = GetNextAllTurnedLiving();
				UpdateWind(GetNextWind(), sendToClient: false);
				CurrentTurnTotalDamage = 0;
				TurnedLiving[] array = nextAllTurnedLiving;
				foreach (TurnedLiving turnedLiving in array)
				{
					MinusDelays(turnedLiving.Delay);
					turnedLiving.PrepareSelfTurn();
					turnedLiving.StartAttacking();
					SendSyncLifeTime();
					SendSigleNextTurn(turnedLiving, this, newBoxes);
					SendFightStatus(turnedLiving, 1);
					AddAction(new WaitLivingAttackingAction(turnedLiving, base.m_turnIndex, (m_timeType + 20) * 1000));
				}
			}
			else
			{
				if (CanShowBigBox)
				{
					ShowBigBox();
					CanEnterGate = true;
				}
				MinusDelays(m_currentLiving.Delay);
				if (m_currentLiving is Player)
				{
					UpdateWind(GetNextWind(), sendToClient: false);
				}
				CurrentTurnTotalDamage = 0;
				m_currentLiving.PrepareSelfTurn();
				if (m_currentLiving.IsLiving && !m_currentLiving.IsFrost && !m_currentLiving.BlockTurn)
				{
					m_currentLiving.StartAttacking();
					SendSyncLifeTime();
					SendGameNextTurn(m_currentLiving, this, newBoxes);
					if (m_currentLiving.IsAttacking)
					{
						AddAction(new WaitLivingAttackingAction(m_currentLiving, base.m_turnIndex, (getTurnTime() + 20) * 1000));
					}
				}
			}
		}
		OnBeginNewTurn();
		try
		{
			m_missionAI.OnBeginNewTurn();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
		}
	}

	public void LivingRandSay()
	{
		if (m_livings == null || m_livings.Count == 0)
		{
			return;
		}
		int num = 0;
		int count = m_livings.Count;
		foreach (Living living in m_livings)
		{
			living.IsSay = false;
		}
		if (base.TurnIndex % 2 == 0)
		{
			return;
		}
		num = ((count <= 5) ? base.Random.Next(0, 2) : ((count <= 5 || count > 10) ? base.Random.Next(1, 4) : base.Random.Next(1, 3)));
		if (num <= 0)
		{
			return;
		}
		int[] array = new int[num];
		int num2 = 0;
		while (num2 < num)
		{
			int index = base.Random.Next(0, count);
			if (!m_livings[index].IsSay)
			{
				m_livings[index].IsSay = true;
				num2++;
			}
		}
	}

	public override bool TakeCard(Player player)
	{
		int index = 0;
		for (int i = 0; i < Cards.Length; i++)
		{
			if (Cards[i] == 0)
			{
				index = i;
				break;
			}
		}
		return TakeCard(player, index, isAuto: true);
	}

	public override bool TakeCard(Player player, int index, bool isAuto)
	{
		if (player.CanTakeOut == 0)
		{
			return false;
		}
		if (index < 0 || index > Cards.Length || player.FinishTakeCard || Cards[index] > 0)
		{
			return false;
		}
		int num = 0;
		int num2 = 0;
		int count = 0;
		List<ItemInfo> info = null;
		int copyId = m_missionInfo.Id;
		if (Misssions.ContainsKey(PreSessionId))
		{
			copyId = Misssions[PreSessionId].Id;
		}
		int value = 30;
		if (IsShowLargeCards())
		{
			value = 0;
		}
		if (!player.PlayerDetail.MissionEnergyEmpty(value))
		{
			copyId = 0;
		}
		if (DropInventory.CopyDrop(copyId, 1, ref info))
		{
			if (info != null)
			{
				foreach (ItemInfo item in info)
				{
					if (item != null)
					{
						num2 = item.TemplateID;
						count = item.Count;
						player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
					}
				}
			}
			if (num2 == 0 && num > 0)
			{
				num2 = -100;
				count = num;
			}
		}
		if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon || base.RoomType == eRoomType.Element_PVE)
		{
			player.CanTakeOut--;
			if (player.CanTakeOut == 0)
			{
				player.FinishTakeCard = true;
			}
		}
		else
		{
			player.FinishTakeCard = true;
		}
		Cards[index] = 1;
		SendGamePlayerTakeCard(player, isAuto, index, num2, count);
		return true;
	}

	public bool CanGameOver()
	{
		if (base.PlayerCount == 0)
		{
			return true;
		}
		if (GetDiedPlayerCount() == base.PlayerCount)
		{
			IsWin = false;
			return true;
		}
		try
		{
			return m_missionAI.CanGameOver();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		return true;
	}

	public void TakeSnow()
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(201144);
		if (itemTemplateInfo == null)
		{
			return;
		}
		int num = base.Random.Next(1, 9);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, num, 101);
		itemInfo.IsBinds = true;
		string text = "";
		foreach (Player allFightPlayer in GetAllFightPlayers())
		{
			allFightPlayer.PlayerDetail.AddTemplate(itemInfo, itemInfo.Template.BagType, num, eGameView.dungeonTypeGet);
			text += LanguageMgr.GetTranslation("PVEGame.Msg1", itemInfo.Template.Name, num);
			allFightPlayer.PlayerDetail.SendMessage(text);
		}
	}

	public void TakeConsortiaBossAward(bool isWin)
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdatePveResult("consortiaboss", item.TotalDameLiving, isWin);
		}
	}

	public void ShowBigBox()
	{
		List<ItemInfo> info = null;
		DropInventory.CopyDrop(m_missionInfo.Id, SessionId, ref info);
		List<int> list = new List<int>();
		if (info != null)
		{
			foreach (ItemInfo item in info)
			{
				list.Add(item.TemplateID);
			}
		}
		foreach (Player allFightPlayer in GetAllFightPlayers())
		{
			if (CanGetLabyrinthAward(allFightPlayer.PlayerDetail.ProcessLabyrinthAward))
			{
				SendGameBigBox(allFightPlayer, list);
				allFightPlayer.PlayerDetail.UpdateLabyrinth(SessionId, m_missionInfo.Id, bigAward: true);
			}
		}
	}

	private bool CanGetLabyrinthAward(string param2)
	{
		bool result = false;
		string text = "0";
		if (param2.Length > 0)
		{
			string[] array = param2.Split('-');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = text2;
				if (text == SessionId.ToString())
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public void GameOverMovie()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		m_gameState = eGameState.GameOver;
		ClearWaitTimer();
		ClearDiedPhysicals();
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdateLabyrinth(SessionId, m_missionInfo.Id, bigAward: false);
		}
		try
		{
			m_missionAI.OnGameOverMovie();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		bool flag = HasNextSession();
		if (!flag)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(112);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(base.PlayerCount);
			foreach (Player item2 in allFightPlayers)
			{
				if (IsLabyrinth())
				{
					item2.PlayerDetail.OutLabyrinth(IsWin);
				}
				item2.OnPlayerClearBuffSkillPet();
				int gp = CalculateExperience(item2);
				int num = CalculateScore(item2);
				int num2 = m_missionAI.CalculateScoreGrade(item2.TotalAllScore);
				if (item2.CurrentIsHitTarget)
				{
					item2.TotalHitTargetCount++;
				}
				int num3 = CalculateHitRate(item2.TotalHitTargetCount, item2.TotalShootCount);
				item2.TotalAllHurt += item2.TotalHurt;
				item2.TotalAllCure += item2.TotalCure;
				item2.TotalAllHitTargetCount += item2.TotalHitTargetCount;
				item2.TotalAllShootCount += item2.TotalShootCount;
				item2.GainGP = item2.PlayerDetail.AddGP(gp);
				item2.TotalAllExperience += item2.GainGP;
				item2.TotalAllScore += num;
				item2.BossCardCount = BossCardCount;
				gSPacketIn.WriteInt(item2.PlayerDetail.PlayerCharacter.ID);
				gSPacketIn.WriteInt(item2.PlayerDetail.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(item2.GainGP);
				gSPacketIn.WriteBoolean(IsWin);
				gSPacketIn.WriteInt(BossCardCount);
				gSPacketIn.WriteInt(item2.BossCardCount);
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteBoolean(val: false);
			}
			if (BossCardCount > 0)
			{
				gSPacketIn.WriteInt(m_gameOverResources.Count);
				foreach (string gameOverResource in m_gameOverResources)
				{
					gSPacketIn.WriteString(gameOverResource);
				}
			}
			SendToAll(gSPacketIn);
			OnGameStopped();
			OnGameOverred();
			return;
		}
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item3 in allPhysicalSafe)
		{
			item3.PrepareNewTurn();
		}
		m_currentLiving = FindNextTurnedLiving(isPvP: false);
		if (m_currentLiving != null && CanEnterGate)
		{
			base.m_turnIndex++;
			m_currentLiving.PrepareSelfTurn();
			List<SimpleBox> newBoxes = new List<SimpleBox>();
			SendGameNextTurn(m_currentLiving, this, newBoxes);
			CanEnterGate = false;
			CanShowBigBox = false;
			EnterNextFloor();
			sendTransmissionGate();
		}
		OnBeginNewTurn();
		try
		{
			m_missionAI.OnBeginNewTurn();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
		}
	}

	public void EnterNextFloor()
	{
		SendSelectLabyrinthBuff("");
	}

	public bool IsLabyrinth()
	{
		return base.RoomType == eRoomType.Lanbyrinth;
	}

	public void GameOver()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		try
		{
			m_gameState = eGameState.GameOver;
			SendUpdateUiData(null);
			try
			{
				m_missionAI.OnGameOver();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
			List<Player> allFightPlayers = GetAllFightPlayers();
			CurrentTurnTotalDamage = 0;
			BossCardCount = 5;
			m_bossCardCount = 1;
			bool flag = HasNextSession();
			if (!IsWin || !flag)
			{
				m_bossCardCount = 0;
			}
			if (IsWin && !flag && !isTrainer())
			{
				m_bossCardCount = 2;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(112);
			gSPacketIn.WriteInt(BossCardCount);
			if (flag || IsShowLargeCards())
			{
				gSPacketIn.WriteBoolean(val: true);
				gSPacketIn.WriteString($"show{1 + SessionId}.jpg");
				gSPacketIn.WriteBoolean(val: true);
			}
			else
			{
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteBoolean(val: false);
			}
			gSPacketIn.WriteInt(base.PlayerCount);
			foreach (Player item in allFightPlayers)
			{
				item.OnPlayerClearBuffSkillPet();
				if (!IsWin && IsLabyrinth())
				{
					item.PlayerDetail.OutLabyrinth(IsWin);
				}
				int num = CalculateExperience(item);
				if (item.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
				{
					num += num * item.FightBuffers.ConsortionAddPercentGoldOrGP / 100;
				}
				int num2 = CalculateScore(item);
				int num3 = m_missionAI.CalculateScoreGrade(item.TotalAllScore);
				item.CanTakeOut = BossCardCount;
				if (item.CurrentIsHitTarget)
				{
					item.TotalHitTargetCount++;
				}
				int num4 = CalculateHitRate(item.TotalHitTargetCount, item.TotalShootCount);
				item.TotalAllHurt += item.TotalHurt;
				item.TotalAllCure += item.TotalCure;
				item.TotalAllHitTargetCount += item.TotalHitTargetCount;
				item.TotalAllShootCount += item.TotalShootCount;
				item.GainGP = item.PlayerDetail.AddGP(num);
				item.TotalAllExperience += item.GainGP;
				item.TotalAllScore += num2;
				item.BossCardCount = m_bossCardCount;
				gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.ID);
				gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt((item.GainGP > 10000) ? 10000 : item.GainGP);
				gSPacketIn.WriteBoolean(IsWin);
				gSPacketIn.WriteInt(BossCardCount);
				gSPacketIn.WriteInt(item.BossCardCount);
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteBoolean(val: false);
			}
			if (BossCardCount > 0)
			{
				gSPacketIn.WriteInt(m_gameOverResources.Count);
				foreach (string gameOverResource in m_gameOverResources)
				{
					gSPacketIn.WriteString(gameOverResource);
				}
			}
			SendToAll(gSPacketIn);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Player item2 in allFightPlayers)
			{
				stringBuilder.Append(item2.PlayerDetail.PlayerCharacter.ID).Append(",");
				item2.Ready = false;
				item2.PlayerDetail.OnMissionOver(item2.Game, IsWin, MissionInfo.Id, item2.TurnNum);
			}
			int winTeam = (IsWin ? 1 : 2);
			string teamA = stringBuilder.ToString();
			string teamB = "";
			string playResult = "";
			if (!IsWin)
			{
				OnGameStopped();
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			if (IsWin && IsBossWar != "")
			{
				stringBuilder2.Append(IsBossWar).Append(",");
				foreach (Player item3 in allFightPlayers)
				{
					stringBuilder2.Append("PlayerCharacter ID: ").Append(item3.PlayerDetail.PlayerCharacter.ID).Append(",");
					stringBuilder2.Append("Grade: ").Append(item3.PlayerDetail.PlayerCharacter.Grade).Append(",");
					stringBuilder2.Append("TurnNum): ").Append(item3.TurnNum).Append(",");
					stringBuilder2.Append("Attack: ").Append(item3.PlayerDetail.PlayerCharacter.Attack).Append(",");
					stringBuilder2.Append("Defence: ").Append(item3.PlayerDetail.PlayerCharacter.Defence).Append(",");
					stringBuilder2.Append("Agility: ").Append(item3.PlayerDetail.PlayerCharacter.Agility).Append(",");
					stringBuilder2.Append("Luck: ").Append(item3.PlayerDetail.PlayerCharacter.Luck).Append(",");
					stringBuilder2.Append("BaseAttack: ").Append(item3.PlayerDetail.BaseAttack).Append(",");
					stringBuilder2.Append("MaxBlood: ").Append(item3.MaxBlood).Append(",");
					stringBuilder2.Append("BaseDefence: ").Append(item3.PlayerDetail.BaseDefence).Append(",");
					if (item3.PlayerDetail.SecondWeapon != null)
					{
						stringBuilder2.Append("SecondWeapon TemplateID: ").Append(item3.PlayerDetail.SecondWeapon.TemplateID).Append(",");
						stringBuilder2.Append("SecondWeapon StrengthenLevel: ").Append(item3.PlayerDetail.SecondWeapon.StrengthenLevel).Append(".");
					}
				}
			}
			BossWarField = stringBuilder2.ToString();
			OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, beginTime, DateTime.Now, BeginPlayersCount, MissionInfo.Id, teamA, teamB, playResult, winTeam, BossWarField);
			OnGameOverred();
		}
		catch (Exception message)
		{
			m_gameState = eGameState.Stopped;
			log.Error(message);
		}
	}

	public void GameOverAreaAll()
	{
		if (base.GameState == eGameState.Playing)
		{
			m_gameState = eGameState.GameOver;
			SendUpdateUiData(null);
			try
			{
				m_missionAI.OnGameOver();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
			List<Player> allFightPlayers = GetAllFightPlayers();
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(112);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(base.PlayerCount);
			foreach (Player item in allFightPlayers)
			{
				item.BossCardCount = 0;
				gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.ID);
				gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(item.GainGP);
				gSPacketIn.WriteBoolean(IsWin);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteBoolean(val: false);
			}
			SendToAll(gSPacketIn);
		}
		if (base.GameState != eGameState.GameOver)
		{
			return;
		}
		m_gameState = eGameState.ALLSessionStopped;
		try
		{
			m_gameAI.OnGameOverAllSession();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
		}
		List<Player> allFightPlayers2 = GetAllFightPlayers();
		GSPacketIn gSPacketIn2 = new GSPacketIn(91);
		gSPacketIn2.WriteByte(115);
		gSPacketIn2.WriteInt(base.PlayerCount);
		foreach (Player item2 in allFightPlayers2)
		{
			item2.CanTakeOut = 0;
			item2.PlayerDetail.OnGameOver(this, IsWin, item2.GainGP);
			gSPacketIn2.WriteInt(item2.PlayerDetail.PlayerCharacter.ID);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(item2.GainGP);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn2.WriteInt(0);
			}
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn2.WriteInt(0);
			}
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(0);
			gSPacketIn2.WriteInt(item2.TotalAllExperience);
			gSPacketIn2.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn2.WriteInt(0);
			}
			gSPacketIn2.WriteBoolean(IsWin);
		}
		gSPacketIn2.WriteInt(m_gameOverResources.Count);
		foreach (string gameOverResource in m_gameOverResources)
		{
			gSPacketIn2.WriteString(gameOverResource);
		}
		SendToAll(gSPacketIn2);
	}

	public bool IsShowLargeCards()
	{
		if (!IsWin)
		{
			return false;
		}
		if (Misssions.ContainsKey(1 + SessionId) && (m_info.ID == 5 || m_info.ID == 14))
		{
			return true;
		}
		return false;
	}

	public bool IsWorldCup()
	{
		return m_info.ID == 14;
	}

	public bool HasNextSession()
	{
		if (base.PlayerCount == 0 || !IsWin || base.RoomType == eRoomType.ConsortiaBoss || IsShowLargeCards())
		{
			return false;
		}
		if (Misssions.ContainsKey(1 + SessionId))
		{
			return true;
		}
		return false;
	}

	public void GameOverAllSession()
	{
		if (base.GameState != eGameState.GameOver)
		{
			return;
		}
		m_gameState = eGameState.ALLSessionStopped;
		try
		{
			m_gameAI.OnGameOverAllSession();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(115);
		int canTakeOut = 1;
		if (!IsWin)
		{
			canTakeOut = 0;
		}
		else
		{
			eRoomType roomType = base.RoomType;
			eRoomType eRoomType2 = roomType;
			if (eRoomType2 == eRoomType.Dungeon || eRoomType2 == eRoomType.SpecialActivityDungeon || eRoomType2 == eRoomType.Element_PVE)
			{
				canTakeOut = 2;
			}
		}
		gSPacketIn.WriteInt(base.PlayerCount);
		foreach (Player item in allFightPlayers)
		{
			item.CanTakeOut = canTakeOut;
			bool flag = Misssions.ContainsKey(1 + SessionId);
			if (!flag && IsWin && base.RoomType == eRoomType.Dungeon)
			{
				item.PlayerDetail.RemoveMissionEnergy(GameProperties.NormalDungeonCostEnergy);
			}
			else if (!flag && IsWin && base.RoomType == eRoomType.Element_PVE)
			{
				item.PlayerDetail.RemoveMissionEnergy(GameProperties.ElementDungeonCostEnergy);
			}
			if (!flag && m_hardLevel == eHardLevel.Nightmare && IsWin)
			{
				item.PlayerDetail.UpdateDungeonCount(m_info.ID);
			}
			item.PlayerDetail.OnGameOver(this, IsWin, item.GainGP);
			gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.GainGP);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.TotalAllExperience);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteBoolean(IsWin);
		}
		gSPacketIn.WriteInt(m_gameOverResources.Count);
		foreach (string gameOverResource in m_gameOverResources)
		{
			gSPacketIn.WriteString(gameOverResource);
		}
		SendToAll(gSPacketIn);
		if (IsShowLargeCards())
		{
			WaitTime(16000);
		}
		else
		{
			WaitTime(23000);
		}
		CanStopGame();
	}

	public void CanStopGame()
	{
		if (!IsWin)
		{
			if (base.GameType == eGameType.Dungeon && SessionId > 1)
			{
				ClearWaitTimer();
			}
		}
		else if (IsShowLargeCards())
		{
			WantTryAgain = 1;
		}
		SendTempStyle(0);
	}

	public void ShowLargeCard()
	{
		if (base.GameState != eGameState.ALLSessionStopped || !IsWin)
		{
			return;
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.CanTakeOut > 0)
			{
				item.HasPaymentTakeCard = true;
				int canTakeOut = item.CanTakeOut;
				for (int i = 0; i < canTakeOut; i++)
				{
					TakeCard(item);
				}
			}
		}
		SendShowCards();
	}

	public override void Stop()
	{
		if (base.GameState != eGameState.ALLSessionStopped)
		{
			return;
		}
		m_gameState = eGameState.Stopped;
		List<Player> allFightPlayers = GetAllFightPlayers();
		if (IsWin)
		{
			foreach (Player item in allFightPlayers)
			{
				if (item.CanTakeOut > 0)
				{
					item.HasPaymentTakeCard = true;
					int canTakeOut = item.CanTakeOut;
					for (int i = 0; i < canTakeOut; i++)
					{
						TakeCard(item);
					}
				}
			}
			if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon || base.RoomType == eRoomType.Element_PVE)
			{
				SendShowCards();
			}
			if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.Element_PVE)
			{
				foreach (Player item2 in allFightPlayers)
				{
					item2.PlayerDetail.SetPvePermission(m_info.ID, m_hardLevel);
				}
			}
		}
		bool flag = Misssions.ContainsKey(1 + SessionId);
		foreach (Player item3 in allFightPlayers)
		{
			item3.PlayerDetail.ResetRoom(IsWin, flag.ToString());
		}
		lock (m_players)
		{
			m_players.Clear();
		}
		OnGameStopped();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		foreach (Living living in m_livings)
		{
			living.Dispose();
		}
		try
		{
			m_missionAI.Dispose();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_missionAI.Dispose() error:{0}", arg);
		}
		try
		{
			m_gameAI.Dispose();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script m_gameAI.Dispose() error:{0}", arg2);
		}
	}

	public void DoOther()
	{
		try
		{
			m_missionAI.DoOther();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.DoOther() error:{0}", arg);
		}
	}

	internal void OnDied()
	{
		try
		{
			m_missionAI.OnDied();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.OnDied() error:{0}", arg);
		}
	}

	internal void OnShooted()
	{
		try
		{
			m_missionAI.OnShooted();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.OnShooted() error:{0}", arg);
		}
	}

	internal void OnMoving()
	{
		try
		{
			m_missionAI.OnMoving();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.OnMoving() error:{0}", arg);
		}
	}

	internal void OnTakeDamage()
	{
		try
		{
			m_missionAI.OnTakeDamage();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.OnTakeDamage() error:{0}", arg);
		}
	}

	private int CalculateExperience(Player p)
	{
		if (TotalKillCount == 0)
		{
			return 1;
		}
		double num = Math.Abs((double)p.Grade - TotalNpcGrade / (double)TotalKillCount);
		if (num >= 7.0)
		{
			return 1;
		}
		double num2 = 0.0;
		if (TotalKillCount > 0)
		{
			num2 += (double)p.TotalKill / (double)TotalKillCount * 0.4;
		}
		if (TotalHurt > 0)
		{
			num2 += (double)p.TotalHurt / (double)TotalHurt * 0.4;
		}
		if (p.IsLiving)
		{
			num2 += 0.4;
		}
		double num3 = 1.0;
		if (num >= 3.0 && num <= 4.0)
		{
			num3 = 0.7;
		}
		else if (num >= 5.0 && num <= 6.0)
		{
			num3 = 0.4;
		}
		double num4 = (0.9 + (double)(BeginPlayersCount - 1) * 0.4) / (double)base.PlayerCount;
		double num5 = 0.0;
		num5 = TotalNpcExperience * num2 * num3 * num4 * 0.0;
		num5 = ((num5 == 0.0) ? 1.0 : num5);
		return (int)num5;
	}

	private int CalculateScore(Player p)
	{
		int num = (200 - base.TurnIndex) * 5 + p.TotalKill * 5 + (int)((double)p.Blood / (double)p.MaxBlood) * 10;
		if (!IsWin)
		{
			num -= 400;
		}
		return num;
	}

	private int CalculateHitRate(int hitTargetCount, int shootCount)
	{
		double num = 0.0;
		if (shootCount > 0)
		{
			num = (double)hitTargetCount / (double)shootCount;
		}
		return (int)(num * 100.0);
	}

	public override void CheckState(int delay)
	{
		AddAction(new CheckPVEGameStateAction(delay));
	}

	public bool TakeBossCard(Player player)
	{
		int index = 0;
		for (int i = 0; i < BossCards.Length; i++)
		{
			if (Cards[i] == 0)
			{
				index = i;
				break;
			}
		}
		return TakeCard(player, index, isAuto: false);
	}

	public bool TakeBossCard(Player player, int index)
	{
		if (player.BossCardCount <= 0 || index < 0 || index > BossCards.Length || BossCards[index] > 0)
		{
			return false;
		}
		List<ItemInfo> info = null;
		int num = 0;
		int count = 0;
		int num2 = 0;
		int missionId = int.Parse(IsBossWar);
		DropInventory.BossDrop(missionId, ref info);
		if (info != null)
		{
			foreach (ItemInfo item in info)
			{
				if (item != null)
				{
					player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
					num = item.TemplateID;
					count = item.Count;
				}
			}
		}
		if (num == 0 && num2 > 0)
		{
			num = -100;
			count = num2;
		}
		player.BossCardCount--;
		BossCards[index] = 1;
		SendGamePlayerTakeCard(player, isAuto: true, index, num, count);
		return true;
	}

	public void SendMissionInfo(Player p)
	{
		if (m_missionInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(113);
			gSPacketIn.WriteInt(m_missionInfo.Id);
			gSPacketIn.WriteString(m_missionInfo.Name);
			gSPacketIn.WriteString(m_missionInfo.Success);
			gSPacketIn.WriteString(m_missionInfo.Failure);
			gSPacketIn.WriteString(m_missionInfo.Description);
			gSPacketIn.WriteString(m_missionInfo.Title);
			gSPacketIn.WriteInt(TotalMissionCount);
			gSPacketIn.WriteInt(SessionId);
			gSPacketIn.WriteInt(TotalTurn);
			gSPacketIn.WriteInt(TotalCount);
			gSPacketIn.WriteInt(Param1);
			gSPacketIn.WriteInt(Param2);
			gSPacketIn.WriteInt(WantTryAgain);
			gSPacketIn.WriteString(Pic);
			if (p != null)
			{
				p.PlayerDetail.SendTCP(gSPacketIn);
			}
			else
			{
				SendToAll(gSPacketIn);
			}
		}
	}

	public void SendUpdateUiData(Player p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(104);
		int val = 0;
		try
		{
			val = m_missionAI.UpdateUIData();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", $"m_missionAI.UpdateUIData()", arg);
		}
		gSPacketIn.WriteInt(base.TurnIndex);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(Param3);
		gSPacketIn.WriteInt(Param4);
		if (p != null)
		{
			p.PlayerDetail.SendTCP(gSPacketIn);
		}
		else
		{
			SendToAll(gSPacketIn);
		}
	}

	internal void SendShowCards()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(89);
		List<int> list = new List<int>();
		for (int i = 0; i < Cards.Length; i++)
		{
			if (Cards[i] == 0)
			{
				list.Add(i);
			}
		}
		int val = 0;
		int val2 = 0;
		gSPacketIn.WriteInt(list.Count);
		int id = m_missionInfo.Id;
		if (Misssions.ContainsKey(PreSessionId))
		{
			id = Misssions[PreSessionId].Id;
		}
		foreach (int item in list)
		{
			List<ItemInfo> list2 = DropInventory.CopySystemDrop(id, list.Count);
			if (list2 != null)
			{
				foreach (ItemInfo item2 in list2)
				{
					val = item2.TemplateID;
					val2 = item2.Count;
				}
			}
			gSPacketIn.WriteByte((byte)item);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(val2);
		}
		SendToAll(gSPacketIn);
	}

	public void SendGameObjectFocus(int type, string name, int delay, int finishTime)
	{
		Physics[] array = FindPhysicalObjByName(name);
		Physics[] array2 = array;
		Physics[] array3 = array2;
		foreach (Physics obj in array3)
		{
			AddAction(new FocusAction(obj, type, delay, finishTime));
		}
	}

	public void SendGameFocus(int x, int y, int type, int delay, int finishTime)
	{
		Createlayer(x, y, "focuslayer", "", "", 1, 0);
		SendGameObjectFocus(1, "focuslayer", delay, finishTime);
	}

	public void SendObjectFocus(Physics obj, int type, int delay, int finishTime)
	{
		AddAction(new FocusAction(obj, type, delay, finishTime));
	}

	public void SendFreeFocus(int x, int y, int type, int delay, int finishTime)
	{
		AddAction(new FocusFreeAction(x, y, type, delay, finishTime));
	}

	public void SendHideBlood(List<SimpleNpc> livings, int hide)
	{
		foreach (SimpleNpc living in livings)
		{
			SendLivingShowBlood(living, hide);
		}
	}

	public void SendHideBlood(Living living, int hide)
	{
		SendLivingShowBlood(living, hide);
	}

	public void SendLivingActionMapping(Living liv, string source, string value)
	{
		if (liv != null)
		{
			SendLivingActionMapping(liv.Id, source, value);
		}
	}

	public void SendPlayersPicture(Living living, int type, bool state)
	{
		SendPlayerPicture(living, type, state);
	}

	private void SendAddNewPlayer(Player p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(86);
		gSPacketIn.WriteInt(p.PlayerDetail.ZoneId);
		gSPacketIn.WriteString(p.PlayerDetail.ZoneName);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.ID);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.NickName);
		gSPacketIn.WriteBoolean(p.PlayerDetail.PlayerCharacter.isViewer);
		gSPacketIn.WriteByte(p.PlayerDetail.PlayerCharacter.typeVIP);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.VIPLevel);
		gSPacketIn.WriteBoolean(p.PlayerDetail.PlayerCharacter.Sex);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Hide);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.Style);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.Colors);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.Skin);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Grade);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Repute);
		gSPacketIn.WriteInt(p.PlayerDetail.MainWeapon.TemplateID);
		if (p.PlayerDetail.MainWeapon.TemplateID != 0)
		{
			gSPacketIn.WriteInt(p.PlayerDetail.MainWeapon.RefineryLevel);
			gSPacketIn.WriteString("");
			gSPacketIn.WriteDateTime(DateTime.Now);
		}
		gSPacketIn.WriteInt(p.PlayerDetail.SecondWeapon.TemplateID);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Nimbus);
		gSPacketIn.WriteBoolean(p.PlayerDetail.PlayerCharacter.IsShowConsortia);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.ConsortiaID);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.ConsortiaName);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.badgeID);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Win);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Total);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.FightPower);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.AcademyInfo.ApprenticeshipState);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.AcademyInfo.MasterID);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.AcademyInfo.MasterOrApprentices);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.AchievementPoint);
		gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.Honor);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.Offer);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.guardCoreID);
		gSPacketIn.WriteBoolean(p.PlayerDetail.PlayerCharacter.IsMarried);
		if (p.PlayerDetail.PlayerCharacter.IsMarried)
		{
			gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.SpouseID);
			gSPacketIn.WriteString(p.PlayerDetail.PlayerCharacter.SpouseName);
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt((p.PlayerDetail.Weapon3D != null) ? p.PlayerDetail.Weapon3D.TemplateID : 0);
		gSPacketIn.WriteInt(p.Team);
		gSPacketIn.WriteInt(p.Id);
		gSPacketIn.WriteInt(p.Blood);
		if (p.Pet == null)
		{
			gSPacketIn.WriteInt(0);
		}
		else
		{
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(p.Pet.Place);
			gSPacketIn.WriteInt(p.Pet.TemplateID);
			gSPacketIn.WriteInt(p.Pet.ID);
			gSPacketIn.WriteString(p.Pet.Name);
			gSPacketIn.WriteInt(p.Pet.UserID);
			gSPacketIn.WriteInt(p.Pet.Level);
			string[] array = p.Pet.SkillEquip.Split('|');
			gSPacketIn.WriteInt(array.Length);
			string[] array2 = array;
			foreach (string text in array2)
			{
				gSPacketIn.WriteInt(int.Parse(text.Split(',')[1]));
				gSPacketIn.WriteInt(int.Parse(text.Split(',')[0]));
			}
		}
		gSPacketIn.WriteInt(p.HorseSkillEquip.Length);
		int[] horseSkillEquip = p.HorseSkillEquip;
		foreach (int val in horseSkillEquip)
		{
			gSPacketIn.WriteInt(val);
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteBoolean(p.PlayerDetail.PlayerCharacter.IsAutoBot);
		gSPacketIn.WriteBoolean(p.PlayerDetail.PlayerCharacter.ringFlag);
		gSPacketIn.WriteInt(p.PlayerDetail.PlayerCharacter.loveBuffLevel);
		gSPacketIn.WriteInt(p.TurnNum);
		if (GameProperties.VERSION >= 11000)
		{
			if (p.PlayerDetail.PlayerCharacter.IsHideElf)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt((p.PlayerDetail.PlayerCharacter?.ElfInfo?.FirstOrDefault((Sys_Users_Elf e) => e.IsFight)?.TempID).GetValueOrDefault());
			}
		}
		gSPacketIn.WriteInt(p.Id);
		gSPacketIn.WriteInt(p.X);
		gSPacketIn.WriteInt(p.Y);
		gSPacketIn.WriteInt(p.Direction);
		gSPacketIn.WriteInt(p.Blood);
		gSPacketIn.WriteInt(p.MaxBlood);
		gSPacketIn.WriteInt(p.Team);
		gSPacketIn.WriteInt(p.PlayerDetail.MainWeapon.RefineryLevel);
		gSPacketIn.WriteInt(p.deputyWeaponCount);
		gSPacketIn.WriteInt(p.PowerRatio);
		gSPacketIn.WriteInt(p.Dander);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(p.PlayerDetail.FightBuffs.Count);
		foreach (BufferInfo fightBuff in p.PlayerDetail.FightBuffs)
		{
			gSPacketIn.WriteInt(fightBuff.Type);
			gSPacketIn.WriteInt(fightBuff.Value);
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteBoolean(p.IsFrost);
		gSPacketIn.WriteBoolean(p.IsHide);
		gSPacketIn.WriteBoolean(p.IsNoHole);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteInt(0);
		SendToAll(gSPacketIn, p.PlayerDetail);
	}

	private void SendCreateGameToSingle(PVEGame game, IGamePlayer gamePlayer)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(121);
		gSPacketIn.WriteInt(game.Map.Info.ID);
		gSPacketIn.WriteInt((byte)game.RoomType);
		gSPacketIn.WriteInt((byte)game.GameType);
		gSPacketIn.WriteInt(game.TimeType);
		List<Player> allFightPlayers = game.GetAllFightPlayers();
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			IGamePlayer playerDetail = item.PlayerDetail;
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.NickName);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.isViewer);
			gSPacketIn.WriteByte(playerDetail.PlayerCharacter.typeVIP);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.Sex);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Hide);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Style);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Colors);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Repute);
			if (playerDetail.MainWeapon == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				if (playerDetail.MainWeapon.GoldValidDate())
				{
					gSPacketIn.WriteInt(playerDetail.MainWeapon.GoldEquip.TemplateID);
				}
				else
				{
					gSPacketIn.WriteInt(playerDetail.MainWeapon.TemplateID);
				}
				gSPacketIn.WriteInt(playerDetail.MainWeapon.RefineryLevel);
				gSPacketIn.WriteString(playerDetail.MainWeapon.Template.Name);
				gSPacketIn.WriteDateTime(DateTime.MinValue);
			}
			gSPacketIn.WriteInt((playerDetail.SecondWeapon != null) ? playerDetail.SecondWeapon.TemplateID : 0);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.badgeID);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt((playerDetail.Weapon3D != null) ? playerDetail.Weapon3D.TemplateID : 0);
			}
			gSPacketIn.WriteInt(item.Team);
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.MaxBlood);
			gSPacketIn.WriteBoolean(item.Ready);
		}
		int sessionId = game.SessionId;
		MissionInfo missionInfo = game.Misssions[sessionId];
		gSPacketIn.WriteString(missionInfo.Name);
		gSPacketIn.WriteString($"show{sessionId}.jpg");
		gSPacketIn.WriteString(missionInfo.Success);
		gSPacketIn.WriteString(missionInfo.Failure);
		gSPacketIn.WriteString(missionInfo.Description);
		gSPacketIn.WriteInt(game.TotalMissionCount);
		gSPacketIn.WriteInt(sessionId);
		gamePlayer.SendTCP(gSPacketIn);
	}

	public void SendPlaySound(string playStr)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(63);
		gSPacketIn.WriteString(playStr);
		SendToAll(gSPacketIn);
	}

	public void SendLoadResource(List<LoadingFileInfo> loadingFileInfos)
	{
		if (loadingFileInfos == null || loadingFileInfos.Count <= 0)
		{
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(67);
		gSPacketIn.WriteInt(loadingFileInfos.Count);
		foreach (LoadingFileInfo loadingFileInfo in loadingFileInfos)
		{
			gSPacketIn.WriteInt(loadingFileInfo.Type);
			gSPacketIn.WriteString(loadingFileInfo.Path);
			gSPacketIn.WriteString(loadingFileInfo.ClassName);
		}
		SendToAll(gSPacketIn);
	}

	public override void MinusDelays(int lowestDelay)
	{
		m_pveGameDelay -= lowestDelay;
		base.MinusDelays(lowestDelay);
	}

	public void Print(string str)
	{
		Console.WriteLine(str);
	}

	public void sendTransmissionGate()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(137);
		gSPacketIn.WriteBoolean(val: true);
		base.SendToAll(gSPacketIn);
	}

	public LivingConfig BaseLivingConfig()
	{
		return new LivingConfig
		{
			isBotom = 1,
			isShowBlood = true,
			HasTurn = true,
			isShowSmallMapPoint = true,
			CanTakeDamage = true,
			CanCountKill = true,
			ReduceBloodStart = 1
		};
	}
}

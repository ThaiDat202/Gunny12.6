using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Reflection;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class PVPGame : BaseGame
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private List<Player> m_redTeam;

	private float m_redAvgLevel;

	private List<Player> m_blueTeam;

	private float m_blueAvgLevel;

	private int BeginPlayerCount;

	private int m_redTeamIndex;

	private int m_blueTeamIndex;

	private string teamAStr;

	private string teamBStr;

	private DateTime beginTime;

	private int typeEnterMode;

	private string m_continuousNick;

	public Player CurrentPlayer => m_currentLiving as Player;

	public string ContinuousRunningPlayer
	{
		get
		{
			return m_continuousNick;
		}
		set
		{
			m_continuousNick = value;
		}
	}

	public PVPGame(int id, int roomId, List<IGamePlayer> red1, List<IGamePlayer> blue1, List<IGamePlayer> red2, List<IGamePlayer> blue2, List<IGamePlayer> red3, List<IGamePlayer> blue3, List<IGamePlayer> red4, List<IGamePlayer> blue4, Map map, eRoomType roomType, eGameType gameType, int timeType)
		: base(id, roomId, map, roomType, gameType, timeType)
	{
		m_redTeam = new List<Player>();
		m_blueTeam = new List<Player>();
		StringBuilder stringBuilder = new StringBuilder();
		m_redAvgLevel = 0f;
		m_redTeamIndex = 1;
		foreach (IGamePlayer item in red1)
		{
			Player player = new Player(item, PhysicalId++, this, m_redTeamIndex, item.PlayerCharacter.hp);
			stringBuilder.Append(item.PlayerCharacter.ID).Append(",").Append("1");
			player.Reset();
			player.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
			AddPlayer(player);
			m_redTeam.Add(player);
			m_redAvgLevel += item.PlayerCharacter.Grade;
		}
		if (red2 != null)
		{
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				m_redTeamIndex += 2;
			}
			foreach (IGamePlayer item2 in red2)
			{
				Player player2 = new Player(item2, PhysicalId++, this, m_redTeamIndex, item2.PlayerCharacter.hp);
				stringBuilder.Append(item2.PlayerCharacter.ID).Append(",").Append("1");
				player2.Reset();
				player2.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player2);
				m_redTeam.Add(player2);
				m_redAvgLevel += item2.PlayerCharacter.Grade;
			}
		}
		if (red3 != null)
		{
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				m_redTeamIndex += 2;
			}
			foreach (IGamePlayer item3 in red3)
			{
				Player player3 = new Player(item3, PhysicalId++, this, m_redTeamIndex, item3.PlayerCharacter.hp);
				stringBuilder.Append(item3.PlayerCharacter.ID).Append(",").Append("1");
				player3.Reset();
				player3.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player3);
				m_redTeam.Add(player3);
				m_redAvgLevel += item3.PlayerCharacter.Grade;
			}
		}
		if (red4 != null)
		{
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				m_redTeamIndex += 2;
			}
			foreach (IGamePlayer item4 in red4)
			{
				Player player4 = new Player(item4, PhysicalId++, this, m_redTeamIndex, item4.PlayerCharacter.hp);
				stringBuilder.Append(item4.PlayerCharacter.ID).Append(",").Append("1");
				player4.Reset();
				player4.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player4);
				m_redTeam.Add(player4);
				m_redAvgLevel += item4.PlayerCharacter.Grade;
			}
		}
		m_redAvgLevel /= m_redTeam.Count;
		teamAStr = stringBuilder.ToString();
		m_blueTeamIndex = 2;
		StringBuilder stringBuilder2 = new StringBuilder();
		m_blueAvgLevel = 0f;
		foreach (IGamePlayer item5 in blue1)
		{
			Player player5 = new Player(item5, PhysicalId++, this, m_blueTeamIndex, item5.PlayerCharacter.hp);
			stringBuilder2.Append(item5.PlayerCharacter.ID).Append(",").Append("2");
			player5.Reset();
			player5.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
			AddPlayer(player5);
			m_blueTeam.Add(player5);
			m_blueAvgLevel += item5.PlayerCharacter.Grade;
		}
		if (blue2 != null)
		{
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				m_blueTeamIndex += 2;
			}
			foreach (IGamePlayer item6 in blue2)
			{
				Player player6 = new Player(item6, PhysicalId++, this, m_blueTeamIndex, item6.PlayerCharacter.hp);
				stringBuilder2.Append(item6.PlayerCharacter.ID).Append(",").Append("2");
				player6.Reset();
				player6.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player6);
				m_blueTeam.Add(player6);
				m_blueAvgLevel += item6.PlayerCharacter.Grade;
			}
		}
		if (blue3 != null)
		{
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				m_blueTeamIndex += 2;
			}
			foreach (IGamePlayer item7 in blue3)
			{
				Player player7 = new Player(item7, PhysicalId++, this, m_blueTeamIndex, item7.PlayerCharacter.hp);
				stringBuilder2.Append(item7.PlayerCharacter.ID).Append(",").Append("2");
				player7.Reset();
				player7.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player7);
				m_blueTeam.Add(player7);
				m_blueAvgLevel += item7.PlayerCharacter.Grade;
			}
		}
		if (blue4 != null)
		{
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				m_blueTeamIndex += 2;
			}
			foreach (IGamePlayer item8 in blue4)
			{
				Player player8 = new Player(item8, PhysicalId++, this, m_blueTeamIndex, item8.PlayerCharacter.hp);
				stringBuilder2.Append(item8.PlayerCharacter.ID).Append(",").Append("2");
				player8.Reset();
				player8.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player8);
				m_blueTeam.Add(player8);
				m_blueAvgLevel += item8.PlayerCharacter.Grade;
			}
		}
		m_blueAvgLevel /= blue1.Count;
		teamBStr = stringBuilder2.ToString();
		BeginPlayerCount = m_redTeam.Count + m_blueTeam.Count;
		beginTime = DateTime.Now;
		typeEnterMode = red1.Count;
	}

	public void Prepare()
	{
		if (base.GameState == eGameState.Inited)
		{
			SendCreateGame();
			m_gameState = eGameState.Prepared;
			CheckState(0);
		}
	}

	public void StartLoading()
	{
		if (base.GameState != eGameState.Prepared)
		{
			return;
		}
		if (base.RoomType == eRoomType.FightFootballTime)
		{
			LoadFightFootballResources();
		}
		ClearWaitTimer();
		SendStartLoading(80);
		VaneLoading(null);
		SendTempStyle();
		if (base.RoomType == eRoomType.Encounter)
		{
			List<Player> allFightPlayers = GetAllFightPlayers();
			foreach (Player item in allFightPlayers)
			{
				SendSelectObject(item.Id);
			}
		}
		AddAction(new WaitPlayerLoadingAction(this, 61000));
		m_gameState = eGameState.Loading;
	}

	public void UpdateAllTarget()
	{
		if (base.RoomType != eRoomType.SurvivalMode)
		{
			return;
		}
		List<Player> allLivingPlayers = GetAllLivingPlayers();
		List<int> list = new List<int>();
		for (int i = 0; i < allLivingPlayers.Count; i++)
		{
			List<Player> allTarget = GetAllTarget(allLivingPlayers[i]);
			foreach (Player item in allTarget)
			{
				if (!list.Contains(item.Id))
				{
					UpdateTarget(allLivingPlayers[i].Id, item.Id);
					SendPlayerTarget(allLivingPlayers[i]);
					list.Add(item.Id);
					break;
				}
			}
		}
	}

	public void UpdateNextTarget()
	{
		if (base.RoomType != eRoomType.SurvivalMode)
		{
			return;
		}
		List<Player> allLivingPlayers = GetAllLivingPlayers();
		List<int> list = new List<int>();
		for (int i = 0; i < allLivingPlayers.Count; i++)
		{
			if (IsTargetLife(allLivingPlayers[i].targetId))
			{
				continue;
			}
			List<Player> allTarget = GetAllTarget(allLivingPlayers[i]);
			foreach (Player item in allTarget)
			{
				if (!list.Contains(item.Id))
				{
					UpdateTarget(allLivingPlayers[i].Id, item.Id);
					SendPlayerTarget(allLivingPlayers[i]);
					list.Add(item.Id);
					break;
				}
			}
		}
	}

	internal void SendPlayerTarget(Player local)
	{
		List<Player> list = new List<Player>();
		list.Add(local);
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(152);
		gSPacketIn.WriteInt(list.Count);
		foreach (Player item in list)
		{
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.targetId);
			gSPacketIn.WriteInt(0);
		}
		local.PlayerDetail.SendTCP(gSPacketIn);
	}

	public bool HasChangeStyle()
	{
		eRoomType roomType = base.RoomType;
		eRoomType eRoomType2 = roomType;
		if (eRoomType2 == eRoomType.FightFootballTime)
		{
			return true;
		}
		return false;
	}

	internal void SendTempStyle()
	{
		SendTempStyle(base.RoomType);
	}

	internal void SendTempStyle(eRoomType skin)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(134);
		List<Player> allFightPlayers = GetAllFightPlayers();
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			IGamePlayer playerDetail = item.PlayerDetail;
			string style = playerDetail.PlayerCharacter.Style;
			if (skin == eRoomType.FightFootballTime)
			{
				gSPacketIn.WriteString(playerDetail.GetFightFootballStyle(item.Team));
			}
			else
			{
				gSPacketIn.WriteString(style);
			}
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

	public void LoadFightFootballResources()
	{
		int[] array = new int[5] { 10008, 10005, 10006, 10009, 10007 };
		int[] array2 = array;
		foreach (int id in array2)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(id);
			if (npcInfoById == null)
			{
				log.Error("LoadResources npcInfo resoure is not exits");
			}
			else
			{
				AddLoadingFile(2, npcInfoById.ResourcesPath, npcInfoById.ModelID);
			}
		}
		AddLoadingFile(1, "bombs/24.swf", "tank.resource.bombs.Bomb24");
	}

	public void StartGame()
	{
		if (base.GameState != eGameState.Loading)
		{
			return;
		}
		m_gameState = eGameState.Playing;
		ClearWaitTimer();
		SendSyncLifeTime();
		List<Player> allFightPlayers = GetAllFightPlayers();
		MapPoint mapRandomPos = MapMgr.GetMapRandomPos(m_map.Info.ID);
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(99);
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			item.Reset();
			Point playerPoint = GetPlayerPoint(mapRandomPos, item.Team);
			item.SetXY(playerPoint);
			m_map.AddPhysical(item);
			item.StartMoving();
			item.StartGame();
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
				gSPacketIn.WriteInt(-1);
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
            AddLogIp(item.Id, item.PlayerDetail.TcpEndPoint());
			item.OnGameStarted();
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteDateTime(beginTime);
		if (GameProperties.VERSION >= 8100)
		{
			gSPacketIn.WriteInt(0);
		}
		SendToAll(gSPacketIn);
		WaitTime(allFightPlayers.Count * 1000);
		OnGameStarted();
	}

	public void CreateNpc(int npcId, int x, int y, int type, int direction)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleNpc simpleNpc = new SimpleNpc(PhysicalId++, this, npcInfoById, type, direction, "");
		simpleNpc.Reset();
		simpleNpc.SetXY(x, y);
		AddLiving(simpleNpc);
		simpleNpc.StartMoving();
	}

	public void CreateNpc()
	{
		int[] array = new int[5] { 10008, 10005, 10006, 10009, 10007 };
		int[] array2 = new int[3] { 350, 500, 680 };
		ThreadSafeRandom.ShufferStatic(array2);
		ThreadSafeRandom.ShufferStatic(array);
		ClearAllNpc();
		int num = array2[base.Random.Next(array2.Length)];
		foreach (int npcId in array)
		{
			CreateNpc(npcId, num, 259, 1, -1);
			num += 210;
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
		UpdateNextTarget();
		base.m_turnIndex++;
		List<SimpleBox> newBoxes = CreateBox();
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item in allPhysicalSafe)
		{
			item.PrepareNewTurn();
		}
		LastTurnLiving = m_currentLiving;
		if (base.RoomType == eRoomType.FightFootballTime)
		{
			CreateNpc();
			m_currentLiving = FindNextTurnedFightFootball();
		}
		else
		{
			m_currentLiving = FindNextTurnedLiving(isPvP: true);
		}
		if (m_currentLiving.VaneOpen)
		{
			UpdateWind(GetNextWind(), GetNextWindRate(), sendToClient: false);
		}
		if (m_currentLiving != LastTurnLiving && LastTurnLiving is Player && (LastTurnLiving as Player).IsAddTurn)
		{
			m_currentLiving = LastTurnLiving;
			(LastTurnLiving as Player).IsAddTurn = false;
		}
		if (m_currentLiving is Player && (m_currentLiving as Player).PlayerDetail.PlayerCharacter.NickName == ContinuousRunningPlayer)
		{
			foreach (Player allFightPlayer in GetAllFightPlayers())
			{
				allFightPlayer.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("Game.Logic.ContinuousRunning", ContinuousRunningPlayer));
			}
		}
		MinusDelays(m_currentLiving.Delay);
		m_currentLiving.PrepareSelfTurn();
		if (!base.CurrentLiving.IsFrost && m_currentLiving.IsLiving)
		{
			m_currentLiving.StartAttacking();
			SendGameNextTurn(m_currentLiving, this, newBoxes);
			if (m_currentLiving.IsAttacking)
			{
				AddAction(new WaitLivingAttackingAction(m_currentLiving, base.m_turnIndex, (getTurnTime() + 20) * 1000));
			}
			if (m_currentLiving is Player && EntertainmentMode())
			{
				(m_currentLiving as Player).PlayerDetail.CreateRandomEnterModeItem();
			}
		}
		if (m_currentLiving is Player && FindNextTurnedLiving(isPvP: true) is Player)
		{
			ContinuousRunningPlayer = (m_currentLiving as Player).PlayerDetail.PlayerCharacter.NickName;
		}
		OnBeginNewTurn();
	}

	public bool EntertainmentMode()
	{
		if (base.RoomType == eRoomType.Entertainment || base.RoomType == eRoomType.EntertainmentPK)
		{
			return true;
		}
		return false;
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
		if (player.CanTakeOut == 0 || index < 0 || index > Cards.Length || player.FinishTakeCard || Cards[index] > 0)
		{
			return false;
		}
		player.CanTakeOut--;
		int templateID = 0;
		int count = 0;
		List<ItemInfo> info = null;
		if (DropInventory.CardDrop(base.RoomType, ref info))
		{
			if (info != null)
			{
				foreach (ItemInfo item in info)
				{
					templateID = item.TemplateID;
					count = item.Count;
					player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.BatleTypeGet);
				}
				if (info.Count == 0)
				{
					log.ErrorFormat("Have not DropItem for RoomType.{0}", base.RoomType);
				}
			}
		}
		else
		{
			log.ErrorFormat("Have not DropCondition for RoomType.{0}", base.RoomType);
		}
		if (player.CanTakeOut == 0)
		{
			player.FinishTakeCard = true;
		}
		Cards[index] = 1;
		SendGamePlayerTakeCard(player, isAuto, index, templateID, count);
		return true;
	}

	private int CalculateExperience(Player player, int winTeam, ref int reward)
	{
		int num = 1;
		if (m_roomType == eRoomType.Match || base.RoomType == eRoomType.FightGround)
		{
			float num2 = ((player.Team == 1) ? m_blueAvgLevel : m_redAvgLevel);
			float num3 = ((player.Team == 1) ? m_blueTeam.Count : m_redTeam.Count);
			Math.Abs(num2 - (float)player.PlayerDetail.PlayerCharacter.Grade);
			if (player.TotalHurt == 0)
			{
				if (((double)(num2 - m_blueAvgLevel) < 5.0 && (double)(num2 - m_redAvgLevel) < 5.0) || TotalHurt <= 0)
				{
					return 1;
				}
				SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward"), null, 2);
				reward = 200;
				return 201;
			}
			float num4 = ((player.Team == winTeam) ? 2f : 0f);
			double num5 = 0.2;
			player.TotalShootCount = ((player.TotalShootCount == 0) ? 1 : player.TotalShootCount);
			if (player.TotalShootCount < player.TotalHitTargetCount)
			{
				player.TotalShootCount = player.TotalHitTargetCount;
			}
			int num6 = ((player.Team == 1) ? ((int)((double)((float)m_blueTeam.Count * m_blueAvgLevel) * 300.0)) : ((int)((double)(m_redAvgLevel * (float)m_redTeam.Count) * 300.0)));
			int num7 = ((player.TotalHurt > num6) ? num6 : player.TotalHurt);
			Math.Ceiling(((double)num4 + (double)num7 * (0.019 + num5) + (double)player.TotalKill * 0.5 + (double)(player.TotalHitTargetCount / player.TotalShootCount * 2)) * (double)num2 * (0.9 + ((double)num3 - 1.0) * 0.3));
			int num8 = 0;
			if (((double)(num2 - m_blueAvgLevel) >= 5.0 || (double)(num2 - m_redAvgLevel) >= 5.0) && TotalHurt > 0)
			{
				SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward"), null, 2);
				reward = 200;
				num8 = num8;
			}
			num = GainCoupleGp(player, num8);
			if (num > 100000)
			{
				num = 100000;
			}
		}
		if (m_roomType == eRoomType.FightFootballTime)
		{
			int num9 = 0;
			foreach (int item in player.ScoreArr)
			{
				num9 += item;
			}
			num = num9 * 50;
		}
		return (num < 1) ? 1 : num;
	}

	public int GainCoupleGp(Player player, int gp)
	{
		Player[] sameTeamPlayer = GetSameTeamPlayer(player);
		foreach (Player player2 in sameTeamPlayer)
		{
			if (player2.PlayerDetail.PlayerCharacter.SpouseID == player.PlayerDetail.PlayerCharacter.SpouseID)
			{
				return (int)((double)gp * 1.2);
			}
		}
		return gp;
	}

	public Player[] GetSameTeamPlayer(Player player)
	{
		List<Player> list = new List<Player>();
		foreach (Player allFightPlayer in GetAllFightPlayers())
		{
			if (allFightPlayer != player && allFightPlayer.Team == player.Team)
			{
				list.Add(allFightPlayer);
			}
		}
		return list.ToArray();
	}

	public int AddValue(BattleBonusInfo bonus)
	{
		if (bonus.DisableRandom)
		{
			return bonus.MaxValue;
		}
		int num = base.Random.Next(bonus.MinValue, bonus.MaxValue);
		return num * bonus.Rate / 10;
	}

	public ItemInfo AddItems(BattleBonusInfo bonus)
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(bonus.TemplateID);
		if (itemTemplateInfo != null)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, bonus.MaxValue, 105);
			itemInfo.IsBinds = bonus.IsBinds;
			itemInfo.ValidDate = bonus.ValidDate;
			itemInfo.Count = bonus.MaxValue;
			itemInfo.StrengthenLevel = bonus.StrengthenLevel;
			itemInfo.AttackCompose = bonus.AttackCompose;
			itemInfo.DefendCompose = bonus.DefendCompose;
			itemInfo.AgilityCompose = bonus.AgilityCompose;
			itemInfo.LuckCompose = bonus.LuckCompose;
			itemInfo.MagicAttack = bonus.MagicAttack;
			itemInfo.MagicDefence = bonus.MagicDefence;
			if (bonus.goldValidate > 0)
			{
				itemInfo.goldBeginTime = DateTime.Now;
				itemInfo.goldValidDate = bonus.goldValidate;
			}
			return itemInfo;
		}
		return null;
	}

	public bool CheckIp(List<Player> players, Player self)
	{
		string text = self.PlayerDetail.TcpEndPoint();
		foreach (int key in m_logStartIps.Keys)
		{
			if (key != self.Id && m_logStartIps[key] == text)
			{
				return true;
			}
		}
		return false;
	}

	public int GetDragonBoatScrore(List<Player> players, int winTeam)
	{
		int num = 0;
		foreach (Player player in players)
		{
			if (player.Team == winTeam)
			{
				num++;
			}
		}
		return num switch
		{
			2 => 2, 
			3 => 3, 
			4 => 4, 
			_ => 1, 
		};
	}

	private int GetWinTeam(List<Player> players)
	{
		int num = -1;
		if (m_roomType == eRoomType.BombKing || m_roomType == eRoomType.ConsortiaBattle)
		{
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			int num4 = -1;
			int num5 = 0;
			foreach (Player player in players)
			{
				if (!player.IsLiving)
				{
					flag = true;
					break;
				}
				if (player.Team == 1)
				{
					num2 += player.TotalDamagePlayer;
				}
				else
				{
					num3 += player.TotalDamagePlayer;
					num4 = player.Team;
				}
				if (player.TotalDamagePlayer <= 0)
				{
					num5++;
				}
			}
			if (num5 >= players.Count && m_roomType == eRoomType.ConsortiaBattle)
			{
				return -1;
			}
			if (!flag)
			{
				return (num2 > num3) ? 1 : num4;
			}
		}
		foreach (Player player2 in players)
		{
			if (player2.IsLiving)
			{
				num = player2.Team;
				break;
			}
		}
		if (num == -1)
		{
			if (CurrentPlayer != null)
			{
				return CurrentPlayer.Team;
			}
			if (base.CurrentLiving != null)
			{
				num = base.CurrentLiving.Team;
			}
		}
		return num;
	}

	public void GameOver()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		m_gameState = eGameState.GameOver;
		ClearWaitTimer();
		CurrentTurnTotalDamage = 0;
		List<Player> allFightPlayers = GetAllFightPlayers();
		int winTeam = GetWinTeam(allFightPlayers);
		int num = 0;
		int num2 = 0;
		int num3 = CalculateGuildMatchResult(allFightPlayers, winTeam);
		if (base.RoomType == eRoomType.Match)
		{
			if (base.GameType == eGameType.Guild)
			{
				num2 = 10;
				num = -10;
				num2 += allFightPlayers.Count / 2;
				num += (int)Math.Round((double)(allFightPlayers.Count / 2) * 0.5);
			}
			else
			{
				num2 = 3;
				num = -3;
			}
		}
		if (base.RoomType == eRoomType.TEAM_ROOM)
		{
			PlayerData playerData = new PlayerData();
			PlayerData playerData2 = new PlayerData();
			int count = GetAllTeamPlayerDies(m_blueTeam[0]).Count;
			int count2 = GetAllTeamPlayerDies(m_redTeam[0]).Count;
			int num4 = ((m_blueTeam[0].Team == winTeam) ? 10 : 5);
			List<string> list = new List<string>();
			foreach (Player item in m_blueTeam)
			{
				list.Add(num4 + "," + item.PlayerDetail.PlayerCharacter.Grade + "," + item.PlayerDetail.PlayerCharacter.NickName);
			}
			int num5 = ((m_redTeam[0].Team == winTeam) ? 10 : 5);
			List<string> list2 = new List<string>();
			foreach (Player item2 in m_redTeam)
			{
				list2.Add(num5 + "," + item2.PlayerDetail.PlayerCharacter.Grade + "," + item2.PlayerDetail.PlayerCharacter.NickName);
			}
			Sys_Users_TeamRecord sys_Users_TeamRecord = new Sys_Users_TeamRecord();
			sys_Users_TeamRecord.teamID = m_blueTeam[0].PlayerDetail.PlayerCharacter.Team.teamID;
			sys_Users_TeamRecord.isWin = ((m_blueTeam[0].Team == winTeam) ? true : false);
			sys_Users_TeamRecord.active = ((m_blueTeam[0].Team == winTeam) ? 10 : 5) * m_blueTeam.Count;
			sys_Users_TeamRecord.date = DateTime.Now;
			sys_Users_TeamRecord.teamName = m_blueTeam[0].PlayerDetail.PlayerCharacter.Team.teamName;
			sys_Users_TeamRecord.teamZone = m_blueTeam[0].PlayerDetail.ZoneName;
			sys_Users_TeamRecord.teamKill = count;
			sys_Users_TeamRecord.teamSurvival = m_blueTeam.Count - count;
			sys_Users_TeamRecord.teamMemberInfo = string.Join("|", list);
			sys_Users_TeamRecord.enemyName = m_redTeam[0].PlayerDetail.PlayerCharacter.Team.teamName;
			sys_Users_TeamRecord.enemyZone = m_redTeam[0].PlayerDetail.ZoneName;
			sys_Users_TeamRecord.enemyKill = count2;
			sys_Users_TeamRecord.enemySurvival = m_redTeam.Count - count2;
			sys_Users_TeamRecord.enemyMemberInfo = string.Join("|", list2);
			Sys_Users_TeamRecord sys_Users_TeamRecord2 = new Sys_Users_TeamRecord();
			sys_Users_TeamRecord2.teamID = m_redTeam[0].PlayerDetail.PlayerCharacter.Team.teamID;
			sys_Users_TeamRecord2.isWin = ((m_redTeam[0].Team == winTeam) ? true : false);
			sys_Users_TeamRecord2.active = ((m_redTeam[0].Team == winTeam) ? 10 : 5) * m_redTeam.Count;
			sys_Users_TeamRecord2.date = DateTime.Now;
			sys_Users_TeamRecord2.teamName = m_redTeam[0].PlayerDetail.PlayerCharacter.Team.teamName;
			sys_Users_TeamRecord2.teamZone = m_redTeam[0].PlayerDetail.ZoneName;
			sys_Users_TeamRecord2.teamKill = count2;
			sys_Users_TeamRecord2.teamSurvival = m_redTeam.Count - count2;
			sys_Users_TeamRecord2.teamMemberInfo = string.Join("|", list2);
			sys_Users_TeamRecord2.enemyName = m_blueTeam[0].PlayerDetail.PlayerCharacter.Team.teamName;
			sys_Users_TeamRecord2.enemyZone = m_blueTeam[0].PlayerDetail.ZoneName;
			sys_Users_TeamRecord2.enemyKill = count;
			sys_Users_TeamRecord2.enemySurvival = m_blueTeam.Count - count;
			sys_Users_TeamRecord2.enemyMemberInfo = string.Join("|", list);
			playerData.Sys_Users_TeamRecord.Add(sys_Users_TeamRecord);
			Sys_Users_Team sys_Users_Team = playerData.Sys_Users_Team.Find(m_blueTeam[0].PlayerDetail.PlayerCharacter.Team.teamID);
			sys_Users_Team.teamTotalTime++;
			sys_Users_Team.teamWinTime += ((m_blueTeam[0].Team == winTeam) ? 1 : 0);
			playerData.Entry(sys_Users_Team).State = EntityState.Modified;
			playerData.SaveChanges();
			playerData2.Sys_Users_TeamRecord.Add(sys_Users_TeamRecord2);
			Sys_Users_Team sys_Users_Team2 = playerData2.Sys_Users_Team.Find(m_blueTeam[0].PlayerDetail.PlayerCharacter.Team.teamID);
			sys_Users_Team2.teamTotalTime++;
			sys_Users_Team2.teamWinTime += ((m_redTeam[0].Team == winTeam) ? 1 : 0);
			playerData2.Entry(sys_Users_Team2).State = EntityState.Modified;
			playerData2.SaveChanges();
		}
		int num6 = 0;
		int num7 = 0;
		if (base.RoomType != eRoomType.ConsortiaBattle)
		{
			foreach (Player item3 in allFightPlayers)
			{
				if (item3.TotalHurt > 0)
				{
					if (item3.Team == 1)
					{
						num7 = 1;
					}
					else
					{
						num6 = 1;
					}
				}
			}
		}
		int num8 = ((winTeam == -1) ? winTeam : 0);
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(100);
		gSPacketIn.WriteInt(num8);
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item4 in allFightPlayers)
		{
			float num9 = ((item4.Team == 1) ? m_blueAvgLevel : m_redAvgLevel);
			float num10 = ((item4.Team == 1) ? m_blueTeam.Count : m_redTeam.Count);
			float num11 = Math.Abs(num9 - (float)item4.PlayerDetail.PlayerCharacter.Grade);
			float num12 = ((item4.Team == winTeam) ? 2 : 0);
			int num13 = 0;
			int reward = 0;
			int num14 = ((item4.TotalShootCount == 0) ? 1 : item4.TotalShootCount);
			int num15 = 0;
			if (base.RoomType == eRoomType.FightGround || m_roomType == eRoomType.Match || num11 < 5f)
			{
				num15 = CalculateExperience(item4, winTeam, ref reward);
			}
			string translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.NoDame");
			if (base.RoomType == eRoomType.SurvivalMode)
			{
				if (item4.TotalHurt > 0)
				{
					item4.CanTakeOut = 1;
				}
				if (item4.TotalKill > 0)
				{
					item4.CanTakeOut += item4.TotalKill;
				}
				if (item4.IsLiving)
				{
					item4.CanTakeOut += 2;
				}
				if (item4.TotalHurt == 0)
				{
					item4.CanTakeOut = 0;
				}
			}
			else
			{
				item4.CanTakeOut = ((item4.Team == 1) ? num7 : num6);
			}
			if (item4.PlayerDetail.PlayerCharacter.IsAutoBot)
			{
				item4.CanTakeOut = 0;
			}
			num3 += item4.GainOffer;
			bool flag = false;
			flag = ((base.RoomType != eRoomType.FightFootballTime) ? (item4.Team == winTeam) : ((item4.Team == 1 && blueScore > redScore) || (item4.Team == 2 && blueScore < redScore)));
			item4.PlayerDetail.FootballTakeOut(flag);
			if (m_roomType == eRoomType.Match || base.RoomType == eRoomType.FightGround || base.RoomType == eRoomType.SurvivalMode || base.RoomType == eRoomType.Entertainment || base.RoomType == eRoomType.EntertainmentPK)
			{
				List<BattleBonusInfo> exitBatleBonus = BattleBonusMgr.GetExitBatleBonus();
				List<ItemInfo> list3 = new List<ItemInfo>();
				foreach (BattleBonusInfo item5 in exitBatleBonus)
				{
					switch (item5.Type)
					{
					case -100:
						if (flag)
						{
							int num19 = AddValue(item5);
							item4.PlayerDetail.AddGold(num19);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num19));
						}
						break;
					case -1100:
						if (flag)
						{
							int num28 = AddValue(item5);
							item4.PlayerDetail.AddGiftToken(num28);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num28));
						}
						break;
					case -200:
						if (flag)
						{
							int num27 = AddValue(item5);
							item4.PlayerDetail.LogAddMoney(AddMoneyType.Battle_Bonus, AddMoneyType.Battle_Bonus_Add, item4.PlayerDetail.PlayerCharacter.ID, num27, item4.PlayerDetail.PlayerCharacter.Money);
							item4.PlayerDetail.AddCupomPVP(num27);
							item4.PlayerDetail.UpdateUserMoney(num27, 2);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num27));
						}
						break;
					case -300:
						if (flag)
						{
							int num24 = AddValue(item5);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num24));
							item4.PlayerDetail.AddMedal(num24);
						}
						break;
					case 1:
						if (item5.ValiddateHour() && flag)
						{
							int num17 = AddValue(item5);
							item4.PlayerDetail.LogAddMoney(AddMoneyType.Battle_Bonus, AddMoneyType.Battle_Bonus_Add, item4.PlayerDetail.PlayerCharacter.ID, num17, item4.PlayerDetail.PlayerCharacter.Money);
							item4.PlayerDetail.AddCupomPVP(num17);
							item4.PlayerDetail.UpdateUserMoney(num17, 2);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num17));
						}
						break;
					case 2:
						if (item5.Random > base.Random.Next(1000) && flag)
						{
							ItemInfo itemInfo = AddItems(item5);
							if (itemInfo != null)
							{
								list3.Add(itemInfo);
							}
						}
						break;
					case 3:
						if (item5.Random > base.Random.Next(1000) && flag)
						{
							int num21 = AddValue(item5);
							item4.PlayerDetail.LogAddMoney(AddMoneyType.Battle_Bonus, AddMoneyType.Battle_Bonus_Add, item4.PlayerDetail.PlayerCharacter.ID, num21, item4.PlayerDetail.PlayerCharacter.Money);
							item4.PlayerDetail.AddCupomPVP(num21);
							item4.PlayerDetail.UpdateUserMoney(num21, 2);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num21));
						}
						break;
					case 4:
						if (flag)
						{
							int num32 = AddValue(item5);
							num13 += num32;
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num32));
						}
						break;
					case 5:
						if (item5.ValiddateHour() && flag)
						{
							int num30 = AddValue(item5);
							num13 += num30;
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num30));
						}
						break;
					case 105:
					{
						int restCount = item4.PlayerDetail.MatchInfo.restCount;
						bool flag2 = item4.PlayerDetail.PlayerCharacter.Grade >= 20;
						if (base.PlayerCount == 4 && flag2 && restCount > 0)
						{
							int num22 = item5.MinValue;
							string translation2 = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg7", num22);
							if (flag)
							{
								num22 = item5.MaxValue;
								translation2 = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg6", num22);
							}
							item4.PlayerDetail.SendMessage(translation2);
							item4.PlayerDetail.AddLeagueMoney(num22);
							item4.PlayerDetail.UpdateRestCount();
						}
						break;
					}
					case 106:
					{
						int num31 = item5.MinValue;
						if (flag)
						{
							num31 = item5.MaxValue * 2;
						}
						if (item4.PlayerDetail.DragonBoatOpen() && item4.PlayerDetail.IsCrossZone)
						{
							num31 *= GetDragonBoatScrore(allFightPlayers, winTeam);
							item4.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("PVPGame.SendGameOVer.DragonBoatOpen", num31));
							item4.PlayerDetail.AddBoatScore(num31);
						}
						break;
					}
					case 6:
						if (!flag)
						{
							int num29 = AddValue(item5);
							num13 += num29;
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num29));
						}
						break;
					case 7:
						if (item5.ValiddateHour() && !flag)
						{
							int num26 = AddValue(item5);
							num13 += num26;
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num26));
						}
						break;
					case -10010:
						if (!flag)
						{
							int num25 = AddValue(item5);
							item4.PlayerDetail.AddGold(num25);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num25));
						}
						break;
					case -20010:
						if (!flag)
						{
							int num23 = AddValue(item5);
							item4.PlayerDetail.LogAddMoney(AddMoneyType.Battle_Bonus, AddMoneyType.Battle_Bonus_Add, item4.PlayerDetail.PlayerCharacter.ID, num23, item4.PlayerDetail.PlayerCharacter.Money);
							item4.PlayerDetail.AddCupomPVP(num23);
							item4.PlayerDetail.UpdateUserMoney(num23, 2);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num23));
						}
						break;
					case 8:
						if (item5.ValiddateHour() && !flag)
						{
							int num20 = AddValue(item5);
							item4.PlayerDetail.LogAddMoney(AddMoneyType.Battle_Bonus, AddMoneyType.Battle_Bonus_Add, item4.PlayerDetail.PlayerCharacter.ID, num20, item4.PlayerDetail.PlayerCharacter.Money);
							item4.PlayerDetail.AddCupomPVP(num20);
							item4.PlayerDetail.UpdateUserMoney(num20, 2);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num20));
						}
						break;
					case 30010:
						if (!flag)
						{
							int num18 = AddValue(item5);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num18));
							item4.PlayerDetail.AddMedal(num18);
						}
						break;
					case 110010:
						if (!flag)
						{
							int num16 = AddValue(item5);
							item4.PlayerDetail.AddGiftToken(num16);
							item4.PlayerDetail.SendMessage(string.Format(item5.Notice, num16));
						}
						break;
					}
				}
				foreach (ItemInfo item6 in list3)
				{
					item4.PlayerDetail.AddTemplate(item6, eBageType.TempBag, item6.Count, eGameView.BatleTypeGet);
				}
				foreach (int item7 in item4.PlayerDetail.QuestColect)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item7);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 106);
						itemInfo2.IsBinds = true;
						item4.PlayerDetail.AddTemplate(itemInfo2, eBageType.TempBag, 1, eGameView.BatleTypeGet);
					}
				}
				int num33 = item4.PlayerDetail.DragonBoatAddExpPlus();
				if (num33 > 0)
				{
					num13 *= num33;
					item4.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg9", num33));
				}
				item4.PlayerDetail.AddPrestige(flag, base.RoomType);
				if (item4.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
				{
					num13 += num13 * item4.FightBuffers.ConsortionAddPercentGoldOrGP / 100;
				}
				if (item4.FightBuffers.ConsortionAddOfferRate > 0)
				{
					num3 *= item4.FightBuffers.ConsortionAddOfferRate;
				}
				num13 = ((num13 <= 0) ? 1 : num13);
				num15 = ((num15 <= 0) ? 1 : num15);
				if (!GameProperties.expNoPvp)
				{
					num13 = 0;
					num15 = 0;
				}
				item4.GainGP = item4.PlayerDetail.AddGP(num13 + num15);
				item4.GainOffer = item4.PlayerDetail.AddOffer(num3);
				if (flag)
				{
					item4.PlayerDetail.AddScoreCityBattle(1);
				}
			}
			else if (m_roomType == eRoomType.BombKing)
			{
				if (item4.Team == winTeam)
				{
					int num34 = ((item4.Blood * 10 / item4.MaxBlood <= 0) ? 1 : (item4.Blood * 10 / item4.MaxBlood));
					item4.PlayerDetail.AddScoreBombKing(num34 + 1);
				}
				else
				{
					item4.PlayerDetail.AddScoreBombKing(1);
				}
			}
			else if (m_roomType == eRoomType.ConsortiaBattle)
			{
				item4.PlayerDetail.UpdateConsortiaBattle(item4.Blood, flag, num8);
			}
			gSPacketIn.WriteInt(item4.Id);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteInt(item4.Grade);
			gSPacketIn.WriteInt(num13);
			gSPacketIn.WriteInt(item4.TotalKill);
			gSPacketIn.WriteInt((num15 > 10000) ? 10000 : num15);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt((item4.GainGP > 10000) ? 10000 : item4.GainGP);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item4.GainOffer);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(item4.CanTakeOut);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteInt(0);
			}
			if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt(item4.TotalKill);
			}
			item4.PlayerDetail.OnGameOver(this, item4.Team == winTeam, item4.GainGP);
		}
		gSPacketIn.WriteInt(num3);
		if (GameProperties.VERSION >= 7600 || (GameProperties.VERSION < 7600 && base.RoomType == eRoomType.FightFootballTime))
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		if (GameProperties.VERSION >= 10200)
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		SendToAll(gSPacketIn);
		if (base.RoomType == eRoomType.SurvivalMode)
		{
			WaitTime(19000);
		}
		else if (base.RoomType == eRoomType.ConsortiaBattle)
		{
			WaitTime(1000);
		}
		else
		{
			WaitTime(16000);
		}
		OnGameOverred();
	}

	public override void Stop()
	{
		if (base.GameState != eGameState.GameOver)
		{
			return;
		}
		SendTempStyle(eRoomType.Exploration);
		m_gameState = eGameState.Stopped;
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (base.RoomType != eRoomType.FightFootballTime && !item.FinishTakeCard)
			{
				int canTakeOut = item.CanTakeOut;
				for (int i = 0; i < canTakeOut; i++)
				{
					TakeCard(item);
				}
			}
		}
		lock (m_players)
		{
			m_players.Clear();
		}
		base.Stop();
	}

	public void GameOverAreaMode()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		m_gameState = eGameState.Stopped;
		ClearWaitTimer();
		CurrentTurnTotalDamage = 0;
		List<Player> allFightPlayers = GetAllFightPlayers();
		int num = -1;
		foreach (Player item in allFightPlayers)
		{
			if (item.IsLiving)
			{
				num = item.Team;
				break;
			}
		}
		if (num == -1 && CurrentPlayer != null)
		{
			num = CurrentPlayer.Team;
		}
		int val = 0;
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(100);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(base.PlayerCount);
		foreach (Player item2 in allFightPlayers)
		{
			bool flag = false;
			flag = item2.Team == num;
			BattleBonusInfo battleBonusInfo = BattleBonusMgr.FindBattleBonusByType(typeEnterMode + 100);
			if (EntertainmentMode() && battleBonusInfo != null)
			{
				int num2 = battleBonusInfo.MinValue;
				if (flag)
				{
					num2 = battleBonusInfo.MaxValue;
				}
				if (num2 > 0)
				{
					item2.PlayerDetail.AddEnterModePoint(num2);
					item2.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("GameServer.TakeEntertamainPoint.Msg", num2));
				}
				if (base.RoomType == eRoomType.EntertainmentPK && flag)
				{
					int entertainmentModeMoneyAdd = GameProperties.EntertainmentModeMoneyAdd;
					item2.PlayerDetail.LogAddMoney(AddMoneyType.EntertainmentPk, AddMoneyType.EntertainmentPk_Add, item2.PlayerDetail.PlayerCharacter.ID, entertainmentModeMoneyAdd, item2.PlayerDetail.PlayerCharacter.Money);
					item2.PlayerDetail.AddMoney(entertainmentModeMoneyAdd);
					item2.PlayerDetail.UpdateUserMoney(entertainmentModeMoneyAdd, 2);
					item2.PlayerDetail.SendMessage(LanguageMgr.GetTranslation("GameServer.TakeMoneyOfEntertamainRoom.Msg"));
				}
				item2.PlayerDetail.ClearFightBag();
			}
			else
			{
				item2.PlayerDetail.AddPrestige(flag, base.RoomType);
				if (!item2.PlayerDetail.PlayerCharacter.IsAutoBot)
				{
					item2.PlayerDetail.RingstationResult(flag);
				}
			}
			gSPacketIn.WriteInt(item2.Id);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteInt(item2.Grade);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item2.TotalKill);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt((item2.GainGP > 10000) ? 10000 : item2.GainGP);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item2.GainOffer);
			gSPacketIn.WriteInt(0);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteInt(0);
			}
			gSPacketIn.WriteInt(item2.CanTakeOut);
			if (GameProperties.VERSION >= 8100)
			{
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteInt(0);
			}
			if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt(item2.TotalKill);
			}
			item2.PlayerDetail.OnGameOver(this, item2.Team == num, item2.GainGP);
		}
		gSPacketIn.WriteInt(0);
		if (GameProperties.VERSION >= 7600 || (GameProperties.VERSION < 7600 && base.RoomType == eRoomType.FightFootballTime))
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		if (GameProperties.VERSION >= 10200)
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		SendToAll(gSPacketIn);
		OnGameOverred();
		lock (m_players)
		{
			m_players.Clear();
		}
		base.Stop();
	}

	public void AddShadow(Player fp)
	{
		AddLiving(fp);
		fp.StartMoving();
		SendShadownPlayer(fp);
	}

	private int CalculateGuildMatchResult(List<Player> players, int winTeam)
	{
		if (base.RoomType == eRoomType.Match)
		{
			StringBuilder stringBuilder = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
			StringBuilder stringBuilder2 = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
			IGamePlayer gamePlayer = null;
			IGamePlayer gamePlayer2 = null;
			int num = 0;
			foreach (Player player in players)
			{
				if (player.Team == winTeam)
				{
					stringBuilder.Append($"[{player.PlayerDetail.PlayerCharacter.NickName}]");
					gamePlayer = player.PlayerDetail;
				}
				else
				{
					stringBuilder2.Append($"{player.PlayerDetail.PlayerCharacter.NickName}");
					gamePlayer2 = player.PlayerDetail;
					num++;
				}
			}
			if (gamePlayer2 != null)
			{
				stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1") + gamePlayer2.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2"));
				stringBuilder2.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3") + gamePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4"));
				int num2 = 0;
				if (base.GameType == eGameType.Guild)
				{
					num2 = num + TotalHurt / 2000;
				}
				gamePlayer.ConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, TotalHurt, players.Count);
				if (gamePlayer.ServerID != gamePlayer2.ServerID)
				{
					gamePlayer2.ConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, TotalHurt, players.Count);
				}
				if (base.GameType == eGameType.Guild)
				{
					gamePlayer.SendConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, num2, stringBuilder.ToString());
				}
				return num2;
			}
		}
		return 0;
	}

	public bool CanGameOver()
	{
		if (base.RoomType == eRoomType.FightFootballTime && base.TurnIndex > 7)
		{
			return true;
		}
		if (base.RoomType == eRoomType.SurvivalMode)
		{
			int num = m_redTeam.Count + m_blueTeam.Count;
			foreach (Player item in m_redTeam)
			{
				if (!item.IsLiving)
				{
					num--;
				}
			}
			foreach (Player item2 in m_blueTeam)
			{
				if (!item2.IsLiving)
				{
					num--;
				}
			}
			return num <= 1;
		}
		if (base.RoomType == eRoomType.BombKing && beginTime.AddMinutes(4.0).AddSeconds(30.0) < DateTime.Now)
		{
			return true;
		}
		if (base.RoomType == eRoomType.ConsortiaBattle && beginTime.AddSeconds(300.0) <= DateTime.Now)
		{
			return true;
		}
		bool flag = true;
		bool flag2 = true;
		foreach (Player item3 in m_redTeam)
		{
			if (item3.IsLiving)
			{
				flag = false;
				break;
			}
		}
		foreach (Player item4 in m_blueTeam)
		{
			if (item4.IsLiving)
			{
				flag2 = false;
				break;
			}
		}
		return flag || flag2;
	}

	public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
	{
		Player player = base.RemovePlayer(gp, IsKick);
		if (player != null && player.IsLiving && base.GameState != eGameState.Loading)
		{
			gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
			player.PlayerDetail.ClearFightBuffOneMatch();
			string text = null;
			string text2 = null;
			if (base.RoomType == eRoomType.Match)
			{
				if (base.GameType == eGameType.Guild)
				{
					text = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 15);
					gp.RemoveOffer(15);
					text2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 15);
				}
				else if (base.GameType == eGameType.Free)
				{
					text = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 5);
					gp.RemoveOffer(5);
					text2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 5);
				}
				BattleBonusInfo battleBonusInfo = BattleBonusMgr.FindBattleBonusByType(9002);
				if (battleBonusInfo != null)
				{
					int num = ThreadSafeRandom.NextStatic(battleBonusInfo.MinValue, battleBonusInfo.MaxValue);
					gp.RemoveMoney(num);
					gp.SendMessage(string.Format(battleBonusInfo.Notice, num));
				}
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				SendMessage(gp, text, text2, 3);
			}
			if (GetSameTeam() && base.CurrentLiving != null)
			{
				base.CurrentLiving.StopAttacking();
				CheckState(0);
			}
		}
		return player;
	}

	public override void CheckState(int delay)
	{
		AddAction(new CheckPVPGameStateAction(delay));
	}

	public Player FindNearestEnemyPlayer(Player player)
	{
		double num = double.MaxValue;
		Player result = null;
		foreach (Player allEnemyPlayer in GetAllEnemyPlayers(player))
		{
			if (allEnemyPlayer.IsLiving)
			{
				double num2 = allEnemyPlayer.Distance(player.X, player.Y);
				if (num2 < num)
				{
					num = num2;
					result = allEnemyPlayer;
				}
			}
		}
		return result;
	}
}

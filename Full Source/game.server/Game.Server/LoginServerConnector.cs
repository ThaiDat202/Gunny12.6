using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Interface;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.BombKing;
using Game.Server.DragonBoat;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server;

public class LoginServerConnector : BaseConnector
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private int _serverId;

	private int _zoneId;

	private string _loginKey;

	protected override void OnDisconnect()
	{
		log.Info("LoginServerConnector is OnDisconnect!");
		base.OnDisconnect();
	}

	public override void OnRecvPacket(GSPacketIn pkg)
	{
		ThreadPool.QueueUserWorkItem(AsynProcessPacket, pkg);
	}

	protected void AsynProcessPacket(object state)
	{
		try
		{
			if (state is GSPacketIn gSPacketIn)
			{
				switch (gSPacketIn.Code)
				{
				case 0:
					HandleRSAKey(gSPacketIn);
					break;
				case 2:
					HandleKitoffPlayer(gSPacketIn);
					break;
				case 3:
					HandleAllowUserLogin(gSPacketIn);
					break;
				case 4:
					HandleUserOffline(gSPacketIn);
					break;
				case 5:
					HandleUserOnline(gSPacketIn);
					break;
				case 37:
					HandleChatPersonal(gSPacketIn);
					break;
				case 107:
					HandleChatAreaPersonal(gSPacketIn);
					break;
				case 71:
				case 72:
				case 73:
					HandleBigBugle(gSPacketIn);
					break;
				case 160:
					HandleFriend(gSPacketIn);
					break;
				case 9:
					HandleChargeMoney(gSPacketIn);
					break;
				case 10:
					HandleSystemNotice(gSPacketIn);
					break;
				case 117:
					HandleMailResponse(gSPacketIn);
					break;
				case 19:
					HandleChatConsortia(gSPacketIn);
					break;
				case 128:
					HandleConsortiaResponse(gSPacketIn);
					break;
				case 158:
					HandleConsortiaFight(gSPacketIn);
					break;
				case 7:
					HandleASSState(gSPacketIn);
					break;
				case 38:
					HandleSysMess(gSPacketIn);
					break;
				case 130:
					HandleConsortiaCreate(gSPacketIn);
					break;
				case 177:
					HandleRate(gSPacketIn);
					break;
				case 178:
					HandleMacroDrop(gSPacketIn);
					break;
				case 8:
					HandleConfigState(gSPacketIn);
					break;
				case 13:
					HandleUpdatePlayerMarriedState(gSPacketIn);
					break;
				case 14:
					HandleMarryRoomInfoToPlayer(gSPacketIn);
					break;
				case 15:
					HandleShutdown(gSPacketIn);
					break;
				case 79:
					HandleWorldBossUpdateBlood(gSPacketIn);
					break;
				case 80:
					HandleWorldBossUpdate(gSPacketIn);
					break;
				case 81:
					HandleWorldBossRank(gSPacketIn);
					break;
				case 82:
					HandleWorldBossFightOver(gSPacketIn);
					break;
				case 83:
					HandleWorldBossRoomClose(gSPacketIn);
					break;
				case 85:
					HandleWorldBossPrivateInfo(gSPacketIn);
					break;
				case 88:
					HandleBattleGoundOpenClose(gSPacketIn);
					break;
				case 87:
					HandleLeagueOpenClose(gSPacketIn);
					break;
				case 89:
					HandleFightFootballTime(gSPacketIn);
					break;
				case 180:
					HandleConsortiaBossInfo(gSPacketIn);
					break;
				case 185:
					HandleConsortiaBossSendAward(gSPacketIn);
					break;
				case 90:
					HandleEventRank(gSPacketIn);
					break;
				case 91:
					HandleWorldEvent(gSPacketIn);
					break;
				case 189:
					HandleEnterModeGetPoint(gSPacketIn);
					break;
				case 192:
					HandleUpdateAreaConfigState(gSPacketIn);
					break;
				case 193:
					HandleReload(gSPacketIn);
					break;
				case 300:
					HandleBombKingSyn(gSPacketIn);
					break;
				case 301:
					HandleBombKingOpenClose(gSPacketIn);
					break;
				case 302:
					HandleBombUpdateRequest();
					break;
				case 303:
					HandleBombKingUpdatePlayerStep1(gSPacketIn);
					break;
				case 304:
					HandleBombKingUpdatePlayerStep2(gSPacketIn);
					break;
				case 305:
					HandleBombKingAddRank(gSPacketIn);
					break;
				case 306:
					HandleBombKingUpdatePlayer(gSPacketIn);
					break;
				case 307:
					HandleBombKingClearPlayer(gSPacketIn);
					break;
				case 368:
					HandleCityBattle(gSPacketIn);
					break;
				case 329:
					HandleSanXiao(gSPacketIn);
					break;
				case 308:
					HandleLoginDeviceCheck(gSPacketIn);
					break;
				}
			}
		}
		catch (Exception exception)
		{
			log.Error("AsynProcessPacket", exception);
		}
	}

	public void HandleLoginDeviceCheck(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		if (playerById != null)
		{
			bool isLogin = pkg.ReadBoolean();
			playerById.Out.SendLoginDeviceCheck(playerById, isLogin);
		}
	}

	public void HandleCityBattle(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.SendTCP(pkg);
	}

	public void HandleSanXiao(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		if (playerById != null)
		{
			pkg.ClearOffset();
			GSPacketIn gSPacketIn = pkg.Clone();
			gSPacketIn.ClearOffset();
			byte b = gSPacketIn.ReadByte();
			if (b == 34)
			{
				playerById.DDTQiYuan.ProcessData(playerById, pkg);
			}
			else
			{
				playerById.SendTCP(pkg);
			}
		}
	}

	public void HandleBombKingClearPlayer(GSPacketIn pkg)
	{
		BombKingMgr.ClearPlayer();
	}

	public void HandleBombKingUpdatePlayer(GSPacketIn pkg)
	{
		pkg.UnCompress();
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			BombKingPlayerInfo bombKingPlayerInfo = new BombKingPlayerInfo();
			bombKingPlayerInfo.UserID = pkg.ReadInt();
			bombKingPlayerInfo.AreaID = pkg.ReadInt();
			bombKingPlayerInfo.Place = pkg.ReadInt();
			bombKingPlayerInfo.NickName = pkg.ReadString();
			bombKingPlayerInfo.AreaName = pkg.ReadString();
			bombKingPlayerInfo.vipType = pkg.ReadInt();
			bombKingPlayerInfo.vipLevel = pkg.ReadInt();
			bombKingPlayerInfo.myRank = pkg.ReadInt();
			bombKingPlayerInfo.myScore = pkg.ReadInt();
			bombKingPlayerInfo.Score = pkg.ReadInt();
			bombKingPlayerInfo.RankType = pkg.ReadInt();
			bombKingPlayerInfo.status = pkg.ReadInt();
			bombKingPlayerInfo.style = pkg.ReadString();
			bombKingPlayerInfo.color = pkg.ReadString();
			bombKingPlayerInfo.sex = pkg.ReadBoolean();
			bombKingPlayerInfo.smg = pkg.ReadString();
			BombKingMgr.UpdatePlayer(bombKingPlayerInfo);
		}
	}

	public void HandleBombKingAddRank(GSPacketIn pkg)
	{
		BombKingMgr.AddNewRank(pkg.Parameter1, pkg.Parameter2, pkg.ClientID);
	}

	public void HandleBombKingOpenClose(GSPacketIn pkg)
	{
		BombKingMgr.sendBombKingOpenClose(pkg.ReadBoolean());
	}

	public void HandleBombKingUpdatePlayerStep1(GSPacketIn pkg)
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			if (gamePlayer.battleStage == 2)
			{
				gamePlayer.AddScoreBombKing(1);
			}
			gamePlayer.battleStage = 0;
			gamePlayer.battleId = 0;
		}
	}

	public void HandleBombKingUpdatePlayerStep2(GSPacketIn pkg)
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		foreach (GamePlayer gamePlayer in allPlayers)
		{
			if (gamePlayer.battleStage == 2)
			{
				if (gamePlayer.battleId == -1 || gamePlayer.battleId > 0)
				{
					gamePlayer.AddScoreBombKing(2);
				}
				else
				{
					gamePlayer.AddScoreBombKing(1);
				}
			}
			gamePlayer.battleStage = 0;
			gamePlayer.battleId = 0;
		}
	}

	public void HandleBombUpdateRequest()
	{
		BombKingMgr.SendUpdateBombKingFrame();
	}

	public void HandleBombKingSyn(GSPacketIn pkg)
	{
		BombKingMgr.battleStage = pkg.ReadInt();
		BombKingMgr.turn = pkg.ReadInt();
		BombKingMgr.status = pkg.ReadInt();
		BombKingMgr.IsBombKingOpen = pkg.ReadBoolean();
		BombKingMgr.Week = pkg.ReadInt();
		BombKingMgr.BombKingBeginTime = pkg.ReadDateTime();
		BombKingMgr.BombKingEndTime = pkg.ReadDateTime();
	}

	public void HandleUpdateAreaConfigState(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		int num2 = num;
		int num3 = num2;
		if (num3 == 1)
		{
			int num4 = pkg.ReadInt();
			List<AreaConfigInfo> list = new List<AreaConfigInfo>();
			for (int i = 0; i < num4; i++)
			{
				AreaConfigInfo areaConfigInfo = new AreaConfigInfo();
				areaConfigInfo.AreaID = pkg.ReadInt();
				areaConfigInfo.AreaServer = pkg.ReadString();
				areaConfigInfo.AreaName = pkg.ReadString();
				areaConfigInfo.DataSource = pkg.ReadString();
				areaConfigInfo.Catalog = pkg.ReadString();
				areaConfigInfo.UserID = pkg.ReadString();
				areaConfigInfo.Password = pkg.ReadString();
				areaConfigInfo.RequestUrl = pkg.ReadString();
				areaConfigInfo.CrossChatAllow = pkg.ReadBoolean();
				areaConfigInfo.CrossPrivateChat = pkg.ReadBoolean();
				areaConfigInfo.Version = pkg.ReadString();
				list.Add(areaConfigInfo);
			}
			WorldMgr.AddAreaConfig(list.ToArray());
		}
		else
		{
			log.Error("HandleUpdateAreaConfigState no found: " + num);
		}
	}

	public void HandleEnterModeGetPoint(GSPacketIn pkg)
	{
		int playerId = pkg.ReadInt();
		GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.Actives.Info.EntertamentPoint = pkg.ReadInt();
		}
	}

	public void HandleReload(GSPacketIn packet)
	{
		eReloadType type = (eReloadType)packet.ReadInt();
		Reload(type, isSchema: true);
	}

	public void Reload(eReloadType type, bool isSchema)
	{
		bool flag = true;
		switch (type)
		{
		case eReloadType.all:
		{
			IEnumerable<eReloadType> enumerable = Enum.GetValues(typeof(eReloadType)).Cast<eReloadType>();
			foreach (eReloadType item in enumerable)
			{
				if (item != eReloadType.all)
				{
					Reload(item, isSchema: false);
				}
			}
			flag = true;
			break;
		}
		case eReloadType.ball:
			flag = BallMgr.ReLoad();
			break;
		case eReloadType.ballconfig:
			flag = BallConfigMgr.ReLoad();
			break;
		case eReloadType.fusion:
			flag = FusionMgr.ReLoad();
			break;
		case eReloadType.item:
			if (LocalItemMgr.ReLoad())
			{
				flag = ItemMgr.ReLoad();
			}
			break;
		case eReloadType.map:
			flag = MapMgr.ReLoadMap();
			break;
		case eReloadType.npc:
			flag = NPCInfoMgr.ReLoad();
			break;
		case eReloadType.pve:
			flag = PveInfoMgr.ReLoad();
			break;
		case eReloadType.mapserver:
			flag = MapMgr.ReLoadMapServer();
			break;
		case eReloadType.quest:
			flag = QuestMgr.ReLoad();
			break;
		case eReloadType.server:
			GameServer.Instance.Configuration.Refresh();
			break;
		case eReloadType.serverconfig:
			GameProperties.Refresh();
			break;
		case eReloadType.rate:
			flag = RateMgr.ReLoad();
			break;
		case eReloadType.fight:
			flag = FightRateMgr.ReLoad();
			break;
		case eReloadType.dailyaward:
			flag = AwardMgr.ReLoad();
			break;
		case eReloadType.language:
			flag = LanguageMgr.Reload("");
			break;
		case eReloadType.shop:
			flag = ShopMgr.ReLoad();
			break;
		case eReloadType.consortia:
			flag = ConsortiaMgr.ReLoad();
			break;
		case eReloadType.prop:
			flag = PropItemMgr.Reload();
			break;
		case eReloadType.shopbox:
			flag = ItemBoxMgr.ReLoad();
			break;
		case eReloadType.active:
			flag = ActiveMgr.ReLoad();
			if (flag)
			{
				ActiveMgr.UpdateCurrentServerActive();
			}
			break;
		case eReloadType.drop:
			flag = DropMgr.ReLoad();
			break;
		case eReloadType.mission:
			flag = MissionInfoMgr.Reload();
			break;
		case eReloadType.pet:
			flag = PetMgr.ReLoad();
			break;
		case eReloadType.mount:
			flag = MountMgr.ReLoad();
			break;
		case eReloadType.rune:
			flag = RuneMgr.ReLoad();
			break;
		case eReloadType.level:
			flag = LevelMgr.ReLoad();
			break;
		case eReloadType.strengthen:
			flag = StrengthenMgr.ReLoad();
			break;
		case eReloadType.accumulactivelogin:
			flag = AccumulActiveLoginMgr.ReLoad();
			break;
		case eReloadType.achievement:
			flag = AchievementMgr.Reload();
			break;
		case eReloadType.activityhalloweenitems:
			flag = ActivityHalloweenItemsMgr.ReLoad();
			break;
		case eReloadType.activityquest:
			flag = ActivityQuestMgr.ReLoad();
			break;
		case eReloadType.allquestions:
			flag = AllQuestionsMgr.ReLoad();
			break;
		case eReloadType.avatarcolection:
			flag = AvatarColectionMgr.ReLoad();
			break;
		case eReloadType.chargeactivetemplate:
			flag = ChargeActiveTemplateMgr.ReLoad();
			break;
		case eReloadType.chargespendrewardtemplate:
			flag = ChargeSpendRewardTemplateMgr.ReLoad();
			break;
		case eReloadType.communalactive:
			flag = CommunalActiveMgr.ReLoad();
			break;
		case eReloadType.consortiaextra:
			flag = ConsortiaExtraMgr.ReLoad();
			break;
		case eReloadType.dailyleagueaward:
			flag = DailyLeagueAwardMgr.ReLoad();
			break;
		case eReloadType.dicegameaward:
			flag = DiceGameAwardMgr.ReLoad();
			break;
		case eReloadType.dicelevelaward:
			flag = DiceLevelAwardMgr.ReLoad();
			break;
		case eReloadType.eventaward:
			flag = EventAwardMgr.ReLoad();
			break;
		case eReloadType.everydayactive:
			flag = EveryDayActiveMgr.ReLoad();
			break;
		case eReloadType.exercise:
			flag = ExerciseMgr.ReLoad();
			break;
		case eReloadType.fairbattlereward:
			flag = FairBattleRewardMgr.ReLoad();
			break;
		case eReloadType.fightlabdropitem:
			flag = FightLabDropItemMgr.ReLoad();
			break;
		case eReloadType.fightspirittemplate:
			flag = FightSpiritTemplateMgr.ReLoad();
			break;
		case eReloadType.goldequip:
			flag = GoldEquipMgr.ReLoad();
			break;
		case eReloadType.helpgamereward:
			flag = HelpGameRewardMgr.ReLoad();
			break;
		case eReloadType.homefish:
			flag = HomeFishMgr.ReLoad();
			break;
		case eReloadType.itemstrengthendata:
			flag = ItemStrengThenDataMgr.ReLoad();
			break;
		case eReloadType.lightriddlequest:
			flag = LightriddleQuestMgr.ReLoad();
			break;
		case eReloadType.lotteryshowtemplate:
			flag = LotteryShowTemplateMgr.ReLoad();
			break;
		case eReloadType.magicfusiondata:
			flag = MagicFusionDataMgr.ReLoad();
			break;
		case eReloadType.magicitemtemplate:
			flag = MagicItemTemplateMgr.ReLoad();
			break;
		case eReloadType.magicstone:
			flag = MagicStoneMgr.ReLoad();
			break;
		case eReloadType.missionenergy:
			flag = MissionEnergyMgr.Reload();
			break;
		case eReloadType.newtitle:
			flag = NewTitleMgr.ReLoad();
			break;
		case eReloadType.petmoeproperty:
			flag = PetMoePropertyMgr.ReLoad();
			break;
		case eReloadType.ranktemplate:
			flag = RankTemplateMgr.ReLoad();
			break;
		case eReloadType.searchgoods:
			flag = SearchGoodsMgr.ReLoad();
			break;
		case eReloadType.subactive:
			flag = SubActiveMgr.ReLoad();
			break;
		case eReloadType.suit:
			flag = SuitMgr.ReLoad();
			break;
		case eReloadType.totemhonor:
			flag = TotemHonorMgr.ReLoad();
			break;
		case eReloadType.totem:
			flag = TotemMgr.ReLoad();
			break;
		case eReloadType.treasureaward:
			flag = TreasureAwardMgr.ReLoad();
			break;
		case eReloadType.treetemplate:
			flag = TreeTemplateMgr.ReLoad();
			break;
		case eReloadType.qqtip:
			flag = QQTipsMgr.ReLoad();
			break;
		case eReloadType.communalactivestartnew:
			DragonBoatMgr.StartNewEvent();
			break;
		case eReloadType.communalactiveendonly:
			DragonBoatMgr.EndEvent(sendAward: false);
			break;
		case eReloadType.communalactiveendwithaward:
			DragonBoatMgr.EndEvent(sendAward: true);
			break;
		case eReloadType.comandos:
			ComandosMgr.Init();
			break;
		case eReloadType.card:
			CardMgr.ReLoad();
			break;
		case eReloadType.center:
		case eReloadType.notice:
		case eReloadType.clearchat:
		case eReloadType.GP:
		{
			GSPacketIn gSPacketIn = new GSPacketIn(193);
			gSPacketIn.WriteInt((int)type);
			SendTCP(gSPacketIn);
			break;
		}
		default:
			Console.WriteLine("Commamd {0} do not suport!", type);
			break;
		}
		if (flag)
		{
			Console.WriteLine("{0} is reload!", type);
			log.InfoFormat("{0} is reload!", type);
		}
		else
		{
			Console.WriteLine("{0} reload fail!", type);
		}
		if (!isSchema)
		{
			return;
		}
		AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(GameServer.Instance.Configuration.AreaId);
		if (areaConfigInfo == null || string.IsNullOrEmpty(areaConfigInfo.RequestUrl))
		{
			return;
		}
		string text = areaConfigInfo.RequestUrl + "CreateAllXml.ashx";
		try
		{
			string value = BaseInterface.RequestContent(text);
			Console.WriteLine(value);
			GSPacketIn gSPacketIn2 = new GSPacketIn(139);
			gSPacketIn2.WriteInt(1);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				gamePlayer.SendTCP(gSPacketIn2);
			}
			gSPacketIn2 = new GSPacketIn(139);
			gSPacketIn2.WriteInt(2);
			GamePlayer[] allPlayers2 = WorldMgr.GetAllPlayers();
			foreach (GamePlayer gamePlayer2 in allPlayers2)
			{
				gamePlayer2.SendTCP(gSPacketIn2);
			}
		}
		catch
		{
			Console.WriteLine("Wrong url {0}", text);
		}
	}

	public void HandleWorldEvent(GSPacketIn pkg)
	{
		byte b = pkg.ReadByte();
		byte b2 = b;
		byte b3 = b2;
		if (b3 == 2 && pkg.ReadInt() == 1)
		{
			LanternriddlesInfo lanternriddlesInfo = new LanternriddlesInfo();
			lanternriddlesInfo.PlayerID = pkg.ReadInt();
			lanternriddlesInfo.QuestionIndex = pkg.ReadInt();
			lanternriddlesInfo.QuestionView = pkg.ReadInt();
			lanternriddlesInfo.EndDate = pkg.ReadDateTime();
			lanternriddlesInfo.DoubleFreeCount = pkg.ReadInt();
			lanternriddlesInfo.DoublePrice = pkg.ReadInt();
			lanternriddlesInfo.HitFreeCount = pkg.ReadInt();
			lanternriddlesInfo.HitPrice = pkg.ReadInt();
			lanternriddlesInfo.MyInteger = pkg.ReadInt();
			lanternriddlesInfo.QuestionNum = pkg.ReadInt();
			lanternriddlesInfo.Option = pkg.ReadInt();
			lanternriddlesInfo.IsHint = pkg.ReadBoolean();
			lanternriddlesInfo.IsDouble = pkg.ReadBoolean();
			ActiveSystemMgr.AddOrUpdateLanternriddles(lanternriddlesInfo.PlayerID, lanternriddlesInfo);
		}
	}

	public GSPacketIn SendGetLightriddleInfo(int playerID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(playerID);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void SendLightriddleInfo(LanternriddlesInfo Lanternriddles)
	{
		if (Lanternriddles != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(Lanternriddles.PlayerID);
			gSPacketIn.WriteInt(Lanternriddles.QuestionIndex);
			gSPacketIn.WriteInt(Lanternriddles.QuestionView);
			gSPacketIn.WriteDateTime(Lanternriddles.EndDate);
			gSPacketIn.WriteInt(Lanternriddles.DoubleFreeCount);
			gSPacketIn.WriteInt(Lanternriddles.DoublePrice);
			gSPacketIn.WriteInt(Lanternriddles.HitFreeCount);
			gSPacketIn.WriteInt(Lanternriddles.HitPrice);
			gSPacketIn.WriteInt(Lanternriddles.MyInteger);
			gSPacketIn.WriteInt(Lanternriddles.QuestionNum);
			gSPacketIn.WriteInt(Lanternriddles.Option);
			gSPacketIn.WriteBoolean(Lanternriddles.IsHint);
			gSPacketIn.WriteBoolean(Lanternriddles.IsDouble);
			SendTCP(gSPacketIn);
		}
	}

	public GSPacketIn SendLightriddleRank(string Nickname, int PlayeID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(90);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteString(Nickname);
		gSPacketIn.WriteInt(PlayeID);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLightriddleUpateRank(int Integer, PlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(90);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteString(player.NickName);
		gSPacketIn.WriteInt(player.typeVIP);
		gSPacketIn.WriteInt(Integer);
		gSPacketIn.WriteInt(player.ID);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(90);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(PlayerID);
		gSPacketIn.WriteString(nickName);
		gSPacketIn.WriteInt(TemplateID);
		gSPacketIn.WriteInt(Count);
		gSPacketIn.WriteInt(isVip);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendHalloweenRank(int PlayerID, string nickName, int useNum, int isVip)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(90);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(PlayerID);
		gSPacketIn.WriteString(nickName);
		gSPacketIn.WriteInt(useNum);
		gSPacketIn.WriteInt(isVip);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void HandleEventRank(GSPacketIn pkg)
	{
		switch (pkg.ReadByte())
		{
		case 1:
		{
			List<RankingLightriddleInfo> list = new List<RankingLightriddleInfo>();
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				RankingLightriddleInfo rankingLightriddleInfo = new RankingLightriddleInfo();
				rankingLightriddleInfo.Rank = pkg.ReadInt();
				rankingLightriddleInfo.NickName = pkg.ReadString();
				rankingLightriddleInfo.TypeVIP = pkg.ReadByte();
				rankingLightriddleInfo.Integer = pkg.ReadInt();
				list.Add(rankingLightriddleInfo);
			}
			int myRank = pkg.ReadInt();
			int playerId = pkg.ReadInt();
			WorldMgr.GetPlayerById(playerId)?.Actives.SendLightriddleRank(myRank, list);
			break;
		}
		case 5:
			ActiveSystemMgr.ReloadHalloweenRank();
			break;
		}
	}

	public void HandleConsortiaBossSendAward(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int consortiaId = pkg.ReadInt();
			ConsortiaBossMgr.SendConsortiaAward(consortiaId);
		}
	}

	public void HandleConsortiaBossInfo(GSPacketIn pkg)
	{
		ConsortiaInfo consortiaInfo = new ConsortiaInfo();
		consortiaInfo.ConsortiaID = pkg.ReadInt();
		consortiaInfo.ChairmanID = pkg.ReadInt();
		consortiaInfo.bossState = pkg.ReadByte();
		consortiaInfo.endTime = pkg.ReadDateTime();
		consortiaInfo.extendAvailableNum = pkg.ReadInt();
		consortiaInfo.callBossLevel = pkg.ReadInt();
		consortiaInfo.Level = pkg.ReadInt();
		consortiaInfo.SmithLevel = pkg.ReadInt();
		consortiaInfo.StoreLevel = pkg.ReadInt();
		consortiaInfo.SkillLevel = pkg.ReadInt();
		consortiaInfo.Riches = pkg.ReadInt();
		consortiaInfo.LastOpenBoss = pkg.ReadDateTime();
		consortiaInfo.MaxBlood = pkg.ReadLong();
		consortiaInfo.TotalAllMemberDame = pkg.ReadLong();
		consortiaInfo.IsBossDie = pkg.ReadBoolean();
		consortiaInfo.RankList = new Dictionary<string, RankingPersonInfo>();
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			RankingPersonInfo rankingPersonInfo = new RankingPersonInfo();
			rankingPersonInfo.Name = pkg.ReadString();
			rankingPersonInfo.ID = pkg.ReadInt();
			rankingPersonInfo.TotalDamage = pkg.ReadInt();
			rankingPersonInfo.Honor = pkg.ReadInt();
			rankingPersonInfo.Damage = pkg.ReadInt();
			rankingPersonInfo.ZoneID = GameServer.Instance.Configuration.AreaId;
			consortiaInfo.RankList.Add(rankingPersonInfo.Name, rankingPersonInfo);
		}
		switch (pkg.ReadByte())
		{
		case 180:
			SendToAllConsortiaMember(consortiaInfo, -1);
			break;
		case 183:
			HandleConsortiaBossCreateBoss(consortiaInfo);
			break;
		case 184:
			HandleConsortiaBossReload(consortiaInfo);
			break;
		case 182:
			HandleConsortiaBossExtendAvailable(consortiaInfo);
			break;
		case 187:
			HandleConsortiaBossClose(consortiaInfo);
			break;
		case 188:
			HandleConsortiaBossDie(consortiaInfo);
			break;
		case 181:
		case 185:
		case 186:
			break;
		}
	}

	public void HandleConsortiaBossExtendAvailable(ConsortiaInfo consortia)
	{
		SendToAllConsortiaMember(consortia, 3);
	}

	public void HandleConsortiaBossReload(ConsortiaInfo consortia)
	{
		SendToAllConsortiaMember(consortia, -1);
	}

	public void HandleConsortiaBossCreateBoss(ConsortiaInfo consortia)
	{
		SendToAllConsortiaMember(consortia, 0);
	}

	public void HandleConsortiaBossClose(ConsortiaInfo consortia)
	{
		SendToAllConsortiaMember(consortia, 1);
	}

	public void HandleConsortiaBossDie(ConsortiaInfo consortia)
	{
		SendToAllConsortiaMember(consortia, 2);
	}

	public void SendToAllConsortiaMember(ConsortiaInfo consortia, int type)
	{
		if (!ConsortiaBossMgr.AddConsortia(consortia.ConsortiaID, consortia))
		{
			ConsortiaBossMgr.UpdateConsortia(consortia);
		}
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == consortia.ConsortiaID)
			{
				gamePlayer.SendConsortiaBossInfo(consortia);
				switch (type)
				{
				case 0:
					gamePlayer.SendConsortiaBossOpenClose(0);
					break;
				case 1:
					gamePlayer.SendConsortiaBossOpenClose(1);
					break;
				case 2:
					gamePlayer.SendConsortiaBossOpenClose(2);
					break;
				case 3:
					gamePlayer.SendConsortiaBossOpenClose(3);
					break;
				}
			}
		}
	}

	protected void HandleRSAKey(GSPacketIn packet)
	{
		RSAParameters parameters = default(RSAParameters);
		parameters.Modulus = packet.ReadBytes(128);
		parameters.Exponent = packet.ReadBytes();
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.ImportParameters(parameters);
		SendRSALogin(rSACryptoServiceProvider, _loginKey);
		SendListenIPPort(IPAddress.Parse(GameServer.Instance.Configuration.Ip), GameServer.Instance.Configuration.Port);
	}

	protected void HandleKitoffPlayer(object stateInfo)
	{
		try
		{
			GSPacketIn gSPacketIn = (GSPacketIn)stateInfo;
			int num = gSPacketIn.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (playerById != null)
			{
				string msg = gSPacketIn.ReadString();
				playerById.Out.SendKitoff(msg);
				playerById.Client.Disconnect();
			}
			else
			{
				SendUserOffline(num, 0);
			}
		}
		catch (Exception exception)
		{
			GameServer.log.Error("HandleKitoffPlayer", exception);
		}
	}

	protected void HandleAllowUserLogin(object stateInfo)
	{
		try
		{
			GSPacketIn gSPacketIn = (GSPacketIn)stateInfo;
			int num = gSPacketIn.ReadInt();
			if (!gSPacketIn.ReadBoolean())
			{
				return;
			}
			GamePlayer gamePlayer = LoginMgr.LoginClient(num);
			if (gamePlayer != null)
			{
				if (gamePlayer.Login())
				{
					SendUserOnline(gamePlayer.PlayerCharacter);
					WorldMgr.OnPlayerOnline(num, gamePlayer.PlayerCharacter.ConsortiaID);
				}
				else
				{
					gamePlayer.Client.Disconnect();
					SendUserOffline(num, 0);
				}
			}
			else
			{
				SendUserOffline(num, 0);
			}
		}
		catch (Exception exception)
		{
			GameServer.log.Error("HandleAllowUserLogin", exception);
		}
	}

	protected void HandleUserOffline(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int num2 = packet.ReadInt();
			int consortiaID = packet.ReadInt();
			if (LoginMgr.ContainsUser(num2))
			{
				SendAllowUserLogin(num2);
			}
			WorldMgr.OnPlayerOffline(num2, consortiaID);
		}
	}

	protected void HandleUserOnline(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int num2 = packet.ReadInt();
			int consortiaID = packet.ReadInt();
			LoginMgr.ClearLoginPlayer(num2);
			GamePlayer playerById = WorldMgr.GetPlayerById(num2);
			if (playerById != null)
			{
				GameServer.log.Error("Player hang in server!!!");
				playerById.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
				playerById.Client.Disconnect();
				WorldMgr.RemovePlayer(num2);
			}
			else
			{
				WorldMgr.OnPlayerOnline(num2, consortiaID);
			}
		}
	}

	protected void HandleChatPersonal(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string text = packet.ReadString();
		string sender = packet.ReadString();
		int playerID = packet.ReadInt();
		string msg = packet.ReadString();
		bool isAutoReply = packet.ReadBoolean();
		GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
		if (clientByPlayerNickName != null && !clientByPlayerNickName.IsBlackFriend(playerID))
		{
			num = clientByPlayerNickName.PlayerCharacter.ID;
			clientByPlayerNickName.SendPrivateChat(num, text, sender, msg, isAutoReply);
		}
	}

	protected void HandleChatAreaPersonal(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int zoneId = packet.ReadInt();
		string msg = packet.ReadString();
		string nickName = packet.ReadString();
		string sender = packet.ReadString();
		string zoneName = packet.ReadString();
		WorldMgr.GetClientByPlayerNickName(nickName)?.SendAreaPrivateChat(sender, zoneName, msg, zoneId);
	}

	protected void HandleBigBugle(GSPacketIn packet)
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.Out.SendTCP(packet);
		}
	}

	public void HandleFriend(GSPacketIn pkg)
	{
		switch (pkg.ReadByte())
		{
		case 165:
			HandleFriendState(pkg);
			break;
		case 166:
			HandleFirendResponse(pkg);
			break;
		}
	}

	public void HandleFriendState(GSPacketIn pkg)
	{
		WorldMgr.ChangePlayerState(pkg.ClientID, pkg.ReadInt(), pkg.ReadInt());
	}

	public void HandleFirendResponse(GSPacketIn packet)
	{
		int playerId = packet.ReadInt();
		WorldMgr.GetPlayerById(playerId)?.Out.SendTCP(packet);
	}

	public void HandleMailResponse(GSPacketIn packet)
	{
		int playerId = packet.ReadInt();
		WorldMgr.GetPlayerById(playerId)?.Out.SendTCP(packet);
	}

	public void HandleChargeMoney(GSPacketIn packet)
	{
		int clientID = packet.ClientID;
		WorldMgr.GetPlayerById(clientID)?.ChargeToUser();
	}

	public void HandleSystemNotice(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string str = packet.ReadString();
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteString(str);
		int num2 = num;
		int num3 = num2;
		if (num3 == 4)
		{
			gSPacketIn.WriteInt(0);
		}
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.Out.SendTCP(gSPacketIn);
		}
	}

	public void HandleASSState(GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		AntiAddictionMgr.SetASSState(flag);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.Out.SendAASControl(flag, gamePlayer.IsAASInfo, gamePlayer.IsMinor);
		}
	}

	public void HandleConfigState(GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		bool flag3 = (AwardMgr.DailyAwardState = packet.ReadBoolean());
		AntiAddictionMgr.SetASSState(flag);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.Out.SendAASControl(flag, gamePlayer.IsAASInfo, gamePlayer.IsMinor);
		}
	}

	public void HandleSysMess(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = num;
		int num3 = num2;
		if (num3 == 1)
		{
			int playerId = packet.ReadInt();
			string text = packet.ReadString();
			WorldMgr.GetPlayerById(playerId)?.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("LoginServerConnector.HandleSysMess.Msg1", text));
		}
	}

	protected void HandleChatConsortia(GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		bool flag = packet.ReadBoolean();
		string text = packet.ReadString();
		string text2 = packet.ReadString();
		int num = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	protected void HandleConsortiaResponse(GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 1:
			HandleConsortiaUserPass(packet);
			break;
		case 2:
			HandleConsortiaDelete(packet);
			break;
		case 3:
			HandleConsortiaUserDelete(packet);
			break;
		case 4:
			HandleConsortiaUserInvite(packet);
			break;
		case 5:
			HandleConsortiaBanChat(packet);
			break;
		case 6:
			HandleConsortiaUpGrade(packet);
			break;
		case 7:
			HandleConsortiaAlly(packet);
			break;
		case 8:
			HandleConsortiaDuty(packet);
			break;
		case 9:
			HandleConsortiaRichesOffer(packet);
			break;
		case 10:
			HandleConsortiaShopUpGrade(packet);
			break;
		case 11:
			HandleConsortiaStoreUpGrade(packet);
			break;
		case 12:
			HandleConsortiaSmithUpGrade(packet);
			break;
		case 13:
			HandleConsortiaSkillUpGrade(packet);
			break;
		}
	}

	public void HandleConsortiaFight(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		string message = packet.ReadString();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.Out.SendMessage(eMessageType.ChatNormal, message);
			}
		}
	}

	public void HandleConsortiaCreate(GSPacketIn packet)
	{
		int consortiaID = packet.ReadInt();
		int num = packet.ReadInt();
		ConsortiaMgr.AddConsortia(consortiaID);
	}

	public void HandleConsortiaUserPass(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		int num2 = packet.ReadInt();
		string consortiaName = packet.ReadString();
		int num3 = packet.ReadInt();
		string text = packet.ReadString();
		int num4 = packet.ReadInt();
		string text2 = packet.ReadString();
		int num5 = packet.ReadInt();
		string dutyName = packet.ReadString();
		int num6 = packet.ReadInt();
		int num7 = packet.ReadInt();
		int num8 = packet.ReadInt();
		DateTime dateTime = packet.ReadDateTime();
		int num9 = packet.ReadInt();
		int dutyLevel = packet.ReadInt();
		int num10 = packet.ReadInt();
		bool flag2 = packet.ReadBoolean();
		int right = packet.ReadInt();
		int num11 = packet.ReadInt();
		int num12 = packet.ReadInt();
		int num13 = packet.ReadInt();
		int consortiaRepute = packet.ReadInt();
		string text3 = packet.ReadString();
		int num14 = packet.ReadInt();
		int num15 = packet.ReadInt();
		string text4 = packet.ReadString();
		int num16 = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ID == num3)
			{
				gamePlayer.BeginChanges();
				gamePlayer.PlayerCharacter.ConsortiaID = num2;
				gamePlayer.PlayerCharacter.ConsortiaName = consortiaName;
				gamePlayer.PlayerCharacter.DutyName = dutyName;
				gamePlayer.PlayerCharacter.DutyLevel = dutyLevel;
				gamePlayer.PlayerCharacter.Right = right;
				gamePlayer.PlayerCharacter.ConsortiaRepute = consortiaRepute;
				ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(num2);
				if (consortiaInfo != null)
				{
					gamePlayer.PlayerCharacter.ConsortiaLevel = consortiaInfo.Level;
				}
				gamePlayer.CommitChanges();
			}
			if (gamePlayer.PlayerCharacter.ConsortiaID == num2)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaDelete(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.ClearConsortia();
				gamePlayer.AddRobRiches(-gamePlayer.PlayerCharacter.RichesRob);
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaUserDelete(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num2 || gamePlayer.PlayerCharacter.ID == num)
			{
				if (gamePlayer.PlayerCharacter.ID == num)
				{
					gamePlayer.ClearConsortia();
				}
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaUserInvite(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ID == num2)
			{
				gamePlayer.Out.SendTCP(packet);
				break;
			}
		}
	}

	public void HandleConsortiaBanChat(GSPacketIn packet)
	{
		bool isBanChat = packet.ReadBoolean();
		int num = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ID == num)
			{
				gamePlayer.PlayerCharacter.IsBanChat = isBanChat;
				gamePlayer.Out.SendTCP(packet);
				break;
			}
		}
	}

	public void HandleConsortiaUpGrade(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string text = packet.ReadString();
		int consortiaLevel = packet.ReadInt();
		ConsortiaMgr.ConsortiaUpGrade(num, consortiaLevel);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.PlayerCharacter.ConsortiaLevel = consortiaLevel;
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaStoreUpGrade(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string text = packet.ReadString();
		int storeLevel = packet.ReadInt();
		ConsortiaMgr.ConsortiaStoreUpGrade(num, storeLevel);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.PlayerCharacter.StoreLevel = storeLevel;
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaShopUpGrade(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string text = packet.ReadString();
		int shopLevel = packet.ReadInt();
		ConsortiaMgr.ConsortiaShopUpGrade(num, shopLevel);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.PlayerCharacter.ShopLevel = shopLevel;
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaSmithUpGrade(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string text = packet.ReadString();
		int smithLevel = packet.ReadInt();
		ConsortiaMgr.ConsortiaSmithUpGrade(num, smithLevel);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.PlayerCharacter.SmithLevel = smithLevel;
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaSkillUpGrade(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		string text = packet.ReadString();
		int skillLevel = packet.ReadInt();
		ConsortiaMgr.ConsortiaSkillUpGrade(num, skillLevel);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.PlayerCharacter.SkillLevel = skillLevel;
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaAlly(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int state = packet.ReadInt();
		ConsortiaMgr.UpdateConsortiaAlly(num, num2, state);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num || gamePlayer.PlayerCharacter.ConsortiaID == num2)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleConsortiaDuty(GSPacketIn packet)
	{
		int num = packet.ReadByte();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		string text = packet.ReadString();
		int num4 = packet.ReadInt();
		string dutyName = packet.ReadString();
		int right = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num2)
			{
				if (num == 2 && gamePlayer.PlayerCharacter.DutyLevel == num4)
				{
					gamePlayer.PlayerCharacter.DutyName = dutyName;
				}
				else if (gamePlayer.PlayerCharacter.ID == num3 && (num == 5 || num == 6 || num == 7 || num == 8 || num == 9))
				{
					gamePlayer.PlayerCharacter.DutyLevel = num4;
					gamePlayer.PlayerCharacter.DutyName = dutyName;
					gamePlayer.PlayerCharacter.Right = right;
				}
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleRate(GSPacketIn packet)
	{
		RateMgr.ReLoad();
	}

	public void HandleConsortiaRichesOffer(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == num)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void HandleUpdatePlayerMarriedState(GSPacketIn packet)
	{
		int playerId = packet.ReadInt();
		GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
		if (playerById != null)
		{
			playerById.LoadMarryProp();
			playerById.LoadMarryMessage();
			playerById.QuestInventory.ClearMarryQuest();
		}
	}

	public void HandleMarryRoomInfoToPlayer(GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GamePlayer playerById = WorldMgr.GetPlayerById(num);
		if (playerById != null)
		{
			packet.Code = 252;
			packet.ClientID = num;
			playerById.Out.SendTCP(packet);
		}
	}

	public void HandleWorldBossUpdate(GSPacketIn pkg)
	{
		RoomMgr.WorldBossRoom.UpdateWorldBoss(pkg);
	}

	public void HandleWorldBossUpdateBlood(GSPacketIn pkg)
	{
		RoomMgr.WorldBossRoom.SendUpdateBlood(pkg);
	}

	public void HandleWorldBossRank(GSPacketIn pkg)
	{
		RoomMgr.WorldBossRoom.UpdateWorldBossRankCrosszone(pkg);
	}

	public void HandleWorldBossPrivateInfo(GSPacketIn pkg)
	{
		string name = pkg.ReadString();
		int damage = pkg.ReadInt();
		int honor = pkg.ReadInt();
		RoomMgr.WorldBossRoom.SendPrivateInfo(name, damage, honor);
	}

	public void HandleWorldBossFightOver(GSPacketIn pkg)
	{
		RoomMgr.WorldBossRoom.SendFightOver();
	}

	public void HandleBattleGoundOpenClose(GSPacketIn pkg)
	{
		ActiveSystemMgr.UpdateIsBattleGoundOpen(pkg.ReadBoolean());
	}

	public void HandleFightFootballTime(GSPacketIn pkg)
	{
		ActiveSystemMgr.UpdateIsFightFootballTime(pkg.ReadBoolean());
	}

	public void HandleLeagueOpenClose(GSPacketIn pkg)
	{
		ActiveSystemMgr.UpdateIsLeagueOpen(pkg.ReadBoolean());
	}

	public void HandleWorldBossRoomClose(GSPacketIn pkg)
	{
		if (pkg.ReadByte() == 0)
		{
			RoomMgr.WorldBossRoom.SendRoomClose();
		}
		else
		{
			RoomMgr.WorldBossRoom.WorldBossClose();
		}
	}

	public void HandleShutdown(GSPacketIn pkg)
	{
		GameServer.Instance.Shutdown(6);
	}

	public void HandleMacroDrop(GSPacketIn pkg)
	{
		Dictionary<int, MacroDropInfo> dictionary = new Dictionary<int, MacroDropInfo>();
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int key = pkg.ReadInt();
			int dropCount = pkg.ReadInt();
			int maxDropCount = pkg.ReadInt();
			MacroDropInfo value = new MacroDropInfo(dropCount, maxDropCount);
			dictionary.Add(key, value);
		}
		MacroDropMgr.UpdateDropInfo(dictionary);
	}

	public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(1);
		gSPacketIn.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), fOAEP: false));
		SendTCP(gSPacketIn);
	}

	public void SendListenIPPort(IPAddress ip, int port)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(240);
		gSPacketIn.Write(ip.GetAddressBytes());
		gSPacketIn.WriteInt(port);
		SendTCP(gSPacketIn);
	}

	public void SendPingCenter()
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		int val = ((allPlayers != null) ? allPlayers.Length : 0);
		GSPacketIn gSPacketIn = new GSPacketIn(12);
		gSPacketIn.WriteInt(val);
		SendTCP(gSPacketIn);
	}

	public GSPacketIn SendUserOnline(Dictionary<int, int> users)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(5);
		gSPacketIn.WriteInt(users.Count);
		foreach (KeyValuePair<int, int> user in users)
		{
			gSPacketIn.WriteInt(user.Key);
			gSPacketIn.WriteInt(user.Value);
		}
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendUserOnline(PlayerInfo player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(5);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(player.ID);
		gSPacketIn.WriteInt(player.ConsortiaID);
		gSPacketIn.WriteString(player.NickName);
		gSPacketIn.WriteBoolean(player.Sex);
		gSPacketIn.WriteString(player.Style);
		gSPacketIn.WriteString(player.Colors);
		gSPacketIn.WriteString(player.Skin);
		gSPacketIn.WriteInt(player.Hide);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendUserOffline(int playerid, int consortiaID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(4);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteInt(consortiaID);
		SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void SendAllowUserLogin(int playerid)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(playerid);
		SendTCP(gSPacketIn);
	}

	public void SendMailResponse(int playerid)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(117);
		gSPacketIn.WriteInt(playerid);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaUserPass(int playerid, string playerName, ConsortiaUserInfo info, bool isInvite, int consortiaRepute, string loginName, int fightpower, int Offer)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128, playerid);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(info.ID);
		gSPacketIn.WriteBoolean(isInvite);
		gSPacketIn.WriteInt(info.ConsortiaID);
		gSPacketIn.WriteString(info.ConsortiaName);
		gSPacketIn.WriteInt(info.UserID);
		gSPacketIn.WriteString(info.UserName);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteString(playerName);
		gSPacketIn.WriteInt(info.DutyID);
		gSPacketIn.WriteString(info.DutyName);
		gSPacketIn.WriteInt(info.Offer);
		gSPacketIn.WriteInt(info.RichesOffer);
		gSPacketIn.WriteInt(info.RichesRob);
		gSPacketIn.WriteDateTime(info.LastDate);
		gSPacketIn.WriteInt(info.Grade);
		gSPacketIn.WriteInt(info.Level);
		gSPacketIn.WriteInt(info.State);
		gSPacketIn.WriteBoolean(info.Sex);
		gSPacketIn.WriteInt(info.Right);
		gSPacketIn.WriteInt(info.Win);
		gSPacketIn.WriteInt(info.Total);
		gSPacketIn.WriteInt(info.Escape);
		gSPacketIn.WriteInt(consortiaRepute);
		gSPacketIn.WriteString(loginName);
		gSPacketIn.WriteInt(fightpower);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString("Honor");
		gSPacketIn.WriteInt(info.RichesOffer + info.RichesRob);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaDelete(int consortiaID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(consortiaID);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaUserDelete(int playerid, int consortiaID, bool isKick, string nickName, string kickName)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteBoolean(isKick);
		gSPacketIn.WriteString(nickName);
		gSPacketIn.WriteString(kickName);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaInvite(int ID, int playerid, string playerName, int inviteID, string intviteName, string consortiaName, int consortiaID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(ID);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteString(playerName);
		gSPacketIn.WriteInt(inviteID);
		gSPacketIn.WriteString(intviteName);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteString(consortiaName);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaBanChat(int playerid, string playerName, int handleID, string handleName, bool isBan)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteBoolean(isBan);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteString(playerName);
		gSPacketIn.WriteInt(handleID);
		gSPacketIn.WriteString(handleName);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaFight(int consortiaID, int riches, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(158);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(riches);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaOffer(int consortiaID, int offer, int riches)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(156);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(offer);
		gSPacketIn.WriteInt(riches);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaCreate(int consortiaID, int offer, string consotiaName)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(130);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(offer);
		gSPacketIn.WriteString(consotiaName);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaUpGrade(ConsortiaInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(info.ConsortiaID);
		gSPacketIn.WriteString(info.ConsortiaName);
		gSPacketIn.WriteInt(info.Level);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaShopUpGrade(ConsortiaInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteInt(info.ConsortiaID);
		gSPacketIn.WriteString(info.ConsortiaName);
		gSPacketIn.WriteInt(info.ShopLevel);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaSmithUpGrade(ConsortiaInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(12);
		gSPacketIn.WriteInt(info.ConsortiaID);
		gSPacketIn.WriteString(info.ConsortiaName);
		gSPacketIn.WriteInt(info.SmithLevel);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaStoreUpGrade(ConsortiaInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteInt(info.ConsortiaID);
		gSPacketIn.WriteString(info.ConsortiaName);
		gSPacketIn.WriteInt(info.StoreLevel);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaKillUpGrade(ConsortiaInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(13);
		gSPacketIn.WriteInt(info.ConsortiaID);
		gSPacketIn.WriteString(info.ConsortiaName);
		gSPacketIn.WriteInt(info.SkillLevel);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaAlly(int consortiaID1, int consortiaID2, int state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(consortiaID1);
		gSPacketIn.WriteInt(consortiaID2);
		gSPacketIn.WriteInt(state);
		SendTCP(gSPacketIn);
		ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
	}

	public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID)
	{
		SendConsortiaDuty(info, updateType, consortiaID, 0, "", 0, "");
	}

	public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID, int playerID, string playerName, int handleID, string handleName)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(8);
		gSPacketIn.WriteByte((byte)updateType);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(playerID);
		gSPacketIn.WriteString(playerName);
		gSPacketIn.WriteInt(info.Level);
		gSPacketIn.WriteString(info.DutyName);
		gSPacketIn.WriteInt(info.Right);
		gSPacketIn.WriteInt(handleID);
		gSPacketIn.WriteString(handleName);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaRichesOffer(int consortiaID, int playerID, string playerName, int riches)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(playerID);
		gSPacketIn.WriteString(playerName);
		gSPacketIn.WriteInt(riches);
		SendTCP(gSPacketIn);
	}

	public void SendUpdatePlayerMarriedStates(int playerId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(13);
		gSPacketIn.WriteInt(playerId);
		SendTCP(gSPacketIn);
	}

	public void SendMarryRoomDisposeToPlayer(int roomId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(241);
		gSPacketIn.WriteInt(roomId);
		SendTCP(gSPacketIn);
	}

	public void SendMarryRoomInfoToPlayer(int playerId, bool state, MarryRoomInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(14);
		gSPacketIn.WriteInt(playerId);
		gSPacketIn.WriteBoolean(state);
		if (state)
		{
			gSPacketIn.WriteInt(info.ID);
			gSPacketIn.WriteString(info.Name);
			gSPacketIn.WriteInt(info.MapIndex);
			gSPacketIn.WriteInt(info.AvailTime);
			gSPacketIn.WriteInt(info.PlayerID);
			gSPacketIn.WriteInt(info.GroomID);
			gSPacketIn.WriteInt(info.BrideID);
			gSPacketIn.WriteDateTime(info.BeginTime);
			gSPacketIn.WriteBoolean(info.IsGunsaluteUsed);
			gSPacketIn.WriteInt(info.SeniorType);
		}
		SendTCP(gSPacketIn);
	}

	public void SendShutdown(bool isStoping)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(15);
		gSPacketIn.WriteInt(_serverId);
		gSPacketIn.WriteBoolean(isStoping);
		SendTCP(gSPacketIn);
	}

	public void SendBombKingUpdatePlayer(BombKingPlayerInfo p, int score, bool win)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(306);
		gSPacketIn.WriteInt(p.UserID);
		gSPacketIn.WriteInt(p.AreaID);
		gSPacketIn.WriteString(p.NickName);
		gSPacketIn.WriteString(p.AreaName);
		gSPacketIn.WriteInt(p.vipType);
		gSPacketIn.WriteInt(p.vipLevel);
		gSPacketIn.WriteString(p.style);
		gSPacketIn.WriteString(p.color);
		gSPacketIn.WriteBoolean(p.sex);
		gSPacketIn.WriteString(p.smg);
		gSPacketIn.WriteInt(score);
		gSPacketIn.WriteBoolean(win);
		SendTCP(gSPacketIn);
	}

	public void SendPacket(GSPacketIn packet)
	{
		SendTCP(packet);
	}

	public LoginServerConnector(string ip, int port, int serverid, int zoneId, string name, byte[] readBuffer, byte[] sendBuffer)
		: base(ip, port, autoReconnect: true, readBuffer, sendBuffer)
	{
		_serverId = serverid;
		_zoneId = zoneId;
		_loginKey = $"{serverid},{name}";
		base.Strict = true;
	}
}

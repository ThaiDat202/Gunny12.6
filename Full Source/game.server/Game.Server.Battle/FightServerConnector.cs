using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Logic.Protocol;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.RingStation;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Battle;

public class FightServerConnector : BaseConnector
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private BattleServer m_server;

	private string m_key;

	protected override void OnDisconnect()
	{
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
			GSPacketIn gSPacketIn = state as GSPacketIn;
			int code = gSPacketIn.Code;
			switch (code)
			{
			case 23:
				HandlePlayerAddMoneyPVP(gSPacketIn);
				break;
			case 0:
				HandleRSAKey(gSPacketIn);
				break;
			case 65:
				HandleRoomRemove(gSPacketIn);
				break;
			case 67:
				HandleSendToRoom(gSPacketIn);
				break;
			case 32:
				HandleSendToPlayer(gSPacketIn);
				break;
			case 66:
				HandleStartGame(gSPacketIn);
				break;
			case 68:
				HandleStopGame(gSPacketIn);
				break;
			case 33:
				HandleUpdatePlayerGameId(gSPacketIn);
				break;
			case 34:
				HandleDisconnectPlayer(gSPacketIn);
				break;
			case 35:
				HandlePlayerOnGameOver(gSPacketIn);
				break;
			case 36:
				HandlePlayerOnUsingItem(gSPacketIn);
				break;
			case 38:
				HandlePlayerAddGold(gSPacketIn);
				break;
			case 74:
				HandlePlayerAddMoney(gSPacketIn);
				break;
			case 205:
				HandlePlayerAddScoreCityBattle(gSPacketIn);
				break;
			case 89:
				HandlePlayerAddActiveMoney(gSPacketIn);
				break;
			case 75:
				HandlePlayerAddGiftToken(gSPacketIn);
				break;
			case 76:
				HandlePlayerAddMedal(gSPacketIn);
				break;
			case 39:
				HandlePlayerAddGP(gSPacketIn);
				break;
			case 91:
				HandlePlayerAddBoatScore(gSPacketIn);
				break;
			case 40:
				HandlePlayerOnKillingLiving(gSPacketIn);
				break;
			case 41:
				HandlePlayerOnMissionOver(gSPacketIn);
				break;
			case 42:
				HandlePlayerConsortiaFight(gSPacketIn);
				break;
			case 43:
				HandlePlayerSendConsortiaFight(gSPacketIn);
				break;
			case 44:
				HandlePlayerRemoveGold(gSPacketIn);
				break;
			case 45:
				HandlePlayerRemoveMoney(gSPacketIn);
				break;
			case 48:
				HandlePlayerAddTemplate1(gSPacketIn);
				break;
			case 77:
				HandleFindConsortiaAlly(gSPacketIn);
				break;
			case 49:
				HandlePlayerRemoveGP(gSPacketIn);
				break;
			case 50:
				HandlePlayerRemoveOffer(gSPacketIn);
				break;
			case 19:
				HandlePlayerChatSend(gSPacketIn);
				break;
			case 73:
				HandlePlayerHealstone(gSPacketIn);
				break;
			case 84:
				HandlePlayerAddLeagueMoney(gSPacketIn);
				break;
			case 85:
				HandlePlayerAddPrestige(gSPacketIn);
				break;
			case 86:
				HandlePlayerUpdateRestCount(gSPacketIn);
				break;
			case 87:
				HandleFootballTakeOut(gSPacketIn);
				break;
			case 88:
				HandleFightNPC(gSPacketIn);
				break;
			case 90:
				HandleRingstationResult(gSPacketIn);
				break;
			case 93:
				HandleCreateEnterModeItems(gSPacketIn);
				break;
			case 92:
				HandlePlayerAddEnterModePoint(gSPacketIn);
				break;
			case 203:
				HandlePlayerUserMoney(gSPacketIn);
				break;
			case 204:
				HandlePlayerAddScoreBombKing(gSPacketIn);
				break;
			case 301:
				HandleUpdatePublicPlayer(gSPacketIn);
				break;
			default:
			{
				eFightPackageType eFightPackageType = (eFightPackageType)code;
				Console.WriteLine("??????????LoginServerConnector: " + eFightPackageType);
				break;
			}
			}
		}
		catch (Exception exception)
		{
			GameServer.log.Error("AsynProcessPacket", exception);
		}
	}

	private void HandleUpdatePublicPlayer(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.UpdatePublicPlayer(pkg.ReadString());
	}

	private void HandlePlayerAddMoneyPVP(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddCupomPVP(pkg.Parameter1);
	}

	private void HandlePlayerAddScoreBombKing(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddScoreBombKing(pkg.Parameter1);
	}

	private void HandlePlayerUserMoney(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.UpdateUserMoney(pkg.ReadInt(), pkg.ReadInt());
	}

	private void HandleCreateEnterModeItems(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.CreateRandomEnterModeItem();
	}

	private void HandlePlayerAddEnterModePoint(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddEnterModePoint(pkg.Parameter1);
	}

	private void HandlePlayerChatSend(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.SendMessage(pkg.ReadString());
	}

	public void HandleFindConsortiaAlly(GSPacketIn pkg)
	{
		int state = ConsortiaMgr.FindConsortiaAlly(pkg.ReadInt(), pkg.ReadInt());
		SendFindConsortiaAlly(state, pkg.ReadInt());
	}

	private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		AbstractGame game = playerById.CurrentRoom.Game;
		playerById?.OnKillingLiving(game, pkg.ReadInt(), pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
	}

	private void HandlePlayerOnMissionOver(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		AbstractGame game = playerById.CurrentRoom.Game;
		playerById?.OnMissionOver(game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
	}

	private void HandlePlayerConsortiaFight(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		Dictionary<int, Player> dictionary = new Dictionary<int, Player>();
		int consortiaWin = pkg.ReadInt();
		int consortiaLose = pkg.ReadInt();
		int num = pkg.ReadInt();
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			GamePlayer playerById2 = WorldMgr.GetPlayerById(pkg.ReadInt());
			if (playerById2 != null)
			{
				Player value = new Player(playerById2, 0, null, 0, playerById2.PlayerCharacter.hp);
				dictionary.Add(i, value);
			}
		}
		eRoomType roomType = (eRoomType)pkg.ReadByte();
		eGameType gameClass = (eGameType)pkg.ReadByte();
		int totalKillHealth = pkg.ReadInt();
		if (playerById != null)
		{
			num2 = playerById.ConsortiaFight(consortiaWin, consortiaLose, dictionary, roomType, gameClass, totalKillHealth, num);
		}
		if (num2 == 0)
		{
		}
	}

	private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.SendConsortiaFight(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
	}

	private void HandlePlayerRemoveGold(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.RemoveGold(pkg.ReadInt());
	}

	private void HandlePlayerRemoveMoney(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.RemoveMoney(pkg.ReadInt());
	}

	private void HandlePlayerRemoveOffer(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.RemoveOffer(pkg.ReadInt());
	}

	private void HandlePlayerAddTemplate1(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		if (playerById == null)
		{
			return;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(pkg.ReadInt());
		eBageType bagType = (eBageType)pkg.ReadByte();
		if (itemTemplateInfo == null)
		{
			return;
		}
		int count = pkg.ReadInt();
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 118);
		itemInfo.Count = count;
		itemInfo.ValidDate = pkg.ReadInt();
		itemInfo.IsBinds = pkg.ReadBoolean();
		itemInfo.IsUsed = pkg.ReadBoolean();
		itemInfo.StrengthenLevel = pkg.ReadInt();
		itemInfo.AttackCompose = pkg.ReadInt();
		itemInfo.DefendCompose = pkg.ReadInt();
		itemInfo.AgilityCompose = pkg.ReadInt();
		itemInfo.LuckCompose = pkg.ReadInt();
		itemInfo.MagicAttack = pkg.ReadInt();
		itemInfo.MagicDefence = pkg.ReadInt();
		if (pkg.ReadBoolean())
		{
			GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(itemTemplateInfo.TemplateID);
			if (goldEquipTemplateInfo != null)
			{
				ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(goldEquipTemplateInfo.NewTemplateId);
				if (itemTemplateInfo2 != null)
				{
					itemInfo.GoldEquip = itemTemplateInfo2;
					itemInfo.goldBeginTime = pkg.ReadDateTime();
					itemInfo.goldValidDate = pkg.ReadInt();
				}
			}
		}
		playerById.AddTemplate(itemInfo, bagType, itemInfo.Count, eGameView.BatleTypeGet);
	}

	private void HandlePlayerAddBoatScore(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddBoatScore(pkg.Parameter1);
	}

	private void HandlePlayerAddGP(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddGP(pkg.Parameter1);
	}

	private void HandlePlayerAddMoney(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddMoney(pkg.Parameter1);
	}

	private void HandlePlayerAddScoreCityBattle(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddScoreCityBattle(pkg.Parameter1);
	}

	private void HandlePlayerAddActiveMoney(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddActiveMoney(pkg.Parameter1);
	}

	private void HandlePlayerAddGiftToken(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddGiftToken(pkg.Parameter1);
	}

	private void HandlePlayerHealstone(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.RemoveHealstone();
	}

	private void HandlePlayerAddMedal(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddMedal(pkg.Parameter1);
	}

	private void HandlePlayerAddLeagueMoney(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddLeagueMoney(pkg.Parameter1);
	}

	private void HandlePlayerAddPrestige(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddPrestige(pkg.ReadBoolean(), (eRoomType)pkg.Parameter1);
	}

	private void HandleRingstationResult(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.RingstationResult(pkg.ReadBoolean());
	}

	private void HandlePlayerUpdateRestCount(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.UpdateRestCount();
	}

	private void HandleFootballTakeOut(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		if (playerById != null)
		{
			bool isWin = pkg.ReadBoolean();
			playerById.FootballTakeOut(isWin);
		}
	}

	private void HandlePlayerRemoveGP(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.RemoveGP(pkg.Parameter1);
	}

	private void HandlePlayerAddGold(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.AddGold(pkg.Parameter1);
	}

	private void HandlePlayerOnUsingItem(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		if (playerById != null)
		{
			int templateId = pkg.ReadInt();
			bool result = playerById.UsePropItem(null, pkg.Parameter1, pkg.Parameter2, templateId, pkg.ReadBoolean());
			SendUsingPropInGame(playerById.CurrentRoom.Game.Id, playerById.GamePlayerId, templateId, result);
		}
	}

	private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(36, gameId);
		gSPacketIn.Parameter1 = playerId;
		gSPacketIn.Parameter2 = templateId;
		gSPacketIn.WriteBoolean(result);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(83, gameId);
		gSPacketIn.Parameter1 = playerId;
		SendTCP(gSPacketIn);
	}

	public void SendKitOffPlayer(int playerid)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(4);
		gSPacketIn.WriteInt(playerid);
		SendTCP(gSPacketIn);
	}

	private void HandlePlayerOnGameOver(GSPacketIn pkg)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
		if (playerById != null && playerById.CurrentRoom != null && playerById.CurrentRoom.Game != null)
		{
			playerById.OnGameOver(playerById.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt());
		}
	}

	private void HandleDisconnectPlayer(GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(pkg.ClientID)?.Disconnect();
	}

	public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(1);
		gSPacketIn.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), fOAEP: false));
		SendTCP(gSPacketIn);
	}

	protected void HandleRSAKey(GSPacketIn packet)
	{
		RSAParameters parameters = default(RSAParameters);
		parameters.Modulus = packet.ReadBytes(128);
		parameters.Exponent = packet.ReadBytes();
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.ImportParameters(parameters);
		SendRSALogin(rSACryptoServiceProvider, m_key);
	}

	public FightServerConnector(BattleServer server, string ip, int port, string key)
		: base(ip, port, autoReconnect: true, new byte[16384], new byte[16384])
	{
		m_server = server;
		m_key = key;
		base.Strict = true;
	}

	public void SendAddRoom(BaseRoom room)
	{
		bool flag = room.GameType == eGameType.BattleGame || room.GameType == eGameType.RingStation || room.RoomType == eRoomType.Entertainment || room.RoomType == eRoomType.EntertainmentPK;
		GSPacketIn gSPacketIn = new GSPacketIn(64);
		gSPacketIn.WriteInt(room.RoomId);
		gSPacketIn.WriteInt(room.PickUpNpcId);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteBoolean(room.StartWithNpc);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteBoolean(room.isCrosszone);
		gSPacketIn.WriteInt((int)room.RoomType);
		gSPacketIn.WriteInt((int)room.GameType);
		gSPacketIn.WriteInt(room.groupType);
		gSPacketIn.WriteInt(room.GuildId);
		List<GamePlayer> players = room.GetPlayers();
		gSPacketIn.WriteInt(players.Count);
		foreach (GamePlayer item in players)
		{
			gSPacketIn.WriteInt(item.ZoneId);
			gSPacketIn.WriteString(item.ZoneName);
			gSPacketIn.WriteInt(item.BombKingDelay);
			gSPacketIn.WriteInt(item.battleId);
			gSPacketIn.WriteInt(item.PlayerCharacter.ID);
			gSPacketIn.WriteString(item.PlayerCharacter.NickName);
			gSPacketIn.WriteBoolean(item.PlayerCharacter.Sex);
			gSPacketIn.WriteByte(item.PlayerCharacter.typeVIP);
			gSPacketIn.WriteInt(item.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteInt(item.PlayerCharacter.Hide);
			gSPacketIn.WriteString(item.PlayerCharacter.Style);
			gSPacketIn.WriteString(item.CreateFightFootballStyle(1));
			gSPacketIn.WriteString(item.CreateFightFootballStyle(2));
			gSPacketIn.WriteString(item.PlayerCharacter.Colors);
			gSPacketIn.WriteString(item.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(item.PlayerCharacter.Offer);
			gSPacketIn.WriteInt((item.PlayerCharacter.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(item.PlayerCharacter.GP));
			gSPacketIn.WriteInt(item.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(item.PlayerCharacter.Repute);
			gSPacketIn.WriteInt(item.PlayerCharacter.ConsortiaID);
			gSPacketIn.WriteString(item.PlayerCharacter.ConsortiaName);
			gSPacketIn.WriteInt(item.PlayerCharacter.ConsortiaLevel);
			gSPacketIn.WriteInt(item.PlayerCharacter.ConsortiaRepute);
			gSPacketIn.WriteInt(item.PlayerCharacter.badgeID);
			gSPacketIn.WriteString(item.PlayerCharacter.WeaklessGuildProgressStr);
			gSPacketIn.WriteString(item.PlayerCharacter.Honor);
			if (room.GameType == eGameType.BattleGame)
			{
				gSPacketIn.WriteInt(item.BattleData.Attack);
				gSPacketIn.WriteInt(item.BattleData.Defend);
				gSPacketIn.WriteInt(item.BattleData.Agility);
				gSPacketIn.WriteInt(item.BattleData.Lucky);
				gSPacketIn.WriteInt(item.BattleData.Blood);
			}
			else
			{
				gSPacketIn.WriteInt(item.PlayerCharacter.Attack);
				gSPacketIn.WriteInt(item.PlayerCharacter.Defence);
				gSPacketIn.WriteInt(item.PlayerCharacter.Agility);
				gSPacketIn.WriteInt(item.PlayerCharacter.Luck);
				gSPacketIn.WriteInt(item.PlayerCharacter.hp);
			}
			gSPacketIn.WriteInt(item.PlayerCharacter.MagicAttack);
			gSPacketIn.WriteInt(item.PlayerCharacter.MagicDefence);
			if (GameProperties.VERSION >= 11000)
			{
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackMax);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackSKill);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackSKill_Value);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackSKill_Rate);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackMax);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackSKill);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackSKill_Value);
				gSPacketIn.WriteInt(item.PlayerCharacter.EmblemInfoAttackSKill_Rate);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.Crit);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.Kill);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.Guard);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.Speed);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.Tenacity);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.WillFight);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.AvoidInjury);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.SunderArmor);
				gSPacketIn.WriteInt(item.PlayerCharacter.SecondPro.ViolenceInjury);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.FireAttack);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.WaterAttack);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.WindAttack);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.LandAttack);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.FireDefence);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.WaterDefence);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.WindDefence);
				gSPacketIn.WriteInt(item.PlayerCharacter.ElementPro.LandDefence);
				gSPacketIn.WriteInt(item.PlayerCharacter.CurrentSealSkill);
			}
			gSPacketIn.WriteInt(item.PlayerCharacter.FightPower);
			gSPacketIn.WriteInt(item.PlayerCharacter.AcademyInfo.ApprenticeshipState);
			gSPacketIn.WriteInt(item.PlayerCharacter.AcademyInfo.MasterID);
			gSPacketIn.WriteString(item.PlayerCharacter.AcademyInfo.MasterOrApprentices);
			gSPacketIn.WriteString(item.PlayerCharacter.GhostEquipList);
			gSPacketIn.WriteBoolean(item.PlayerCharacter.IsMarried);
			if (item.PlayerCharacter.IsMarried)
			{
				gSPacketIn.WriteInt(item.PlayerCharacter.SpouseID);
				gSPacketIn.WriteString(item.PlayerCharacter.SpouseName);
				gSPacketIn.WriteBoolean(item.PlayerCharacter.ringFlag);
				gSPacketIn.WriteInt(item.PlayerCharacter.loveBuffLevel);
				gSPacketIn.WriteInt(item.PlayerCharacter.loveBuffValue);
			}
			gSPacketIn.WriteInt(item.PlayerCharacter.guardCoreID);
			if (room.GameType == eGameType.BattleGame)
			{
				gSPacketIn.WriteDouble(item.BattleData.Damage);
				gSPacketIn.WriteDouble(item.BattleData.Guard);
				gSPacketIn.WriteDouble(item.BattleData.BaseAgility);
				gSPacketIn.WriteDouble(item.BattleData.Blood);
			}
			else
			{
				gSPacketIn.WriteDouble(item.BaseAttack);
				gSPacketIn.WriteDouble(item.BaseDefence);
				gSPacketIn.WriteDouble(item.GetBaseAgility());
				gSPacketIn.WriteDouble(item.GetBaseBlood());
			}
			gSPacketIn.WriteInt(item.MainWeapon.TemplateID);
			gSPacketIn.WriteInt(item.MainWeapon.StrengthenLevel);
			if (item.MainWeapon.GoldEquip == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(item.MainWeapon.GoldEquip.TemplateID);
				gSPacketIn.WriteDateTime(item.MainWeapon.goldBeginTime);
				gSPacketIn.WriteInt(item.MainWeapon.goldValidDate);
			}
			gSPacketIn.WriteBoolean(item.CanUseProp);
			if (item.SecondWeapon != null && room.GameType != eGameType.BattleGame)
			{
				gSPacketIn.WriteInt(item.SecondWeapon.TemplateID);
				gSPacketIn.WriteInt(item.SecondWeapon.StrengthenLevel);
				gSPacketIn.WriteInt(item.SecondWeapon.curExp);
			}
			else
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
			if (item.Weapon3D != null && room.GameType != eGameType.BattleGame)
			{
				gSPacketIn.WriteInt(item.Weapon3D.TemplateID);
			}
			else
			{
				gSPacketIn.WriteInt(0);
			}
			if (item.Healstone != null && room.GameType != eGameType.BattleGame)
			{
				gSPacketIn.WriteInt(item.Healstone.TemplateID);
				gSPacketIn.WriteInt(item.Healstone.Count);
			}
			else
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
			if (item.Amulet == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(item.Amulet.TemplateID);
				gSPacketIn.WriteInt(item.Amulet.StrengthenLevel);
				gSPacketIn.WriteInt(item.Amulet.Hole1);
				gSPacketIn.WriteInt(item.Amulet.Hole2);
				gSPacketIn.WriteInt(item.Amulet.Hole3);
				gSPacketIn.WriteInt(item.Amulet.Hole4);
				gSPacketIn.WriteInt(item.Amulet.Hole5);
				gSPacketIn.WriteInt(item.Amulet.Hole6);
				gSPacketIn.WriteInt(item.Amulet.Hole5Exp);
				gSPacketIn.WriteInt(item.Amulet.Hole6Exp);
			}
			gSPacketIn.WriteDouble((double)RateMgr.GetRate(eRateType.Experience_Rate) * AntiAddictionMgr.GetAntiAddictionCoefficient(item.PlayerCharacter.AntiAddiction) * ((item.GPAddPlus == 0.0) ? 1.0 : item.GPAddPlus));
			gSPacketIn.WriteDouble(AntiAddictionMgr.GetAntiAddictionCoefficient(item.PlayerCharacter.AntiAddiction) * ((item.OfferAddPlus == 0.0) ? 1.0 : item.OfferAddPlus));
			gSPacketIn.WriteDouble(RateMgr.GetRate(eRateType.Experience_Rate));
			gSPacketIn.WriteInt(GameServer.Instance.Configuration.ServerID);
			gSPacketIn.WriteBoolean(item.DragonBoatOpen());
			gSPacketIn.WriteInt(item.DragonBoatAddExpPlus());
			gSPacketIn.WriteString(item.TcpEndPoint());
			gSPacketIn.WriteInt(item.BattleData.MatchInfo.restCount);
			gSPacketIn.WriteInt(item.BattleData.MatchInfo.maxCount);
			bool val = false;
			if (flag)
			{
				gSPacketIn.WriteBoolean(val);
			}
			else
			{
				val = item.Pet != null;
				gSPacketIn.WriteBoolean(val);
				if (val)
				{
					gSPacketIn.WriteInt(item.Pet.Place);
					gSPacketIn.WriteInt(item.Pet.TemplateID);
					gSPacketIn.WriteInt(item.Pet.ID);
					gSPacketIn.WriteString(item.Pet.Name);
					gSPacketIn.WriteInt(item.Pet.UserID);
					gSPacketIn.WriteInt(item.Pet.Level);
					gSPacketIn.WriteString(item.Pet.Skill);
					gSPacketIn.WriteString(item.Pet.SkillEquip);
				}
			}
			if (flag)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				List<AbstractBuffer> allBufferByTemplate = item.BufferList.GetAllBufferByTemplate();
				gSPacketIn.WriteInt(allBufferByTemplate.Count);
				foreach (AbstractBuffer item2 in allBufferByTemplate)
				{
					BufferInfo info = item2.Info;
					gSPacketIn.WriteInt(info.Type);
					gSPacketIn.WriteBoolean(info.IsExist);
					gSPacketIn.WriteDateTime(info.BeginDate);
					gSPacketIn.WriteInt(info.ValidDate);
					gSPacketIn.WriteInt(info.Value);
					gSPacketIn.WriteInt(info.ValidCount);
					gSPacketIn.WriteInt(info.TemplateID);
				}
			}
			if (flag)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(item.EquipEffect.Count);
				foreach (ItemInfo item3 in item.EquipEffect)
				{
					gSPacketIn.WriteInt(item3.TemplateID);
					gSPacketIn.WriteInt(item3.Hole1);
				}
			}
			if (flag)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(item.FightBuffs.Count);
				foreach (BufferInfo fightBuff in item.FightBuffs)
				{
					gSPacketIn.WriteInt(fightBuff.Type);
					gSPacketIn.WriteInt(fightBuff.Value);
				}
			}
			gSPacketIn.WriteInt(item.Horse.curUsedSkill.Count);
			foreach (int value in item.Horse.curUsedSkill.Values)
			{
				gSPacketIn.WriteInt(value);
			}
			gSPacketIn.WriteInt(item.BatleConfig.MagicHouse);
			gSPacketIn.WriteInt(item.BatleConfig.PetFormReduceDamage);
			gSPacketIn.WriteInt(item.HorseAmuletEquipEffect.Count);
			foreach (ItemInfo item4 in item.HorseAmuletEquipEffect)
			{
				gSPacketIn.WriteInt(item4.TemplateID);
				gSPacketIn.WriteInt(item4.Hole1);
			}
			gSPacketIn.WriteInt(item.PetSealSkillEquip.Length);
			int[] petSealSkillEquip = item.PetSealSkillEquip;
			foreach (int val2 in petSealSkillEquip)
			{
				gSPacketIn.WriteInt(val2);
			}
			gSPacketIn.WriteInt(item.QuestColect.Count);
			foreach (int item5 in item.QuestColect)
			{
				gSPacketIn.WriteInt(item5);
			}
			gSPacketIn.WriteInt(item.PlayerCharacter.badgePvpid);
			if (GameProperties.batismoNoPvp && item.PvePowerBuff != null && item.PvePowerBuff.IsActive())
			{
				gSPacketIn.WriteBoolean(val: true);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffAtk);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffDef);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffAgl);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffLuck);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffDmg);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffAr);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffHp);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffMAtk);
				gSPacketIn.WriteInt(item.PvePowerBuff.getBuffMDef);
				gSPacketIn.WriteDateTime(item.PvePowerBuff.getBuffDate);
			}
			else
			{
				gSPacketIn.WriteBoolean(val: false);
			}
			if (GameProperties.VERSION >= 11000)
			{
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamID);
				gSPacketIn.WriteString(item.PlayerCharacter.Team.teamName);
				gSPacketIn.WriteString(item.PlayerCharacter.Team.teamTag);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamGrade);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamWinTime);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamTotalTime);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamDivision);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamScore);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamDuty);
				gSPacketIn.WriteInt(item.PlayerCharacter.Team.teamPersonalScore);
			}
		}
		gSPacketIn.Compress();
		SendTCP(gSPacketIn);
	}

	private void HandleFightNPC(GSPacketIn packet)
	{
		int roomtype = packet.ReadInt();
		int gametype = packet.ReadInt();
		int npcId = packet.ReadInt();
		int playerCount = packet.ReadInt();
		GamePlayer playerById = WorldMgr.GetPlayerById(packet.Parameter1);
		if (playerById != null)
		{
			RingStationMgr.CreateAutoBot(playerById, roomtype, gametype, npcId, playerCount);
		}
	}

	public void SendRemoveRoom(BaseRoom room)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(65);
		gSPacketIn.Parameter1 = room.RoomId;
		SendTCP(gSPacketIn);
	}

	public void SendToGame(int gameId, GSPacketIn pkg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(2, gameId);
		gSPacketIn.WritePacket(pkg);
		SendTCP(gSPacketIn);
	}

	protected void HandleRoomRemove(GSPacketIn packet)
	{
		m_server.RemoveRoomImp(packet.ClientID);
	}

	protected void HandleStartGame(GSPacketIn pkg)
	{
		ProxyGame game = new ProxyGame(pkg.Parameter2, this, (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt());
		m_server.StartGame(pkg.Parameter1, game);
	}

	protected void HandleStopGame(GSPacketIn pkg)
	{
		int parameter = pkg.Parameter1;
		int parameter2 = pkg.Parameter2;
		m_server.StopGame(parameter, parameter2);
	}

	protected void HandleSendToRoom(GSPacketIn pkg)
	{
		int clientID = pkg.ClientID;
		GSPacketIn pkg2 = pkg.ReadPacket();
		m_server.SendToRoom(clientID, pkg2, pkg.Parameter1, pkg.Parameter2);
	}

	protected void HandleSendToPlayer(GSPacketIn pkg)
	{
		int clientID = pkg.ClientID;
		try
		{
			GSPacketIn pkg2 = pkg.ReadPacket();
			m_server.SendToUser(clientID, pkg2);
		}
		catch (Exception exception)
		{
			log.Error($"pkg len:{pkg.Length}", exception);
			log.Error(Marshal.ToHexDump("pkg content:", pkg.Buffer, 0, pkg.Length));
		}
	}

	private void HandleUpdatePlayerGameId(GSPacketIn pkg)
	{
		m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
	}

	public void SendChangeGameType(BaseRoom room)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(72)
		{
			Parameter1 = room.RoomId
		};
		gSPacketIn.WriteInt((int)room.GameType);
		SendTCP(gSPacketIn);
	}

	public void SendChatMessage(string msg, GamePlayer player, bool team)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(19, player.CurrentRoom.Game.Id);
		gSPacketIn.WriteInt(player.GamePlayerId);
		gSPacketIn.WriteBoolean(team);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendSmileChatMessage(GSPacketIn pkg)
	{
		SendTCP(pkg);
	}

	public void SendFightNotice(GamePlayer player, int GameId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3, GameId);
		gSPacketIn.Parameter1 = player.GamePlayerId;
		SendTCP(gSPacketIn);
	}

	public void SendFindConsortiaAlly(int state, int gameid)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(77, gameid);
		gSPacketIn.WriteInt(state);
		gSPacketIn.WriteInt((int)RateMgr.GetRate(eRateType.Riches_Rate));
		SendTCP(gSPacketIn);
	}
}

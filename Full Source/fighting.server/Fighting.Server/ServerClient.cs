using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Fighting.Server.GameObjects;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Fighting.Server;

public class ServerClient : BaseClient
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private RSACryptoServiceProvider m_rsa;

	private FightServer m_svr;

	private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

	protected override void OnConnect()
	{
		base.OnConnect();
		m_rsa = new RSACryptoServiceProvider();
		RSAParameters rSAParameters = m_rsa.ExportParameters(includePrivateParameters: false);
		SendRSAKey(rSAParameters.Modulus, rSAParameters.Exponent);
	}

	protected override void OnDisconnect()
	{
		base.OnDisconnect();
		m_rsa = null;
	}

	public override void OnRecvPacket(GSPacketIn pkg)
	{
		switch (pkg.Code)
		{
		case 1:
			HandleLogin(pkg);
			break;
		case 64:
			HandleGameRoomCreate(pkg);
			break;
		case 65:
			HandleGameRoomCancel(pkg);
			break;
		case 2:
			HanleSendToGame(pkg);
			break;
		case 36:
			HandlePlayerUsingProp(pkg);
			break;
		case 83:
			HandlePlayerExit(pkg);
			break;
		case 19:
			HandlePlayerMessage(pkg);
			break;
		case 20:
			HandlePlayerSceneFace(pkg);
			break;
		case 3:
			HandleSysNotice(pkg);
			break;
		case 77:
			HandleConsortiaAlly(pkg);
			break;
		}
	}

	private void HandlePlayerUsingProp(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame == null)
		{
			return;
		}
		baseGame.Resume();
		if (pkg.ReadBoolean())
		{
			Player player = baseGame.FindPlayer(pkg.Parameter1);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(pkg.Parameter2);
			if (player != null && itemTemplateInfo != null)
			{
				player.UseItem(itemTemplateInfo);
			}
		}
	}

	private void HandlePlayerExit(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame == null)
		{
			return;
		}
		Player player = baseGame.FindPlayer(pkg.Parameter1);
		if (player != null)
		{
			GSPacketIn pkg2 = new GSPacketIn(83, player.PlayerDetail.PlayerCharacter.ID);
			baseGame.SendToAll(pkg2);
			baseGame.RemovePlayer(player.PlayerDetail, IsKick: false);
			ProxyRoom roomUnsafe = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Red1.RoomId);
			if (roomUnsafe != null && !roomUnsafe.RemovePlayer(player.PlayerDetail))
			{
				ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Blue1.RoomId)?.RemovePlayer(player.PlayerDetail);
			}
			ProxyRoom roomUnsafe2 = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Red1.RoomId);
			if (roomUnsafe2 != null && !roomUnsafe2.RemovePlayer(player.PlayerDetail))
			{
				ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Blue1.RoomId)?.RemovePlayer(player.PlayerDetail);
			}
			ProxyRoom roomUnsafe3 = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Red1.RoomId);
			if (roomUnsafe3 != null && !roomUnsafe3.RemovePlayer(player.PlayerDetail))
			{
				ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Blue1.RoomId)?.RemovePlayer(player.PlayerDetail);
			}
			ProxyRoom roomUnsafe4 = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Red1.RoomId);
			if (roomUnsafe4 != null && !roomUnsafe4.RemovePlayer(player.PlayerDetail))
			{
				ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Blue1.RoomId)?.RemovePlayer(player.PlayerDetail);
			}
		}
	}

	public void HandleConsortiaAlly(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame != null)
		{
			baseGame.ConsortiaAlly = pkg.ReadInt();
			baseGame.RichesRate = pkg.ReadInt();
		}
	}

	private void HandleSysNotice(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame != null)
		{
			Player player = baseGame.FindPlayer(pkg.Parameter1);
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
			player.PlayerDetail.SendTCP(gSPacketIn);
			gSPacketIn.ClearContext();
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", player.PlayerDetail.PlayerCharacter.NickName, player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
			baseGame.SendToAll(gSPacketIn, player.PlayerDetail);
		}
	}

	private void HandlePlayerMessage(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame == null)
		{
			return;
		}
		Player player = baseGame.FindPlayer(pkg.ReadInt());
		bool flag = pkg.ReadBoolean();
		string str = pkg.ReadString();
		if (player != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(19);
			gSPacketIn.ClientID = player.PlayerDetail.PlayerCharacter.ID;
			gSPacketIn.WriteInt(player.PlayerDetail.ZoneId);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
			gSPacketIn.WriteString(str);
			if (flag)
			{
				baseGame.SendToTeam(pkg, player.Team);
			}
			else
			{
				baseGame.SendToAll(gSPacketIn);
			}
		}
	}

	private void HandlePlayerSceneFace(GSPacketIn pkg)
	{
		GameMgr.FindGame(pkg.Parameter2)?.SendToAll(pkg);
	}

	public void HandleLogin(GSPacketIn pkg)
	{
		byte[] rgb = pkg.ReadBytes();
		string[] array = Encoding.UTF8.GetString(m_rsa.Decrypt(rgb, fOAEP: false)).Split(',');
		if (array.Length == 2)
		{
			m_rsa = null;
			int num = int.Parse(array[0]);
			base.Strict = false;
		}
		else
		{
			log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
			Disconnect();
		}
	}

	public void HandleGameRoomCreate(GSPacketIn pkg)
	{
		pkg.UnCompress();
		int num = pkg.ReadInt();
		int npcId = pkg.ReadInt();
		bool isAutoBot = pkg.ReadBoolean();
		bool startWithNpc = pkg.ReadBoolean();
		bool isFreedom = pkg.ReadBoolean();
		bool isCrossZone = pkg.ReadBoolean();
		int roomType = pkg.ReadInt();
		int gameType = pkg.ReadInt();
		int groupType = pkg.ReadInt();
		int guildId = pkg.ReadInt();
		int num2 = pkg.ReadInt();
		int num3 = 0;
		int num4 = 0;
		int selfId = 0;
		IGamePlayer[] array = new IGamePlayer[num2];
		for (int i = 0; i < num2; i++)
		{
			PlayerInfo info = new PlayerInfo();
			ProxyPlayerInfo proxyPlayerInfo = new ProxyPlayerInfo
			{
				ZoneId = pkg.ReadInt(),
				ZoneName = pkg.ReadString()
			};
			int bombKingDelay = pkg.ReadInt();
			int battleId = pkg.ReadInt();
			info.ID = pkg.ReadInt();
			info.NickName = pkg.ReadString();
			info.Sex = pkg.ReadBoolean();
			info.typeVIP = pkg.ReadByte();
			info.VIPLevel = pkg.ReadInt();
			info.Hide = pkg.ReadInt();
			info.Style = pkg.ReadString();
			proxyPlayerInfo.RedFootballStyle = pkg.ReadString();
			proxyPlayerInfo.BlueFootballStyle = pkg.ReadString();
			info.Colors = pkg.ReadString();
			info.Skin = pkg.ReadString();
			info.Offer = pkg.ReadInt();
			info.GP = pkg.ReadInt();
			info.Grade = pkg.ReadInt();
			info.Repute = pkg.ReadInt();
			info.ConsortiaID = pkg.ReadInt();
			info.ConsortiaName = pkg.ReadString();
			info.ConsortiaLevel = pkg.ReadInt();
			info.ConsortiaRepute = pkg.ReadInt();
			info.badgeID = pkg.ReadInt();
			info.weaklessGuildProgress = Convert.FromBase64String(pkg.ReadString());
			info.Honor = pkg.ReadString();
			info.Attack = pkg.ReadInt();
			info.Defence = pkg.ReadInt();
			info.Agility = pkg.ReadInt();
			info.Luck = pkg.ReadInt();
			info.hp = pkg.ReadInt();
			info.MagicAttack = pkg.ReadInt();
			info.MagicDefence = pkg.ReadInt();
			if (GameProperties.VERSION >= 11000)
			{
				info.EmblemInfoAttackMax = pkg.ReadInt();
				info.EmblemInfoAttackSKill = pkg.ReadInt();
				info.EmblemInfoAttackSKill_Value = pkg.ReadInt();
				info.EmblemInfoAttackSKill_Rate = pkg.ReadInt();
				info.EmblemInfoAttackMax = pkg.ReadInt();
				info.EmblemInfoAttackSKill = pkg.ReadInt();
				info.EmblemInfoAttackSKill_Value = pkg.ReadInt();
				info.EmblemInfoAttackSKill_Rate = pkg.ReadInt();
				PlayerData playerData = new PlayerData();
				info.ElfInfo = playerData.Sys_Users_Elf.Where((Sys_Users_Elf p) => p.UserID == info.ID).ToList();
				info.SecondPro = playerData.Sys_Users_SecondPro.FirstOrDefault((Sys_Users_SecondPro p) => p.UserID == info.ID);
				info.ElementPro = playerData.Sys_Users_ElementPro.FirstOrDefault((Sys_Users_ElementPro p) => p.UserID == info.ID);
				if (info.SecondPro != null)
				{
					info.SecondPro.Crit = pkg.ReadInt();
					info.SecondPro.Kill = pkg.ReadInt();
					info.SecondPro.Guard = pkg.ReadInt();
					info.SecondPro.Speed = pkg.ReadInt();
					info.SecondPro.Tenacity = pkg.ReadInt();
					info.SecondPro.WillFight = pkg.ReadInt();
					info.SecondPro.AvoidInjury = pkg.ReadInt();
					info.SecondPro.SunderArmor = pkg.ReadInt();
					info.SecondPro.ViolenceInjury = pkg.ReadInt();
				}
				if (info.ElementPro != null)
				{
					info.ElementPro.FireAttack = pkg.ReadInt();
					info.ElementPro.WaterAttack = pkg.ReadInt();
					info.ElementPro.WindAttack = pkg.ReadInt();
					info.ElementPro.LandAttack = pkg.ReadInt();
					info.ElementPro.FireDefence = pkg.ReadInt();
					info.ElementPro.WaterDefence = pkg.ReadInt();
					info.ElementPro.WindDefence = pkg.ReadInt();
					info.ElementPro.LandDefence = pkg.ReadInt();
				}
				info.CurrentSealSkill = pkg.ReadInt();
			}
			info.FightPower = pkg.ReadInt();
			info.AcademyInfo = new Sys_Users_Academy();
			info.AcademyInfo.ApprenticeshipState = pkg.ReadInt();
			info.AcademyInfo.MasterID = pkg.ReadInt();
			info.AcademyInfo.MasterOrApprentices = pkg.ReadString();
			info.GhostEquipList = pkg.ReadString();
			info.IsMarried = pkg.ReadBoolean();
			if (info.IsMarried)
			{
				info.SpouseID = pkg.ReadInt();
				info.SpouseName = pkg.ReadString();
				info.ringFlag = pkg.ReadBoolean();
				info.loveBuffLevel = pkg.ReadInt();
				info.loveBuffValue = pkg.ReadInt();
			}
			info.guardCoreID = pkg.ReadInt();
			proxyPlayerInfo.BaseAttack = pkg.ReadDouble();
			proxyPlayerInfo.BaseDefence = pkg.ReadDouble();
			proxyPlayerInfo.BaseAgility = pkg.ReadDouble();
			proxyPlayerInfo.BaseBlood = pkg.ReadDouble();
			proxyPlayerInfo.TemplateId = pkg.ReadInt();
			proxyPlayerInfo.WeaponStrengthLevel = pkg.ReadInt();
			int num5 = pkg.ReadInt();
			if (num5 > 0)
			{
				proxyPlayerInfo.GoldTemplateId = num5;
				proxyPlayerInfo.goldBeginTime = pkg.ReadDateTime();
				proxyPlayerInfo.goldValidDate = pkg.ReadInt();
			}
			proxyPlayerInfo.CanUserProp = pkg.ReadBoolean();
			proxyPlayerInfo.SecondWeapon = pkg.ReadInt();
			proxyPlayerInfo.StrengthLevel = pkg.ReadInt();
			proxyPlayerInfo.CurExp = pkg.ReadInt();
			proxyPlayerInfo.Weapon3D = pkg.ReadInt();
			proxyPlayerInfo.Healstone = pkg.ReadInt();
			proxyPlayerInfo.HealstoneCount = pkg.ReadInt();
			int num6 = pkg.ReadInt();
			if (num6 > 0)
			{
				proxyPlayerInfo.Amulet = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(num6), 1, 1);
				proxyPlayerInfo.Amulet.StrengthenLevel = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole1 = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole2 = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole3 = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole4 = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole5 = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole6 = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole5Exp = pkg.ReadInt();
				proxyPlayerInfo.Amulet.Hole6Exp = pkg.ReadInt();
			}
			proxyPlayerInfo.GPAddPlus = pkg.ReadDouble();
			proxyPlayerInfo.OfferAddPlus = pkg.ReadDouble();
			proxyPlayerInfo.AntiAddictionRate = pkg.ReadDouble();
			proxyPlayerInfo.ServerId = pkg.ReadInt();
			proxyPlayerInfo.DragonBoatOpen = pkg.ReadBoolean();
			proxyPlayerInfo.DragonBoatAddExpPlus = pkg.ReadInt();
			proxyPlayerInfo.TcpEndPoint = pkg.ReadString();
			UserMatchInfo userMatchInfo = new UserMatchInfo();
			userMatchInfo.restCount = pkg.ReadInt();
			userMatchInfo.maxCount = pkg.ReadInt();
			bool flag = pkg.ReadBoolean();
			UsersPetInfo pet = null;
			if (flag)
			{
				pet = new UsersPetInfo
				{
					Place = pkg.ReadInt(),
					TemplateID = pkg.ReadInt(),
					ID = pkg.ReadInt(),
					Name = pkg.ReadString(),
					UserID = pkg.ReadInt(),
					Level = pkg.ReadInt(),
					Skill = pkg.ReadString(),
					SkillEquip = pkg.ReadString()
				};
			}
			List<BufferInfo> list = new List<BufferInfo>();
			int num7 = pkg.ReadInt();
			for (int j = 0; j < num7; j++)
			{
				BufferInfo item = new BufferInfo
				{
					Type = pkg.ReadInt(),
					IsExist = pkg.ReadBoolean(),
					BeginDate = pkg.ReadDateTime(),
					ValidDate = pkg.ReadInt(),
					Value = pkg.ReadInt(),
					ValidCount = pkg.ReadInt(),
					TemplateID = pkg.ReadInt()
				};
				list.Add(item);
			}
			List<ItemInfo> list2 = new List<ItemInfo>();
			int num8 = pkg.ReadInt();
			for (int k = 0; k < num8; k++)
			{
				int templateId = pkg.ReadInt();
				int hole = pkg.ReadInt();
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 1);
				itemInfo.Hole1 = hole;
				list2.Add(itemInfo);
			}
			List<BufferInfo> list3 = new List<BufferInfo>();
			int num9 = pkg.ReadInt();
			for (int l = 0; l < num9; l++)
			{
				BufferInfo item2 = new BufferInfo
				{
					Type = pkg.ReadInt(),
					Value = pkg.ReadInt()
				};
				list3.Add(item2);
			}
			List<int> list4 = new List<int>();
			int num10 = pkg.ReadInt();
			for (int m = 0; m < num10; m++)
			{
				int item3 = pkg.ReadInt();
				list4.Add(item3);
			}
			BatleConfigInfo batleConfig = new BatleConfigInfo
			{
				MagicHouse = pkg.ReadInt(),
				PetFormReduceDamage = pkg.ReadInt()
			};
			List<ItemInfo> list5 = new List<ItemInfo>();
			int num11 = pkg.ReadInt();
			for (int n = 0; n < num11; n++)
			{
				int templateId2 = pkg.ReadInt();
				int hole2 = pkg.ReadInt();
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId2), 1, 1);
				itemInfo2.Hole1 = hole2;
				list5.Add(itemInfo2);
			}
			List<int> list6 = new List<int>();
			int num12 = pkg.ReadInt();
			for (int num13 = 0; num13 < num12; num13++)
			{
				list6.Add(pkg.ReadInt());
			}
			selfId = info.ID;
			num3 += info.Grade;
			info.IsAutoBot = isAutoBot;
			num4 += info.FightPower;
			array[i] = new ProxyPlayer(this, info, userMatchInfo, proxyPlayerInfo, pet, list, list2, list5, list3, list4, batleConfig, list6);
			array[i].CanUseProp = proxyPlayerInfo.CanUserProp;
			array[i].IsCrossZone = isCrossZone;
			array[i].BombKingDelay = bombKingDelay;
			array[i].battleId = battleId;
			int num14 = pkg.ReadInt();
			for (int num15 = 0; num15 < num14; num15++)
			{
				int item4 = pkg.ReadInt();
				array[i].QuestColect.Add(item4);
			}
			array[i].PlayerCharacter.badgePvpid = pkg.ReadInt();
			if (pkg.ReadBoolean())
			{
				UserPvePowerBuffInfo userPvePowerBuffInfo = new UserPvePowerBuffInfo();
				userPvePowerBuffInfo.getBuffAtk = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffDef = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffAgl = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffLuck = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffDmg = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffAr = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffHp = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffMAtk = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffMDef = pkg.ReadInt();
				userPvePowerBuffInfo.getBuffDate = pkg.ReadDateTime();
				array[i].PvePowerBuff = userPvePowerBuffInfo;
			}
			Sys_Users_Team team = new Sys_Users_Team
			{
				teamID = pkg.ReadInt(),
				teamName = pkg.ReadString(),
				teamTag = pkg.ReadString(),
				teamGrade = pkg.ReadInt(),
				teamWinTime = pkg.ReadInt(),
				teamTotalTime = pkg.ReadInt(),
				teamDivision = pkg.ReadInt(),
				teamScore = pkg.ReadInt(),
				teamDuty = pkg.ReadInt(),
				teamPersonalScore = pkg.ReadInt()
			};
			array[i].PlayerCharacter.Team = team;
		}
		ProxyRoom proxyRoom = new ProxyRoom(ProxyRoomMgr.NextRoomId(), num, array[0].ZoneId, array, this, num3, num4, npcId, isAutoBot)
		{
			GuildId = guildId,
			groupType = groupType,
			selfId = selfId,
			startWithNpc = startWithNpc,
			IsFreedom = isFreedom,
			IsCrossZone = isCrossZone,
			RoomType = (eRoomType)roomType,
			GameType = (eGameType)gameType
		};
		lock (m_rooms)
		{
			if (!m_rooms.ContainsKey(num))
			{
				m_rooms.Add(num, proxyRoom);
			}
			else
			{
				proxyRoom = null;
			}
		}
		if (proxyRoom != null)
		{
			ProxyRoomMgr.AddRoom(proxyRoom);
			return;
		}
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(num))
			{
				proxyRoom = m_rooms[num];
			}
		}
		RemoveRoom(num, proxyRoom);
	}

	public void HandleGameRoomCancel(GSPacketIn pkg)
	{
		ProxyRoom proxyRoom = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(pkg.Parameter1))
			{
				proxyRoom = m_rooms[pkg.Parameter1];
			}
		}
		if (proxyRoom != null)
		{
			ProxyRoomMgr.RemoveRoom(proxyRoom);
		}
	}

	public void HanleSendToGame(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame != null)
		{
			GSPacketIn pkg2 = pkg.ReadPacket();
			baseGame.ProcessData(pkg2);
		}
	}

	public void SendRSAKey(byte[] m, byte[] e)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(0);
		gSPacketIn.Write(m);
		gSPacketIn.Write(e);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerUpdateUserMoney(int playerId, int value, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(203, playerId);
		gSPacketIn.WriteInt(value);
		gSPacketIn.WriteInt(type);
		SendTCP(gSPacketIn);
	}

	public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(32, playerId);
		gSPacketIn.WritePacket(pkg);
		SendTCP(gSPacketIn);
	}

	public void SendRemoveRoom(int roomId)
	{
		GSPacketIn pkg = new GSPacketIn(65, roomId);
		SendTCP(pkg);
	}

	public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(67, roomId);
		if (except != null)
		{
			gSPacketIn.Parameter1 = except.PlayerCharacter.ID;
			gSPacketIn.Parameter2 = except.GamePlayerId;
		}
		else
		{
			gSPacketIn.Parameter1 = 0;
			gSPacketIn.Parameter2 = 0;
		}
		gSPacketIn.WritePacket(pkg);
		SendTCP(gSPacketIn);
	}

	public void SendStartGame(int roomId, AbstractGame game)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(66);
		gSPacketIn.Parameter1 = roomId;
		gSPacketIn.Parameter2 = game.Id;
		gSPacketIn.WriteInt((int)game.RoomType);
		gSPacketIn.WriteInt((int)game.GameType);
		gSPacketIn.WriteInt(game.TimeType);
		SendTCP(gSPacketIn);
	}

	public void SendStopGame(int roomId, int gameId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.Parameter1 = roomId;
		gSPacketIn.Parameter2 = gameId;
		SendTCP(gSPacketIn);
	}

	public void SendGamePlayerId(IGamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(33);
		gSPacketIn.Parameter1 = player.PlayerCharacter.ID;
		gSPacketIn.Parameter2 = player.GamePlayerId;
		SendTCP(gSPacketIn);
	}

	public void SendDisconnectPlayer(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(34, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(35, playerId);
		gSPacketIn.Parameter1 = gameId;
		gSPacketIn.WriteBoolean(isWin);
		gSPacketIn.WriteInt(gainXp);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(36, playerId);
		gSPacketIn.Parameter1 = bag;
		gSPacketIn.Parameter2 = place;
		gSPacketIn.WriteInt(templateId);
		gSPacketIn.WriteBoolean(isLiving);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddGold(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(38, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendFootballTakeOut(int playerId, bool isWin)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, playerId);
		gSPacketIn.WriteBoolean(isWin);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerBoatScore(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddEnterModePoint(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(92, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendCreateEnterModeItem(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(93, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerAddScoreBombKing(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(204, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddMoney(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(74, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddScoreCityBattle(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(205, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddActiveMoney(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(89, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddGiftToken(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(75, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveHealstone(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(73, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerAddMedal(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(76, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddLeagueMoney(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddPrestige(int playerId, bool isWin, eRoomType roomType)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(85, playerId);
		gSPacketIn.Parameter1 = (int)roomType;
		gSPacketIn.WriteBoolean(isWin);
		SendTCP(gSPacketIn);
	}

	public void SendRingstationResult(int playerId, bool isWin)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(90, playerId);
		gSPacketIn.WriteBoolean(isWin);
		SendTCP(gSPacketIn);
	}

	public void SendUpdateRestCount(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(86, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerAddGP(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(39, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveGP(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(49, playerId);
		gSPacketIn.Parameter1 = value;
		SendTCP(gSPacketIn);
	}

	public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(40, playerId);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(isLiving);
		gSPacketIn.WriteInt(demage);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(41, playerId);
		gSPacketIn.WriteBoolean(isWin);
		gSPacketIn.WriteInt(MissionID);
		gSPacketIn.WriteInt(turnNum);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
	{
		try
		{
			GSPacketIn gSPacketIn = new GSPacketIn(42, playerId);
			gSPacketIn.WriteInt(consortiaWin);
			gSPacketIn.WriteInt(consortiaLose);
			gSPacketIn.WriteInt(players.Count);
			for (int i = 0; i < players.Count; i++)
			{
				gSPacketIn.WriteInt(players[i].PlayerDetail.PlayerCharacter.ID);
			}
			gSPacketIn.WriteByte((byte)roomType);
			gSPacketIn.WriteByte((byte)gameClass);
			gSPacketIn.WriteInt(totalKillHealth);
			SendTCP(gSPacketIn);
		}
		catch
		{
			log.ErrorFormat("SendPlayerConsortiaFight players.Count {0}", players.Count);
		}
	}

	public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(43, playerId);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(riches);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveGold(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(44, playerId);
		gSPacketIn.WriteInt(value);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveMoney(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(45, playerId);
		gSPacketIn.WriteInt(value);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveOffer(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(50, playerId);
		gSPacketIn.WriteInt(value);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddTemplate(int playerId, ItemInfo item, eBageType bagType, int count)
	{
		if (item != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(48, playerId);
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteByte((byte)bagType);
			gSPacketIn.WriteInt(count);
			gSPacketIn.WriteInt(item.ValidDate);
			gSPacketIn.WriteBoolean(item.IsBinds);
			gSPacketIn.WriteBoolean(item.IsUsed);
			gSPacketIn.WriteInt(item.StrengthenLevel);
			gSPacketIn.WriteInt(item.AttackCompose);
			gSPacketIn.WriteInt(item.DefendCompose);
			gSPacketIn.WriteInt(item.AgilityCompose);
			gSPacketIn.WriteInt(item.LuckCompose);
			gSPacketIn.WriteInt(item.MagicAttack);
			gSPacketIn.WriteInt(item.MagicDefence);
			gSPacketIn.WriteBoolean(item.IsGold);
			if (item.IsGold)
			{
				gSPacketIn.WriteDateTime(item.goldBeginTime);
				gSPacketIn.WriteInt(item.goldValidDate);
			}
			SendTCP(gSPacketIn);
		}
	}

	public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(77);
		gSPacketIn.WriteInt(Consortia1);
		gSPacketIn.WriteInt(Consortia2);
		gSPacketIn.WriteInt(GameId);
		SendTCP(gSPacketIn);
	}

	public void SendBeginFightNpc(int playerId, int RoomType, int GameType, int OrientRoomId, int playerCount)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(88);
		gSPacketIn.Parameter1 = playerId;
		gSPacketIn.WriteInt(RoomType);
		gSPacketIn.WriteInt(GameType);
		gSPacketIn.WriteInt(OrientRoomId);
		gSPacketIn.WriteInt(playerCount);
		SendTCP(gSPacketIn);
	}

	public ServerClient(FightServer svr)
		: base(new byte[8192], new byte[8192])
	{
		m_svr = svr;
	}

	public override string ToString()
	{
		return $"Server Client: {0} IsConnected:{base.IsConnected}  RoomCount:{m_rooms.Count}";
	}

	public void RemoveRoom(int orientId, ProxyRoom room)
	{
		bool flag = false;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(orientId) && m_rooms[orientId] == room)
			{
				flag = m_rooms.Remove(orientId);
			}
		}
		if (flag)
		{
			SendRemoveRoom(orientId);
		}
	}

	public void SendPlayerAddMoneyPVP(int playerId, int value)
	{
		GSPacketIn pkg = new GSPacketIn(23, playerId)
		{
			Parameter1 = value
		};
		SendTCP(pkg);
	}

	public void SendUpdatePublicPlayer(int playerId, string tempStyle)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(301, playerId);
		gSPacketIn.WriteString(tempStyle);
		SendTCP(gSPacketIn);
	}
}

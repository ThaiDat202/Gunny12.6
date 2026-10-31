using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bussiness;
using Bussiness.Protocol;
using Center.Server.Managers;
using Game.Base;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server
{
	public class ServerClient : BaseClient
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private RSACryptoServiceProvider _rsa;

		private CenterServer _svr;

		private WorldZoneMgr _wzm;

		public bool NeedSyncMacroDrop = false;

		private string m_zoneName;

		private readonly int awardTime = 20;

		public ServerInfo Info { get; set; }

		public int ID => (Info != null) ? Info.ID : (-1);

		public int ZoneID => (Info != null) ? Info.AreaId : (-1);

		public string ZoneName => m_zoneName;

		protected override void OnConnect()
		{
			base.OnConnect();
			_rsa = new RSACryptoServiceProvider();
			RSAParameters rSAParameters = _rsa.ExportParameters(includePrivateParameters: false);
			SendRSAKey(rSAParameters.Modulus, rSAParameters.Exponent);
		}

		protected override void OnDisconnect()
		{
			base.OnDisconnect();
			_rsa = null;
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				List<Player> serverPlayers = zoneLoginByAreaID.GetServerPlayers(this);
				zoneLoginByAreaID.RemovePlayer(serverPlayers);
				SendUserOffline(serverPlayers);
			}
			if (Info != null)
			{
				Info.State = 1;
				Info.Online = 0;
			}
		}

		public override void OnRecvPacket(GSPacketIn pkg)
		{
			switch (pkg.Code)
			{
			case 1:
				HandleLogin(pkg);
				break;
			case 3:
				HandleUserLogin(pkg);
				break;
			case 4:
				HandleUserOffline(pkg);
				break;
			case 5:
				HandleUserOnline(pkg);
				break;
			case 6:
				HandleQuestUserState(pkg);
				break;
			case 10:
				HandkeItemStrengthen(pkg);
				break;
			case 11:
				HandleReload(pkg);
				break;
			case 12:
				HandlePing(pkg);
				break;
			case 13:
				HandleUpdatePlayerState(pkg);
				break;
			case 14:
				HandleMarryRoomInfoToPlayer(pkg);
				break;
			case 15:
				HandleShutdown(pkg);
				break;
			case 19:
				HandleChatScene(pkg);
				break;
			case 37:
				HandleChatPersonal(pkg);
				break;
			case 71:
				HandleSmallBugle(pkg);
				break;
			case 72:
				HandleBigBugle(pkg);
				break;
			case 73:
				HandleCBugle(pkg);
				break;
			case 81:
				HandleWorldBossRank(pkg, update: true);
				break;
			case 82:
				HandleWorldBossFightOver(pkg);
				break;
			case 83:
				HandleWorldBossRoomClose(pkg);
				break;
			case 84:
				HandleWorldBossUpdateBlood(pkg);
				break;
			case 85:
				HandleWorldBossPrivateInfo(pkg);
				break;
			case 86:
				HandleWorldBossRank(pkg, update: false);
				break;
			case 90:
				HandleEventRank(pkg);
				break;
			case 91:
				HandleWorldEvent(pkg);
				break;
			case 107:
				HandleChatAreaPersonal(pkg);
				break;
			case 117:
				HandleMailResponse(pkg);
				break;
			case 128:
				HandleConsortiaResponse(pkg);
				break;
			case 130:
				HandleConsortiaCreate(pkg);
				break;
			case 156:
				HandleConsortiaOffer(pkg);
				break;
			case 158:
				HandleConsortiaFight(pkg);
				break;
			case 160:
				HandleFriend(pkg);
				break;
			case 178:
				HandleMacroDrop(pkg);
				break;
			case 180:
				HandleRecvConsortiaBossAdd(pkg);
				break;
			case 181:
				HandleRecvConsortiaBossUpdateRank(pkg);
				break;
			case 182:
				HandleRecvConsortiaBossExtendAvailable(pkg);
				break;
			case 183:
				HandleRecvConsortiaBossCreate(pkg);
				break;
			case 184:
				HandleRecvConsortiaBossReload(pkg);
				break;
			case 186:
				HandleRecvConsortiaBossUpdateBlood(pkg);
				break;
			case 189:
				HandleEnterModeGetPoint(pkg);
				break;
			case 190:
				HandleEnterModeUpdatePoint(pkg);
				break;
			case 193:
				HandleReload(pkg);
				break;
			case 240:
				HandleIPAndPort(pkg);
				break;
			case 306:
				HandleBombKingUpdatePlayer(pkg);
				break;
			case 308:
				HandleLoginDeviceCheck(pkg);
				break;
			case 329:
				HandleSanXiao(pkg);
				break;
			case 368:
				HandleCityBattle(pkg);
				break;
			}
		}

		public void HandleSanXiao(GSPacketIn packet)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID == null)
			{
				return;
			}
			Player player = zoneLoginByAreaID.GetPlayer(packet.ClientID);
			if (player == null)
			{
				return;
			}
			UserQiYuanInfo userQiYuanInfo = WorldMgr.QiYuan.FindPlayer(ZoneID, player.Id) ?? WorldMgr.QiYuan.CreatePlayer(player);
			switch (packet.ReadByte())
			{
			case 34:
			{
				bool isBind = packet.ReadBoolean();
				if (WorldMgr.QiYuan.OfferTimes / GameProperties.QiYuanOfferTimesPerTreasureBox - userQiYuanInfo.hasGainTreasureBoxNum > 0)
				{
					userQiYuanInfo.hasGainTreasureBoxNum++;
					WorldMgr.QiYuan.AddOfferDegree(5);
					GSPacketIn gSPacketIn2 = WorldMgr.QiYuan.SendBoguBoxOpenPkg(userQiYuanInfo, isBind);
					if (gSPacketIn2 != null)
					{
						SendTCP(gSPacketIn2);
					}
				}
				break;
			}
			case 35:
			{
				GSPacketIn gSPacketIn3 = WorldMgr.QiYuan.SendQueryTowerTaskPkg(userQiYuanInfo);
				if (gSPacketIn3 != null)
				{
					SendTCP(gSPacketIn3);
				}
				break;
			}
			case 37:
			{
				int type = packet.ReadInt();
				GSPacketIn gSPacketIn = WorldMgr.QiYuan.SendAreaRankPkg(userQiYuanInfo, type);
				if (gSPacketIn != null)
				{
					SendTCP(gSPacketIn);
				}
				break;
			}
			case 38:
			{
				int total = ((packet.ReadInt() <= 0) ? 1 : 10);
				WorldMgr.QiYuan.AddOffter(userQiYuanInfo, total);
				int num = packet.ReadInt();
				for (int i = 0; i < num; i++)
				{
					int key = packet.ReadInt();
					int num2 = packet.ReadInt();
					if (userQiYuanInfo.ListTowerItemsCount.ContainsKey(key))
					{
						userQiYuanInfo.ListTowerItemsCount[key] += num2;
					}
					else
					{
						userQiYuanInfo.ListTowerItemsCount.Add(key, num2);
					}
				}
				break;
			}
			case 39:
				SendTCP(WorldMgr.QiYuan.SendMyInfoPkg(userQiYuanInfo));
				break;
			case 36:
				break;
			}
		}

		public void HandleCityBattle(GSPacketIn packet)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID == null)
			{
				return;
			}
			Player player = zoneLoginByAreaID.GetPlayer(packet.ClientID);
			if (player == null)
			{
				return;
			}
			UserCityBattleInfo userCityBattleInfo = WorldMgr.CityBattle.FindPlayer(ZoneID, player.Id);
			switch (packet.ReadByte())
			{
			case 0:
			{
				int mySlide = 0;
				if (userCityBattleInfo != null)
				{
					mySlide = userCityBattleInfo.Team;
				}
				SendTCP(WorldMgr.CityBattle.SendCityBattleOpen(player.Id, mySlide));
				break;
			}
			case 1:
			{
				GSPacketIn gSPacketIn3 = new GSPacketIn(368, player.Id);
				gSPacketIn3.WriteByte(1);
				gSPacketIn3.WriteInt((int)((WorldMgr.CityBattle.DayFighting.DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)7) : WorldMgr.CityBattle.DayFighting.DayOfWeek));
				List<UserCityBattleInfo> allWinner = WorldMgr.CityBattle.GetAllWinner();
				gSPacketIn3.WriteInt(allWinner.Count);
				foreach (UserCityBattleInfo item in allWinner)
				{
					if (item != null)
					{
						gSPacketIn3.WriteInt(item.Team);
						gSPacketIn3.WriteInt(item.ZoneID);
						gSPacketIn3.WriteString(item.ZoneName);
						gSPacketIn3.WriteInt(item.UserID);
						gSPacketIn3.WriteString(item.NickName);
						gSPacketIn3.WriteBoolean(item.Sex);
						gSPacketIn3.WriteInt(item.Hide);
						gSPacketIn3.WriteString(item.Style);
						gSPacketIn3.WriteString(item.Colors);
						gSPacketIn3.WriteString(item.Skin);
					}
					else
					{
						gSPacketIn3.WriteInt(0);
						gSPacketIn3.WriteInt(ZoneID);
						gSPacketIn3.WriteString("ZoneName");
						gSPacketIn3.WriteInt(0);
						gSPacketIn3.WriteString("Unknown");
						gSPacketIn3.WriteBoolean(val: true);
						gSPacketIn3.WriteInt(0);
						gSPacketIn3.WriteString("");
						gSPacketIn3.WriteString("");
						gSPacketIn3.WriteString("");
					}
				}
				for (int i = 0; i < 7; i++)
				{
					if (i >= allWinner.Count)
					{
						gSPacketIn3.WriteInt(0);
					}
					else if (allWinner[i] != null)
					{
						gSPacketIn3.WriteInt(allWinner[i].Team);
					}
					else
					{
						gSPacketIn3.WriteInt(0);
					}
				}
				SendTCP(gSPacketIn3);
				break;
			}
			case 2:
				if (userCityBattleInfo == null || userCityBattleInfo.Team == 0)
				{
					int num2 = packet.ReadInt();
					if (userCityBattleInfo == null)
					{
						userCityBattleInfo = WorldMgr.CityBattle.CreatePlayer(player);
					}
					if (num2 == 0)
					{
						num2 = WorldMgr.CityBattle.GetLowPlayerTeam();
						WorldMgr.CityBattle.SendAwardRandomTeam(userCityBattleInfo);
					}
					userCityBattleInfo.Team = num2;
					GSPacketIn gSPacketIn = new GSPacketIn(368, player.Id);
					gSPacketIn.WriteByte(2);
					gSPacketIn.WriteInt(userCityBattleInfo.Team);
					SendTCP(gSPacketIn);
				}
				break;
			case 3:
			{
				GSPacketIn gSPacketIn2 = new GSPacketIn(368, player.Id);
				gSPacketIn2.WriteByte(3);
				List<UserCityBattleInfo> tOP10PlayerWithTeam = WorldMgr.CityBattle.GetTOP10PlayerWithTeam(1);
				gSPacketIn2.WriteInt(tOP10PlayerWithTeam.Count);
				foreach (UserCityBattleInfo item2 in tOP10PlayerWithTeam)
				{
					gSPacketIn2.WriteString(item2.NickName);
					gSPacketIn2.WriteInt(item2.Score);
					gSPacketIn2.WriteString(item2.ZoneName);
				}
				List<UserCityBattleInfo> tOP10PlayerWithTeam2 = WorldMgr.CityBattle.GetTOP10PlayerWithTeam(2);
				gSPacketIn2.WriteInt(tOP10PlayerWithTeam2.Count);
				foreach (UserCityBattleInfo item3 in tOP10PlayerWithTeam2)
				{
					gSPacketIn2.WriteString(item3.NickName);
					gSPacketIn2.WriteInt(item3.Score);
					gSPacketIn2.WriteString(item3.ZoneName);
				}
				SendTCP(gSPacketIn2);
				break;
			}
			case 4:
				packet.ReadInt();
				if (userCityBattleInfo != null)
				{
					SendTCP(WorldMgr.CityBattle.SendExchangeInfo(userCityBattleInfo));
				}
				break;
			case 6:
			{
				int num3 = packet.ReadInt();
				int goodId = packet.ReadInt();
				int num4 = packet.ReadInt();
				if (num4 <= 0 || userCityBattleInfo == null)
				{
					break;
				}
				UserCityBattleExchageInfo userCityBattleExchageInfo = userCityBattleInfo.ExchangeInfo.SingleOrDefault((UserCityBattleExchageInfo a) => a.ID == goodId);
				if (userCityBattleExchageInfo == null)
				{
					break;
				}
				CityOccupationSystemsInfo cityOccupationSystemsInfo = WorldMgr.CityBattle.FindAward(goodId);
				if (cityOccupationSystemsInfo == null || userCityBattleExchageInfo.Count + num4 > cityOccupationSystemsInfo.ExchangeCount || (cityOccupationSystemsInfo.Quality == 2 && num3 != 0 && cityOccupationSystemsInfo.Probability != num3))
				{
					break;
				}
				UserCityBattleInfo winnerInWeek = WorldMgr.CityBattle.GetWinnerInWeek(cityOccupationSystemsInfo.Probability);
				if (cityOccupationSystemsInfo.Quality != 3 || (winnerInWeek != null && winnerInWeek.Team == userCityBattleInfo.Team))
				{
					int num5 = cityOccupationSystemsInfo.NeedScore * num4;
					if (userCityBattleInfo.myScore >= num5)
					{
						userCityBattleInfo.myScore -= num5;
						userCityBattleExchageInfo.Count += num4;
						WorldMgr.CityBattle.Exchange(userCityBattleInfo, cityOccupationSystemsInfo, num4);
						SendTCP(WorldMgr.CityBattle.SendUpdateScorePkg(userCityBattleInfo));
						SendTCP(WorldMgr.CityBattle.SendExchangeInfo(userCityBattleInfo));
					}
				}
				break;
			}
			case 7:
			{
				int num = packet.ReadInt();
				bool flag = packet.ReadBoolean();
				if (num > 0 && userCityBattleInfo != null && WorldMgr.CityBattle.IsOpen)
				{
					userCityBattleInfo.Score += num;
					userCityBattleInfo.myScore += num;
					if (flag)
					{
						SendTCP(WorldMgr.CityBattle.SendUpdateScorePkg(userCityBattleInfo));
					}
				}
				break;
			}
			case 8:
				SendTCP(WorldMgr.CityBattle.SendUpdateScorePkg(userCityBattleInfo));
				break;
			case 5:
				break;
			}
		}

		public void HandleLoginDeviceCheck(GSPacketIn pkg)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				Player player = zoneLoginByAreaID.GetPlayer(pkg.ClientID);
				if (player != null)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(308, player.Id);
					gSPacketIn.WriteBoolean(player.IsLoginDevice);
					SendTCP(gSPacketIn);
				}
			}
		}

		public void HandleBombKingUpdatePlayer(GSPacketIn pkg)
		{
			BombKingPlayerInfo bombKingPlayerInfo = new BombKingPlayerInfo();
			bombKingPlayerInfo.UserID = pkg.ReadInt();
			bombKingPlayerInfo.AreaID = pkg.ReadInt();
			bombKingPlayerInfo.NickName = pkg.ReadString();
			bombKingPlayerInfo.AreaName = pkg.ReadString();
			bombKingPlayerInfo.vipType = pkg.ReadInt();
			bombKingPlayerInfo.vipLevel = pkg.ReadInt();
			bombKingPlayerInfo.style = pkg.ReadString();
			bombKingPlayerInfo.color = pkg.ReadString();
			bombKingPlayerInfo.sex = pkg.ReadBoolean();
			bombKingPlayerInfo.smg = pkg.ReadString();
			int score = pkg.ReadInt();
			bool win = pkg.ReadBoolean();
			BombKingMgr.AddScore(bombKingPlayerInfo, score, win);
		}

		public void HandleChatAreaPersonal(GSPacketIn pkg)
		{
			AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
			foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
			{
				if (areaConfigInfo != null && areaConfigInfo.CrossPrivateChat)
				{
					_svr.SendToALL(pkg, areaConfigInfo.AreaID);
				}
			}
		}

		public void HandleCBugle(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			int num2 = pkg.ReadInt();
			string text = pkg.ReadString();
			string text2 = pkg.ReadString();
			string str = pkg.ReadString();
			GSPacketIn gSPacketIn = pkg.Clone();
			gSPacketIn.ClearContext();
			gSPacketIn.ClientID = pkg.ClientID;
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(num2);
			gSPacketIn.WriteString(text);
			gSPacketIn.WriteString(text2);
			gSPacketIn.WriteString(str);
			AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
			foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
			{
				if (areaConfigInfo != null && areaConfigInfo.CrossChatAllow)
				{
					_svr.SendToALL(gSPacketIn, areaConfigInfo.AreaID);
				}
			}
			ChatMrg.AddChat(new ChatInfo
			{
				ZoneId = num,
				UserID = num2,
				NickName = text,
				Type = 4,
				Msg = text2,
				ChatTime = DateTime.Now
			});
		}

		public void HandleEnterModeGetPoint(GSPacketIn pkg)
		{
			SendTCP(EntertamentModeMgr.SendGetPoint(pkg.ReadInt(), pkg.ReadInt()));
		}

		public void HandleEnterModeUpdatePoint(GSPacketIn pkg)
		{
			EntertamentModeMgr.UpdatePoint(pkg.ReadInt(), pkg.ReadInt());
		}

		public void HandleWorldEvent(GSPacketIn pkg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			switch (pkg.ReadByte())
			{
			case 1:
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
				_wzm.AddOrUpdateLanternriddles(lanternriddlesInfo.PlayerID, lanternriddlesInfo);
				break;
			}
			case 2:
			{
				LanternriddlesInfo lanternriddles = _wzm.GetLanternriddles(pkg.ReadInt());
				gSPacketIn.WriteByte(2);
				if (lanternriddles == null)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(lanternriddles.PlayerID);
					gSPacketIn.WriteInt(lanternriddles.QuestionIndex);
					gSPacketIn.WriteInt(lanternriddles.QuestionView);
					gSPacketIn.WriteDateTime(lanternriddles.EndDate);
					gSPacketIn.WriteInt(lanternriddles.DoubleFreeCount);
					gSPacketIn.WriteInt(lanternriddles.DoublePrice);
					gSPacketIn.WriteInt(lanternriddles.HitFreeCount);
					gSPacketIn.WriteInt(lanternriddles.HitPrice);
					gSPacketIn.WriteInt(lanternriddles.MyInteger);
					gSPacketIn.WriteInt(lanternriddles.QuestionNum);
					gSPacketIn.WriteInt(lanternriddles.Option);
					gSPacketIn.WriteBoolean(lanternriddles.IsHint);
					gSPacketIn.WriteBoolean(lanternriddles.IsDouble);
				}
				_svr.SendToALL(gSPacketIn, ZoneID);
				break;
			}
			}
		}

		public void HandleEventRank(GSPacketIn pkg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(90);
			switch (pkg.ReadByte())
			{
			case 1:
			{
				RankingLightriddleInfo rankingLightriddleInfo = null;
				string text = pkg.ReadString();
				int val = pkg.ReadInt();
				gSPacketIn.WriteByte(1);
				List<RankingLightriddleInfo> list = _wzm.SelectTopEight();
				gSPacketIn.WriteInt(list.Count);
				foreach (RankingLightriddleInfo item in list)
				{
					gSPacketIn.WriteInt(item.Rank);
					gSPacketIn.WriteString(item.NickName);
					gSPacketIn.WriteByte((byte)item.TypeVIP);
					gSPacketIn.WriteInt(item.Integer);
					if (text == item.NickName)
					{
						rankingLightriddleInfo = item;
					}
				}
				if (rankingLightriddleInfo == null)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(rankingLightriddleInfo.Rank);
				}
				gSPacketIn.WriteInt(val);
				_svr.SendToALL(gSPacketIn, ZoneID);
				break;
			}
			case 2:
			{
				string nickName = pkg.ReadString();
				int typeVip = pkg.ReadInt();
				int integer = pkg.ReadInt();
				int playerId = pkg.ReadInt();
				_wzm.UpdateLightriddleRank(integer, typeVip, nickName, playerId);
				break;
			}
			case 4:
				_wzm.UpdateLuckStarRewardRecord(pkg.ReadInt(), pkg.ReadString(), pkg.ReadInt(), pkg.ReadInt(), pkg.ReadInt());
				break;
			case 5:
				_wzm.UpdateHalloweenRank(pkg.ReadInt(), pkg.ReadString(), pkg.ReadInt(), pkg.ReadInt());
				break;
			case 3:
				break;
			}
		}

		public void HandleRecvConsortiaBossUpdateBlood(GSPacketIn pkg)
		{
			ConsortiaBossMgr.UpdateBlood(pkg.ReadInt(), pkg.ReadInt());
		}

		public void HandleRecvConsortiaBossUpdateRank(GSPacketIn pkg)
		{
			ConsortiaBossMgr.UpdateRank(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString(), pkg.ReadInt());
		}

		public void HandleRecvConsortiaBossExtendAvailable(GSPacketIn pkg)
		{
			int consortiaId = pkg.ReadInt();
			int riches = pkg.ReadInt();
			if (ConsortiaBossMgr.ExtendAvailable(consortiaId, riches))
			{
				ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(consortiaId);
				if (consortiaById != null)
				{
					HandleSendConsortiaBossInfo(consortiaById, 182);
				}
			}
			else
			{
				ConsortiaInfo consortiaById2 = ConsortiaBossMgr.GetConsortiaById(consortiaId);
				if (consortiaById2 != null)
				{
					HandleSendConsortiaBossInfo(consortiaById2, 184);
				}
			}
		}

		public void HandleRecvConsortiaBossReload(GSPacketIn pkg)
		{
			ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(pkg.ReadInt());
			if (consortiaById == null)
			{
				return;
			}
			if (consortiaById.bossState == 2 && consortiaById.SendToClient)
			{
				if (consortiaById.IsBossDie)
				{
					HandleSendConsortiaBossInfo(consortiaById, 188);
				}
				else
				{
					HandleSendConsortiaBossInfo(consortiaById, 187);
				}
				ConsortiaBossMgr.UpdateSendToClient(consortiaById.ConsortiaID);
			}
			else
			{
				HandleSendConsortiaBossInfo(consortiaById, 184);
			}
		}

		public void HandleRecvConsortiaBossCreate(GSPacketIn pkg)
		{
			int consortiaId = pkg.ReadInt();
			byte bossState = pkg.ReadByte();
			DateTime endTime = pkg.ReadDateTime();
			DateTime lastOpenBoss = pkg.ReadDateTime();
			long maxBlood = pkg.ReadInt();
			if (ConsortiaBossMgr.UpdateConsortia(consortiaId, bossState, endTime, lastOpenBoss, maxBlood))
			{
				ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(consortiaId);
				if (consortiaById != null)
				{
					HandleSendConsortiaBossInfo(consortiaById, 183);
				}
			}
		}

		public void HandleRecvConsortiaBossAdd(GSPacketIn pkg)
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
			if (!ConsortiaBossMgr.AddConsortia(consortiaInfo.ConsortiaID, consortiaInfo))
			{
				consortiaInfo = ConsortiaBossMgr.GetConsortiaById(consortiaInfo.ConsortiaID);
			}
			HandleSendConsortiaBossInfo(consortiaInfo, 180);
		}

		public void HandleSendConsortiaBossInfo(ConsortiaInfo consortia, byte code)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(180);
			gSPacketIn.WriteInt(consortia.ConsortiaID);
			gSPacketIn.WriteInt(consortia.ChairmanID);
			gSPacketIn.WriteByte((byte)consortia.bossState);
			gSPacketIn.WriteDateTime(consortia.endTime);
			gSPacketIn.WriteInt(consortia.extendAvailableNum);
			gSPacketIn.WriteInt(consortia.callBossLevel);
			gSPacketIn.WriteInt(consortia.Level);
			gSPacketIn.WriteInt(consortia.SmithLevel);
			gSPacketIn.WriteInt(consortia.StoreLevel);
			gSPacketIn.WriteInt(consortia.SkillLevel);
			gSPacketIn.WriteInt(consortia.Riches);
			gSPacketIn.WriteDateTime(consortia.LastOpenBoss);
			gSPacketIn.WriteLong(consortia.MaxBlood);
			gSPacketIn.WriteLong(consortia.TotalAllMemberDame);
			gSPacketIn.WriteBoolean(consortia.IsBossDie);
			List<RankingPersonInfo> list = ConsortiaBossMgr.SelectRank(consortia.ConsortiaID);
			gSPacketIn.WriteInt(list.Count);
			int num = 1;
			foreach (RankingPersonInfo item in list)
			{
				gSPacketIn.WriteString(item.Name);
				gSPacketIn.WriteInt(num);
				gSPacketIn.WriteInt(item.TotalDamage);
				gSPacketIn.WriteInt(item.Honor);
				gSPacketIn.WriteInt(item.Damage);
				num++;
			}
			gSPacketIn.WriteByte(code);
			_svr.SendToALL(gSPacketIn);
		}

		public void HandleLogin(GSPacketIn pkg)
		{
			string[] array = Encoding.UTF8.GetString(_rsa.Decrypt(pkg.ReadBytes(), fOAEP: false)).Split(',');
			if (array.Length == 2)
			{
				_rsa = null;
				int num = int.Parse(array[0]);
				Info = ServerMgr.GetServerInfo(num);
				if (Info == null || Info.State != 1)
				{
					log.ErrorFormat("Error Login Packet from {0} want to login serverid: {1}", base.TcpEndpoint, num);
					Disconnect();
					return;
				}
				base.Strict = false;
				CenterServer.Instance.SendConfigState();
				SendAreaConfigState();
				SendUpdateWorldEvent();
				SendSynBombKing();
				SendSchemaData();
				Info.Online = 0;
				Info.State = 2;
				_wzm.AreaId = Info.AreaId;
				AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(ZoneID);
				if (areaConfigInfo != null)
				{
					m_zoneName = areaConfigInfo.AreaName;
				}
			}
			else
			{
				log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
				Disconnect();
			}
		}

		public void SendSchemaData()
		{
		}

		public void SendAreaConfigState()
		{
			AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
			GSPacketIn gSPacketIn = new GSPacketIn(192);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(allAreaConfig.Length);
			AreaConfigInfo[] array = allAreaConfig;
			foreach (AreaConfigInfo areaConfigInfo in array)
			{
				gSPacketIn.WriteInt(areaConfigInfo.AreaID);
				gSPacketIn.WriteString(areaConfigInfo.AreaServer);
				gSPacketIn.WriteString(areaConfigInfo.AreaName);
				gSPacketIn.WriteString(areaConfigInfo.DataSource);
				gSPacketIn.WriteString(areaConfigInfo.Catalog);
				gSPacketIn.WriteString(areaConfigInfo.UserID);
				gSPacketIn.WriteString(areaConfigInfo.Password);
				gSPacketIn.WriteString(areaConfigInfo.RequestUrl);
				gSPacketIn.WriteBoolean(areaConfigInfo.CrossChatAllow);
				gSPacketIn.WriteBoolean(areaConfigInfo.CrossPrivateChat);
				gSPacketIn.WriteString(areaConfigInfo.Version);
			}
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void HandleIPAndPort(GSPacketIn pkg)
		{
		}

		private void HandleUserLogin(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				if (zoneLoginByAreaID.TryLoginPlayer(num, this))
				{
					SendAllowUserLogin(num, allow: true);
				}
				else
				{
					SendAllowUserLogin(num, allow: false);
				}
			}
		}

		private void HandleUserOnline(GSPacketIn pkg)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID == null)
			{
				return;
			}
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int id = pkg.ReadInt();
				Player player = zoneLoginByAreaID.PlayerLogined(id, this);
				if (player != null)
				{
					player.Info = new PlayerInfo();
					player.Info.ConsortiaID = pkg.ReadInt();
					player.NickName = pkg.ReadString();
					player.ZoneName = ZoneName;
					player.Info.Sex = pkg.ReadBoolean();
					player.Info.Style = pkg.ReadString();
					player.Info.Colors = pkg.ReadString();
					player.Info.Skin = pkg.ReadString();
					player.Info.Hide = pkg.ReadInt();
				}
			}
			_svr.SendToALL(pkg, this, ZoneID);
		}

		private void HandleUserOffline(GSPacketIn pkg)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				int num = pkg.ReadInt();
				for (int i = 0; i < num; i++)
				{
					int id = pkg.ReadInt();
					pkg.ReadInt();
					zoneLoginByAreaID.PlayerLoginOut(id, this);
				}
				_svr.SendToALL(pkg, ZoneID);
			}
		}

		private void HandleUserPrivateMsg(GSPacketIn pkg, int playerid)
		{
			ServerMgr.GetZoneLoginByAreaID(ZoneID)?.GetServerClient(playerid)?.SendTCP(pkg);
		}

		public void HandleUserPublicMsg(GSPacketIn pkg)
		{
			_svr.SendToAllZones(pkg, this);
		}

		public void HandleQuestUserState(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				if (zoneLoginByAreaID.GetServerClient(num) == null)
				{
					SendUserState(num, state: false);
				}
				else
				{
					SendUserState(num, state: true);
				}
			}
		}

		public void HandlePing(GSPacketIn pkg)
		{
			Info.Online = pkg.ReadInt();
			Info.State = ServerMgr.GetState(Info.Online, Info.Total);
		}

		public void HandleChatPersonal(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, ZoneID);
		}

		public void HandleSmallBugle(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			string text = pkg.ReadString();
			string text2 = pkg.ReadString();
			GSPacketIn gSPacketIn = pkg.Clone();
			gSPacketIn.ClearContext();
			gSPacketIn.ClientID = pkg.ClientID;
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteString(text);
			gSPacketIn.WriteString(text2);
			_svr.SendToALL(gSPacketIn, ZoneID);
			ChatMrg.AddChat(new ChatInfo
			{
				ZoneId = ZoneID,
				UserID = num,
				NickName = text,
				Type = 2,
				Msg = text2,
				ChatTime = DateTime.Now
			});
		}

		public void HandleBigBugle(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			int num2 = pkg.ReadInt();
			string text = pkg.ReadString();
			string text2 = pkg.ReadString();
			GSPacketIn gSPacketIn = pkg.Clone();
			gSPacketIn.ClearContext();
			gSPacketIn.ClientID = pkg.ClientID;
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(num2);
			gSPacketIn.WriteString(text);
			gSPacketIn.WriteString(text2);
			_svr.SendToALL(gSPacketIn, ZoneID);
			ChatMrg.AddChat(new ChatInfo
			{
				ZoneId = ZoneID,
				BigbugType = num,
				UserID = num2,
				NickName = text,
				Type = 3,
				Msg = text2,
				ChatTime = DateTime.Now
			});
			Console.WriteLine("bigbugType:{0}, nickName:{1}, id:{2}, msg:{3}", num, text, num2, text2);
		}

		public void HandleFriend(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, this, ZoneID);
		}

		public void HandleFriendState(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, this, ZoneID);
		}

		public void HandleFirendResponse(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, this, ZoneID);
		}

		public void HandleMailResponse(GSPacketIn pkg)
		{
			int playerid = pkg.ReadInt();
			HandleUserPrivateMsg(pkg, playerid);
		}

		public void HandleReload(GSPacketIn pkg)
		{
			eReloadType eReloadType = (eReloadType)pkg.ReadInt();
			_svr.SendReload(eReloadType);
			Console.WriteLine("Client call back: reload eReloadType.{0}", eReloadType);
		}

		public void HandleChatScene(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			byte b = pkg.ReadByte();
			bool val = pkg.ReadBoolean();
			string text = pkg.ReadString();
			string text2 = pkg.ReadString();
			string consortiaName = "";
			if (b == 3)
			{
				int val2 = pkg.ReadInt();
				consortiaName = pkg.ReadString();
				GSPacketIn gSPacketIn = pkg.Clone();
				gSPacketIn.ClearContext();
				gSPacketIn.ClientID = pkg.ClientID;
				gSPacketIn.WriteInt(num);
				gSPacketIn.WriteByte(b);
				gSPacketIn.WriteBoolean(val);
				gSPacketIn.WriteString(text);
				gSPacketIn.WriteString(text2);
				gSPacketIn.WriteInt(val2);
				HandleChatConsortia(gSPacketIn);
			}
			ChatMrg.AddChat(new ChatInfo
			{
				ZoneId = num,
				UserID = pkg.ClientID,
				NickName = text,
				Type = 100 + b,
				Msg = text2,
				ChatTime = DateTime.Now,
				ConsortiaName = consortiaName
			});
		}

		public void HandleChatConsortia(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, this, ZoneID);
		}

		public void HandleConsortiaResponse(GSPacketIn pkg)
		{
			byte b = pkg.ReadByte();
			byte b2 = b;
			_svr.SendToALL(pkg, null, ZoneID);
		}

		public void HandleConsortiaOffer(GSPacketIn pkg)
		{
			pkg.ReadInt();
			pkg.ReadInt();
			pkg.ReadInt();
		}

		public void HandleBuyBadge(GSPacketIn pkg)
		{
			pkg.ReadInt();
			_svr.SendToALL(pkg, null, ZoneID);
		}

		public void HandleConsortiaCreate(GSPacketIn pkg)
		{
			pkg.ReadInt();
			pkg.ReadInt();
			_svr.SendToALL(pkg, null, ZoneID);
		}

		public void HandleConsortiaUpGrade(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, this, ZoneID);
		}

		public void HandleConsortiaFight(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, ZoneID);
		}

		public void HandkeItemStrengthen(GSPacketIn pkg)
		{
			_svr.SendToALL(pkg, this, ZoneID);
		}

		public void HandleUpdatePlayerState(GSPacketIn pkg)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				int playerId = pkg.ReadInt();
				Player player = zoneLoginByAreaID.GetPlayer(playerId);
				if (player != null && player.CurrentServer != null)
				{
					player.CurrentServer.SendTCP(pkg);
				}
			}
		}

		public void HandleMarryRoomInfoToPlayer(GSPacketIn pkg)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(ZoneID);
			if (zoneLoginByAreaID != null)
			{
				int playerId = pkg.ReadInt();
				Player player = zoneLoginByAreaID.GetPlayer(playerId);
				if (player != null && player.CurrentServer != null)
				{
					player.CurrentServer.SendTCP(pkg);
				}
			}
		}

		public void HandleShutdown(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			if (pkg.ReadBoolean())
			{
				Console.WriteLine(num + "  begin stoping !");
			}
			else
			{
				Console.WriteLine(num + "  is stoped !");
			}
		}

		public void HandleWorldBossUpdateBlood(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			if (num > 0)
			{
				_wzm.ReduceBlood(num);
			}
			SendUpdateWorldBlood();
		}

		public void HandleWorldBossFightOver(GSPacketIn pkg)
		{
			_wzm.WorldBossFightOver();
			SendWorldBossFightOver();
		}

		public void HandleWorldBossRoomClose(GSPacketIn pkg)
		{
			_wzm.WorldBossRoomClose();
			SendRoomClose(0);
		}

		public void HandleWorldBossRank(GSPacketIn pkg, bool update)
		{
			if (update)
			{
				_wzm.UpdateRank(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
			}
			SendUpdateRank(type: false);
		}

		public void HandleWorldBossPrivateInfo(GSPacketIn pkg)
		{
			SendPrivateInfo(pkg.ReadString());
		}

		public void HandleMacroDrop(GSPacketIn pkg)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int key = pkg.ReadInt();
				int value = pkg.ReadInt();
				dictionary.Add(key, value);
			}
			MacroDropMgr.DropNotice(dictionary);
			NeedSyncMacroDrop = true;
		}

		public void SendRSAKey(byte[] m, byte[] e)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(0);
			gSPacketIn.Parameter1 = ZoneID;
			gSPacketIn.Write(m);
			gSPacketIn.Write(e);
			SendTCP(gSPacketIn);
		}

		public void SendAllowUserLogin(int playerid, bool allow)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteBoolean(allow);
			SendTCP(gSPacketIn);
		}

		public void SendKitoffUser(int playerid)
		{
			SendKitoffUser(playerid, LanguageMgr.GetTranslation("Center.Server.SendKitoffUser"));
		}

		public void SendKitoffUser(int playerid, string msg)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(2);
			gSPacketIn.WriteInt(playerid);
			gSPacketIn.WriteString(msg);
			SendTCP(gSPacketIn);
		}

		public void SendUserOffline(List<Player> users)
		{
			for (int i = 0; i < users.Count; i += 100)
			{
				int num = ((i + 100 > users.Count) ? (users.Count - i) : 100);
				GSPacketIn gSPacketIn = new GSPacketIn(4);
				gSPacketIn.WriteInt(num);
				for (int j = i; j < i + num; j++)
				{
					gSPacketIn.WriteInt(users[j].Id);
					gSPacketIn.WriteInt(0);
				}
				SendTCP(gSPacketIn);
				_svr.SendToAllZones(gSPacketIn, this);
			}
		}

		public void SendUserState(int player, bool state)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(6, player);
			gSPacketIn.WriteBoolean(state);
			SendTCP(gSPacketIn);
		}

		public void SendChargeMoney(int player, string chargeID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(9, player);
			gSPacketIn.WriteString(chargeID);
			SendTCP(gSPacketIn);
		}

		public void SendASS(bool state)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(7);
			gSPacketIn.WriteBoolean(state);
			SendTCP(gSPacketIn);
		}

		public void SendSystemNotice(string msg)
		{
			SendSystemNotice(msg, 3);
		}

		public void SendSystemNotice(string msg, int type)
		{
			ChatMrg.AddChat(new ChatInfo
			{
				Type = 0,
				Msg = msg,
				ChatTime = DateTime.Now
			});
			GSPacketIn gSPacketIn = new GSPacketIn(10);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteString(msg);
			SendTCP(gSPacketIn);
		}

		public void SendUpdateWorldEvent()
		{
			int hour = DateTime.Now.Hour;
			if (hour >= 16 && hour < 22)
			{
				if (!_wzm.IsLeagueOpen)
				{
					SendLeagueOpenClose(value: true);
					_wzm.IsLeagueOpen = true;
					_wzm.LeagueOpenTime = DateTime.Now;
					SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.LeagueOpen"));
				}
			}
			else if (_wzm.IsLeagueOpen)
			{
				SendLeagueOpenClose(value: false);
				_wzm.IsLeagueOpen = false;
				SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.LeagueOpen"));
			}
			string[] array = GameProperties.FightFootballTime.Split('|');
			if (_wzm.IsFightFootballTime)
			{
				if (!AvailTime(_wzm.FightFootballTime, int.Parse(array[1])))
				{
					SendFightFootballTime(value: false);
					_wzm.IsFightFootballTime = false;
					SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.FightFootballTimeClose"));
				}
			}
			else if (hour.ToString() == array[0] && !_wzm.IsFightFootballTime)
			{
				SendFightFootballTime(value: true);
				_wzm.IsFightFootballTime = true;
				_wzm.FightFootballTime = DateTime.Now;
				SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.FightFootballTimeOpen"));
			}
			if (!_wzm.worldOpen)
			{
				switch (hour)
				{
				case 13:
					SendPrivateInfo();
					_wzm.SetupWorldBoss(0);
					SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.WorldBossOpen", _wzm.Name[0]));
					break;
				case 19:
					SendPrivateInfo();
					_wzm.SetupWorldBoss(3);
					SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.WorldBossOpen", _wzm.Name[3]));
					break;
				default:
					SendRoomClose(1);
					SendRoomClose(0);
					break;
				}
			}
			if (_wzm.worldOpen)
			{
				int minute = DateTime.Now.Minute;
				if (hour == 14 || hour == 20)
				{
					if (!_wzm.fightOver)
					{
						_wzm.WorldBossFightOver();
						SendWorldBossFightOver();
					}
					if (minute < 5)
					{
						SendSystemNotice(LanguageMgr.GetTranslation("Center.Server.ServerClient.WorldBossNoticeClose", _wzm.Name[_wzm.currentPVE_ID], 5 - minute));
					}
					else if (!_wzm.roomClose)
					{
						_wzm.WorldBossRoomClose();
						_wzm.WorldBossClose();
						SendRoomClose(0);
						SendUpdateRank(type: true);
					}
				}
				SendPrivateInfo();
				_wzm.UpdateFightTime();
			}
			int hour2 = DateTime.Now.Hour;
			if (hour2 > awardTime && _wzm.CanSendLightriddleAward)
			{
				_wzm.SendLightriddleTopEightAward();
			}
			if (hour2 > awardTime && _wzm.CanSendLuckyStarAward)
			{
				_wzm.SendLuckyStarTopTenAward();
			}
			if (hour2 > 1 && hour2 < awardTime)
			{
				if (!_wzm.CanSendLightriddleAward)
				{
					_wzm.CanSendLightriddleAward = true;
					_wzm.ResetLightriddleRank();
				}
				if (!_wzm.CanSendLuckyStarAward)
				{
					_wzm.CanSendLuckyStarAward = true;
					_wzm.ResetLuckStar();
				}
			}
			_wzm.SaveLuckyStarRewardRecord();
		}

		public bool AvailTime(DateTime startTime, int min)
		{
			return min - (int)(DateTime.Now - startTime).TotalMinutes > 0;
		}

		public void SendSynBombKing()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(300);
			gSPacketIn.WriteInt(BombKingMgr.battleStage);
			gSPacketIn.WriteInt(BombKingMgr.turn);
			gSPacketIn.WriteInt(BombKingMgr.status);
			gSPacketIn.WriteBoolean(BombKingMgr.IsBombKingOpen);
			gSPacketIn.WriteInt(BombKingMgr.Week);
			gSPacketIn.WriteDateTime(BombKingMgr.BombKingBeginTime);
			gSPacketIn.WriteDateTime(BombKingMgr.BombKingEndTime);
			SendTCP(gSPacketIn);
		}

		public void SendBombKingAddRank(int ID, int Vaild, int UserId)
		{
			SendTCP(new GSPacketIn(305, UserId)
			{
				Parameter1 = ID,
				Parameter2 = Vaild
			});
		}

		public void SendLeagueOpenClose(bool value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(87);
			gSPacketIn.WriteBoolean(value);
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void SendBattleGoundOpenClose(bool value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(88);
			gSPacketIn.WriteBoolean(value);
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void SendFightFootballTime(bool value)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(89);
			gSPacketIn.WriteBoolean(value);
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void SendPrivateInfo()
		{
			int currentPVE_ID = _wzm.currentPVE_ID;
			GSPacketIn gSPacketIn = new GSPacketIn(80);
			gSPacketIn.WriteLong(_wzm.MAX_BLOOD);
			gSPacketIn.WriteLong(_wzm.current_blood);
			gSPacketIn.WriteString(_wzm.Name[currentPVE_ID]);
			gSPacketIn.WriteString(_wzm.bossResourceId[currentPVE_ID]);
			gSPacketIn.WriteInt(_wzm.Pve_Id[currentPVE_ID]);
			gSPacketIn.WriteBoolean(_wzm.fightOver);
			gSPacketIn.WriteBoolean(_wzm.roomClose);
			gSPacketIn.WriteDateTime(_wzm.begin_time);
			gSPacketIn.WriteDateTime(_wzm.end_time);
			gSPacketIn.WriteInt(_wzm.fight_time);
			gSPacketIn.WriteBoolean(_wzm.worldOpen);
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void SendUpdateRank(bool type)
		{
			List<RankingPersonInfo> list = _wzm.SelectTopTen();
			if (list.Count == 0)
			{
				return;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(81);
			gSPacketIn.WriteBoolean(type);
			gSPacketIn.WriteInt(list.Count);
			foreach (RankingPersonInfo item in list)
			{
				gSPacketIn.WriteInt(item.ID);
				gSPacketIn.WriteString(item.Name);
				gSPacketIn.WriteInt(item.Damage);
			}
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void SendPrivateInfo(string name)
		{
			if (_wzm.CheckName(name))
			{
				GSPacketIn gSPacketIn = new GSPacketIn(85);
				RankingPersonInfo singleRank = _wzm.GetSingleRank(name);
				gSPacketIn.WriteString(name);
				gSPacketIn.WriteInt(singleRank.Damage);
				gSPacketIn.WriteInt(singleRank.Honor);
				_svr.SendToALL(gSPacketIn, ZoneID);
			}
		}

		public void SendWorldBossFightOver()
		{
			_svr.SendToALL(new GSPacketIn(82), ZoneID);
		}

		public void SendUpdateWorldBlood()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(79);
			gSPacketIn.WriteLong(_wzm.MAX_BLOOD);
			gSPacketIn.WriteLong(_wzm.current_blood);
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public void SendRoomClose(byte type)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(83);
			gSPacketIn.WriteByte(type);
			_svr.SendToALL(gSPacketIn, ZoneID);
		}

		public ServerClient(CenterServer svr)
			: base(new byte[8192], new byte[8192])
		{
			_svr = svr;
			_wzm = new WorldZoneMgr();
			_wzm.Start();
		}
	}
}

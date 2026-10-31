using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server.Managers
{
	public class QiYuanControl
	{
		private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private object m_lock = new object();

		private Dictionary<string, UserQiYuanInfo> m_players = new Dictionary<string, UserQiYuanInfo>();

		private int m_offerDegree = 0;

		private int m_offterTimes = 0;

		public int OfferTimes => m_offterTimes;

		public void AddPlayer(UserQiYuanInfo user)
		{
			lock (m_players)
			{
				if (!m_players.ContainsKey(user.ZoneID + "_" + user.UserID))
				{
					m_players.Add(user.ZoneID + "_" + user.UserID, user);
				}
			}
		}

		public UserQiYuanInfo CreatePlayer(Player p)
		{
			UserQiYuanInfo userQiYuanInfo = new UserQiYuanInfo();
			userQiYuanInfo.UserID = p.Id;
			userQiYuanInfo.ZoneID = p.ZoneId;
			userQiYuanInfo.ZoneName = p.ZoneName;
			userQiYuanInfo.NickName = p.NickName;
			userQiYuanInfo.HasGetGoods = new List<int>();
			userQiYuanInfo.ListTowerItemsCount = new Dictionary<int, int>();
			userQiYuanInfo.HasGetTowerGoods = new List<int>();
			AddPlayer(userQiYuanInfo);
			return userQiYuanInfo;
		}

		public UserQiYuanInfo FindPlayer(int zoneId, int userId)
		{
			lock (m_players)
			{
				return m_players.ContainsKey(zoneId + "_" + userId) ? m_players[zoneId + "_" + userId] : null;
			}
		}

		public void AddOffter(UserQiYuanInfo user, int total)
		{
			user.myOfferTimes += total;
			ReceiveOffterTimesAwards(user);
			AddTotalOffer(total);
		}

		public List<UserQiYuanInfo> GetAllRank(int areaId)
		{
			List<UserQiYuanInfo> list = new List<UserQiYuanInfo>();
			return (areaId > 0) ? (from a in m_players.Values
				where a.myOfferTimes >= GameProperties.QiYuanRankLeastOfferTimes && a.ZoneID == areaId
				orderby a.myOfferTimes descending
				select a).Take(50).ToList() : (from a in m_players.Values
				where a.myOfferTimes >= GameProperties.QiYuanRankLeastOfferTimes
				orderby a.myOfferTimes descending
				select a).Take(50).ToList();
		}

		public void AddTotalOffer(int value)
		{
			lock (m_lock)
			{
				m_offterTimes += value;
				m_offerDegree += value;
			}
		}

		public void AddOfferDegree(int value)
		{
			lock (m_lock)
			{
				m_offerDegree += value;
			}
		}

		public void CompleteEventWeek()
		{
			List<QiYuanAwardInfo> source = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.AREA_RANK_AWARD);
			int top = 1;
			AreaConfigInfo[] allAreaConfig = WorldMgr.GetAllAreaConfig();
			foreach (AreaConfigInfo areaConfigInfo in allAreaConfig)
			{
				List<UserQiYuanInfo> allRank = GetAllRank(areaConfigInfo.AreaID);
				if (allRank == null || allRank.Count <= 0)
				{
					continue;
				}
				top = 1;
				foreach (UserQiYuanInfo item in allRank)
				{
					QiYuanAwardInfo qiYuanAwardInfo = source.SingleOrDefault((QiYuanAwardInfo a) => a.Rank == top);
					if (qiYuanAwardInfo != null)
					{
						SendToMail(CreateAward(new List<QiYuanAwardInfo> { qiYuanAwardInfo }), item, LanguageMgr.GetTranslation("QiYuan.CenterServer.AreaRankTitle"), LanguageMgr.GetTranslation("QiYuan.CenterServer.AreaRankTitle"));
						top++;
						continue;
					}
					break;
				}
			}
			List<QiYuanAwardInfo> source2 = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.ALL_AREA_RANK_AWARD);
			List<UserQiYuanInfo> allRank2 = GetAllRank(0);
			if (allRank2 == null || allRank2.Count <= 0)
			{
				return;
			}
			top = 1;
			foreach (UserQiYuanInfo item2 in allRank2)
			{
				QiYuanAwardInfo qiYuanAwardInfo2 = source2.SingleOrDefault((QiYuanAwardInfo a) => a.Rank == top);
				if (qiYuanAwardInfo2 != null)
				{
					SendToMail(CreateAward(new List<QiYuanAwardInfo> { qiYuanAwardInfo2 }), item2, LanguageMgr.GetTranslation("QiYuan.CenterServer.AllAreaRankTitle"), LanguageMgr.GetTranslation("QiYuan.CenterServer.AllAreaRankTitle"));
					continue;
				}
				break;
			}
		}

		private void ResetAllData()
		{
			lock (m_players)
			{
				m_players.Clear();
			}
			lock (m_lock)
			{
				m_offerDegree = 0;
				m_offterTimes = 0;
			}
		}

		public void SendToMail(List<ItemInfo> items, UserQiYuanInfo user, string title, string content)
		{
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(user.ZoneID);
			if (areaConfigInfo == null)
			{
				return;
			}
			using PlayerBussiness pb = new PlayerBussiness(areaConfigInfo);
			WorldEventMgr.SendItemsToMailCrosszone(pb, items, user.UserID, user.NickName, user.ZoneID, title, content);
			CenterServer.Instance.SendMailResponse(user.ZoneID, user.UserID);
		}

		public List<ItemInfo> CreateAward(List<QiYuanAwardInfo> list)
		{
			List<ItemInfo> list2 = new List<ItemInfo>();
			foreach (QiYuanAwardInfo item in list)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), item.Count, 105);
				itemInfo.IsBinds = item.IsBind;
				itemInfo.ValidDate = item.VaildDate;
				list2.Add(itemInfo);
			}
			return list2;
		}

		public void ReceiveOffterTimesAwards(UserQiYuanInfo user)
		{
			foreach (QiYuanAwardInfo item in (from a in QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.BELIEF_AWARD)
				orderby a.Rank
				select a).ToList())
			{
				if (!user.HasGetGoods.Contains(item.ID) && item.Rank >= user.myOfferTimes)
				{
					user.HasGetGoods.Add(item.ID);
					SendToMail(CreateAward(new List<QiYuanAwardInfo> { item }), user, LanguageMgr.GetTranslation("QiYuan.CenterServer.OffterTimesAwardTitle", item.Rank), LanguageMgr.GetTranslation("QiYuan.CenterServer.OffterTimesAwardTitle", item.Rank));
				}
			}
		}

		public GSPacketIn SendMyInfoPkg(UserQiYuanInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(329, user.UserID);
			gSPacketIn.WriteByte(39);
			gSPacketIn.WriteInt(m_offterTimes);
			gSPacketIn.WriteInt(m_offerDegree);
			gSPacketIn.WriteInt(user.myOfferTimes);
			gSPacketIn.WriteInt(user.HasGetGoods.Count);
			foreach (int hasGetGood in user.HasGetGoods)
			{
				QiYuanAwardInfo qiYuanAwardInfo = QiYuanAwardMgr.FindQiYuanAward(hasGetGood);
				if (qiYuanAwardInfo != null)
				{
					gSPacketIn.WriteInt(qiYuanAwardInfo.TemplateID);
					gSPacketIn.WriteInt(qiYuanAwardInfo.Count);
				}
			}
			gSPacketIn.WriteInt(user.hasGainTreasureBoxNum);
			gSPacketIn.WriteInt(user.hasGainJoinRewardCount);
			return gSPacketIn;
		}

		public GSPacketIn SendQueryTowerTaskPkg(UserQiYuanInfo user)
		{
			List<int> list = new List<int>();
			GSPacketIn gSPacketIn = new GSPacketIn(329, user.UserID);
			gSPacketIn.WriteByte(35);
			gSPacketIn.WriteInt(QiYuanAwardMgr.TowerAwards.Count);
			int num = 1;
			foreach (KeyValuePair<QiYuanAwardInfo, List<QiYuanAwardInfo>> towerAward in QiYuanAwardMgr.TowerAwards)
			{
				gSPacketIn.WriteInt(num);
				gSPacketIn.WriteInt(towerAward.Key.TemplateID);
				gSPacketIn.WriteInt(towerAward.Key.Count);
				gSPacketIn.WriteBoolean(towerAward.Key.IsBind);
				gSPacketIn.WriteInt(towerAward.Value.Count);
				foreach (QiYuanAwardInfo item in towerAward.Value)
				{
					gSPacketIn.WriteInt(item.TemplateID);
					gSPacketIn.WriteInt(item.Count);
					if (user.ListTowerItemsCount.ContainsKey(item.TemplateID))
					{
						gSPacketIn.WriteInt(user.ListTowerItemsCount[item.TemplateID]);
					}
					else
					{
						gSPacketIn.WriteInt(0);
					}
				}
				if (user.HasGetTowerGoods.Contains(towerAward.Key.ID))
				{
					list.Add(num);
				}
				num++;
			}
			gSPacketIn.WriteInt(list.Count);
			foreach (int item2 in list)
			{
				gSPacketIn.WriteInt(item2);
			}
			return gSPacketIn;
		}

		public GSPacketIn SendAreaRankPkg(UserQiYuanInfo user, int type)
		{
			List<UserQiYuanInfo> list = new List<UserQiYuanInfo>();
			List<UserQiYuanInfo> list2 = ((type != 0) ? GetAllRank(0) : GetAllRank(user.ZoneID));
			if (list2.Count <= 0)
			{
				return null;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(329, user.UserID);
			gSPacketIn.WriteByte(37);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteInt(list2.Count);
			foreach (UserQiYuanInfo item in list2)
			{
				if (type == 0)
				{
					gSPacketIn.WriteInt(item.UserID);
					gSPacketIn.WriteString(item.NickName);
					gSPacketIn.WriteInt(item.myOfferTimes);
				}
				else
				{
					gSPacketIn.WriteString(item.ZoneName);
					gSPacketIn.WriteInt(item.ZoneID);
					gSPacketIn.WriteString(item.NickName);
					gSPacketIn.WriteInt(item.myOfferTimes);
				}
			}
			return gSPacketIn;
		}

		public GSPacketIn SendBoguBoxOpenPkg(UserQiYuanInfo user, bool isBind)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(329, user.UserID);
			gSPacketIn.WriteByte(34);
			gSPacketIn.WriteBoolean(isBind);
			gSPacketIn.WriteBoolean(val: true);
			return gSPacketIn;
		}
	}
}

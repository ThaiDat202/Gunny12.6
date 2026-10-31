using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server
{
	public class WorldZoneMgr
	{
		private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private object _syncStop = new object();

		private Dictionary<string, RankingPersonInfo> m_rankList;

		private Dictionary<string, RankingLightriddleInfo> m_lightriddleRankList;

		public string[] Name = new string[4]
		{
			LanguageMgr.GetTranslation("CenterServer.WorldBoss.BossName1"),
			LanguageMgr.GetTranslation("CenterServer.WorldBoss.BossName2"),
			LanguageMgr.GetTranslation("CenterServer.WorldBoss.BossName3"),
			LanguageMgr.GetTranslation("CenterServer.WorldBoss.BossName4")
		};

		public string[] bossResourceId = new string[4] { "1", "2", "2", "4" };

		public int[] Pve_Id = new int[4] { 1243, 30001, 30002, 30004 };

		public readonly long MAX_BLOOD = 3000000000L;

		public long current_blood = 0L;

		public DateTime begin_time;

		public DateTime end_time;

		public bool fightOver;

		public bool roomClose;

		public bool worldOpen;

		private readonly int worldbossTime = 60;

		public DateTime LeagueOpenTime;

		public bool IsLeagueOpen;

		public DateTime BattleGoundOpenTime;

		public bool IsBattleGoundOpen;

		public DateTime FightFootballTime;

		public bool IsFightFootballTime;

		public int currentPVE_ID;

		public int fight_time;

		private Dictionary<int, LanternriddlesInfo> m_lanternriddlesInfo;

		private Dictionary<int, LuckStarRewardRecordInfo> m_luckStarRewardRecordInfo;

		private Dictionary<int, HalloweenRankInfo> m_halloweenRandInfo;

		public bool CanSendLightriddleAward;

		public bool CanSendLuckyStarAward;

		private int LuckStarCountDown;

		public int AreaId;

		private int _halloweenCountDown = 0;

		public bool Start()
		{
			try
			{
				CanSendLightriddleAward = true;
				CanSendLuckyStarAward = true;
				m_rankList = new Dictionary<string, RankingPersonInfo>();
				m_lightriddleRankList = new Dictionary<string, RankingLightriddleInfo>();
				m_lanternriddlesInfo = new Dictionary<int, LanternriddlesInfo>();
				m_luckStarRewardRecordInfo = new Dictionary<int, LuckStarRewardRecordInfo>();
				m_halloweenRandInfo = new Dictionary<int, HalloweenRankInfo>();
				current_blood = MAX_BLOOD;
				begin_time = DateTime.Now;
				LeagueOpenTime = DateTime.Now;
				BattleGoundOpenTime = DateTime.Now;
				FightFootballTime = DateTime.Now;
				ResetLuckStar();
				end_time = begin_time.AddDays(1.0);
				fightOver = true;
				roomClose = true;
				worldOpen = false;
				IsLeagueOpen = false;
				IsBattleGoundOpen = false;
				IsFightFootballTime = false;
				return true;
			}
			catch (Exception arg)
			{
				log.ErrorFormat("Load server list from db failed:{0}", arg);
				return false;
			}
		}

		public void ResetLightriddleRank()
		{
			m_lightriddleRankList.Clear();
		}

		public void ResetHalloween()
		{
			m_halloweenRandInfo.Clear();
		}

		public void UpdateHalloweenRank(int ID, string nickName, int useNum, int isVip)
		{
			if (m_halloweenRandInfo.ContainsKey(ID))
			{
				m_halloweenRandInfo[ID].useNum = useNum;
				return;
			}
			m_halloweenRandInfo.Add(ID, new HalloweenRankInfo
			{
				UserID = ID,
				nickName = nickName,
				useNum = useNum,
				isVip = isVip
			});
		}

		public void UpdateHalloweenRankAll()
		{
			List<HalloweenRankInfo> list = new List<HalloweenRankInfo>();
			foreach (HalloweenRankInfo value in m_halloweenRandInfo.Values)
			{
				list.Add(value);
			}
			if (list.Count > 0)
			{
				AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(AreaId);
				if (areaConfigInfo != null)
				{
					List<HalloweenRankInfo> list2 = list.OrderByDescending((HalloweenRankInfo pair) => pair.useNum).ToList();
					using PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo);
					lock (m_halloweenRandInfo)
					{
						int num = 1;
						foreach (HalloweenRankInfo item in list2)
						{
							item.rank = num;
							if (m_halloweenRandInfo.ContainsKey(item.UserID))
							{
								m_halloweenRandInfo[item.UserID] = item;
								playerBussiness.UpdateHalloweenRank(item);
							}
							num++;
						}
					}
				}
			}
			CenterServer.Instance.SendReloadHalloweenRank();
		}

		public void ResetLuckStar()
		{
			m_luckStarRewardRecordInfo.Clear();
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(AreaId);
			if (areaConfigInfo != null)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo);
				playerBussiness.ResetLuckStarRank();
			}
			LuckStarCountDown = Math.Abs(60 - DateTime.Now.Minute - 30);
		}

		public void UpdateLuckStarRewardRecord(int playerId, string nickName, int templateId, int count, int isVip)
		{
			if (m_luckStarRewardRecordInfo.Keys.Contains(playerId))
			{
				m_luckStarRewardRecordInfo[playerId].useStarNum++;
				return;
			}
			LuckStarRewardRecordInfo value = new LuckStarRewardRecordInfo
			{
				PlayerID = playerId,
				nickName = nickName,
				useStarNum = 1,
				TemplateID = templateId,
				Count = count,
				isVip = isVip
			};
			m_luckStarRewardRecordInfo.Add(playerId, value);
		}

		public void SendLuckyStarTopTenAward()
		{
			int minUseNum = GameProperties.MinUseNum;
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(AreaId);
			if (areaConfigInfo == null)
			{
				return;
			}
			using (PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo))
			{
				LuckStarRewardRecordInfo[] luckStarTopTenRank = playerBussiness.GetLuckStarTopTenRank(minUseNum);
				foreach (LuckStarRewardRecordInfo luckStarRewardRecordInfo in luckStarTopTenRank)
				{
					List<LuckyStartToptenAwardInfo> luckyStartAwardByRank = WorldEventMgr.GetLuckyStartAwardByRank(luckStarRewardRecordInfo.rank);
					List<ItemInfo> list = new List<ItemInfo>();
					foreach (LuckyStartToptenAwardInfo item in luckyStartAwardByRank)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), 1, 105);
						itemInfo.IsBinds = item.IsBinds;
						itemInfo.ValidDate = item.Validate;
						itemInfo.Count = item.Count;
						list.Add(itemInfo);
					}
					WorldEventMgr.SendItemsToMailCrosszone(playerBussiness, list, luckStarRewardRecordInfo.PlayerID, luckStarRewardRecordInfo.nickName, luckStarRewardRecordInfo.ZoneID, LanguageMgr.GetTranslation("LuckyStarTopTenAward.GetAward", luckStarRewardRecordInfo.rank));
					CenterServer.Instance.SendMailResponse(luckStarRewardRecordInfo.ZoneID, luckStarRewardRecordInfo.PlayerID);
				}
			}
			CanSendLuckyStarAward = false;
		}

		public void SendTestAward(UserCityBattleInfo user, List<ItemInfo> items)
		{
			int minUseNum = GameProperties.MinUseNum;
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(AreaId);
			if (areaConfigInfo == null)
			{
				return;
			}
			using (new PlayerBussiness(areaConfigInfo))
			{
				WorldEventMgr.SendItemsToMails(items, user.UserID, user.NickName, user.ZoneID, "test lại cái phát mệt mỏi vl");
			}
		}

		public List<LuckStarRewardRecordInfo> GetAllLuckyStarRank()
		{
			List<LuckStarRewardRecordInfo> list = new List<LuckStarRewardRecordInfo>();
			foreach (LuckStarRewardRecordInfo value in m_luckStarRewardRecordInfo.Values)
			{
				list.Add(value);
			}
			return list;
		}

		public void SaveLuckyStarToDatabase()
		{
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(AreaId);
			if (areaConfigInfo == null)
			{
				return;
			}
			List<LuckStarRewardRecordInfo> allLuckyStarRank = GetAllLuckyStarRank();
			using PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo);
			foreach (LuckStarRewardRecordInfo item in allLuckyStarRank)
			{
				playerBussiness.SaveLuckStarRankInfo(item);
			}
		}

		public void SaveLuckyStarRewardRecord()
		{
			int hour = DateTime.Now.Hour;
			if (LuckStarCountDown > 0)
			{
				LuckStarCountDown--;
			}
			if (LuckStarCountDown == 0)
			{
				if (CanSendLuckyStarAward)
				{
					SaveLuckyStarToDatabase();
					LuckStarCountDown = 30;
				}
				else
				{
					LuckStarCountDown = -1;
				}
			}
			if (_halloweenCountDown == 1)
			{
				UpdateHalloweenRankAll();
				_halloweenCountDown = 0;
			}
			else
			{
				_halloweenCountDown++;
			}
		}

		public void AddOrUpdateLanternriddles(int playerId, LanternriddlesInfo lanternriddles)
		{
			if (!m_lanternriddlesInfo.ContainsKey(playerId))
			{
				m_lanternriddlesInfo.Add(playerId, lanternriddles);
			}
			else
			{
				m_lanternriddlesInfo[playerId] = lanternriddles;
			}
		}

		public LanternriddlesInfo GetLanternriddles(int playerId)
		{
			return m_lanternriddlesInfo.ContainsKey(playerId) ? m_lanternriddlesInfo[playerId] : null;
		}

		public void UpdateLightriddleRank(int integer, int typeVip, string nickName, int playerId)
		{
			if (m_lightriddleRankList.Keys.Contains(nickName))
			{
				m_lightriddleRankList[nickName].Integer = integer;
			}
			else
			{
				RankingLightriddleInfo value = new RankingLightriddleInfo
				{
					NickName = nickName,
					Integer = integer,
					TypeVIP = typeVip,
					PlayerId = playerId
				};
				m_lightriddleRankList.Add(nickName, value);
			}
			SortRank();
		}

		public void SendLightriddleTopEightAward()
		{
			AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(AreaId);
			if (areaConfigInfo != null)
			{
				using PlayerBussiness pb = new PlayerBussiness(areaConfigInfo);
				foreach (RankingLightriddleInfo item in SelectTopEight())
				{
					List<LuckyStartToptenAwardInfo> lanternriddlesAwardByRank = WorldEventMgr.GetLanternriddlesAwardByRank(item.Rank);
					List<ItemInfo> list = new List<ItemInfo>();
					foreach (LuckyStartToptenAwardInfo item2 in lanternriddlesAwardByRank)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateID), 1, 105);
						itemInfo.IsBinds = item2.IsBinds;
						itemInfo.ValidDate = item2.Validate;
						itemInfo.Count = item2.Count;
						list.Add(itemInfo);
					}
					WorldEventMgr.SendItemsToMailCrosszone(pb, list, item.PlayerId, item.NickName, item.ZoneID, LanguageMgr.GetTranslation("LightriddleTopEightAward.GetAward", item.Rank));
					CenterServer.Instance.SendMailResponse(item.ZoneID, item.PlayerId);
				}
			}
			CanSendLightriddleAward = false;
		}

		public List<RankingLightriddleInfo> SelectTopEight()
		{
			List<RankingLightriddleInfo> list = new List<RankingLightriddleInfo>();
			foreach (KeyValuePair<string, RankingLightriddleInfo> item in from pair in m_lightriddleRankList
				where pair.Value.Integer > 1
				orderby pair.Value.Integer descending
				select pair)
			{
				if (list.Count != 8)
				{
					list.Add(item.Value);
					continue;
				}
				break;
			}
			return list;
		}

		public void SortRank()
		{
			IOrderedEnumerable<KeyValuePair<string, RankingLightriddleInfo>> orderedEnumerable = m_lightriddleRankList.OrderByDescending((KeyValuePair<string, RankingLightriddleInfo> pair) => pair.Value.Integer);
			int num = 1;
			Dictionary<string, RankingLightriddleInfo> dictionary = new Dictionary<string, RankingLightriddleInfo>();
			foreach (KeyValuePair<string, RankingLightriddleInfo> item in orderedEnumerable)
			{
				item.Value.Rank = num;
				dictionary.Add(item.Key, item.Value);
				num++;
			}
			m_lightriddleRankList = dictionary;
		}

		public void UpdateRank(int damage, int honor, string nickName)
		{
			if (m_rankList.Keys.Contains(nickName))
			{
				m_rankList[nickName].Damage += damage;
				m_rankList[nickName].Honor += honor;
				return;
			}
			m_rankList.Add(nickName, new RankingPersonInfo
			{
				ID = m_rankList.Count + 1,
				Name = nickName,
				Damage = damage,
				Honor = honor
			});
		}

		public bool CheckName(string nickName)
		{
			return m_rankList.Keys.Contains(nickName);
		}

		public RankingPersonInfo GetSingleRank(string name)
		{
			return m_rankList[name];
		}

		public List<RankingPersonInfo> SelectTopTen()
		{
			List<RankingPersonInfo> list = new List<RankingPersonInfo>();
			foreach (KeyValuePair<string, RankingPersonInfo> item in m_rankList.OrderByDescending((KeyValuePair<string, RankingPersonInfo> pair) => pair.Value.Damage))
			{
				if (list.Count != 10)
				{
					list.Add(item.Value);
					continue;
				}
				break;
			}
			return list;
		}

		public void SetupWorldBoss(int id)
		{
			current_blood = MAX_BLOOD;
			begin_time = DateTime.Now;
			end_time = begin_time.AddDays(1.0);
			fight_time = worldbossTime - begin_time.Minute;
			fightOver = false;
			roomClose = false;
			currentPVE_ID = id;
			worldOpen = true;
		}

		public void WorldBossFightOver()
		{
			fightOver = true;
		}

		public void WorldBossRoomClose()
		{
			roomClose = true;
		}

		public void UpdateFightTime()
		{
			if (!fightOver)
			{
				fight_time = worldbossTime - begin_time.Minute;
			}
		}

		public void WorldBossClose()
		{
			worldOpen = false;
		}

		public void WorldBossClearRank()
		{
			m_rankList.Clear();
		}

		public void ReduceBlood(int value)
		{
			if (current_blood > 0)
			{
				current_blood -= value;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Center.Server.Managers
{
	public class CityBattleMgr
	{
		private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private object m_lock = new object();

		private TimeSpan m_timeStart = DateTime.Parse(GameProperties.CityBattleBeginTime).TimeOfDay;

		private TimeSpan m_timeEnd = DateTime.Parse(GameProperties.CityBattleEndTime).TimeOfDay;

		private bool m_isOpen = false;

		private Dictionary<string, UserCityBattleInfo> m_players = new Dictionary<string, UserCityBattleInfo>();

		private Dictionary<int, CityOccupationSystemsInfo> m_cityAwards = new Dictionary<int, CityOccupationSystemsInfo>();

		private List<UserCityBattleExchageInfo> m_cloneExchage;

		private int m_totalRedScore;

		private int m_totalBlueScore;

		private Dictionary<int, UserCityBattleInfo> m_listWinner = new Dictionary<int, UserCityBattleInfo>();

		private DateTime m_dayFighting;

		public bool IsOpen => m_isOpen;

		public int TotalRedScore => m_totalRedScore;

		public int TotalBlueScore => m_totalBlueScore;

		public Dictionary<int, UserCityBattleInfo> ListWinner => m_listWinner;

		public DateTime DayFighting => m_dayFighting;

		public CityBattleMgr()
		{
			try
			{
				using (ProduceBussiness produceBussiness = new ProduceBussiness())
				{
					m_cloneExchage = new List<UserCityBattleExchageInfo>();
					CityOccupationSystemsInfo[] allCityOccupationSystems = produceBussiness.GetAllCityOccupationSystems();
					foreach (CityOccupationSystemsInfo cityOccupationSystemsInfo in allCityOccupationSystems)
					{
						if (!m_cityAwards.ContainsKey(cityOccupationSystemsInfo.ID))
						{
							m_cityAwards.Add(cityOccupationSystemsInfo.ID, cityOccupationSystemsInfo);
							m_cloneExchage.Add(new UserCityBattleExchageInfo
							{
								ID = cityOccupationSystemsInfo.ID,
								Count = 0
							});
						}
					}
					ServerDataInfo serverDataInfo = ServerDataMgr.Find("CityBattleAccount");
					if (serverDataInfo != null)
					{
						m_players = JsonConvert.DeserializeObject<Dictionary<string, UserCityBattleInfo>>(serverDataInfo.ValString);
					}
					ServerDataInfo serverDataInfo2 = ServerDataMgr.Find("CityBattleListWinner");
					if (serverDataInfo2 != null)
					{
						m_listWinner = JsonConvert.DeserializeObject<Dictionary<int, UserCityBattleInfo>>(serverDataInfo2.ValString);
					}
					ServerDataInfo serverDataInfo3 = ServerDataMgr.Find("CityBattleDayFight");
					if (serverDataInfo3 != null)
					{
						m_dayFighting = serverDataInfo3.ValDateTime;
					}
				}
				if (m_listWinner == null || m_listWinner.Count <= 0)
				{
					ResetListDayWinner();
					m_dayFighting = DateTime.Now;
					m_players = new Dictionary<string, UserCityBattleInfo>();
				}
				UpdateCityBattleTimer();
			}
			catch (Exception exception)
			{
				log.Error("CityBattleMgr Init", exception);
			}
		}

		public void AddPlayer(UserCityBattleInfo user)
		{
			lock (m_players)
			{
				if (!m_players.ContainsKey(user.ZoneID + "_" + user.UserID))
				{
					m_players.Add(user.ZoneID + "_" + user.UserID, user);
				}
			}
		}

		public UserCityBattleInfo CreatePlayer(Player p)
		{
			UserCityBattleInfo userCityBattleInfo = new UserCityBattleInfo();
			userCityBattleInfo.UserID = p.Id;
			userCityBattleInfo.ZoneID = p.ZoneId;
			userCityBattleInfo.ZoneName = p.ZoneName;
			userCityBattleInfo.Team = 0;
			userCityBattleInfo.Score = 0;
			userCityBattleInfo.myScore = 0;
			userCityBattleInfo.NickName = p.NickName;
			userCityBattleInfo.Rank = 0;
			userCityBattleInfo.Sex = p.Info.Sex;
			userCityBattleInfo.Style = p.Info.Style;
			userCityBattleInfo.Colors = p.Info.Colors;
			userCityBattleInfo.Skin = p.Info.Skin;
			userCityBattleInfo.Hide = p.Info.Hide;
			userCityBattleInfo.ExchangeInfo = CloneExchangeInfo();
			AddPlayer(userCityBattleInfo);
			return userCityBattleInfo;
		}

		private List<UserCityBattleExchageInfo> CloneExchangeInfo()
		{
			lock (m_cloneExchage)
			{
				List<UserCityBattleExchageInfo> list = new List<UserCityBattleExchageInfo>();
				foreach (UserCityBattleExchageInfo item in m_cloneExchage)
				{
					list.Add(item.Clone());
				}
				return list;
			}
		}

		public List<ItemInfo> CreateAward(List<CityOccupationSystemsInfo> list)
		{
			List<ItemInfo> list2 = new List<ItemInfo>();
			foreach (CityOccupationSystemsInfo item in list)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), item.Count, 105);
				itemInfo.IsBinds = item.IsBind;
				itemInfo.ValidDate = item.ValidDate;
				itemInfo.StrengthenLevel = item.StrengthLevel;
				itemInfo.AttackCompose = item.AttackCompose;
				itemInfo.DefendCompose = item.DefendCompose;
				itemInfo.AgilityCompose = item.AgilityCompose;
				itemInfo.LuckCompose = item.LuckCompose;
				list2.Add(itemInfo);
			}
			return list2;
		}

		public CityOccupationSystemsInfo FindAward(int id)
		{
			return m_cityAwards.Values.SingleOrDefault((CityOccupationSystemsInfo a) => a.ID == id);
		}

		public List<CityOccupationSystemsInfo> FindAward(int quailty, int probability)
		{
			return m_cityAwards.Values.Where((CityOccupationSystemsInfo a) => a.Quality == quailty && a.Probability == probability).ToList();
		}

		public UserCityBattleInfo FindPlayer(int zoneId, int userId)
		{
			lock (m_players)
			{
				return m_players.ContainsKey(zoneId + "_" + userId) ? m_players[zoneId + "_" + userId] : null;
			}
		}

		public int GetLowPlayerTeam()
		{
			return (GetPlayerWithTeam(1, sort: false).Count <= GetPlayerWithTeam(2, sort: false).Count) ? 1 : 2;
		}

		public List<UserCityBattleInfo> GetPlayerWithTeam(int team, bool sort)
		{
			return sort ? (from a in m_players.Values
				where a.Team == team
				orderby a.Rank
				select a).ToList() : m_players.Values.Where((UserCityBattleInfo a) => a.Team == team).ToList();
		}

		public List<UserCityBattleInfo> GetTOP10PlayerWithTeam(int team)
		{
			return (from a in m_players.Values
				where a.Team == team && a.Rank != 0
				orderby a.Rank
				select a).Take(10).ToList();
		}

		public UserCityBattleInfo GetPlayerHighRank(int team)
		{
			List<UserCityBattleInfo> list = (from a in m_players.Values
				where a.Team == team
				orderby a.Rank
				select a).Take(1).ToList();
			return (list != null && list.Count > 0) ? list[0] : null;
		}

		public List<UserCityBattleInfo> GetAllPlayers()
		{
			List<UserCityBattleInfo> list = new List<UserCityBattleInfo>();
			lock (m_players)
			{
				list.AddRange(m_players.Values);
			}
			return list;
		}

		public List<UserCityBattleInfo> GetAllWinner()
		{
			int num = (int)((m_dayFighting.DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)7) : m_dayFighting.DayOfWeek);
			List<UserCityBattleInfo> list = new List<UserCityBattleInfo>();
			lock (m_listWinner)
			{
				foreach (KeyValuePair<int, UserCityBattleInfo> item in m_listWinner)
				{
					if (item.Key < num || (item.Key == num && item.Value != null))
					{
						list.Add(item.Value);
						continue;
					}
					break;
				}
			}
			return list;
		}

		public UserCityBattleInfo GetWinnerInWeek(int dayOfweek)
		{
			lock (m_listWinner)
			{
				if (m_listWinner.ContainsKey(dayOfweek))
				{
					return m_listWinner[dayOfweek];
				}
			}
			return null;
		}

		private void ResetListDayWinner()
		{
			lock (m_listWinner)
			{
				m_listWinner = new Dictionary<int, UserCityBattleInfo>();
				for (int i = 1; i <= 7; i++)
				{
					m_listWinner.Add(i, null);
				}
			}
		}

		public void InstallRank()
		{
			m_totalBlueScore = 0;
			m_totalRedScore = 0;
			List<UserCityBattleInfo> allPlayers = GetAllPlayers();
			int num = 1;
			foreach (UserCityBattleInfo item in from a in allPlayers
				where a.Team == 1
				orderby a.Score descending, a.Rank
				select a)
			{
				item.Rank = num;
				m_totalBlueScore += item.Score;
				num++;
			}
			int num2 = 1;
			foreach (UserCityBattleInfo item2 in from a in allPlayers
				where a.Team == 2
				orderby a.Score descending, a.Rank
				select a)
			{
				item2.Rank = num2;
				m_totalRedScore += item2.Score;
				num2++;
			}
		}

		private void ResetWeek()
		{
			lock (m_players)
			{
				m_totalBlueScore = 0;
				m_totalRedScore = 0;
				m_dayFighting = DateTime.Now;
				ResetListDayWinner();
				m_players.Clear();
				SendResetData();
			}
		}

		private void ResetDay()
		{
			lock (m_players)
			{
				m_totalBlueScore = 0;
				m_totalRedScore = 0;
				m_dayFighting = DateTime.Now;
				foreach (UserCityBattleInfo value in m_players.Values)
				{
					value.Score = 0;
					value.Rank = 0;
				}
			}
		}

		private void SendResetData()
		{
			ZoneLoginMgr[] allZoneLogin = ServerMgr.GetAllZoneLogin();
			foreach (ZoneLoginMgr zoneLoginMgr in allZoneLogin)
			{
				Player[] allPlayer = zoneLoginMgr.GetAllPlayer();
				foreach (Player player in allPlayer)
				{
					if (player != null && player.State == ePlayerState.Play)
					{
						GSPacketIn pkg = WorldMgr.CityBattle.SendCityBattleOpen(player.Id, 0);
						player.CurrentServer.SendTCP(pkg);
					}
				}
			}
		}

		private void SetWinner()
		{
			int num = (int)((m_dayFighting.DayOfWeek == DayOfWeek.Sunday) ? ((DayOfWeek)7) : m_dayFighting.DayOfWeek);
			lock (m_listWinner)
			{
				m_listWinner[num] = ((m_totalBlueScore <= m_totalRedScore) ? GetPlayerHighRank(2) : GetPlayerHighRank(1));
				List<CityOccupationSystemsInfo> list = FindAward(5, num);
				if (list.Count > 0 && m_listWinner[num] != null)
				{
					SendToMail(CreateAward(list), m_listWinner[num], LanguageMgr.GetTranslation("CenterServer.CityBattle.Awarday.Title"), LanguageMgr.GetTranslation("CenterServer.CityBattle.Awarday.Content"));
				}
			}
		}

		public void Exchange(UserCityBattleInfo user, CityOccupationSystemsInfo award, int count)
		{
			List<ItemInfo> list = CreateAward(new List<CityOccupationSystemsInfo> { award });
			list[0].Count *= count;
			SendToMail(list, user, LanguageMgr.GetTranslation("CenterServer.CityBattle.Exchange.Content"), LanguageMgr.GetTranslation("CenterServer.CityBattle.Exchange.Content"));
		}

		public void SendAwardRandomTeam(UserCityBattleInfo user)
		{
			List<CityOccupationSystemsInfo> list = FindAward(4, 1);
			if (list.Count > 0)
			{
				SendToMail(CreateAward(list), user, LanguageMgr.GetTranslation("CenterServer.CityBattle.RandomTeam.Award.Title"), LanguageMgr.GetTranslation("CenterServer.CityBattle.RandomTeam.Award.Content"));
			}
		}

		public void SendToMail(List<ItemInfo> items, UserCityBattleInfo user, string title, string content)
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

		private bool CheckOpen()
		{
			TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
			m_isOpen = timeOfDay >= m_timeStart && timeOfDay < m_timeEnd;
			return m_isOpen;
		}

		public void UpdateCityBattleTimer()
		{
			DateTime now = DateTime.Now;
			bool isOpen = m_isOpen;
			if (CheckOpen())
			{
				Console.WriteLine("In opening......");
				if (now.GetWeekOfMonth() != m_dayFighting.GetWeekOfMonth())
				{
					Console.WriteLine("--resetweek......");
					ResetWeek();
				}
				else if (now.DayOfWeek != m_dayFighting.DayOfWeek)
				{
					Console.WriteLine("--resetday......");
					InstallRank();
					ResetDay();
				}
				else
				{
					InstallRank();
				}
			}
			else if (isOpen && now.DayOfWeek == m_dayFighting.DayOfWeek)
			{
				Console.WriteLine("In lasttime is Open and is today => set winner......");
				InstallRank();
				SetWinner();
			}
			else
			{
				Console.WriteLine("LasOpen: " + isOpen + "|CurrentOpen: " + m_isOpen + "|timeOpen: " + m_timeStart.ToString() + "|timeClose: " + m_timeEnd.ToString());
			}
		}

		public GSPacketIn SendUpdateScorePkg(UserCityBattleInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(368, user.UserID);
			gSPacketIn.WriteByte(8);
			gSPacketIn.WriteInt(WorldMgr.CityBattle.TotalBlueScore);
			gSPacketIn.WriteInt(WorldMgr.CityBattle.TotalRedScore);
			gSPacketIn.WriteInt(user.Score);
			gSPacketIn.WriteInt(user.Rank);
			return gSPacketIn;
		}

		public GSPacketIn SendCityBattleOpen(int userId, int mySlide)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(368, userId);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(mySlide);
			return gSPacketIn;
		}

		public GSPacketIn SendExchangeInfo(UserCityBattleInfo user)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(368, user.UserID);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(user.ExchangeInfo.Count);
			foreach (UserCityBattleExchageInfo item in user.ExchangeInfo)
			{
				gSPacketIn.WriteInt(item.ID);
				gSPacketIn.WriteInt(item.Count);
			}
			gSPacketIn.WriteInt(user.myScore);
			return gSPacketIn;
		}

		public void Save()
		{
			lock (m_players)
			{
				ServerDataMgr.Update("CityBattleAccount", JsonConvert.SerializeObject(m_players));
			}
			lock (m_listWinner)
			{
				ServerDataMgr.Update("CityBattleListWinner", JsonConvert.SerializeObject(m_listWinner));
			}
			ServerDataMgr.Update("CityBattleDayFight", m_dayFighting);
		}
	}
}

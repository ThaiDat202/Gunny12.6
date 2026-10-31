using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class AchievementInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_lock;

	protected List<BaseAchievement> m_list;

	private Dictionary<int, AchievementDataInfo> m_success;

	private GamePlayer m_player;

	protected List<BaseAchievement> m_changedAchs = new List<BaseAchievement>();

	private int m_changeCount;

	public AchievementInventory(GamePlayer player)
	{
		m_player = player;
		m_lock = new object();
		m_list = new List<BaseAchievement>();
		m_success = new Dictionary<int, AchievementDataInfo>();
	}

	public void LoadFromDatabase(int playerId)
	{
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			AchievementDataInfo[] allAchievementData = playerBussiness.GetAllAchievementData(playerId);
			BeginChanges();
			AchievementDataInfo[] array = allAchievementData;
			foreach (AchievementDataInfo achievementDataInfo in array)
			{
				if (achievementDataInfo.IsComplete)
				{
					m_success.Add(achievementDataInfo.AchievementID, achievementDataInfo);
					continue;
				}
				AchievementInfo singleAchievement = AchievementMgr.GetSingleAchievement(achievementDataInfo.AchievementID);
				if (singleAchievement != null)
				{
					AddAchievement(new BaseAchievement(singleAchievement, achievementDataInfo));
				}
			}
			AddAchievementPre();
			CommitChanges();
		}
	}

	public void SaveToDatabase()
	{
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			foreach (AchievementDataInfo value in m_success.Values)
			{
				if (value.IsDirty)
				{
					playerBussiness.UpdateAchievementData(value);
				}
			}
		}
	}

	public bool Finish(BaseAchievement achiev)
	{
		AchievementInfo info = achiev.Info;
		AchievementDataInfo data = achiev.Data;
		m_player.BeginAllChanges();
		try
		{
			if (achiev.Finish(m_player))
			{
				List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
				foreach (AchievementRewardInfo item in list)
				{
					switch (item.RewardType)
					{
					case 1:
						m_player.Rank.AddAchievementRank(item.RewardPara);
						break;
					case 3:
					{
						ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(int.Parse(item.RewardPara));
						if (itemTemplateInfo != null)
						{
							ItemInfo cloneItem = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 106);
							m_player.AddTemplate(cloneItem);
							m_player.SendMessage(LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward") + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", itemTemplateInfo.Name, 1));
						}
						break;
					}
					}
				}
				if (info.AchievementPoint != 0)
				{
					m_player.AddAchievementPoint(info.AchievementPoint);
				}
				SendAchievementSuccess(data);
				AddAchievementFinish(data);
				m_player.OnAchievementFinish(data);
				AddNextAchievement(data.AchievementID);
				RemoveAchievement(achiev);
			}
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Achivement Finish：" + ex);
			}
			return false;
		}
		finally
		{
			m_player.CommitAllChanges();
		}
		return true;
	}

	public bool RemoveAchievement(BaseAchievement ach)
	{
		ach.RemoveFromPlayer(m_player);
		return true;
	}

	public void AddAchievementPre()
	{
		List<AchievementInfo> list = AchievementMgr.Achievement.Values.ToList();
		foreach (AchievementInfo item in list)
		{
			if (FindAchievement(item.ID) == null && !IsAchievementFinish(item.ID))
			{
				AddAchievement(item);
			}
		}
	}

	private bool AddAchievement(BaseAchievement ach)
	{
		lock (m_list)
		{
			m_list.Add(ach);
		}
		OnAchievementsChanged(ach);
		ach.AddToPlayer(m_player);
		if (ach.CanCompleted(m_player))
		{
			Finish(ach);
		}
		return true;
	}

	private void AddNextAchievement(int id)
	{
		List<AchievementInfo> list = new List<AchievementInfo>();
		foreach (AchievementInfo item in list)
		{
			if (!(item.PreAchievementID != "0,"))
			{
				continue;
			}
			string[] array = item.PreAchievementID.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text != null && text != "")
				{
					AchievementInfo singleAchievement = AchievementMgr.GetSingleAchievement(int.Parse(text));
					if (singleAchievement != null && singleAchievement.ID == id)
					{
						AddAchievement(item);
						break;
					}
				}
			}
		}
	}

	public bool AddAchievement(AchievementInfo info)
	{
		try
		{
			if (info == null || m_player.PlayerCharacter.Grade < info.NeedMinLevel || m_player.PlayerCharacter.Grade > info.NeedMaxLevel)
			{
				return false;
			}
			if (info.PreAchievementID != "0,")
			{
				string[] array = info.PreAchievementID.Split(',');
				for (int i = 0; i < array.Length - 1; i++)
				{
					if (!IsAchievementFinish(Convert.ToInt32(array[i])))
					{
						return false;
					}
				}
			}
		}
		catch (Exception ex)
		{
			log.Info(ex.InnerException);
		}
		BaseAchievement baseAchievement = FindAchievement(info.ID);
		if (baseAchievement != null)
		{
			return false;
		}
		BeginChanges();
		baseAchievement = new BaseAchievement(info, new AchievementDataInfo());
		AddAchievement(baseAchievement);
		CommitChanges();
		return true;
	}

	private bool IsAchievementFinish(int id)
	{
		lock (m_success)
		{
			if (m_success.ContainsKey(id))
			{
				return true;
			}
			return false;
		}
	}

	private void AddAchievementFinish(AchievementDataInfo ach)
	{
		lock (m_success)
		{
			if (!m_success.ContainsKey(ach.AchievementID))
			{
				m_success.Add(ach.AchievementID, ach);
			}
		}
	}

	public BaseAchievement FindAchievement(int id)
	{
		foreach (BaseAchievement item in m_list)
		{
			if (item.Info.ID == id)
			{
				return item;
			}
		}
		return null;
	}

	public List<AchievementDataInfo> GetSuccessAchievement()
	{
		lock (m_success)
		{
			return m_success.Values.ToList();
		}
	}

	public AchievementDataInfo GetSuccessAchievement(int achid)
	{
		lock (m_success)
		{
			if (m_success.ContainsKey(achid))
			{
				return m_success[achid];
			}
			return null;
		}
	}

	protected void OnAchievementsChanged(BaseAchievement ach)
	{
		if (!m_changedAchs.Contains(ach))
		{
			m_changedAchs.Add(ach);
		}
		if (m_changeCount <= 0 && m_changedAchs.Count > 0)
		{
			UpdateChangedAchievements();
		}
	}

	private void BeginChanges()
	{
		Interlocked.Increment(ref m_changeCount);
	}

	private void CommitChanges()
	{
		int num = Interlocked.Decrement(ref m_changeCount);
		if (num < 0)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
			}
			Thread.VolatileWrite(ref m_changeCount, 0);
		}
		if (num <= 0 && m_changedAchs.Count > 0)
		{
			UpdateChangedAchievements();
		}
	}

	public void UpdateChangedAchievements()
	{
		m_changedAchs.Clear();
	}

	public GSPacketIn SendAchievementSuccess(AchievementDataInfo info)
	{
		if (m_player == null || info == null)
		{
			return null;
		}
		int year = DateTime.Now.Year;
		int month = DateTime.Now.Month;
		int day = DateTime.Now.Day;
		GSPacketIn gSPacketIn = new GSPacketIn(230, m_player.PlayerCharacter.ID);
		gSPacketIn.WriteInt(info.AchievementID);
		gSPacketIn.WriteInt(year);
		gSPacketIn.WriteInt(month);
		gSPacketIn.WriteInt(day);
		return gSPacketIn;
	}

	public void Update(BaseAchievement ach)
	{
		OnAchievementsChanged(ach);
	}
}

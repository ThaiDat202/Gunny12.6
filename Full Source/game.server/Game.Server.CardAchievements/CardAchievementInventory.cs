using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.CardAchievements;

public class CardAchievementInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_lock;

	private Dictionary<int, CardAchievementDataInfo> m_list;

	private GamePlayer m_player;

	private int[] m_property;

	private int m_changeCount;

	private System.Timers.Timer timer;

	public int[] Property => m_property;

	public CardAchievementInventory(GamePlayer player)
	{
		m_player = player;
		m_lock = new object();
		m_list = new Dictionary<int, CardAchievementDataInfo>();
		m_property = new int[8];
	}

	public void LoadFromDatabase(int playerId)
	{
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			CardAchievementDataInfo[] allCardAchievementData = playerBussiness.GetAllCardAchievementData(playerId);
			CardAchievementDataInfo[] array = allCardAchievementData;
			foreach (CardAchievementDataInfo cardAchievementDataInfo in array)
			{
				CardAchievementInfo cardAchievementInfo = CardAchievementMgr.FindCardAchievement(cardAchievementDataInfo.AchievementID);
				if (cardAchievementInfo != null)
				{
					AddAchievement(cardAchievementDataInfo);
				}
			}
			if (m_list.Count == 0)
			{
				List<CardAchievementInfo> allCardAchievement = CardAchievementMgr.GetAllCardAchievement();
				foreach (CardAchievementInfo item in allCardAchievement)
				{
					CardAchievementDataInfo cardAchievementDataInfo2 = new CardAchievementDataInfo();
					cardAchievementDataInfo2.UserID = m_player.PlayerId;
					cardAchievementDataInfo2.AchievementID = item.AchievementID;
					cardAchievementDataInfo2.CompletedDate = DateTime.Now.AddDays(-7.0);
					cardAchievementDataInfo2.Condition = item.Type;
					AddAchievement(cardAchievementDataInfo2);
				}
			}
			UpdateProperty();
		}
	}

	public void SaveToDatabase()
	{
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			foreach (CardAchievementDataInfo value in m_list.Values)
			{
				if (value.IsDirty)
				{
					playerBussiness.UpdateCardAchievementData(value);
				}
			}
		}
	}

	public void CheckFinish()
	{
		if (timer == null)
		{
			timer = new System.Timers.Timer(4000.0);
			timer.Elapsed += sendUpdate;
			timer.Start();
		}
		else
		{
			timer.Stop();
			timer.Interval = 4000.0;
			timer.Start();
		}
	}

	private void sendUpdate(object sender, ElapsedEventArgs e)
	{
		UpdateChangedAchievements();
		timer.Stop();
	}

	public bool Finish(int type)
	{
		BeginChanges();
		lock (m_list)
		{
			foreach (CardAchievementDataInfo value in m_list.Values)
			{
				if (value.Condition == type && value.IsComplete == 0)
				{
					m_list[value.AchievementID].CompletedDate = DateTime.Now;
					m_list[value.AchievementID].IsComplete = type;
					AddHonor(value.AchievementID);
				}
			}
		}
		CommitChanges();
		return false;
	}

	public void AddHonor(int id)
	{
		CardAchievementInfo cardAchievementInfo = CardAchievementMgr.FindCardAchievement(id);
		if (cardAchievementInfo != null)
		{
			UserRankInfo rankByHonnor = m_player.Rank.GetRankByHonnor(cardAchievementInfo.Honor_id);
			if (rankByHonnor == null)
			{
				m_player.Rank.AddNewRank(cardAchievementInfo.Honor_id);
			}
		}
	}

	public CardAchievementDataInfo FindCard(int id)
	{
		if (m_list.ContainsKey(id))
		{
			return m_list[id];
		}
		return null;
	}

	private bool AddAchievement(CardAchievementDataInfo ach)
	{
		lock (m_list)
		{
			if (!m_list.ContainsKey(ach.AchievementID))
			{
				m_list.Add(ach.AchievementID, ach);
			}
		}
		return true;
	}

	public void UpdateProperty()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		foreach (CardAchievementDataInfo value in m_list.Values)
		{
			CardAchievementInfo cardAchievementInfo = CardAchievementMgr.FindCardAchievement(value.AchievementID);
			if (cardAchievementInfo != null && value.IsComplete > 0)
			{
				num += cardAchievementInfo.AddAttack;
				num2 += cardAchievementInfo.AddDefend;
				num3 += cardAchievementInfo.AddLucky;
				num6 += cardAchievementInfo.AddDamage;
				num4 += cardAchievementInfo.AddGuard;
				num7 += cardAchievementInfo.AddMagicAttack;
				num8 += cardAchievementInfo.AddMagicDefend;
				num5 += cardAchievementInfo.AddBlood;
			}
		}
		lock (m_lock)
		{
			m_property[0] = num;
			m_property[1] = num2;
			m_property[2] = num3;
			m_property[3] = num4;
			m_property[4] = num5;
			m_property[5] = num6;
			m_property[6] = num7;
			m_property[7] = num8;
		}
	}

	protected void OnAchievementsChanged(CardAchievementDataInfo ach)
	{
		if (m_changeCount <= 0)
		{
			UpdateProperty();
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
		if (num <= 0)
		{
			UpdateProperty();
			UpdateChangedAchievements();
		}
	}

	public void UpdateChangedAchievements()
	{
		m_player.Out.SendCardAchievementUpdate(m_player.PlayerId, m_property, m_list.Values.ToList());
	}
}

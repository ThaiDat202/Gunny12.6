using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class BaseAchievement
{
	private AchievementInfo m_info;

	private AchievementDataInfo m_data;

	private List<BaseCondition> m_list;

	private GamePlayer m_player;

	public AchievementInfo Info => m_info;

	public AchievementDataInfo Data => m_data;

	public BaseAchievement(AchievementInfo info, AchievementDataInfo data)
	{
		CreateBaseAchievement(info, data);
	}

	public void CreateBaseAchievement(AchievementInfo info, AchievementDataInfo data)
	{
		m_info = info;
		m_data = data;
		m_data.AchievementID = m_info.ID;
		m_list = new List<BaseCondition>();
		List<AchievementConditionInfo> achievementCondition = AchievementMgr.GetAchievementCondition(info);
		foreach (AchievementConditionInfo item in achievementCondition)
		{
			int value = (data.IsComplete ? 1 : 0);
			BaseCondition baseCondition = BaseCondition.CreateCondition(this, item, value);
			if (baseCondition != null)
			{
				m_list.Add(baseCondition);
			}
		}
	}

	public BaseCondition GetConditionById(int id)
	{
		foreach (BaseCondition item in m_list)
		{
			if (item.Info.CondictionID == id)
			{
				return item;
			}
		}
		return null;
	}

	public void AddToPlayer(GamePlayer player)
	{
		m_player = player;
		m_data.UserID = player.PlayerCharacter.ID;
		if (!m_data.IsComplete)
		{
			AddTrigger(player);
		}
	}

	public void RemoveFromPlayer(GamePlayer player)
	{
		if (m_data.IsComplete)
		{
			RemveTrigger(player);
		}
		m_player = null;
	}

	private void AddTrigger(GamePlayer player)
	{
		foreach (BaseCondition item in m_list)
		{
			item.AddTrigger(player);
		}
	}

	private void RemveTrigger(GamePlayer player)
	{
		foreach (BaseCondition item in m_list)
		{
			item.RemoveTrigger(player);
		}
	}

	public void SaveData()
	{
		if (m_player == null)
		{
			return;
		}
		foreach (BaseCondition item in m_list)
		{
		}
	}

	public void Update()
	{
		SaveData();
		if (m_player != null)
		{
			m_player.AchievementInventory.Update(this);
			if (CanCompleted(m_player))
			{
				m_player.AchievementInventory.Finish(this);
			}
		}
	}

	public bool CanCompleted(GamePlayer player)
	{
		if (m_data.IsComplete)
		{
			return false;
		}
		foreach (BaseCondition item in m_list)
		{
			if (!item.IsCompleted(player))
			{
				return false;
			}
		}
		return true;
	}

	public bool Finish(GamePlayer player)
	{
		if (CanCompleted(player))
		{
			foreach (BaseCondition item in m_list)
			{
				if (!item.Finish(player))
				{
					return false;
				}
			}
			m_data.IsComplete = true;
			RemveTrigger(player);
			m_data.CompletedDate = DateTime.Now;
			return true;
		}
		return false;
	}
}

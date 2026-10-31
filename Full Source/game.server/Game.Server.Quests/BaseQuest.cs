using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class BaseQuest
{
	private QuestInfo m_info;

	private QuestDataInfo m_data;

	private List<BaseCondition> m_list;

	private GamePlayer m_player;

	private List<int> m_questColect;

	private DateTime m_oldFinishDate;

	public QuestInfo Info => m_info;

	public QuestDataInfo Data => m_data;

	public BaseQuest(QuestInfo info, QuestDataInfo data)
	{
		m_info = info;
		m_data = data;
		m_data.QuestID = m_info.ID;
		m_list = new List<BaseCondition>();
		m_questColect = new List<int>();
		List<QuestConditionInfo> questCondiction = QuestMgr.GetQuestCondiction(info);
		int num = 0;
		foreach (QuestConditionInfo item in questCondiction)
		{
			BaseCondition baseCondition = BaseCondition.CreateCondition(this, item, data.GetConditionValue(num++));
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
		if (m_player != null)
		{
			m_player.QuestColect = m_questColect;
		}
		if (!m_data.IsComplete)
		{
			AddTrigger(player);
		}
	}

	public void RemoveFromPlayer(GamePlayer player)
	{
		if (!m_data.IsComplete)
		{
			RemveTrigger(player);
		}
		m_player = null;
	}

	public void CheckRepeat()
	{
		if ((DateTime.Now.Date - m_data.CompletedDate.Date).TotalDays >= (double)m_info.RepeatInterval && m_info.CanRepeat && m_info.RepeatInterval > 0)
		{
			m_data.RepeatFinish = m_info.RepeatMax;
		}
	}

	public void Reset(GamePlayer player, int rand)
	{
		m_data.QuestID = m_info.ID;
		m_data.UserID = player.PlayerId;
		m_data.IsComplete = false;
		m_data.IsExist = true;
		if (m_data.CompletedDate == DateTime.MinValue)
		{
			m_data.CompletedDate = DateTime.Now;
		}
		if ((DateTime.Now - m_data.CompletedDate).TotalDays >= (double)m_info.RepeatInterval)
		{
			m_data.RepeatFinish = m_info.RepeatMax;
		}
		m_data.RepeatFinish--;
		m_data.RandDobule = rand;
		m_data.QuestLevel = 1;
		if (!player.PlayerCharacter.IsVIPExpire())
		{
			m_data.QuestLevel = ((player.PlayerCharacter.VIPLevel > 5) ? 3 : 2);
		}
		foreach (BaseCondition item in m_list)
		{
			item.Reset(player);
		}
		SaveData();
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
		int num = 0;
		foreach (BaseCondition item in m_list)
		{
			m_data.SaveConditionValue(num++, item.Value);
		}
	}

	public void Update()
	{
		SaveData();
		if (m_data.IsDirty && m_player != null)
		{
			m_player.QuestInventory.Update(this);
		}
	}

	public bool CanCompleted(GamePlayer player)
	{
		if (m_data.IsComplete)
		{
			return false;
		}
		int num = m_info.NotMustCount;
		foreach (BaseCondition item in m_list)
		{
			if (!item.IsCompleted(player))
			{
				if (!item.Info.isOpitional)
				{
					return false;
				}
			}
			else
			{
				num--;
			}
		}
		if (num > 0)
		{
			return false;
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
			if (!Info.CanRepeat)
			{
				m_data.IsComplete = true;
				RemveTrigger(player);
			}
			m_oldFinishDate = m_data.CompletedDate;
			m_data.CompletedDate = DateTime.Now;
			return true;
		}
		return false;
	}

	public bool CancelFinish(GamePlayer player)
	{
		m_data.IsComplete = false;
		m_data.CompletedDate = m_oldFinishDate;
		foreach (BaseCondition item in m_list)
		{
			item.CancelFinish(player);
		}
		return true;
	}
}

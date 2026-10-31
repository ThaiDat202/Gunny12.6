using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class GameKillByGameCondition : BaseCondition
{
	public GameKillByGameCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.AfterKillingLiving += player_AfterKillingLiving;
		player.QuestOneKeyFinish += player_QuestOneKeyFinish;
	}

	private void player_QuestOneKeyFinish(QuestConditionInfo condition)
	{
		if (condition.QuestID == m_info.QuestID && condition.Para2 <= m_info.Para2 && base.Value < m_info.Para2)
		{
			base.Value = condition.Para2;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.QuestOneKeyFinish -= player_QuestOneKeyFinish;
		player.AfterKillingLiving -= player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
	{
		if (isLiving || type != 1)
		{
			return;
		}
		switch (game.GameType)
		{
		case eGameType.Free:
			if ((m_info.Para1 == 0 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.Guild:
			if ((m_info.Para1 == 1 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.Training:
			if ((m_info.Para1 == 2 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.ALL:
			if ((m_info.Para1 == 4 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.Exploration:
			if ((m_info.Para1 == 5 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.FightLib:
			if ((m_info.Para1 == 8 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.matchNpc:
			if ((m_info.Para1 == 9 || m_info.Para1 == -1) && base.Value < m_info.Para2)
			{
				base.Value++;
			}
			break;
		}
		if (base.Value >= m_info.Para2)
		{
			base.Value = m_info.Para2;
		}
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Para2;
	}
}

using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class GameOverByGameCondition : BaseCondition
{
	public GameOverByGameCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.GameOver += player_GameOver;
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
	}

	private void player_GameOver(AbstractGame game, bool isWin, int gainXp)
	{
		if (!isWin)
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

using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class GameCopyPassCondition : BaseCondition
{
	public GameCopyPassCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.MissionOver += player_MissionOver;
		player.QuestOneKeyFinish += player_QuestOneKeyFinish;
	}

	private void player_QuestOneKeyFinish(QuestConditionInfo condition)
	{
		if (condition.QuestID == m_info.QuestID && condition.Para2 <= m_info.Para2 && base.Value > 0)
		{
			base.Value = 0;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.QuestOneKeyFinish -= player_QuestOneKeyFinish;
		player.MissionOver -= player_MissionOver;
	}

	private void player_MissionOver(AbstractGame game, int missionId, bool isWin)
	{
		if (isWin && missionId == m_info.Para1 && base.Value > 0)
		{
			base.Value--;
		}
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value <= 0;
	}
}

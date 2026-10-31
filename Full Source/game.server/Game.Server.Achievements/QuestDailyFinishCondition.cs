using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class QuestDailyFinishCondition : BaseCondition
{
	public QuestDailyFinishCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.QuestFinishEvent += player_GiftToken;
	}

	private void player_GiftToken(QuestDataInfo data, QuestInfo info)
	{
		if (info.QuestID == 2)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.QuestFinishEvent -= player_GiftToken;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}

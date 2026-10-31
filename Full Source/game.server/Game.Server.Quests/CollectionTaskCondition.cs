using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class CollectionTaskCondition : BaseCondition
{
	public CollectionTaskCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.CollectionTaskEvent += player_CollectionTask;
	}

	private void player_CollectionTask(int type)
	{
		if (m_info.Para1 == type && base.Value < m_info.Para1)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.CollectionTaskEvent -= player_CollectionTask;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Para2;
	}
}

using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class UserToemGemstoneCondition : BaseCondition
{
	public UserToemGemstoneCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.UserToemGemstonetEvent += player_UserToemGemstone;
	}

	private void player_UserToemGemstone(int type)
	{
		if (m_info.Para1 == type && base.Value == 0)
		{
			base.Value = m_info.Para2;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.UserToemGemstonetEvent -= player_UserToemGemstone;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value <= m_info.Para2;
	}
}

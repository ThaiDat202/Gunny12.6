using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class GiftTokenCollectionCondition : BaseCondition
{
	public GiftTokenCollectionCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.GiftTokenCollect += player_GiftToken;
	}

	private void player_GiftToken(int value)
	{
		base.Value += value;
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.GiftTokenCollect -= player_GiftToken;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}

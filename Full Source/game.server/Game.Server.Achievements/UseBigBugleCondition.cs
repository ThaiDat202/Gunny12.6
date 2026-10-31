using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class UseBigBugleCondition : BaseCondition
{
	public UseBigBugleCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.UseBugle += player_UseBugle;
	}

	private void player_UseBugle(int templateid)
	{
		if (templateid == 11102)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.UseBugle -= player_UseBugle;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}

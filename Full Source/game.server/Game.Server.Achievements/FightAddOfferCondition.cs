using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class FightAddOfferCondition : BaseCondition
{
	public FightAddOfferCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.FightAddOfferEvent += player_AddOffer;
	}

	private void player_AddOffer(int value)
	{
		base.Value += value;
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.FightAddOfferEvent -= player_AddOffer;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}

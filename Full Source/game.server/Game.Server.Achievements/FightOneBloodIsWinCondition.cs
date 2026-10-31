using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class FightOneBloodIsWinCondition : BaseCondition
{
	public FightOneBloodIsWinCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.FightOneBloodIsWin += player_AddOffer;
	}

	private void player_AddOffer(eRoomType roomType)
	{
		base.Value++;
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.FightOneBloodIsWin -= player_AddOffer;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}

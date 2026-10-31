using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class GameOverPassCondition : BaseCondition
{
	private int m_type;

	public GameOverPassCondition(BaseAchievement quest, AchievementConditionInfo info, int type, int value)
		: base(quest, info, value)
	{
		m_type = type;
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.MissionFullOver += player_AddOffer;
	}

	private void player_AddOffer(AbstractGame game, int missionId, bool isWin, int turnNum)
	{
		if (!isWin)
		{
			return;
		}
		switch (m_type)
		{
		case 1:
			if (missionId == 6104 || missionId == 6204 || missionId == 6304)
			{
				base.Value++;
			}
			break;
		case 2:
			if (missionId == 7004 || missionId == 7104 || missionId == 7204)
			{
				base.Value++;
			}
			break;
		case 3:
			if (missionId == 3106 || missionId == 3206 || missionId == 3306)
			{
				base.Value++;
			}
			break;
		case 4:
			if (missionId == 1073 || missionId == 1176 || missionId == 1277 || missionId == 1378)
			{
				base.Value++;
			}
			break;
		case 5:
			if (missionId == 2002 || missionId == 2102)
			{
				base.Value++;
			}
			break;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.MissionFullOver -= player_AddOffer;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}

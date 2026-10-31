using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.Game;

public class WarriorsArenaSimpleGame : APVEGameControl
{
	public override int CalculateScoreGrade(int score)
	{
		if (score > 800)
		{
			return 3;
		}
		if (score > 725)
		{
			return 2;
		}
		if (score > 650)
		{
			return 1;
		}
		return 0;
	}

	public override void OnCreated()
	{
		base.Game.SetupMissions("13004");
		base.Game.TotalMissionCount = 1;
	}

	public override void OnGameOverAllSession()
	{
	}

	public override void OnPrepated()
	{
	}
}

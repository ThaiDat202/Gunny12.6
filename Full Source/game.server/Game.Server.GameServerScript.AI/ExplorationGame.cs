using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.Game;

public class ExplorationGame : APVEGameControl
{
	public int totalMissionCount = 0;

	public string missionIds = string.Empty;

	public override void OnCreated()
	{
	}

	public override void OnPrepated()
	{
		base.Game.SessionId = 0;
	}

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

	public override void OnGameOverAllSession()
	{
	}
}

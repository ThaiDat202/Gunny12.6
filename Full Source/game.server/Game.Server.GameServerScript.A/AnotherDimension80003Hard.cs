using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.Game;

public class AnotherDimension80003Hard : APVEGameControl
{
	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 900)
		{
			return 3;
		}
		if (score > 825)
		{
			return 2;
		}
		if (score > 725)
		{
			return 1;
		}
		return 0;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Game.SetupMissions("80013");
		base.Game.TotalMissionCount = 1;
	}

	public override void OnGameOverAllSession()
	{
		base.OnGameOverAllSession();
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
	}
}

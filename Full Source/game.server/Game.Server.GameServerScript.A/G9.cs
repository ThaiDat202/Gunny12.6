using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.Game;

public class G9 : APVEGameControl
{
	public override void OnCreated()
	{
		base.Game.SetupMissions("10000009");
		base.Game.TotalMissionCount = 3;
	}

	public override void OnPrepated()
	{
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

using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.Game;

internal class SteamFactoryNormal : APVEGameControl
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
		base.Game.SetupMissions("200101");
		base.Game.TotalMissionCount = 1;
	}

	public override void OnGameOverAllSession()
	{
	}

	public override void OnPrepated()
	{
		base.Game.SessionId = 0;
	}
}

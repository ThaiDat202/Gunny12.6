namespace Game.Server.GameServerScript.AI.Game;

public class ExplorationGame2 : ExplorationGame
{
	public override void OnCreated()
	{
		base.Game.SetupMissions("1002,1002,1002,1002,1002");
		base.Game.TotalMissionCount = 5;
		base.OnCreated();
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
	}
}

namespace Game.Server.GameServerScript.AI.Game;

public class ExplorationGame4 : ExplorationGame
{
	public override void OnCreated()
	{
		base.Game.SetupMissions("1004,1004,1004,1004,1004");
		base.Game.TotalMissionCount = 5;
		base.OnCreated();
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
	}
}

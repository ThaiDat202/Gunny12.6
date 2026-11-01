namespace Game.Server.GameServerScript.AI.Game;

public class ExplorationGame3 : ExplorationGame
{
	public override void OnCreated()
	{
		base.Game.SetupMissions("1003,1003,1003,1003,1003");
		base.Game.TotalMissionCount = 5;
		base.OnCreated();
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
	}
}

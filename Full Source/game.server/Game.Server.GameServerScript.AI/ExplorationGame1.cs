namespace Game.Server.GameServerScript.AI.Game;

internal class ExplorationGame1 : ExplorationGame
{
	public override void OnCreated()
	{
		base.Game.SetupMissions("1001,1001,1001,1001,1001");
		base.Game.TotalMissionCount = 5;
		base.OnCreated();
	}

	public override void OnPrepated()
	{
		base.OnPrepated();
	}
}

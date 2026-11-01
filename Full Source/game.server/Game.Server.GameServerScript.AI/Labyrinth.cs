using Game.Logic.AI;

namespace Game.Server.GameServerScript.AI.Game
{
  public class Labyrinth : APVEGameControl
  {
    public override int CalculateScoreGrade(int score)
    {
      if (score > 800)
        return 3;
      return score > 725 ? 2 : (score > 650 ? 1 : 0);
    }

    public override void OnCreated()
    {
      this.Game.SetupMissions("7001");
      this.Game.TotalMissionCount = 1;
    }

    public override void OnGameOverAllSession()
    {
    }

    public override void OnPrepated() => this.Game.SessionId = 0;
  }
}

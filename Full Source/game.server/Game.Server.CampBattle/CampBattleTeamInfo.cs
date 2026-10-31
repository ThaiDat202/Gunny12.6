using Game.Server.Games;

namespace Game.Server.CampBattle;

public class CampBattleTeamInfo
{
	public int TeamID { get; set; }

	public int Point { get; set; }

	public bool OpenGate { get; set; }

	public int MosterCount { get; set; }

	public int TotalPlayers => GameMgr.CampBattle.GetListPlayersActive(TeamID).Count;
}

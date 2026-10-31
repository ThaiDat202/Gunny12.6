namespace Game.Server.CampBattle;

public interface ICampBattleAction
{
	void Execute(CampBattleControl battle, long tick);

	bool IsFinished(long tick);
}

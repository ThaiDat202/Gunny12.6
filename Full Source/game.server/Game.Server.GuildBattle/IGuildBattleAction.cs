namespace Game.Server.GuildBattle;

public interface IGuildBattleAction
{
	void Execute(GuildBattleMgr battle, long tick);

	bool IsFinished(long tick);
}

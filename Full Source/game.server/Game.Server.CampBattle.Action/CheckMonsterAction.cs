using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Logic;
using log4net;

namespace Game.Server.CampBattle.Action;

public class CheckMonsterAction : ICampBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	public CheckMonsterAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(CampBattleControl game, long tick)
	{
		if (m_tick > tick)
		{
			return;
		}
		List<CampBattleTeamInfo> allTeam = game.GetAllTeam();
		foreach (CampBattleTeamInfo item in allTeam)
		{
			List<CampBattleMonsterInfo> monster = game.GetMonster(item.TeamID);
			if (monster.Count <= 0)
			{
				game.SendMessenge(LanguageMgr.GetTranslation("CampBattle.Msg.Monster.Respawn", item.MosterCount), item.TeamID);
				game.InstallMonster(item);
				continue;
			}
			List<CampBattleMonsterInfo> livingMonster = game.GetLivingMonster(item.TeamID);
			if (livingMonster.Count <= 0)
			{
				item.MosterCount++;
				game.SendMessenge(LanguageMgr.GetTranslation("CampBattle.Msg.Monster.Respawn", item.MosterCount), item.TeamID);
				game.InstallMonster(item);
				continue;
			}
			foreach (CampBattleMonsterInfo item2 in livingMonster)
			{
				if (item2.StateType == CampBattlePlayerState.FIGHTING && (item2.CurrentGame == null || item2.CurrentGame.GameState == eGameState.Stopped))
				{
					item2.CurrentGame = null;
					item2.StateType = CampBattlePlayerState.NORMAL;
				}
			}
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

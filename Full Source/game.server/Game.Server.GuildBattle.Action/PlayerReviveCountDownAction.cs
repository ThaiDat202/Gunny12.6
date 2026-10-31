using System;
using System.Reflection;
using Game.Logic;
using Game.Server.Games;
using log4net;

namespace Game.Server.GuildBattle.Action;

public class PlayerReviveCountDownAction : IGuildBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	private UserGuildBattleInfo m_player;

	private bool m_stayRespawn;

	public PlayerReviveCountDownAction(UserGuildBattleInfo player, int delay, bool stayRespawn)
	{
		m_player = player;
		m_isFinished = false;
		m_stayRespawn = stayRespawn;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(GuildBattleMgr game, long tick)
	{
		if (m_tick > tick)
		{
			return;
		}
		if (m_player != null && m_player.IsActive && m_player.IsDead)
		{
			m_player.IsDead = false;
			m_player.TombStoneEndTime = DateTime.Now.AddHours(-1.0);
			if (!m_stayRespawn)
			{
				GuildBattleConsortiaInfo guildBattleConsortiaInfo = GameMgr.GuildBattle.FindConsortia(m_player.ConsortiaID);
				if (guildBattleConsortiaInfo != null)
				{
					m_player.Postion = guildBattleConsortiaInfo.DefaultPoint;
				}
			}
			GameMgr.GuildBattle.SendUpdateSceneInfo(m_player);
			GameMgr.GuildBattle.SendUpdatePlayerStatus(m_player);
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}

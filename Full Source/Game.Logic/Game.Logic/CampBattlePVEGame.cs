using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Mission;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class CampBattlePVEGame : PVEGame
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public CampBattlePVEGame(int id, int roomId, PveInfo info, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int currentFloor)
		: base(id, roomId, info, new List<IGamePlayer>(), map, roomType, gameType, timeType, hardLevel, currentFloor)
	{
	}

	public override void CheckState(int delay)
	{
		AddAction(new CheckCampPVEGameStateAction(delay));
	}

	public override void Prepare()
	{
		m_gameState = eGameState.Prepared;
		try
		{
			m_gameAI.OnPrepated();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		SessionId++;
		ClearLoadingFiles();
		ClearMissionData();
		m_gameOverResources.Clear();
		WantTryAgain = 0;
		m_missionInfo = Misssions[SessionId];
		m_pveGameDelay = m_missionInfo.Delay;
		TotalCount = m_missionInfo.TotalCount;
		TotalTurn = m_missionInfo.TotalTurn;
		Param1 = m_missionInfo.Param1;
		Param2 = m_missionInfo.Param2;
		Param3 = -1;
		Param4 = -1;
		m_missionAI = ScriptMgr.CreateInstance(m_missionInfo.Script) as AMissionControl;
		if (m_missionAI == null)
		{
			log.ErrorFormat("Can't create game mission ai :{0}", m_missionInfo.Script);
			m_missionAI = SimpleMissionControl.Simple;
			StopGame();
		}
		if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.Element_PVE)
		{
			Pic = $"show{SessionId}.jpg";
			List<Player> allFightPlayers = GetAllFightPlayers();
			foreach (Player item in allFightPlayers)
			{
				item.PlayerDetail.UpdateBarrier(SessionId, Pic);
			}
		}
		m_missionAI.Game = this;
		try
		{
			m_missionAI.OnPrepareNewSession();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
		}
		CheckState(0);
	}

	public override bool CanAddPlayer()
	{
		lock (m_players)
		{
			return (base.GameState == eGameState.Prepared || base.GameState == eGameState.Playing) && m_players.Count < 4;
		}
	}

	public void InitCreateGame()
	{
		m_gameState = eGameState.SessionPrepared;
		SendCreateGame();
	}

	public void StopGame()
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.ResetRoom(IsWin, "pve," + BossCampId);
			RemovePlayer(item.PlayerDetail, isKick: false);
		}
		m_gameState = eGameState.Stopped;
	}
}

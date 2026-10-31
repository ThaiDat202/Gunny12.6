using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Packets;
using Game.Server.RingStation;

namespace Game.Server.Rooms;

public class StartGameAction : IAction
{
	private BaseRoom m_room;

	public StartGameAction(BaseRoom room)
	{
		m_room = room;
	}

	public void Execute()
	{
		if (!m_room.CanStart())
		{
			return;
		}
		m_room.StartWithNpc = false;
		m_room.PickUpNpcId = -1;
		List<GamePlayer> players = m_room.GetPlayers();
		if (m_room.RoomType == eRoomType.Freedom)
		{
			List<IGamePlayer> list = new List<IGamePlayer>();
			List<IGamePlayer> list2 = new List<IGamePlayer>();
			foreach (GamePlayer item in players)
			{
				if (item != null)
				{
					if (item.CurrentRoomTeam == 1)
					{
						list.Add(item);
					}
					else
					{
						list2.Add(item);
					}
				}
			}
			BaseGame game = GameMgr.StartPVPGame(m_room.RoomId, list, list2, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode);
			StartGame(game);
		}
		else if (IsPVE(m_room.RoomType))
		{
			if (GameMgr.SynDate < 0)
			{
				{
					foreach (GamePlayer item2 in players)
					{
						item2?.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.WaitingPveRestart"));
					}
					return;
				}
			}
			if (m_room.MapId != 0 && m_room.MapId != 100000 && m_room.HardLevel == eHardLevel.Nightmare)
			{
				Console.WriteLine("on nightmare");
				List<string> list3 = new List<string>();
				foreach (GamePlayer item3 in players)
				{
					if (item3 != null)
					{
						Dictionary<int, int> dungeonCount = item3.Extra.GetDungeonCount();
						if (dungeonCount.ContainsKey(m_room.MapId) && dungeonCount[m_room.MapId] >= GameProperties.ChickOrAntTreasureTimes)
						{
							list3.Add(item3.PlayerCharacter.NickName);
						}
					}
				}
				if (list3.Count > 0)
				{
					m_room.Host.SendMessage(LanguageMgr.GetTranslation("DungeonCount.Limit.Today", string.Join(",", list3)));
					m_room.SendCancelPickUp();
					return;
				}
			}
			List<IGamePlayer> list4 = new List<IGamePlayer>();
			foreach (GamePlayer item4 in players)
			{
				if (item4 != null)
				{
					list4.Add(item4);
				}
			}
			UpdatePveRoomTimeMode();
			BaseGame game2 = GameMgr.StartPVEGame(m_room.RoomId, list4, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode, m_room.HardLevel, m_room.LevelLimits, m_room.currentFloor);
			StartGame(game2);
		}
		else if (IsPVP(m_room.RoomType))
		{
			m_room.UpdateAvgLevel();
			if (!m_room.isCrosszone && m_room.RoomType == eRoomType.Match && m_room.GameStyle == 0 && m_room.Host != null)
			{
				if (m_room.Host.PlayerCharacter.Grade <= 10)
				{
					m_room.StartWithNpc = true;
					m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType, 1);
				}
				else
				{
					m_room.StartWithNpc = true;
					m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType, 1);
				}
			}
			if (!m_room.isCrosszone && m_room.RoomType == eRoomType.PolarRegion && m_room.GameStyle == 0 && m_room.Host != null)
			{
				m_room.StartWithNpc = true;
				m_room.PickUpNpcId = RingStationMgr.GetAutoBot(m_room.Host, (int)m_room.RoomType, (int)m_room.GameType, 1);
			}
			BattleServer battleServer = BattleMgr.AddRoom(m_room);
			if (battleServer != null)
			{
				m_room.BattleServer = battleServer;
				m_room.IsPlaying = true;
				m_room.SendStartPickUp();
			}
			else
			{
				GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
				m_room.SendToAll(pkg, m_room.Host);
				m_room.SendCancelPickUp();
			}
		}
		RoomMgr.WaitingRoom.SendUpdateCurrentRoom(m_room);
	}

	private void StartGame(BaseGame game)
	{
		if (game != null)
		{
			m_room.IsPlaying = true;
			m_room.StartGame(game);
			return;
		}
		m_room.IsPlaying = false;
		m_room.SendPlayerState();
		if (m_room.RoomType == eRoomType.Freedom)
		{
			GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
			m_room.SendToAll(pkg, m_room.Host);
		}
		else
		{
			GSPacketIn pkg2 = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("副本尚未开启或者副本已经关闭！"));
			m_room.SendToAll(pkg2, m_room.Host);
		}
		m_room.SendCancelPickUp();
	}

	private bool IsPVP(eRoomType roomType)
	{
		switch (roomType)
		{
		case eRoomType.Match:
		case eRoomType.Entertainment:
		case eRoomType.EntertainmentPK:
		case eRoomType.TEAM_ROOM:
		case eRoomType.PolarRegion:
			return true;
		default:
			return false;
		}
	}

	private bool IsPVE(eRoomType roomType)
	{
		switch (roomType)
		{
		case eRoomType.Dungeon:
		case eRoomType.FightLib:
		case eRoomType.Freshman:
		case eRoomType.AcademyDungeon:
		case eRoomType.WordBossFight:
		case eRoomType.Lanbyrinth:
		case eRoomType.ConsortiaBoss:
		case eRoomType.ActivityDungeon:
		case eRoomType.SpecialActivityDungeon:
		case eRoomType.FarmBoss:
		case eRoomType.Christmas:
		case eRoomType.CatchInsect:
		case eRoomType.Element_PVE:
			return true;
		default:
			return false;
		}
	}

	private void UpdatePveRoomTimeMode()
	{
		switch (m_room.HardLevel)
		{
		case eHardLevel.Easy:
			m_room.TimeMode = 3;
			break;
		case eHardLevel.Normal:
			m_room.TimeMode = 2;
			break;
		case eHardLevel.Hard:
		case eHardLevel.Terror:
		case eHardLevel.Nightmare:
		case eHardLevel.Epic:
			m_room.TimeMode = 1;
			break;
		}
	}
}

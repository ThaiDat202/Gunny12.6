using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Rooms;

public class CreateFightFootballTimeRoomAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	private Random rand;

	public CreateFightFootballTimeRoomAction(GamePlayer player, eRoomType roomType)
	{
		m_player = player;
		m_name = "FightFootballTime";
		m_password = "12dasSda44";
		m_roomType = roomType;
		m_timeType = 2;
		rand = new Random();
	}

	public void Execute()
	{
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		if (!m_player.IsActive)
		{
			return;
		}
		BaseRoom[] rooms = RoomMgr.Rooms;
		BaseRoom baseRoom = null;
		int num = rand.Next(rooms.Length);
		for (int i = 0; i < rooms.Length; i++)
		{
			if (!rooms[num].IsUsing)
			{
				baseRoom = rooms[num];
				break;
			}
			num = rand.Next(rooms.Length);
		}
		if (baseRoom != null)
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			baseRoom.Start();
			baseRoom.HardLevel = eHardLevel.Normal;
			baseRoom.LevelLimits = (int)baseRoom.GetLevelLimit(m_player);
			baseRoom.isCrosszone = true;
			baseRoom.isOpenBoss = false;
			baseRoom.MapId = 1313;
			baseRoom.currentFloor = 1;
			baseRoom.TimeMode = m_timeType;
			baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, baseRoom.MapId);
			GSPacketIn gSPacketIn = m_player.Out.SendRoomCreate(baseRoom);
			baseRoom.AddPlayerUnsafe(m_player);
		}
		BattleServer battleServer = BattleMgr.AddRoom(baseRoom);
		if (battleServer != null)
		{
			baseRoom.BattleServer = battleServer;
			baseRoom.IsPlaying = true;
			baseRoom.SendStartPickUp();
		}
		else
		{
			GSPacketIn pkg = baseRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
			baseRoom.SendToAll(pkg, baseRoom.Host);
			baseRoom.SendCancelPickUp();
		}
	}
}

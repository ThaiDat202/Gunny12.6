using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Rooms;

internal class EnterRoomAction : IAction
{
	private GamePlayer m_player;

	private int m_roomId;

	private string m_pwd;

	private int m_type;

	private bool _isInvite;

	public EnterRoomAction(GamePlayer player, int roomId, string pwd, int type, bool isInvite)
	{
		m_player = player;
		m_roomId = roomId;
		m_pwd = pwd;
		m_type = type;
		_isInvite = isInvite;
	}

	public void Execute()
	{
		if (!m_player.IsActive)
		{
			return;
		}
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		m_player.PlayerCharacter.isViewer = false;
		BaseRoom[] rooms = RoomMgr.Rooms;
		BaseRoom baseRoom = null;
		if (m_roomId == -1)
		{
			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i].PlayerCount <= 0 || !rooms[i].CanAddPlayer() || rooms[i].NeedPassword || rooms[i].IsPlaying)
				{
					continue;
				}
				if (10 != m_type)
				{
					if (rooms[i].RoomType == (eRoomType)m_type)
					{
						baseRoom = rooms[i];
					}
				}
				else if (rooms[i].RoomType == (eRoomType)m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(m_player))
				{
					baseRoom = rooms[i];
				}
			}
			if (baseRoom == null)
			{
				m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg1"));
				m_player.Out.SendRoomLoginResult(result: false);
				return;
			}
		}
		else
		{
			baseRoom = rooms[(m_roomId == 0) ? m_roomId : (m_roomId - 1)];
		}
		if (!baseRoom.IsUsing)
		{
			m_player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("EnterRoomAction.Msg2"));
		}
		else if (baseRoom.RoomType == eRoomType.ActivityDungeon && m_player.Actives.Info.activityTanabataNum > 0)
		{
			m_player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("EnterRoomAction.Msg3"));
		}
		else if (baseRoom.RoomType == eRoomType.FarmBoss && baseRoom.Host != null && m_player != baseRoom.Host && m_player.PlayerCharacter.ID != baseRoom.Host.PlayerCharacter.SpouseID)
		{
			m_player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("EnterRoomAction.Msg7"));
		}
		else if (baseRoom.PlayerCount == baseRoom.PlacesCount)
		{
			if (baseRoom.CanAddViewPlayer())
			{
				RoomMgr.WaitingRoom.RemovePlayer(m_player);
				m_player.Out.SendRoomLoginResult(result: true);
				m_player.Out.SendRoomCreate(baseRoom);
				if (baseRoom.AddPlayerUnsafe(m_player))
				{
					baseRoom.Game?.AddPlayer(m_player);
				}
				RoomMgr.WaitingRoom.SendUpdateCurrentRoom(baseRoom);
				m_player.Out.SendGameRoomSetupChange(baseRoom);
                if (GameProperties.VERSION >= 12100)
                {
                    m_player.Out.SendRoomRobotInfo(false);
                }
                baseRoom.UpdatePlayerState(m_player, 1, sendToClient: false);
			}
			else
			{
				m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg4"));
			}
		}
		else if (!baseRoom.NeedPassword || baseRoom.Password == m_pwd || _isInvite)
		{
			if (baseRoom.Game != null && !baseRoom.Game.CanAddPlayer())
			{
				return;
			}
			if (baseRoom.LevelLimits > (int)baseRoom.GetLevelLimit(m_player))
			{
				m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg5"));
				return;
			}
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			m_player.Out.SendRoomLoginResult(result: true);
			m_player.Out.SendRoomCreate(baseRoom);
			if (baseRoom.AddPlayerUnsafe(m_player))
			{
				baseRoom.Game?.AddPlayer(m_player);
			}
			RoomMgr.WaitingRoom.SendUpdateCurrentRoom(baseRoom);
			m_player.Out.SendGameRoomSetupChange(baseRoom);
		}
		else if (!string.IsNullOrEmpty(baseRoom.Password))
		{
			if (!string.IsNullOrEmpty(m_pwd))
			{
				m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg6"));
			}
			GSPacketIn gSPacketIn = new GSPacketIn(94);
			gSPacketIn.WriteByte(14);
			gSPacketIn.WriteInt(m_roomId);
			m_player.SendTCP(gSPacketIn);
		}
		else
		{
			m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.Msg6"));
			m_player.Out.SendRoomLoginResult(result: false);
		}
	}
}

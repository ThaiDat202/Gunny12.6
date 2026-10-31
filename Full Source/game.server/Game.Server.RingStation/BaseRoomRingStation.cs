using System.Collections.Generic;
using System.Timers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.RingStation.Battle;

namespace Game.Server.RingStation;

public class BaseRoomRingStation
{
	private List<VirtualGamePlayer> m_places;

	public RingStationBattleServer BattleServer;

	public int RoomId;

	public int PickUpNpcId;

	public bool IsAutoBot;

	public bool IsFreedom;

	private AbstractGame m_game;

	private Timer addrom = new Timer();

	public AbstractGame Game => m_game;

	public int RoomType { get; set; }

	public int GameType { get; set; }

	public int GuildId { get; set; }

	public bool IsPlaying { get; set; }

	public BaseRoomRingStation(int roomId)
	{
		RoomId = roomId;
		m_places = new List<VirtualGamePlayer>();
	}

	public bool AddPlayer(VirtualGamePlayer player)
	{
		lock (m_places)
		{
			player.CurRoom = this;
			m_places.Add(player);
		}
		return true;
	}

	internal List<VirtualGamePlayer> GetPlayers()
	{
		return m_places;
	}

	public void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public void SendToAll(GSPacketIn pkg, VirtualGamePlayer except)
	{
		lock (m_places)
		{
			foreach (VirtualGamePlayer place in m_places)
			{
				if (place != null && place != except)
				{
					place.ProcessPacket(pkg);
					pkg.ClearOffset();
				}
			}
		}
	}

	internal void SendTCP(GSPacketIn pkg)
	{
		if (m_game != null)
		{
			BattleServer.Server.SendToGame(m_game.Id, pkg);
		}
	}

	public void RemovePlayer(VirtualGamePlayer player)
	{
		if (BattleServer != null)
		{
			if (m_game != null)
			{
				BattleServer.Server.SendPlayerDisconnet(Game.Id, player.GamePlayerId, RoomId);
				BattleServer.Server.SendRemoveRoom(this);
			}
			IsPlaying = false;
		}
		if (Game != null)
		{
			Game.Stop();
		}
		RingStationMgr.RemovePlayer(player.ID);
	}

	public void StartGame(AbstractGame game)
	{
		m_game = game;
	}
}

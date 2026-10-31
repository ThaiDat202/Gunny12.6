using System.Collections.Generic;
using System.Text;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;

namespace Fighting.Server.Games;

public class BattleGame : PVPGame
{
	private ProxyRoom m_roomRed1;

	private ProxyRoom m_roomBlue1;

	private ProxyRoom m_roomRed2;

	private ProxyRoom m_roomBlue2;

	private ProxyRoom m_roomRed3;

	private ProxyRoom m_roomBlue3;

	private ProxyRoom m_roomRed4;

	private ProxyRoom m_roomBlue4;

	public ProxyRoom Red1 => m_roomRed1;

	public ProxyRoom Blue1 => m_roomBlue1;

	public ProxyRoom Red2 => m_roomRed2;

	public ProxyRoom Blue2 => m_roomBlue2;

	public ProxyRoom Red3 => m_roomRed3;

	public ProxyRoom Blue3 => m_roomBlue3;

	public ProxyRoom Red4 => m_roomRed4;

	public ProxyRoom Blue4 => m_roomBlue4;

	public BattleGame(int id, List<IGamePlayer> red1, ProxyRoom roomRed1, List<IGamePlayer> blue1, ProxyRoom roomBlue1, List<IGamePlayer> red2, ProxyRoom roomRed2, List<IGamePlayer> blue2, ProxyRoom roomBlue2, List<IGamePlayer> red3, ProxyRoom roomRed3, List<IGamePlayer> blue3, ProxyRoom roomBlue3, List<IGamePlayer> red4, ProxyRoom roomRed4, List<IGamePlayer> blue4, ProxyRoom roomBlue4, Map map, eRoomType roomType, eGameType gameType, int timeType)
		: base(id, roomBlue1.RoomId, red1, blue1, red2, blue2, red3, blue3, red4, blue4, map, roomType, gameType, timeType)
	{
		m_roomRed1 = roomRed1;
		m_roomBlue1 = roomBlue1;
		m_roomRed2 = roomRed2;
		m_roomBlue2 = roomBlue2;
		m_roomRed3 = roomRed3;
		m_roomBlue3 = roomBlue3;
		m_roomRed4 = roomRed4;
		m_roomBlue4 = roomBlue4;
	}

	public override void SendToAll(GSPacketIn pkg, IGamePlayer except)
	{
		if (m_roomRed1 != null)
		{
			m_roomRed1.SendToAll(pkg, except);
		}
		if (m_roomBlue1 != null)
		{
			m_roomBlue1.SendToAll(pkg, except);
		}
		if (m_roomRed2 != null)
		{
			m_roomRed2.SendToAll(pkg, except);
		}
		if (m_roomBlue2 != null)
		{
			m_roomBlue2.SendToAll(pkg, except);
		}
		if (m_roomRed3 != null)
		{
			m_roomRed3.SendToAll(pkg, except);
		}
		if (m_roomBlue3 != null)
		{
			m_roomBlue3.SendToAll(pkg, except);
		}
		if (m_roomRed4 != null)
		{
			m_roomRed4.SendToAll(pkg, except);
		}
		if (m_roomBlue4 != null)
		{
			m_roomBlue4.SendToAll(pkg, except);
		}
	}

	public override void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
	{
		switch (team)
		{
		case 1:
		case 3:
		case 5:
			m_roomRed1.SendToAll(pkg, except);
			if (m_roomRed2 != null)
			{
				m_roomRed2.SendToAll(pkg, except);
			}
			if (m_roomRed3 != null)
			{
				m_roomRed3.SendToAll(pkg, except);
			}
			if (m_roomRed4 != null)
			{
				m_roomRed4.SendToAll(pkg, except);
			}
			break;
		case 2:
		case 4:
		case 6:
			m_roomBlue1.SendToAll(pkg, except);
			if (m_roomBlue2 != null)
			{
				m_roomBlue2.SendToAll(pkg, except);
			}
			if (m_roomBlue3 != null)
			{
				m_roomBlue3.SendToAll(pkg, except);
			}
			if (m_roomBlue4 != null)
			{
				m_roomBlue4.SendToAll(pkg, except);
			}
			break;
		}
	}

	public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
	{
		Player player = base.RemovePlayer(gp, IsKick);
		if (player != null)
		{
			switch (player.Team)
			{
			case 1:
			case 3:
			case 5:
			case 7:
				m_roomRed1.RemovePlayer(gp);
				if (m_roomRed2 != null)
				{
					m_roomRed2.RemovePlayer(gp);
				}
				if (m_roomRed3 != null)
				{
					m_roomRed3.RemovePlayer(gp);
				}
				if (m_roomRed4 != null)
				{
					m_roomRed4.RemovePlayer(gp);
				}
				break;
			case 2:
			case 4:
			case 6:
			case 8:
				m_roomBlue1.RemovePlayer(gp);
				if (m_roomBlue2 != null)
				{
					m_roomBlue2.RemovePlayer(gp);
				}
				if (m_roomBlue3 != null)
				{
					m_roomBlue3.RemovePlayer(gp);
				}
				if (m_roomBlue4 != null)
				{
					m_roomBlue4.RemovePlayer(gp);
				}
				break;
			}
		}
		return player;
	}

	public override string ToString()
	{
		return new StringBuilder(base.ToString()).Append(",class=BattleGame").ToString();
	}
}

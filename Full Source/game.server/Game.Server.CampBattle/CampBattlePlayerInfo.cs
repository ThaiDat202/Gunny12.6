using System;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.CampBattle;

public class CampBattlePlayerInfo
{
	[NonSerialized]
	private GamePlayer m_player;

	private CampBattlePlayerState m_StateType;

	private int _Score;

	private int _WinCount;

	public int UserID { get; set; }

	public string NickName { get; set; }

	public int ZoneId { get; set; }

	public string ZoneName { get; set; }

	public GamePlayer Player
	{
		get
		{
			return m_player;
		}
		set
		{
			m_player = value;
		}
	}

	public int PosX { get; set; }

	public int PosY { get; set; }

	public int Team { get; set; }

	public CampBattlePlayerState StateType
	{
		get
		{
			return m_StateType;
		}
		set
		{
			m_StateType = value;
			GameMgr.CampBattle.SendUpdatePlayerStatus(this);
		}
	}

	public int TimerCount { get; set; }

	public int Score
	{
		get
		{
			return _Score;
		}
		set
		{
			_Score = value;
			GameMgr.CampBattle.SendUpdateScore(this);
		}
	}

	public int WinCount
	{
		get
		{
			return _WinCount;
		}
		set
		{
			_WinCount = value;
			GameMgr.CampBattle.SendUpdateScore(this);
		}
	}

	public int Rank { get; set; }

	public bool IsActive { get; set; }

	public int MapId { get; set; }

	public DateTime LastTimeLogout { get; set; }

	public CampBattlePlayerInfo()
	{
		m_StateType = CampBattlePlayerState.NORMAL;
		_Score = 0;
	}
}

using System;
using System.Drawing;
using Game.Server.GameObjects;

namespace Game.Server.GuildBattle;

[Serializable]
public class UserGuildBattleInfo
{
	[NonSerialized]
	private GamePlayer m_player;

	public int UserID { get; set; }

	public string NickName { get; set; }

	public int ConsortiaID { get; set; }

	public string ConsortiaName { get; set; }

	public Point Postion { get; set; }

	public UserGuildBattleStatus Status { get; set; }

	public DateTime TombStoneEndTime { get; set; }

	public bool IsDead { get; set; }

	public int VictoryCount { get; set; }

	public int WinStreak { get; set; }

	public int Score { get; set; }

	public int LostCount { get; set; }

	public int FailBuffCount { get; set; }

	public bool DupeScoreConsortiaBattle { get; set; }

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

	public bool IsActive { get; set; }

	public UserGuildBattleInfo(GamePlayer player)
	{
		UserID = player.PlayerId;
		NickName = player.PlayerCharacter.NickName;
		ConsortiaID = player.PlayerCharacter.ConsortiaID;
		ConsortiaName = player.PlayerCharacter.ConsortiaName;
		Postion = new Point(0, 0);
		Status = UserGuildBattleStatus.NORMAL;
		TombStoneEndTime = DateTime.Now.AddHours(-1.0);
		player.PlayerCharacter.ReduceStartBlood = player.PlayerCharacter.hp;
		IsActive = true;
		IsDead = false;
		DupeScoreConsortiaBattle = false;
		Player = player;
	}
}

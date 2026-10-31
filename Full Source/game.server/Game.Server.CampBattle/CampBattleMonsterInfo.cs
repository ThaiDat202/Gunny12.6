using Game.Logic;
using Game.Server.Games;

namespace Game.Server.CampBattle;

public class CampBattleMonsterInfo
{
	private CampBattlePlayerState _StateType;

	public int ID { get; set; }

	public string ActionMovieName { get; set; }

	public int MissionId { get; set; }

	public int Team { get; set; }

	public int PosX { get; set; }

	public int PosY { get; set; }

	public string Name { get; set; }

	public bool IsBoss { get; set; }

	public CampBattlePlayerState StateType
	{
		get
		{
			return _StateType;
		}
		set
		{
			_StateType = value;
			GameMgr.CampBattle.SendUpdateMonsterStatus(this);
		}
	}

	public BaseGame CurrentGame { get; set; }

	public CampBattleMonsterInfo()
	{
		_StateType = CampBattlePlayerState.NORMAL;
	}
}

using Game.Logic.Phy.Maps;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SpringTimerBomb : SimpleBomb
{
	public SpringTimerBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled, bool invoked, int angle)
		: base(id, type, owner, game, info, shape, controled, invoked, angle, isPlatom: false)
	{
	}
}

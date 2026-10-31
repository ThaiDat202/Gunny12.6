using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class PhantomBomb : SimpleBomb
{
	private int _phantomCount;

	private BombAction _currBombAction;

	public PhantomBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled, bool invoked, int angle)
		: base(id, type, owner, game, info, shape, controled, invoked, angle, isPlatom: false)
	{
		_phantomCount = 0;
		_currBombAction = null;
	}

	public override void StartMoving()
	{
		base.StartMoving();
		foreach (BombAction action in m_actions)
		{
			if (action.Type == 2 || action.Type == 4)
			{
				_currBombAction = action;
				break;
			}
		}
		if (_currBombAction != null)
		{
			_phantomCount = (int)((_currBombAction.Time - 2500f) / 500f);
		}
		if (base.LifeTime % 80f != 0f)
		{
		}
	}

	public override void MoveTo(int px, int py)
	{
		base.MoveTo(px, py);
	}
}

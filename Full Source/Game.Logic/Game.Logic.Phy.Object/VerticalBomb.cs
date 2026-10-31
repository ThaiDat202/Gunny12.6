using System.Drawing;
using Game.Logic.Phy.Maps;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class VerticalBomb : SimpleBomb
{
	private bool _isFall;

	public VerticalBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled, bool invoked, int angle)
		: base(id, type, owner, game, info, shape, controled, invoked, angle, isPlatom: false)
	{
		_isFall = false;
	}

	public override Point CompleteNextMovePoint(float dt)
	{
		float x = m_vy.x1;
		if (!_isFall)
		{
			m_vx.ComputeOneEulerStep(m_mass, m_arf, m_wf, dt);
		}
		m_vy.ComputeOneEulerStep(m_mass, m_arf, m_gf, dt);
		if (x * m_vy.x1 <= 0f)
		{
			_isFall = true;
		}
		return new Point((int)m_vx.x0, (int)m_vy.x0);
	}
}

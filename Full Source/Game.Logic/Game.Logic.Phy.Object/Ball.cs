using System.Drawing;

namespace Game.Logic.Phy.Object;

public class Ball : PhysicalObj
{
	private int _liveCount;

	public int LiveCount
	{
		get
		{
			return _liveCount;
		}
		set
		{
			_liveCount = value;
		}
	}

	public override int Type => 2;

	public Ball(int id, string name, string defaultAction, int scale, int rotation)
		: base(id, name, "asset.game.six.ball", defaultAction, scale, rotation, 0, canPenetrate: false)
	{
		m_rect = new Rectangle(-15, -15, 30, 30);
	}

	public override void CollidedByObject(Physics phy)
	{
		if (phy is SimpleBomb)
		{
			SimpleBomb simpleBomb = phy as SimpleBomb;
			simpleBomb.Owner.PickBall(this);
		}
	}
}

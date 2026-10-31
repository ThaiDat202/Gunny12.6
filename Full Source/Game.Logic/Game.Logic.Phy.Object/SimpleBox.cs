using System.Drawing;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SimpleBox : PhysicalObj
{
	private int _userID;

	private int _liveCount;

	private ItemInfo m_item;

	private int m_boxType;

	public int UserID
	{
		get
		{
			return _userID;
		}
		set
		{
			_userID = value;
		}
	}

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

	public ItemInfo Item => m_item;

	public override int Type => m_boxType;

	public SimpleBox(int id, string model, ItemInfo item, int type)
		: base(id, "", model, "", 1, 1, 0, canPenetrate: false)
	{
		_userID = 0;
		m_rect = new Rectangle(-15, -15, 30, 30);
		m_item = item;
		m_boxType = type;
	}

	public override void CollidedByObject(Physics phy)
	{
		if (phy is SimpleBomb)
		{
			SimpleBomb simpleBomb = phy as SimpleBomb;
			if (simpleBomb.Owner is Player)
			{
				simpleBomb.Owner.PickBox(this);
			}
		}
	}
}

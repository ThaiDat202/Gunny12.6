namespace SqlDataProvider.Data;

public class UserTrailEliteInfo : DataObject
{
	private int _id;

	private int _userID;

	private int _trailEliteScore;

	private int _trailEliteTotal;

	private int _trailEliteWin;

	private int _trailEliteRankTotal;

	private int _trailEliteRankWin;

	private bool _trailEliteIsUpRank;

	private int _trailEliteLevel;

	public int ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
			_isDirty = true;
		}
	}

	public int UserID
	{
		get
		{
			return _userID;
		}
		set
		{
			_userID = value;
			_isDirty = true;
		}
	}

	public int trailEliteScore
	{
		get
		{
			return _trailEliteScore;
		}
		set
		{
			_trailEliteScore = value;
			_isDirty = true;
		}
	}

	public int trailEliteTotal
	{
		get
		{
			return _trailEliteTotal;
		}
		set
		{
			_trailEliteTotal = value;
			_isDirty = true;
		}
	}

	public int trailEliteWin
	{
		get
		{
			return _trailEliteWin;
		}
		set
		{
			_trailEliteWin = value;
			_isDirty = true;
		}
	}

	public int trailEliteRankTotal
	{
		get
		{
			return _trailEliteRankTotal;
		}
		set
		{
			_trailEliteRankTotal = value;
			_isDirty = true;
		}
	}

	public int trailEliteRankWin
	{
		get
		{
			return _trailEliteRankWin;
		}
		set
		{
			_trailEliteRankWin = value;
			_isDirty = true;
		}
	}

	public bool trailEliteIsUpRank
	{
		get
		{
			return _trailEliteIsUpRank;
		}
		set
		{
			_trailEliteIsUpRank = value;
			_isDirty = true;
		}
	}

	public int trailEliteLevel
	{
		get
		{
			return _trailEliteLevel;
		}
		set
		{
			_trailEliteLevel = value;
			_isDirty = true;
		}
	}
}

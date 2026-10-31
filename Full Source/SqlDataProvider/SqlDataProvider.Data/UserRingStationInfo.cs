using System;

namespace SqlDataProvider.Data;

public class UserRingStationInfo : DataObject
{
	private PlayerInfo m_player;

	private int _iD;

	private int _userID;

	private int _rank;

	private int _weaponID;

	private string _signMsg;

	private int _buyCount;

	private int _total;

	private int _challengeNum;

	private DateTime _challengeTime;

	private bool _reardEnable;

	private DateTime _lastDate;

	private int _baseDamage;

	private int _baseGuard;

	private int _baseEnergy;

	private bool _onFight;

	public PlayerInfo Info
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

	public int ID
	{
		get
		{
			return _iD;
		}
		set
		{
			_iD = value;
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

	public int Rank
	{
		get
		{
			return _rank;
		}
		set
		{
			_rank = value;
			_isDirty = true;
		}
	}

	public int WeaponID
	{
		get
		{
			return _weaponID;
		}
		set
		{
			_weaponID = value;
			_isDirty = true;
		}
	}

	public string signMsg
	{
		get
		{
			return _signMsg;
		}
		set
		{
			_signMsg = value;
			_isDirty = true;
		}
	}

	public int buyCount
	{
		get
		{
			return _buyCount;
		}
		set
		{
			_buyCount = value;
			_isDirty = true;
		}
	}

	public int Total
	{
		get
		{
			return _total;
		}
		set
		{
			_total = value;
			_isDirty = true;
		}
	}

	public int ChallengeNum
	{
		get
		{
			return _challengeNum;
		}
		set
		{
			_challengeNum = value;
			_isDirty = true;
		}
	}

	public DateTime ChallengeTime
	{
		get
		{
			return _challengeTime;
		}
		set
		{
			_challengeTime = value;
			_isDirty = true;
		}
	}

	public bool ReardEnable
	{
		get
		{
			return _reardEnable;
		}
		set
		{
			_reardEnable = value;
			_isDirty = true;
		}
	}

	public DateTime LastDate
	{
		get
		{
			return _lastDate;
		}
		set
		{
			_lastDate = value;
			_isDirty = true;
		}
	}

	public int BaseDamage
	{
		get
		{
			return _baseDamage;
		}
		set
		{
			_baseDamage = value;
			_isDirty = true;
		}
	}

	public int BaseGuard
	{
		get
		{
			return _baseGuard;
		}
		set
		{
			_baseGuard = value;
			_isDirty = true;
		}
	}

	public int BaseEnergy
	{
		get
		{
			return _baseEnergy;
		}
		set
		{
			_baseEnergy = value;
			_isDirty = true;
		}
	}

	public bool OnFight
	{
		get
		{
			return _onFight;
		}
		set
		{
			_onFight = value;
		}
	}

	public bool CanChallenge()
	{
		int num = 600000 - (int)(DateTime.Now - _challengeTime).TotalMilliseconds;
		num = num / 10 / 60 / 1000;
		return num <= 0;
	}
}

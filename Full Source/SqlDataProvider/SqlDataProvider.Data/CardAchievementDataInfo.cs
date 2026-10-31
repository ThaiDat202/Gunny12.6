using System;

namespace SqlDataProvider.Data;

public class CardAchievementDataInfo : DataObject
{
	private int _userID;

	private int _achievementID;

	private int _condition;

	private DateTime _completedDate;

	private int _isComplete;

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

	public int AchievementID
	{
		get
		{
			return _achievementID;
		}
		set
		{
			_achievementID = value;
			_isDirty = true;
		}
	}

	public int Condition
	{
		get
		{
			return _condition;
		}
		set
		{
			_condition = value;
			_isDirty = true;
		}
	}

	public DateTime CompletedDate
	{
		get
		{
			return _completedDate;
		}
		set
		{
			_completedDate = value;
			_isDirty = true;
		}
	}

	public int IsComplete
	{
		get
		{
			return _isComplete;
		}
		set
		{
			_isComplete = value;
			_isDirty = true;
		}
	}
}

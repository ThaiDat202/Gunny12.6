using System;
using System.Collections.Generic;

namespace Game.Server.BuffHorseRace;

public class UserHorseRaceInfo
{
	private int _speed;

	public bool IsSendSpeed = false;

	private List<int> BuffIdLists = new List<int> { 1, 2, 3, 4, 5, 6 };

	private BuffHorseRaceList _BuffList;

	private Random rand;

	public int UserID { get; set; }

	public string NickName { get; set; }

	public int DefaultSpeed { get; set; }

	public int Speed
	{
		get
		{
			return _speed;
		}
		set
		{
			_speed = value;
			IsSendSpeed = false;
		}
	}

	public int BuffItem1 { get; set; }

	public int BuffItem2 { get; set; }

	public long LastTimerUpdate { get; set; }

	public int RaceLength { get; set; }

	public DateTime FinishTime { get; set; }

	public int Index { get; set; }

	public int Rank { get; set; }

	public DateTime LastTimeGetBuff1 { get; set; }

	public DateTime LastTimeGetBuff2 { get; set; }

	public BuffHorseRaceList BuffList => _BuffList;

	public UserHorseRaceInfo()
	{
	}

	public UserHorseRaceInfo(int userId, string nickName, int mountLevel, int index, int speedDefault)
	{
		UserID = userId;
		NickName = nickName;
		DefaultSpeed = speedDefault * mountLevel;
		Speed = speedDefault * mountLevel;
		LastTimerUpdate = GetTimer();
		RaceLength = 0;
		Index = index + 1;
		Rank = 0;
		rand = new Random();
		_BuffList = new BuffHorseRaceList(this);
	}

	public long GetTimer()
	{
		return DateTime.Now.Ticks / 10000;
	}

	public void ChangePos()
	{
		long timer = GetTimer();
		int num = (int)(timer - LastTimerUpdate);
		RaceLength += Speed * num / 1000;
		LastTimerUpdate = timer;
	}

	public void Reset()
	{
		LastTimeGetBuff1 = DateTime.Now;
		LastTimeGetBuff2 = DateTime.Now;
		LastTimerUpdate = GetTimer();
		BuffItem1 = 0;
		BuffItem2 = 0;
		CreateBuff();
	}

	public void CreateBuff()
	{
		BuffIdLists = new List<int> { 1, 2, 3, 5, 6, 7 };
		if (BuffItem1 == 0)
		{
			int index = rand.Next(0, BuffIdLists.Count - 1);
			BuffItem1 = BuffIdLists[index];
			BuffIdLists.Remove(BuffItem1);
		}
		if (BuffItem2 == 0)
		{
			int index2 = rand.Next(0, BuffIdLists.Count - 1);
			BuffItem2 = BuffIdLists[index2];
			BuffIdLists.Remove(BuffItem2);
		}
	}

	public bool RemoveBuff(int type)
	{
		bool result = false;
		if (BuffItem1 == type)
		{
			BuffItem1 = 0;
			result = true;
		}
		else if (BuffItem2 == type)
		{
			BuffItem2 = 0;
			result = true;
		}
		return result;
	}

	public bool CheckBuff()
	{
		BuffIdLists = new List<int> { 1, 2, 3, 5, 6, 7 };
		BuffIdLists.Remove(BuffItem1);
		BuffIdLists.Remove(BuffItem2);
		bool result = false;
		BuffList.Update();
		if (BuffItem1 == 0 && LastTimeGetBuff1 <= DateTime.Now)
		{
			int index = rand.Next(0, BuffIdLists.Count - 1);
			BuffItem1 = BuffIdLists[index];
			BuffIdLists.Remove(BuffItem1);
			LastTimeGetBuff1 = DateTime.Now.AddSeconds(15.0);
			result = true;
		}
		if (BuffItem2 == 0 && LastTimeGetBuff2 <= DateTime.Now)
		{
			int index2 = rand.Next(0, BuffIdLists.Count - 1);
			BuffItem2 = BuffIdLists[index2];
			BuffIdLists.Remove(BuffItem2);
			LastTimeGetBuff2 = DateTime.Now.AddSeconds(15.0);
			result = true;
		}
		return result;
	}

	public void Stop()
	{
		BuffList.StopAll();
	}
}

using System;

namespace SqlDataProvider.Data;

public class RingstationConfigInfo
{
	public int ID { get; set; }

	public int buyCount { get; set; }

	public int buyPrice { get; set; }

	public int cdPrice { get; set; }

	public DateTime AwardTime { get; set; }

	public int AwardNum { get; set; }

	public string AwardFightWin { get; set; }

	public string AwardFightLost { get; set; }

	public string ChampionText { get; set; }

	public int ChallengeNum { get; set; }

	public bool IsFirstUpdateRank { get; set; }

	public bool IsEndTime()
	{
		return AwardTime.Date < DateTime.Now.Date;
	}

	public int AwardNumByRank(int rank)
	{
		if (rank == 0)
		{
			return 0;
		}
		if (rank < 30 && rank > 0)
		{
			return AwardNum - 10 * rank;
		}
		return AwardNum - 350;
	}

	public int AwardBattleByRank(int rank, bool isWin)
	{
		if (rank == 0 && isWin)
		{
			return 10;
		}
		if (rank == 0 && !isWin)
		{
			return 5;
		}
		if (string.IsNullOrEmpty(AwardFightLost))
		{
			return 0;
		}
		if (string.IsNullOrEmpty(AwardFightWin))
		{
			return 0;
		}
		string[] array = AwardFightLost.Split('|');
		if (isWin)
		{
			array = AwardFightWin.Split('|');
		}
		if (array.Length < 3)
		{
			return 0;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(',');
			if (array3.Length < 2)
			{
				return 0;
			}
			int num = int.Parse(array3[0].Split('-')[0]);
			int num2 = int.Parse(array3[0].Split('-')[1]);
			if (rank >= num && rank <= num2)
			{
				return int.Parse(array3[1]);
			}
		}
		return 0;
	}
}

using System.ComponentModel.DataAnnotations;

namespace EntityDatabase.ServerModels;

public class TS_Butterfly
{
	[Key]
	public int Grades { get; set; }

	public int Gp { get; set; }

	public int FeelingMax { get; set; }

	public int DayFeeling { get; set; }

	public int DayFondMax { get; set; }

	public int GradeRewardItemId { get; set; }
}

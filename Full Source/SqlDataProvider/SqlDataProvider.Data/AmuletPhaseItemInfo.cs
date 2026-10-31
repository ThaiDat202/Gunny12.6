using System;

namespace SqlDataProvider.Data;

public class AmuletPhaseItemInfo
{
	public int AmuletLevel { get; set; }

	public int AvoidInjury { get; set; }

	public int Crit { get; set; }

	public int Expend { get; set; }

	public int Guard { get; set; }

	public int Kill { get; set; }

	public int LockPrice { get; set; }

	public int Phase { get; set; }

	public int Speed { get; set; }

	public int SunderArmor { get; set; }

	public int Tenacity { get; set; }

	public int ViolenceInjury { get; set; }

	public int WillFight { get; set; }

	public double GetProperty(int type, int value)
	{
		double num = 1.0;
		switch (type)
		{
		case 1:
			num = Crit;
			break;
		case 2:
			num = Tenacity;
			break;
		case 3:
			num = SunderArmor;
			break;
		case 4:
			num = AvoidInjury;
			break;
		case 5:
			num = Kill;
			break;
		case 6:
			num = WillFight;
			break;
		case 7:
			num = ViolenceInjury;
			break;
		case 8:
			num = Guard;
			break;
		case 9:
			num = Speed;
			break;
		}
		return Math.Round((double)value / 10000.0 * num);
	}
}

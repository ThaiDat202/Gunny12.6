namespace SqlDataProvider.Data;

public class PveInfo
{
	public int ID { get; set; }

	public string Name { get; set; }

	public string SimpleTemplateIds { get; set; }

	public string NormalTemplateIds { get; set; }

	public string HardTemplateIds { get; set; }

	public string TerrorTemplateIds { get; set; }

	public string EpicTemplateIds { get; set; }

	public string NightmareTemplateIds { get; set; }

	public int Type { get; set; }

	public int LevelLimits { get; set; }

	public string Pic { get; set; }

	public string Description { get; set; }

	public int Ordering { get; set; }

	public string AdviceTips { get; set; }

	public string SimpleGameScript { get; set; }

	public string NormalGameScript { get; set; }

	public string HardGameScript { get; set; }

	public string TerrorGameScript { get; set; }

	public string EpicGameScript { get; set; }

	public string NightmareGameScript { get; set; }

	public string BossFightNeedMoney { get; set; }

	public int MinLv { get; set; }

	public int MaxLv { get; set; }

	public string FightPower { get; set; }

	public string Missions { get; set; }

	public int GetPrice(int selectedLevel)
	{
		int result = 1;
		string bossFightNeedMoney = BossFightNeedMoney;
		string[] array = bossFightNeedMoney.Split('|');
		if (array.Length != 0)
		{
			switch (selectedLevel)
			{
			case 0:
				result = int.Parse(array[0]);
				break;
			case 1:
				result = int.Parse(array[1]);
				break;
			case 2:
				result = int.Parse(array[2]);
				break;
			case 3:
				result = int.Parse(array[3]);
				break;
			case 4:
				result = int.Parse(array[4]);
				break;
			}
		}
		return result;
	}
}

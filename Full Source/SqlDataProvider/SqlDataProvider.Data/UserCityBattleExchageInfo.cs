namespace SqlDataProvider.Data;

public class UserCityBattleExchageInfo
{
	public int ID { get; set; }

	public int Count { get; set; }

	public UserCityBattleExchageInfo Clone()
	{
		UserCityBattleExchageInfo userCityBattleExchageInfo = new UserCityBattleExchageInfo();
		userCityBattleExchageInfo.ID = ID;
		userCityBattleExchageInfo.Count = Count;
		return userCityBattleExchageInfo;
	}
}

namespace SqlDataProvider.Data;

public class CityBattleUserDataInfo : DataObject
{
	private string _ID;

	private string _UserData;

	private string _UserExchangeData;

	private string _GameData;

	public string ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
			_isDirty = true;
		}
	}

	public string UserData
	{
		get
		{
			return _UserData;
		}
		set
		{
			_UserData = value;
			_isDirty = true;
		}
	}

	public string UserExchangeData
	{
		get
		{
			return _UserExchangeData;
		}
		set
		{
			_UserExchangeData = value;
			_isDirty = true;
		}
	}

	public string GameData
	{
		get
		{
			return _GameData;
		}
		set
		{
			_GameData = value;
			_isDirty = true;
		}
	}
}

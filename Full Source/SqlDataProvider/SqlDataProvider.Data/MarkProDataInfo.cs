namespace SqlDataProvider.Data;

public class MarkProDataInfo
{
	public int type { get; set; }

	public int value { get; set; }

	public int attachValue { get; set; }

	public int hummerCount { get; set; }

	public MarkProDataInfo()
	{
		type = 0;
		value = 0;
		attachValue = 0;
		hummerCount = 0;
	}

	public MarkProDataInfo(int _type, int _value)
	{
		type = _type;
		value = _value;
	}
}

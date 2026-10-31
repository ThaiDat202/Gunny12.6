using System.Collections.Generic;

public class ObjectFactory
{
	public static Dictionary<string, object> CreateInstance()
	{
		return new Dictionary<string, object>();
	}

	public static Dictionary<string, object> CreateInstance(string name)
	{
		return new Dictionary<string, object>();
	}
}

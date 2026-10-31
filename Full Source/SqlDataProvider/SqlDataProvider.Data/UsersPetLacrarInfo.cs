using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class UsersPetLacrarInfo
{
	public int petId;

	public int place;

	public bool isSeal;

	public List<StoreFineSealLockInfo> lockArr;

	public string GetProStr()
	{
		List<StoreFineSealLockInfo> list = lockArr;
		if (list != null && list.Count == 0)
		{
			return "";
		}
		string text = "";
		foreach (StoreFineSealLockInfo item in lockArr)
		{
			text = ((item.skillId != 0) ? (text + item.proType + "," + item.skillId + "|") : (text + item.proType + "," + item.proNum + "|"));
		}
		return text;
	}
}

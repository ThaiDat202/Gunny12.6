using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class JampsChapterItemList
{
	public int ID;

	public int Sort;

	public string Name;

	public string Describe;

	public string Reserved;

	public Dictionary<int, JampsPageItemList> paginas;
}

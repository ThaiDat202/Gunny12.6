using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class EngraveSetElementInfo
{
	public int Id { get; set; }

	public string Name { get; set; }

	public int SetId { get; set; }

	public int SkillId { get; set; }

	public string Explain { get; set; }

	public int Demand { get; set; }

	public string Attribute { get; set; }

	public List<MarkProDataInfo> AttributeList
	{
		get
		{
			List<MarkProDataInfo> list = new List<MarkProDataInfo>();
			if (Attribute != "0")
			{
				string[] array = Attribute.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split('|');
					MarkProDataInfo markProDataInfo = new MarkProDataInfo();
					markProDataInfo.type = int.Parse(array3[0]);
					markProDataInfo.value = int.Parse(array3[1]);
					markProDataInfo.attachValue = int.Parse(array3[2]);
					list.Add(markProDataInfo);
				}
			}
			return list;
		}
	}
}

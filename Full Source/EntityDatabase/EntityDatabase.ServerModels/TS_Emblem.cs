using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_Emblem
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int TemplateID { get; set; }

	public string TemplateName { get; set; }

	public int Types { get; set; }

	public int Profile { get; set; }

	public int MainType { get; set; }

	public string MainValue { get; set; }

	public int SubCount { get; set; }

	public string SubValue { get; set; }

	public string SkillValue { get; set; }

	public int SaleValue { get; set; }

	public int NeedItem1 { get; set; }

	public int ItemCount1 { get; set; }

	public int NeedItem2 { get; set; }

	public int ItemCount2 { get; set; }

	public int NeedItem3 { get; set; }

	public int ItemCount3 { get; set; }

	public int NeedItem4 { get; set; }

	public int ItemCount4 { get; set; }
}

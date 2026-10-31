using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_UpgradeTemplate
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int Types { get; set; }

	public int TemplateId { get; set; }

	public int Grades { get; set; }

	public int Data { get; set; }

	public string TemplateName { get; set; }

	public string ActiveObj { get; set; }

	public int ItemTempId1 { get; set; }

	public int Param1 { get; set; }

	public int ItemTempId2 { get; set; }

	public int Param2 { get; set; }

	public int ItemTempId3 { get; set; }

	public int Param3 { get; set; }

	public int ItemTempId4 { get; set; }

	public int Param4 { get; set; }
}

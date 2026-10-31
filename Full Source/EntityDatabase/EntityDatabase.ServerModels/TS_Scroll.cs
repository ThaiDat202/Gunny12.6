using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_Scroll
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int ID { get; set; }

	public int TemplateID { get; set; }

	public string TemplateName { get; set; }

	public int Types { get; set; }

	public int Profile { get; set; }
}

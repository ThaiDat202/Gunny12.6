using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class TS_CardMain
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int TemplateId { get; set; }

	public int Level { get; set; }

	public int Attack { get; set; }

	public int Agility { get; set; }

	public int Defence { get; set; }

	public int Lucky { get; set; }

	public int Point { get; set; }

	public int NeedItem1Count { get; set; }

	public int NeedItem2Count { get; set; }
}

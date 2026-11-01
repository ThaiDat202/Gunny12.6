using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class Elf_Template_Info
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int TemplateID { get; set; }

	public string TemplateName { get; set; }

	public int ElfType { get; set; }

	public int StarLevel { get; set; }

	public string HPRandoms { get; set; }

	public string AtckRandoms { get; set; }

	public string FireRandoms { get; set; }

	public string WindRandoms { get; set; }

	public string WateRandoms { get; set; }

	public string LandRandoms { get; set; }

	public string Growths { get; set; }

	public string HPGrowths { get; set; }

	public string AtckGrowths { get; set; }

	public string FireGrowths { get; set; }

	public string WindGrowths { get; set; }

	public string WateGrowths { get; set; }

	public string LandGrowths { get; set; }

	public string MaxSkillCount { get; set; }

	public int Randoms { get; set; }

	public int Recycle { get; set; }

	public string Description { get; set; }

    public string LighRandoms { get; set; }

    public string DrakRandoms { get; set; }

    public string LighGrowths { get; set; }

    public string DrakGrowths { get; set; }
}

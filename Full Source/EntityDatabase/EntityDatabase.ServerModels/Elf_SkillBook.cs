using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.ServerModels;

public class Elf_SkillBook
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int TemplateId { get; set; }

	public string TemplateName { get; set; }

	public int ElfType { get; set; }

	public int NeedBookCount { get; set; }

	public int Randoms { get; set; }

	public int SkillTempId { get; set; }

	public string Description { get; set; }
}

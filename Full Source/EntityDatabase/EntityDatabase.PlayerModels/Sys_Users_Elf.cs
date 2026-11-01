using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_Elf
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int ElfID { get; set; }

	public int TempID { get; set; }

	public int Exp { get; set; }

	public int Grade { get; set; }

	public int HpBase { get; set; }

	public int AttackBase { get; set; }

	public int HuoBase { get; set; }

	public int ShuiBase { get; set; }

	public int TuBase { get; set; }

	public int FengBase { get; set; }

    public int GuangBase { get; set; }

    public int AnBase { get; set; }

    public int HpValue { get; set; }

	public int AttackValue { get; set; }

	public int HuoValue { get; set; }

	public int ShuiValue { get; set; }

	public int TuValue { get; set; }

	public int FengValue { get; set; }

    public int GuangValue { get; set; }

    public int AnValue { get; set; }

    public int HpGrowth { get; set; }

	public int AttackGrowth { get; set; }

	public int HuoGrowth { get; set; }

	public int ShuiGrowth { get; set; }

	public int TuGrowth { get; set; }

	public int FengGrowth { get; set; }

    public int GuangGrowth { get; set; }

    public int AnGrowth { get; set; }

    public int GoodFeling { get; set; }

	public bool IsFight { get; set; }

	public int Quality { get; set; }

	public string SkillList { get; set; }

	public int DungeonGoodFeling { get; set; }

	public int GameGoodFeling { get; set; }

	public int CultivateGoodFeling { get; set; }

	public int ElfEquipId { get; set; }
	public int EquipTempId { get; set; }
}
